Status: Implemented
2026-09-24

# Public API data export: NuGet contracts package for the Public API application

Added a new packable class library project, `aspnetcore/src/api.PublicApiContracts/`
(package id `CSC.ResearchFi.Mydata.PublicApiContracts`), containing only the request/response DTOs
for the Public API integration endpoint, with no dependency on EF Core/ASP.NET Core/Dapper. This is
Phase 1: a mockup endpoint (`POST /api/publicapi`, `{ "username" }` → `{ "message": "Hello ..." }`)
to prove out the plumbing; the real `DimUserProfile`-derived contract is deferred to a future plan.

Distribution is manual for now: `dotnet pack` the contracts project and copy the `.nupkg` into the
Public API app's repo; version is bumped by hand in the csproj. Endpoint auth uses a shared-secret
`PUBLICAPITOKEN` header, mirroring the existing `ADMINTOKEN` pattern.

Files touched:
- `aspnetcore/mydata.sln` — added `api.PublicApiContracts` project.
- `aspnetcore/src/api.PublicApiContracts/` (new) — `api.PublicApiContracts.csproj`,
  `PublicApiHelloRequest.cs`, `PublicApiHelloResponse.cs`, `README.md`.
- `aspnetcore/src/api/api.csproj` — project reference to the new contracts library.
- `aspnetcore/src/api/Services/IPublicApiService.cs`, `PublicApiService.cs` — replaced placeholder
  `GetProfileDataForPublicApi` with `GetHelloMessage(string username)` returning
  `PublicApiHelloResponse`.
- `aspnetcore/src/api/Controllers/PublicApiController.cs` — implemented `POST /api/publicapi` with
  `PUBLICAPITOKEN` header check.
- `aspnetcore/src/api/README.md` — documented `PUBLICAPITOKEN` env var.
- `aspnetcore/src/api.Tests/api.Tests.csproj` — project reference to the new contracts library.
- `aspnetcore/src/api.Tests/Services_Tests/PublicApiServiceTest.cs` — added `GetHelloMessage` test.
- `aspnetcore/src/api.Tests/Services_Tests/PublicApiControllerTest.cs` (new) — auth gate tests
  (missing/invalid/valid `PUBLICAPITOKEN` header).

Verified: `dotnet build aspnetcore/mydata.sln` succeeds; all `PublicApi*` tests pass; `dotnet pack`
produces a `.nupkg` containing the DLL, XML docs, and README, with no extra dependencies.
