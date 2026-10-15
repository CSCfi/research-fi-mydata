#!/usr/bin/env python3
"""Generate seed_test_data.sql and seed_plan.md for SQL Server from a DDL file.

Usage: generate_seed.py <ddl_path> <output_dir> [--verification-log PATH]

The DDL is only read; nothing from it is copied into the outputs except table and
column names.
"""
import argparse
import datetime
import heapq
import itertools
import os
import random
import re
import sys
from collections import defaultdict

from ddl_parser import parse
import seed_rows as S

PREFERRED_DEFER = {('dim_organization', 'dim_registered_data_source_id')}
NULLABLE_FK_MINUS_ONE = {'dim_pid'}  # UserProfileService.GetEmptyDimPid() sets nullable FKs to -1
DEFAULT_ROWS = 10

FIXED_ROWS = {
    'dim_organization': [
        dict(id=1, name_en='ORCID', name_fi='ORCID', name_sv='ORCID', dim_sectorid=-1, dim_registered_data_source_id=-1, source_id=' '),
        dict(id=2, name_en='Research.fi', name_fi='Tiedejatutkimus.fi', name_sv='Forskning.fi', dim_sectorid=-1,
             dim_registered_data_source_id=-1, source_id=' '),
    ],
    'dim_registered_data_source': [
        dict(id=1, name='ORCID', dim_organization_id=1, source_id=' '),
        dict(id=2, name='Tiedejatutkimus.fi', dim_organization_id=2, source_id=' '),
    ],
}

# Generic sparse fact tables: anchor columns are always real, one column per group is real, the rest -1.
SPARSE = {
    'fact_concept_context': (['dim_concept_id'], [['dim_publication_id', 'dim_research_dataset_id', 'dim_funding_decision_id',
                                                   'dim_research_activity_id', 'dim_profile_only_publication_id']]),
    'fact_dim_referencedata_field_of_science': (['dim_referencedata_id'], [['dim_known_person_id', 'dim_publication_id',
                                                                            'dim_research_dataset_id', 'dim_research_activity_id',
                                                                            'dim_funding_decision_id']]),
    'fact_keywords': (['dim_keyword_id'], [['dim_research_dataset_id', 'dim_research_project_id']]),
    'fact_referencedata': (['dim_referencedata_id'], [['dim_publication_ id', 'dim_research_dataset_id', 'dim_research_activity_id',
                                                       'dim_funding_decision_id', 'dim_infrastructure_id', 'dim_call_programme_id',
                                                       'dim_service_id']]),
    'fact_relation': (['relation_type_code', 'dim_registered_data_source_id'],
                      [['from_publication_id', 'from_research_dataset_id', 'from_infrastructure_id'],
                       ['to_research_dataset_id', 'to_publication_id', 'to_infrastructure_id']]),
    'fact_word_cluster_to_domain': (['dim_word_cluster_id'], [['dim_publication_id', 'dim_dataset_id', 'dim_funding_decision_id',
                                                               'dim_infrastructure_id']]),
}

WORDS = ['climate', 'genome', 'neural network', 'catalysis', 'archaeology', 'sustainability', 'quantum', 'linguistics',
         'epidemiology', 'robotics']
THEMES = {
    'dim_address': dict(cols={'street': lambda i: f'Yliopistonkatu {i}', 'postalCode': '00100', 'locality_fi': 'Helsinki',
                              'locality_sv': 'Helsingfors', 'locality_en': 'Helsinki', 'address_type': 'visiting',
                              'premise': None, 'postOfficeBox': None}),
    'dim_call_decisions': dict(cols={'call_processing_phase': lambda i: ['submitted', 'in review', 'decided'][i % 3]}),
    'dim_competence': dict(en='Competence', fi='Osaaminen', sv='Kompetens'),
    'dim_concept': dict(en='Concept', fi='Käsite', sv='Begrepp',
                        cols={'scheme': 'YSO', 'scheme_uri': 'http://www.yso.fi/onto/yso/',
                              'concept_uri': lambda i: f'http://www.yso.fi/onto/yso/p{1000 + i}'}),
    'dim_contact_information': dict(cols={'contact_label': lambda i: f'Service desk {i}'}),
    'dim_identifierless_organization': dict(en='Unlinked organization', fi='Tunnistamaton organisaatio', sv='Okänd organisation'),
    'dim_infrastructure': dict(acronym='INFRA', cols={'finland_roadmap': lambda i: i % 3 == 0}),
    'dim_international_infra': dict(en='International research infrastructure'),
    'dim_locally_reported_pub_info': dict(cols={'self_archived_type': 'accepted manuscript',
                                                'self_archived_url': lambda i: f'https://example.org/repository/{i}'}),
    'dim_mined_words': dict(cols={'word': lambda i: WORDS[(i - 1) % len(WORDS)]}),
    'dim_news_feed': dict(cols={'title': lambda i: f'Research news feed {i}'}),
    'dim_news_item': dict(cols={'news_headline': lambda i: f'News headline {i}', 'news_content': lambda i: f'News content {i}.'}),
    'dim_organisation_media': dict(cols={'language_variant': 'fi'}),
    'dim_research_community': dict(en='Research community', fi='Tutkimusyhteisö', sv='Forskningsgemenskap', acronym='RC'),
    'dim_researcher_to_research_community': dict(en='Member of research community'),
    'dim_word_cluster': dict(cols={'content': lambda i: f'{WORDS[(i - 1) % len(WORDS)]} cluster'}),
}

STRING = {'nvarchar', 'varchar', 'nchar', 'char'}
NUMERIC = {'int', 'bigint', 'smallint', 'tinyint', 'decimal', 'numeric', 'float'}
DATES = {'datetime', 'smalldatetime', 'date', 'datetime2'}


# ---------------------------------------------------------------- helpers
def q(name):
    return '[' + name.replace(']', ']]') + ']'


def base_type(c):
    return re.match(r'[a-z0-9]+', c.type).group(0)


def max_len(c):
    m = re.match(r'^n?(?:var)?char\((\d+|max)\)$', c.type)
    return int(m.group(1)) if m and m.group(1) != 'max' else None


def id_like(name):
    return name.lower().endswith('id')


def sql_str(s):
    s = s.replace("'", "''")
    parts, buf = [], []
    for ch in s:
        if ord(ch) < 128:
            buf.append(ch)
        else:
            if buf:
                parts.append("N'" + ''.join(buf) + "'")
                buf = []
            parts.append(f'NCHAR({ord(ch)})')
    if buf or not parts:
        parts.append("N'" + ''.join(buf) + "'")
    return ' + '.join(parts)


