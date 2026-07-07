# OrcidImportService – Performance Improvement Plan

**File:** `aspnetcore/src/api/Services/OrcidImportService.cs`  
**Date:** 2026-07-07  
**Goal:** Measurable database round-trip reduction and maintainability gains, with minimal risk to proven production logic. Changes are broken into independent phases that can be reviewed, tested, and deployed separately.

---

## Executive Summary

The service is functionally correct and well-tested. The main performance problems are:

| # | Problem | Severity | Effort |
|---|---------|----------|--------|
| 1 | ORCID JSON parsed twice for 4 data types | Medium | Low |
| 2 | 4 separate `SaveChangesAsync` calls in `AddDimDates` | Low | Low |
| 3 | N+1 DimDate DB queries in `AddDimDates` (one per item per date field) | High | Medium |
| 4 | N+1 DimDate DB queries in main import body (same dates queried again after `AddDimDates`) | High | Low |
| 5 | N+1 organisation disambiguation queries in employment/funding/research-activity loops | High | Medium |
| 6 | 4 sequential `DimReferencedata` queries for funding types (parallelisable) | Low | Low |
| 7 | Per-item `DimReferencedata` query inside research-activity "create new" branch | Medium | Medium |
| 8 | Dead code: `processedKeywordFactFieldValues` list is populated but never consumed | Low | Trivial |
| 9 | `AsNoTracking()` missing on read-only DimDate lookups in main import body | Low | Trivial |
| 10 | 2000-line monolithic method; hard to maintain and unit-test at granular level | Medium | High |

---

## Phase 1 – Trivial Quick Wins (minimal diff, zero logic change)

**Target:** Zero regressions, purely mechanical edits. Can be done in a single PR.

### 1.1 Remove dead code: `processedKeywordFactFieldValues`

`processedKeywordFactFieldValues` (line ~858) is a `List<FactFieldValue>` that is populated during keyword processing but is **never read afterwards**. The removal logic uses `orcidImportHelper.dimKeywordIds` instead.

**Change:** Delete the list declaration and the `.Add(...)` call inside the keyword loop.

**Risk:** None. Existing tests cover keyword import/removal.

---

### 1.2 Merge the 4 `SaveChangesAsync` calls in `AddDimDates` into one

`AddDimDates` calls `SaveChangesAsync` four times – once after each section (education, employment, funding, research activities). Since none of the sections depend on the persisted IDs of a previous section, all four batches can be saved in a single call.

**Change:** Remove the three intermediate `try/finally` blocks and keep only a single `SaveChangesAsync` at the very end of the method.

```csharp
// BEFORE: 4 × try { ... } finally { AutoDetect = true; await SaveChangesAsync(); }

// AFTER: 4 × _ttvContext.ChangeTracker.AutoDetectChangesEnabled = false;
//             foreach (...) { ... }
//             _ttvContext.ChangeTracker.AutoDetectChangesEnabled = true;
//        ...all four sections...
//        await _ttvContext.SaveChangesAsync();  // single call at the end
```

**Risk:** Very low. `AddDimDates_DoesNotDuplicateDates` and `AddDimDates_CreatesDatesFromFixture` tests will catch any regression.

---

### 1.3 Add `AsNoTracking()` to DimDate lookups in the main import body

Inside `ImportOrcidRecordJsonIntoUserProfile`, dates are retrieved with:

```csharp
DimDate educationStartDate = await _ttvContext.DimDates
    .FirstOrDefaultAsync(dd => dd.Year == ... && dd.Month == ... && dd.Day == ...);
```

These dates are **only read** (assigned to navigation properties), never modified. Adding `.AsNoTracking()` tells EF Core not to snapshot these entities, which reduces both memory pressure and CPU overhead from change detection.

**Change:** Add `.AsNoTracking()` to every `_ttvContext.DimDates.FirstOrDefaultAsync(...)` call in the main import body (there are approximately 10 such calls across the education, employment, funding, and research-activity loops).

