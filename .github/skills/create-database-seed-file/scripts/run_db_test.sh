#!/usr/bin/env bash
# Run seed_test_data.sql against a disposable SQL Server container.
# Usage: run_db_test.sh <ddl_path> <seed_sql> <log_path>
# The DDL is patched into a temp copy outside the repository (DEFAULT false/true -> 0/1).
set -euo pipefail
DDL="$1"; SEED="$2"; LOG="$3"
NAME=ttv-seed-test
IMAGE=mcr.microsoft.com/mssql/server:2022-latest
WORK="$(mktemp -d "${TMPDIR:-/tmp}/ttv-seed.XXXXXX")"
PW="Seed$(openssl rand -hex 12)Aa1#"

sed -E 's/DEFAULT[[:space:]]+false/DEFAULT 0/Ig; s/DEFAULT[[:space:]]+true/DEFAULT 1/Ig' "$DDL" > "$WORK/schema.sql"

docker rm -f "$NAME" >/dev/null 2>&1 || true
docker run -d --name "$NAME" --platform linux/amd64 -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=$PW" "$IMAGE" >/dev/null
SQLCMD=(docker exec -i "$NAME" /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$PW" -C -b -I)
for i in $(seq 1 60); do
  if "${SQLCMD[@]}" -Q "SELECT 1" >/dev/null 2>&1; then break; fi
  sleep 2
done
"${SQLCMD[@]}" -Q "CREATE DATABASE ttv_seed_test" >/dev/null
docker cp "$WORK/schema.sql" "$NAME:/tmp/schema.sql"
docker cp "$SEED" "$NAME:/tmp/seed.sql"

{
  echo "== apply DDL (patched copy: DEFAULT false/true -> 0/1)"
  "${SQLCMD[@]}" -d ttv_seed_test -i /tmp/schema.sql 2>&1 | tail -20 && echo "DDL OK"
  echo "== run 1"
  "${SQLCMD[@]}" -d ttv_seed_test -i /tmp/seed.sql 2>&1
  echo "exit=$?"
  echo "== run 2 (re-run, guarded inserts)"
  "${SQLCMD[@]}" -d ttv_seed_test -i /tmp/seed.sql 2>&1
  echo "exit=$?"
  echo "== row counts (id > 0 where applicable)"
  "${SQLCMD[@]}" -d ttv_seed_test -h -1 -W -Q "SET NOCOUNT ON; SELECT CONCAT(t.name, '=', SUM(p.rows)) FROM sys.tables t JOIN sys.partitions p ON p.object_id = t.object_id AND p.index_id IN (0,1) GROUP BY t.name ORDER BY t.name" 2>&1 | tr '\n' ' '
  echo
} > "$LOG" 2>&1 || true
rm -rf "$WORK"
echo "log: $LOG"
