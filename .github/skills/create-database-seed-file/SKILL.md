---
name: create-database-seed-file
description: 'Generate an executable SQL Server (T-SQL) seed script with -1 sentinel rows and realistic test rows from a DDL file, plus a plan file. Use when the user asks for seed data, test data, sentinel rows, -1 rows, INSERT scripts, sample data, or a data fixture from DDL, including sparse fact table rows (fact_field_values, fact_contribution) where most FK columns are -1.'
---

# Create Database Seed File

## When to Use
- The user asks for seed data, test data, sample data, or an INSERT script for a database
- The user mentions sentinel rows (-1 rows) or a data fixture derived from DDL
- A DDL file changed and the existing seed script must be checked or regenerated

## Inputs
- DDL file path (required)
- Output folder (ask the user if not given)

## Model and Environment Requirements
- **Reasoning effort: high.** The task combines FK graph analysis, cycle resolution, composite-PK uniqueness and the sparse fact-row rules below. Low-effort runs tend to fill every FK with a real id or skip the verification steps.
- **Model capability:** a frontier model that is strong at agentic coding. It must write, run and debug a generator script over several iterations, and hold a long DDL plus this skill in context. Do not use small or fast models for this skill.
- **Tools:** terminal access and file creation are required, and Python 3 (or the .NET SDK) is needed to run the generator. `sqlcmd` or Docker with SQL Server is optional, for running the script against a disposable database.
- If the current session does not meet these requirements, say so before starting rather than producing a partial script.
- Check that the interpreter actually starts (`python3 --version`), not just that it is on `PATH`. A pyenv shim can exist but crash on a missing library. Fall back to `/opt/homebrew/bin/python3.<n>` or `/usr/bin/python3`.

## Bundled Scripts
Reuse the scripts in [scripts/](scripts/) and adapt them; don't rewrite them. They were verified against the 2026-10-14 draft DDL: 416/416 checks passed on SQL Server 2022.

| Script | Purpose |
|---|---|
| `ddl_parser.py` | Parses `CREATE TABLE` and `ALTER TABLE ... FOREIGN KEY/UNIQUE`, and returns the statements it could not parse |
| `dump_schema.py <ddl> [table ...]` | Prints a compact schema summary. Read this instead of the DDL |
| `seed_rows.py` | Hand-designed rows with explicit ids, plus the app constants (field identifiers, primary link columns, put-code fields, referencedata pools) |
| `generate_seed.py <ddl> <out> [--verification-log F]` | Insert order, cycle handling, sentinel, fixed and generic rows, the insert simulation, verification queries, and the SQL and plan output |
| `run_db_test.sh <ddl> <seed_sql> <log>` | Runs the script against a disposable SQL Server in Docker: patched DDL copy in `$TMPDIR`, two runs, then row counts |

- Run them with an absolute script path, or `cd` into `scripts/` first. Relative paths resolve against the terminal's working directory, which may not be the repository.
- When the DDL changes:
  1. Run `dump_schema.py` and compare the output with what `seed_rows.py` assumes.
  2. Fix any error the generator raises, such as an unknown column, a NULL in a NOT NULL column, a missing parent row or a duplicate key.
  3. Extend `seed_rows.py`, `SPARSE`, `THEMES` or `REF_POOLS` for new tables that the API uses.
- Every generated file and any DDL copy stays outside the repository. The scripts only take the DDL path as an argument.

## Procedure
The steps below describe what the bundled scripts do and why. Use them to adapt the scripts, or to rebuild them if they are missing.

1. **Parse the DDL with a script. Do not hand-read it.**
   - Use `head` and `grep -c` on the statement types only to learn the syntax. Then write the parser and dump a compact schema summary (columns, types, nullability, PK, FK targets) to `$TMPDIR`. Read that summary instead of the DDL.
   - Fail on any statement the parser does not recognize. Cross-check the counts against `grep -c '^CREATE TABLE'` and the `ALTER TABLE ... FOREIGN KEY` lines.
   - Visual Paradigm DDL quirks seen so far:
     - CRLF line endings.
     - All FKs are separate `ALTER TABLE ... ADD CONSTRAINT` statements.
     - Bracketed table, column and constraint names can contain spaces or a trailing space (`[dim_publication ]`, `[br_successor organization]`, `[dim_publication_ id]`, `[project start]`). Strip the brackets, then trim, for both identifiers and constraint names.
     - Many `EXEC sp_addextendedproperty` statements to skip.
     - A table with no PK (`dim_organisation_media`).
     - `DEFAULT false`, which is not valid T-SQL.