**Risk:** None. Navigation properties set on tracked entities work identically with `AsNoTracking` objects when the related entity is just being set as a reference.

---

### 1.4 Parallelise the 4 funding reference-data queries

Before the funding loop, the service fires 4 sequential `await` queries:

```csharp
DimReferencedatum dimReferencedata_award    = await _ttvContext.DimReferencedata.Where(...AWARD...).AsNoTracking().FirstOrDefaultAsync();
DimReferencedatum dimReferencedata_contract = await _ttvContext.DimReferencedata.Where(...CONTRACT...).AsNoTracking().FirstOrDefaultAsync();
DimReferencedatum dimReferencedata_grant    = await _ttvContext.DimReferencedata.Where(...GRANT...).AsNoTracking().FirstOrDefaultAsync();
DimReferencedatum dimReferencedata_salaryAward = await _ttvContext.DimReferencedata.Where(...SALARY_AWARD...).AsNoTracking().FirstOrDefaultAsync();
```

These are independent reads; they can be replaced by a single query that fetches all four rows in one round trip:

```csharp
var codeValues = new[]
{
    Constants.OrcidFundingType_To_ReferenceDataCodeValue.AWARD,
    Constants.OrcidFundingType_To_ReferenceDataCodeValue.CONTRACT,
    Constants.OrcidFundingType_To_ReferenceDataCodeValue.GRANT,
    Constants.OrcidFundingType_To_ReferenceDataCodeValue.SALARY_AWARD
};
var fundingRefData = await _ttvContext.DimReferencedata
    .Where(dr => dr.CodeScheme == Constants.ReferenceDataCodeSchemes.ORCID_FUNDING
              && codeValues.Contains(dr.CodeValue))
    .AsNoTracking()
    .ToListAsync();

var dimReferencedata_award       = fundingRefData.First(r => r.CodeValue == Constants.OrcidFundingType_To_ReferenceDataCodeValue.AWARD);
var dimReferencedata_contract    = fundingRefData.First(r => r.CodeValue == Constants.OrcidFundingType_To_ReferenceDataCodeValue.CONTRACT);
var dimReferencedata_grant       = fundingRefData.First(r => r.CodeValue == Constants.OrcidFundingType_To_ReferenceDataCodeValue.GRANT);
var dimReferencedata_salaryAward = fundingRefData.First(r => r.CodeValue == Constants.OrcidFundingType_To_ReferenceDataCodeValue.SALARY_AWARD);
```

4 DB round trips → 1.

**Risk:** None. The downstream `switch` statement is unchanged.

---

## Phase 2 – Eliminate Duplicate JSON Parsing

**Target:** Parse each ORCID JSON section exactly once per import call.

### 2.1 Context

`AddDimDates` parses **5 sections** of the ORCID JSON:

| Parser call | Line in `AddDimDates` |
|---|---|
| `GetEducations` | ~55 |
| `GetEmployments` | ~125 |
| `GetFundings` | ~194 |
| `GetProfileOnlyResearchActivityItems` | ~263 |
| `GetWorks(processOnlyResearchActivities: true)` | ~279 |

`ImportOrcidRecordJsonIntoUserProfile` then parses **all of them again** plus `GetWorks` (for publications/datasets/research-activities). For a large ORCID record with many items this is 5+ full JSON deserialisation passes.

### 2.2 Approach

Add an overload of `AddDimDates` that accepts pre-parsed model lists:

```csharp
// New internal/private overload
internal async Task AddDimDates(
    List<OrcidEducation>         educations,
    List<OrcidEmployment>        employments,
    List<OrcidFunding>           fundings,
    List<OrcidResearchActivity>  researchActivities,
    DateTime currentDateTime,
    LogUserIdentification logUserIdentification)
```

The existing public `AddDimDates(string orcidRecordJson, ...)` signature — which is called directly by tests — becomes a thin wrapper that parses and delegates:

```csharp
public async Task AddDimDates(string orcidRecordJson, DateTime currentDateTime, LogUserIdentification logUserIdentification)
{
    var educations    = ParseSafe(() => _orcidJsonParserService.GetEducations(orcidRecordJson), ...);
    var employments   = ParseSafe(() => _orcidJsonParserService.GetEmployments(orcidRecordJson), ...);
    var fundings      = ParseSafe(() => _orcidJsonParserService.GetFundings(orcidRecordJson), ...);
    var activities    = ParseSafe(() => _orcidJsonParserService.GetProfileOnlyResearchActivityItems(orcidRecordJson), ...);
    var workActivities= ParseSafe(() => _orcidJsonParserService.GetWorks(orcidRecordJson, processOnlyResearchActivities: true).ResearchActivities, ...);
    await AddDimDates(educations, employments, fundings, activities.Concat(workActivities).ToList(), currentDateTime, logUserIdentification);
}
```

`ImportOrcidRecordJsonIntoUserProfile` then:
1. Parses all sections once at the top.
2. Passes already-parsed lists into the internal overload.
3. Reuses the same parsed lists for the remainder of the method.

**Tests impacted:** `AddDimDates` public signature is unchanged; existing tests continue to pass. The new internal overload can be tested independently if desired.

**Risk:** Low. The parsing logic is not changed, only when it happens.

---

## Phase 3 – Batch DimDate Lookups (Eliminate N+1 in `AddDimDates`)

**Target:** Replace individual per-item DimDate `FirstOrDefaultAsync` calls with a single batch query.

### 3.1 Problem

For every education, employment, funding, and research activity `AddDimDates` fires two queries:

```
SELECT TOP 1 * FROM dim_date WHERE year=? AND month=? AND day=?   -- start date
SELECT TOP 1 * FROM dim_date WHERE year=? AND month=? AND day=?   -- end date
```

A profile with 30 educations + 20 employments + 10 fundings + 15 activities = **150 round trips** just to check/insert dates.

### 3.2 Approach

**Step A:** Collect all unique `(Year, Month, Day)` tuples from all sections before any DB access.

**Step B:** Load all already-existing rows in **one query** using:

```csharp
// Build set of required tuples
var required = educations.SelectMany(e => new[] {
    (e.StartDate.Year, e.StartDate.Month, e.StartDate.Day),
    (e.EndDate.Year,   e.EndDate.Month,   e.EndDate.Day)
})
// ... concat employments, fundings, activities ...
.Distinct()
.ToList();

// Single batch read
var years  = required.Select(t => t.Item1).Distinct().ToList();
var existing = await _ttvContext.DimDates
    .Where(dd => years.Contains(dd.Year))
    .AsNoTracking()
    .ToListAsync();

// Index into a dictionary for O(1) lookup
var existingDict = existing
    .ToDictionary(dd => (dd.Year, dd.Month, dd.Day));
```

**Step C:** For each required tuple, check the dictionary. If missing, add a new `DimDate` to the context (no per-item DB query needed).

**Step D:** Single `SaveChangesAsync` at the end.

Result: **N round trips → 2** (one bulk read, one bulk write).

**Caveats:** The pre-filter by `years.Contains(dd.Year)` is not a perfect tuple-match but is a good pre-filter that dramatically reduces the result set. A secondary in-memory check on month/day completes the match. Alternatively, if the DB supports row value constructors, a more precise query could be used — but the in-memory approach is safe and portable.

**Risk:** Medium. This changes the internal algorithm of `AddDimDates`. The existing `AddDimDates_CreatesDatesFromFixture` and `AddDimDates_DoesNotDuplicateDates` tests must continue to pass.

---

## Phase 4 – Pre-load DimDates in Main Import Body (Eliminate N+1 After `AddDimDates`)

**Target:** Replace the per-item `DimDates.FirstOrDefaultAsync` calls in the main import body with a dictionary that is loaded once.

### 4.1 Problem

In `ImportOrcidRecordJsonIntoUserProfile`, after `AddDimDates` has already ensured all needed date rows exist, the code still queries each date individually inside loops:

```csharp
// Line ~1108 - inside education loop
DimDate educationStartDate = await _ttvContext.DimDates
    .FirstOrDefaultAsync(dd => dd.Year == education.StartDate.Year && ...);
```

This pattern repeats for employment, funding, and research activity loops — approximately **2 × (numEducations + numEmployments + numFundings + numActivities)** additional round trips.

### 4.2 Approach

After `AddDimDates` returns and before the main processing begins, build a lookup dictionary:

```csharp
// All dates used by this import are now guaranteed to exist.
// Load them all into a single dictionary for O(1) lookup.
var neededYears = GetAllNeededYears(parsedEducations, parsedEmployments, parsedFundings, parsedActivities);
var allDates = await _ttvContext.DimDates
    .Where(dd => neededYears.Contains(dd.Year))
    .AsNoTracking()
    .ToDictionaryAsync(dd => (dd.Year, dd.Month, dd.Day));

// Helper used in loops:
DimDate GetDate(OrcidDate d) => allDates.TryGetValue((d.Year, d.Month, d.Day), out var dd) ? dd : null;
```

Replace every `await _ttvContext.DimDates.FirstOrDefaultAsync(...)` in the loops with a dictionary lookup.

This phase is **only meaningful when combined with Phase 3** (which ensures the pre-filter by year is efficient) and **Phase 2** (which makes `parsedEducations` etc. available without re-parsing).

**Risk:** Low once Phase 2 and 3 are in place. The dictionary lookup is semantically identical to a DB query when all rows are guaranteed to exist.

---

## Phase 5 – Cache Organisation Disambiguation Lookups

**Target:** Avoid repeated DB queries for the same organisation within a single import call.

### 5.1 Problem

`FindOrganizationIdByOrcidDisambiguationIdentifier` is called **once per item** inside three separate loops (employment, funding, research-activity). A researcher with multiple positions at the same institution will trigger the same `DimPid` query multiple times.

The method queries:
```sql
SELECT TOP 1 * FROM dim_pid WHERE pid_type=? AND pid_content=? -- includes DimOrganization
```

### 5.2 Approach

Within `ImportOrcidRecordJsonIntoUserProfile`, maintain a per-call in-memory cache:

```csharp
var orgIdCache = new Dictionary<(string source, string identifier), int?>();

async Task<int?> GetOrgId(string source, string identifier)
{
    var key = (source ?? "", identifier ?? "");
    if (!orgIdCache.TryGetValue(key, out int? id))
    {
        id = await _organizationHandlerService.FindOrganizationIdByOrcidDisambiguationIdentifier(
            orcidDisambiguatedOrganizationIdentifier: identifier,
            orcidDisambiguationSource: source);
        orgIdCache[key] = id;
    }
    return id;
}
```

Replace all three `await _organizationHandlerService.FindOrganizationIdByOrcidDisambiguationIdentifier(...)` calls with `await GetOrgId(...)`.

**Risk:** Low. The cache is scoped to a single import call, so no cross-user state is shared. The results of `FindOrganizationIdByOrcidDisambiguationIdentifier` are deterministic for a given input pair.

---

## Phase 6 – Pre-load Research Activity Reference Data

**Target:** Eliminate the per-item `DimReferencedata` query in the "create new" branch of the research-activity loop.

### 6.1 Problem

When creating a **new** `DimProfileOnlyResearchActivity`, there is a per-item DB query:

```csharp
// Line ~2049 – inside research activity loop, only in the "create new" branch
DimReferencedatum dimReferencedata = await _ttvContext.DimReferencedata.Where(
    dr => dr.CodeScheme == Constants.ReferenceDataCodeSchemes.ORCID_RESEARCH_ACTIVITY &&
    dr.CodeValue == orcidResearchActivity.DimReferencedataCodeValue).AsNoTracking().FirstOrDefaultAsync();
```

For a profile with 20 new research activities, this is 20 individual queries.

### 6.2 Approach

Before the research-activity loop, pre-load all research-activity reference data into a dictionary:

