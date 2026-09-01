# Plan: Use ORCID `last-modified-date` for `FactFieldValue.Modified` — Keywords (Phase 1)

**Date:** 2026-09-01
**Status:** Implemented

## Summary

Each ORCID keyword element carries a `last-modified-date` timestamp (Unix milliseconds).
The import service currently sets `FactFieldValue.Modified` and `DimKeyword.Modified` to the
wall-clock time of the import run (`currentDateTime`). This plan changes both fields to use the
item's own ORCID timestamp so the database reflects when the user actually changed the item in ORCID.

This is a **proof of concept**. If the approach works correctly for keywords, the same pattern
will be rolled out to all other ORCID item types (other names, researcher URLs, educations,
employments, fundings, research activities, etc.) in subsequent phases.

Phase 1 scope: ORCID keywords only.

### Clarifications resolved

| Question | Decision |
|---|---|
| Null `last-modified-date` fallback | Fall back to `currentDateTime` |
| Target fields | Both `DimKeyword.Modified` and `FactFieldValue.Modified` |
| Fixture timestamps | Arbitrary round numbers (readability) |
| Helper visibility | `internal` method on `OrcidJsonParserService` (for future reuse) |
| Test scope | Assert `FactFieldValue.Modified` only |

---

## Affected files

| File | Change |
|---|---|
| `Models/Orcid/OrcidKeyword.cs` | Add `DateTime? LastModifiedDate` property |
| `Services/OrcidJsonParserService.cs` | Add `internal` helper; extract timestamp in `GetKeywords()` |
| `Services/OrcidImportService.cs` | Use `keyword.LastModifiedDate ?? currentDateTime` in keyword loop |
| `Infrastructure/orcid_fixtures/keywords.json` | Set distinct known `last-modified-date` values |
| `Infrastructure/orcid_fixtures/keywords_updated.json` | Set later `last-modified-date` values |
| `Services_Tests/OrcidJsonParserServiceTest.cs` | Assert `LastModifiedDate` parsing; add null case test |
| `Services_Tests/OrcidImportServiceTest.cs` | Assert `FactFieldValue.Modified` equals ORCID timestamp on insert and update |

---

## Task 1 — Extend `OrcidKeyword` model

**File:** `aspnetcore/src/api/Models/Orcid/OrcidKeyword.cs`

Add an optional `LastModifiedDate` property and update the constructor with a defaulted parameter
so all existing call sites compile without changes.

```csharp
public partial class OrcidKeyword
{
    public OrcidKeyword(string value, OrcidPutCode putCode, DateTime? lastModifiedDate = null)
    {
        Value = value;
        PutCode = putCode;
        LastModifiedDate = lastModifiedDate;
    }

    public string Value { get; set; }
    public OrcidPutCode PutCode { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
```

**Acceptance criteria:** `OrcidKeyword` holds an optional `LastModifiedDate`. No call site
changes required.

---

## Task 2 — Add internal helper and parse timestamp in `OrcidJsonParserService`

**File:** `aspnetcore/src/api/Services/OrcidJsonParserService.cs`

### 2a — Internal helper `GetLastModifiedDateTime`

Add the helper with `internal` visibility so other parser code in the same assembly can reuse it
in future phases without duplicating logic.

```csharp
// Returns null when the element is absent or null; converts milliseconds to UTC DateTime.
internal static DateTime? GetLastModifiedDateTime(JsonElement element)
{
    if (!element.TryGetProperty("last-modified-date", out JsonElement lmdElement) ||
        lmdElement.ValueKind == JsonValueKind.Null)
        return null;

    if (!lmdElement.TryGetProperty("value", out JsonElement valueElement) ||
        valueElement.ValueKind == JsonValueKind.Null)
        return null;

    return DateTimeOffset.FromUnixTimeMilliseconds(valueElement.GetInt64()).UtcDateTime;
}
```

### 2b — Use helper in `GetKeywords()`

In the existing `foreach` that builds `OrcidKeyword` objects, pass the parsed timestamp:

```csharp
DateTime? lastModified = GetLastModifiedDateTime(element);
otherNames.Add(new OrcidKeyword(value, putCode, lastModified));
```

**Acceptance criteria:**
- `GetKeywords()` returns `OrcidKeyword` objects with `LastModifiedDate` set from JSON.
- `LastModifiedDate` is `null` when the field is absent or `null` in JSON.

---

## Task 3 — Use `LastModifiedDate` in `OrcidImportService`

**File:** `aspnetcore/src/api/Services/OrcidImportService.cs`

In the keyword loop (currently around lines 834–875), introduce a local variable for the
resolved timestamp and use it for both `DimKeyword.Modified` and `FactFieldValue.Modified`.

### Update branch

```csharp
// Before:
dimKeyword.Modified = currentDateTime;
factFieldValuesKeyword.Modified = currentDateTime;

// After:
DateTime keywordModified = keyword.LastModifiedDate ?? currentDateTime;
dimKeyword.Modified = keywordModified;
factFieldValuesKeyword.Modified = keywordModified;
```

### Create branch

```csharp
// Before:
Created = currentDateTime,
Modified = currentDateTime

// After:
DateTime keywordModified = keyword.LastModifiedDate ?? currentDateTime;
...
Created = currentDateTime,
Modified = keywordModified
```