2. **Build the FK graph, resolve cycles and compute the insert order.**
   - Find cycles with Tarjan SCC. Self-references are not ordering edges.
   - The only known cycle is `dim_organization.dim_registered_data_source_id` <-> `dim_registered_data_source.dim_organization_id`, and both columns are NOT NULL. Defer the `dim_organization` edge so `dim_organization` is inserted first.
   - For a deferred NOT NULL edge, run `ALTER TABLE ... NOCHECK CONSTRAINT` before the child's sentinel row. Run `ALTER TABLE ... WITH CHECK CHECK CONSTRAINT` right after the parent's sentinel row, so the FK stays trusted. For a deferred nullable edge, insert NULL and run an UPDATE afterwards.
   - Use a Kahn topological sort with alphabetical tie-breaking.
3. **Generate a -1 sentinel row for every FK target table.**
   - Every FK column is -1, including nullable ones. A self-referencing -1 row pointing at itself is accepted by SQL Server.
   - NOT NULL non-FK columns:
     - strings: `N' '`
     - bit: 0
     - datetime: `1900-01-01`
     - numeric: 0, or -1 when the name ends in `id` or the column has `DEFAULT -1`
   - Nullable non-FK columns are NULL. The `dim_date` sentinel is 0/0/0, which matches the API test infrastructure.
   - Right after the sentinels, insert the [fixed data source rows](#fixed-data-source-rows).
4. **Generate test rows in insert order.**
   - Hand-design rows with explicit ids for the tables the API reads or writes:
     - person data, profiles and settings
     - `dim_pid`, `dim_name` and `dim_referencedata`
     - publications, datasets, funding and activities
     - the profile-only tables, `fact_field_values` and `fact_contribution`
   - Generate the remaining tables from column-name heuristics. FKs point to earlier test rows, referencedata FKs are -1 unless a code pool is defined, and data source FKs are 2.
   - For `fact_field_values` and `fact_contribution`, follow [Fact table conventions](#fact-table-conventions). Never fill every FK column with a real id. Generic `fact_*` tables are sparse too: anchor columns are real, one column per group is real, and the rest are -1.
   - For `dim_profile_only_*` tables, follow [Profile-only tables](#profile-only-tables).
   - Use 3 user profiles (known persons 1-3) and 10 known persons. Each profile gets the 16 settings rows from `UserProfileService.GetFieldIdentifiers()`: 171, 130, 160, 150, 110, 120, 140, 172, 180, 300, 400, 500, 550, 800, 600, 900.
   - Some tables must exceed "about 10 rows" so every profile and every profile-only row is complete. Record this in the plan.
   - When a hand-designed row leaves out a column:
     - FKs become -1 if NOT NULL, otherwise NULL.
     - Exception: in `dim_pid`, nullable FKs are -1 too, because `GetEmptyDimPid()` sets them that way.
   - Insert explicit ids under `SET IDENTITY_INSERT`. Guard every INSERT with `IF NOT EXISTS` on the PK so the script can be re-run.
   - Keep the data obviously fictitious: `example.org` URLs, `10.5555/...` DOIs and ORCID iDs like `0000-0000-0000-00NN`. Write non-ASCII letters as `N'...' + NCHAR(n)`.
   - Code scheme names that appear in `Constants.ReferenceDataCodeSchemes` are real. List every other scheme name or code as illustrative in the plan.
   - Before writing the SQL, simulate the insert sequence in the generator. Every non-NULL FK value must already exist as a sentinel, fixed or earlier row. Also check NOT NULL values, string lengths and PK uniqueness. This catches most bugs before the script reaches a database.
5. **Generate verification queries.**
   - Collect the results in `#seed_checks (check_name, failures)`. `PRINT` the `N/N checks passed` line and `RAISERROR` if any check fails, so `sqlcmd -b` exits non-zero.
   - Include:
     - an orphan check for every FK
     - a check that every FK is enabled and trusted (`sys.foreign_keys`)
     - exactly one -1 row per sentinel table
     - the fixed-row checks, profile-only checks and fact table checks below
   - Prove the checks can fail: break a few rules in the test database and confirm they are reported.
6. **Write `seed_test_data.sql` and `seed_plan.md` to the output folder.**
   - Wrap the inserts in a single transaction with `XACT_ABORT ON`. The verification section runs after `COMMIT`.
   - The plan covers:
     - inputs and parse counts
     - cycles and deferred edges
     - the insert-order table with row counts
     - sentinel defaults and the extra NOT NULL columns in the fixed rows
     - the test data design and deviations from "about 10 rows"
     - unknowns and assumptions
     - the verification result
   - Have the generator take a `--verification-log` path so the database result is embedded in the plan. After embedding, confirm the SQL file did not change.
7. **Run the script against a disposable database** (see [Disposable database recipe](#disposable-database-recipe)), then check that the SQL Server tables the API reads at startup exist. Report a missing table as `<insufficient data>`; do not invent tables.

## Disposable Database Recipe
- Image: `mcr.microsoft.com/mssql/server:2022-latest`. On Apple silicon, run it with `--platform linux/amd64`. There is no `sqlcmd` on the host; use `docker exec <name> /opt/mssql-tools18/bin/sqlcmd -C -b -I`.
- Generate a random SA password in the script and never print it. Poll `SELECT 1` until the server is up, then `CREATE DATABASE`.
- Apply a patched copy of the DDL from `$TMPDIR`: `DEFAULT false/true` -> `DEFAULT 0/1`. The original DDL file stays untouched.
- Run the seed script twice. The second run proves the `IF NOT EXISTS` guards work. Then remove the container.

## Ground Rules
- **Never put the source DDL inside the repository.** Read it from the path the user gives. Do not copy, move, rename or symlink it into the workspace, do not paste its contents into any repository file, and do not `git add` it. If a working copy is needed, put it outside the repository (for example under `$TMPDIR`). If the user's DDL path is already inside the repository, stop and ask them to move it out.
- The generator script must take the DDL path as an argument; never hardcode or embed DDL content in it. Generated outputs (`seed_test_data.sql`, `seed_plan.md`) may name the DDL file but must not reproduce DDL statements.
- Never edit the DDL file.
- Never invent table or column names. Mark unknowns as `<insufficient data>`.
- Use ASCII only and bracket every identifier in generated SQL.

## Fixed Data Source Rows

The API will not start without these rows. On startup, `StartupHelperService` looks up `dim_registered_data_source` by its organization's `name_fi`. The match is `'ORCID'` for ORCID and `'Tiedejatutkimus.fi'` for TTV (`Constants.OrganizationNames`). Copy the rows below exactly, with fixed ids, from the existing manual seed script:

```sql
SET IDENTITY_INSERT dim_organization ON;
INSERT INTO dim_organization (id, name_en, name_fi, name_sv, dim_sectorid, dim_registered_data_source_id, source_id) VALUES (1, 'ORCID', 'ORCID', 'ORCID', -1, -1, ' ');
INSERT INTO dim_organization (id, name_en, name_fi, name_sv, dim_sectorid, dim_registered_data_source_id, source_id) VALUES (2, 'Research.fi', 'Tiedejatutkimus.fi', 'Forskning.fi', -1, -1, ' ');
SET IDENTITY_INSERT dim_organization OFF;

SET IDENTITY_INSERT dim_registered_data_source ON;
INSERT INTO dim_registered_data_source (id, name, dim_organization_id, source_id) VALUES (1, 'ORCID', 1, ' ');
INSERT INTO dim_registered_data_source (id, name, dim_organization_id, source_id) VALUES (2, 'Tiedejatutkimus.fi', 2, ' ');
SET IDENTITY_INSERT dim_registered_data_source OFF;
```

- Insert them after the -1 sentinels and before any generated test row in either table. Otherwise the identity counter may already have handed out ids 1 and 2.
- The organizations keep `dim_registered_data_source_id = -1`. They need no cycle UPDATE, and their `dim_sectorid` is -1.
- If the DDL has more NOT NULL columns than the script lists, fill them with the sentinel defaults from Step 3 and record this in the plan. Bracket the identifiers, prefix literals with `N`, and guard each row with `IF NOT EXISTS (... WHERE [id] = <id>)`.
- Verification: rows 1 and 2 exist in both tables with the `name_fi` and `name` values above, and exactly one data source per name.
- Test data references these rows by literal id: **ORCID data source = 1, TTV data source = 2**.

## Profile-Only Tables

The ORCID import creates `dim_profile_only_publication`, `dim_profile_only_dataset`, `dim_profile_only_funding_decision` and `dim_profile_only_research_activity`. Their test rows must look like imported ORCID data:

- `dim_registered_data_source_id = 1` (ORCID) on every profile-only row and on its `fact_field_values` row.
- Each profile-only row gets its own ORCID put code in `dim_pid`, linked from `fact_field_values.dim_pid_id_orcid_put_code`. Set up the put-code `dim_pid` row like `UserProfileService.GetEmptyDimPid()` does:
  - `pid_type = N'ORCID put code'` (`Constants.PidTypes.ORCID_PUT_CODE`).
  - `pid_content` is the put code as a numeric string, for example `N'1000001'`, unique per item.
  - `dim_known_person_id` is the known person of the owning `dim_user_profile`.
  - Every other FK is -1, including the `dim_profile_only_*_id` columns. The import links the put code to the item only through `fact_field_values`.
  - `source_id = N'Profile API'`.
- The `fact_field_values` row has real values in `dim_user_profile_id`, `dim_field_display_settings_id`, `dim_registered_data_source_id = 1`, the `dim_profile_only_*_id` column and `dim_pid_id_orcid_put_code`. The remaining FKs are -1, except the secondary columns allowed under [Fact Table Conventions](#fact-table-conventions).
- Field identifiers: 550 for publications, 600 for datasets, 800 for funding decisions, 900 for research activities.
- Verification: every profile-only row has `dim_registered_data_source_id = 1`. Every `fact_field_values` row with a real `dim_profile_only_*_id` has `dim_registered_data_source_id = 1` and a `dim_pid_id_orcid_put_code` pointing at a `dim_pid` with `pid_type = 'ORCID put code'`. No two such rows share a put-code `dim_pid`.

## Fact Table Conventions

The DDL does not express these rules; they come from the Mydata API code (`aspnetcore/src/api`). In both fact tables every FK column is NOT NULL and part of the composite PK, yet a real row is sparse: a few FK columns hold real ids and **all others are -1**. Application queries filter with `<col> != -1` or `<col> > 0`, so real ids must be positive.

### fact_field_values

One row = one item in one user's profile. The app builds every row from `UserProfileService.GetEmptyFactFieldValue()` (all FKs -1, `show` = 0, `primary_value` = 0) and then sets only the columns below.

**Always real (never -1):**
- `dim_user_profile_id`
- `dim_field_display_settings_id`: a settings row of the **same** user profile whose `field_identifier` matches the link column (table below). Profile queries join on `field_identifier`; a mismatch makes the row invisible.
- `dim_registered_data_source_id`: 1 (ORCID) for ORCID-imported rows, including all profile-only rows. 2 (TTV) or the item's own source otherwise. See [Fixed Data Source Rows](#fixed-data-source-rows).

**Exactly one primary link column is real**, chosen by `field_identifier` (`Constants.FieldIdentifiers`):

| field_identifier | Primary link column |
|---|---|
| 110 PERSON_NAME, 120 PERSON_OTHER_NAMES | `dim_name_id` |
| 130 PERSON_EXTERNAL_IDENTIFIER | `dim_pid_id` |
| 140 PERSON_RESEARCHER_DESCRIPTION | `dim_researcher_description_id` |
| 150 PERSON_KEYWORD | `dim_keyword_id` |
| 171 PERSON_EMAIL_ADDRESS | `dim_email_addrress_id` |
| 172 PERSON_TELEPHONE_NUMBER | `dim_telephone_number_id` |
| 180 PERSON_WEB_LINK | `dim_web_link_id` |
| 300 ACTIVITY_AFFILIATION | `dim_affiliation_id` |
| 400 ACTIVITY_EDUCATION | `dim_education_id` |
| 500 ACTIVITY_PUBLICATION | `dim_publication_id` |
| 550 ACTIVITY_PUBLICATION_PROFILE_ONLY | `dim_profile_only_publication_id` |
| 600 ACTIVITY_RESEARCH_DATASET | `dim_research_dataset_id` or `dim_profile_only_dataset_id` |
| 800 ACTIVITY_FUNDING_DECISION | `dim_funding_decision_id` or `dim_profile_only_funding_decision_id` |
| 900 ACTIVITY_RESEARCH_ACTIVITY | `dim_research_activity_id` or `dim_profile_only_research_activity_id` |

The ORCID import stores profile-only datasets, funding decisions and research activities under 600/800/900, not 650/850/950. The API has no create path for 160 PERSON_FIELD_OF_SCIENCE (`dim_referencedata_field_of_science_id`); it only reads that column when deleting a profile. Record that mapping as `<insufficient data>` unless the user confirms it.

**Secondary columns.** Only these combinations occur next to the primary link:
- `dim_pid_id_orcid_put_code`: a separate `dim_pid` row holding the ORCID put code (not the same row as `dim_pid_id`). Set it only on ORCID-sourced rows for 120, 130, 150, 171 (when ORCID provides a put code), 180, 300, 400, 550, 600, 800 and 900. Keep it -1 for 110, 140 and all TTV-sourced rows.
- `dim_identifierless_data_id`: only with 300 (affiliation), and with profile-only 800 and 900. It points to `dim_identifierless_data` of type `organization_name` when the organization is not matched (the item's organization FK is -1), or to `organization_unit` for a department name. An `organization_unit` row can be the child of an `organization_name` row via `dim_identifierless_data.dim_identifierless_data_id`.
- `dim_referencedata_actor_role_id`: only with profile-only 800 (ORCID funding type: award, contract, grant, salary-award) and profile-only 900 (ORCID research activity type code scheme).
- The API never sets `dim_event_id`, `dim_competence_id`, `dim_research_community_id` or `dim_researcher_to_research_community_id`; it only reads them on profile delete. Keep them -1.

**Other rules:**
- `show` and `primary_value` are always 0 or 1, never NULL.
- Because the PK spans all FK columns, a given (profile, item) pair appears once. Spread the 10 test rows over different link types.
- Items linked through `dim_known_person_id` (email, web link, phone, description, affiliation, education, `dim_name`) belong to the profile's `dim_user_profile.dim_known_person_id`.

### fact_contribution

The API only reads this table. It is loaded by the data warehouse. Of the 15 FK/PK columns, unused ones are -1. Seed these patterns:

- **Person to output:** real `dim_name_id` and exactly one real column among `dim_publication_id`, `dim_research_dataset_id` and `dim_research_activity_id`. `TtvSqlService.GetSqlQuery_Select_FactContribution` inner-joins `dim_publication` on `dim_publication_id`, so dataset and activity rows depend on the `dim_publication(-1)` sentinel. The app does not read `contribution_type` for these rows, so its value here is `<insufficient data>`.
- **Co-publication:** include at least one child `dim_publication` whose self-reference `dim_publication_id` points to a parent publication, with a contribution row on the child. The app adds the parent to the profile.
- **Activity type:** `contribution_type = 'activity_type'`, a real `dim_research_activity_id`, and `dim_referencedata_actor_role_id` set to the activity type referencedata. The app does not read `dim_name_id` here; use -1.
- **Researcher role in activity:** `contribution_type = 'researcher_name_activity'` with real `dim_name_id`, `dim_research_activity_id` and `dim_referencedata_actor_role_id`. The role's parent (`dim_referencedata.dim_referencedata_id`) is the activity type.
- Funding decisions are not linked to people through `fact_contribution`. The app uses `br_participates_in_funding_group` (`dim_name_id`, `dim_funding_decisionid`).
- Profile creation includes a `dim_name` only when `dim_registered_data_source_id != -1` and the name links to a `dim_known_person` that has an ORCID `dim_pid`.

### Fact table verification checks
- `fact_field_values`: no always-real column is -1; exactly one primary link column is not -1; secondary columns appear only in the combinations above; the settings row's `field_identifier` matches the link column, and its `dim_user_profile_id` matches the row's.
- `fact_contribution`: every row with a real `dim_name_id` has exactly one real column among publication, dataset and activity; `activity_type` and `researcher_name_activity` rows have real `dim_research_activity_id` and `dim_referencedata_actor_role_id`.

## Completion Checks
- `git status --porcelain --ignored` shows no DDL file or DDL copy anywhere in the repository.
- Parsed counts match the DDL. Check tables and FKs against `grep -c` on the statements. Count columns independently, remembering that the last column of a PK-less table ends with `);`.
- Every parent row precedes its child in the script. The generator's insert simulation passes without deferred UPDATEs other than the documented cycle edges.
- Verification queries pass against a disposable database, the second run passes too, and a deliberately broken row makes them fail. If no database was available, report that limit instead.
- Both output files contain only ASCII (`LC_ALL=C grep -c '[^ -~]'` returns 0).
- Fact table rows are sparse (mostly -1) and pass the fact table verification checks.
- ORCID (id 1) and TTV (id 2) data sources exist; every profile-only row uses data source 1 and has its own ORCID put code in `dim_pid`.
