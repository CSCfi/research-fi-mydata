Status: In Progress
Date: 2026-11-24

# ORCID Timestamp-Based Import Optimization Plan

## Goal

Extend the production-proven keyword timestamp handling from
`orcid-keyword-last-modified-and-created-dates.md` to the other ORCID-imported
data types. The importer should avoid updating EF-tracked entities and performing
related lookups when an ORCID item's `last-modified-date` shows that its
corresponding `FactFieldValue` is already current.

The work must be delivered incrementally over a long period. Each ORCID item
type has a separate implementation phase with independent verification and
deployment criteria.

The prerequisite is strict: `FactFieldValue.Created` and
`FactFieldValue.Modified` must be correctly populated for every ORCID item
type before any new timestamp-based skipping is introduced.

## Current State

`OrcidImportService` currently processes the following ORCID data:

- primary person name
- other names
- researcher URLs
- biography
- email addresses
- keywords
- external identifiers
- educations
- employments, mapped to affiliations
- publications from ORCID works
- datasets from ORCID works
- fundings
- research activities, sourced from works and from distinctions, invited
  positions, memberships, peer reviews, qualifications, and services

Most update paths currently rewrite the domain entity and its `FactFieldValue`
using the import runtime clock. Keyword import is the exception: parsed ORCID
`created-date` and `last-modified-date` values are stored in the
corresponding `FactFieldValue`, while `DimKeyword` continues to use the import
runtime clock for its own audit fields.

The importer also performs organization disambiguation, date lookups, related
URL updates, and occasional `FactFieldValue` primary-key recreation in
update paths. These are the expensive side effects that later phases should
avoid for unchanged items.

## Invariants

1. ORCID owns the timestamps of the imported relation:
   - `FactFieldValue.Created` uses the ORCID item's `created-date`.
   - `FactFieldValue.Modified` uses the ORCID item's
     `last-modified-date`.
2. Domain entities such as `DimEducation`, `DimAffiliation`,
   `DimProfileOnlyPublication`, and related dimensions continue to use the
   application import clock for their own audit fields.
3. A missing ORCID timestamp must not cause data loss. Populate a new row with
   the import runtime clock as a safe fallback, but treat missing incoming
   timestamps as not safely comparable. Process the item fully rather than
   skipping it.
4. An unchanged item is marked as present in the import helper before its
   domain update is skipped. Existing deletion processing must therefore still
   remove only records actually absent from the new ORCID payload.
5. `Created` is set on insert and may be repaired from ORCID on a processed
   update. It is not advanced during normal updates.
6. `Modified` is advanced only from the incoming ORCID item timestamp, never
   from the application import time when an ORCID timestamp is available.
7. A timestamp comparison is only an optimization for an existing record. New
   records always follow the normal full insert path.
8. If timestamp extraction, shape, identity, or matching is uncertain, use the
   existing full-processing path and log or expose the condition according to
   existing importer conventions. Do not silently assume equality.
9. Organization relations, identifierless data, related web links, reference
   data, dates, and child dimensions must remain behaviorally identical when an
   item is processed.
10. Recreated `FactFieldValue` rows must copy or reapply the same timestamp
    values and must not lose the source timestamp metadata.

## Shared Technical Design

### Parsed model contract

Add nullable `CreatedDate` and `LastModifiedDate` properties to every parsed
ORCID item model during Phase 0. Populate them from the
item's own JSON node with the existing `GetOrcidDateTime` helper, accepting
milliseconds-since-epoch ORCID values and converting them to UTC.

Name, biography, and any source-level singleton must use the timestamp attached
to the containing ORCID object that semantically owns that value. Do not use the
top-level ORCID record timestamp as a substitute without explicit verification
that it represents changes to the individual value.

For grouped summaries, use the timestamp from the exact summary represented by
the parsed model. If grouped work selection chooses only one work summary,
the timestamp and payload fields must come from the same selected summary.

### FactFieldValue timestamp population

On insert, resolve both values independently:

- `Created = item.CreatedDate ?? currentDateTime`
- `Modified = item.LastModifiedDate ?? item.CreatedDate ?? currentDateTime`

On a full update, set:

- `Created = item.CreatedDate ?? existing Created ?? currentDateTime`
- `Modified = item.LastModifiedDate ?? currentDateTime`

