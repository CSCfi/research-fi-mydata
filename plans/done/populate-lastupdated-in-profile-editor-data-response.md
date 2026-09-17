# ProfileEditorDataResponse.updated

**Status:** Implemented — 2026-09-09 (316 tests pass)

Renamed `ProfileEditorDataResponse.lastUpdated` → `updated` (JSON field rename; no known
consumers were reading the never-populated field). `UserProfileService.GetProfileData()` now
populates it from `DimUserProfile.Modified`. `ElasticsearchMapper.MapToElasticsearchPerson` maps
`src.updated` directly, letting `BackgroundProfiledata.GetProfiledataForElasticsearch` drop its
now-redundant separate `DimUserProfiles` query.

**Files changed:** `ProfileEditorDataResponse.cs`, `UserProfileService.cs`,
`ElasticsearchMapper.cs`, `BackgroundProfileData.cs`, `UserProfileServiceTest.cs`,
`ElasticsearchMapperTest.cs`.
