# Plan: Populate `ProfileEditorDataResponse.updated` from `DimUserProfile.Modified`

2026-09-09

**Status: Implemented (2026-09-09).** Build succeeds and all 316 tests pass.

## Background

[ProfileEditorDataResponse](../aspnetcore/src/api/Models/ProfileEditor/Items/ProfileEditorDataResponse.cs) currently has a `lastUpdated` property (`DateTime?`), always initialized to `null` in its constructor and never populated with real data.

**Target has changed**: instead of keeping the name `lastUpdated`, rename the property to `updated`, matching the existing [ElasticsearchPerson.updated](../aspnetcore/src/api/Models/Elasticsearch/ElasticsearchPerson.cs#L37) property, and populate it the same way `ElasticsearchPerson.updated` is currently populated — from `DimUserProfile.Modified` for the given `userprofileId`.

`ProfileEditorDataResponse` is built in one place only:

- [UserProfileService.GetProfileData()](../aspnetcore/src/api/Services/UserProfileService.cs#L1364) — constructs the response used by:
  - [ProfileDataController](../aspnetcore/src/api/Controllers/ProfileDataController.cs#L71) (`GET` profile data endpoint, also cached in memory)
  - [DebugController](../aspnetcore/src/api/Controllers/DebugController.cs#L238) (debug endpoint)
  - [BackgroundProfiledata.GetProfiledataForElasticsearch()](../aspnetcore/src/api/Background/BackgroundProfileData.cs#L43-L56) (Elasticsearch sync), which **already** separately queries `DimUserProfile.Modified` via a `DateTimeDTO` projection and assigns it to `ElasticsearchPerson.updated`.

This existing query in `BackgroundProfiledata` is the reference implementation/pattern to reuse (and, once `ProfileEditorDataResponse.updated` is populated upstream, to remove as redundant — see step 4):

```csharp
DateTimeDTO userProfileModified = await localTtvContext.DimUserProfiles
    .Where(dup => dup.Id == userprofileId).AsNoTracking()
    .Select(dimUserProfile => new DateTimeDTO() { Value = dimUserProfile.Modified })
    .FirstOrDefaultAsync();
```

## Goal

1. Rename `ProfileEditorDataResponse.lastUpdated` to `updated` (matching `ElasticsearchPerson.updated`).
2. `UserProfileService.GetProfileData()` sets `updated` from `DimUserProfile.Modified` for the requested `userprofileId`, using the same query pattern currently used in `BackgroundProfiledata` for `ElasticsearchPerson.updated`.
3. `ElasticsearchMapper.MapToElasticsearchPerson()` maps `src.updated` → `updated` directly (both properties now share the same name), so `BackgroundProfiledata.GetProfiledataForElasticsearch()` no longer needs its own separate `DimUserProfiles` query — the mapping call alone produces the correct value.
4. No behavior change to existing consumers other than the renamed/populated field (in particular, caching behavior in `ProfileDataController` is unaffected — the cached response's `updated` reflects the value at the time it was cached, same as every other field).
5. Add/update unit tests to cover all of the above.

## Out of scope

- Changing the API response contract/serialization beyond the property rename itself (see risk note below about consumers of the JSON field name `lastUpdated`).

## Implementation Steps

### 1. Rename `lastUpdated` to `updated` on `ProfileEditorDataResponse`

File: [ProfileEditorDataResponse.cs](../aspnetcore/src/api/Models/ProfileEditor/Items/ProfileEditorDataResponse.cs)

- Use a symbol rename (not find/replace) so all references update consistently:
  - Property declaration `public DateTime? lastUpdated { get; set; }` → `public DateTime? updated { get; set; }`.
  - Constructor initializer `lastUpdated = null;` → `updated = null;` (or drop the explicit `null` assignment entirely, since `DateTime?` already defaults to `null` — keep as-is for consistency with the rest of the constructor which explicitly initializes every property).
- This changes the serialized JSON field name from `lastUpdated` to `updated`. Check for any frontend/consumer code outside this repo (or other workspaces) that reads `lastUpdated` from the profile data API response — flag this as a breaking API contract change if any external consumer exists. Within this repo, no other `.cs` file currently reads `profileDataResponse.lastUpdated` (the property was added but never populated or consumed), so the blast radius here is limited to the declaration + its future population site.

### 2. Query `DimUserProfile.Modified` in `GetProfileData`

File: [UserProfileService.cs](../aspnetcore/src/api/Services/UserProfileService.cs)

In `GetProfileData(int userprofileId, LogUserIdentification logUserIdentification, bool forElasticsearch = false)`:

- Add a query, identical in shape to the one currently in `BackgroundProfiledata`, fetching only the `Modified` column with `AsNoTracking()`:

```csharp
DateTime? updated = await _ttvContext.DimUserProfiles
    .Where(dup => dup.Id == userprofileId)
    .AsNoTracking()
    .Select(dup => dup.Modified)
    .FirstOrDefaultAsync();
```

  - Prefer selecting `dup.Modified` directly (`DateTime?`) rather than projecting into `DateTimeDTO`, since we don't need the wrapper type here — keeps the change minimal and avoids adding a new dependency to `api.Models.Common` in this file (it may already be imported; check before adding).
- Assign the result to `profileDataResponse.updated` when building/after building the `ProfileEditorDataResponse` object literal.
- Run this query concurrently with the other independent `await` calls where practical, or simply add it as one more `await` in the existing sequential chain (existing code already awaits ~15 services sequentially, so consistency with the current style is preferred over introducing `Task.WhenAll` here).

### 3. Decide behavior when `DimUserProfile` row does not exist

- If no matching `DimUserProfile` row exists (`userprofileId` invalid), `FirstOrDefaultAsync()` on a `DateTime?` projection returns `default` = `null`. This is acceptable and matches the existing nullable `updated` semantics — no special-casing needed. This mirrors the current behavior of the equivalent query in `BackgroundProfiledata` (which does not special-case a missing row either).

### 4. No interface signature changes required (other than the rename)

- `IUserProfileService.GetProfileData` signature stays the same.
- `ProfileEditorDataResponse` keeps the same shape, just with the renamed property.

### 5. Map `updated` in `ElasticsearchMapper` and simplify `BackgroundProfiledata`

File: [ElasticsearchMapper.cs](../aspnetcore/src/api/ElasticsearchMapper/ElasticsearchMapper.cs#L12-L23)

- In `MapToElasticsearchPerson`, add `updated = src.updated` to the `ElasticsearchPerson` object literal:

```csharp
return new ElasticsearchPerson(orcidId)
{
    id = orcidId ?? "",
    personal = MapToElasticsearchPersonal(src.personal),
    activity = MapToElasticsearchActivity(src.activity),
    settings = MapToElasticsearchProfileSettings(src.settings),
    cooperation = MapToElasticsearchCooperation(src.cooperation),
    uniqueDataSources = MapToElasticsearchSource(src.uniqueDataSources),
    updated = src.updated
};
```

File: [BackgroundProfileData.cs](../aspnetcore/src/api/Background/BackgroundProfileData.cs#L35-L58)

- Remove the separate `DimUserProfiles` query and the now-redundant manual assignment, since the mapping call in the previous step already sets `elasticsearchPerson.updated` from `profileEditorDataResponse.updated`:

```csharp
DateTimeDTO userProfileModified = await localTtvContext.DimUserProfiles.Where(dup => dup.Id == userprofileId).AsNoTracking().Select(dimUserProfile => new DateTimeDTO()  
    {  
        Value = dimUserProfile.Modified
    }).FirstOrDefaultAsync();
elasticsearchPerson.updated = userProfileModified.Value;
```

- After removal, `elasticsearchPerson` returned from `MapToElasticsearchPerson` already has `updated` populated — no further assignment needed.
- This removes one redundant database round trip per Elasticsearch sync call (`GetProfiledataForElasticsearch` is called with `forElasticsearch: true`, and `GetProfileData` now always populates `updated` regardless of that flag).
- `localTtvContext` may become entirely unused in `BackgroundProfiledata` after this change — check whether it's still needed for anything else in the method; if not, remove the now-unnecessary `TtvContext` retrieval from the scope (but keep `IServiceScope scope` since `IUserProfileService` still needs it).
- `DateTimeDTO` (`api.Models.Common`) and `Microsoft.EntityFrameworkCore` usings in this file may become unused — remove if no longer referenced elsewhere in the file.

## Unit Test Plan

### 6. New test file/coverage for `UserProfileService.GetProfileData`

There is currently **no existing unit test** that exercises `UserProfileService.GetProfileData()` end-to-end (verified via search). Add one, following the existing patterns in [UserProfileServiceTest.cs](../aspnetcore/src/api.Tests/Services_Tests/UserProfileServiceTest.cs):

- Reuse the `CreateInMemoryContext(string dbName)` helper already present in that file (creates a `TtvContext` backed by `UseInMemoryDatabase`).
- Seed a `DimUserProfile` row with a known `Modified` timestamp, e.g.:

```csharp
using var context = CreateInMemoryContext(nameof(getProfileData_01));
DateTime expectedModified = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
context.DimUserProfiles.Add(new DimUserProfile
{
    Id = 1,
    OrcidId = "0000-0001-2345-6789",
    SourceId = "test",
    SourceDescription = "test",
    DimKnownPersonId = -1,
    Modified = expectedModified
});
await context.SaveChangesAsync();
```

- Construct a `UserProfileService` with the real, lightweight dependent services (no mocking framework is used anywhere in this test project — confirmed no `Moq` usage). All of `GetProfileData`'s dependencies are simple services requiring only `TtvContext` (+ `ILogger<T>`, and `ILanguageService` for `NameService`/similar). Pass `logger: null` for each, consistent with existing tests in this file (e.g. `getUserprofileIdForOrcidId_01`/`_02` already pass `logger: null`).
  - Services needed: `IAffiliationService`, `IEducationService`, `IEmailService`, `IExternalIdentifierService`, `IKeywordService`, `INameService`, `IPublicationService`, `IResearcherDescriptionService`, `ITelephoneNumberService`, `IWebLinkService`, `IFundingDecisionService`, `IResearchDatasetService`, `IResearchActivityService`, `IUniqueDataSourcesService`, `ISettingsService`, `ICooperationChoicesService`.
  - Since the seeded profile has no `FactFieldValue` rows, all of the above will return empty lists/defaults, keeping the test focused on `updated`.
  - `ILanguageService` (needed by `NameService` and a few others) — check whether a simple concrete `LanguageService` can be instantiated directly (it's used elsewhere in tests, e.g. `AffiliationServiceTestData.cs`); reuse that pattern instead of writing a new fake.
- Call `await userProfileService.GetProfileData(userprofileId: 1, logUserIdentification: new LogUserIdentification())` (check the simplest valid construction of `LogUserIdentification` from other tests).
- Assert:
  - `result.updated` equals `expectedModified`.
  - (Optional, light smoke assertions) `result.personal`, `result.activity`, `result.cooperation`, `result.uniqueDataSources` are non-null and empty, to confirm the rest of the method still behaves as before.

Add a second test case:
- `DimUserProfile.Modified == null` seeded explicitly → assert `result.updated` is `null`.

Add a third test case (not-found path, if in scope):
- `userprofileId` does not match any seeded row → assert `result.updated` is `null` and no exception is thrown.

### 7. Update existing `ElasticsearchMapperTest.cs` test to cover `updated`

File: [ElasticsearchMapperTest.cs](../aspnetcore/src/api.Tests/Services_Tests/ElasticsearchMapperTest.cs)

- `GetProfileEditorDataResponse()` (private test helper, ~line 90) builds a `ProfileEditorDataResponse` object literal. Update it to set `updated` to a known non-null test value (e.g. a fixed `DateTime`), since the property is now renamed and mapped.
- In the `[Fact(DisplayName = "ProfileEditorDataResponse is correctly mapped to ElasticsearchPerson")]` test (~line 1639), add an assertion that the resulting `ElasticsearchPerson.updated` equals the `updated` value set on the source `ProfileEditorDataResponse`, now that `MapToElasticsearchPerson` maps it directly.

### 8. Update/add test coverage for `BackgroundProfiledata.GetProfiledataForElasticsearch`

- Search the test project for any existing test exercising `BackgroundProfiledata`/`IBackgroundProfiledata` (none found as of this plan's writing — verify again at implementation time since this is a background/service-scope-heavy class that may be hard to unit test in isolation).
- If a test exists, update it to assert `elasticsearchPerson.updated` still reflects `DimUserProfile.Modified`, now arriving via the mapping (`profileEditorDataResponse.updated` → `ElasticsearchMapper`) rather than the removed direct query — the expected behavior/output does not change, only the internal source of the value.
- If no test exists, adding one is optional for this plan (would require standing up `IServiceScopeFactory`/DI scope plumbing); prioritize the `UserProfileService.GetProfileData` tests (step 6) and the `ElasticsearchMapper` test update (step 7) as the primary regression guard, since both code paths now depend on the same underlying data.

### 9. Regression check on caching

File: [ProfileDataController.cs](../aspnetcore/src/api/Controllers/ProfileDataController.cs#L64)

- No test changes anticipated: the controller caches the whole `ProfileEditorDataResponse` object as-is, so `updated` is naturally included in the cached value. If a `ProfileDataController` unit/integration test exists asserting the full shape of the cached/returned object, verify it still passes (search turned up no such test currently, so this is a verification step, not new-test-writing).

## Acceptance Criteria

- [x] `ProfileEditorDataResponse.lastUpdated` is renamed to `updated`.
- [x] `GetProfileData` populates `updated` from `DimUserProfile.Modified` for the requested `userprofileId`.
- [x] `updated` is `null` when `DimUserProfile.Modified` is `null`, and when no matching `DimUserProfile` row exists (no exception thrown).
- [x] `ElasticsearchMapper.MapToElasticsearchPerson` maps `src.updated` to `ElasticsearchPerson.updated`.
- [x] `BackgroundProfiledata.GetProfiledataForElasticsearch` no longer runs its own `DimUserProfiles` query for `updated`; the redundant query and now-unused `DateTimeDTO`/`localTtvContext` usages are removed.
- [x] No change to method signatures (`IUserProfileService`, `IBackgroundProfiledata`, `ElasticsearchPerson`); `ProfileEditorDataResponse` has only the property rename.
- [x] New unit test(s) added to `UserProfileServiceTest.cs` covering: value populated, null `Modified`, and (if applicable) missing profile.
- [x] `ElasticsearchMapperTest.cs` updated to assert `updated` is mapped through.
- [x] `dotnet build` (task: `build`) succeeds.
- [x] All tests pass, run via `dotnet test aspnetcore/mydata.sln` (or the equivalent test task if one exists).

## Risks / Considerations

- **API contract change**: renaming the JSON field from `lastUpdated` to `updated` is a breaking change for any consumer already reading `lastUpdated` from the profile data response. Since the property was added but never populated (always `null`), the practical impact is likely low, but confirm with the team/frontend before shipping, especially if the field has already been released to any consumer.
- Adding one more database round trip per `GetProfileData` call (currently ~15 already happen sequentially against the same `FactFieldValues`/related tables). This is a single indexed lookup by primary key (`DimUserProfile.Id`) and should have negligible performance impact.
- Because `GetProfileData` is called both for interactive requests (`ProfileDataController`, cached) and background Elasticsearch sync (`forElasticsearch: true`), confirm the new query behaves identically in both modes — it does not depend on `forElasticsearch` and should always run.
- Confirm `Microsoft.EntityFrameworkCore` (`Select`/`FirstOrDefaultAsync`) is already imported in `UserProfileService.cs` (it is, per existing usages) — no new `using` needed for the query itself.
- Net effect on `BackgroundProfiledata` is a net-negative database call: the old direct query is removed and no new one is added there, since the value now flows in through the mapping call already made in that method. Overall query count across the whole sync operation stays the same (the query simply moved into `GetProfileData`, executed once, and its result flows through `ElasticsearchMapper` instead of being re-fetched).
