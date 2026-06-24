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

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterSecond = context.DimProfileOnlyResearchActivities.Count(e => e.Id > 0);

            Assert.Equal(1, countAfterFirst);
            Assert.Equal(countAfterFirst, countAfterSecond);
        }
    }
}
