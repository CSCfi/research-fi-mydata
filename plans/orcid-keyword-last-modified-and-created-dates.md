# Plan: Use ORCID `last-modified-date` / `created-date` for `FactFieldValue.Modified` / `Created` — Keywords (Phase 1)

**Date:** 2026-09-01
**Status:** Implemented

## Summary

Each ORCID keyword element carries a `last-modified-date` and a `created-date` timestamp
(Unix milliseconds). The import service currently sets `FactFieldValue.Modified` / `Created`
and `DimKeyword.Modified` / `Created` to the wall-clock time of the import run
(`currentDateTime`). This plan changes all four fields to use the item's own ORCID timestamps
so the database reflects when the user actually created or changed the item in ORCID.

The handling logic for `Created` / `created-date` is identical to `Modified` /
`last-modified-date`: extract the millisecond Unix timestamp from JSON, convert to UTC
`DateTime`, fall back to `currentDateTime` when the field is absent or null.

This is a **proof of concept**. If the approach works correctly for keywords, the same pattern
will be rolled out to all other ORCID item types (other names, researcher URLs, educations,
employments, fundings, research activities, etc.) in subsequent phases.

Phase 1 scope: ORCID keywords only.

### Clarifications resolved

| Question | Decision |
|---|---|
| Null `last-modified-date` fallback | Fall back to `currentDateTime` |
| Null `created-date` fallback | Fall back to `currentDateTime` (same rule) |
| Target fields | `DimKeyword.Modified`, `FactFieldValue.Modified`, `DimKeyword.Created`, `FactFieldValue.Created` |
| Fixture timestamps | Arbitrary round numbers (readability) |
| Helper visibility | `internal` method on `OrcidJsonParserService` (for future reuse) |
| Test scope | Assert `FactFieldValue.Modified` and `FactFieldValue.Created` |

---

## Affected files

| File | Change |
|---|---|
| `Models/Orcid/OrcidKeyword.cs` | Add `DateTime? LastModifiedDate` and `DateTime? CreatedDate` properties |
| `Services/OrcidJsonParserService.cs` | Generalize helper to accept property name; extract both timestamps in `GetKeywords()` |
| `Services/OrcidImportService.cs` | Use ORCID timestamps (with fallback) for both `Modified` and `Created` in keyword loop |
| `Infrastructure/orcid_fixtures/keywords.json` | Set distinct known `last-modified-date` and `created-date` values |
| `Infrastructure/orcid_fixtures/keywords_updated.json` | Set later `last-modified-date` and `created-date` values |
| `Services_Tests/OrcidJsonParserServiceTest.cs` | Assert `LastModifiedDate` and `CreatedDate` parsing; add null case tests |
| `Services_Tests/OrcidImportServiceTest.cs` | Assert `FactFieldValue.Modified` and `FactFieldValue.Created` on insert and update |

---

## Task 1 — Extend `OrcidKeyword` model *(Implemented)*

**File:** `aspnetcore/src/api/Models/Orcid/OrcidKeyword.cs`

Add optional `LastModifiedDate` and `CreatedDate` properties; update the constructor with
defaulted parameters so all existing call sites compile without changes.

```csharp
public partial class OrcidKeyword
{
    public OrcidKeyword(string value, OrcidPutCode putCode,
        DateTime? lastModifiedDate = null, DateTime? createdDate = null)
    {
        Value = value;
        PutCode = putCode;
        LastModifiedDate = lastModifiedDate;
        CreatedDate = createdDate;
    }

    public string Value { get; set; }
    public OrcidPutCode PutCode { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public DateTime? CreatedDate { get; set; }
}
```

**Acceptance criteria:** `OrcidKeyword` holds both optional timestamps. No call site changes
required.

---

## Task 2 — Add internal helper and parse timestamps in `OrcidJsonParserService` *(Implemented)*

**File:** `aspnetcore/src/api/Services/OrcidJsonParserService.cs`

### 2a — Generalize helper

The existing `GetLastModifiedDateTime` reads a hard-coded property name. Generalize it (or add
a sibling) so both `last-modified-date` and `created-date` can be read without duplicating logic.
Preferred approach: rename to `GetOrcidDateTime(JsonElement element, string propertyName)` and
update the existing call site.

```csharp
// Returns null when the field is absent or null; converts milliseconds to UTC DateTime.
internal static DateTime? GetOrcidDateTime(JsonElement element, string propertyName)
{
    if (!element.TryGetProperty(propertyName, out JsonElement dateElement) ||
        dateElement.ValueKind == JsonValueKind.Null)
        return null;

    if (!dateElement.TryGetProperty("value", out JsonElement valueElement) ||
        valueElement.ValueKind == JsonValueKind.Null)
        return null;

    return DateTimeOffset.FromUnixTimeMilliseconds(valueElement.GetInt64()).UtcDateTime;
}
```

### 2b — Use helper in `GetKeywords()`

```csharp
keywords.Add(new OrcidKeyword(
    value: element.GetProperty("content").GetString(),
    putCode: this.GetOrcidPutCode(element),
    lastModifiedDate: GetOrcidDateTime(element, "last-modified-date"),
    createdDate: GetOrcidDateTime(element, "created-date")
));
```

**Acceptance criteria:**
- `GetKeywords()` returns `OrcidKeyword` objects with both timestamps set from JSON.
- Each is `null` when the respective field is absent or `null` in JSON.

---

## Task 3 — Use timestamps in `OrcidImportService` *(Implemented)*

**File:** `aspnetcore/src/api/Services/OrcidImportService.cs`

