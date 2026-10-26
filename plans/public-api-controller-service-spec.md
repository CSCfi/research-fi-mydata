Status: In Progress
2026-10-26

# Public API controller and service implementation spec

## Purpose

Define the intended behavior for `POST /api/publicapi/profile` and implement it in
`PublicApiController` and `PublicApiService`. This document is the lightweight, human-readable
specification for agentic implementation; no specification framework or runtime dependency is
introduced.

The endpoint checks profile-data transfer consent for a person and a controller, and returns the
person's profile data only as permitted by the resolved consent policy. The request and response
contracts are the existing types in `api.PublicApiContracts`; this plan does not change their
shape unless implementation uncovers a necessary, agreed contract correction.

## Scope

In scope:

- Request authentication and validation at the controller boundary.
- Resolving the requested person profile from the submitted identifiers.
- Resolving the controller identified by ROR.
- Determining whether a valid consent exists between the profile and controller.
- Returning a response consistent with the consent result and the contract.
- Returning profile editor data in the existing public API contract shape.
- Structured request logging that avoids exposing credentials and personal identifiers.
- Unit tests for controller and service behavior.

Out of scope unless separately agreed:

- Changing the public contract package/schema or its NuGet version.
- Adding a specification/test framework.
- Changing how consent is created, withdrawn, expired, or managed elsewhere in the application.
- Expanding the profile-data contract with new source data.
- Changing the shared-secret authentication mechanism or introducing a new identity provider.

## Contract and current implementation context

- Route: `POST /api/publicapi/profile`.
- Request type: `ResearchFi.PersonPublicApi.ProfileDataRequest`.
- The request schema requires `personIdentifiers` (1–2 items) and `controllerROR`; identifier
  types are `ORCID` and `HETU`.
- Response type: `ResearchFi.PersonPublicApi.ProfileDataResponse`.
- The response schema requires `profileFound`, `controllerFound`, and `consentManagementURL`.
  `consentActive` is meaningful only when both the profile and controller are found.
- The controller authenticates using configured `PUBLICAPITOKEN` compared with the
  `publicapitoken` header, accepts a `public-api-clientid` header for logging, and validates
  identifier types/content and the required request shape.
- The service prefers ORCID and only uses the HETU-derived username when ORCID is absent. It
  resolves bare ROR IDs via `DimPid` and the associated `FactOrganizationGeneral`, then checks
  `FactProfileDataSharingConsent` before requesting profile-editor data.
- Consent target/purpose is not part of this endpoint's validity check. Profile-editor data
  continues to be requested with `forElasticsearch: true` only for visible profiles with active
  consent.
- Existing mapping and controller/service tests are in
  `aspnetcore/src/api.Tests/Services_Tests/PublicApiServiceTest.cs` and
  `aspnetcore/src/api.Tests/Services_Tests/PublicApiControllerTest.cs`.

## Normative behavior

The policy questions below have been answered and their requirements are normative.

### Controller behavior

1. Accept the request at `POST /api/publicapi/profile`.
2. Require a configured, non-empty `PUBLICAPITOKEN` and a matching `publicapitoken` request
   header. Missing configuration, a missing header, or a non-matching header results in
   `401 Unauthorized`; the service must not be called.
3. Require a non-null request, 1–2 person identifiers, and a non-blank `controllerROR`. Invalid
   request shapes result in `400 Bad Request`; the service must not be called.
4. Do not log the shared token, HETU, ORCID, or profile data. Preserve structured request logging
   and the caller client ID header where present.
5. On a valid request, return the service response with `200 OK`. Do not convert service/data
   errors into success-shaped responses.

### Service behavior

1. Resolve only supported identifiers with non-blank values. Resolve the profile according to the
   agreed precedence and fallback policy. Do not call profile lookup methods for unusable values.
2. A missing profile is reported as not found. A hidden profile is reported as found, but its
   profile editor data is never requested or returned, even when consent is active.
3. Resolve the supplied controller ROR to a registered controller organization. An unknown ROR
   is represented as `controllerFound = false`; it must not be reported as found by default.
4. Determine consent only when both profile and controller are found, using `FactProfileDataSharingConsent`
   rows linking them. Consent is active if any row is not withdrawn and is not expired. Compare
   expiry inclusively against the current UTC date; a null expiry does not expire.
5. Return profile data only when consent is active and the profile is not hidden.
6. Set `consentManagementURL` to the approved placeholder URL in all three language variants.
7. Retain the existing public profile field mapping where it satisfies the contract and the
   consent policy. Only data intended for public display is included. Absent source values remain
   absent/null/empty according to the established contract behavior; do not invent values.
8. Prefer ORCID when present and do not fall back to HETU if ORCID lookup fails. If ORCID is
   absent, use the existing HETU-derived username lookup. The hash is an internal lookup
   mechanism; neither the submitted HETU nor its hash is returned or logged.