On optimization phases, only skip a domain update when both stored and incoming
`last-modified-date` values are valid and:

- `incoming LastModifiedDate <= existing FactFieldValue.Modified`, after
  converting both values to the `datetime` precision used by the persisted
  column so SQL Server's 1/300-second rounding does not turn equal values into
  apparent updates. All skip-enabled phases use the shared comparator in
  `OrcidImportService`, including primary name, biography, other names, and
  researcher URLs.

An incoming value older than or equal to stored metadata is considered already
represented. This makes imports idempotent and protects against stale ORCID
payloads. A missing incoming timestamp, missing stored timestamp, or other
untrusted comparison falls back to full processing.

The Phase 0 audit must also identify records whose stored timestamp is only a
runtime fallback. Such a row must be full-processed until its `Modified` value
can be replaced with an ORCID timestamp; otherwise an application import time
could be incorrectly interpreted as a newer ORCID item time.

Each phase must revisit the user's existing `Show`, `PrimaryValue`, source,
organization identifier data, and related records to ensure optimized updates do
not alter them.

### Suggested implementation shape

Keep the shared rules consistent without prematurely extracting a large
cross-cutting abstraction. A small timestamp helper may be introduced when two
or more phases would otherwise duplicate it, for example to:

- resolve insert/update/fallback timestamp values;
- evaluate whether an item is safely unchanged;
- copy timestamps when recreating a `FactFieldValue`.

Item matching, processed-ID tracking, and all domain-specific mutations should
remain in the corresponding import section until the existing improvement plan's
structural refactor is implemented.

Do not combine a type's timestamp rollout with unrelated import refactoring.

## Phase 0 - Populate and Verify Timestamps for Every ORCID Item

Implementation note (2026-11-23): Parser timestamp propagation and
`FactFieldValue` timestamp population are implemented, including the dataset
update correction and timestamp preservation when organization changes recreate
rows. Parser/import regression tests pass. Production import and data-audit
verification were confirmed by the user on 2026-11-23.

### Scope

This is the mandatory precondition for every later phase. It changes timestamp
population only; it must not introduce timestamp-based skipping yet.

1. Audit the actual ORCID JSON fixtures and parser paths for the timestamp node
   available on every imported type:
   - person name
   - biography
   - other names
   - researcher URLs
   - emails
   - keywords
   - external identifiers
   - education
   - employment/affiliation
   - publication work summaries
   - dataset work summaries
   - funding summaries
   - research-activity work summaries
   - distinction, invited-position, membership, peer-review, qualification,
     and service summaries
2. Extend parser models and parser methods to expose nullable ORCID
   `CreatedDate` and `LastModifiedDate` values.
3. Update all insert and full-update paths to populate `FactFieldValue.Created`
   and `FactFieldValue.Modified` from those values, using the keyword behavior
   as the reference and the fallback policy above.
4. Correct the dataset update path, which currently updates the dataset
   entity timestamp in the `FactFieldValue` update position, so the
   `FactFieldValue` timestamp is populated as required.
5. Preserve timestamps when replacing a `FactFieldValue` because a composite
   key changes in affiliation, funding, or research-activity organization
   handling.
6. Ensure the funding flow, including `ImportAdditionalData`, does not overwrite
   the source-owned timestamp with an application runtime timestamp.
7. Run a production-data audit or equivalent SQL verification after deployment:
   - no newly imported ORCID-backed rows have null `Created` or `Modified`;
   - sampled values equal the corresponding ORCID item timestamps when
     timestamps were supplied;
   - fallback values are visible and bounded to cases where ORCID omitted the
     timestamp;
   - newly reimported legacy rows acquire valid timestamp values.

### Gate to proceed

Do not start any Phase 1+ update-logic work until:

- all item parser types expose timestamps or an explicitly documented reason
  why ORCID does not provide them;
- all ORCID-backed `FactFieldValue` insert/update/recreation paths populate both
  fields;
- parser and importer regression tests pass for supplied, null, and missing
  timestamp cases;
- at least one normal production import cycle confirms the audit results.

The missing-timestamp outcome is a permanent full-processing fallback, not a
permission to skip that item based on a runtime-generated value.

## Phase 1 - Primary Person Name

Apply timestamp comparison to the existing primary name `FactFieldValue`.

