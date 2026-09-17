# .NET 8 → .NET 10 upgrade

**Status:** Implemented — August 2026 (305+ tests pass)

Bumped `api` and `api.Tests` `TargetFramework` to `net10.0` and updated all NuGet packages to
10.0.x-compatible versions. Replaced `Swashbuckle.AspNetCore` with `Microsoft.AspNetCore.OpenApi`.
Kept `IdentityModel.AspNetCore` (still actively used) and `NEST` 7.17.5 (net10.0-compatible;
migration to `Elastic.Clients.Elasticsearch` deferred as tracked tech debt). Updated Dockerfile
base images and the GitHub Actions workflow to `dotnet-version: 10.0.x`.

**Files changed:** `api.csproj`, `api.Tests.csproj`, `Startup.cs`,
`aspnetcore/openshift/api/rahti2/Dockerfile`, `.github/workflows/dotnet.yml`.