**Acceptance criteria:**
- When `LastModifiedDate` is set: both `DimKeyword.Modified` and `FactFieldValue.Modified`
  equal the ORCID timestamp.
- When `LastModifiedDate` is `null`: both fall back to `currentDateTime`.

---

## Task 4 — Update fixture JSON files

Use readable round-number Unix millisecond timestamps.

| Fixture | Keyword put-code | `last-modified-date.value` | UTC equivalent |
|---|---|---|---|
| `keywords.json` | 4001 | `1700000000000` | 2023-11-14 22:13:20 |
| `keywords.json` | 4002 | `1710000000000` | 2024-03-09 16:00:00 |
| `keywords_updated.json` | 4001 | `1720000000000` | 2024-07-03 16:00:00 |
| `keywords_updated.json` | 4002 | `1730000000000` | 2024-10-27 08:00:00 |

Both fixtures already contain `"last-modified-date": { "value": 1000000000000 }` on every
keyword; replace those values with the table above.

**Acceptance criteria:** The two timestamps in `keywords.json` differ from each other and from
the timestamps in `keywords_updated.json`, enabling distinct assertions per keyword and per
import run.

---

## Task 5 — Update `OrcidJsonParserServiceTest`

**File:** `aspnetcore/src/api.Tests/Services_Tests/OrcidJsonParserServiceTest.cs`

### 5a — Extend `TestGetKeywords()`

The sandbox record (`orcidSandbox_0000-0002-9227-8514_record.json`) carries timestamps such as
`1477423527029` and `1477423527039`. Add assertions:

```csharp
Assert.Equal(
    DateTimeOffset.FromUnixTimeMilliseconds(1461170250189).UtcDateTime,
    actualKeywords[0].LastModifiedDate);
// ... repeat for other keywords
```

Use the actual values present in the sandbox fixture.

### 5b — New test `TestGetKeywords_NullLastModifiedDate()`

Inline a minimal JSON with `"last-modified-date": null` on a keyword and assert
`LastModifiedDate == null`.

**Acceptance criteria:** Parser tests cover the happy path (timestamp present) and the null case.

---

## Task 6 — Update `OrcidImportServiceTest`

**File:** `aspnetcore/src/api.Tests/Services_Tests/OrcidImportServiceTest.cs`

All new tests use `context.FactFieldValues` assertions only (not `DimKeyword.Modified`).

### 6a — New test: `Keywords_FactFieldValue_Modified_UsesOrcidTimestamp`

After a single import of `keywords.json`:

```csharp
var ffvs = context.FactFieldValues
    .Where(f => f.DimKeywordId > 0)
    .OrderBy(f => f.DimKeywordId)
    .ToList();

Assert.Equal(
    DateTimeOffset.FromUnixTimeMilliseconds(1700000000000).UtcDateTime,
    ffvs[0].Modified);
Assert.Equal(
    DateTimeOffset.FromUnixTimeMilliseconds(1710000000000).UtcDateTime,
    ffvs[1].Modified);
```

### 6b — Update `Keywords_Updated_WhenChangedInOrcid`

After the second import (`keywords_updated.json`), add assertions:

```csharp
var ffvs = context.FactFieldValues
    .Where(f => f.DimKeywordId > 0)
    .OrderBy(f => f.DimKeywordId)
    .ToList();

Assert.Equal(
    DateTimeOffset.FromUnixTimeMilliseconds(1720000000000).UtcDateTime,
    ffvs[0].Modified);
Assert.Equal(
    DateTimeOffset.FromUnixTimeMilliseconds(1730000000000).UtcDateTime,
    ffvs[1].Modified);
```

### 6c — New test: `Keywords_FactFieldValue_Modified_FallsBackToCurrentDateTime`

Use a variant of `keywords.json` with `"last-modified-date": null` on all keywords (either a
new fixture `keywords_no_timestamp.json` or inline JSON). Assert that `FactFieldValue.Modified`
is within a 5-second window of `DateTime.UtcNow`.

```csharp
var modified = context.FactFieldValues
    .Where(f => f.DimKeywordId > 0)
    .Select(f => f.Modified)
    .First();

Assert.NotNull(modified);
Assert.True((DateTime.UtcNow - modified.Value).TotalSeconds < 5);
```

**Acceptance criteria:** Tests cover timestamp propagation on insert, on update, and the
`currentDateTime` fallback.

---

## Implementation order

```
Task 1 → Task 2a → Task 2b → Task 4 → Task 5 → Task 3 → Task 6
```

Tasks 1 and 2a can be done in parallel. Task 4 (fixtures) must precede Task 6 (import tests).
Task 3 depends on Task 1. Task 5 depends on Tasks 1 and 2.

---

## Risks

- **Millisecond precision**: `DateTimeOffset.FromUnixTimeMilliseconds()` is exact. SQLite stores
  `DateTime` with millisecond precision; ORCID timestamps used in fixtures are whole-second
  multiples so no rounding loss occurs.
- **No interface changes**: `IOrcidJsonParserService` and `IOrcidImportService` are unchanged.
- **No schema changes**: `FactFieldValue.Modified` and `DimKeyword.Modified` already exist as
  `DateTime?`.
- **Scope creep**: This plan deliberately excludes all other item types; the `internal` helper
  is the only forward-looking element, and it adds no runtime cost.
