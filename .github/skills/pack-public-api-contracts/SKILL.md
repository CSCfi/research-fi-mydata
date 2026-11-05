---
name: pack-public-api-contracts
description: "Use when releasing or packing the Public API contracts NuGet package (CSC.ResearchFi.Mydata.PublicApiContracts): bump the version, run dotnet pack, and remove the old nupkg. Keywords: pack, bump version, nupkg, PublicApiContracts, publish contracts."
---

# Pack Public API contracts

Project: `aspnetcore/src/api.PublicApiContracts/api.PublicApiContracts.csproj`. Run all commands from the repository root.

## Steps

1. Read `<Version>` in the csproj and increment the patch number by 0.0.1 (e.g. `0.0.6` -> `0.0.7`). Always bump the patch, even for breaking changes. Edit only the `<Version>` element.
2. Run:
   ```
   dotnet pack aspnetcore/src/api.PublicApiContracts/api.PublicApiContracts.csproj -c Release
   ```
   Stop and report if the pack fails; do not delete anything in that case.
3. In `aspnetcore/src/api.PublicApiContracts/bin/Release/`, delete every `CSC.ResearchFi.Mydata.PublicApiContracts.*.nupkg` except the one for the new version. Keep the new package.
4. Report the old version, the new version, and which files were deleted.

## Notes

- Only delete `.nupkg` files in `bin/Release/`; leave `net10.0/` and other files alone.
- Do not commit or push the package.
