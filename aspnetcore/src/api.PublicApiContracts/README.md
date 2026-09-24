# CSC.ResearchFi.Mydata.PublicApiContracts

Request/response contract types for the Research.fi Mydata Public API integration endpoint
(`POST /api/publicapi` in the Mydata API). This backend is the source of truth for the contract;
this package only carries the plain data types, with no dependency on EF Core, ASP.NET Core, or
any other part of the Mydata API.

## Current contents (mockup phase)

- `PublicApiHelloRequest` — `{ "username": "..." }`
- `PublicApiHelloResponse` — `{ "message": "Hello ..." }`

The real, `DimUserProfile`-derived contract will replace/extend these types in a later phase.

## Consuming this package

There is currently no NuGet feed for this package. To consume it:

1. In the Mydata API repository, run:
   ```
   dotnet pack aspnetcore/src/api.PublicApiContracts/api.PublicApiContracts.csproj -c Release
   ```
2. Copy the produced `.nupkg` (under `aspnetcore/src/api.PublicApiContracts/bin/Release/`) into a
   local package source in the Public API app's repository.
3. Reference `CSC.ResearchFi.Mydata.PublicApiContracts` at the version printed in the `.nupkg`
   filename from the Public API app's project.

Bump `<Version>` in `api.PublicApiContracts.csproj` manually whenever the contract changes.
