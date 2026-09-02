# Plan Revision: ORCID timestamps for FactFieldValue only, restore DimKeyword import-time timestamps

**Date:** 2026-09-02
**Status:** Implemented

## Summary

This plan revises the previous keyword timestamp approach.

Correct target behavior:
- `FactFieldValue.Created` and `FactFieldValue.Modified` are sourced from ORCID keyword timestamps (`created-date`, `last-modified-date`).
- `DimKeyword.Created` is set once to `currentDateTime` when the row is first created.
- `DimKeyword.Modified` is set to `currentDateTime` on updates.

This restores `DimKeyword` to original import-time tracking while keeping ORCID-origin timestamps in `FactFieldValue`.

Scope remains ORCID keywords only.

---

## Field behavior matrix

| Entity field | Insert | Update | Source |
|---|---|---|---|
| `DimKeyword.Created` | Set to `currentDateTime` once | Never overwritten | Import runtime |
| `DimKeyword.Modified` | Set to `currentDateTime` | Set to `currentDateTime` | Import runtime |
| `FactFieldValue.Created` | Set from ORCID `created-date`, falling back to `currentDateTime` | Refresh from ORCID `created-date`, falling back to `currentDateTime` | ORCID |
| `FactFieldValue.Modified` | Set from ORCID `last-modified-date`, falling back to `currentDateTime` | Set from ORCID `last-modified-date`, falling back to `currentDateTime` | ORCID |

---

## Affected files

| File | Change |
|---|---|
| `aspnetcore/src/api/Services/OrcidImportService.cs` | Restore `DimKeyword` timestamp behavior to runtime clock; keep ORCID mapping only for `FactFieldValue` fields |
| `aspnetcore/src/api.Tests/Services_Tests/OrcidImportServiceTest.cs` | Update assertions to enforce split behavior between `DimKeyword` and `FactFieldValue` |
| `aspnetcore/src/api.Tests/Infrastructure/orcid_fixtures/keywords*.json` | Keep ORCID timestamp fixtures for `FactFieldValue` assertions |

No changes are required for parsing model and parser helper if `OrcidKeyword.CreatedDate` and `OrcidKeyword.LastModifiedDate` are already available.

---

## Implementation tasks

## Task 1 - Adjust timestamp assignment in keyword import flow

**File:** `aspnetcore/src/api/Services/OrcidImportService.cs`

### Update existing keyword branch

Apply:
- `dimKeyword.Modified = currentDateTime`
- Do not update `dimKeyword.Created`
- `factFieldValuesKeyword.Modified = keyword.LastModifiedDate ?? currentDateTime`
- `factFieldValuesKeyword.Created = keyword.CreatedDate ?? currentDateTime`

### Create new keyword branch

Apply:
- `dimKeyword.Created = currentDateTime`
- `dimKeyword.Modified = currentDateTime`
- `factFieldValuesKeyword.Created = keyword.CreatedDate ?? currentDateTime`
- `factFieldValuesKeyword.Modified = keyword.LastModifiedDate ?? currentDateTime`

**Acceptance criteria:**
- `DimKeyword` timestamps no longer come from ORCID.
- `FactFieldValue` timestamps come from their respective ORCID fields, falling back to `currentDateTime` when absent or null.
- An existing `FactFieldValue.Created` is refreshed from ORCID `created-date` on every import.

---

## Task 2 - Update tests for split timestamp ownership

**File:** `aspnetcore/src/api.Tests/Services_Tests/OrcidImportServiceTest.cs`

Add or update tests:
1. `Keywords_DimKeyword_Created_SetToCurrentDateTime_OnInsert`
2. `Keywords_DimKeyword_Created_NotOverwritten_OnUpdate`
3. `Keywords_DimKeyword_Modified_SetToCurrentDateTime_OnUpdate`
4. `Keywords_FactFieldValue_Modified_UsesOrcidLastModified`
5. `Keywords_FactFieldValue_Created_UsesOrcidCreated`

Include null timestamp tests that assert `FactFieldValue` falls back to `currentDateTime`.

**Acceptance criteria:**
- Tests explicitly verify the separation of concerns:
  - Runtime clock for `DimKeyword`
  - ORCID timestamps for `FactFieldValue`

---

## Task 3 - Validate fixtures and expected values

**Files:**
- `aspnetcore/src/api.Tests/Infrastructure/orcid_fixtures/keywords.json`
- `aspnetcore/src/api.Tests/Infrastructure/orcid_fixtures/keywords_updated.json`
- `aspnetcore/src/api.Tests/Infrastructure/orcid_fixtures/keywords_null_timestamp.json`

Ensure fixture values support all assertions:
- Distinct `created-date` and `last-modified-date`
- Monotonic `last-modified-date` across updated payloads
- Stable `created-date` across updated payloads

**Acceptance criteria:** fixtures are deterministic and aligned with test intent.

---

## Testing and verification

Run:
- Targeted tests for ORCID import service timestamp behavior
- Full API test suite if targeted tests pass

Expected outcome:
- No regression in keyword import
- All timestamp ownership rules validated

---

## Risks

- Existing tests may assert old behavior and require coordinated updates.

---

## Confirmed decisions

1. A missing or null ORCID timestamp falls back to `currentDateTime` for the corresponding `FactFieldValue` field.
2. On every update, `FactFieldValue.Created` is refreshed from ORCID `created-date`.
3. On insert, both `DimKeyword.Created` and `DimKeyword.Modified` are set to `currentDateTime`.
4. This revision is limited to keywords.