def lit(v):
    if v is None:
        return 'NULL'
    if isinstance(v, bool):
        return '1' if v else '0'
    if isinstance(v, (int, float)):
        return str(v)
    if isinstance(v, str):
        return sql_str(v)
    raise TypeError(f'Unsupported literal {v!r}')


def fkmap(t):
    return {fk.columns[0]: fk for fk in t.fks}


def humanize(name):
    for p in ('dim_', 'fact_', 'br_'):
        if name.startswith(p):
            name = name[len(p):]
    return name.replace('_', ' ').strip().capitalize()


def slug(name):
    return re.sub(r'[^a-z0-9]+', '-', name.lower().replace('dim_', '')).strip('-')


# ---------------------------------------------------------------- graph
def tarjan(nodes, adj):
    index, low, stack, on, out, counter = {}, {}, [], set(), [], [0]

    def visit(v):
        index[v] = low[v] = counter[0]
        counter[0] += 1
        stack.append(v)
        on.add(v)
        for w in sorted(adj[v]):
            if w not in index:
                visit(w)
                low[v] = min(low[v], low[w])
            elif w in on:
                low[v] = min(low[v], index[w])
        if low[v] == index[v]:
            comp = []
            while True:
                w = stack.pop()
                on.discard(w)
                comp.append(w)
                if w == v:
                    break
            out.append(comp)

    sys.setrecursionlimit(10000)
    for n in sorted(nodes):
        if n not in index:
            visit(n)
    return out


def find_cycle(scc, adj):
    scc = set(scc)
    start = sorted(scc)[0]
    path, seen = [start], {start: 0}

    def dfs(v):
        for w in sorted(adj[v]):
            if w not in scc:
                continue
            if w in seen:
                return path[seen[w]:] + [w]
            seen[w] = len(path)
            path.append(w)
            r = dfs(w)
            if r:
                return r
            path.pop()
            del seen[w]
        return None

    cyc = dfs(start)
    return list(zip(cyc, cyc[1:]))


def insert_order(tables):
    edges = defaultdict(list)
    for t in tables.values():
        for fk in t.fks:
            if fk.ref_table != t.name:
                edges[(t.name, fk.ref_table)].append(fk)
    adj = defaultdict(set)
    for c, p in edges:
        adj[c].add(p)
    deferred, cycles = [], []
    while True:
        sccs = [s for s in tarjan(tables.keys(), adj) if len(s) > 1]
        if not sccs:
            break
        for scc in sccs:
            cyc = find_cycle(scc, adj)
            cycles.append([c for c, _ in cyc])

            def score(e):
                fks = edges[e]
                preferred = any((fk.table, fk.columns[0]) in PREFERRED_DEFER for fk in fks)
                nullable = all(tables[fk.table].col(fk.columns[0]).nullable for fk in fks)
                return (not preferred, not nullable, e)

            e = min(cyc, key=score)
            adj[e[0]].discard(e[1])
            deferred.extend(edges[e])
    children = defaultdict(set)
    for c in adj:
        for p in adj[c]:
            children[p].add(c)
    remaining = {n: set(adj[n]) for n in tables}
    ready = [n for n in tables if not remaining[n]]
    heapq.heapify(ready)
    order = []
    while ready:
        n = heapq.heappop(ready)
        order.append(n)
        for c in sorted(children[n]):
            remaining[c].discard(n)
            if not remaining[c]:
                heapq.heappush(ready, c)
    if len(order) != len(tables):
        raise RuntimeError('Topological sort failed')
    self_refs = [fk for t in tables.values() for fk in t.fks if fk.ref_table == t.name]
    return order, deferred, cycles, self_refs


# ---------------------------------------------------------------- values
def sentinel_default(c):
    if c.nullable:
        return None
    bt = base_type(c)
    if bt in STRING:
        return ' '
    if bt == 'bit':
        return False
    if bt in DATES:
        return '1900-01-01T00:00:00'
    if bt in NUMERIC:
        return -1 if (c.default == '-1' or id_like(c.name)) else 0
    raise ValueError(f'No sentinel default for type {c.type}')


def generic_value(t, c, i):
    theme = THEMES.get(t.name, {})
    n, bt = c.name.lower(), base_type(c)
    if c.name in theme.get('cols', {}):
        v = theme['cols'][c.name]
        return v(i) if callable(v) else v
    if bt in DATES:
        d = datetime.datetime(2025, 1, 1, 9, 0, 0) + datetime.timedelta(days=i)
        if n == 'modified':
            d += datetime.timedelta(days=30)
        return d.strftime('%Y-%m-%dT%H:%M:%S')
    if bt == 'bit':
        return i % 2 == 1
    if bt in NUMERIC:
        if 'year' in n:
            return 2014 + i
        if 'month' in n:
            return (i % 12) + 1
        if n == 'day' or n.endswith('_day'):
            return (i % 28) + 1
        if bt in ('decimal', 'numeric', 'float'):
            return 10000 * i
        return i
    en = theme.get('en', humanize(t.name))
    fi, sv = theme.get('fi', en), theme.get('sv', en)
    if n == 'source_id':
        v = theme.get('source_id', 'Tiedejatutkimus.fi')
    elif n in ('source_description', 'source_desciption', 'soure_description'):
        v = 'Seed test data'
    elif any(k in n for k in ('url', 'uri', 'weblink', 'link')):
        v = f'https://example.org/{slug(t.name)}/{i}'
    elif n.startswith('description') or n.startswith('definition'):
        lang = n.rsplit('_', 1)[-1]
        v = {'fi': f'{fi} {i}: kuvaus', 'sv': f'{sv} {i}: beskrivning'}.get(lang, f'{en} {i}: description')
    elif n.endswith('_fi'):
        v = f'{fi} {i}'
    elif n.endswith('_sv'):
        v = f'{sv} {i}'
    elif n.endswith('_en') or n.endswith('_und'):
        v = f'{en} {i}'
    elif n == 'acronym':
        v = f"{theme.get('acronym', 'TST')}{i}"
    elif 'identifier' in n or id_like(n):
        v = f'{slug(t.name).upper()}-{i:04d}'
    else:
        v = f'{en} {i}'
    ml = max_len(c)
    return v[:ml] if ml else v


