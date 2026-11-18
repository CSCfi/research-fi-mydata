Status: Implemented
2026-10-28

# Consent controller

Added authenticated profile consent target discovery, ROR filtering, active-consent listing,
organization-wide grants, and caller-scoped soft withdrawal. Withdrawn rows remain unchanged
during subsequent consent operations; regranting creates a new active row rather than reusing
history. Full user-profile deletion retains its existing physical cleanup behavior. Added a scoped service, API
models, regression tests, and request examples. Updated consent date handling to use UTC
`DateTime` fields. Public API consent authorization uses only `ConsentActive` for the
requested profile and controller, ignoring expiry and withdrawal dates and inactive history.

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
