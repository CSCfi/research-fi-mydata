Status: Implemented
2026-10-28

# Consent controller

Added authenticated profile consent target discovery, ROR filtering, active-consent listing,
organization-wide grants, and caller-scoped physical withdrawal. Added a scoped service, API
models, regression tests, and request examples. Updated consent date handling to use UTC
`DateTime` fields and synchronized Public API expiry/withdrawal checks.

Files touched: `aspnetcore/src/api/Controllers/ConsentController.cs`,
`aspnetcore/src/api/Models/Api/ConsentModels.cs`,
`aspnetcore/src/api/Models/Common/Constants.cs`,
`aspnetcore/src/api/Models/Ttv/DimDate.cs`,
`aspnetcore/src/api/Models/Ttv/FactProfileDataSharingConsent.cs`,
`aspnetcore/src/api/Models/Ttv/TtvContext.cs`,
`aspnetcore/src/api/Services/ConsentService.cs`,
`aspnetcore/src/api/Services/IConsentService.cs`,
`aspnetcore/src/api/Services/PublicApiService.cs`,
`aspnetcore/src/api/Startup.cs`,
`aspnetcore/src/api.Tests/Services_Tests/ConsentControllerTest.cs`,
`aspnetcore/src/api.Tests/Services_Tests/ConsentServiceTest.cs`,
`aspnetcore/src/api.Tests/Services_Tests/PublicApiServiceTest.cs`,
`aspnetcore/http/consent.http`.
