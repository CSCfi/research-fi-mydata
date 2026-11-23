Status: Implemented
Date: 2026-11-23

ORCID keyword imports now use separate timestamp ownership:

- `DimKeyword.Created` is set once on insert and `DimKeyword.Modified` uses the import runtime clock.
- `FactFieldValue.Created` and `FactFieldValue.Modified` use the corresponding ORCID timestamps, with the import runtime clock as fallback.
- Existing `FactFieldValue.Created` values are refreshed from ORCID on update.

Files touched:
- `aspnetcore/src/api/Services/OrcidImportService.cs`
- `aspnetcore/src/api/Services/OrcidJsonParserService.cs`
- `aspnetcore/src/api.Tests/Services_Tests/OrcidImportServiceTest.cs`
- `aspnetcore/src/api.Tests/Infrastructure/orcid_fixtures/keywords.json`
- `aspnetcore/src/api.Tests/Infrastructure/orcid_fixtures/keywords_updated.json`
- `aspnetcore/src/api.Tests/Infrastructure/orcid_fixtures/keywords_null_timestamp.json`

Verification:
- 80/80 ORCID import service tests pass.
- 419/419 full API test suite tests pass.
