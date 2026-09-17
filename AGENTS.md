# AGENTS.md

Instructions for AI coding agents working in this repository.

## Repository overview

This repo contains **Research.fi Mydata**, a service letting researchers manage their public
researcher profile. It has two main components:

- `aspnetcore/` — the Mydata API: an ASP.NET Core (.NET 10) web API (SQL Server via EF Core +
  Dapper) that is the primary focus of most coding tasks. See
  [aspnetcore/src/api/README.md](aspnetcore/src/api/README.md) for environment variables.
- `keycloak/` — Keycloak customizations (custom Java SPI mapper under
  `keycloak/custom/researchfi.mapper`, plus local dev docker-compose setup and OpenShift
  deployment templates).

Deployment templates for OpenShift (Rahti2) live under `aspnetcore/openshift/` and
`keycloak/openshift/`.

## Working in `aspnetcore/`

- Solution file: `aspnetcore/mydata.sln`. Two projects: `src/api` (the application) and
  `src/api.Tests` (xUnit tests).
- Target framework: `net10.0`.
- Build: `dotnet build aspnetcore/mydata.sln` (or use the VS Code `build` task).
- Test: `dotnet test aspnetcore/mydata.sln` from repo root, or run the `api.Tests` project
  directly. Always run tests after changing anything under `src/api/Services` or `Models`.
- Run locally: `dotnet watch run --project aspnetcore/mydata.sln` (VS Code `watch` task), or
  `dotnet publish` (VS Code `publish` task). The app requires the environment variables
  documented in [aspnetcore/src/api/README.md](aspnetcore/src/api/README.md) (DB connection
  string, Keycloak, ORCID, Elasticsearch settings) to start correctly.

### Code layout (`aspnetcore/src/api/`)

- `Controllers/` — API endpoints (one controller per resource area, e.g.
  `ProfileDataController`, `OrcidController`, `SharingController`). Most inherit from
  `TtvControllerBase`/`TtvAdminControllerBase` for shared auth/user-context helpers.
- `Services/` — business logic, one service + interface pair per concern (e.g.
  `OrcidImportService`/`IOrcidImportService`). `Services/Profiledata/` holds per-entity services
  used to build/update the user's profile editor data (affiliations, publications, datasets,
  funding decisions, etc.).
- `Models/` — DTOs and EF Core entities, grouped by domain (`Ttv/` = the core data warehouse
  entity model, `ProfileEditor/`, `Api/`, `Orcid/`, `Elasticsearch/`, `Keycloak/`, `Aitta/`).
- `Background/` — hosted background task queue used for async work (e.g. ORCID sync).
- `ElasticsearchMapper/` — maps TTV entities to Elasticsearch documents.

### Testing conventions

- Tests live in `aspnetcore/src/api.Tests/Services_Tests/`, mirroring the `Services/` structure
  (`Services_Tests/Profiledata/` mirrors `Services/Profiledata/`).
- Many service tests use an in-memory SQLite `DbContext` rather than mocks. When adding entities
  to an EF Core `Include(...)` chain used by a service under test, be aware that **EF Core
  generates INNER JOINs for non-nullable FK columns**: sentinel rows (id = -1) must exist for
  every table in the chain or dedup/update logic will silently fail in tests. See
  `/memories/repo/orcid-import-service-test-infra.md` for the established pattern
  (`SeedRequiredData`, persistent `SqliteConnection`, `PRAGMA foreign_keys = OFF`).
- JSON fixtures for ORCID import tests live in `aspnetcore/src/api.Tests/Infrastructure/orcid_fixtures/`.
- After changing dedup/merge logic (e.g. publication DOI matching), add/extend regression tests
  in the corresponding `Services_Tests` file rather than only manual verification.

## CI

`.github/workflows/dotnet.yml` runs on every PR: `dotnet restore`, `dotnet build --no-restore`,
`dotnet test --no-build`, all with working directory `aspnetcore/`. Keep the build and full test
suite green before considering a change complete.

## `plans/` lifecycle

`plans/` holds design/implementation plans, one per non-trivial change. Keep it lean so agents
don't have to read stale, already-executed plans:

- Every plan starts with a `Status: Draft | In Progress | Implemented | Abandoned` line (+ date).
- Only `Draft`/`In Progress` plans live at the top level of `plans/`.
- Once a plan is fully implemented, replace it with a short summary (status/date, what changed
  and why, files touched — no phase-by-phase task lists or code snippets) and move it to
  `plans/done/`. The full original stays recoverable via git history under its old path.
- Durable, reusable lessons (gotchas, patterns worth remembering for future work) belong in
  `/memories/repo/*.md`, not in the plan doc itself.
- A plan with only some phases done (see e.g. a plan still tracking future phases) stays at the
  top level, uncompacted, until every phase is finished.

## Conventions

- Nullable reference type warnings (CS8632) are silenced repo-wide via `.editorconfig`; don't
  rely on that as license to ignore null-safety, just don't add `#nullable enable` pragmas to
  suppress it further.
- Prefer extending an existing service/controller/model file that matches the entity or concern
  over introducing new cross-cutting abstractions.
- Do not commit secrets. Configuration values (connection strings, API keys, tokens) are supplied
  via environment variables, never hardcoded — see the env var table in
  [aspnetcore/src/api/README.md](aspnetcore/src/api/README.md).