Implementation note (2026-11-23): Timestamp comparison and regression tests are
implemented. Production verification was confirmed by the user on 2026-11-24.

- Parse the timestamp of the ORCID name object once and reuse it for given and
  family name handling.
- Keep the current combined update semantics: a change to either name component
  updates the same `DimName`.
- A safely unchanged name is marked processed but no `DimName` field or audit
  timestamp is rewritten.
- Cover name present, changed, unchanged, older incoming timestamp, missing
  timestamp, and transition to empty values.

## Phase 2 - Biography

Apply timestamp comparison to the researcher-description item.

Implementation note (2026-11-24): Timestamp comparison and regression tests are
implemented and verified in production.

- Preserve the current behavior when biography is null or absent, including
  removal through the existing processed-ID mechanism.
- Avoid writing `DimResearcherDescription` for an unchanged item.
- Cover non-null-to-null transitions, newly present biography, unchanged data,
  and missing timestamps.

## Phase 3 - Other Names

Apply comparison independently to each other-name item by ORCID put code.

Implementation note (2026-11-24): Timestamp comparison and regression tests are
implemented and verified in production.

- Mark every unchanged match as processed.
- Preserve deletion of names no longer returned by ORCID.
- Ensure a put-code collision or missing put code cannot be treated as an
  unchanged match.
- Cover multiple items with mixed changed/unchanged states in one import.

## Phase 4 - Researcher URLs

Apply comparison independently to each researcher URL by ORCID put code.

Implementation note (2026-11-24): Timestamp comparison and regression tests are
implemented, including a regression for SQL Server `datetime` round-tripping.
Production verification was confirmed on 2026-11-25.

- Skip URL and label writes for safely unchanged items.
- Preserve current URL truncation and validation behavior for processed items.
- Cover URL-label-only changes, URL changes, unchanged items, and deletion.

## Phase 5 - Email Addresses

Apply timestamp comparison to email items.

Implementation note (2026-11-25): Email identity remains the exact stored
address, independent of put code; a changed address is removed and reinserted.
Only a unique incoming and stored, untruncated address match qualifies for
timestamp comparison. Ambiguous inputs are logged, existing matching rows are
retained without timestamp updates, and ambiguous new rows are not inserted.
Production verification was confirmed on 2026-11-25.

- First solve the stable matching strategy when ORCID put code is null, since
  current matching is by email value.
- A timestamp for an existing email must be applied only if the incoming item
  can be associated with the same persisted `FactFieldValue` deterministically.
- Preserve the current behavior where an email can be added or removed, and do
  not create duplicate rows when timestamp metadata is backfilled.
- Cover put-code-present, put-code-null, changed email value, unchanged email,
  missing timestamp, and deletion cases.

## Phase 6 - External Identifiers

Apply comparison independently to external-identifier items.

Implementation note (2026-11-25): Timestamp comparison uses unique put-code and
field-setting matches, with regression tests for mixed items and null URLs.
Production verification remains pending before proceeding to Phase 7.

- Match by the established ORCID put code and field display setting.
- Skip `DimPid` rewrites for unchanged items.
- Validate null/empty external ID URL handling and identifier type/value
  changes.
- Cover mixed changed and unchanged identifiers in one payload.

## Phase 7 - Education

Apply comparison to each education item before date lookups and entity updates.

- Move start/end `DimDate` lookups after the unchanged fast path.
- Mark skipped items processed so unchanged educations are retained.
- Preserve behavior for organization/department name fields, dates, and
  show/primary state.
- Cover changed date-only data, changed institution/role, unchanged data, and
  deletion.

## Phase 8 - Employment / Affiliation

Apply comparison before organization disambiguation, date lookups, department
handling, and `FactFieldValue` recreation.

- A changed item retains the full existing organization-to-identifierless
  transition behavior.
- `FactFieldValue` replacements copy timestamp metadata.
- An unchanged item must not call organization lookup or recreate a relation.
- Cover both linked-organization and identifierless paths, lost/found
  organization transitions, department changes, changed dates, unchanged
  items, and deletion.

## Phase 9 - Publications from ORCID Works

Apply comparison to publication summaries after the existing group/DOI
selection.

- The selected summary's identity, content, DOI, and timestamps must remain
  aligned.
