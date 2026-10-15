"""Parse a SQL Server / Visual Paradigm DDL file into tables, columns, PKs and FKs."""
import re
from dataclasses import dataclass, field


@dataclass
class Column:
    name: str
    type: str
    nullable: bool
    identity: bool
    default: str | None


@dataclass
class ForeignKey:
    name: str
    table: str
    columns: list
    ref_table: str
    ref_columns: list


@dataclass
class Table:
    name: str
    columns: list = field(default_factory=list)
    pk: list = field(default_factory=list)
    unique: list = field(default_factory=list)
    fks: list = field(default_factory=list)

    def col(self, name):
        for c in self.columns:
            if c.name == name:
                return c
        return None


IDENT = r'(?:\[[^\]]+\]|[A-Za-z_][A-Za-z0-9_]*)'


def norm(ident):
    ident = ident.strip()
    if ident.startswith('['):
        ident = ident[1:-1]
    return ident.strip()


def split_top_level(body):
    parts, depth, cur = [], 0, []
    for ch in body:
        if ch == '(':
            depth += 1
        elif ch == ')':
            depth -= 1
        if ch == ',' and depth == 0:
            parts.append(''.join(cur))
            cur = []
        else:
            cur.append(ch)
    if ''.join(cur).strip():
        parts.append(''.join(cur))
    return [p.strip() for p in parts if p.strip()]


def strip_comments(text):
    text = re.sub(r'/\*.*?\*/', ' ', text, flags=re.S)
    return re.sub(r'--[^\n]*', ' ', text)


def statements(text):
    """Split on ';' outside string literals."""
    out, cur, in_str = [], [], False
    for ch in text:
        if ch == "'":
            in_str = not in_str
        if ch == ';' and not in_str:
            out.append(''.join(cur).strip())
            cur = []
        else:
            cur.append(ch)
    if ''.join(cur).strip():
        out.append(''.join(cur).strip())
    return [s for s in out if s]


def ident_list(s):
    return [norm(x) for x in split_top_level(s)]


COL_RE = re.compile(
    rf'^(?P<name>{IDENT})\s+(?P<type>[A-Za-z]+(?:\s*\([^)]*\))?)(?P<rest>.*)$', re.S)


def parse_create_table(stmt):
    m = re.match(rf'CREATE\s+TABLE\s+(?P<name>{IDENT})\s*\((?P<body>.*)\)\s*$', stmt, re.S | re.I)
    t = Table(norm(m.group('name')))
    for part in split_top_level(m.group('body')):
        up = part.upper()
        pk = re.match(rf'(?:CONSTRAINT\s+{IDENT}\s+)?PRIMARY\s+KEY\s*(?:CLUSTERED|NONCLUSTERED)?\s*\((.*)\)', part, re.I | re.S)
        if pk:
            t.pk = ident_list(pk.group(1))
            continue
        uq = re.match(rf'(?:CONSTRAINT\s+{IDENT}\s+)?UNIQUE\s*(?:CLUSTERED|NONCLUSTERED)?\s*\((.*)\)', part, re.I | re.S)
        if uq:
            t.unique.append(ident_list(uq.group(1)))
            continue
        fk = re.match(rf'(?:CONSTRAINT\s+({IDENT})\s+)?FOREIGN\s+KEY\s*\((.*?)\)\s*REFERENCES\s+({IDENT})\s*\((.*?)\)', part, re.I | re.S)
        if fk:
            t.fks.append(ForeignKey(fk.group(1) or '', t.name, ident_list(fk.group(2)),
                                    norm(fk.group(3)), ident_list(fk.group(4))))
            continue
        if up.startswith('CHECK') or up.startswith('CONSTRAINT'):
            raise ValueError(f'Unhandled table element in {t.name}: {part}')
        cm = COL_RE.match(part)
        if not cm:
            raise ValueError(f'Cannot parse column in {t.name}: {part}')
        rest = cm.group('rest')
        rest_up = rest.upper()
        default = None
        dm = re.search(r'DEFAULT\s+(\(.*?\)|\'[^\']*\'|N\'[^\']*\'|[^\s,]+)', rest, re.I)
        if dm:
            default = dm.group(1)
        nullable = not re.search(r'\bNOT\s+NULL\b', rest_up)
        identity = 'IDENTITY' in rest_up
        if re.search(r'\bPRIMARY\s+KEY\b', rest_up):
            t.pk = [norm(cm.group('name'))]
        if re.search(r'\bUNIQUE\b', rest_up):
            t.unique.append([norm(cm.group('name'))])
        t.columns.append(Column(norm(cm.group('name')), re.sub(r'\s+', '', cm.group('type')).lower(),
                                nullable, identity, default))
    return t


def parse(path):
    with open(path, encoding='utf-8-sig') as f:
        text = f.read().replace('\r\n', '\n')
    text = strip_comments(text)
    tables, fks, unparsed = {}, [], []
    for stmt in statements(text):
        s = stmt.lstrip()
        head = s[:40].upper()
        if head.startswith('CREATE TABLE'):
            t = parse_create_table(s)
            tables[t.name] = t
        elif head.startswith('ALTER TABLE'):
            m = re.match(rf'ALTER\s+TABLE\s+({IDENT})\s+ADD\s+CONSTRAINT\s+({IDENT})\s+FOREIGN\s+KEY\s*\((.*?)\)\s*REFERENCES\s+({IDENT})\s*\((.*?)\)',
                         s, re.I | re.S)
            if m:
                fks.append(ForeignKey(m.group(2), norm(m.group(1)), ident_list(m.group(3)),
                                      norm(m.group(4)), ident_list(m.group(5))))
                continue
            m = re.match(rf'ALTER\s+TABLE\s+({IDENT})\s+ADD\s+CONSTRAINT\s+({IDENT})\s+UNIQUE\s*(?:CLUSTERED|NONCLUSTERED)?\s*\((.*?)\)', s, re.I | re.S)
            if m:
                fks.append(('UNIQUE', norm(m.group(1)), ident_list(m.group(3))))
                continue
            unparsed.append(s[:200])
        elif head.startswith('CREATE INDEX') or head.startswith('CREATE UNIQUE INDEX') \
                or head.startswith('CREATE NONCLUSTERED') or head.startswith('CREATE CLUSTERED') \
                or head.startswith('EXEC SP_ADDEXTENDEDPROPERTY'):
            m = re.match(rf'CREATE\s+UNIQUE\s+(?:NONCLUSTERED\s+|CLUSTERED\s+)?INDEX\s+{IDENT}\s+ON\s+({IDENT})\s*\((.*?)\)', s, re.I | re.S)
            if m:
                cols = [re.sub(r'\s+(ASC|DESC)$', '', c, flags=re.I) for c in ident_list(m.group(2))]
                fks.append(('UNIQUE', norm(m.group(1)), [norm(c) for c in cols]))
            continue
        else:
            unparsed.append(s[:200])
    for fk in fks:
        if isinstance(fk, tuple):
            tables[fk[1]].unique.append(fk[2])
        else:
            if fk.table not in tables or fk.ref_table not in tables:
                raise ValueError(f'FK {fk.name} references unknown table: {fk.table} -> {fk.ref_table}')
            tables[fk.table].fks.append(fk)
    return tables, unparsed
