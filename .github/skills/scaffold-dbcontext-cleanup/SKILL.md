---
name: scaffold-dbcontext-cleanup
description: "Clean up and verify the EF Core TtvContext and entities in aspnetcore/src/api/Models/Ttv after the user has run dotnet ef dbcontext scaffold against the dw_rih SQL Server database. Normalizes line endings to LF, separates line-ending noise from real changes, checks for credentials, builds, runs tests, and reports real schema changes. USE FOR: post-scaffold cleanup; reviewing what a scaffold run changed; checking the build after regeneration. DO NOT USE FOR: running the scaffold itself (the user runs it); handling or asking for the connection string; hand-editing entities (they are generated); changing the Services, Controllers, or ElasticsearchMapper code that consumes them."
---

# Scaffold DbContext Cleanup

Cleans up `aspnetcore/src/api/Models/Ttv` (`TtvContext.cs` and the entity files) after the user has regenerated it, so only real schema changes remain in the diff.

Model suitability: the Steps are mechanical and can run on a Haiku-class model. The schema review in Reporting needs a stronger model.

## Responsibilities

- The user runs the `dotnet ef dbcontext scaffold` command in their own terminal. The agent never runs it.
- The agent never asks for, prints, or stores the connection string or any password.
- If a password has appeared in a chat or terminal history, ask the user to rotate it before continuing.

## User step (for reference)

The user runs this from `aspnetcore/src/api` in their own terminal, after exporting `ConnectionStrings__DefaultConnection` in that same shell (the variable is listed in `aspnetcore/src/api/README.md`):

```
dotnet ef dbcontext scaffold Name=ConnectionStrings:DefaultConnection Microsoft.EntityFrameworkCore.SqlServer --output-dir Models/Ttv --force --context TtvContext --no-onconfiguring
```

`--no-onconfiguring` keeps the connection string out of `TtvContext.cs`. Without it, the scaffold writes the resolved connection string, password included, into `OnConfiguring`.

## Steps

Run every command from `aspnetcore/`.

0. Ask the user to confirm the scaffold command has finished successfully. If they have not run it, stop and wait. Do not run the scaffold.

1. Normalize line endings to LF. The root `.editorconfig` sets `end_of_line = lf`, and `.gitattributes` sets `*.cs text eol=lf`. The index still holds mixed CRLF/LF in most of these files, so this step makes them show as changed. Then compare the full diff with the `--ignore-cr-at-eol` diff. The difference is the line-ending-only files.

   ```
   perl -pi -e 's/\r\n/\n/' src/api/Models/Ttv/*.cs
   git status --short -- src/api/Models/Ttv
   git diff --stat -- src/api/Models/Ttv
   git diff --ignore-cr-at-eol --stat -- src/api/Models/Ttv
   ```

2. Confirm no credentials were written. Expect no output.

   ```
   grep -rn -i "password\|OnConfiguring" src/api/Models/Ttv || true
   ```

   If anything is printed, stop, report the file and line without repeating the matched value, and ask the user how to proceed.

3. Build the solution, as CI does.

   ```
   dotnet build mydata.sln /consoleloggerparameters:NoSummary
   ```

4. Run the tests. `AGENTS.md` requires this after changes under `src/api/Models`.

   ```
   dotnet test mydata.sln --no-build
   ```

## Reporting

- List the files with real changes from step 1, plus new (`??`) and deleted (`D`) files from `git status`. Do not list files that only changed line endings; report only their count. Those files are the LF normalization, so ask the user whether to commit them separately.
- The scaffold runs with `--force`, which overwrites hand edits in `Models/Ttv`. It may also leave entity files for tables that were dropped. Call out any such files and ask before removing or restoring anything.
- Call out schema changes that were not expected, such as new columns, indexes, or default constraints. Ask the user to confirm them before they are committed.
- Report the build and test results.
- Do not commit until the user confirms the schema changes.