In the keyword loop, introduce local variables for both resolved timestamps and apply them to
all four target fields.

### Update branch

```csharp
DateTime keywordModified = keyword.LastModifiedDate ?? currentDateTime;
DateTime keywordCreated  = keyword.CreatedDate      ?? currentDateTime;
dimKeyword.Modified = keywordModified;
dimKeyword.Created  = keywordCreated;   // add this line
factFieldValuesKeyword.Modified = keywordModified;
factFieldValuesKeyword.Created  = keywordCreated;  // add this line
```

### Create branch

```csharp
DateTime keywordModified = keyword.LastModifiedDate ?? currentDateTime;
DateTime keywordCreated  = keyword.CreatedDate      ?? currentDateTime;
DimKeyword dimKeyword = new()
{
    ...
    Created  = keywordCreated,
    Modified = keywordModified
};
...
factFieldValuesKeyword.Modified = keywordModified;
factFieldValuesKeyword.Created  = keywordCreated;  // add this line
```

**Acceptance criteria:**
- When timestamps are set: all four DB fields equal the ORCID timestamps.
- When timestamps are `null`: all four fall back to `currentDateTime`.

---

## Task 4 — Update fixture JSON files *(Implemented)*

Use readable round-number Unix millisecond timestamps.

| Fixture | Keyword put-code | `last-modified-date.value` | `created-date.value` |
|---|---|---|---|
| `keywords.json` | 4001 | `1700000000000` (done) | `1690000000000` |
| `keywords.json` | 4002 | `1710000000000` (done) | `1691000000000` |
| `keywords_updated.json` | 4001 | `1720000000000` (done) | `1690000000000` |
| `keywords_updated.json` | 4002 | `1730000000000` (done) | `1691000000000` |
| `keywords_null_timestamp.json` | 4001 | `null` (done) | `null` |
| `keywords_null_timestamp.json` | 4002 | `null` (done) | `null` |

The `created-date` values in the updated fixture intentionally match the original fixture because
creation timestamps do not change on update — only `last-modified-date` advances.

**Acceptance criteria:** Both timestamp fields are distinct and predictable across all fixtures.

---

## Task 5 — Update `OrcidJsonParserServiceTest` *(Implemented)*

**File:** `aspnetcore/src/api.Tests/Services_Tests/OrcidJsonParserServiceTest.cs`

### 5a — Extend `TestGetKeywords()`

The sandbox record carries both timestamps. Add assertions for `CreatedDate` alongside the
existing `LastModifiedDate` assertions.

Sandbox record timestamps for reference:

| put-code | `last-modified-date.value` | `created-date.value` |
|---|---|---|
| 4504 | `1477423527029` | `1461170250189` |
| 4603 | `1477423527037` | `1477423515476` |
| 4604 | `1477423527039` | `1477423527039` |

### 5b — New test `TestGetKeywords_NullLastModifiedDate()`

Verifies `LastModifiedDate == null` when the field is absent/null.

### 5c — New test `TestGetKeywords_NullCreatedDate()`

Same pattern as 5b but for `CreatedDate`.

**Acceptance criteria:** Parser tests cover both timestamps in the happy path and the null case.

---

## Task 6 — Update `OrcidImportServiceTest` *(Implemented)*

**File:** `aspnetcore/src/api.Tests/Services_Tests/OrcidImportServiceTest.cs`

### 6a — `Keywords_FactFieldValue_Modified_UsesOrcidTimestamp`

Asserts `FactFieldValue.Modified` equals the ORCID `last-modified-date` on initial import.

### 6b — `Keywords_Updated_WhenChangedInOrcid`

Asserts `FactFieldValue.Modified` equals the updated ORCID `last-modified-date` after re-import.

### 6c — `Keywords_FactFieldValue_Modified_FallsBackToCurrentDateTime`

Asserts fallback to `currentDateTime` when `last-modified-date` is null.

### 6d — `Keywords_FactFieldValue_Created_UsesOrcidTimestamp`

After a single import of `keywords.json`, assert:

```csharp
Assert.Equal(
    DateTimeOffset.FromUnixTimeMilliseconds(1690000000000).UtcDateTime,
    ffvs[0].Created);
Assert.Equal(
    DateTimeOffset.FromUnixTimeMilliseconds(1691000000000).UtcDateTime,
    ffvs[1].Created);
```

### 6e — `Keywords_FactFieldValue_Created_FallsBackToCurrentDateTime`

Same pattern as 6c but for `FactFieldValue.Created`, using `keywords_null_timestamp.json`.

**Acceptance criteria:** All four DB fields are covered by tests.

---

## Implementation order

```
All tasks complete.
```

Tasks 1 and 2 can be done in parallel. All tasks are complete.

---

## Risks

- **Millisecond precision**: `DateTimeOffset.FromUnixTimeMilliseconds()` is exact. SQLite stores
  `DateTime` with millisecond precision; ORCID timestamps used in fixtures are whole-second
  multiples so no rounding loss occurs.
- **Helper rename**: `GetLastModifiedDateTime` must be renamed to `GetOrcidDateTime` (or a new
  overload added). The existing call site in `GetKeywords()` must be updated to pass the
  property name explicitly.
- **No interface changes**: `IOrcidJsonParserService` and `IOrcidImportService` are unchanged.
- **No schema changes**: `FactFieldValue.Modified`, `FactFieldValue.Created`,
  `DimKeyword.Modified`, and `DimKeyword.Created` already exist as `DateTime?`.
- **Scope creep**: This plan deliberately excludes all other item types; the generalized helper
  is the only forward-looking element, and it adds no runtime cost.