- Do not change DOI matching and de-duplication semantics in this phase.
- A safely unchanged publication avoids writing `DimProfileOnlyPublication`.
- Cover duplicate summaries/groups, DOI-only changes, title/year/type changes,
  timestamp ordering, and deletion.

## Phase 10 - Datasets from ORCID Works

Apply comparison before updating the dataset or its related web link.

- Ensure the related `DimWebLink` is only reconciled when the parent ORCID
  dataset item is processed as changed.
- Preserve add/update/remove URL behavior for changed items.
- Verify the dataset's publication date conversion remains unchanged.
- Cover URL-only changes, dataset field changes, unchanged data, absent URL,
  and deletion.

## Phase 11 - Funding

Apply comparison before organization disambiguation, reference-data use, date
lookups, URL reconciliation, and `FactFieldValue` recreation.

- Preserve the existing organization and identifierless transitions.
- Preserve behavior of the funding-detail import initiated by
  `ImportAdditionalData`.
- Define and test which ORCID timestamp represents the full funding data
  available from summary plus optional detail calls. Never skip the detail path
  if it can provide a mutation not represented by the compared summary
  timestamp.
- Cover award/contract/grant/salary types, organization transitions, URL
  changes, date changes, missing detail data, unchanged items, and deletion.

## Phase 12 - Research Activities from ORCID Works

Apply comparison to work summaries mapped to research activities.

- Reuse Phase 9's verified grouped-work selection rules without treating
  publication, dataset, and activity categories as interchangeable.
- Skip organization/date/URL processing for unchanged items.
- Cover each mapped ORCID work type and deletion.

## Phase 13 - Affiliation-Based Research Activities

Apply comparison independently to distinctions, invited positions,
memberships, peer reviews, qualifications, and services.

- Keep the single persisted research-activity representation but test every
  source JSON shape because their organization, date, role, and URL fields
  are located differently.
- Preserve actor-reference-data assignment, organization transitions,
  identifierless data, department handling, and related URLs for changed
  items.
- Do not let the first representative's timestamp stand in for another item.
- Cover one mixed payload containing changed and unchanged items from multiple
  ORCID activity sections.

## Testing Strategy

### Phase 0 parser and persistence tests

For each parser entry:

- parses valid UTC millisecond timestamps;
- returns null for explicitly null or absent timestamp properties;
- does not use a parent/group timestamp for the wrong item;
- chooses timestamps from the same summary as the payload fields.

For each importer entry:

- sets `Created` and `Modified` on insert;
- refreshes `Created` and `Modified` correctly on full update;
- retains existing `Created` when an incoming created timestamp is missing;
- uses the runtime fallback without allowing skipped processing when an
  incoming modified timestamp is missing;
- preserves both fields if a `FactFieldValue` is recreated.

### Per-type optimization tests

Every Phase 1-13 must add focused importer tests covering:

1. new item: full insert;
2. unchanged item with equal timestamp: no domain update or expensive lookup;
3. changed item with newer timestamp: full update;
4. stale item with older timestamp: no stale overwrite;
5. missing incoming timestamp: safe full update;
6. legacy existing row with missing stored timestamp: safe backfill and update;
7. deleted item: removed even though other items may be skipped;
8. mixed payload: changed, unchanged, new, and deleted items together;
9. fallback preservation of user-controlled `Show` and `PrimaryValue`;
10. phase-specific related entities and transition behavior.

Use change-tracker state or instrumented dependencies where appropriate to
prove that the unchanged path does not perform entity mutations or expensive
lookups. Do not rely only on final persisted values, because a rewritten value
can mask an optimization regression.

Keep focused JSON fixtures small. Add timestamp-only changes separately from
payload changes so tests distinguish source metadata changes from data changes.

## Observability and Verification

Add phase-specific counters or structured diagnostic fields only where
consistent with existing logging:

- items considered unchanged;
- items processed because timestamp was newer;
- items processed because timestamp data was missing/untrusted;
- items inserted;
- items removed.

Do not log ORCID payload values. Log counts and item type only.

Run for every phase:

- the targeted parser tests;
- the ORCID import service tests;
- the full API test suite;
- a representative import twice against the same payload to verify that the
  second run performs fewer writes/lookups while producing the same database
  state.

## Deployment and Rollout