## Confirmed policy decisions

These decisions were confirmed before implementing behavior; tests encode them.

| Decision | Options / considerations | Confirmed answer |
|---|---|---|
| Profile data release | Return data only when consent is active. | Confirmed |
| Identifier precedence | Prefer ORCID when both ORCID and HETU are supplied; ignore HETU. | Confirmed |
| Identifier fallback | If ORCID is supplied and does not resolve, stop without trying HETU. Use the HETU-derived username lookup only when ORCID is absent. | Confirmed |
| Hidden profiles | Report hidden profiles as found, check consent as usual, but withhold profile data even if consent is active. | Confirmed |
| Controller ROR lookup | Match bare `controllerROR` against `dim_pid.pid_content` where `pid_type = 'RORID'`, case-insensitively. The corresponding `dim_organization` maps through `fact_organization_general`; URL-form ROR values are not accepted as aliases. | Confirmed |
| Consent validity | Consent exists when `FactProfileDataSharingConsent` links the profile to the controller organization. It is active when not withdrawn and not expired. Expiry is inclusive through the UTC current date; null expiry means no expiry. If multiple rows exist, any currently valid, non-withdrawn row makes consent active. | Confirmed |
| Consent target/purpose | Do not filter this endpoint by `DimConsentTarget` or purpose code. | Confirmed |
| Response matrix | Use the values in the completed matrix below. `consentActive` is null unless both profile and controller are found. | Confirmed |
| Consent management URL | Use `https://localhost:5003/fi/mydata/account-settings?login=1` for Finnish, Swedish, and English values. | Confirmed |
| Profile visibility | Continue to request profile-editor data with `forElasticsearch: true` for eligible profiles. | Confirmed |
| Unsupported identifier types | Reject unsupported types and blank identifier values with `400 Bad Request`. | Confirmed |

### Response scenarios

The response must not disclose personal data in any scenario where release is not authorized.

| Profile | Controller | Consent | `profileData` | Required response flags/URL |
|---|---|---|---|---|
| Not found | Found or not found | N/A | Null | `profileFound=false`; `controllerFound` reflects the ROR lookup; `consentActive=null`; management URL populated |
| Hidden | Found or not found | Inactive, absent, or active when controller is found | Null | `profileFound=true`; `controllerFound` reflects the ROR lookup; `consentActive` is null if controller is not found, otherwise reflects consent; management URL populated |
| Found | Not found | N/A | Null | `profileFound=true`; `controllerFound=false`; `consentActive=null`; management URL populated |
| Found | Found | Inactive / absent | Null | `profileFound=true`; `controllerFound=true`; `consentActive=false`; management URL populated |
| Found | Found | Active | Public profile data | `profileFound=true`; `controllerFound=true`; `consentActive=true`; management URL populated |

## Implementation plan

### Phase 0 — Confirm the policy and data model

1. Policy decisions were confirmed in discussion; see the table above.
2. Generated EF relationships show ROR PIDs attached to `DimOrganization` and consent controller
   references attached to `FactOrganizationGeneral`.
3. `DimDate` stores calendar components; expiry is compared inclusively with the UTC current date.
4. The response scenario table is complete and confirmed.

**Acceptance criteria**

- No policy row remains `TBD` for behavior that will be implemented.
- The ROR lookup source, consent validity predicate, identifier resolution behavior, and response
  matrix are explicit and testable.
- Any requested contract/configuration change is identified separately before implementation.

### Phase 1 — Specify and test controller boundary behavior

Target files:

- `aspnetcore/src/api/Controllers/PublicApiController.cs`
- `aspnetcore/src/api.Tests/Services_Tests/PublicApiControllerTest.cs`

1. Add/adjust controller tests for token missing, mismatched token, and valid token.
2. Assert the service is not called on unauthorized or malformed requests.
3. Cover null request, missing/empty identifier list, list sizes outside 1–2, missing/blank ROR,
   and unsupported identifier types according to the confirmed decision.
4. Verify valid requests pass through the service result unchanged and return `200 OK`.
5. Verify logs do not contain token or person identifier values; retain useful client-id context.

**Acceptance criteria**

- Every controller rule above is covered by a focused test.
- Unauthorized and malformed requests do not invoke `IPublicApiService`.
- Existing endpoint route, response contract, and structured logging conventions are preserved.

### Phase 2 — Implement profile, controller, and consent resolution

Target files:

- `aspnetcore/src/api/Services/PublicApiService.cs`
- `aspnetcore/src/api/Services/IPublicApiService.cs` only if a service contract change is needed.
- `aspnetcore/src/api.Tests/Services_Tests/PublicApiServiceTest.cs`

1. Implement identifier selection, precedence, and fallback exactly as confirmed.
2. Resolve hidden/missing profiles without requesting profile-editor data.
3. Resolve ROR to the registered controller organization using the confirmed data source.
4. Query consent for the resolved profile/controller and apply the confirmed target/purpose, date,
   withdrawal, expiry, and duplicate-row rules.