```csharp
var researchActivityRefData = await _ttvContext.DimReferencedata
    .Where(dr => dr.CodeScheme == Constants.ReferenceDataCodeSchemes.ORCID_RESEARCH_ACTIVITY)
    .AsNoTracking()
    .ToDictionaryAsync(dr => dr.CodeValue);
```

Replace the per-item query with:
```csharp
researchActivityRefData.TryGetValue(orcidResearchActivity.DimReferencedataCodeValue, out DimReferencedatum dimReferencedata);
```

**Risk:** Very low. The reference data table is static (populated at DB initialisation). The dictionary lookup is semantically equivalent.

---

## Phase 7 – Extract Private Methods per Entity Type (Structural Refactoring)

**Target:** Improve maintainability and granular testability by splitting the 2000-line `ImportOrcidRecordJsonIntoUserProfile` into focused private helper methods.

### 7.1 Proposed decomposition

```
ImportOrcidRecordJsonIntoUserProfile(...)         // orchestrator only
  ├── ProcessName(...)
  ├── ProcessOtherNames(...)
  ├── ProcessResearcherUrls(...)
  ├── ProcessBiography(...)
  ├── ProcessEmails(...)
  ├── ProcessKeywords(...)
  ├── ProcessExternalIdentifiers(...)
  ├── ProcessEducations(...)
  ├── ProcessEmployments(...)
  ├── ProcessPublications(...)
  ├── ProcessDatasets(...)
  ├── ProcessFundings(...)
  ├── ProcessResearchActivities(...)
  └── RemoveDeletedItems(...)
```

Each private method receives: `dimUserProfile`, `orcidImportHelper`, `orcidRegisteredDataSourceId`, `currentDateTime`, `logUserIdentification`, and the relevant pre-parsed list.

### 7.2 Removal helper

The removal section at the end of the main method has the same pattern repeated 11 times. It can be reduced to a single generic helper:

```csharp
private void RemoveOrphanedFactFieldValues<TEntity>(
    IEnumerable<FactFieldValue> factFieldValues,
    Func<FactFieldValue, int> entityIdSelector,
    Func<FactFieldValue, TEntity> entitySelector,
    HashSet<int> processedIds,
    Action<TEntity> additionalCleanup = null)
    where TEntity : class
```

### 7.3 Notes

- This phase has the **highest risk** because it touches all code paths.
- It should be done **after** Phases 1–6 are merged and verified in production.
- The existing comprehensive integration tests in `OrcidImportServiceTest.cs` provide the safety net.

---

## Summary: Recommended Phased Delivery

| Phase | Description | DB Queries Saved (est.) | Risk | Effort |
|-------|-------------|------------------------|------|--------|
| 1 | Trivial: dead code, single `SaveChanges`, `AsNoTracking`, parallel ref-data | 3 round trips per import | Minimal | 1–2 hours |
| 2 | Eliminate JSON re-parsing | 0 DB (CPU/memory only) | Low | 2–4 hours |
| 3 | Batch DimDate lookups in `AddDimDates` | ~2 × N_dates per import | Medium | 4–8 hours |
| 4 | Pre-load DimDates in main body | ~2 × N_dates per import | Low (needs Ph.2+3) | 2–4 hours |
| 5 | Cache organisation disambiguation | Up to N_distinct_orgs | Low | 2–3 hours |
| 6 | Pre-load research activity ref data | N_new_activities | Minimal | 1–2 hours |
| 7 | Extract private methods | — (maintainability) | High | 1–2 days |

For a typical ORCID import with 20 educations + 30 employments + 15 fundings + 20 research activities, Phases 1–6 combined could reduce the number of DB round trips from **~200+** to approximately **15–20**.

---

## Acceptance Criteria for Each Phase

1. All existing tests in `OrcidImportServiceTest.cs` pass.
2. `AddDimDates_CreatesDatesFromFixture` and `AddDimDates_DoesNotDuplicateDates` pass (direct public method tests).
3. No change to the public API surface of `IOrcidImportService`.
4. Integration test against the development environment produces identical DB state before and after each phase.
