using Xunit;
using api.Services;
using api.Models.Common;
using api.Models.Ttv;
using api.Models.Log;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace api.Tests
{
    [Collection("OrcidImportService tests")]
    public class OrcidImportServiceTests : IDisposable
    {
        // Interceptor that disables FK constraints after every connection open.
        // EF Core SQLite sets PRAGMA foreign_keys = ON on each open; we override it because
        // the service code and seed data use -1 as a sentinel for unused FK columns.
        private sealed class FkOffInterceptor : DbConnectionInterceptor
        {
            public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData) =>
                DisableFk(connection);
            public override Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
            {
                DisableFk(connection);
                return Task.CompletedTask;
            }
            private static void DisableFk(DbConnection connection)
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "PRAGMA foreign_keys = OFF;";
                cmd.ExecuteNonQuery();
            }
        }

        private const int OrcidDataSourceId = 1;
        private const int UserProfileId = 1;
        private const int KnownPersonId = 1;
        private const int OrcidOrganizationId = -1;

        // Seeded organization used by Priority 2 org-linking tests
        private const int TestOrgId = 100;
        private const int TestOrgDimPidId = 1000;

        private static readonly FkOffInterceptor _fkOffInterceptor = new();

        // Persistent in-memory SQLite connection per test DB name.
        // Passing a live SqliteConnection to UseSqlite() keeps one connection open for the
        // entire test, so EF Core never closes/reopens it and data written by SaveChanges
        // is always visible to the next query — even after ChangeTracker.Clear().
        private readonly Dictionary<string, SqliteConnection> _connections = new();

        private TtvContext CreateContext(string dbName)
        {
            if (!_connections.TryGetValue(dbName, out var conn))
            {
                conn = new SqliteConnection("DataSource=:memory:");
                conn.Open();
                // Disable FK constraints once on the persistent connection.
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "PRAGMA foreign_keys = OFF;";
                cmd.ExecuteNonQuery();
                _connections[dbName] = conn;
            }
            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseSqlite(conn)
                .Options;
            var context = new TtvContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        public void Dispose()
        {
            foreach (var conn in _connections.Values)
                try { conn.Dispose(); } catch { }
            _connections.Clear();
        }

        private async Task SeedRequiredData(TtvContext context)
        {
            context.DimOrganizations.Add(new DimOrganization
            {
                Id = OrcidOrganizationId,
                NameFi = "ORCID", NameEn = "ORCID", NameSv = "ORCID",
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = ""
            });
            context.DimRegisteredDataSources.Add(new DimRegisteredDataSource
            {
                Id = OrcidDataSourceId,
                DimOrganizationId = OrcidOrganizationId,
                Name = "ORCID",
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                Created = DateTime.UtcNow
            });
            context.DimKnownPeople.Add(new DimKnownPerson
            {
                Id = KnownPersonId,
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = "",
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            });
            context.DimUserProfiles.Add(new DimUserProfile
            {
                Id = UserProfileId,
                DimKnownPersonId = KnownPersonId,
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = "",
                AllowAllSubscriptions = false,
                Hidden = false,
                PublishNewOrcidData = false,
                HighlightOpeness = false,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            });

            int dfdsId = 1;
            foreach (var fi in new[]
            {
                Constants.FieldIdentifiers.PERSON_NAME,
                Constants.FieldIdentifiers.PERSON_OTHER_NAMES,
                Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER,
                Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION,
                Constants.FieldIdentifiers.PERSON_KEYWORD,
                Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE,
                Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS,
                Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER,
                Constants.FieldIdentifiers.PERSON_WEB_LINK,
                Constants.FieldIdentifiers.ACTIVITY_AFFILIATION,
                Constants.FieldIdentifiers.ACTIVITY_EDUCATION,
                Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY,
                Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET,
                Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION,
                Constants.FieldIdentifiers.ACTIVITY_RESEARCH_ACTIVITY
            })
            {
                context.DimFieldDisplaySettings.Add(new DimFieldDisplaySetting
                {
                    Id = dfdsId++,
                    DimUserProfileId = UserProfileId,
                    FieldIdentifier = fi,
                    Show = false,
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = ""
                });
            }

            var seenCodeValues = new HashSet<string>();
            int refId = 1;
            foreach (var (codeValue, name) in new (string, string)[]
            {
                (Constants.OrcidFundingType_To_ReferenceDataCodeValue.AWARD,    "Award"),
                (Constants.OrcidFundingType_To_ReferenceDataCodeValue.CONTRACT, "Contract"),
                (Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.DISTINCTION,       "Distinction"),
                (Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.INVITED_POSITION,  "Invited position"),
                (Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.MEMBERSHIP,        "Membership / Service"),
                (Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.PEER_REVIEW,       "Peer review"),
                (Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.QUALIFICATION,     "Qualification"),
                (Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.WORK_CONFERENCE,   "Conference work"),
                (Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.WORK_LECTURE_SPEECH, "Lecture/speech"),
                (Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.WORK_OTHER,        "Other"),
                (Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.WORK_SUPERVISED_STUDENT_PUBLICATION, "Supervised student publication"),
                (Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.WORK_TRANSLATION,  "Translation"),
            })
            {
                if (seenCodeValues.Add(codeValue))
                    context.DimReferencedata.Add(new DimReferencedatum
                    {
                        Id = refId++,
                        CodeScheme = Constants.ReferenceDataCodeSchemes.ORCID_FUNDING,
                        CodeValue = codeValue,
                        NameEn = name, NameFi = name, NameSv = name,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = "",
                        DimReferencedataId = -1
                    });
            }

            await context.SaveChangesAsync();

            // -------------------------------------------------------------------
            // Sentinel entities with id = -1.
            // FactFieldValues uses -1 as the "unused FK" sentinel for all FK
            // columns that don't apply to a given data item.  EF Core generates
            // INNER JOINs for required (non-nullable) FK relationships in the
            // Include chain, so every table in that chain must have a row with
            // id = -1 or the FFV will be filtered out and dedup fails.
            // FK constraints are off (FkOffInterceptor), so these rows don't need
            // to point to valid FK targets.
            // -------------------------------------------------------------------
            const string src = Constants.SourceIdentifiers.PROFILE_API;
            context.DimDates.Add(new DimDate
            {
                Id = -1, Year = 0, Month = 0, Day = 0,
                SourceId = src, SourceDescription = ""
            });
            context.DimNames.Add(new DimName
            {
                Id = -1, LastName = "", FirstNames = "",
                DimKnownPersonIdConfirmedIdentity = -1, DimRegisteredDataSourceId = -1,
                SourceId = src, SourceDescription = ""
            });
            context.DimWebLinks.Add(new DimWebLink
            {
                Id = -1, Url = "", LinkLabel = "", LinkType = "", LanguageVariant = "",
                SourceId = src, SourceDescription = ""
            });            context.DimFundingDecisions.Add(new DimFundingDecision
            {
                Id = -1,
                DimDateIdApproval = -1, DimDateIdStart = -1, DimDateIdEnd = -1,
                DimNameIdContactPerson = -1, DimCallProgrammeId = -1, DimGeoId = -1,
                DimTypeOfFundingId = -1, DimFundingDecisionIdParentDecision = -1,
                DimPidPidContent = "", FunderProjectNumber = "", Acronym = "",
                NameFi = "", NameSv = "", NameEn = "", NameUnd = "",
                DescriptionFi = "", DescriptionEn = "", DescriptionSv = "",
                AmountInEur = 0, FundingDecisionCurrencyAbbreviation = "",
                DimRegisteredDataSourceId = -1,
                SourceId = src, SourceDescription = ""
            });
            context.DimProfileOnlyFundingDecisions.Add(new DimProfileOnlyFundingDecision
            {
                Id = -1,
                DimDateIdApproval = -1, DimDateIdStart = -1, DimDateIdEnd = -1,
                DimCallProgrammeId = -1, DimTypeOfFundingId = -1,
                OrcidWorkType = "", FunderProjectNumber = "", Acronym = "",
                NameFi = "", NameSv = "", NameEn = "", NameUnd = "",
                DescriptionFi = "", DescriptionEn = "",
                SourceId = src, SourceDescription = ""
            });
            context.DimProfileOnlyDatasets.Add(new DimProfileOnlyDataset
            {
                Id = -1, OrcidWorkType = "", LocalIdentifier = "",
                NameFi = "", NameSv = "", NameEn = "", NameUnd = "",
                DescriptionFi = "", DescriptionSv = "", DescriptionEn = "", DescriptionUnd = "",
                VersionInfo = "", DimRegisteredDataSourceId = -1,
                SourceId = src, SourceDescription = ""
            });
            context.DimProfileOnlyResearchActivities.Add(new DimProfileOnlyResearchActivity
            {
                Id = -1,
                DimDateIdStart = -1, DimDateIdEnd = -1,
                DimOrganizationId = -1, DimEventId = -1,
                LocalIdentifier = "", OrcidWorkType = "",
                NameFi = "", NameSv = "", NameEn = "", NameUnd = "",
                DescriptionFi = "", DescriptionEn = "", DescriptionSv = "",
                IndentifierlessTargetOrg = "",
                SourceId = src, SourceDescription = ""
            });
            context.DimProfileOnlyPublications.Add(new DimProfileOnlyPublication
            {
                Id = -1,
                ParentTypeClassificationCode = 0, TypeClassificationCode = 0,
                PublicationFormatCode = 0, ArticleTypeCode = 0, TargetAudienceCode = 0,
                OrcidWorkType = "", PublicationName = "", ConferenceName = "",
                ShortDescription = "", PublicationId = "", AuthorsText = "",
                PageNumberText = "", ArticleNumberText = "", IssueNumber = "",
                Volume = "", PublicationCountryCode = 0,
                PublisherName = "", PublisherLocation = "",
                ParentPublicationName = "", ParentPublicationEditors = "",
                LicenseCode = 0, OpenAccessCode = "", OriginalPublicationId = "",
                ThesisTypeCode = 0, DoiHandle = "", LanguageCode = 0,
                DimKnownPersonId = -1, DimRegisteredDataSourceId = -1,
                SourceId = src, SourceDescription = ""
            });
            context.DimPids.Add(new DimPid
            {
                Id = -1, PidContent = "", PidType = "",
                DimOrganizationId = -1, DimKnownPersonId = -1, DimPublicationId = -1,
                DimServiceId = -1, DimInfrastructureId = -1, DimPublicationChannelId = -1,
                DimResearchDatasetId = -1, DimResearchDataCatalogId = -1,
                DimResearchActivityId = -1, DimEventId = -1, DimProfileOnlyPublicationId = -1,
                SourceId = src, SourceDescription = ""
            });
            context.DimResearchActivities.Add(new DimResearchActivity
            {
                Id = -1, LocalIdentifier = "", InternationalCollaboration = false,
                NameFi = "", NameSv = "", NameEn = "", NameUnd = "",
                DescriptionFi = "", DescriptionEn = "", DescriptionSv = "",
                IndentifierlessTargetOrg = "",
                DimStartDate = -1, DimEndDate = -1,
                SourceId = src, SourceDescription = ""
            });
            context.DimEducations.Add(new DimEducation
            {
                Id = -1, LocalIdentifier = "",
                NameFi = "", NameSv = "", NameEn = "",
                DescriptionFi = "", DescriptionSv = "", DescriptionEn = "",
                DegreeNameFi = "", DegreeNameSv = "", DegreeNameEn = "",
                DimKnownPersonId = -1, DimRegisteredDataSourceId = -1,
                SourceId = src, SourceDescription = ""
            });
            context.DimAffiliations.Add(new DimAffiliation
            {
                Id = -1, DimKnownPersonId = -1, DimOrganizationId = -1, StartDate = -1,
                PositionNameFi = "", PositionNameEn = "", PositionNameSv = "",
                AffiliationTypeFi = "", AffiliationTypeEn = "", AffiliationTypeSv = "",
                LocalIdentifier = "", DimRegisteredDataSourceId = -1,
                SourceId = src, SourceDescription = ""
            });
            context.DimTelephoneNumbers.Add(new DimTelephoneNumber
            {
                Id = -1, TelephoneNumber = "",
                DimRegisteredDataSourceId = -1, DimKnownPersonId = -1, DimContactInformationId = -1,
                SourceId = src, SourceDescription = ""
            });
            context.DimEmailAddrresses.Add(new DimEmailAddrress
            {
                Id = -1, Email = "",
                DimRegisteredDataSourceId = -1, DimKnownPersonId = -1, DimContactInformationId = -1,
                SourceId = src, SourceDescription = ""
            });
            context.DimResearcherDescriptions.Add(new DimResearcherDescription
            {
                Id = -1,
                ResearchDescriptionFi = "", ResearchDescriptionEn = "", ResearchDescriptionSv = "",
                DimKnownPersonId = -1, DimRegisteredDataSourceId = -1,
                SourceId = src, SourceDescription = ""
            });
            context.DimIdentifierlessData.Add(new DimIdentifierlessDatum
            {
                Id = -1, Type = "",
                ValueFi = "", ValueEn = "", ValueSv = "", ValueUnd = "", UnlinkedIdentifier = "",
                DimOrganizationId = -1,
                SourceId = src, SourceDescription = ""
            });
            context.DimKeywords.Add(new DimKeyword
            {
                Id = -1, Keyword = "", Scheme = "", Language = "",
                ConceptUri = "", SchemeUri = "",
                DimRegisteredDataSourceId = -1,
                SourceId = src, SourceDescription = ""
            });
            await context.SaveChangesAsync();
        }

        private OrcidImportService CreateService(TtvContext context)
        {
            var utilityService = new UtilityService();
            var userProfileService = new UserProfileService(
                utilityService: utilityService,
                logger: new NullLogger<UserProfileService>());
            var organizationHandlerService = new OrganizationHandlerService(
                ttvContext: context,
                utilityService: utilityService);
            return new OrcidImportService(
                ttvContext: context,
                userProfileService: userProfileService,
                orcidApiService: null,
                orcidJsonParserService: new OrcidJsonParserService(),
                organizationHandlerService: organizationHandlerService,
                utilityService: utilityService,
                dataSourceHelperService: new DataSourceHelperService { DimRegisteredDataSourceId_ORCID = OrcidDataSourceId },
                logger: new NullLogger<OrcidImportService>());
        }

        private string LoadFixture(string fileName)
        {
            string dir = Path.Combine(
                Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName,
                "Infrastructure", "orcid_fixtures");
            return File.ReadAllText(Path.Combine(dir, fileName));
        }

        private LogUserIdentification LogId => new("0000-0001-0001-0001");

        // Seeds a real DimOrganization (Id=100) linked via a RinggoldID DimPid (Id=1000,
        // PidContent="12345"). Tests that need the org-not-found path simply don't call this.
        private async Task SeedOrgWithRinggoldId(TtvContext context)
        {
            context.DimOrganizations.Add(new DimOrganization
            {
                Id = TestOrgId,
                NameFi = "Test Organization",
                NameEn = "Test Organization",
                NameSv = "Test Organization",
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = ""
            });
            context.DimPids.Add(new DimPid
            {
                Id = TestOrgDimPidId,
                PidContent = "12345",
                PidType = "RinggoldID",
                DimOrganizationId = TestOrgId,
                DimKnownPersonId = -1,
                DimPublicationId = -1,
                DimServiceId = -1,
                DimInfrastructureId = -1,
                DimPublicationChannelId = -1,
                DimResearchDatasetId = -1,
                DimResearchDataCatalogId = -1,
                DimResearchActivityId = -1,
                DimEventId = -1,
                DimProfileOnlyDatasetId = -1,
                DimProfileOnlyFundingDecisionId = -1,
                DimProfileOnlyPublicationId = -1,
                DimResearchCommunityId = -1,
                DimResearchProjectId = -1,
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = "",
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        // -------------------------------------------------------------------------
        // Section: person / name
        // -------------------------------------------------------------------------

        [Fact(DisplayName = "Name: imported once, not duplicated on re-import")]
        public async Task Name_ImportedAndNotDuplicated()
        {
            string dbName = nameof(Name_ImportedAndNotDuplicated);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);
            string json = LoadFixture("name.json");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterFirst = context.DimNames.Count(e => e.Id > 0);

            var importedName = context.DimNames.Single(e => e.Id > 0);
            Assert.Equal("Alice", importedName.FirstNames);
            Assert.Equal("Smith", importedName.LastName);
            Assert.Equal(KnownPersonId, importedName.DimKnownPersonIdConfirmedIdentity);
            Assert.Equal(OrcidDataSourceId, importedName.DimRegisteredDataSourceId);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterSecond = context.DimNames.Count(e => e.Id > 0);

            Assert.Equal(1, countAfterFirst);
            Assert.Equal(countAfterFirst, countAfterSecond);
        }

        // -------------------------------------------------------------------------
        // Section: person / other-names  (2 entries)
        // -------------------------------------------------------------------------

        [Fact(DisplayName = "Other names: 2 imported, not duplicated on re-import")]
        public async Task OtherNames_ImportedAndNotDuplicated()
        {
            string dbName = nameof(OtherNames_ImportedAndNotDuplicated);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);
            string json = LoadFixture("other_names.json");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            // 1 main name + 2 other-names
            int countAfterFirst = context.DimNames.Count(e => e.Id > 0);

            var otherNames = context.DimNames.Where(e => e.Id > 0 && e.FullName != null).OrderBy(e => e.Id).ToList();
            Assert.Equal(2, otherNames.Count);
            Assert.Equal("Alice S.", otherNames[0].FullName);
            Assert.Equal("A. Smith", otherNames[1].FullName);
            Assert.All(otherNames, n => Assert.Equal(KnownPersonId, n.DimKnownPersonIdConfirmedIdentity));
            Assert.All(otherNames, n => Assert.Equal(OrcidDataSourceId, n.DimRegisteredDataSourceId));

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterSecond = context.DimNames.Count(e => e.Id > 0);

            Assert.Equal(3, countAfterFirst);
            Assert.Equal(countAfterFirst, countAfterSecond);
        }

        // -------------------------------------------------------------------------
        // Section: person / biography
        // -------------------------------------------------------------------------

        [Fact(DisplayName = "Biography: imported once, not duplicated on re-import")]
        public async Task Biography_ImportedAndNotDuplicated()
        {
            string dbName = nameof(Biography_ImportedAndNotDuplicated);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);
            string json = LoadFixture("biography.json");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterFirst = context.DimResearcherDescriptions.Count(e => e.Id > 0);

            var biography = context.DimResearcherDescriptions.Single(e => e.Id > 0);
            Assert.Equal("Test researcher biography content.", biography.ResearchDescriptionEn);
            Assert.Equal(KnownPersonId, biography.DimKnownPersonId);
            Assert.Equal(OrcidDataSourceId, biography.DimRegisteredDataSourceId);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterSecond = context.DimResearcherDescriptions.Count(e => e.Id > 0);

            Assert.Equal(1, countAfterFirst);
            Assert.Equal(countAfterFirst, countAfterSecond);
        }

        // -------------------------------------------------------------------------
        // Section: person / researcher-urls  (1 entry)
        // -------------------------------------------------------------------------

        [Fact(DisplayName = "Researcher URL: imported once, not duplicated on re-import")]
        public async Task ResearcherUrl_ImportedAndNotDuplicated()
        {
            string dbName = nameof(ResearcherUrl_ImportedAndNotDuplicated);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);
            string json = LoadFixture("researcher_url.json");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterFirst = context.DimWebLinks.Count(e => e.Id > 0);

            var webLink = context.DimWebLinks.Single(e => e.Id > 0);
            Assert.Equal("https://example.com/profile", webLink.Url);
            Assert.Equal("Homepage", webLink.LinkLabel);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterSecond = context.DimWebLinks.Count(e => e.Id > 0);

            Assert.Equal(1, countAfterFirst);
            Assert.Equal(countAfterFirst, countAfterSecond);
        }

        // -------------------------------------------------------------------------
        // Section: person / emails  (1 entry)
        // -------------------------------------------------------------------------

        [Fact(DisplayName = "Email: imported once, not duplicated on re-import")]
        public async Task Email_ImportedAndNotDuplicated()
        {
            string dbName = nameof(Email_ImportedAndNotDuplicated);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);
            string json = LoadFixture("email.json");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterFirst = context.DimEmailAddrresses.Count(e => e.Id > 0);

            var email = context.DimEmailAddrresses.Single(e => e.Id > 0);
            Assert.Equal("alice@example.com", email.Email);
            Assert.Equal(KnownPersonId, email.DimKnownPersonId);
            Assert.Equal(OrcidDataSourceId, email.DimRegisteredDataSourceId);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterSecond = context.DimEmailAddrresses.Count(e => e.Id > 0);

            Assert.Equal(1, countAfterFirst);
            Assert.Equal(countAfterFirst, countAfterSecond);
        }

        // -------------------------------------------------------------------------
        // Section: person / keywords  (2 entries)
        // -------------------------------------------------------------------------

        [Fact(DisplayName = "Keywords: 2 imported, not duplicated on re-import")]
        public async Task Keywords_ImportedAndNotDuplicated()
        {
            string dbName = nameof(Keywords_ImportedAndNotDuplicated);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);
            string json = LoadFixture("keywords.json");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterFirst = context.DimKeywords.Count(e => e.Id > 0);

            var keywords = context.DimKeywords.Where(e => e.Id > 0).OrderBy(e => e.Id).ToList();
            Assert.Equal("machine learning", keywords[0].Keyword);
            Assert.Equal("data science", keywords[1].Keyword);
            Assert.All(keywords, k => Assert.Equal(OrcidDataSourceId, k.DimRegisteredDataSourceId));

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterSecond = context.DimKeywords.Count(e => e.Id > 0);

            Assert.Equal(2, countAfterFirst);
            Assert.Equal(countAfterFirst, countAfterSecond);
        }

        // -------------------------------------------------------------------------
        // Section: person / external-identifiers  (1 entry)
        // -------------------------------------------------------------------------

        [Fact(DisplayName = "External identifier: imported once, not duplicated on re-import")]
        public async Task ExternalIdentifier_ImportedAndNotDuplicated()
        {
            string dbName = nameof(ExternalIdentifier_ImportedAndNotDuplicated);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);
            string json = LoadFixture("external_identifier.json");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            // Service creates 1 DimPid for the external id value and 1 for its put-code.
            // Assert only the external-id one (non-put-code).
            int countAfterFirst = context.DimPids.Count(p => p.Id > 0 && p.PidType != Constants.PidTypes.ORCID_PUT_CODE);

            var externalId = context.DimPids.Single(p => p.Id > 0 && p.PidType != Constants.PidTypes.ORCID_PUT_CODE);
            Assert.Equal("99999", externalId.PidContent);
            Assert.Equal("Loop profile", externalId.PidType);
            Assert.Equal(KnownPersonId, externalId.DimKnownPersonId);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterSecond = context.DimPids.Count(p => p.Id > 0 && p.PidType != Constants.PidTypes.ORCID_PUT_CODE);

            Assert.Equal(1, countAfterFirst);
            Assert.Equal(countAfterFirst, countAfterSecond);
        }

        // -------------------------------------------------------------------------
        // Section: activities / educations  (1 entry)
        // -------------------------------------------------------------------------

        [Fact(DisplayName = "Education: imported once, not duplicated on re-import")]
        public async Task Education_ImportedAndNotDuplicated()
        {
            string dbName = nameof(Education_ImportedAndNotDuplicated);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);
            string json = LoadFixture("education.json");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterFirst = context.DimEducations.Count(e => e.Id > 0);

            var education = context.DimEducations.Single(e => e.Id > 0);
            Assert.Equal("BSc", education.NameEn);
            Assert.Equal("Test University", education.DegreeGrantingInstitutionName);
            Assert.Equal(KnownPersonId, education.DimKnownPersonId);
            Assert.Equal(OrcidDataSourceId, education.DimRegisteredDataSourceId);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterSecond = context.DimEducations.Count(e => e.Id > 0);

            Assert.Equal(1, countAfterFirst);
            Assert.Equal(countAfterFirst, countAfterSecond);
        }

        // -------------------------------------------------------------------------
        // Section: activities / employments  (1 entry, no disambiguated org)
        // -------------------------------------------------------------------------

        [Fact(DisplayName = "Employment: imported once, not duplicated on re-import")]
        public async Task Employment_ImportedAndNotDuplicated()
        {
            string dbName = nameof(Employment_ImportedAndNotDuplicated);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);
            string json = LoadFixture("employment.json");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterFirst = context.DimAffiliations.Count(e => e.Id > 0);

            var affiliation = context.DimAffiliations.Single(e => e.Id > 0);
            Assert.Equal("Researcher", affiliation.PositionNameEn);
            Assert.Equal(KnownPersonId, affiliation.DimKnownPersonId);
            Assert.Equal(OrcidDataSourceId, affiliation.DimRegisteredDataSourceId);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterSecond = context.DimAffiliations.Count(e => e.Id > 0);

            Assert.Equal(1, countAfterFirst);
            Assert.Equal(countAfterFirst, countAfterSecond);
        }

        // -------------------------------------------------------------------------
        // Section: activities / works  (1 journal-article + 1 data-set)
        // -------------------------------------------------------------------------

        [Fact(DisplayName = "Publication and dataset: imported once, not duplicated on re-import")]
        public async Task PublicationAndDataset_ImportedAndNotDuplicated()
        {
            string dbName = nameof(PublicationAndDataset_ImportedAndNotDuplicated);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);
            string json = LoadFixture("works.json");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int pubCountAfterFirst = context.DimProfileOnlyPublications.Count(e => e.Id > 0);
            int dsCountAfterFirst  = context.DimProfileOnlyDatasets.Count(e => e.Id > 0);

            var publication = context.DimProfileOnlyPublications.Single(e => e.Id > 0);
            Assert.Equal("Test Publication", publication.PublicationName);
            Assert.Equal("journal-article", publication.OrcidWorkType);
            Assert.Equal("10.1000/test1", publication.DoiHandle);
            Assert.Equal(OrcidDataSourceId, publication.DimRegisteredDataSourceId);

            var dataset = context.DimProfileOnlyDatasets.Single(e => e.Id > 0);
            Assert.Equal("Test Dataset", dataset.NameEn);
            Assert.Equal("data-set", dataset.OrcidWorkType);
            Assert.Equal(OrcidDataSourceId, dataset.DimRegisteredDataSourceId);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int pubCountAfterSecond = context.DimProfileOnlyPublications.Count(e => e.Id > 0);
            int dsCountAfterSecond  = context.DimProfileOnlyDatasets.Count(e => e.Id > 0);

            Assert.Equal(1, pubCountAfterFirst);
            Assert.Equal(1, dsCountAfterFirst);
            Assert.Equal(pubCountAfterFirst, pubCountAfterSecond);
            Assert.Equal(dsCountAfterFirst, dsCountAfterSecond);
        }

        // -------------------------------------------------------------------------
        // Section: activities / fundings  (1 entry)
        // -------------------------------------------------------------------------

        [Fact(DisplayName = "Funding: imported once, not duplicated on re-import")]
        public async Task Funding_ImportedAndNotDuplicated()
        {
            string dbName = nameof(Funding_ImportedAndNotDuplicated);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);
            string json = LoadFixture("funding.json");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterFirst = context.DimProfileOnlyFundingDecisions.Count(e => e.Id > 0);

            var funding = context.DimProfileOnlyFundingDecisions.Single(e => e.Id > 0);
            Assert.Equal("Test Grant", funding.NameEn);
            Assert.Equal(OrcidDataSourceId, funding.DimRegisteredDataSourceId);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterSecond = context.DimProfileOnlyFundingDecisions.Count(e => e.Id > 0);

            Assert.Equal(1, countAfterFirst);
            Assert.Equal(countAfterFirst, countAfterSecond);
        }

        // -------------------------------------------------------------------------
        // Section: activities / distinctions  (1 entry → DimProfileOnlyResearchActivity)
        // -------------------------------------------------------------------------

        [Fact(DisplayName = "Distinction: imported once, not duplicated on re-import")]
        public async Task Distinction_ImportedAndNotDuplicated()
        {
            string dbName = nameof(Distinction_ImportedAndNotDuplicated);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);
            string json = LoadFixture("distinction.json");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterFirst = context.DimProfileOnlyResearchActivities.Count(e => e.Id > 0);

            var distinction = context.DimProfileOnlyResearchActivities.Single(e => e.Id > 0);
            Assert.Equal("Best Researcher Award", distinction.NameEn);
            Assert.Equal("", distinction.OrcidWorkType);
            Assert.Equal(OrcidDataSourceId, distinction.DimRegisteredDataSourceId);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterSecond = context.DimProfileOnlyResearchActivities.Count(e => e.Id > 0);

            Assert.Equal(1, countAfterFirst);
            Assert.Equal(countAfterFirst, countAfterSecond);
        }

        // =========================================================================
        // PRIORITY 1A: Update tests — field values change between imports
        // =========================================================================

        [Fact(DisplayName = "Name: updated when changed in ORCID")]
        public async Task Name_Updated_WhenChangedInOrcid()
        {
            string dbName = nameof(Name_Updated_WhenChangedInOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("name.json"), LogId);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("name_updated.json"), LogId);

            Assert.Equal(1, context.DimNames.Count(e => e.Id > 0));
            var name = context.DimNames.Single(e => e.Id > 0);
            Assert.Equal("Bob", name.FirstNames);
            Assert.Equal("Jones", name.LastName);
        }

        [Fact(DisplayName = "Biography: updated when changed in ORCID")]
        public async Task Biography_Updated_WhenChangedInOrcid()
        {
            string dbName = nameof(Biography_Updated_WhenChangedInOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("biography.json"), LogId);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("biography_updated.json"), LogId);

            Assert.Equal(1, context.DimResearcherDescriptions.Count(e => e.Id > 0));
            Assert.Equal("Updated biography text.", context.DimResearcherDescriptions.Single(e => e.Id > 0).ResearchDescriptionEn);
        }

        [Fact(DisplayName = "Researcher URL: updated when changed in ORCID")]
        public async Task ResearcherUrl_Updated_WhenChangedInOrcid()
        {
            string dbName = nameof(ResearcherUrl_Updated_WhenChangedInOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("researcher_url.json"), LogId);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("researcher_url_updated.json"), LogId);

            Assert.Equal(1, context.DimWebLinks.Count(e => e.Id > 0));
            var webLink = context.DimWebLinks.Single(e => e.Id > 0);
            Assert.Equal("https://example.com/updated", webLink.Url);
            Assert.Equal("Updated Homepage", webLink.LinkLabel);
        }

        [Fact(DisplayName = "Keywords: updated when changed in ORCID")]
        public async Task Keywords_Updated_WhenChangedInOrcid()
        {
            string dbName = nameof(Keywords_Updated_WhenChangedInOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("keywords.json"), LogId);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("keywords_updated.json"), LogId);

            Assert.Equal(2, context.DimKeywords.Count(e => e.Id > 0));
            var keywords = context.DimKeywords.Where(e => e.Id > 0).Select(k => k.Keyword).ToHashSet();
            Assert.Contains("deep learning", keywords);
            Assert.Contains("statistics", keywords);
            Assert.DoesNotContain("machine learning", keywords);
            Assert.DoesNotContain("data science", keywords);
        }

        [Fact(DisplayName = "External identifier: updated when changed in ORCID")]
        public async Task ExternalIdentifier_Updated_WhenChangedInOrcid()
        {
            string dbName = nameof(ExternalIdentifier_Updated_WhenChangedInOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("external_identifier.json"), LogId);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("external_identifier_updated.json"), LogId);

            var externalId = context.DimPids.Single(p => p.Id > 0 && p.PidType != Constants.PidTypes.ORCID_PUT_CODE);
            Assert.Equal("88888", externalId.PidContent);
            Assert.Equal(1, context.DimPids.Count(p => p.Id > 0 && p.PidType != Constants.PidTypes.ORCID_PUT_CODE));
        }

        [Fact(DisplayName = "Education: updated when changed in ORCID")]
        public async Task Education_Updated_WhenChangedInOrcid()
        {
            string dbName = nameof(Education_Updated_WhenChangedInOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("education.json"), LogId);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("education_updated.json"), LogId);

            Assert.Equal(1, context.DimEducations.Count(e => e.Id > 0));
            var education = context.DimEducations.Single(e => e.Id > 0);
            Assert.Equal("MSc", education.NameEn);
            Assert.Equal("Updated University", education.DegreeGrantingInstitutionName);
        }

        [Fact(DisplayName = "Employment: updated when changed in ORCID")]
        public async Task Employment_Updated_WhenChangedInOrcid()
        {
            string dbName = nameof(Employment_Updated_WhenChangedInOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("employment.json"), LogId);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("employment_updated.json"), LogId);

            Assert.Equal(1, context.DimAffiliations.Count(e => e.Id > 0));
            Assert.Equal("Senior Researcher", context.DimAffiliations.Single(e => e.Id > 0).PositionNameEn);
        }

        [Fact(DisplayName = "Publication: updated when changed in ORCID")]
        public async Task Publication_Updated_WhenChangedInOrcid()
        {
            string dbName = nameof(Publication_Updated_WhenChangedInOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("works.json"), LogId);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("works_updated.json"), LogId);

            Assert.Equal(1, context.DimProfileOnlyPublications.Count(e => e.Id > 0));
            var publication = context.DimProfileOnlyPublications.Single(e => e.Id > 0);
            Assert.Equal("Updated Publication", publication.PublicationName);
            Assert.Equal("10.1000/updated1", publication.DoiHandle);
        }

        [Fact(DisplayName = "Dataset: updated when changed in ORCID")]
        public async Task Dataset_Updated_WhenChangedInOrcid()
        {
            string dbName = nameof(Dataset_Updated_WhenChangedInOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("works.json"), LogId);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("works_updated.json"), LogId);

            Assert.Equal(1, context.DimProfileOnlyDatasets.Count(e => e.Id > 0));
            Assert.Equal("Updated Dataset", context.DimProfileOnlyDatasets.Single(e => e.Id > 0).NameEn);
        }

        [Fact(DisplayName = "Funding: updated when changed in ORCID")]
        public async Task Funding_Updated_WhenChangedInOrcid()
        {
            string dbName = nameof(Funding_Updated_WhenChangedInOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("funding.json"), LogId);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("funding_updated.json"), LogId);

            Assert.Equal(1, context.DimProfileOnlyFundingDecisions.Count(e => e.Id > 0));
            Assert.Equal("Updated Grant", context.DimProfileOnlyFundingDecisions.Single(e => e.Id > 0).NameEn);
        }

        [Fact(DisplayName = "Distinction (research activity): updated when changed in ORCID")]
        public async Task Distinction_Updated_WhenChangedInOrcid()
        {
            string dbName = nameof(Distinction_Updated_WhenChangedInOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("distinction.json"), LogId);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("distinction_updated.json"), LogId);

            Assert.Equal(1, context.DimProfileOnlyResearchActivities.Count(e => e.Id > 0));
            Assert.Equal("Excellence Award", context.DimProfileOnlyResearchActivities.Single(e => e.Id > 0).NameEn);
        }

        // =========================================================================
        // PRIORITY 1B: Deletion tests — items removed from ORCID are deleted from DB
        // Strategy: import the section fixture, then re-import name.json (all
        // activity sections empty) — the service must remove the orphaned rows.
        // =========================================================================

        [Fact(DisplayName = "Other names: deleted when removed from ORCID")]
        public async Task OtherNames_Deleted_WhenRemovedFromOrcid()
        {
            string dbName = nameof(OtherNames_Deleted_WhenRemovedFromOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("other_names.json"), LogId);
            Assert.Equal(2, context.DimNames.Count(e => e.Id > 0 && e.FullName != null));

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("name.json"), LogId);

            Assert.Equal(0, context.DimNames.Count(e => e.Id > 0 && e.FullName != null));
            Assert.Equal(1, context.DimNames.Count(e => e.Id > 0)); // main name remains
        }

        [Fact(DisplayName = "Biography: deleted when removed from ORCID")]
        public async Task Biography_Deleted_WhenRemovedFromOrcid()
        {
            string dbName = nameof(Biography_Deleted_WhenRemovedFromOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("biography.json"), LogId);
            Assert.Equal(1, context.DimResearcherDescriptions.Count(e => e.Id > 0));

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("name.json"), LogId);

            Assert.Equal(0, context.DimResearcherDescriptions.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Researcher URL: deleted when removed from ORCID")]
        public async Task ResearcherUrl_Deleted_WhenRemovedFromOrcid()
        {
            string dbName = nameof(ResearcherUrl_Deleted_WhenRemovedFromOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("researcher_url.json"), LogId);
            Assert.Equal(1, context.DimWebLinks.Count(e => e.Id > 0));

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("name.json"), LogId);

            Assert.Equal(0, context.DimWebLinks.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Email: deleted when removed from ORCID")]
        public async Task Email_Deleted_WhenRemovedFromOrcid()
        {
            string dbName = nameof(Email_Deleted_WhenRemovedFromOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("email.json"), LogId);
            Assert.Equal(1, context.DimEmailAddrresses.Count(e => e.Id > 0));

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("name.json"), LogId);

            Assert.Equal(0, context.DimEmailAddrresses.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Keywords: deleted when removed from ORCID")]
        public async Task Keywords_Deleted_WhenRemovedFromOrcid()
        {
            string dbName = nameof(Keywords_Deleted_WhenRemovedFromOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("keywords.json"), LogId);
            Assert.Equal(2, context.DimKeywords.Count(e => e.Id > 0));

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("name.json"), LogId);

            Assert.Equal(0, context.DimKeywords.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "External identifier: deleted when removed from ORCID")]
        public async Task ExternalIdentifier_Deleted_WhenRemovedFromOrcid()
        {
            string dbName = nameof(ExternalIdentifier_Deleted_WhenRemovedFromOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("external_identifier.json"), LogId);
            Assert.Equal(1, context.DimPids.Count(p => p.Id > 0 && p.PidType != Constants.PidTypes.ORCID_PUT_CODE));

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("name.json"), LogId);

            Assert.Equal(0, context.DimPids.Count(p => p.Id > 0 && p.PidType != Constants.PidTypes.ORCID_PUT_CODE));
        }

        [Fact(DisplayName = "Education: deleted when removed from ORCID")]
        public async Task Education_Deleted_WhenRemovedFromOrcid()
        {
            string dbName = nameof(Education_Deleted_WhenRemovedFromOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("education.json"), LogId);
            Assert.Equal(1, context.DimEducations.Count(e => e.Id > 0));

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("name.json"), LogId);

            Assert.Equal(0, context.DimEducations.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Employment: deleted when removed from ORCID")]
        public async Task Employment_Deleted_WhenRemovedFromOrcid()
        {
            string dbName = nameof(Employment_Deleted_WhenRemovedFromOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("employment.json"), LogId);
            Assert.Equal(1, context.DimAffiliations.Count(e => e.Id > 0));

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("name.json"), LogId);

            Assert.Equal(0, context.DimAffiliations.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Publication and dataset: deleted when removed from ORCID")]
        public async Task PublicationAndDataset_Deleted_WhenRemovedFromOrcid()
        {
            string dbName = nameof(PublicationAndDataset_Deleted_WhenRemovedFromOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("works.json"), LogId);
            Assert.Equal(1, context.DimProfileOnlyPublications.Count(e => e.Id > 0));
            Assert.Equal(1, context.DimProfileOnlyDatasets.Count(e => e.Id > 0));

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("name.json"), LogId);

            Assert.Equal(0, context.DimProfileOnlyPublications.Count(e => e.Id > 0));
            Assert.Equal(0, context.DimProfileOnlyDatasets.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Funding: deleted when removed from ORCID")]
        public async Task Funding_Deleted_WhenRemovedFromOrcid()
        {
            string dbName = nameof(Funding_Deleted_WhenRemovedFromOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("funding.json"), LogId);
            Assert.Equal(1, context.DimProfileOnlyFundingDecisions.Count(e => e.Id > 0));

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("name.json"), LogId);

            Assert.Equal(0, context.DimProfileOnlyFundingDecisions.Count(e => e.Id > 0));
            // Org-name DimIdentifierlessDatum created for the unlinked funder must also be cleaned up
            Assert.Equal(0, context.DimIdentifierlessData.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Research activity: deleted when removed from ORCID")]
        public async Task ResearchActivity_Deleted_WhenRemovedFromOrcid()
        {
            string dbName = nameof(ResearchActivity_Deleted_WhenRemovedFromOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("distinction.json"), LogId);
            Assert.Equal(1, context.DimProfileOnlyResearchActivities.Count(e => e.Id > 0));

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("name.json"), LogId);

            Assert.Equal(0, context.DimProfileOnlyResearchActivities.Count(e => e.Id > 0));
        }

        // =========================================================================
        // PRIORITY 1C: DimWebLink lifecycle — add / update / remove on dataset,
        // funding, and research activity
        // =========================================================================

        // --- Dataset ---

        [Fact(DisplayName = "Dataset: DimWebLink created when URL is present on first import")]
        public async Task Dataset_WebLink_CreatedWithUrl()
        {
            string dbName = nameof(Dataset_WebLink_CreatedWithUrl);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("dataset_with_url.json"), LogId);

            Assert.Equal(1, context.DimWebLinks.Count(e => e.Id > 0));
            Assert.Equal("https://dataset.example.com", context.DimWebLinks.Single(e => e.Id > 0).Url);
            Assert.Equal(1, context.DimProfileOnlyDatasets.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Dataset: DimWebLink updated when URL changes on re-import")]
        public async Task Dataset_WebLink_UpdatedWhenUrlChanges()
        {
            string dbName = nameof(Dataset_WebLink_UpdatedWhenUrlChanges);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("dataset_with_url.json"), LogId);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("dataset_url_updated.json"), LogId);

            Assert.Equal(1, context.DimWebLinks.Count(e => e.Id > 0));
            Assert.Equal("https://dataset-updated.example.com", context.DimWebLinks.Single(e => e.Id > 0).Url);
        }

        [Fact(DisplayName = "Dataset: DimWebLink deleted when URL is cleared on re-import")]
        public async Task Dataset_WebLink_DeletedWhenUrlCleared()
        {
            string dbName = nameof(Dataset_WebLink_DeletedWhenUrlCleared);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            // First import: dataset 8002 with URL
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("dataset_with_url.json"), LogId);
            Assert.Equal(1, context.DimWebLinks.Count(e => e.Id > 0));

            // Second import: same put-code 8002, url=null (works.json)
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("works.json"), LogId);

            Assert.Equal(0, context.DimWebLinks.Count(e => e.Id > 0));
            Assert.Equal(1, context.DimProfileOnlyDatasets.Count(e => e.Id > 0)); // dataset row still present
        }

        // --- Funding ---

        [Fact(DisplayName = "Funding: DimWebLink created when URL is present on first import")]
        public async Task Funding_WebLink_CreatedWithUrl()
        {
            string dbName = nameof(Funding_WebLink_CreatedWithUrl);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("funding_with_url.json"), LogId);

            Assert.Equal(1, context.DimWebLinks.Count(e => e.Id > 0));
            Assert.Equal("https://funding.example.com", context.DimWebLinks.Single(e => e.Id > 0).Url);
            Assert.Equal(1, context.DimProfileOnlyFundingDecisions.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Funding: DimWebLink updated when URL changes on re-import")]
        public async Task Funding_WebLink_UpdatedWhenUrlChanges()
        {
            string dbName = nameof(Funding_WebLink_UpdatedWhenUrlChanges);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("funding_with_url.json"), LogId);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("funding_url_updated.json"), LogId);

            Assert.Equal(1, context.DimWebLinks.Count(e => e.Id > 0));
            Assert.Equal("https://funding-updated.example.com", context.DimWebLinks.Single(e => e.Id > 0).Url);
        }

        [Fact(DisplayName = "Funding: DimWebLink deleted when URL is cleared on re-import")]
        public async Task Funding_WebLink_DeletedWhenUrlCleared()
        {
            string dbName = nameof(Funding_WebLink_DeletedWhenUrlCleared);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            // First import: funding 9001 with URL
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("funding_with_url.json"), LogId);
            Assert.Equal(1, context.DimWebLinks.Count(e => e.Id > 0));

            // Second import: same put-code 9001, url=null (funding.json)
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("funding.json"), LogId);

            Assert.Equal(0, context.DimWebLinks.Count(e => e.Id > 0));
            Assert.Equal(1, context.DimProfileOnlyFundingDecisions.Count(e => e.Id > 0)); // funding still present
        }

        // --- Research activity (distinction) ---

        [Fact(DisplayName = "Research activity: DimWebLink created when URL is present on first import")]
        public async Task ResearchActivity_WebLink_CreatedWithUrl()
        {
            string dbName = nameof(ResearchActivity_WebLink_CreatedWithUrl);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("distinction_with_url.json"), LogId);

            Assert.Equal(1, context.DimWebLinks.Count(e => e.Id > 0));
            Assert.Equal("https://distinction.example.com", context.DimWebLinks.Single(e => e.Id > 0).Url);
            Assert.Equal(1, context.DimProfileOnlyResearchActivities.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Research activity: DimWebLink updated when URL changes on re-import")]
        public async Task ResearchActivity_WebLink_UpdatedWhenUrlChanges()
        {
            string dbName = nameof(ResearchActivity_WebLink_UpdatedWhenUrlChanges);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("distinction_with_url.json"), LogId);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("distinction_url_updated.json"), LogId);

            Assert.Equal(1, context.DimWebLinks.Count(e => e.Id > 0));
            Assert.Equal("https://distinction-updated.example.com", context.DimWebLinks.Single(e => e.Id > 0).Url);
        }

        [Fact(DisplayName = "Research activity: DimWebLink deleted when URL is cleared on re-import")]
        public async Task ResearchActivity_WebLink_DeletedWhenUrlCleared()
        {
            string dbName = nameof(ResearchActivity_WebLink_DeletedWhenUrlCleared);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            // First import: distinction 10001 with URL
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("distinction_with_url.json"), LogId);
            Assert.Equal(1, context.DimWebLinks.Count(e => e.Id > 0));

            // Second import: same put-code 10001, url=null (distinction.json)
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("distinction.json"), LogId);

            Assert.Equal(0, context.DimWebLinks.Count(e => e.Id > 0));
            Assert.Equal(1, context.DimProfileOnlyResearchActivities.Count(e => e.Id > 0)); // activity still present
        }

        // =========================================================================
        // PRIORITY 2: Organization linking logic
        // Each entity type (employment/funding/research-activity) has four tests:
        //   1. Org found → FK set directly, no DimIdentifierlessData created
        //   2. Org not found → DimIdentifierlessData used for org name
        //   3. Transition IdentifierlessData → Org (org seeded between imports)
        //   4. Transition Org → IdentifierlessData (unknown RINGGOLD on re-import)
        // =========================================================================

        // --- Employment ---

        [Fact(DisplayName = "Employment: DimOrganizationId set when RINGGOLD match found in DB")]
        public async Task Employment_LinkedToOrg_WhenDisambiguatedOrgFound()
        {
            string dbName = nameof(Employment_LinkedToOrg_WhenDisambiguatedOrgFound);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            await SeedOrgWithRinggoldId(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("employment_with_ringgold.json"), LogId);

            var affiliation = context.DimAffiliations.Single(e => e.Id > 0);
            Assert.Equal(TestOrgId, affiliation.DimOrganizationId);
            Assert.Equal(0, context.DimIdentifierlessData.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Employment: DimIdentifierlessData used for org name when no RINGGOLD match")]
        public async Task Employment_UsesIdentifierlessData_WhenOrgNotFound()
        {
            string dbName = nameof(Employment_UsesIdentifierlessData_WhenOrgNotFound);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            // No org seeded → disambiguation lookup returns null
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("employment_with_ringgold.json"), LogId);

            var affiliation = context.DimAffiliations.Single(e => e.Id > 0);
            Assert.Equal(-1, affiliation.DimOrganizationId);
            Assert.Equal(1, context.DimIdentifierlessData.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Employment: transitions from DimIdentifierlessData to DimOrganization when org found on re-import")]
        public async Task Employment_Transition_IdentifierlessToOrg()
        {
            string dbName = nameof(Employment_Transition_IdentifierlessToOrg);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            // 1st import: org not yet in DB → identifierless data created
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("employment_with_ringgold.json"), LogId);
            Assert.Equal(-1, context.DimAffiliations.Single(e => e.Id > 0).DimOrganizationId);
            Assert.Equal(1, context.DimIdentifierlessData.Count(e => e.Id > 0));

            // Seed org between imports
            await SeedOrgWithRinggoldId(context);

            // 2nd import: org now found → FK updated, identifierless data removed
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("employment_with_ringgold.json"), LogId);
            Assert.Equal(TestOrgId, context.DimAffiliations.Single(e => e.Id > 0).DimOrganizationId);
            Assert.Equal(0, context.DimIdentifierlessData.Count(e => e.Id > 0));
            Assert.Equal(1, context.DimAffiliations.Count(e => e.Id > 0)); // no duplicate affiliation
        }

        [Fact(DisplayName = "Employment: transitions from DimOrganization to DimIdentifierlessData when org match lost on re-import")]
        public async Task Employment_Transition_OrgToIdentifierless()
        {
            string dbName = nameof(Employment_Transition_OrgToIdentifierless);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            await SeedOrgWithRinggoldId(context);
            var service = CreateService(context);

            // 1st import: org found → FK set
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("employment_with_ringgold.json"), LogId);
            Assert.Equal(TestOrgId, context.DimAffiliations.Single(e => e.Id > 0).DimOrganizationId);

            // 2nd import: RINGGOLD "99999" has no match → identifierless data created
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("employment_with_unknown_ringgold.json"), LogId);
            Assert.Equal(-1, context.DimAffiliations.Single(e => e.Id > 0).DimOrganizationId);
            Assert.Equal(1, context.DimIdentifierlessData.Count(e => e.Id > 0));
            Assert.Equal(1, context.DimAffiliations.Count(e => e.Id > 0)); // no duplicate affiliation
        }

        // --- Funding ---

        [Fact(DisplayName = "Funding: DimOrganizationIdFunder set when RINGGOLD match found in DB")]
        public async Task Funding_LinkedToOrg_WhenDisambiguatedOrgFound()
        {
            string dbName = nameof(Funding_LinkedToOrg_WhenDisambiguatedOrgFound);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            await SeedOrgWithRinggoldId(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("funding_with_ringgold.json"), LogId);

            var funding = context.DimProfileOnlyFundingDecisions.Single(e => e.Id > 0);
            Assert.Equal(TestOrgId, funding.DimOrganizationIdFunder);
            Assert.Equal(0, context.DimIdentifierlessData.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Funding: DimIdentifierlessData used for funder name when no RINGGOLD match")]
        public async Task Funding_UsesIdentifierlessData_WhenOrgNotFound()
        {
            string dbName = nameof(Funding_UsesIdentifierlessData_WhenOrgNotFound);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("funding_with_ringgold.json"), LogId);

            var funding = context.DimProfileOnlyFundingDecisions.Single(e => e.Id > 0);
            Assert.Null(funding.DimOrganizationIdFunder);
            Assert.Equal(1, context.DimIdentifierlessData.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Funding: transitions from DimIdentifierlessData to DimOrganization when org found on re-import")]
        public async Task Funding_Transition_IdentifierlessToOrg()
        {
            string dbName = nameof(Funding_Transition_IdentifierlessToOrg);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            // 1st import: no org → identifierless data
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("funding_with_ringgold.json"), LogId);
            Assert.Null(context.DimProfileOnlyFundingDecisions.Single(e => e.Id > 0).DimOrganizationIdFunder);
            Assert.Equal(1, context.DimIdentifierlessData.Count(e => e.Id > 0));

            await SeedOrgWithRinggoldId(context);

            // 2nd import: org found → FK updated, identifierless data removed
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("funding_with_ringgold.json"), LogId);
            Assert.Equal(TestOrgId, context.DimProfileOnlyFundingDecisions.Single(e => e.Id > 0).DimOrganizationIdFunder);
            Assert.Equal(0, context.DimIdentifierlessData.Count(e => e.Id > 0));
            Assert.Equal(1, context.DimProfileOnlyFundingDecisions.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Funding: transitions from DimOrganization to DimIdentifierlessData when org match lost on re-import")]
        public async Task Funding_Transition_OrgToIdentifierless()
        {
            string dbName = nameof(Funding_Transition_OrgToIdentifierless);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            await SeedOrgWithRinggoldId(context);
            var service = CreateService(context);

            // 1st import: org found
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("funding_with_ringgold.json"), LogId);
            Assert.Equal(TestOrgId, context.DimProfileOnlyFundingDecisions.Single(e => e.Id > 0).DimOrganizationIdFunder);

            // 2nd import: unknown RINGGOLD → identifierless data created
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("funding_with_unknown_ringgold.json"), LogId);
            // Service sets DimOrganizationIdFunder = -1 in the OrgToIdentifierless transition
            Assert.NotEqual(TestOrgId, context.DimProfileOnlyFundingDecisions.Single(e => e.Id > 0).DimOrganizationIdFunder);
            Assert.Equal(1, context.DimIdentifierlessData.Count(e => e.Id > 0));
            Assert.Equal(1, context.DimProfileOnlyFundingDecisions.Count(e => e.Id > 0));
        }

        // --- Research activity (distinction) ---

        [Fact(DisplayName = "Research activity: DimOrganizationId set when RINGGOLD match found in DB")]
        public async Task ResearchActivity_LinkedToOrg_WhenDisambiguatedOrgFound()
        {
            string dbName = nameof(ResearchActivity_LinkedToOrg_WhenDisambiguatedOrgFound);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            await SeedOrgWithRinggoldId(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("distinction_with_ringgold.json"), LogId);

            var activity = context.DimProfileOnlyResearchActivities.Single(e => e.Id > 0);
            Assert.Equal(TestOrgId, activity.DimOrganizationId);
            Assert.Equal(0, context.DimIdentifierlessData.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Research activity: DimIdentifierlessData used for org name when no RINGGOLD match")]
        public async Task ResearchActivity_UsesIdentifierlessData_WhenOrgNotFound()
        {
            string dbName = nameof(ResearchActivity_UsesIdentifierlessData_WhenOrgNotFound);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("distinction_with_ringgold.json"), LogId);

            var activity = context.DimProfileOnlyResearchActivities.Single(e => e.Id > 0);
            Assert.Equal(-1, activity.DimOrganizationId);
            Assert.Equal(1, context.DimIdentifierlessData.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Research activity: transitions from DimIdentifierlessData to DimOrganization when org found on re-import")]
        public async Task ResearchActivity_Transition_IdentifierlessToOrg()
        {
            string dbName = nameof(ResearchActivity_Transition_IdentifierlessToOrg);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            // 1st import: no org → identifierless data
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("distinction_with_ringgold.json"), LogId);
            Assert.Equal(-1, context.DimProfileOnlyResearchActivities.Single(e => e.Id > 0).DimOrganizationId);
            Assert.Equal(1, context.DimIdentifierlessData.Count(e => e.Id > 0));

            await SeedOrgWithRinggoldId(context);

            // 2nd import: org found → FK updated, identifierless data removed
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("distinction_with_ringgold.json"), LogId);
            Assert.Equal(TestOrgId, context.DimProfileOnlyResearchActivities.Single(e => e.Id > 0).DimOrganizationId);
            Assert.Equal(0, context.DimIdentifierlessData.Count(e => e.Id > 0));
            Assert.Equal(1, context.DimProfileOnlyResearchActivities.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Research activity: transitions from DimOrganization to DimIdentifierlessData when org match lost on re-import")]
        public async Task ResearchActivity_Transition_OrgToIdentifierless()
        {
            string dbName = nameof(ResearchActivity_Transition_OrgToIdentifierless);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            await SeedOrgWithRinggoldId(context);
            var service = CreateService(context);

            // 1st import: org found
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("distinction_with_ringgold.json"), LogId);
            Assert.Equal(TestOrgId, context.DimProfileOnlyResearchActivities.Single(e => e.Id > 0).DimOrganizationId);

            // 2nd import: unknown RINGGOLD → identifierless data created
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("distinction_with_unknown_ringgold.json"), LogId);
            Assert.Equal(-1, context.DimProfileOnlyResearchActivities.Single(e => e.Id > 0).DimOrganizationId);
            Assert.Equal(1, context.DimIdentifierlessData.Count(e => e.Id > 0));
            Assert.Equal(1, context.DimProfileOnlyResearchActivities.Count(e => e.Id > 0));
        }
    }
}
