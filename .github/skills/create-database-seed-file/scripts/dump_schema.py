"""Print a compact schema summary parsed from the DDL: python3 dump_schema.py <ddl> [table ...]"""
import sys
from ddl_parser import parse

tables, unparsed = parse(sys.argv[1])
only = set(sys.argv[2:])
print(f'tables={len(tables)} columns={sum(len(t.columns) for t in tables.values())} '
      f'fks={sum(len(t.fks) for t in tables.values())} unparsed={len(unparsed)}')
for u in unparsed:
    print('UNPARSED:', u.replace('\n', ' ')[:160])
for t in sorted(tables.values(), key=lambda t: t.name):
    if only and t.name not in only:
        continue
    fkmap = {fk.columns[0]: fk for fk in t.fks if len(fk.columns) == 1}
    print(f'\n## {t.name}  PK={t.pk}' + (f' UQ={t.unique}' if t.unique else ''))
    for c in t.columns:
        fk = fkmap.get(c.name)
        flags = ('ID ' if c.identity else '') + ('NULL' if c.nullable else 'NN') \
            + (f' DEF={c.default}' if c.default else '')
        print(f'  {c.name} {c.type} {flags}' + (f' -> {fk.ref_table}.{fk.ref_columns[0]}' if fk else ''))
    for fk in t.fks:
        if len(fk.columns) != 1:
            print(f'  COMPOSITE FK {fk.columns} -> {fk.ref_table}{fk.ref_columns}')