class Ctx:
    def __init__(self, tables, order, explicit):
        self.tables, self.order, self.explicit = tables, order, explicit
        self.pools = {}

    def pool_for(self, t, col):
        fk = fkmap(t)[col]
        if (t.name, col) in S.REF_POOLS:
            return S.REF_POOLS[(t.name, col)]
        p = fk.ref_table
        if p == 'dim_registered_data_source':
            return [S.TTV_DS]
        if p == 'dim_referencedata' or p == t.name:
            return []
        return self.pools.get(p, [])

    def generic_fk(self, t, c, i):
        pool = self.pool_for(t, c.name)
        if not pool:
            return None if c.nullable else -1
        return pool[(i - 1) % len(pool)]


def complete(ctx, t, row, i, explicit):
    fks = fkmap(t)
    for c in t.columns:
        if c.name in row:
            continue
        if c.name in fks:
            if explicit:
                row[c.name] = -1 if (not c.nullable or t.name in NULLABLE_FK_MINUS_ONE) else None
            else:
                row[c.name] = ctx.generic_fk(t, c, i)
        elif explicit:
            if c.name in ('created', 'modified'):
                row[c.name] = generic_value(t, c, i)
            elif not c.nullable:
                row[c.name] = -1 if (base_type(c) in NUMERIC and id_like(c.name)) else generic_value(t, c, i)
            else:
                row[c.name] = None
        else:
            row[c.name] = generic_value(t, c, i)
    unknown = set(row) - {c.name for c in t.columns}
    if unknown:
        raise KeyError(f'{t.name}: unknown columns {sorted(unknown)}')
    return row