5. Build response flags, profile-data presence, and consent management URL according to the
   completed response matrix.
6. Remove misleading TODOs and placeholder success values once replaced with real behavior.
7. Keep query behavior asynchronous and use existing EF Core patterns. Surface database or
   configuration failures rather than reporting a false `controllerFound` or inactive consent.

**Acceptance criteria**

- Tests cover each response-matrix row and consent date boundaries.
- Tests cover unknown controller, no consent, withdrawn consent, expired consent, and active
  consent as applicable to the confirmed policy.
- Tests prove profile-editor data is only requested in permitted scenarios.
- Controller matching and consent filtering are exercised against the test database or an
  appropriately isolated repository/service boundary.

### Phase 3 — Verify and preserve public profile mapping

Target files:

- `aspnetcore/src/api/Services/PublicApiService.cs`
- `aspnetcore/src/api.Tests/Services_Tests/PublicApiServiceTest.cs`

1. Retain and verify the existing mappings for names, contact details, researcher description,
   affiliations, education, funding, publications, datasets, identifiers, and collaboration
   interests.
2. Confirm `forElasticsearch: true` yields only data intended for public release, including
   per-item visibility behavior.
3. Add focused regressions for filtering/visibility and missing optional source values where
   existing tests do not fully establish behavior.
4. Confirm the service response serializes with the contract shape and required response fields.

**Acceptance criteria**

- Existing mapping assertions continue to pass.
- No unpublished/private item is included contrary to the confirmed visibility policy.
- Null/optional data does not cause a mapping exception or fabricate output.
- No contract change is made without explicit agreement and corresponding schema/package update.

### Phase 4 — Full verification and documentation

1. Run targeted `PublicApiControllerTests` and `PublicApiServiceTests`.
2. Run `dotnet test aspnetcore/mydata.sln` and `dotnet build aspnetcore/mydata.sln`.
3. Update API documentation/configuration documentation only if implementation adds or changes
   required settings, routes, headers, or response behavior.
4. Review the final diff for accidental logging of personal data and for any remaining placeholder
   consent/controller behavior.

**Acceptance criteria**

- Targeted tests, full test suite, and build pass.
- Documented endpoint behavior agrees with the confirmed spec and contract.
- No silent fallback reports a controller or consent as found/active without evidence.

## Test scenario checklist

- [ ] Missing `PUBLICAPITOKEN` configuration; missing and incorrect token headers.
- [ ] Valid token with malformed request, including null identifiers and invalid count.
- [ ] Valid request; service response is returned as `200 OK`.
- [ ] Each supported identifier independently resolves a profile.
- [ ] Both identifiers supplied: precedence, same-person, and conflict behavior.
- [ ] Unsupported identifier and blank identifier behavior.
- [ ] Missing/hidden profile; no profile-data lookup and correct response matrix.
- [ ] Registered and unknown ROR behavior, including normalization policy.
- [ ] Consent absent, withdrawn, expired, active, and boundary-date cases.
- [ ] Consent-target/purpose match and mismatch if applicable.
- [ ] Profile data withheld or returned according to the consent policy.
- [ ] Consent management URL populated according to configuration/policy.
- [ ] Public profile mapping, including unpublished/private items and optional values.
- [ ] Logs omit token, HETU, ORCID, and profile data.
- [ ] Service/query exceptions do not become a success-shaped response.

## Risks and mitigations

- **Unconfirmed consent rules:** Incorrectly returning personal data is a privacy risk. Treat policy
  decisions as a hard gate; do not ship guessed behavior.
- **ROR storage ambiguity:** The entity model may not make ROR resolution obvious. Verify actual
  stored identifier data and use a focused query/test rather than matching names or arbitrary
  organization fields.
- **Generated EF model:** TTV entities are scaffolded. Do not hand-edit generated entities; query
  through the existing model or request a scaffold/schema change separately.
- **Identifier privacy:** HETU is sensitive. Keep it out of logs, errors, assertions beyond
  established test fixtures, and returned data.
- **Date semantics:** Consent validity can change at date boundaries and may be stored as date
  dimensions. Explicitly test agreed inclusive/exclusive rules.
- **Overreliance on `forElasticsearch`:** This flag may be intended for indexing rather than
  public API release. Verify its visibility guarantees before relying on it.
- **Contract drift:** The JSON schema currently documents a consent-status exchange while the C#
  DTOs define the response shape. Keep both aligned if a contract correction is approved.

## Completion and plan lifecycle

After all phases are implemented and verified, change the status to `Implemented`, replace this
document with a concise summary of the behavior and files changed, and move it to `plans/done/`.
If implementation pauses with any phase incomplete, keep the full plan at the top level with
status `In Progress`.