- Deploy one type phase at a time, after Phase 0 is complete and verified.
- Prefer a feature flag or otherwise easily reversible switch for the first
  update-skipping phase. Remove it once the behavior is proven if the
  repository does not normally retain per-feature flags.
- Monitor import errors, counts, database writes, and user profile reconciliation
  after each release.
- Roll back the phase if missing/deleted data is detected; retain Phase 0
  timestamp population because it is data-correctness work rather than an
  optimization.
- Do not proceed to the next type solely because tests pass. Require a
  successful production observation for the current phase.

## Risks and Mitigations

| Risk | Mitigation |
| --- | --- |
| ORCID does not provide timestamps at the assumed JSON level | Verify real fixtures and parser shape in Phase 0; retain explicit full-processing fallback. |
| Grouped work summaries select a representative inconsistently | Keep payload and timestamp extraction in the same selected summary and add grouped-work tests. |
| Skipped items are later removed as missing | Mark every matched incoming item processed before timestamp comparison. |
| Recreated `FactFieldValue` rows lose metadata | Centralize or explicitly test timestamp copying for composite-key replacement paths. |
| Organization state changes outside the ORCID item | Keep organization-reference changes out of the fast path only after validating whether disambiguation metadata is timestamp-covered; otherwise force full processing for affected types. |
| Funding detail data changes independently of summary metadata | Document and test the relationship before enabling the funding fast path; fall back to full processing if uncertain. |
| Email put codes are frequently null | Make matching correctness a prerequisite within the email phase. |
| Older code or legacy rows have null timestamps | Treat as non-comparable and full-process them until timestamp data is backfilled. |
| Timestamp values collide at millisecond precision or clocks differ | Use `<=` idempotence and rely only on ORCID-provided timestamps converted to UTC; never compare local application time to remote item time. |
| A broad refactor obscures production regressions | Keep each type's rollout independent and avoid unrelated refactoring in the same phase. |

## Acceptance Criteria

### Phase 0

- Every ORCID item mapped to a `FactFieldValue` has a documented timestamp
  source and fallback behavior.
- All ORCID `FactFieldValue` insert, update, and recreation paths populate
  `Created` and `Modified` correctly.
- No item update logic is skipped solely because of timestamps.
- Focused tests and the full suite pass.
- Production audit confirms timestamp completeness and correctness.

### Each optimization phase

- The target type is unchanged functionally for new, changed, stale, missing,
  and deleted item cases.
- A repeated identical import performs no domain mutation or phase-specific
  expensive lookup for safely comparable items.
- Related child entities and organization transitions continue to work for
  changed items.
- Deletion processing remains correct in mixed payloads.
- Focused and full tests pass, and the phase is verified in production before
  the next phase starts.

## Likely Files to Change

- `aspnetcore/src/api/Models/Orcid/` - parsed item timestamp properties
- `aspnetcore/src/api/Services/OrcidJsonParserService.cs` - timestamp extraction
  and grouped-summary selection
- `aspnetcore/src/api/Services/IOrcidJsonParserService.cs` - contract changes if
  method signatures or model-level interfaces require them
- `aspnetcore/src/api/Services/OrcidImportService.cs` - `FactFieldValue` timestamp
  population and per-type fast paths
- `aspnetcore/src/api/Services/UserProfileService.cs` - empty
  `FactFieldValue`/domain-model defaults only if timestamp initialization belongs
  there
- `aspnetcore/src/api.Tests/Services_Tests/OrcidJsonParserServiceTest.cs` -
  parser timestamp tests
- `aspnetcore/src/api.Tests/Services_Tests/OrcidImportServiceTest.cs` -
  persistence, skip-path, and regression tests
- `aspnetcore/src/api.Tests/Infrastructure/orcid_fixtures/` - focused fixtures
  for each item type

## Sequencing

1. Complete and deploy Phase 0.
2. Validate Phase 0 in production and retain its timestamp data.
3. Implement one optimization phase per release, in the numbered order unless
   production evidence justifies changing priority.
4. Record per-phase evidence and any durable timestamp-handling lesson in the
   plan and repository memory as work proceeds.
5. Do not mark this plan Implemented until all desired type phases and their
   production verification are complete; unfinished phases keep the plan at root
   level as `In Progress`.