def generic_rows(ctx, t):
    fks = fkmap(t)
    if t.name in SPARSE:
        anchors, groups = SPARSE[t.name]
        rows, seen = [], set()
        for i in range(1, DEFAULT_ROWS * 4):
            if len(rows) >= DEFAULT_ROWS:
                break
            row = {}
            for a in anchors:
                pool = ctx.pool_for(t, a)
                row[a] = pool[(i - 1) % len(pool)]
            for g, group in enumerate(groups):
                avail = [c for c in group if ctx.pool_for(t, c)]
                col = avail[(i - 1 + g) % len(avail)]
                pool = ctx.pool_for(t, col)
                row[col] = pool[((i - 1) // len(avail) + g) % len(pool)]
            for c in t.columns:
                if c.name in fks and c.name not in row:
                    row[c.name] = None if c.nullable else -1
            key = tuple(row[c] for c in t.pk)
            if key in seen:
                continue
            seen.add(key)
            rows.append(complete(ctx, t, row, len(rows) + 1, False))
        return rows
    if t.pk and all(c in fks for c in t.pk):
        pools = [ctx.pool_for(t, c)[:5] or [-1] for c in t.pk]
        combos = []
        for cmb in itertools.product(*pools):
            by_parent = defaultdict(list)
            for col, v in zip(t.pk, cmb):
                by_parent[fks[col].ref_table].append(v)
            if all(len(set(vs)) == len(vs) for vs in by_parent.values()):
                combos.append(cmb)
        random.Random(t.name).shuffle(combos)
        rows = []
        for i, cmb in enumerate(sorted(combos[:DEFAULT_ROWS]), start=1):
            rows.append(complete(ctx, t, dict(zip(t.pk, cmb)), i, False))
        return rows
    rows = []
    for i in range(1, DEFAULT_ROWS + 1):
        row = {'id': i} if t.col('id') else {}
        rows.append(complete(ctx, t, row, i, False))
    return rows


# ---------------------------------------------------------------- build
def pk_of(t):
    if t.pk:
        return t.pk
    return [c.name for c in t.columns if not c.nullable and c.name not in ('source_id',)] + \
        [c.name for c in t.columns if c.nullable and base_type(c) in STRING and max_len(c)][:1]


def where(t, row):
    parts = []
    for c in pk_of(t):
        v = row[c]
        parts.append(f'{q(c)} IS NULL' if v is None else f'{q(c)} = {lit(v)}')
    return ' AND '.join(parts)


def check_row(t, row, label):
    for c in t.columns:
        v = row.get(c.name)
        if v is None and not c.nullable:
            raise ValueError(f'{label} {t.name}.{c.name} is NOT NULL but got NULL: {row}')
        ml = max_len(c)
        if isinstance(v, str) and ml and len(v) > ml:
            raise ValueError(f'{label} {t.name}.{c.name} too long ({len(v)} > {ml})')


def build(ddl_path):
    tables, unparsed = parse(ddl_path)
    if unparsed:
        raise RuntimeError(f'Unparsed statements: {unparsed[:3]}')
    order, deferred, cycles, self_refs = insert_order(tables)
    deferred_cols = {(fk.table, fk.columns[0]) for fk in deferred}
    sentinel_tables = sorted({fk.ref_table for t in tables.values() for fk in t.fks})
    for tn in sentinel_tables:
        if tables[tn].pk != ['id']:
            raise RuntimeError(f'Sentinel target {tn} has PK {tables[tn].pk}')

    explicit = S.build()
    for tn in explicit:
        if tn not in tables:
            raise KeyError(f'Explicit rows for unknown table {tn}')
    ctx = Ctx(tables, order, explicit)

    sentinels = {}
    for tn in sentinel_tables:
        t = tables[tn]
        row = {}
        for c in t.columns:
            if c.name == 'id':
                row['id'] = -1
            elif c.name in fkmap(t):
                row[c.name] = None if ((tn, c.name) in deferred_cols and c.nullable) else -1
            else:
                row[c.name] = sentinel_default(c)
        check_row(t, row, 'sentinel')
        sentinels[tn] = row

    fixed, fixed_extra = {}, defaultdict(list)
    for tn, rows in FIXED_ROWS.items():
        t = tables[tn]
        fixed[tn] = []
        for r in rows:
            r = dict(r)
            for c in t.columns:
                if c.name not in r and not c.nullable:
                    r[c.name] = sentinel_default(c)
                    if c.name not in fixed_extra[tn]:
                        fixed_extra[tn].append(c.name)
            check_row(t, r, 'fixed')
            fixed[tn].append(r)

    rows_by_table = {}
    for tn in order:
        t = tables[tn]
        if tn in explicit:
            rows = [complete(ctx, t, dict(r), i, True) for i, r in enumerate(explicit[tn], start=1)]
        else:
            rows = generic_rows(ctx, t)
        rows_by_table[tn] = rows
        if t.col('id') and t.pk == ['id']:
            ctx.pools[tn] = [r['id'] for r in rows]

    # Simulate the insert sequence: every non-NULL FK must point to a row inserted earlier.
    existing = defaultdict(set)
    for tn in order:
        if tn in sentinels:
            existing[tn].add(-1)
    for tn, rows in fixed.items():
        existing[tn].update(r['id'] for r in rows)
    post_updates = []
    for tn in order:
        t = tables[tn]
        keys = set()
        for row in rows_by_table[tn]:
            check_row(t, row, 'test')
            for fk in t.fks:
                col = fk.columns[0]
                v = row.get(col)
                if v is None or (fk.ref_table == tn and v == row.get('id')) or v in existing[fk.ref_table]:
                    continue
                if (tn, col) in deferred_cols and col not in pk_of(t):
                    post_updates.append((tn, dict(row), col, v))
                    row[col] = None if t.col(col).nullable else -1
                    continue
                raise ValueError(f'{tn}.{col}={v} has no earlier parent row in {fk.ref_table}')
            key = tuple(row[c] for c in pk_of(t))
            if key in keys:
                raise ValueError(f'Duplicate key in {tn}: {key}')
            keys.add(key)
            if 'id' in row:
                existing[tn].add(row['id'])
            if tn in fixed and row.get('id') in {r['id'] for r in fixed[tn]}:
                raise ValueError(f'{tn} test row reuses fixed id {row["id"]}')
            if row.get('id') == -1:
                raise ValueError(f'{tn} test row uses sentinel id')
    return dict(tables=tables, order=order, deferred=deferred, cycles=cycles, self_refs=self_refs,
                sentinel_tables=sentinel_tables, sentinels=sentinels, fixed=fixed, fixed_extra=fixed_extra,
                rows=rows_by_table, post_updates=post_updates, explicit=set(explicit))


# ---------------------------------------------------------------- SQL output
def insert_sql(t, row):
    cols = [c.name for c in t.columns if c.name in row]
    return (f'IF NOT EXISTS (SELECT 1 FROM {q(t.name)} WHERE {where(t, row)})\n'
            f'    INSERT INTO {q(t.name)} ({", ".join(q(c) for c in cols)})\n'
            f'    VALUES ({", ".join(lit(row[c]) for c in cols)});')


def block(t, rows):
    ident = any(c.identity for c in t.columns)
    out = []
    if ident:
        out.append(f'SET IDENTITY_INSERT {q(t.name)} ON;')
    out += [insert_sql(t, r) for r in rows]
    if ident:
        out.append(f'SET IDENTITY_INSERT {q(t.name)} OFF;')
    return out


def verification_checks(m):
    tables = m['tables']
    checks = []
    add = lambda name, sql: checks.append((name, sql))
    # FK integrity
    for t in sorted(tables.values(), key=lambda t: t.name):
        for fk in t.fks:
            c = fk.columns[0]
            add(f'fk {t.name}.{c} -> {fk.ref_table}',
                f'SELECT COUNT(*) FROM {q(t.name)} AS c WHERE c.{q(c)} IS NOT NULL AND NOT EXISTS '
                f'(SELECT 1 FROM {q(fk.ref_table)} AS p WHERE p.{q(fk.ref_columns[0])} = c.{q(c)})')
    add('all foreign keys enabled and trusted',
        'SELECT COUNT(*) FROM sys.foreign_keys WHERE is_disabled = 1 OR is_not_trusted = 1')
    for tn in m['sentinel_tables']:
        add(f'sentinel {tn}(-1)', f'SELECT CASE WHEN COUNT(*) = 1 THEN 0 ELSE 1 END FROM {q(tn)} WHERE {q("id")} = -1')
    # Fixed data source rows
    for oid, fi in [(1, 'ORCID'), (2, 'Tiedejatutkimus.fi')]:
        add(f'fixed dim_organization({oid}) name_fi={fi}',
            f"SELECT CASE WHEN COUNT(*) = 1 THEN 0 ELSE 1 END FROM [dim_organization] WHERE [id] = {oid} AND [name_fi] = N'{fi}' "
            f"AND [dim_registered_data_source_id] = -1 AND [dim_sectorid] = -1")
        add(f'fixed dim_registered_data_source({oid}) name={fi}',
            f"SELECT CASE WHEN COUNT(*) = 1 THEN 0 ELSE 1 END FROM [dim_registered_data_source] WHERE [id] = {oid} "
            f"AND [name] = N'{fi}' AND [dim_organization_id] = {oid}")
        add(f'exactly one data source for organization name_fi={fi}',
            f"SELECT CASE WHEN COUNT(*) = 1 THEN 0 ELSE 1 END FROM [dim_registered_data_source] AS r "
            f"JOIN [dim_organization] AS o ON o.[id] = r.[dim_organization_id] WHERE o.[name_fi] = N'{fi}'")
        add(f'exactly one data source named {fi}',
            f"SELECT CASE WHEN COUNT(*) = 1 THEN 0 ELSE 1 END FROM [dim_registered_data_source] WHERE [name] = N'{fi}'")
    # Profile-only rows
    for po, (col, fid) in S.PROFILE_ONLY.items():
        add(f'{po}: data source is ORCID (1)', f'SELECT COUNT(*) FROM [{po}] WHERE [id] > 0 AND [dim_registered_data_source_id] <> 1')
        add(f'{po}: every row linked from fact_field_values',
            f'SELECT COUNT(*) FROM [{po}] AS p WHERE p.[id] > 0 AND NOT EXISTS (SELECT 1 FROM [fact_field_values] AS f WHERE f.[{col}] = p.[id])')
        add(f'fact_field_values.{col}: ORCID source and ORCID put code',
            f"SELECT COUNT(*) FROM [fact_field_values] AS f LEFT JOIN [dim_pid] AS d ON d.[id] = f.[dim_pid_id_orcid_put_code] "
            f"WHERE f.[{col}] <> -1 AND (f.[dim_registered_data_source_id] <> 1 OR f.[dim_pid_id_orcid_put_code] = -1 "
            f"OR d.[pid_type] IS NULL OR d.[pid_type] <> N'ORCID put code')")
    add('fact_field_values: no two rows share an ORCID put code dim_pid',
        'SELECT COUNT(*) FROM (SELECT [dim_pid_id_orcid_put_code] FROM [fact_field_values] WHERE [dim_pid_id_orcid_put_code] <> -1 '
        'GROUP BY [dim_pid_id_orcid_put_code] HAVING COUNT(*) > 1) AS x')
    other_pid_fks = [fk.columns[0] for fk in tables['dim_pid'].fks if fk.columns[0] != 'dim_known_person_id']
    add('put-code dim_pid: every FK except dim_known_person_id is -1',
        "SELECT COUNT(*) FROM [dim_pid] WHERE [pid_type] = N'ORCID put code' AND ("
        + ' OR '.join(f'ISNULL({q(c)}, 0) <> -1' for c in other_pid_fks) + ')')
    add("put-code dim_pid: numeric pid_content and source_id 'Profile API'",
        "SELECT COUNT(*) FROM [dim_pid] WHERE [pid_type] = N'ORCID put code' AND ([pid_content] LIKE N'%[^0-9]%' "
        "OR [pid_content] = N'' OR [source_id] <> N'Profile API')")
    add('put-code dim_pid: pid_content unique',
        "SELECT COUNT(*) FROM (SELECT [pid_content] FROM [dim_pid] WHERE [pid_type] = N'ORCID put code' "
        "GROUP BY [pid_content] HAVING COUNT(*) > 1) AS x")
    add('put-code dim_pid: known person is the owning profile\'s known person',
        'SELECT COUNT(*) FROM [fact_field_values] AS f JOIN [dim_pid] AS d ON d.[id] = f.[dim_pid_id_orcid_put_code] '
        'JOIN [dim_user_profile] AS u ON u.[id] = f.[dim_user_profile_id] '
        'WHERE f.[dim_pid_id_orcid_put_code] <> -1 AND d.[dim_known_person_id] <> u.[dim_known_person_id]')
    # fact_field_values
    ffv = "[fact_field_values] AS f JOIN [dim_field_display_settings] AS s ON s.[id] = f.[dim_field_display_settings_id]"
    add('ffv: always-real columns are not -1',
        'SELECT COUNT(*) FROM [fact_field_values] WHERE [dim_user_profile_id] <= 0 OR [dim_field_display_settings_id] <= 0 '
        'OR [dim_registered_data_source_id] <= 0')
    add('ffv: exactly one primary link column is set',
        'SELECT COUNT(*) FROM [fact_field_values] WHERE ('
        + ' + '.join(f'CASE WHEN {q(c)} <> -1 THEN 1 ELSE 0 END' for c in S.PRIMARY_LINK_COLUMNS) + ') <> 1')
    match = ' OR '.join(
        f"(s.[field_identifier] = {fid} AND ({' OR '.join(f'f.{q(c)} <> -1' for c in cols)}))" for fid, cols in S.PRIMARY_LINKS.items())
    add('ffv: settings field_identifier matches the link column', f'SELECT COUNT(*) FROM {ffv} WHERE NOT ({match})')
    add('ffv: settings row belongs to the same user profile', f'SELECT COUNT(*) FROM {ffv} WHERE s.[dim_user_profile_id] <> f.[dim_user_profile_id]')
    add('ffv: put code only on allowed ORCID-sourced fields',
        f"SELECT COUNT(*) FROM {ffv} WHERE f.[dim_pid_id_orcid_put_code] <> -1 AND (f.[dim_registered_data_source_id] <> 1 "
        f"OR s.[field_identifier] NOT IN ({', '.join(map(str, S.PUT_CODE_FIELDS))}))")
    add('ffv: ORCID rows of put-code fields have a put code',
        f"SELECT COUNT(*) FROM {ffv} WHERE f.[dim_registered_data_source_id] = 1 AND f.[dim_pid_id_orcid_put_code] = -1 "
        f"AND s.[field_identifier] IN ({', '.join(map(str, S.PUT_CODE_REQUIRED_FIELDS))})")
    add('ffv: put code dim_pid differs from dim_pid_id',
        'SELECT COUNT(*) FROM [fact_field_values] WHERE [dim_pid_id_orcid_put_code] <> -1 AND [dim_pid_id_orcid_put_code] = [dim_pid_id]')
    add('ffv: dim_identifierless_data_id only with 300 or profile-only 800/900',
        f'SELECT COUNT(*) FROM {ffv} WHERE f.[dim_identifierless_data_id] <> -1 AND NOT ((s.[field_identifier] = 300 AND f.[dim_affiliation_id] <> -1) '
        f'OR f.[dim_profile_only_funding_decision_id] <> -1 OR f.[dim_profile_only_research_activity_id] <> -1)')
    add('ffv: identifierless data has a valid type',
        "SELECT COUNT(*) FROM [fact_field_values] AS f JOIN [dim_identifierless_data] AS i ON i.[id] = f.[dim_identifierless_data_id] "
        "WHERE f.[dim_identifierless_data_id] <> -1 AND i.[type] NOT IN (N'organization_name', N'organization_unit')")
    add('ffv: dim_referencedata_actor_role_id only with profile-only 800/900',
        'SELECT COUNT(*) FROM [fact_field_values] WHERE [dim_referencedata_actor_role_id] <> -1 AND '
        '[dim_profile_only_funding_decision_id] = -1 AND [dim_profile_only_research_activity_id] = -1')
    add('ffv: never-set columns are -1',
        'SELECT COUNT(*) FROM [fact_field_values] WHERE ' + ' OR '.join(f'{q(c)} <> -1' for c in S.NEVER_SET_FFV_COLUMNS))
    add('ffv: show and primary_value are not NULL',
        'SELECT COUNT(*) FROM [fact_field_values] WHERE [show] IS NULL OR [primary_value] IS NULL')
    for col, tn, kpcol in [('dim_email_addrress_id', 'dim_email_addrress', 'dim_known_person_id'),
                           ('dim_telephone_number_id', 'dim_telephone_number', 'dim_known_person_id'),
                           ('dim_researcher_description_id', 'dim_researcher_description', 'dim_known_person_id'),
                           ('dim_affiliation_id', 'dim_affiliation', 'dim_known_person_id'),
                           ('dim_education_id', 'dim_education', 'dim_known_person_id'),
                           ('dim_web_link_id', 'dim_web_link', 'dim_known_person_id'),
                           ('dim_pid_id', 'dim_pid', 'dim_known_person_id'),
                           ('dim_name_id', 'dim_name', 'dim_known_person_id_confirmed_identity'),
                           ('dim_profile_only_publication_id', 'dim_profile_only_publication', 'dim_known_person_id')]:
        add(f'ffv: {tn} belongs to the profile\'s known person',
            f'SELECT COUNT(*) FROM [fact_field_values] AS f JOIN [{tn}] AS x ON x.[id] = f.[{col}] '
            f'JOIN [dim_user_profile] AS u ON u.[id] = f.[dim_user_profile_id] '
            f'WHERE f.[{col}] <> -1 AND ISNULL(x.[{kpcol}], -1) <> u.[dim_known_person_id]')
    add('ffv 110: name has a data source and an ORCID dim_pid on its known person',
        f"SELECT COUNT(*) FROM {ffv} JOIN [dim_name] AS n ON n.[id] = f.[dim_name_id] WHERE s.[field_identifier] = 110 "
        f"AND (n.[dim_registered_data_source_id] = -1 OR NOT EXISTS (SELECT 1 FROM [dim_pid] AS p WHERE "
        f"p.[dim_known_person_id] = n.[dim_known_person_id_confirmed_identity] AND p.[pid_type] = N'ORCID'))")
    add('dim_field_display_settings: each profile has the 16 field identifiers',
        f"SELECT COUNT(*) FROM [dim_user_profile] AS u WHERE u.[id] > 0 AND (SELECT COUNT(DISTINCT s.[field_identifier]) "
        f"FROM [dim_field_display_settings] AS s WHERE s.[dim_user_profile_id] = u.[id] AND s.[field_identifier] IN "
        f"({', '.join(map(str, S.FIELD_IDENTIFIERS))})) <> {len(S.FIELD_IDENTIFIERS)}")
    add('dim_user_profile: ORCID dim_pid of its known person matches orcid_id',
        "SELECT COUNT(*) FROM [dim_user_profile] AS u WHERE u.[id] > 0 AND NOT EXISTS (SELECT 1 FROM [dim_pid] AS p "
        "WHERE p.[dim_known_person_id] = u.[dim_known_person_id] AND p.[pid_type] = N'ORCID' AND p.[pid_content] = u.[orcid_id])")
    # fact_contribution
    fc_used = {'dim_name_id', 'dim_publication_id', 'dim_research_dataset_id', 'dim_research_activity_id', 'dim_referencedata_actor_role_id'}
    fc_unused = [fk.columns[0] for fk in tables['fact_contribution'].fks if fk.columns[0] not in fc_used]
    add('fact_contribution: name rows have exactly one of publication/dataset/activity',
        'SELECT COUNT(*) FROM [fact_contribution] WHERE [dim_name_id] <> -1 AND (CASE WHEN [dim_publication_id] <> -1 THEN 1 ELSE 0 END '
        '+ CASE WHEN [dim_research_dataset_id] <> -1 THEN 1 ELSE 0 END + CASE WHEN [dim_research_activity_id] <> -1 THEN 1 ELSE 0 END) <> 1')
    add('fact_contribution: activity_type/researcher_name_activity have activity and actor role',
        "SELECT COUNT(*) FROM [fact_contribution] WHERE [contribution_type] IN (N'activity_type', N'researcher_name_activity') "
        "AND ([dim_research_activity_id] = -1 OR [dim_referencedata_actor_role_id] = -1)")
    add('fact_contribution: activity_type rows have dim_name_id -1',
        "SELECT COUNT(*) FROM [fact_contribution] WHERE [contribution_type] = N'activity_type' AND [dim_name_id] <> -1")
    add('fact_contribution: researcher_name_activity rows have a real dim_name_id',
        "SELECT COUNT(*) FROM [fact_contribution] WHERE [contribution_type] = N'researcher_name_activity' AND [dim_name_id] = -1")
    add("fact_contribution: role's parent is the activity's activity_type",
        "SELECT COUNT(*) FROM [fact_contribution] AS r JOIN [dim_referencedata] AS role ON role.[id] = r.[dim_referencedata_actor_role_id] "
        "WHERE r.[contribution_type] = N'researcher_name_activity' AND NOT EXISTS (SELECT 1 FROM [fact_contribution] AS a "
        "WHERE a.[contribution_type] = N'activity_type' AND a.[dim_research_activity_id] = r.[dim_research_activity_id] "
        "AND a.[dim_referencedata_actor_role_id] = role.[dim_referencedataid])")
    add('fact_contribution: unused FK columns are -1',
        'SELECT COUNT(*) FROM [fact_contribution] WHERE ' + ' OR '.join(f'{q(c)} <> -1' for c in fc_unused))
    add('fact_contribution: a co-publication child has a contribution row',
        'SELECT CASE WHEN COUNT(*) > 0 THEN 0 ELSE 1 END FROM [fact_contribution] AS fc JOIN [dim_publication] AS p '
        'ON p.[id] = fc.[dim_publication_id] WHERE fc.[dim_name_id] <> -1 AND p.[dim_publication_id] > 0')
    return checks


def write_sql(m, ddl_name, path):
    tables, out = m['tables'], []
    now = datetime.date.today().isoformat()
    out += [
        '-- seed_test_data.sql',
        f'-- Generated {now} by .github/skills/create-database-seed-file/scripts/generate_seed.py from DDL file: {ddl_name}',
        '-- Contents: -1 sentinel rows, fixed ORCID/TTV data source rows, test rows, verification queries.',
        '-- Every INSERT is guarded by IF NOT EXISTS on the primary key, so the script can be re-run.',
        '-- Run against an empty database created from the same DDL, e.g. sqlcmd -b -I -i seed_test_data.sql',
        '',
        'SET NOCOUNT ON;',
        'SET XACT_ABORT ON;',
        'SET QUOTED_IDENTIFIER ON;',
        'BEGIN TRANSACTION;',
        '',
        '-- =====================================================================',
        '-- 1. Sentinel rows (id = -1) for every FK target table, in insert order',
        '-- =====================================================================',
    ]
    deferred_by_child = defaultdict(list)
    deferred_by_parent = defaultdict(list)
    for fk in m['deferred']:
        if fk.table in m['sentinels']:
            deferred_by_child[fk.table].append(fk)
            deferred_by_parent[fk.ref_table].append(fk)
    for tn in m['order']:
        if tn not in m['sentinels']:
            continue
        t = tables[tn]
        out.append(f'-- {tn}')
        for fk in deferred_by_child[tn]:
            if not t.col(fk.columns[0]).nullable:
                out.append(f'-- cycle: {tn}.{fk.columns[0]} -> {fk.ref_table} is checked after the parent sentinel exists')
                out.append(f'ALTER TABLE {q(tn)} NOCHECK CONSTRAINT {q(fk.name)};')
        out += block(t, [m['sentinels'][tn]])
        for fk in deferred_by_parent[tn]:
            ct = tables[fk.table]
            if ct.col(fk.columns[0]).nullable:
                out.append(f'UPDATE {q(fk.table)} SET {q(fk.columns[0])} = -1 WHERE [id] = -1;')
            else:
                out.append(f'ALTER TABLE {q(fk.table)} WITH CHECK CHECK CONSTRAINT {q(fk.name)};')
        out.append('')
    out += ['-- =====================================================================',
            '-- 2. Fixed data source rows required by StartupHelperService',
            '--    ORCID data source = 1, Tiedejatutkimus.fi (TTV) data source = 2',
            '-- =====================================================================']
    for tn in ('dim_organization', 'dim_registered_data_source'):
        if m['fixed_extra'][tn]:
            out.append(f'-- NOT NULL columns not in the manual script, filled with sentinel defaults: {", ".join(m["fixed_extra"][tn])}')
        out += block(tables[tn], m['fixed'][tn])
        out.append('')
    out += ['-- =====================================================================',
            '-- 3. Test rows, in insert order (parents before children)',
            '-- =====================================================================']
    for tn in m['order']:
        rows = m['rows'][tn]
        if not rows:
            continue
        out.append(f'-- {tn}: {len(rows)} rows')
        out += block(tables[tn], rows)
        out.append('')
    if m['post_updates']:
        out += ['-- =====================================================================',
                '-- 4. Deferred FK values (cycle edges whose parent rows are inserted later)',
                '-- =====================================================================']
        for tn, row, col, v in m['post_updates']:
            out.append(f'UPDATE {q(tn)} SET {q(col)} = {lit(v)} WHERE {where(tables[tn], row)};')
        out.append('')
    out += ['COMMIT TRANSACTION;', '', '-- =====================================================================',
            '-- 5. Verification: every check must report 0 failures',
            '-- =====================================================================',
            "IF OBJECT_ID(N'tempdb..#seed_checks') IS NOT NULL DROP TABLE #seed_checks;",
            'CREATE TABLE #seed_checks ([check_name] nvarchar(400) NOT NULL, [failures] int NOT NULL);']
    checks = verification_checks(m)
    for name, sql in checks:
        out.append(f'INSERT INTO #seed_checks ([check_name], [failures]) SELECT {sql_str(name)}, ({sql});')
    out += ['SELECT [check_name], [failures] FROM #seed_checks WHERE [failures] <> 0 ORDER BY [check_name];',
            'DECLARE @failed int = (SELECT COUNT(*) FROM #seed_checks WHERE [failures] <> 0);',
            'DECLARE @total int = (SELECT COUNT(*) FROM #seed_checks);',
            "PRINT CONCAT(N'Seed verification: ', @total - @failed, N'/', @total, N' checks passed');",
            "IF @failed > 0 RAISERROR(N'Seed verification failed: %d check(s) reported failures', 16, 1, @failed);",
            'DROP TABLE #seed_checks;', '']
    text = '\n'.join(out)
    bad = [ch for ch in text if ord(ch) > 127]
    if bad:
        raise ValueError(f'Non-ASCII characters in SQL output: {set(bad)}')
    with open(path, 'w', encoding='ascii', newline='\n') as f:
        f.write(text)
    return len(checks)


# ---------------------------------------------------------------- plan output
def write_plan(m, ddl_name, path, n_checks, verification_log):
    tables = m['tables']
    n_cols = sum(len(t.columns) for t in tables.values())
    n_fks = sum(len(t.fks) for t in tables.values())
    L = []
    L += [f'Status: Implemented ({datetime.date.today().isoformat()})', '', '# Seed plan', '',
          '## Inputs', '',
          f'- DDL file: `{ddl_name}` (read from outside the repository; not copied).',
          f'- Parsed: {len(tables)} tables, {n_cols} columns, {n_fks} foreign keys (all single-column, all referencing `id`).',
          f'- Self-referencing FKs: {len(m["self_refs"])} ({", ".join(f"`{fk.table}.{fk.columns[0]}`" for fk in m["self_refs"])}).',
          f'- FK target tables (get a -1 sentinel): {len(m["sentinel_tables"])}.',
          '- Generator: `.github/skills/create-database-seed-file/scripts/generate_seed.py <ddl> <output_dir>` in the research-fi-mydata repository. It takes the DDL path as an argument.',
          '', '## FK cycles', '']
    if m['cycles']:
        for cyc in m['cycles']:
            L.append(f'- Cycle: {" -> ".join(cyc + [cyc[0]])}')
        for fk in m['deferred']:
            nn = 'NOT NULL' if not tables[fk.table].col(fk.columns[0]).nullable else 'nullable'
            L.append(f'- Deferred edge: `{fk.table}.{fk.columns[0]}` -> `{fk.ref_table}` ({nn}, constraint `{fk.name}`). '
                     + ('Sentinel inserted with the constraint set to NOCHECK, re-enabled `WITH CHECK` right after the parent sentinel.'
                        if nn == 'NOT NULL' else 'Inserted as NULL, updated to -1 after the parent sentinel.'))
        L.append('- Self-references are not ordering edges: sentinels point at themselves (-1), test rows point at earlier rows or -1/NULL.')
    else:
        L.append('- None.')
    L += ['', '## Insert order', '',
          'Parents before children (Kahn topological sort, ties broken alphabetically). S = has a -1 sentinel row.', '',
          '| # | Table | S | Test rows | Source of rows |', '|---|---|---|---|---|']
    for i, tn in enumerate(m['order'], start=1):
        src = 'hand-designed (app conventions)' if tn in m['explicit'] else \
            ('generic sparse fact' if tn in SPARSE else 'generic')
        fixed = ' + fixed ids 1, 2' if tn in m['fixed'] else ''
        L.append(f'| {i} | `{tn}` | {"S" if tn in m["sentinels"] else ""} | {len(m["rows"][tn])}{fixed} | {src} |')
    L += ['', '## Sentinel defaults', '',
          '- `id` = -1; every FK column = -1 (deferred nullable edges excepted, see above).',
          "- NOT NULL non-FK columns: strings `N' '`, bit 0, datetime `1900-01-01`, numeric 0 (or -1 when the column name ends in `id` "
          "or has `DEFAULT -1`). This makes the `dim_date` sentinel 0/0/0, matching the API test infrastructure.",
          '- Nullable non-FK columns: NULL.', '',
          '## Fixed data source rows', '',
          '- `dim_organization` 1 (ORCID) and 2 (Tiedejatutkimus.fi), `dim_registered_data_source` 1 and 2, inserted right after the sentinels.']
    for tn, cols in m['fixed_extra'].items():
        if cols:
            L.append(f'- `{tn}`: NOT NULL columns missing from the manual script filled with sentinel defaults: '
                     + ', '.join(f'`{c}`' for c in cols) + '.')
    L += ['- Test rows reference ORCID data source = 1 and TTV data source = 2 by literal id. Test data sources 3-8: virta, metax, '
          'sftp_funding, Helsingin yliopisto, Aalto-yliopisto, Jyvaskylan yliopisto.', '',
          '## Test data design', '',
          '- Known persons 1-10 (ORCID iDs `0000-0000-0000-00NN`, fictitious). Persons 1-3 have user profiles 1-3; 4-10 exist only in TTV data.',
          '- Each profile has the 16 `dim_field_display_settings` rows that `UserProfileService.CreateProfile` creates (48 rows).',
          '- `fact_field_values`: sparse rows (all FKs -1 except the always-real columns, one primary link column and the allowed secondary columns).',
          '  - Profile 1 covers every link type (110-900, incl. two affiliations: one matched organization + `organization_unit`, one unmatched '
          '`organization_name` with an `organization_unit` child).',
          '  - Profile 2 mixes TTV-sourced items (email/affiliation from Helsingin yliopisto, publications from virta, datasets from metax, '
          'funding from sftp_funding) with ORCID items.',
          '  - Profile 3 is a fresh profile with mostly hidden ORCID items.',
          '  - Every ORCID-sourced row of fields 120, 130, 150, 180, 300, 400, 550, 600, 800, 900 (and one 171 row) has its own put-code `dim_pid` '
          '(ids from 101, `pid_content` from 1000001).',
          '- Profile-only tables: 3 rows each (one per profile), all `dim_registered_data_source_id = 1`, each linked from `fact_field_values` '
          '(550/600/800/900) with an ORCID put code. Funding/activity rows carry `dim_referencedata_actor_role_id`; unmatched organizations use '
          '`dim_identifierless_data` of type `organization_name`.',
          '- `fact_contribution`: person-to-publication (incl. co-publication child 10 of parent 9), person-to-dataset, person-to-activity, '
          '`activity_type` and `researcher_name_activity` rows (role parent = activity type). Unused FK columns are -1.',
          '- Funding decisions link to people only through `br_participates_in_funding_group`.',
          '- Generic tables (infrastructure, services, news, word clusters, concepts, br_* bridges, other fact tables) get 10 rows built from '
          'column-name heuristics; their FKs point to earlier test rows, referencedata FKs are -1 unless a pool is defined, data source FKs are 2.',
          '- Text uses `example.org` URLs and `10.5555` DOIs. Non-ASCII letters are written as `NCHAR(n)` so the script stays ASCII.', '',
          '## Deviations from "about 10 rows per table"', '',
          '- `dim_field_display_settings` 48, `fact_field_values` and put-code `dim_pid` rows above 10: required so every profile and every '
          'profile-only row is complete.',
          '- `dim_referencedata` 26: activity/role, sharing (`tutkijaprofiilin_luvitus` 01-08), user choice, field of science, publication type, '
          'language, relation type and field of art codes.',
          '- Lookup-style tables (`dim_sector`, `dim_geo`, `dim_research_data_catalog`, `dim_call_programme`, `dim_event`, profile-only tables) '
          'have fewer than 10 rows.', '',
          '## Unknowns and assumptions', '',
          '- `fact_contribution.contribution_type` for person-to-output rows: `<insufficient data>` (the API does not read it). The literal '
          "value `N'<insufficient data>'` is stored so it is easy to find and replace.",
          '- Field identifier 160 (PERSON_FIELD_OF_SCIENCE) -> `dim_referencedata_field_of_science_id`: `<insufficient data>`; no '
          '`fact_field_values` rows use it (settings rows exist, `show` = 0).',
          f'- Code scheme names {", ".join(f"`{s}`" for s in S.ILLUSTRATIVE_CODE_SCHEMES)} and the names/codes of roles `3.1.1` and `2.3.1` '
          'are illustrative, not taken from the official code lists. Scheme names from `Constants` (`aktiviteetitjaroolit`, '
          '`tutkijaprofiilin_luvitus`, `Kiinnostuksen_ilmaiseminen`) are real.',
          '- `dim_controller_purpose.purpose_code_id`: assumed to reference the `tutkijaprofiilin_luvitus` codes (draft consent model); '
          '`<insufficient data>` otherwise.',
          '- `dim_descriptive_item.dim_profile_data_sharing_consentid` is NOT NULL without an FK in the DDL; seeded as -1.',
          '- The DDL has no `dim_purpose` or `br_granted_permissions` table, but the current API reads `dim_purpose` at startup '
          '(`StartupHelperService.GetDimPurposeId_OnStartup_TTV`) and writes `br_granted_permissions` on profile creation. '
          'They cannot be seeded from this DDL: `<insufficient data>`.', '',
          '## Verification', '',
          f'- The script ends with {n_checks} checks collected in `#seed_checks`; it prints `N/N checks passed` and raises an error if any fail.',
          '- Checks: orphan check for every FK, all FKs enabled and trusted, one -1 row per sentinel table, fixed rows 1/2 and one data source '
          'per name, profile-only rules, put-code uniqueness and shape, `fact_field_values` sparse-row rules (always-real columns, exactly one '
          'primary link, field identifier match, same-profile settings, allowed secondary columns, never-set columns, known-person ownership), '
          '16 settings per profile, and the `fact_contribution` patterns.', '']
    if verification_log:
        with open(verification_log, encoding='utf-8') as f:
            L += ['### Result against a disposable database', '', '```text', f.read().strip(), '```', '']
    else:
        L += ['### Result against a disposable database', '', '- Not run.', '']
    with open(path, 'w', encoding='ascii', errors='strict', newline='\n') as f:
        f.write('\n'.join(L))


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument('ddl')
    ap.add_argument('output_dir')
    ap.add_argument('--verification-log')
    a = ap.parse_args()
    m = build(a.ddl)
    ddl_name = os.path.basename(a.ddl)
    os.makedirs(a.output_dir, exist_ok=True)
    n_checks = write_sql(m, ddl_name, os.path.join(a.output_dir, 'seed_test_data.sql'))
    write_plan(m, ddl_name, os.path.join(a.output_dir, 'seed_plan.md'), n_checks, a.verification_log)
    total = sum(len(r) for r in m['rows'].values())
    print(f'tables={len(m["tables"])} sentinels={len(m["sentinels"])} test_rows={total} checks={n_checks} '
          f'cycles={len(m["cycles"])} deferred={[(fk.table, fk.columns[0]) for fk in m["deferred"]]} post_updates={len(m["post_updates"])}')


if __name__ == '__main__':
    main()
