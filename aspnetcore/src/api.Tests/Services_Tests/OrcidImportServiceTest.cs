using Xunit;
using api.Services;
using api.Controllers;
using api.Models.Common;
using api.Models.Ttv;
using api.Models.Log;
using api.Models.Orcid;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Nodes;
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

        private sealed class DateQueryInterceptor : DbCommandInterceptor
        {
            public int Count { get; set; }

            public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
                DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
                CancellationToken cancellationToken = default)
            {
                if (command.CommandText.Contains("FROM \"dim_date\"", StringComparison.OrdinalIgnoreCase))
                    Count++;
                return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
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

        private TtvContext CreateContext(string dbName, DbCommandInterceptor interceptor = null)
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
            var options = new DbContextOptionsBuilder<TtvContext>().UseSqlite(conn);
            if (interceptor != null)
                options.AddInterceptors(interceptor);
            var context = new TtvContext(options.Options);
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
                ProfileUsername = "",
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

        private OrcidImportService CreateService(
            TtvContext context,
            IOrcidJsonParserService parser = null,
            ILogger<OrcidImportService> logger = null,
            IOrcidApiService orcidApiService = null,
            IOrganizationHandlerService organizationHandler = null)
        {
            var utilityService = new UtilityService();
            var userProfileService = new UserProfileService(
                utilityService: utilityService,
                logger: new NullLogger<UserProfileService>());
            var organizationHandlerService = organizationHandler ?? new OrganizationHandlerService(
                ttvContext: context,
                utilityService: utilityService);
            return new OrcidImportService(
                ttvContext: context,
                userProfileService: userProfileService,
                orcidApiService: orcidApiService,
                orcidJsonParserService: parser ?? new OrcidJsonParserService(),
                organizationHandlerService: organizationHandlerService,
                utilityService: utilityService,
                dataSourceHelperService: new DataSourceHelperService { DimRegisteredDataSourceId_ORCID = OrcidDataSourceId },
                logger: logger ?? new NullLogger<OrcidImportService>());
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task AddDimDates_LogsCompleteExceptionChain_AndContinues(bool hasInnerExceptions)
        {
            using var context = CreateContext(nameof(AddDimDates_LogsCompleteExceptionChain_AndContinues));
            Exception innerException = null;
            if (hasInnerExceptions)
            {
                Action throwRootException = () => throw new System.ComponentModel.Win32Exception(258);
                var rootException = Record.Exception(throwRootException);
                Action throwInnerException = () => throw new TimeoutException("Database operation timed out", rootException);
                innerException = Record.Exception(throwInnerException);
            }
            var exception = new InvalidOperationException("Education parsing failed", innerException);
            var parser = new Mock<IOrcidJsonParserService>(MockBehavior.Strict);
            parser.Setup(p => p.GetEducations("{}")).Throws(exception);
            parser.Setup(p => p.GetEmployments("{}")).Returns(new List<OrcidEmployment>());
            parser.Setup(p => p.GetFundings("{}")).Returns(new List<OrcidFunding>());
            parser.Setup(p => p.GetProfileOnlyResearchActivityItems("{}")).Returns(new List<OrcidResearchActivity>());
            parser.Setup(p => p.GetWorks("{}", true)).Returns(new OrcidWorks());
            var logger = new Mock<ILogger<OrcidImportService>>();
            var service = CreateService(context, parser.Object, logger.Object);

            await service.AddDimDates("{}", DateTime.UtcNow, LogId);

            var logCall = Assert.Single(logger.Invocations, i => i.Method.Name == nameof(ILogger.Log));
            Assert.Equal(LogLevel.Error, Assert.IsType<LogLevel>(logCall.Arguments[0]));
            var state = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<string, object>>>(logCall.Arguments[2]);
            var logInfo = Assert.Single(state.Select(p => p.Value).OfType<LogApiInfo>());
            Assert.Equal(exception.ToString(), logInfo.Message);
            Assert.Contains(typeof(InvalidOperationException).FullName, logInfo.Message);
            Assert.False(string.IsNullOrEmpty(exception.StackTrace));
            Assert.Contains(exception.StackTrace, logInfo.Message);
            if (hasInnerExceptions)
            {
                Assert.Contains(typeof(TimeoutException).FullName, logInfo.Message);
                Assert.Contains(typeof(System.ComponentModel.Win32Exception).FullName, logInfo.Message);
                Assert.Contains(innerException.StackTrace, logInfo.Message);
                Assert.Contains(innerException.InnerException.StackTrace, logInfo.Message);
            }
            Assert.Equal(LogContent.Action.ORCID_RECORD_PARSE, logInfo.Action);
            Assert.Equal(LogContent.ActionState.IN_PROGRESS, logInfo.State);
            Assert.True(logInfo.Error);
            Assert.Null(logCall.Arguments[3]);
            Assert.True(context.ChangeTracker.AutoDetectChangesEnabled);
            Assert.Empty(await context.DimDates.ToListAsync());
            parser.VerifyAll();
        }

        private string LoadFixture(string fileName)
        {
            string dir = Path.Combine(
                Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName,
                "Infrastructure", "orcid_fixtures");
            return File.ReadAllText(Path.Combine(dir, fileName));
        }

        private static JsonObject EmploymentSummary(JsonObject json) =>
            json["activities-summary"]["employments"]["affiliation-group"][0]["summaries"][0]["employment-summary"].AsObject();

        private static JsonObject PublicationGroup(JsonObject json) =>
            json["activities-summary"]["works"]["group"].AsArray()
                .First(group => group["work-summary"]?[0]?["type"]?.GetValue<string>() == "journal-article").AsObject();

        private static JsonObject PublicationSummary(JsonObject json) =>
            PublicationGroup(json)["work-summary"][0].AsObject();

        private string UpdatedPublicationWorksFixture()
        {
            JsonObject json = JsonNode.Parse(LoadFixture("works_updated.json")).AsObject();
            PublicationSummary(json)["last-modified-date"]["value"] = JsonValue.Create(1000000001000L);
            return json.ToJsonString();
        }

        private string UpdatedEmploymentFixture(string fileName)
        {
            JsonObject json = JsonNode.Parse(LoadFixture(fileName)).AsObject();
            EmploymentSummary(json)["last-modified-date"]["value"] = JsonValue.Create(1000000001000L);
            return json.ToJsonString();
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
        // Section: truncation of values exceeding nvarchar(255) columns
        // -------------------------------------------------------------------------

        private static string Long(char c) => new string(c, 300);
        private static string Max(char c) => new string(c, 255);

        [Fact]
        public async Task ImportStatistics_CountItemsAndDatabaseChanges()
        {
            using var context = CreateContext(nameof(ImportStatistics_CountItemsAndDatabaseChanges));
            await SeedRequiredData(context);
            var service = CreateService(context);
            string json = LoadFixture("other_names.json").Replace(
                "\"name\": {",
                "\"name\": { \"last-modified-date\": { \"value\": 1000000001000 },");

            var inserted = await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            Assert.Equal(3, inserted.Items);
            Assert.Equal(0, inserted.Unchanged);
            Assert.Equal(8, inserted.Inserts);
            Assert.Equal(0, inserted.Updates);
            Assert.Equal(0, inserted.Deletes);

            var unchanged = await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            Assert.Equal(3, unchanged.Items);
            Assert.Equal(3, unchanged.Unchanged);
            Assert.Equal(0, unchanged.Inserts + unchanged.Updates + unchanged.Deletes);

            var updatedJson = JsonNode.Parse(json).AsObject();
            var otherNames = updatedJson["person"]["other-names"]["other-name"].AsArray();
            otherNames[0]["last-modified-date"]["value"] = 1000000002000L;
            otherNames[0]["content"] = "Updated name";
            otherNames.RemoveAt(1);
            var updated = await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, updatedJson.ToJsonString(), LogId);
            Assert.Equal(2, updated.Items);
            Assert.Equal(1, updated.Unchanged);
            Assert.Equal(0, updated.Inserts);
            Assert.Equal(2, updated.Updates);
            Assert.Equal(3, updated.Deletes);

            var oldCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi-FI");
                Assert.Equal("seconds=1 items=2 unchanged=1 failed=0 db_insert=0 db_update=2 db_delete=3",
                    updated.ToMessage(TimeSpan.FromMilliseconds(1250)));
                Assert.StartsWith("seconds=2 ", updated.ToMessage(TimeSpan.FromMilliseconds(1750)));
            }
            finally
            {
                CultureInfo.CurrentCulture = oldCulture;
            }
        }

        [Fact]
        public async Task ImportStatistics_CountsUnchangedEmailAndDateInserts()
        {
            using var context = CreateContext(nameof(ImportStatistics_CountsUnchangedEmailAndDateInserts));
            await SeedRequiredData(context);
            var service = CreateService(context);
            string emailJson = LoadFixture("email.json").Replace(
                "\"name\": {",
                "\"name\": { \"last-modified-date\": { \"value\": 1000000001000 },");
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, emailJson, LogId);

            var unchanged = await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, emailJson, LogId);
            Assert.Equal(2, unchanged.Items);
            Assert.Equal(2, unchanged.Unchanged);
            Assert.Equal(0, unchanged.Inserts + unchanged.Updates + unchanged.Deletes);

            int datesBefore = await context.DimDates.CountAsync();
            var education = await service.ImportOrcidRecordJsonIntoUserProfile(
                UserProfileId, LoadFixture("education.json"), LogId);
            Assert.Equal(2, await context.DimDates.CountAsync() - datesBefore);
            Assert.Equal(5, education.Inserts);
            Assert.Equal(3, education.Deletes);
        }

        [Fact]
        public async Task AdditionalImportStatistics_CountUpdatesUnchangedAndFailedItems()
        {
            using var context = CreateContext(nameof(AdditionalImportStatistics_CountUpdatesUnchangedAndFailedItems));
            await SeedRequiredData(context);
            await CreateService(context).ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("funding.json"), LogId);
            var ffv = await context.FactFieldValues
                .Include(value => value.DimProfileOnlyFundingDecision)
                .SingleAsync(value => value.DimUserProfileId == UserProfileId && value.DimProfileOnlyFundingDecisionId > 0);

            var api = new Mock<IOrcidApiService>();
            api.Setup(service => service.GetDataFromMemberApi(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("detail");
            var parser = new Mock<IOrcidJsonParserService>();
            parser.Setup(service => service.GetFundingDetail("detail")).Returns(new OrcidFunding(
                type: "grant", name: "Detailed funding", description: "Description", amount: "123",
                currencyCode: "EUR", disambiguatedOrganizationIdentifier: null, disambiguationSource: null,
                organizationName: null, startDate: null, endDate: null, putCode: null, url: null, path: null));
            var service = CreateService(context, parser.Object, orcidApiService: api.Object);

            var updated = await service.ImportAdditionalData(new() { ffv }, "token", LogId);
            updated.CaptureChanges(context);
            await context.SaveChangesAsync();
            Assert.Equal(1, updated.Items);
            Assert.Equal(0, updated.Unchanged);
            Assert.Equal(1, updated.Updates);
            Assert.Equal(0, updated.Inserts + updated.Deletes);

            var unchanged = await service.ImportAdditionalData(new() { ffv }, "token", LogId);
            unchanged.CaptureChanges(context);
            await context.SaveChangesAsync();
            Assert.Equal(1, unchanged.Items);
            Assert.Equal(1, unchanged.Unchanged);
            Assert.Equal(0, unchanged.Inserts + unchanged.Updates + unchanged.Deletes);

            parser.Setup(service => service.GetFundingDetail("detail")).Throws(new FormatException("Invalid detail"));
            var failed = await service.ImportAdditionalData(new() { ffv }, "token", LogId);
            failed.CaptureChanges(context);
            Assert.Equal(1, failed.Items);
            Assert.Equal(1, failed.Failed);
            Assert.Equal(0, failed.Unchanged);
        }

        [Fact]
        public async Task OrcidController_LogsBothImportsAndSavesAdditionalDataInBackgroundScope()
        {
            string dbName = nameof(OrcidController_LogsBothImportsAndSavesAdditionalDataInBackgroundScope);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var api = new Mock<IOrcidApiService>();
            api.Setup(service => service.GetRecordFromPublicApi(It.IsAny<string>()))
                .ReturnsAsync(LoadFixture("funding.json"));
            api.Setup(service => service.GetDataFromPublicApi(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("detail");
            var parser = new Mock<IOrcidJsonParserService>();
            parser.Setup(service => service.GetFundingDetail("detail")).Returns(new OrcidFunding(
                type: "grant", name: "Background detail", description: "Description", amount: "123",
                currencyCode: "EUR", disambiguatedOrganizationIdentifier: null, disambiguationSource: null,
                organizationName: null, startDate: null, endDate: null, putCode: null, url: null, path: null));
            using var provider = new ServiceCollection()
                .AddScoped(_ => CreateContext(dbName))
                .AddScoped<IOrcidImportService>(services => CreateService(
                    services.GetRequiredService<TtvContext>(), parser.Object, orcidApiService: api.Object))
                .BuildServiceProvider();
            var userProfile = new Mock<IUserProfileService>();
            userProfile.Setup(service => service.GetUserprofileIdForOrcidId(It.IsAny<string>()))
                .ReturnsAsync((true, UserProfileId));
            var environment = new Mock<IWebHostEnvironment>();
            environment.SetupGet(env => env.EnvironmentName).Returns("Development");
            var logger = new Mock<ILogger<OrcidController>>();
            var queue = new BackgroundTaskQueue();
            using var cache = new MemoryCache(new MemoryCacheOptions());
            var controller = new OrcidController(userProfile.Object, api.Object, CreateService(context),
                logger.Object, Mock.Of<ITokenService>(), environment.Object, cache, queue,
                provider.GetRequiredService<IServiceScopeFactory>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                            new Claim("orcid", "0000-0001-0001-0001"),
                            new Claim("use_orcid_public_api", "true")
                        }))
                    }
                }
            };

            Assert.IsType<OkObjectResult>(await controller.Get());
            var workItem = await queue.DequeueAsync(CancellationToken.None);
            await workItem(CancellationToken.None);

            var completions = logger.Invocations
                .Where(invocation => invocation.Method.Name == nameof(ILogger.Log))
                .SelectMany(invocation => ((IReadOnlyList<KeyValuePair<string, object>>)invocation.Arguments[2])
                    .Select(pair => pair.Value).OfType<LogApiInfo>())
                .Where(info => info.State == LogContent.ActionState.COMPLETE).ToList();
            var main = Assert.Single(completions, info => info.Action == LogContent.Action.ORCID_RECORD_IMPORT);
            var additional = Assert.Single(completions, info => info.Action == LogContent.Action.ORCID_RECORD_IMPORT_ADDITIONAL);
            Assert.Matches(@"^seconds=\d+ items=2 unchanged=0 failed=0 db_insert=\d+ db_update=\d+ db_delete=\d+$", main.Message);
            Assert.Matches(@"^seconds=\d+ items=1 unchanged=0 failed=0 db_insert=0 db_update=1 db_delete=0$", additional.Message);
            context.ChangeTracker.Clear();
            Assert.Equal("Background detail", (await context.DimProfileOnlyFundingDecisions.SingleAsync(
                funding => funding.Id > 0)).NameEn);
        }

        [Fact(DisplayName = "Name: ORCID timestamps populate and update FactFieldValue timestamps")]
        public async Task Name_OrcidTimestamps_Persist()
        {
            using var context = CreateContext(nameof(Name_OrcidTimestamps_Persist));
            await SeedRequiredData(context);
            var service = CreateService(context);
            string json = LoadFixture("name.json").Replace(
                "\"name\": {",
                "\"name\": { \"created-date\": { \"value\": 1000000000000 }, \"last-modified-date\": { \"value\": 1000000001000 },");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            var factFieldValue = Assert.Single(context.FactFieldValues.Where(ffv => ffv.DimNameId > 0));
            var createdDate = DateTimeOffset.FromUnixTimeMilliseconds(1000000000000).UtcDateTime;
            var modifiedDate = DateTimeOffset.FromUnixTimeMilliseconds(1000000001000).UtcDateTime;
            Assert.Equal(createdDate, factFieldValue.Created);
            Assert.Equal(modifiedDate, factFieldValue.Modified);

            var dimName = context.DimNames.Single(e => e.Id == factFieldValue.DimNameId);
            DateTime dimNameModified = dimName.Modified.Value;
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            factFieldValue = Assert.Single(context.FactFieldValues.Where(ffv => ffv.DimNameId > 0));
            Assert.Equal(modifiedDate, factFieldValue.Modified);
            Assert.Equal(dimNameModified, context.DimNames.Single(e => e.Id == factFieldValue.DimNameId).Modified);

            string olderJson = json
                .Replace("1000000001000", "999999999000")
                .Replace("\"Alice\"", "\"Ignored\"");
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, olderJson, LogId);
            factFieldValue = Assert.Single(context.FactFieldValues.Where(ffv => ffv.DimNameId > 0));
            Assert.Equal(modifiedDate, factFieldValue.Modified);
            Assert.Equal("Alice", context.DimNames.Single(e => e.Id == factFieldValue.DimNameId).FirstNames);

            string newerJson = json
                .Replace("1000000001000", "1000000002000")
                .Replace("\"Alice\"", "\"Alicia\"");
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, newerJson, LogId);
            factFieldValue = Assert.Single(context.FactFieldValues.Where(ffv => ffv.DimNameId > 0));
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1000000002000).UtcDateTime, factFieldValue.Modified);
            Assert.Equal("Alicia", context.DimNames.Single(e => e.Id == factFieldValue.DimNameId).FirstNames);

            string emptyJson = newerJson
                .Replace("1000000002000", "1000000003000")
                .Replace("\"Alicia\"", "null")
                .Replace("\"Smith\"", "null");
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, emptyJson, LogId);
            factFieldValue = Assert.Single(context.FactFieldValues.Where(ffv => ffv.DimNameId > 0));
            dimName = context.DimNames.Single(e => e.Id == factFieldValue.DimNameId);
            Assert.Equal("", dimName.FirstNames);
            Assert.Equal("", dimName.LastName);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("name.json"), LogId);
            factFieldValue = Assert.Single(context.FactFieldValues.Where(ffv => ffv.DimNameId > 0));
            Assert.Equal(createdDate, factFieldValue.Created);
            Assert.True(factFieldValue.Modified > modifiedDate);
            dimName = context.DimNames.Single(e => e.Id == factFieldValue.DimNameId);
            Assert.Equal("Alice", dimName.FirstNames);
            Assert.Equal("Smith", dimName.LastName);
        }

        [Fact(DisplayName = "Name: missing ORCID timestamps use the import clock")]
        public async Task Name_MissingOrcidTimestamps_UseImportClock()
        {
            using var context = CreateContext(nameof(Name_MissingOrcidTimestamps_UseImportClock));
            await SeedRequiredData(context);
            var service = CreateService(context);
            DateTime beforeImport = DateTime.UtcNow;

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("name.json"), LogId);

            DateTime afterImport = DateTime.UtcNow;
            var factFieldValue = Assert.Single(context.FactFieldValues.Where(ffv => ffv.DimNameId > 0));
            Assert.InRange(factFieldValue.Created.Value, beforeImport, afterImport);
            Assert.Equal(factFieldValue.Created, factFieldValue.Modified);
        }

        [Fact(DisplayName = "Name: SQL datetime rounding does not cause repeated writes")]
        public async Task Name_SqlDateTimeRounding_SkipsWrites()
        {
            using var context = CreateContext(nameof(Name_SqlDateTimeRounding_SkipsWrites));
            await SeedRequiredData(context);
            var service = CreateService(context);
            const long timestamp = 1757656106614;
            DateTime incomingTimestamp = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
            JsonObject json = JsonNode.Parse(LoadFixture("name.json")).AsObject();
            JsonObject nameJson = json["person"].AsObject()["name"].AsObject();
            nameJson["created-date"] = new JsonObject { ["value"] = timestamp };
            nameJson["last-modified-date"] = new JsonObject { ["value"] = timestamp };
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var factFieldValue = context.FactFieldValues.Single(ffv => ffv.DimNameId > 0);
            DateTime storedTimestamp = new SqlDateTime(incomingTimestamp).Value;
            Assert.True(storedTimestamp < incomingTimestamp);
            factFieldValue.Modified = storedTimestamp;
            var name = context.DimNames.Single(e => e.Id == factFieldValue.DimNameId);
            DateTime originalDomainModified = new(2000, 1, 1);
            name.Modified = originalDomainModified;
            await context.SaveChangesAsync();

            nameJson["given-names"]["value"] = "Stale Alice";
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            name = context.DimNames.Single(e => e.Id == factFieldValue.DimNameId);
            Assert.Equal("Alice", name.FirstNames);
            Assert.Equal(originalDomainModified, name.Modified);
            Assert.Equal(storedTimestamp, context.FactFieldValues.Single(ffv => ffv.DimNameId > 0).Modified);
        }

        [Fact(DisplayName = "Name: over-long first/last name truncated to 255 on insert and update")]
        public async Task Name_LongValues_TruncatedTo255()
        {
            using var context = CreateContext(nameof(Name_LongValues_TruncatedTo255));
            await SeedRequiredData(context);
            var service = CreateService(context);
            string Fixture(char c) => LoadFixture("name.json")
                .Replace("\"Alice\"", $"\"{Long(c)}\"")
                .Replace("\"Smith\"", $"\"{Long(c)}\"");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, Fixture('a'), LogId);
            var name = context.DimNames.Single(e => e.Id > 0);
            Assert.Equal(Max('a'), name.FirstNames);
            Assert.Equal(Max('a'), name.LastName);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, Fixture('b'), LogId);
            name = context.DimNames.Single(e => e.Id > 0);
            Assert.Equal(Max('b'), name.FirstNames);
            Assert.Equal(Max('b'), name.LastName);
        }

        [Fact(DisplayName = "Other name: over-long full name truncated to 255 on insert and update")]
        public async Task OtherName_LongValue_TruncatedTo255()
        {
            using var context = CreateContext(nameof(OtherName_LongValue_TruncatedTo255));
            await SeedRequiredData(context);
            var service = CreateService(context);
            string Fixture(char c) => LoadFixture("other_names.json").Replace("\"Alice S.\"", $"\"{Long(c)}\"");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, Fixture('a'), LogId);
            Assert.Single(context.DimNames, n => n.Id > 0 && n.FullName == Max('a'));

            JsonObject updatedJson = JsonNode.Parse(Fixture('b')).AsObject();
            JsonArray otherNames = updatedJson["person"].AsObject()["other-names"].AsObject()["other-name"].AsArray();
            otherNames[0].AsObject()["last-modified-date"].AsObject()["value"] = JsonValue.Create(1000000001000L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, updatedJson.ToJsonString(), LogId);
            Assert.Single(context.DimNames, n => n.Id > 0 && n.FullName == Max('b'));
            Assert.DoesNotContain(context.DimNames, n => n.FullName == Max('a'));
        }

        [Fact(DisplayName = "Researcher URL: over-long label truncated to 255 on insert and update")]
        public async Task ResearcherUrl_LongLabel_TruncatedTo255()
        {
            using var context = CreateContext(nameof(ResearcherUrl_LongLabel_TruncatedTo255));
            await SeedRequiredData(context);
            var service = CreateService(context);
            string Fixture(char c) => LoadFixture("researcher_url.json").Replace("\"Homepage\"", $"\"{Long(c)}\"");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, Fixture('a'), LogId);
            Assert.Equal(Max('a'), context.DimWebLinks.Single(e => e.Id > 0).LinkLabel);

            JsonObject updatedJson = JsonNode.Parse(Fixture('b')).AsObject();
            JsonArray researcherUrls = updatedJson["person"].AsObject()["researcher-urls"].AsObject()["researcher-url"].AsArray();
            researcherUrls[0].AsObject()["last-modified-date"].AsObject()["value"] = JsonValue.Create(1000000001000L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, updatedJson.ToJsonString(), LogId);
            Assert.Equal(Max('b'), context.DimWebLinks.Single(e => e.Id > 0).LinkLabel);
        }

        [Fact(DisplayName = "Email: over-long address truncated to 255 and not duplicated on re-import")]
        public async Task Email_LongValue_TruncatedTo255_AndNotDuplicated()
        {
            using var context = CreateContext(nameof(Email_LongValue_TruncatedTo255_AndNotDuplicated));
            await SeedRequiredData(context);
            var service = CreateService(context);
            string json = LoadFixture("email.json").Replace("\"alice@example.com\"", $"\"{Long('a')}@example.com\"");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            Assert.Equal(Max('a'), context.DimEmailAddrresses.Single(e => e.Id > 0).Email);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            Assert.Equal(1, context.DimEmailAddrresses.Count(e => e.Id > 0));
        }

        [Fact(DisplayName = "Keyword: over-long keyword truncated to 255 on insert and update")]
        public async Task Keyword_LongValue_TruncatedTo255()
        {
            using var context = CreateContext(nameof(Keyword_LongValue_TruncatedTo255));
            await SeedRequiredData(context);
            var service = CreateService(context);
            string Fixture(char c) => LoadFixture("keywords.json").Replace("\"machine learning\"", $"\"{Long(c)}\"");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, Fixture('a'), LogId);
            Assert.Single(context.DimKeywords, k => k.Id > 0 && k.Keyword == Max('a'));

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, Fixture('b'), LogId);
            Assert.Single(context.DimKeywords, k => k.Id > 0 && k.Keyword == Max('b'));
            Assert.DoesNotContain(context.DimKeywords, k => k.Keyword == Max('a'));
        }

        [Fact(DisplayName = "External identifier: over-long type and value truncated to 255 on insert and update")]
        public async Task ExternalIdentifier_LongValues_TruncatedTo255()
        {
            using var context = CreateContext(nameof(ExternalIdentifier_LongValues_TruncatedTo255));
            await SeedRequiredData(context);
            var service = CreateService(context);
            string Fixture(char c) => LoadFixture("external_identifier.json")
                .Replace("\"Loop profile\"", $"\"{Long(c)}\"")
                .Replace("\"99999\"", $"\"{Long(c)}\"");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, Fixture('a'), LogId);
            var pid = context.DimPids.Single(p => p.Id > 0 && p.PidType != Constants.PidTypes.ORCID_PUT_CODE);
            Assert.Equal(Max('a'), pid.PidContent);
            Assert.Equal(Max('a'), pid.PidType);

            JsonObject updatedJson = JsonNode.Parse(Fixture('b')).AsObject();
            JsonArray externalIdentifiers = updatedJson["person"]["external-identifiers"]["external-identifier"].AsArray();
            externalIdentifiers[0]["last-modified-date"]["value"] = JsonValue.Create(1000000001000L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, updatedJson.ToJsonString(), LogId);
            pid = context.DimPids.Single(p => p.Id > 0 && p.PidType != Constants.PidTypes.ORCID_PUT_CODE);
            Assert.Equal(Max('b'), pid.PidContent);
            Assert.Equal(Max('b'), pid.PidType);
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

        [Fact(DisplayName = "Other names: unchanged items skip writes while changed items update")]
        public async Task OtherNames_MixedChangedAndUnchanged_SkipOnlyUnchangedWrites()
        {
            using var context = CreateContext(nameof(OtherNames_MixedChangedAndUnchanged_SkipOnlyUnchangedWrites));
            await SeedRequiredData(context);
            var service = CreateService(context);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("other_names.json"), LogId);

            var unchangedNameBefore = context.DimNames.Single(name => name.FullName == "Alice S.");
            var unchangedFactFieldValueBefore = context.FactFieldValues.Single(ffv => ffv.DimNameId == unchangedNameBefore.Id);
            var unchangedNameId = unchangedNameBefore.Id;
            DateTime? unchangedNameModified = unchangedNameBefore.Modified;
            DateTime? unchangedFactFieldValueModified = unchangedFactFieldValueBefore.Modified;
            var changedNameBefore = context.DimNames.Single(name => name.FullName == "A. Smith");
            var changedNameId = changedNameBefore.Id;

            JsonObject updatedJson = JsonNode.Parse(LoadFixture("other_names.json")).AsObject();
            JsonArray otherNames = updatedJson["person"].AsObject()["other-names"].AsObject()["other-name"].AsArray();
            otherNames[0].AsObject()["content"] = "Stale Alice S.";
            otherNames[1].AsObject()["content"] = "Updated A. Smith";
            otherNames[1].AsObject()["last-modified-date"].AsObject()["value"] = JsonValue.Create(1000000001000L);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, updatedJson.ToJsonString(), LogId);

            var unchangedNameAfter = context.DimNames.Single(name => name.Id == unchangedNameId);
            var unchangedFactFieldValueAfter = context.FactFieldValues.Single(ffv => ffv.DimNameId == unchangedNameId);
            Assert.Equal("Alice S.", unchangedNameAfter.FullName);
            Assert.Equal(unchangedNameModified, unchangedNameAfter.Modified);
            Assert.Equal(unchangedFactFieldValueModified, unchangedFactFieldValueAfter.Modified);

            var changedNameAfter = context.DimNames.Single(name => name.Id == changedNameId);
            var changedFactFieldValueAfter = context.FactFieldValues.Single(ffv => ffv.DimNameId == changedNameId);
            Assert.Equal("Updated A. Smith", changedNameAfter.FullName);
            Assert.Equal(
                DateTimeOffset.FromUnixTimeMilliseconds(1000000001000).UtcDateTime,
                changedFactFieldValueAfter.Modified);
            Assert.Equal(2, context.DimNames.Count(name => name.Id > 0 && name.FullName != null));
        }

        [Fact(DisplayName = "Other names: SQL datetime rounding does not cause repeated writes")]
        public async Task OtherNames_SqlDateTimeRounding_SkipsWrites()
        {
            using var context = CreateContext(nameof(OtherNames_SqlDateTimeRounding_SkipsWrites));
            await SeedRequiredData(context);
            var service = CreateService(context);
            const long timestamp = 1757656106614;
            DateTime incomingTimestamp = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
            JsonObject json = JsonNode.Parse(LoadFixture("other_names.json")).AsObject();
            JsonArray otherNamesJson = json["person"].AsObject()["other-names"].AsObject()["other-name"].AsArray();
            JsonObject firstOtherName = otherNamesJson[0].AsObject();
            firstOtherName["last-modified-date"]["value"] = JsonValue.Create(timestamp);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var name = context.DimNames.Single(e => e.FullName == "Alice S.");
            var factFieldValue = context.FactFieldValues.Single(ffv => ffv.DimNameId == name.Id);
            DateTime storedTimestamp = new SqlDateTime(incomingTimestamp).Value;
            Assert.True(storedTimestamp < incomingTimestamp);
            factFieldValue.Modified = storedTimestamp;
            DateTime originalDomainModified = new(2000, 1, 1);
            name.Modified = originalDomainModified;
            await context.SaveChangesAsync();

            firstOtherName["content"] = "Stale Alice";
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            name = context.DimNames.Single(e => e.Id == name.Id);
            Assert.Equal("Alice S.", name.FullName);
            Assert.Equal(originalDomainModified, name.Modified);
            Assert.Equal(storedTimestamp, context.FactFieldValues.Single(ffv => ffv.DimNameId == name.Id).Modified);
        }

        [Fact(DisplayName = "Other names: duplicate put codes do not qualify for timestamp skipping")]
        public async Task OtherNames_DuplicateIncomingPutCode_FullProcessesMatches()
        {
            using var context = CreateContext(nameof(OtherNames_DuplicateIncomingPutCode_FullProcessesMatches));
            await SeedRequiredData(context);
            var service = CreateService(context);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("other_names.json"), LogId);

            JsonObject duplicatePutCodeJson = JsonNode.Parse(LoadFixture("other_names.json")).AsObject();
            JsonArray otherNames = duplicatePutCodeJson["person"].AsObject()["other-names"].AsObject()["other-name"].AsArray();
            otherNames[0].AsObject()["content"] = "First collision name";
            otherNames[1].AsObject()["content"] = "Second collision name";
            otherNames[1].AsObject()["put-code"] = JsonValue.Create(1001);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, duplicatePutCodeJson.ToJsonString(), LogId);

            Assert.Equal(1, context.DimNames.Count(name => name.Id > 0 && name.FullName != null));
            Assert.Equal("Second collision name", context.DimNames.Single(name => name.Id > 0 && name.FullName != null).FullName);
        }

        [Fact(DisplayName = "Other names: multiple stored matches do not qualify for timestamp skipping")]
        public async Task OtherNames_MultipleStoredPutCodeMatches_FullProcessesMatch()
        {
            using var context = CreateContext(nameof(OtherNames_MultipleStoredPutCodeMatches_FullProcessesMatch));
            await SeedRequiredData(context);
            var service = CreateService(context);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("other_names.json"), LogId);

            var secondNameFactFieldValue = context.FactFieldValues
                .Include(ffv => ffv.DimPidIdOrcidPutCodeNavigation)
                .Single(ffv => ffv.DimName.FullName == "A. Smith");
            secondNameFactFieldValue.DimPidIdOrcidPutCodeNavigation.PidContent = "1001";
            await context.SaveChangesAsync();

            JsonObject singleOtherNameJson = JsonNode.Parse(LoadFixture("other_names.json")).AsObject();
            JsonArray otherNames = singleOtherNameJson["person"].AsObject()["other-names"].AsObject()["other-name"].AsArray();
            otherNames[0].AsObject()["content"] = "Updated after stored collision";
            otherNames.RemoveAt(1);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, singleOtherNameJson.ToJsonString(), LogId);

            Assert.Equal(1, context.DimNames.Count(name => name.Id > 0 && name.FullName != null));
            Assert.Equal(
                "Updated after stored collision",
                context.DimNames.Single(name => name.Id > 0 && name.FullName != null).FullName);
        }

        [Fact(DisplayName = "Other names: missing put code is fully processed")]
        public async Task OtherNames_MissingPutCode_FullProcessesItem()
        {
            using var context = CreateContext(nameof(OtherNames_MissingPutCode_FullProcessesItem));
            await SeedRequiredData(context);
            var service = CreateService(context);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("other_names.json"), LogId);

            JsonObject missingPutCodeJson = JsonNode.Parse(LoadFixture("other_names.json")).AsObject();
            JsonArray otherNames = missingPutCodeJson["person"].AsObject()["other-names"].AsObject()["other-name"].AsArray();
            otherNames[0].AsObject().Remove("put-code");
            otherNames[0].AsObject()["content"] = "Alice without put code";

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, missingPutCodeJson.ToJsonString(), LogId);

            Assert.Equal(2, context.DimNames.Count(name => name.Id > 0 && name.FullName != null));
            Assert.Contains(context.DimNames, name => name.FullName == "Alice without put code");
            var missingPutCodeFactFieldValue = context.FactFieldValues
                .Include(ffv => ffv.DimName)
                .Include(ffv => ffv.DimPidIdOrcidPutCodeNavigation)
                .Single(ffv => ffv.DimName.FullName == "Alice without put code");
            Assert.Equal("-1", missingPutCodeFactFieldValue.DimPidIdOrcidPutCodeNavigation.PidContent);
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
            var factFieldValue = context.FactFieldValues.Single(ffv => ffv.DimResearcherDescriptionId > 0);
            DateTime? biographyModified = biography.Modified;
            DateTime? factFieldValueModified = factFieldValue.Modified;

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterSecond = context.DimResearcherDescriptions.Count(e => e.Id > 0);

            Assert.Equal(1, countAfterFirst);
            Assert.Equal(countAfterFirst, countAfterSecond);
            Assert.Equal(biographyModified, context.DimResearcherDescriptions.Single(e => e.Id > 0).Modified);
            Assert.Equal(factFieldValueModified, context.FactFieldValues.Single(ffv => ffv.DimResearcherDescriptionId > 0).Modified);
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
            DateTime? webLinkModified = webLink.Modified;
            DateTime? factFieldValueModified = context.FactFieldValues.Single(ffv => ffv.DimWebLinkId == webLink.Id).Modified;

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterSecond = context.DimWebLinks.Count(e => e.Id > 0);

            Assert.Equal(1, countAfterFirst);
            Assert.Equal(countAfterFirst, countAfterSecond);
            Assert.Equal(webLinkModified, context.DimWebLinks.Single(e => e.Id > 0).Modified);
            Assert.Equal(factFieldValueModified, context.FactFieldValues.Single(ffv => ffv.DimWebLinkId == webLink.Id).Modified);
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

        [Theory(DisplayName = "Email: unchanged value skips timestamp writes with or without a put code")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Email_Unchanged_SkipsTimestampWrites(bool hasPutCode)
        {
            using var context = CreateContext(nameof(Email_Unchanged_SkipsTimestampWrites) + hasPutCode);
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("email.json")).AsObject();
            JsonObject email = json["person"]["emails"]["email"][0].AsObject();
            if (!hasPutCode)
                email["put-code"] = null;
            const long timestamp = 1757656106614;
            email["last-modified-date"]["value"] = JsonValue.Create(timestamp);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var ffv = context.FactFieldValues.Single(f => f.DimEmailAddrressId > 0);
            int emailId = ffv.DimEmailAddrressId;
            DateTime roundedTimestamp = new SqlDateTime(DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime).Value;
            ffv.Modified = roundedTimestamp;
            DateTime originalCreated = new(2000, 1, 1);
            ffv.Created = originalCreated;
            ffv.Show = false;
            await context.SaveChangesAsync();

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            ffv = context.FactFieldValues.Single(f => f.DimEmailAddrressId > 0);
            Assert.Equal(emailId, ffv.DimEmailAddrressId);
            Assert.Equal(originalCreated, ffv.Created);
            Assert.Equal(roundedTimestamp, ffv.Modified);
            Assert.False(ffv.Show);
            Assert.Single(context.DimEmailAddrresses.Where(e => e.Id > 0));
        }

        [Fact(DisplayName = "Email: missing stored timestamp is backfilled without duplication")]
        public async Task Email_MissingStoredTimestamp_BackfillsExistingRow()
        {
            using var context = CreateContext(nameof(Email_MissingStoredTimestamp_BackfillsExistingRow));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("email.json")).AsObject();
            json["person"]["emails"]["email"][0]["put-code"] = null;
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            var ffv = context.FactFieldValues.Single(f => f.DimEmailAddrressId > 0);
            int emailId = ffv.DimEmailAddrressId;
            ffv.Created = null;
            ffv.Modified = null;
            await context.SaveChangesAsync();

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            ffv = context.FactFieldValues.Single(f => f.DimEmailAddrressId > 0);
            var expected = DateTimeOffset.FromUnixTimeMilliseconds(1000000000000).UtcDateTime;
            Assert.Equal(emailId, ffv.DimEmailAddrressId);
            Assert.Equal(expected, ffv.Created);
            Assert.Equal(expected, ffv.Modified);
            Assert.Single(context.DimEmailAddrresses.Where(e => e.Id > 0));
        }

        [Fact(DisplayName = "Email: missing incoming timestamp follows the full update path")]
        public async Task Email_MissingIncomingTimestamp_UpdatesExistingRow()
        {
            using var context = CreateContext(nameof(Email_MissingIncomingTimestamp_UpdatesExistingRow));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("email.json")).AsObject();
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            var ffv = context.FactFieldValues.Single(f => f.DimEmailAddrressId > 0);
            int emailId = ffv.DimEmailAddrressId;
            DateTime? oldModified = ffv.Modified;
            json["person"]["emails"]["email"][0].AsObject().Remove("last-modified-date");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            ffv = context.FactFieldValues.Single(f => f.DimEmailAddrressId > 0);
            Assert.Equal(emailId, ffv.DimEmailAddrressId);
            Assert.True(ffv.Modified > oldModified);
        }

        [Fact(DisplayName = "Email: newer timestamp updates metadata and older timestamp does not overwrite it")]
        public async Task Email_NewerAndOlderTimestamps_UpdateOnlyNewer()
        {
            using var context = CreateContext(nameof(Email_NewerAndOlderTimestamps_UpdateOnlyNewer));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("email.json")).AsObject();
            JsonObject email = json["person"]["emails"]["email"][0].AsObject();
            email["put-code"] = null;
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            int emailId = context.DimEmailAddrresses.Single(e => e.Id > 0).Id;
            email["last-modified-date"]["value"] = JsonValue.Create(1000000002000L);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            var ffv = context.FactFieldValues.Single(f => f.DimEmailAddrressId > 0);
            DateTime newerTimestamp = DateTimeOffset.FromUnixTimeMilliseconds(1000000002000).UtcDateTime;
            Assert.Equal(newerTimestamp, ffv.Modified);
            Assert.Equal(emailId, ffv.DimEmailAddrressId);

            email["last-modified-date"]["value"] = JsonValue.Create(1000000001000L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            ffv = context.FactFieldValues.Single(f => f.DimEmailAddrressId > 0);
            Assert.Equal(newerTimestamp, ffv.Modified);
            Assert.Equal(emailId, ffv.DimEmailAddrressId);
        }

        [Theory(DisplayName = "Email: changed address replaces the old row regardless of put code")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Email_ChangedValue_ReplacesExistingRow(bool hasPutCode)
        {
            using var context = CreateContext(nameof(Email_ChangedValue_ReplacesExistingRow) + hasPutCode);
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("email.json")).AsObject();
            JsonObject email = json["person"]["emails"]["email"][0].AsObject();
            if (!hasPutCode)
                email["put-code"] = null;
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            int previousId = context.DimEmailAddrresses.Single(e => e.Id > 0).Id;
            email["email"] = "new@example.com";
            email["last-modified-date"]["value"] = JsonValue.Create(1000000001000L);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var updated = Assert.Single(context.DimEmailAddrresses.Where(e => e.Id > 0));
            Assert.NotEqual(previousId, updated.Id);
            Assert.Equal("new@example.com", updated.Email);
            Assert.Equal(updated.Id, context.FactFieldValues.Single(f => f.DimEmailAddrressId > 0).DimEmailAddrressId);
        }

        [Fact(DisplayName = "Email: duplicate incoming addresses do not overwrite ambiguous timestamps")]
        public async Task Email_DuplicateIncomingValues_PreservesExistingMetadata()
        {
            using var context = CreateContext(nameof(Email_DuplicateIncomingValues_PreservesExistingMetadata));
            await SeedRequiredData(context);
            var logger = new Mock<ILogger<OrcidImportService>>();
            var service = CreateService(context, logger: logger.Object);
            JsonObject json = JsonNode.Parse(LoadFixture("email.json")).AsObject();
            JsonArray emails = json["person"]["emails"]["email"].AsArray();
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            var ffv = context.FactFieldValues.Single(f => f.DimEmailAddrressId > 0);
            int emailId = ffv.DimEmailAddrressId;
            DateTime? modified = ffv.Modified;
            JsonObject duplicate = emails[0].DeepClone().AsObject();
            duplicate["last-modified-date"]["value"] = JsonValue.Create(1000000002000L);
            emails.Add(duplicate);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            ffv = context.FactFieldValues.Single(f => f.DimEmailAddrressId > 0);
            Assert.Equal(emailId, ffv.DimEmailAddrressId);
            Assert.Equal(modified, ffv.Modified);
            Assert.Single(context.DimEmailAddrresses.Where(e => e.Id > 0));
            Assert.Contains(logger.Invocations, invocation =>
                invocation.Method.Name == nameof(ILogger.Log) &&
                (LogLevel)invocation.Arguments[0] == LogLevel.Warning);
        }

        [Fact(DisplayName = "Email: ambiguous new addresses are not inserted while distinct addresses import")]
        public async Task Email_DuplicateNewValues_ImportsOnlyDistinctAddress()
        {
            using var context = CreateContext(nameof(Email_DuplicateNewValues_ImportsOnlyDistinctAddress));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("email.json")).AsObject();
            JsonArray emails = json["person"]["emails"]["email"].AsArray();
            emails.Add(emails[0].DeepClone());
            JsonObject distinctEmail = emails[0].DeepClone().AsObject();
            distinctEmail["email"] = "distinct@example.com";
            distinctEmail["put-code"] = JsonValue.Create(3002);
            emails.Add(distinctEmail);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var storedEmail = Assert.Single(context.DimEmailAddrresses.Where(e => e.Id > 0));
            Assert.Equal("distinct@example.com", storedEmail.Email);
            Assert.Single(context.FactFieldValues.Where(f => f.DimEmailAddrressId > 0));
        }

        [Fact(DisplayName = "Email: duplicate stored addresses are retained without assigning timestamps")]
        public async Task Email_DuplicateStoredValues_PreservesBothRows()
        {
            using var context = CreateContext(nameof(Email_DuplicateStoredValues_PreservesBothRows));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("email.json")).AsObject();
            JsonArray emails = json["person"]["emails"]["email"].AsArray();
            JsonObject secondEmail = emails[0].DeepClone().AsObject();
            secondEmail["email"] = "second@example.com";
            secondEmail["put-code"] = JsonValue.Create(3002);
            emails.Add(secondEmail);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var secondStoredEmail = context.DimEmailAddrresses.Single(e => e.Email == "second@example.com");
            secondStoredEmail.Email = "alice@example.com";
            await context.SaveChangesAsync();
            emails.RemoveAt(1);
            emails[0]["last-modified-date"]["value"] = JsonValue.Create(1000000002000L);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            Assert.Equal(2, context.DimEmailAddrresses.Count(e => e.Id > 0 && e.Email == "alice@example.com"));
            var originalTimestamp = DateTimeOffset.FromUnixTimeMilliseconds(1000000000000).UtcDateTime;
            Assert.All(context.FactFieldValues.Where(f => f.DimEmailAddrressId > 0),
                ffv => Assert.Equal(originalTimestamp, ffv.Modified));
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

        [Fact(DisplayName = "Keywords: FactFieldValue.Modified uses ORCID last-modified-date timestamp")]
        public async Task Keywords_FactFieldValue_Modified_UsesOrcidTimestamp()
        {
            string dbName = nameof(Keywords_FactFieldValue_Modified_UsesOrcidTimestamp);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("keywords.json"), LogId);

            var ffvs = context.FactFieldValues
                .Where(f => f.DimKeywordId > 0)
                .OrderBy(f => f.DimKeywordId)
                .ToList();

            Assert.Equal(2, ffvs.Count);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1700000000000).UtcDateTime, ffvs[0].Modified);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1710000000000).UtcDateTime, ffvs[1].Modified);
        }

        [Fact(DisplayName = "Keywords: FactFieldValue.Modified falls back to currentDateTime when ORCID timestamp is null")]
        public async Task Keywords_FactFieldValue_Modified_FallsBackToCurrentDateTime()
        {
            string dbName = nameof(Keywords_FactFieldValue_Modified_FallsBackToCurrentDateTime);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("keywords_null_timestamp.json"), LogId);

            var modified = context.FactFieldValues
                .Where(f => f.DimKeywordId > 0)
                .Select(f => f.Modified)
                .First();

            Assert.NotNull(modified);
            Assert.True((DateTime.UtcNow - modified.Value).TotalSeconds < 5);
        }

        [Fact(DisplayName = "Keywords: FactFieldValue.Created uses ORCID created-date timestamp")]
        public async Task Keywords_FactFieldValue_Created_UsesOrcidTimestamp()
        {
            string dbName = nameof(Keywords_FactFieldValue_Created_UsesOrcidTimestamp);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("keywords.json"), LogId);

            var ffvs = context.FactFieldValues
                .Where(f => f.DimKeywordId > 0)
                .OrderBy(f => f.DimKeywordId)
                .ToList();

            Assert.Equal(2, ffvs.Count);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1690000000000).UtcDateTime, ffvs[0].Created);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1691000000000).UtcDateTime, ffvs[1].Created);
        }

        [Fact(DisplayName = "Keywords: FactFieldValue.Created falls back to currentDateTime when ORCID timestamp is null")]
        public async Task Keywords_FactFieldValue_Created_FallsBackToCurrentDateTime()
        {
            string dbName = nameof(Keywords_FactFieldValue_Created_FallsBackToCurrentDateTime);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("keywords_null_timestamp.json"), LogId);

            var created = context.FactFieldValues
                .Where(f => f.DimKeywordId > 0)
                .Select(f => f.Created)
                .First();

            Assert.NotNull(created);
            Assert.True((DateTime.UtcNow - created.Value).TotalSeconds < 5);
        }

        [Fact(DisplayName = "Keywords: DimKeyword.Created and Modified are set to currentDateTime on insert, not ORCID timestamps")]
        public async Task Keywords_DimKeyword_Created_SetToCurrentDateTime_OnInsert()
        {
            string dbName = nameof(Keywords_DimKeyword_Created_SetToCurrentDateTime_OnInsert);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("keywords.json"), LogId);

            var dimKeywords = context.DimKeywords.Where(k => k.Id > 0).ToList();
            Assert.Equal(2, dimKeywords.Count);
            Assert.All(dimKeywords, k => Assert.True((DateTime.UtcNow - k.Created.Value).TotalSeconds < 5));
            Assert.All(dimKeywords, k => Assert.True((DateTime.UtcNow - k.Modified.Value).TotalSeconds < 5));
        }

        [Fact(DisplayName = "Keywords: DimKeyword.Created is not overwritten on update, DimKeyword.Modified is set to currentDateTime")]
        public async Task Keywords_DimKeyword_Created_NotOverwritten_OnUpdate()
        {
            string dbName = nameof(Keywords_DimKeyword_Created_NotOverwritten_OnUpdate);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("keywords.json"), LogId);
            var createdAfterFirst = context.DimKeywords
                .Where(k => k.Id > 0)
                .OrderBy(k => k.Id)
                .Select(k => k.Created)
                .ToList();

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("keywords_updated.json"), LogId);
            var dimKeywordsAfterUpdate = context.DimKeywords
                .Where(k => k.Id > 0)
                .OrderBy(k => k.Id)
                .ToList();

            Assert.Equal(createdAfterFirst, dimKeywordsAfterUpdate.Select(k => k.Created));
            Assert.All(dimKeywordsAfterUpdate, k => Assert.True((DateTime.UtcNow - k.Modified.Value).TotalSeconds < 5));
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

        [Fact(DisplayName = "External identifiers: mixed changes retain unchanged items and update changed types and values")]
        public async Task ExternalIdentifier_MixedChanges_OnlyUpdatesChangedItem()
        {
            using var context = CreateContext(nameof(ExternalIdentifier_MixedChanges_OnlyUpdatesChangedItem));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("external_identifier.json")).AsObject();
            JsonArray identifiers = json["person"]["external-identifiers"]["external-identifier"].AsArray();
            identifiers[0]["last-modified-date"]["value"] = JsonValue.Create(1757656106614L);
            JsonObject secondIdentifier = identifiers[0].DeepClone().AsObject();
            secondIdentifier["put-code"] = JsonValue.Create(5002);
            secondIdentifier["external-id-value"] = "second-id";
            identifiers.Add(secondIdentifier);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var firstPid = context.DimPids.Single(p => p.PidContent == "99999");
            var secondPid = context.DimPids.Single(p => p.PidContent == "second-id");
            int firstId = firstPid.Id;
            int secondId = secondPid.Id;
            DateTime originalModified = new(2000, 1, 1);
            firstPid.Modified = originalModified;
            var firstFfv = context.FactFieldValues.Single(ffv => ffv.DimPidId == firstId);
            DateTime roundedTimestamp = new SqlDateTime(DateTimeOffset.FromUnixTimeMilliseconds(1757656106614L).UtcDateTime).Value;
            firstFfv.Modified = roundedTimestamp;
            firstFfv.Show = false;
            await context.SaveChangesAsync();

            identifiers[0]["external-id-value"] = "stale-id";
            identifiers[0]["external-id-type"] = "stale-type";
            identifiers[1]["external-id-value"] = "updated-id";
            identifiers[1]["external-id-type"] = "updated-type";
            identifiers[1]["last-modified-date"]["value"] = JsonValue.Create(1757656107614L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            firstPid = context.DimPids.Single(p => p.Id == firstId);
            secondPid = context.DimPids.Single(p => p.Id == secondId);
            firstFfv = context.FactFieldValues.Single(ffv => ffv.DimPidId == firstId);
            Assert.Equal("99999", firstPid.PidContent);
            Assert.Equal("Loop profile", firstPid.PidType);
            Assert.Equal(originalModified, firstPid.Modified);
            Assert.Equal(roundedTimestamp, firstFfv.Modified);
            Assert.False(firstFfv.Show);
            Assert.Equal("updated-id", secondPid.PidContent);
            Assert.Equal("updated-type", secondPid.PidType);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1757656107614L).UtcDateTime,
                context.FactFieldValues.Single(ffv => ffv.DimPidId == secondId).Modified);

            identifiers[0]["last-modified-date"]["value"] = JsonValue.Create(1757656105614L);
            identifiers.RemoveAt(1);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            firstPid = context.DimPids.Single(p => p.Id == firstId);
            Assert.Equal("99999", firstPid.PidContent);
            Assert.Equal(originalModified, firstPid.Modified);
            Assert.False(context.DimPids.Any(p => p.Id == secondId));
        }

        [Theory(DisplayName = "External identifier: null or empty URL does not delete the identifier")]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task ExternalIdentifier_NullOrEmptyUrl_RemainsPresent(int urlShape)
        {
            using var context = CreateContext(nameof(ExternalIdentifier_NullOrEmptyUrl_RemainsPresent) + urlShape);
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("external_identifier.json")).AsObject();
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            var pid = context.DimPids.Single(p => p.Id > 0 && p.PidType == "Loop profile");
            int pidId = pid.Id;
            JsonObject identifier = json["person"]["external-identifiers"]["external-identifier"][0].AsObject();
            identifier["external-id-url"] = urlShape == 0 ? null
                : new JsonObject { ["value"] = urlShape == 1 ? null : "" };
            identifier["external-id-value"] = "changed";
            identifier["last-modified-date"]["value"] = JsonValue.Create(1000000001000L);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            pid = context.DimPids.Single(p => p.Id == pidId);
            Assert.Equal("changed", pid.PidContent);
            Assert.Single(context.FactFieldValues.Where(ffv => ffv.DimPidId == pidId));
        }

        [Theory(DisplayName = "External identifier: missing incoming or stored timestamp performs full processing")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ExternalIdentifier_MissingTimestamp_FullProcesses(bool missingIncoming)
        {
            using var context = CreateContext(nameof(ExternalIdentifier_MissingTimestamp_FullProcesses) + missingIncoming);
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("external_identifier.json")).AsObject();
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            var pid = context.DimPids.Single(p => p.Id > 0 && p.PidType == "Loop profile");
            int pidId = pid.Id;
            JsonObject identifier = json["person"]["external-identifiers"]["external-identifier"][0].AsObject();
            identifier["external-id-value"] = "updated";
            if (missingIncoming)
                identifier.Remove("last-modified-date");
            else
            {
                var ffv = context.FactFieldValues.Single(f => f.DimPidId == pidId);
                ffv.Modified = null;
                await context.SaveChangesAsync();
            }

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            Assert.Equal("updated", context.DimPids.Single(p => p.Id == pidId).PidContent);
            Assert.NotNull(context.FactFieldValues.Single(f => f.DimPidId == pidId).Modified);
        }

        [Fact(DisplayName = "External identifier: duplicate incoming put codes do not skip updates")]
        public async Task ExternalIdentifier_DuplicateIncomingPutCode_FullProcesses()
        {
            using var context = CreateContext(nameof(ExternalIdentifier_DuplicateIncomingPutCode_FullProcesses));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("external_identifier.json")).AsObject();
            JsonArray identifiers = json["person"]["external-identifiers"]["external-identifier"].AsArray();
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            JsonObject secondIdentifier = identifiers[0].DeepClone().AsObject();
            identifiers[0]["external-id-value"] = "first";
            secondIdentifier["external-id-value"] = "second";
            identifiers.Add(secondIdentifier);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var pid = Assert.Single(context.DimPids.Where(p => p.Id > 0 && p.PidType == "Loop profile"));
            Assert.Equal("second", pid.PidContent);
        }

        [Fact(DisplayName = "External identifier: duplicate stored put codes do not skip updates")]
        public async Task ExternalIdentifier_DuplicateStoredPutCode_FullProcesses()
        {
            using var context = CreateContext(nameof(ExternalIdentifier_DuplicateStoredPutCode_FullProcesses));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("external_identifier.json")).AsObject();
            JsonArray identifiers = json["person"]["external-identifiers"]["external-identifier"].AsArray();
            JsonObject secondIdentifier = identifiers[0].DeepClone().AsObject();
            secondIdentifier["put-code"] = JsonValue.Create(5002);
            secondIdentifier["external-id-value"] = "second";
            identifiers.Add(secondIdentifier);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            var secondFfv = context.FactFieldValues
                .Include(ffv => ffv.DimPid)
                .Include(ffv => ffv.DimPidIdOrcidPutCodeNavigation)
                .Single(ffv => ffv.DimPid.PidContent == "second");
            secondFfv.DimPidIdOrcidPutCodeNavigation.PidContent = "5001";
            await context.SaveChangesAsync();

            identifiers.RemoveAt(1);
            identifiers[0]["external-id-value"] = "updated";
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var pid = Assert.Single(context.DimPids.Where(p => p.Id > 0 && p.PidType == "Loop profile"));
            Assert.Equal("updated", pid.PidContent);
        }

        [Fact(DisplayName = "External identifier: sentinel put code cannot qualify for timestamp skipping")]
        public async Task ExternalIdentifier_SentinelPutCode_FullProcesses()
        {
            using var context = CreateContext(nameof(ExternalIdentifier_SentinelPutCode_FullProcesses));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("external_identifier.json")).AsObject();
            JsonObject identifier = json["person"]["external-identifiers"]["external-identifier"][0].AsObject();
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            int pidId = context.DimPids.Single(p => p.Id > 0 && p.PidType == "Loop profile").Id;
            var ffv = context.FactFieldValues
                .Include(f => f.DimPidIdOrcidPutCodeNavigation)
                .Single(f => f.DimPidId == pidId);
            ffv.DimPidIdOrcidPutCodeNavigation.PidContent = (-1).ToString();
            await context.SaveChangesAsync();

            identifier["put-code"] = JsonValue.Create(-1);
            identifier["external-id-value"] = "updated";
            Assert.Equal(-1, Assert.Single(new OrcidJsonParserService().GetExternalIdentifiers(json.ToJsonString())).PutCode.Value);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var resultingPid = Assert.Single(context.DimPids.Where(p => p.Id > 0 && p.PidType == "Loop profile"));
            Assert.Equal(pidId, resultingPid.Id);
            Assert.Equal("updated", resultingPid.PidContent);
        }

        [Fact(DisplayName = "External identifier: matching put code in another field does not cause a collision")]
        public async Task ExternalIdentifier_PutCodeInAnotherField_SkipsOnlyExternalIdentifier()
        {
            using var context = CreateContext(nameof(ExternalIdentifier_PutCodeInAnotherField_SkipsOnlyExternalIdentifier));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("external_identifier.json")).AsObject();
            JsonObject namesJson = JsonNode.Parse(LoadFixture("other_names.json")).AsObject();
            JsonArray otherNames = namesJson["person"]["other-names"]["other-name"].AsArray();
            otherNames.RemoveAt(1);
            otherNames[0]["put-code"] = JsonValue.Create(5001);
            json["person"]["other-names"] = namesJson["person"]["other-names"].DeepClone();
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            var pid = context.DimPids.Single(p => p.Id > 0 && p.PidType == "Loop profile");
            int pidId = pid.Id;
            DateTime originalModified = new(2000, 1, 1);
            pid.Modified = originalModified;
            await context.SaveChangesAsync();

            json["person"]["external-identifiers"]["external-identifier"][0]["external-id-value"] = "stale";
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            pid = context.DimPids.Single(p => p.Id == pidId);
            Assert.Equal("99999", pid.PidContent);
            Assert.Equal(originalModified, pid.Modified);
            Assert.Single(context.DimNames.Where(name => name.FullName == "Alice S."));
        }

        [Fact(DisplayName = "External identifier: missing put code cannot skip processing")]
        public async Task ExternalIdentifier_MissingPutCode_ImportsIncomingValue()
        {
            using var context = CreateContext(nameof(ExternalIdentifier_MissingPutCode_ImportsIncomingValue));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("external_identifier.json")).AsObject();
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            int previousId = context.DimPids.Single(p => p.Id > 0 && p.PidType == "Loop profile").Id;
            JsonObject identifier = json["person"]["external-identifiers"]["external-identifier"][0].AsObject();
            identifier.Remove("put-code");
            identifier["external-id-value"] = "updated";

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var pid = Assert.Single(context.DimPids.Where(p => p.Id > 0 && p.PidType == "Loop profile"));
            Assert.NotEqual(previousId, pid.Id);
            Assert.Equal("updated", pid.PidContent);
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

        [Fact(DisplayName = "Education: unchanged item skips education date lookups and preserves user state")]
        public async Task Education_Unchanged_SkipsUpdatesAndDateLookups()
        {
            var interceptor = new DateQueryInterceptor();
            using var context = CreateContext(nameof(Education_Unchanged_SkipsUpdatesAndDateLookups), interceptor);
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("education.json")).AsObject();
            JsonObject summary = json["activities-summary"]["educations"]["affiliation-group"][0]["summaries"][0]["education-summary"].AsObject();
            summary["last-modified-date"]["value"] = JsonValue.Create(1757656106614L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var education = context.DimEducations.Single(e => e.Id > 0);
            int educationId = education.Id;
            int? startDateId = education.DimStartDate;
            int? endDateId = education.DimEndDate;
            DateTime originalModified = new(2000, 1, 1);
            education.Modified = originalModified;
            var ffv = context.FactFieldValues.Single(f => f.DimEducationId == educationId);
            DateTime roundedTimestamp = new SqlDateTime(DateTimeOffset.FromUnixTimeMilliseconds(1757656106614L).UtcDateTime).Value;
            ffv.Modified = roundedTimestamp;
            ffv.Show = false;
            ffv.PrimaryValue = true;
            await context.SaveChangesAsync();

            summary["role-title"] = "stale role";
            summary["organization"]["name"] = "stale institution";
            summary["start-date"]["year"]["value"] = "1999";
            interceptor.Count = 0;
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            Assert.Equal(2, interceptor.Count); // The shared AddDimDates pass still performs two lookups.
            education = context.DimEducations.Single(e => e.Id == educationId);
            ffv = context.FactFieldValues.Single(f => f.DimEducationId == educationId);
            Assert.Equal("BSc", education.NameEn);
            Assert.Equal("Test University", education.DegreeGrantingInstitutionName);
            Assert.Equal(startDateId, education.DimStartDate);
            Assert.Equal(endDateId, education.DimEndDate);
            Assert.Equal(originalModified, education.Modified);
            Assert.Equal(roundedTimestamp, ffv.Modified);
            Assert.False(ffv.Show);
            Assert.True(ffv.PrimaryValue);

            summary["last-modified-date"]["value"] = JsonValue.Create(1757656105614L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            Assert.Equal(originalModified, context.DimEducations.Single(e => e.Id == educationId).Modified);
            Assert.Equal(roundedTimestamp, context.FactFieldValues.Single(f => f.DimEducationId == educationId).Modified);
        }

        [Fact(DisplayName = "Education: changed dates and institution update without losing user state")]
        public async Task Education_ChangedDatesAndDetails_Update()
        {
            using var context = CreateContext(nameof(Education_ChangedDatesAndDetails_Update));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("education.json")).AsObject();
            JsonObject summary = json["activities-summary"]["educations"]["affiliation-group"][0]["summaries"][0]["education-summary"].AsObject();
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            var education = context.DimEducations.Single(e => e.Id > 0);
            int educationId = education.Id;
            var ffv = context.FactFieldValues.Single(f => f.DimEducationId == educationId);
            ffv.Show = true;
            ffv.PrimaryValue = true;
            await context.SaveChangesAsync();

            summary["start-date"]["year"]["value"] = "2001";
            summary["end-date"]["year"]["value"] = "2005";
            summary["last-modified-date"]["value"] = JsonValue.Create(1000000001000L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            education = context.DimEducations.Include(e => e.DimStartDateNavigation)
                .Include(e => e.DimEndDateNavigation).Single(e => e.Id == educationId);
            Assert.Equal(2001, education.DimStartDateNavigation.Year);
            Assert.Equal(2005, education.DimEndDateNavigation.Year);
            Assert.Equal("BSc", education.NameEn);

            summary["role-title"] = "MSc";
            summary["organization"]["name"] = "Updated University";
            summary["department-name"] = "Updated Department";
            summary["last-modified-date"]["value"] = JsonValue.Create(1000000002000L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            education = context.DimEducations.Single(e => e.Id == educationId);
            ffv = context.FactFieldValues.Single(f => f.DimEducationId == educationId);
            Assert.Equal("MSc", education.NameEn);
            Assert.Equal("Updated University", education.DegreeGrantingInstitutionName);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1000000002000L).UtcDateTime, ffv.Modified);
            Assert.True(ffv.Show);
            Assert.True(ffv.PrimaryValue);
        }

        [Fact(DisplayName = "Education: deletion retains an unchanged education")]
        public async Task Education_DeletedWhileOtherUnchanged_RetainsExisting()
        {
            using var context = CreateContext(nameof(Education_DeletedWhileOtherUnchanged_RetainsExisting));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("education.json")).AsObject();
            JsonArray groups = json["activities-summary"]["educations"]["affiliation-group"].AsArray();
            JsonObject secondGroup = groups[0].DeepClone().AsObject();
            JsonObject second = secondGroup["summaries"][0]["education-summary"].AsObject();
            second["put-code"] = JsonValue.Create(6002);
            second["role-title"] = "Second";
            second["start-date"]["year"]["value"] = "2010";
            second["end-date"]["year"]["value"] = "2014";
            groups.Add(secondGroup);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            int retainedId = context.DimEducations.Single(e => e.NameEn == "BSc").Id;
            int removedId = context.DimEducations.Single(e => e.NameEn == "Second").Id;
            DateTime originalModified = new(2000, 1, 1);
            context.DimEducations.Single(e => e.Id == retainedId).Modified = originalModified;
            await context.SaveChangesAsync();

            second["role-title"] = "Second updated";
            second["last-modified-date"]["value"] = JsonValue.Create(1000000001000L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            Assert.Equal(originalModified, context.DimEducations.Single(e => e.Id == retainedId).Modified);
            Assert.Equal("Second updated", context.DimEducations.Single(e => e.Id == removedId).NameEn);

            groups.RemoveAt(1);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            Assert.Equal(retainedId, Assert.Single(context.DimEducations.Where(e => e.Id > 0)).Id);
            Assert.False(context.FactFieldValues.Any(f => f.DimEducationId == removedId));
        }

        [Theory(DisplayName = "Education: missing incoming or stored timestamp performs full processing")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Education_MissingTimestamp_FullProcesses(bool missingIncoming)
        {
            using var context = CreateContext(nameof(Education_MissingTimestamp_FullProcesses) + missingIncoming);
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("education.json")).AsObject();
            JsonObject summary = json["activities-summary"]["educations"]["affiliation-group"][0]["summaries"][0]["education-summary"].AsObject();
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            int educationId = context.DimEducations.Single(e => e.Id > 0).Id;
            summary["role-title"] = "Updated";
            if (missingIncoming)
                summary.Remove("last-modified-date");
            else
            {
                context.FactFieldValues.Single(f => f.DimEducationId == educationId).Modified = null;
                await context.SaveChangesAsync();
            }

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            Assert.Equal("Updated", context.DimEducations.Single(e => e.Id == educationId).NameEn);
            Assert.NotNull(context.FactFieldValues.Single(f => f.DimEducationId == educationId).Modified);
        }

        [Theory(DisplayName = "Education: duplicate incoming or stored put code cannot skip updates")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Education_DuplicatePutCode_FullProcesses(bool duplicateIncoming)
        {
            using var context = CreateContext(nameof(Education_DuplicatePutCode_FullProcesses) + duplicateIncoming);
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("education.json")).AsObject();
            JsonArray groups = json["activities-summary"]["educations"]["affiliation-group"].AsArray();
            JsonObject first = groups[0]["summaries"][0]["education-summary"].AsObject();
            JsonObject secondGroup = groups[0].DeepClone().AsObject();
            JsonObject second = secondGroup["summaries"][0]["education-summary"].AsObject();
            second["put-code"] = JsonValue.Create(6002);
            second["role-title"] = "Second";
            second["start-date"]["year"]["value"] = "2010";
            second["end-date"]["year"]["value"] = "2014";
            if (!duplicateIncoming)
                groups.Add(secondGroup);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            first["role-title"] = "Updated";
            if (duplicateIncoming)
            {
                second["put-code"] = JsonValue.Create(6001);
                second["role-title"] = "Final";
                groups.Add(secondGroup);
            }
            else
            {
                var secondFfv = context.FactFieldValues.Include(f => f.DimEducation)
                    .Include(f => f.DimPidIdOrcidPutCodeNavigation)
                    .Single(f => f.DimEducation.NameEn == "Second");
                secondFfv.DimPidIdOrcidPutCodeNavigation.PidContent = 6001.ToString();
                await context.SaveChangesAsync();
                groups.RemoveAt(1);
            }
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var education = Assert.Single(context.DimEducations.Where(e => e.Id > 0));
            Assert.Equal(duplicateIncoming ? "Final" : "Updated", education.NameEn);
        }

        [Fact(DisplayName = "Education: sentinel put code cannot qualify for timestamp skipping")]
        public async Task Education_SentinelPutCode_FullProcesses()
        {
            using var context = CreateContext(nameof(Education_SentinelPutCode_FullProcesses));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("education.json")).AsObject();
            JsonObject summary = json["activities-summary"]["educations"]["affiliation-group"][0]["summaries"][0]["education-summary"].AsObject();
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            int educationId = context.DimEducations.Single(e => e.Id > 0).Id;
            var ffv = context.FactFieldValues.Include(f => f.DimPidIdOrcidPutCodeNavigation)
                .Single(f => f.DimEducationId == educationId);
            ffv.DimPidIdOrcidPutCodeNavigation.PidContent = (-1).ToString();
            await context.SaveChangesAsync();

            summary["put-code"] = JsonValue.Create(-1);
            summary["role-title"] = "Updated";
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            Assert.Equal("Updated", context.DimEducations.Single(e => e.Id == educationId).NameEn);
        }

        [Fact(DisplayName = "Education: missing put code imports rather than skipping")]
        public async Task Education_MissingPutCode_ImportsIncomingValue()
        {
            using var context = CreateContext(nameof(Education_MissingPutCode_ImportsIncomingValue));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("education.json")).AsObject();
            JsonObject summary = json["activities-summary"]["educations"]["affiliation-group"][0]["summaries"][0]["education-summary"].AsObject();
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            int previousId = context.DimEducations.Single(e => e.Id > 0).Id;

            summary.Remove("put-code");
            summary["role-title"] = "Updated";
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var education = Assert.Single(context.DimEducations.Where(e => e.Id > 0));
            Assert.NotEqual(previousId, education.Id);
            Assert.Equal("Updated", education.NameEn);
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

        [Theory(DisplayName = "Employment: unchanged item skips organization and date lookups")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Employment_Unchanged_SkipsLookupsAndPreservesRelations(bool linked)
        {
            var interceptor = new DateQueryInterceptor();
            using var context = CreateContext(nameof(Employment_Unchanged_SkipsLookupsAndPreservesRelations) + linked, interceptor);
            await SeedRequiredData(context);
            if (linked)
                await SeedOrgWithRinggoldId(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("employment_ringgold_dept.json")).AsObject();
            JsonObject summary = EmploymentSummary(json);
            summary["last-modified-date"]["value"] = JsonValue.Create(1757656106614L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var affiliation = context.DimAffiliations.Single(a => a.Id > 0);
            int affiliationId = affiliation.Id;
            int? startDateId = affiliation.StartDate;
            int? endDateId = affiliation.EndDate;
            DateTime originalModified = new(2000, 1, 1);
            affiliation.Modified = originalModified;
            var ffv = context.FactFieldValues.Single(f => f.DimAffiliationId == affiliationId);
            int identifierlessId = ffv.DimIdentifierlessDataId;
            ffv.Show = true;
            ffv.PrimaryValue = true;
            DateTime roundedTimestamp = new SqlDateTime(DateTimeOffset.FromUnixTimeMilliseconds(1757656106614L).UtcDateTime).Value;
            ffv.Modified = roundedTimestamp;
            var unit = context.DimIdentifierlessData.Single(d => d.Id > 0 && d.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT);
            int unitId = unit.Id;
            unit.Modified = originalModified;
            await context.SaveChangesAsync();

            summary["role-title"] = "stale role";
            summary["department-name"] = "stale department";
            summary["organization"]["name"] = "stale organization";
            summary["start-date"]["year"]["value"] = "1999";
            var organizationHandler = new Mock<IOrganizationHandlerService>(MockBehavior.Strict);
            var unchangedService = CreateService(context, organizationHandler: organizationHandler.Object);
            interceptor.Count = 0;
            await unchangedService.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            Assert.Equal(2, interceptor.Count); // Shared AddDimDates preparation still runs.
            organizationHandler.VerifyNoOtherCalls();
            affiliation = context.DimAffiliations.Single(a => a.Id == affiliationId);
            ffv = context.FactFieldValues.Single(f => f.DimAffiliationId == affiliationId);
            unit = context.DimIdentifierlessData.Single(d => d.Id == unitId);
            Assert.Equal(linked ? TestOrgId : -1, affiliation.DimOrganizationId);
            Assert.Equal("Researcher", affiliation.PositionNameEn);
            Assert.Equal(originalModified, affiliation.Modified);
            Assert.Equal(startDateId, affiliation.StartDate);
            Assert.Equal(endDateId, affiliation.EndDate);
            Assert.Equal(identifierlessId, ffv.DimIdentifierlessDataId);
            Assert.Equal(roundedTimestamp, ffv.Modified);
            Assert.True(ffv.Show);
            Assert.True(ffv.PrimaryValue);
            Assert.Equal("R&D Department", unit.ValueEn);
            Assert.Equal(originalModified, unit.Modified);

            summary["last-modified-date"]["value"] = JsonValue.Create(1757656105614L);
            await unchangedService.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            Assert.Equal(originalModified, context.DimAffiliations.Single(a => a.Id == affiliationId).Modified);
        }

        [Theory(DisplayName = "Employment: changed dates and department are processed")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Employment_ChangedDatesAndDepartment_Update(bool linked)
        {
            using var context = CreateContext(nameof(Employment_ChangedDatesAndDepartment_Update) + linked);
            await SeedRequiredData(context);
            if (linked)
                await SeedOrgWithRinggoldId(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("employment_ringgold_dept.json")).AsObject();
            JsonObject summary = EmploymentSummary(json);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            int affiliationId = context.DimAffiliations.Single(a => a.Id > 0).Id;
            summary["start-date"]["year"]["value"] = "2010";
            summary["end-date"] = new JsonObject
            {
                ["year"] = new JsonObject { ["value"] = "2015" },
                ["month"] = new JsonObject { ["value"] = "06" },
                ["day"] = null
            };
            summary["department-name"] = "New Department";
            summary["role-title"] = "Senior Researcher";
            summary["last-modified-date"]["value"] = JsonValue.Create(1000000001000L);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var affiliation = context.DimAffiliations.Include(a => a.StartDateNavigation)
                .Include(a => a.EndDateNavigation).Single(a => a.Id == affiliationId);
            Assert.Equal(2010, affiliation.StartDateNavigation.Year);
            Assert.Equal(2015, affiliation.EndDateNavigation.Year);
            Assert.Equal("Senior Researcher", affiliation.PositionNameEn);
            Assert.Equal("New Department", context.DimIdentifierlessData.Single(d => d.Id > 0 &&
                d.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT).ValueEn);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1000000001000L).UtcDateTime,
                context.FactFieldValues.Single(f => f.DimAffiliationId == affiliationId).Modified);
        }

        [Theory(DisplayName = "Employment: changed organization transition preserves department and metadata")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Employment_OrganizationTransition_PreservesDepartmentAndTimestamps(bool linkedInitially)
        {
            using var context = CreateContext(nameof(Employment_OrganizationTransition_PreservesDepartmentAndTimestamps) + linkedInitially);
            await SeedRequiredData(context);
            if (linkedInitially)
                await SeedOrgWithRinggoldId(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("employment_ringgold_dept.json")).AsObject();
            JsonObject summary = EmploymentSummary(json);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            int affiliationId = context.DimAffiliations.Single(a => a.Id > 0).Id;
            var ffv = context.FactFieldValues.Single(f => f.DimAffiliationId == affiliationId);
            ffv.Show = true;
            ffv.PrimaryValue = true;
            await context.SaveChangesAsync();

            if (linkedInitially)
                summary["organization"]["disambiguated-organization"]["disambiguated-organization-identifier"] = "99999";
            else
                await SeedOrgWithRinggoldId(context);
            summary["department-name"] = "Updated Department";
            summary["last-modified-date"]["value"] = JsonValue.Create(1000000001000L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var affiliation = context.DimAffiliations.Single(a => a.Id == affiliationId);
            ffv = context.FactFieldValues.Include(f => f.DimIdentifierlessData)
                .ThenInclude(d => d.InverseDimIdentifierlessData).Single(f => f.DimAffiliationId == affiliationId);
            Assert.Equal(linkedInitially ? -1 : TestOrgId, affiliation.DimOrganizationId);
            Assert.True(ffv.Show);
            Assert.True(ffv.PrimaryValue);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1000000000000L).UtcDateTime, ffv.Created);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1000000001000L).UtcDateTime, ffv.Modified);
            if (linkedInitially)
            {
                Assert.Equal(Constants.IdentifierlessDataTypes.ORGANIZATION_NAME, ffv.DimIdentifierlessData.Type);
                var unit = Assert.Single(ffv.DimIdentifierlessData.InverseDimIdentifierlessData);
                Assert.Equal("Updated Department", unit.ValueEn);
            }
            else
            {
                Assert.Equal(Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT, ffv.DimIdentifierlessData.Type);
                Assert.Equal("Updated Department", ffv.DimIdentifierlessData.ValueEn);
            }
            Assert.Single(context.DimIdentifierlessData.Where(d => d.Id > 0 &&
                d.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT));

            int identifierlessId = ffv.DimIdentifierlessDataId;
            var strictHandler = new Mock<IOrganizationHandlerService>(MockBehavior.Strict);
            await CreateService(context, organizationHandler: strictHandler.Object)
                .ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            strictHandler.VerifyNoOtherCalls();
            Assert.Equal(identifierlessId, context.FactFieldValues.Single(f => f.DimAffiliationId == affiliationId).DimIdentifierlessDataId);
        }

        [Theory(DisplayName = "Employment: missing timestamp performs full processing")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Employment_MissingTimestamp_FullProcesses(bool missingIncoming)
        {
            using var context = CreateContext(nameof(Employment_MissingTimestamp_FullProcesses) + missingIncoming);
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("employment.json")).AsObject();
            JsonObject summary = EmploymentSummary(json);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            int affiliationId = context.DimAffiliations.Single(a => a.Id > 0).Id;
            summary["role-title"] = "Updated";
            if (missingIncoming)
                summary.Remove("last-modified-date");
            else
            {
                context.FactFieldValues.Single(f => f.DimAffiliationId == affiliationId).Modified = null;
                await context.SaveChangesAsync();
            }

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            Assert.Equal("Updated", context.DimAffiliations.Single(a => a.Id == affiliationId).PositionNameEn);
            Assert.NotNull(context.FactFieldValues.Single(f => f.DimAffiliationId == affiliationId).Modified);
        }

        [Theory(DisplayName = "Employment: duplicate incoming or stored put code cannot skip")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Employment_DuplicatePutCode_FullProcesses(bool duplicateIncoming)
        {
            using var context = CreateContext(nameof(Employment_DuplicatePutCode_FullProcesses) + duplicateIncoming);
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("employment_with_ringgold.json")).AsObject();
            JsonArray groups = json["activities-summary"]["employments"]["affiliation-group"].AsArray();
            JsonObject first = EmploymentSummary(json);
            JsonObject secondGroup = groups[0].DeepClone().AsObject();
            JsonObject second = secondGroup["summaries"][0]["employment-summary"].AsObject();
            second["put-code"] = JsonValue.Create(7002);
            second["role-title"] = "Second";
            second["start-date"]["year"]["value"] = "2010";
            if (!duplicateIncoming)
                groups.Add(secondGroup);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            first["role-title"] = "Updated";
            if (duplicateIncoming)
            {
                second["put-code"] = JsonValue.Create(7001);
                second["role-title"] = "Final";
                groups.Add(secondGroup);
            }
            else
            {
                var secondFfv = context.FactFieldValues.Include(f => f.DimAffiliation)
                    .Include(f => f.DimPidIdOrcidPutCodeNavigation)
                    .Single(f => f.DimAffiliation.PositionNameEn == "Second");
                secondFfv.DimPidIdOrcidPutCodeNavigation.PidContent = 7001.ToString();
                await context.SaveChangesAsync();
                groups.RemoveAt(1);
            }
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var affiliation = Assert.Single(context.DimAffiliations.Where(a => a.Id > 0));
            Assert.Equal(duplicateIncoming ? "Final" : "Updated", affiliation.PositionNameEn);
        }

        [Fact(DisplayName = "Employment: sentinel put code cannot qualify for timestamp skipping")]
        public async Task Employment_SentinelPutCode_FullProcesses()
        {
            using var context = CreateContext(nameof(Employment_SentinelPutCode_FullProcesses));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("employment.json")).AsObject();
            JsonObject summary = EmploymentSummary(json);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            int affiliationId = context.DimAffiliations.Single(a => a.Id > 0).Id;
            var ffv = context.FactFieldValues.Include(f => f.DimPidIdOrcidPutCodeNavigation)
                .Single(f => f.DimAffiliationId == affiliationId);
            ffv.DimPidIdOrcidPutCodeNavigation.PidContent = (-1).ToString();
            await context.SaveChangesAsync();
            summary["put-code"] = JsonValue.Create(-1);
            summary["role-title"] = "Updated";

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            Assert.Equal("Updated", context.DimAffiliations.Single(a => a.Id == affiliationId).PositionNameEn);
        }

        [Fact(DisplayName = "Employment: missing put code imports rather than skipping")]
        public async Task Employment_MissingPutCode_ImportsIncomingValue()
        {
            using var context = CreateContext(nameof(Employment_MissingPutCode_ImportsIncomingValue));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("employment.json")).AsObject();
            JsonObject summary = EmploymentSummary(json);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            int previousId = context.DimAffiliations.Single(a => a.Id > 0).Id;

            summary.Remove("put-code");
            summary["role-title"] = "Updated";
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var affiliation = Assert.Single(context.DimAffiliations.Where(a => a.Id > 0));
            Assert.NotEqual(previousId, affiliation.Id);
            Assert.Equal("Updated", affiliation.PositionNameEn);
        }

        [Fact(DisplayName = "Employment: changed and removed items do not delete an unchanged item")]
        public async Task Employment_MixedChangesAndDeletion_RetainsUnchanged()
        {
            using var context = CreateContext(nameof(Employment_MixedChangesAndDeletion_RetainsUnchanged));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("employment_with_ringgold.json")).AsObject();
            JsonArray groups = json["activities-summary"]["employments"]["affiliation-group"].AsArray();
            JsonObject secondGroup = groups[0].DeepClone().AsObject();
            JsonObject second = secondGroup["summaries"][0]["employment-summary"].AsObject();
            second["put-code"] = JsonValue.Create(7002);
            second["role-title"] = "Second";
            second["start-date"]["year"]["value"] = "2010";
            groups.Add(secondGroup);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            int retainedId = context.DimAffiliations.Single(a => a.PositionNameEn == "Researcher").Id;
            int removedId = context.DimAffiliations.Single(a => a.PositionNameEn == "Second").Id;
            DateTime originalModified = new(2000, 1, 1);
            context.DimAffiliations.Single(a => a.Id == retainedId).Modified = originalModified;
            await context.SaveChangesAsync();

            second["role-title"] = "Second updated";
            second["last-modified-date"]["value"] = JsonValue.Create(1000000001000L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            Assert.Equal(originalModified, context.DimAffiliations.Single(a => a.Id == retainedId).Modified);
            Assert.Equal("Second updated", context.DimAffiliations.Single(a => a.Id == removedId).PositionNameEn);
            groups.RemoveAt(1);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            Assert.Equal(retainedId, Assert.Single(context.DimAffiliations.Where(a => a.Id > 0)).Id);
            Assert.False(context.FactFieldValues.Any(f => f.DimAffiliationId == removedId));
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

        [Fact(DisplayName = "Publication: unchanged and older summaries skip publication writes")]
        public async Task Publication_UnchangedTimestamp_SkipsWrites()
        {
            using var context = CreateContext(nameof(Publication_UnchangedTimestamp_SkipsWrites));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("works.json")).AsObject();
            JsonObject group = PublicationGroup(json);
            JsonObject summary = PublicationSummary(json);
            summary["last-modified-date"]["value"] = JsonValue.Create(1757656106614L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var publication = context.DimProfileOnlyPublications.Single(p => p.Id > 0);
            int publicationId = publication.Id;
            DateTime originalModified = new(2000, 1, 1);
            publication.Modified = originalModified;
            var ffv = context.FactFieldValues.Single(f => f.DimProfileOnlyPublicationId == publicationId);
            DateTime roundedTimestamp = new SqlDateTime(DateTimeOffset.FromUnixTimeMilliseconds(1757656106614L).UtcDateTime).Value;
            ffv.Modified = roundedTimestamp;
            ffv.Show = false;
            ffv.PrimaryValue = true;
            await context.SaveChangesAsync();

            summary["title"]["title"]["value"] = "stale title";
            summary["publication-date"]["year"]["value"] = "1999";
            summary["type"] = "book";
            group["external-ids"]["external-id"][0]["external-id-value"] = "10.1000/stale";
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            publication = context.DimProfileOnlyPublications.Single(p => p.Id == publicationId);
            ffv = context.FactFieldValues.Single(f => f.DimProfileOnlyPublicationId == publicationId);
            Assert.Equal("Test Publication", publication.PublicationName);
            Assert.Equal(2020, publication.PublicationYear);
            Assert.Equal("journal-article", publication.OrcidWorkType);
            Assert.Equal("10.1000/test1", publication.DoiHandle);
            Assert.Equal(originalModified, publication.Modified);
            Assert.Equal(roundedTimestamp, ffv.Modified);
            Assert.False(ffv.Show);
            Assert.True(ffv.PrimaryValue);

            summary["last-modified-date"]["value"] = JsonValue.Create(1757656105614L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            Assert.Equal(originalModified, context.DimProfileOnlyPublications.Single(p => p.Id == publicationId).Modified);
        }

        [Fact(DisplayName = "Publication: DOI-only change with newer timestamp is applied")]
        public async Task Publication_DoiOnlyChange_Updates()
        {
            using var context = CreateContext(nameof(Publication_DoiOnlyChange_Updates));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("works.json")).AsObject();
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            JsonObject group = PublicationGroup(json);
            group["external-ids"]["external-id"][0]["external-id-value"] = "10.1000/doi-only";
            PublicationSummary(json)["last-modified-date"]["value"] = JsonValue.Create(1000000001000L);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var publication = Assert.Single(context.DimProfileOnlyPublications.Where(p => p.Id > 0));
            Assert.Equal("Test Publication", publication.PublicationName);
            Assert.Equal("10.1000/doi-only", publication.DoiHandle);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1000000001000L).UtcDateTime,
                context.FactFieldValues.Single(f => f.DimProfileOnlyPublicationId == publication.Id).Modified);
        }

        [Fact(DisplayName = "Publication: changed title, year, and type with newer timestamp are applied")]
        public async Task Publication_ChangedSummaryFields_Updates()
        {
            using var context = CreateContext(nameof(Publication_ChangedSummaryFields_Updates));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("works.json")).AsObject();
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            JsonObject summary = PublicationSummary(json);
            summary["title"]["title"]["value"] = "Updated title";
            summary["publication-date"]["year"]["value"] = "2024";
            summary["type"] = "book";
            summary["last-modified-date"]["value"] = JsonValue.Create(1000000001000L);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var publication = Assert.Single(context.DimProfileOnlyPublications.Where(p => p.Id > 0));
            Assert.Equal("Updated title", publication.PublicationName);
            Assert.Equal(2024, publication.PublicationYear);
            Assert.Equal("book", publication.OrcidWorkType);
        }

        [Fact(DisplayName = "Publication: duplicate summaries keep selected summary fields and group DOI aligned")]
        public async Task Publication_DuplicateSummaries_UsesSelectedSummary()
        {
            using var context = CreateContext(nameof(Publication_DuplicateSummaries_UsesSelectedSummary));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("works.json")).AsObject();
            JsonObject group = PublicationGroup(json);
            JsonObject selectedSummary = group["work-summary"][0].AsObject();
            JsonObject duplicateSummary = selectedSummary.DeepClone().AsObject();
            duplicateSummary["put-code"] = JsonValue.Create(8003);
            duplicateSummary["title"]["title"]["value"] = "Duplicate summary";
            duplicateSummary["last-modified-date"]["value"] = JsonValue.Create(2000000000000L);
            group["work-summary"].AsArray().Add(duplicateSummary);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var publication = Assert.Single(context.DimProfileOnlyPublications.Where(p => p.Id > 0));
            var ffv = context.FactFieldValues.Include(f => f.DimPidIdOrcidPutCodeNavigation)
                .Single(f => f.DimProfileOnlyPublicationId == publication.Id);
            Assert.Equal("Test Publication", publication.PublicationName);
            Assert.Equal("journal-article", publication.OrcidWorkType);
            Assert.Equal("10.1000/test1", publication.DoiHandle);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1000000000000L).UtcDateTime, ffv.Modified);
            Assert.Equal("8001", ffv.DimPidIdOrcidPutCodeNavigation.PidContent);
        }

        [Theory(DisplayName = "Publication: incoming duplicate or stored duplicate put codes cannot skip")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Publication_DuplicatePutCode_FullProcesses(bool duplicateIncoming)
        {
            using var context = CreateContext(nameof(Publication_DuplicatePutCode_FullProcesses) + duplicateIncoming);
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("works.json")).AsObject();
            JsonArray groups = json["activities-summary"]["works"]["group"].AsArray();
            JsonObject firstGroup = PublicationGroup(json);
            JsonObject secondGroup = firstGroup.DeepClone().AsObject();
            JsonObject secondSummary = secondGroup["work-summary"][0].AsObject();
            secondSummary["put-code"] = JsonValue.Create(8003);
            secondSummary["title"]["title"]["value"] = "Second publication";
            if (!duplicateIncoming)
                groups.Insert(groups.IndexOf(firstGroup) + 1, secondGroup);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            JsonObject firstSummary = firstGroup["work-summary"][0].AsObject();
            firstSummary["title"]["title"]["value"] = "Updated first";
            if (duplicateIncoming)
            {
                secondSummary["put-code"] = JsonValue.Create(8001);
                secondSummary["title"]["title"]["value"] = "Final title";
                groups.Insert(groups.IndexOf(firstGroup) + 1, secondGroup);
            }
            else
            {
                var secondFfv = context.FactFieldValues.Include(f => f.DimProfileOnlyPublication)
                    .Include(f => f.DimPidIdOrcidPutCodeNavigation)
                    .Single(f => f.DimProfileOnlyPublication.PublicationName == "Second publication");
                secondFfv.DimPidIdOrcidPutCodeNavigation.PidContent = "8001";
                await context.SaveChangesAsync();
                groups.Remove(secondGroup);
            }
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            Assert.Equal(duplicateIncoming ? "Final title" : "Updated first",
                context.DimProfileOnlyPublications.Single(p => p.Id > 0).PublicationName);
        }

        [Fact(DisplayName = "Publication: skipped item is retained and absent item is deleted")]
        public async Task Publication_UnchangedAndDeletedItems_AreHandled()
        {
            using var context = CreateContext(nameof(Publication_UnchangedAndDeletedItems_AreHandled));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("works.json")).AsObject();
            JsonArray groups = json["activities-summary"]["works"]["group"].AsArray();
            JsonObject firstGroup = PublicationGroup(json);
            JsonObject removedGroup = firstGroup.DeepClone().AsObject();
            removedGroup["work-summary"][0]["put-code"] = JsonValue.Create(8003);
            removedGroup["work-summary"][0]["title"]["title"]["value"] = "Removed publication";
            groups.Insert(groups.IndexOf(firstGroup) + 1, removedGroup);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            int retainedId = context.DimProfileOnlyPublications.Single(p => p.PublicationName == "Test Publication").Id;
            int removedId = context.DimProfileOnlyPublications.Single(p => p.PublicationName == "Removed publication").Id;

            groups.Remove(removedGroup);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            Assert.Equal(retainedId, Assert.Single(context.DimProfileOnlyPublications.Where(p => p.Id > 0)).Id);
            Assert.False(context.FactFieldValues.Any(f => f.DimProfileOnlyPublicationId == removedId));
        }

        [Fact(DisplayName = "Publication: missing incoming timestamp performs full processing")]
        public async Task Publication_MissingTimestamp_FullProcesses()
        {
            using var context = CreateContext(nameof(Publication_MissingTimestamp_FullProcesses));
            await SeedRequiredData(context);
            var service = CreateService(context);
            JsonObject json = JsonNode.Parse(LoadFixture("works.json")).AsObject();
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);
            JsonObject summary = PublicationSummary(json);
            summary["title"]["title"]["value"] = "Updated without timestamp";
            summary.Remove("last-modified-date");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            Assert.Equal("Updated without timestamp",
                context.DimProfileOnlyPublications.Single(p => p.Id > 0).PublicationName);
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
            string newerBiography = LoadFixture("biography_updated.json").Replace(
                "\"last-modified-date\": { \"value\": 1000000000000 },",
                "\"last-modified-date\": { \"value\": 1000000001000 },");
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, newerBiography, LogId);

            Assert.Equal(1, context.DimResearcherDescriptions.Count(e => e.Id > 0));
            Assert.Equal("Updated biography text.", context.DimResearcherDescriptions.Single(e => e.Id > 0).ResearchDescriptionEn);
            Assert.Equal(
                DateTimeOffset.FromUnixTimeMilliseconds(1000000001000).UtcDateTime,
                context.FactFieldValues.Single(ffv => ffv.DimResearcherDescriptionId > 0).Modified);
        }

        [Fact(DisplayName = "Biography: equal timestamp skips writes but keeps item processed")]
        public async Task Biography_EqualTimestamp_SkipsWritesAndRemainsPresent()
        {
            using var context = CreateContext(nameof(Biography_EqualTimestamp_SkipsWritesAndRemainsPresent));
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("biography.json"), LogId);
            var biography = context.DimResearcherDescriptions.Single(e => e.Id > 0);
            var factFieldValue = context.FactFieldValues.Single(ffv => ffv.DimResearcherDescriptionId > 0);
            string originalText = biography.ResearchDescriptionEn;
            DateTime? domainModified = biography.Modified;
            DateTime? factFieldValueModified = factFieldValue.Modified;

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("biography_updated.json"), LogId);

            biography = context.DimResearcherDescriptions.Single(e => e.Id > 0);
            factFieldValue = context.FactFieldValues.Single(ffv => ffv.DimResearcherDescriptionId > 0);
            Assert.Equal(originalText, biography.ResearchDescriptionEn);
            Assert.Equal(domainModified, biography.Modified);
            Assert.Equal(factFieldValueModified, factFieldValue.Modified);
            Assert.Single(context.DimResearcherDescriptions.Where(e => e.Id > 0));
        }

        [Fact(DisplayName = "Biography: missing timestamps process the incoming value")]
        public async Task Biography_MissingTimestamp_FullProcesses()
        {
            using var context = CreateContext(nameof(Biography_MissingTimestamp_FullProcesses));
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("biography.json"), LogId);
            var biography = context.DimResearcherDescriptions.Single(e => e.Id > 0);
            var factFieldValue = context.FactFieldValues.Single(ffv => ffv.DimResearcherDescriptionId > 0);
            DateTime? createdDate = factFieldValue.Created;
            DateTime? oldModifiedDate = factFieldValue.Modified;
            string missingTimestamps = LoadFixture("biography_updated.json")
                .Replace("\"created-date\": { \"value\": 1000000000000 },", "")
                .Replace("\"last-modified-date\": { \"value\": 1000000000000 },", "");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, missingTimestamps, LogId);

            biography = context.DimResearcherDescriptions.Single(e => e.Id > 0);
            factFieldValue = context.FactFieldValues.Single(ffv => ffv.DimResearcherDescriptionId > 0);
            Assert.Equal("Updated biography text.", biography.ResearchDescriptionEn);
            Assert.Equal(createdDate, factFieldValue.Created);
            Assert.True(factFieldValue.Modified > oldModifiedDate);
        }

        [Fact(DisplayName = "Biography: SQL datetime rounding does not cause repeated writes")]
        public async Task Biography_SqlDateTimeRounding_SkipsWrites()
        {
            using var context = CreateContext(nameof(Biography_SqlDateTimeRounding_SkipsWrites));
            await SeedRequiredData(context);
            var service = CreateService(context);
            const long timestamp = 1757656106614;
            DateTime incomingTimestamp = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
            JsonObject json = JsonNode.Parse(LoadFixture("biography.json")).AsObject();
            JsonObject biographyJson = json["person"].AsObject()["biography"].AsObject();
            biographyJson["last-modified-date"]["value"] = JsonValue.Create(timestamp);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            var factFieldValue = context.FactFieldValues.Single(ffv => ffv.DimResearcherDescriptionId > 0);
            DateTime storedTimestamp = new SqlDateTime(incomingTimestamp).Value;
            Assert.True(storedTimestamp < incomingTimestamp);
            factFieldValue.Modified = storedTimestamp;
            var biography = context.DimResearcherDescriptions.Single(e => e.Id == factFieldValue.DimResearcherDescriptionId);
            DateTime originalDomainModified = new(2000, 1, 1);
            biography.Modified = originalDomainModified;
            await context.SaveChangesAsync();

            biographyJson["content"] = "Stale biography";
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json.ToJsonString(), LogId);

            biography = context.DimResearcherDescriptions.Single(e => e.Id == factFieldValue.DimResearcherDescriptionId);
            Assert.Equal("Test researcher biography content.", biography.ResearchDescriptionEn);
            Assert.Equal(originalDomainModified, biography.Modified);
            Assert.Equal(
                storedTimestamp,
                context.FactFieldValues.Single(ffv => ffv.DimResearcherDescriptionId > 0).Modified);
        }

        [Fact(DisplayName = "Researcher URL: updated when changed in ORCID")]
        public async Task ResearcherUrl_Updated_WhenChangedInOrcid()
        {
            string dbName = nameof(ResearcherUrl_Updated_WhenChangedInOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("researcher_url.json"), LogId);
            JsonObject updatedJson = JsonNode.Parse(LoadFixture("researcher_url_updated.json")).AsObject();
            JsonArray researcherUrls = updatedJson["person"].AsObject()["researcher-urls"].AsObject()["researcher-url"].AsArray();
            researcherUrls[0].AsObject()["last-modified-date"].AsObject()["value"] = JsonValue.Create(1000000001000L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, updatedJson.ToJsonString(), LogId);

            Assert.Equal(1, context.DimWebLinks.Count(e => e.Id > 0));
            var webLink = context.DimWebLinks.Single(e => e.Id > 0);
            Assert.Equal("https://example.com/updated", webLink.Url);
            Assert.Equal("Updated Homepage", webLink.LinkLabel);
            Assert.Equal(
                DateTimeOffset.FromUnixTimeMilliseconds(1000000001000).UtcDateTime,
                context.FactFieldValues.Single(ffv => ffv.DimWebLinkId == webLink.Id).Modified);
        }

        [Fact(DisplayName = "Researcher URL: label-only change updates label")]
        public async Task ResearcherUrl_LabelOnlyChange_UpdatesLabel()
        {
            using var context = CreateContext(nameof(ResearcherUrl_LabelOnlyChange_UpdatesLabel));
            await SeedRequiredData(context);
            var service = CreateService(context);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("researcher_url.json"), LogId);

            var originalWebLink = context.DimWebLinks.Single(e => e.Id > 0);
            var originalId = originalWebLink.Id;
            DateTime? originalModified = originalWebLink.Modified;
            JsonObject updatedJson = JsonNode.Parse(LoadFixture("researcher_url.json")).AsObject();
            JsonArray researcherUrls = updatedJson["person"].AsObject()["researcher-urls"].AsObject()["researcher-url"].AsArray();
            researcherUrls[0].AsObject()["url-name"] = "Updated Homepage";
            researcherUrls[0].AsObject()["last-modified-date"].AsObject()["value"] = JsonValue.Create(1000000001000L);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, updatedJson.ToJsonString(), LogId);

            var updatedWebLink = context.DimWebLinks.Single(e => e.Id == originalId);
            Assert.Equal("https://example.com/profile", updatedWebLink.Url);
            Assert.Equal("Updated Homepage", updatedWebLink.LinkLabel);
            Assert.NotEqual(originalModified, updatedWebLink.Modified);
        }

        [Fact(DisplayName = "Researcher URL: equal timestamp skips writes and keeps item processed")]
        public async Task ResearcherUrl_EqualTimestamp_SkipsWritesAndRemainsPresent()
        {
            using var context = CreateContext(nameof(ResearcherUrl_EqualTimestamp_SkipsWritesAndRemainsPresent));
            await SeedRequiredData(context);
            var service = CreateService(context);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("researcher_url.json"), LogId);

            var originalWebLink = context.DimWebLinks.Single(e => e.Id > 0);
            var originalId = originalWebLink.Id;
            DateTime? originalModified = originalWebLink.Modified;
            var originalFactFieldValue = context.FactFieldValues.Single(ffv => ffv.DimWebLinkId == originalId);
            DateTime? originalFactFieldValueModified = originalFactFieldValue.Modified;
            JsonObject staleJson = JsonNode.Parse(LoadFixture("researcher_url.json")).AsObject();
            JsonArray researcherUrls = staleJson["person"].AsObject()["researcher-urls"].AsObject()["researcher-url"].AsArray();
            researcherUrls[0].AsObject()["url"]["value"] = "https://example.com/stale";
            researcherUrls[0].AsObject()["url-name"] = "Stale Homepage";

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, staleJson.ToJsonString(), LogId);

            var webLink = context.DimWebLinks.Single(e => e.Id == originalId);
            var factFieldValue = context.FactFieldValues.Single(ffv => ffv.DimWebLinkId == originalId);
            Assert.Equal("https://example.com/profile", webLink.Url);
            Assert.Equal("Homepage", webLink.LinkLabel);
            Assert.Equal(originalModified, webLink.Modified);
            Assert.Equal(originalFactFieldValueModified, factFieldValue.Modified);
            Assert.Single(context.DimWebLinks.Where(e => e.Id > 0));
        }

        [Fact(DisplayName = "Researcher URL: SQL datetime rounding does not cause repeated writes")]
        public async Task ResearcherUrl_SqlDateTimeRounding_SkipsWrites()
        {
            using var context = CreateContext(nameof(ResearcherUrl_SqlDateTimeRounding_SkipsWrites));
            await SeedRequiredData(context);
            var service = CreateService(context);
            const long timestamp = 1757656106614;
            DateTime incomingTimestamp = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
            JsonObject json = JsonNode.Parse(LoadFixture("researcher_url.json")).AsObject();
            JsonArray researcherUrls = json["person"].AsObject()["researcher-urls"].AsObject()["researcher-url"].AsArray();
            researcherUrls[0].AsObject()["last-modified-date"].AsObject()["value"] = JsonValue.Create(timestamp);
            string unchangedJson = json.ToJsonString();
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, unchangedJson, LogId);

            var factFieldValue = context.FactFieldValues.Single(ffv => ffv.DimWebLinkId > 0);
            DateTime storedTimestamp = new SqlDateTime(incomingTimestamp).Value;
            Assert.True(storedTimestamp < incomingTimestamp);
            factFieldValue.Modified = storedTimestamp;
            var webLink = context.DimWebLinks.Single(e => e.Id > 0);
            DateTime originalWebLinkModified = new(2000, 1, 1);
            webLink.Modified = originalWebLinkModified;
            await context.SaveChangesAsync();

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, unchangedJson, LogId);

            Assert.Equal(originalWebLinkModified, context.DimWebLinks.Single(e => e.Id > 0).Modified);
            Assert.Equal(storedTimestamp, context.FactFieldValues.Single(ffv => ffv.DimWebLinkId > 0).Modified);
        }

        [Fact(DisplayName = "Researcher URL: missing put code is fully processed")]
        public async Task ResearcherUrl_MissingPutCode_FullProcessesItem()
        {
            using var context = CreateContext(nameof(ResearcherUrl_MissingPutCode_FullProcessesItem));
            await SeedRequiredData(context);
            var service = CreateService(context);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("researcher_url.json"), LogId);

            JsonObject missingPutCodeJson = JsonNode.Parse(LoadFixture("researcher_url.json")).AsObject();
            JsonArray researcherUrls = missingPutCodeJson["person"].AsObject()["researcher-urls"].AsObject()["researcher-url"].AsArray();
            researcherUrls[0].AsObject().Remove("put-code");
            researcherUrls[0].AsObject()["url"]["value"] = "https://example.com/no-put-code";

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, missingPutCodeJson.ToJsonString(), LogId);

            var webLink = context.DimWebLinks.Single(e => e.Id > 0);
            Assert.Equal("https://example.com/no-put-code", webLink.Url);
            var factFieldValue = context.FactFieldValues
                .Include(ffv => ffv.DimPidIdOrcidPutCodeNavigation)
                .Single(ffv => ffv.DimWebLinkId == webLink.Id);
            Assert.Equal("-1", factFieldValue.DimPidIdOrcidPutCodeNavigation.PidContent);
        }

        [Fact(DisplayName = "Researcher URL: duplicate put codes do not qualify for timestamp skipping")]
        public async Task ResearcherUrl_DuplicateIncomingPutCode_FullProcessesMatches()
        {
            using var context = CreateContext(nameof(ResearcherUrl_DuplicateIncomingPutCode_FullProcessesMatches));
            await SeedRequiredData(context);
            var service = CreateService(context);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("researcher_url.json"), LogId);

            JsonObject duplicatePutCodeJson = JsonNode.Parse(LoadFixture("researcher_url.json")).AsObject();
            JsonArray researcherUrls = duplicatePutCodeJson["person"].AsObject()["researcher-urls"].AsObject()["researcher-url"].AsArray();
            JsonObject secondUrl = JsonNode.Parse(researcherUrls[0].ToJsonString()).AsObject();
            researcherUrls[0].AsObject()["url"]["value"] = "https://example.com/first-collision";
            researcherUrls[0].AsObject()["url-name"] = "First collision";
            secondUrl["url"]["value"] = "https://example.com/second-collision";
            secondUrl["url-name"] = "Second collision";
            researcherUrls.Add(secondUrl);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, duplicatePutCodeJson.ToJsonString(), LogId);

            var webLink = Assert.Single(context.DimWebLinks.Where(e => e.Id > 0));
            Assert.Equal("https://example.com/second-collision", webLink.Url);
            Assert.Equal("Second collision", webLink.LinkLabel);
        }

        [Fact(DisplayName = "Researcher URL: URL truncation is preserved on update")]
        public async Task ResearcherUrl_LongUrl_TruncatedTo511OnUpdate()
        {
            using var context = CreateContext(nameof(ResearcherUrl_LongUrl_TruncatedTo511OnUpdate));
            await SeedRequiredData(context);
            var service = CreateService(context);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("researcher_url.json"), LogId);

            string longUrl = $"https://example.com/{new string('a', 600)}";
            JsonObject updatedJson = JsonNode.Parse(LoadFixture("researcher_url.json")).AsObject();
            JsonArray researcherUrls = updatedJson["person"].AsObject()["researcher-urls"].AsObject()["researcher-url"].AsArray();
            researcherUrls[0].AsObject()["url"]["value"] = longUrl;
            researcherUrls[0].AsObject()["last-modified-date"].AsObject()["value"] = JsonValue.Create(1000000001000L);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, updatedJson.ToJsonString(), LogId);

            Assert.Equal(longUrl.Substring(0, 511), context.DimWebLinks.Single(e => e.Id > 0).Url);
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

            var ffvs = context.FactFieldValues
                .Where(f => f.DimKeywordId > 0)
                .OrderBy(f => f.DimKeywordId)
                .ToList();
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1720000000000).UtcDateTime, ffvs[0].Modified);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1730000000000).UtcDateTime, ffvs[1].Modified);
        }

        [Fact(DisplayName = "External identifier: updated when changed in ORCID")]
        public async Task ExternalIdentifier_Updated_WhenChangedInOrcid()
        {
            string dbName = nameof(ExternalIdentifier_Updated_WhenChangedInOrcid);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("external_identifier.json"), LogId);
            JsonObject updatedJson = JsonNode.Parse(LoadFixture("external_identifier_updated.json")).AsObject();
            updatedJson["person"]["external-identifiers"]["external-identifier"][0]["last-modified-date"]["value"] = JsonValue.Create(1000000001000L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, updatedJson.ToJsonString(), LogId);

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
            JsonObject updatedJson = JsonNode.Parse(LoadFixture("education_updated.json")).AsObject();
            updatedJson["activities-summary"]["educations"]["affiliation-group"][0]["summaries"][0]["education-summary"]["last-modified-date"]["value"] = JsonValue.Create(1000000001000L);
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, updatedJson.ToJsonString(), LogId);

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
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, UpdatedEmploymentFixture("employment_updated.json"), LogId);

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
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, UpdatedPublicationWorksFixture(), LogId);

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
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, UpdatedPublicationWorksFixture(), LogId);

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
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, UpdatedEmploymentFixture("employment_with_ringgold.json"), LogId);
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
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, UpdatedEmploymentFixture("employment_with_unknown_ringgold.json"), LogId);
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

        // =========================================================================
        // PRIORITY 3: Department name handling
        //
        // department-name is stored as a DimIdentifierlessDatum of type
        // "organization_unit". Two storage modes depending on whether the org was
        // disambiguated to a known DimOrganization or not:
        //   • Org NOT found → org_unit created as a CHILD of the org_name row
        //   • Org IS found  → org_unit created STANDALONE (no parent org_name row)
        // =========================================================================

        // --- Employment ---

        [Fact(DisplayName = "Employment dept name: stored as child of org_name DimIdentifierlessDatum when org not found")]
        public async Task Employment_DeptName_StoredAsChildOfOrgNameWhenOrgNotFound()
        {
            string dbName = nameof(Employment_DeptName_StoredAsChildOfOrgNameWhenOrgNotFound);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            // No org seeded → disambiguation returns null → org_name created, org_unit as child
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("employment.json"), LogId);

            // One org_name row, one org_unit row (child of org_name)
            Assert.Equal(2, context.DimIdentifierlessData.Count(e => e.Id > 0));
            Assert.Equal(1, context.DimIdentifierlessData.Count(e => e.Id > 0 && e.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME));
            var orgUnit = context.DimIdentifierlessData.Single(e => e.Id > 0 && e.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT);
            Assert.Equal("Research", orgUnit.ValueEn);
        }

        [Fact(DisplayName = "Employment dept name: stored as standalone org_unit (no parent) when org is found via disambiguation")]
        public async Task Employment_DeptName_StoredStandaloneWhenOrgFound()
        {
            string dbName = nameof(Employment_DeptName_StoredStandaloneWhenOrgFound);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            await SeedOrgWithRinggoldId(context);
            var service = CreateService(context);

            // employment_ringgold_dept.json: RINGGOLD="12345" + dept "R&D Department"
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("employment_ringgold_dept.json"), LogId);

            // Org linked directly, so no org_name row — only a standalone org_unit
            Assert.Equal(TestOrgId, context.DimAffiliations.Single(e => e.Id > 0).DimOrganizationId);
            Assert.Equal(1, context.DimIdentifierlessData.Count(e => e.Id > 0));
            var orgUnit = context.DimIdentifierlessData.Single(e => e.Id > 0);
            Assert.Equal(Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT, orgUnit.Type);
            Assert.Equal("R&D Department", orgUnit.ValueEn);
        }

        [Fact(DisplayName = "Employment dept name: org_unit updated in-place on re-import")]
        public async Task Employment_DeptName_UpdatedOnReImport()
        {
            string dbName = nameof(Employment_DeptName_UpdatedOnReImport);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            // First import: dept "Research"
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("employment.json"), LogId);
            Assert.Equal("Research", context.DimIdentifierlessData.Single(e => e.Id > 0 && e.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT).ValueEn);

            // Second import: same put-code, dept "Engineering"
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, UpdatedEmploymentFixture("employment_dept_updated.json"), LogId);

            // Row count unchanged; org_unit value updated
            Assert.Equal(2, context.DimIdentifierlessData.Count(e => e.Id > 0));
            Assert.Equal("Engineering", context.DimIdentifierlessData.Single(e => e.Id > 0 && e.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT).ValueEn);
        }

        // --- Research activity (distinction) ---

        [Fact(DisplayName = "Research activity dept name: stored as child of org_name when org not found")]
        public async Task ResearchActivity_DeptName_StoredAsChildOfOrgNameWhenOrgNotFound()
        {
            string dbName = nameof(ResearchActivity_DeptName_StoredAsChildOfOrgNameWhenOrgNotFound);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            // distinction.json has dept "Science" and disambiguated-organization: null
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("distinction.json"), LogId);

            Assert.Equal(2, context.DimIdentifierlessData.Count(e => e.Id > 0));
            Assert.Equal(1, context.DimIdentifierlessData.Count(e => e.Id > 0 && e.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME));
            var orgUnit = context.DimIdentifierlessData.Single(e => e.Id > 0 && e.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT);
            Assert.Equal("Science", orgUnit.ValueEn);
        }

        [Fact(DisplayName = "Research activity dept name: stored as standalone org_unit when org is found via disambiguation")]
        public async Task ResearchActivity_DeptName_StoredStandaloneWhenOrgFound()
        {
            string dbName = nameof(ResearchActivity_DeptName_StoredStandaloneWhenOrgFound);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            await SeedOrgWithRinggoldId(context);
            var service = CreateService(context);

            // distinction_ringgold_dept.json: RINGGOLD="12345" + dept "Science Award Dept"
            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("distinction_ringgold_dept.json"), LogId);

            Assert.Equal(TestOrgId, context.DimProfileOnlyResearchActivities.Single(e => e.Id > 0).DimOrganizationId);
            Assert.Equal(1, context.DimIdentifierlessData.Count(e => e.Id > 0));
            var orgUnit = context.DimIdentifierlessData.Single(e => e.Id > 0);
            Assert.Equal(Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT, orgUnit.Type);
            Assert.Equal("Science Award Dept", orgUnit.ValueEn);
        }

        // =========================================================================
        // PRIORITY 4: Edge cases / input bounds
        // =========================================================================

        [Fact(DisplayName = "Employment role title: truncated to 255 chars when longer")]
        public async Task Employment_RoleTitle_TruncatedAt255Chars()
        {
            string dbName = nameof(Employment_RoleTitle_TruncatedAt255Chars);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            // Build employment fixture with a 300-char role title
            string longTitle = new string('A', 300);
            string json = LoadFixture("employment.json")
                .Replace("\"role-title\": \"Researcher\"", $"\"role-title\": \"{longTitle}\"");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);

            Assert.Equal(255, context.DimAffiliations.Single(e => e.Id > 0).PositionNameEn.Length);
        }

        [Fact(DisplayName = "Dataset URL: truncated to 511 chars when longer")]
        public async Task Dataset_Url_TruncatedAt511Chars()
        {
            string dbName = nameof(Dataset_Url_TruncatedAt511Chars);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            // Build works fixture with a 600-char URL on the dataset
            string longUrl = "https://dataset.example.com/" + new string('x', 600);
            string json = LoadFixture("dataset_with_url.json")
                .Replace("\"value\": \"https://dataset.example.com\"", $"\"value\": \"{longUrl}\"");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);

            Assert.Equal(511, context.DimWebLinks.Single(e => e.Id > 0).Url.Length);
        }

        [Fact(DisplayName = "Empty ORCID record: no activity entities created, main name created")]
        public async Task EmptyRecord_DoesNotCreateActivityEntities()
        {
            string dbName = nameof(EmptyRecord_DoesNotCreateActivityEntities);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, LoadFixture("name.json"), LogId);

            // Main name must be created
            Assert.Equal(1, context.DimNames.Count(e => e.Id > 0));
            // Activity entities must NOT be created
            Assert.Equal(0, context.DimAffiliations.Count(e => e.Id > 0));
            Assert.Equal(0, context.DimEducations.Count(e => e.Id > 0));
            Assert.Equal(0, context.DimProfileOnlyPublications.Count(e => e.Id > 0));
            Assert.Equal(0, context.DimProfileOnlyDatasets.Count(e => e.Id > 0));
            Assert.Equal(0, context.DimProfileOnlyFundingDecisions.Count(e => e.Id > 0));
            Assert.Equal(0, context.DimProfileOnlyResearchActivities.Count(e => e.Id > 0));
            Assert.Equal(0, context.DimEmailAddrresses.Count(e => e.Id > 0));
            Assert.Equal(0, context.DimKeywords.Count(e => e.Id > 0));
            Assert.Equal(0, context.DimWebLinks.Count(e => e.Id > 0));
            Assert.Equal(0, context.DimIdentifierlessData.Count(e => e.Id > 0));
        }

        // =========================================================================
        // PRIORITY 5: Works → ResearchActivity path
        // Works of type conference-paper / lecture-speech / other / etc. are parsed
        // into OrcidWorks.ResearchActivities and imported as DimProfileOnlyResearchActivity.
        // This path is distinct from the distinctions/memberships path (Priority 1/2).
        // =========================================================================

        [Fact(DisplayName = "Works research activity (conference-paper): imported once, not duplicated on re-import")]
        public async Task WorkResearchActivity_ImportedAndNotDuplicated()
        {
            string dbName = nameof(WorkResearchActivity_ImportedAndNotDuplicated);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);
            string json = LoadFixture("works_conference_paper.json");

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterFirst = context.DimProfileOnlyResearchActivities.Count(e => e.Id > 0);

            var activity = context.DimProfileOnlyResearchActivities.Single(e => e.Id > 0);
            Assert.Equal("Test Conference Paper", activity.NameEn);
            Assert.Equal(OrcidDataSourceId, activity.DimRegisteredDataSourceId);

            await service.ImportOrcidRecordJsonIntoUserProfile(UserProfileId, json, LogId);
            int countAfterSecond = context.DimProfileOnlyResearchActivities.Count(e => e.Id > 0);

            Assert.Equal(1, countAfterFirst);
            Assert.Equal(countAfterFirst, countAfterSecond);
        }

        // =========================================================================
        // PRIORITY 6: AddDimDates standalone
        // AddDimDates is called at the top of every import. Testing it directly
        // verifies its dedup logic independently of the full import pipeline.
        // =========================================================================

        [Fact(DisplayName = "AddDimDates: creates DimDate rows for all date fields in the fixture")]
        public async Task AddDimDates_CreatesDatesFromFixture()
        {
            string dbName = nameof(AddDimDates_CreatesDatesFromFixture);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);

            // education.json has one education: start (2000,09,null→0), end (2004,06,null→0)
            // The sentinel DimDate(0,0,0) already exists; neither date matches it, so 2 new rows.
            await service.AddDimDates(LoadFixture("education.json"), DateTime.UtcNow, LogId);

            var dates = context.DimDates.Where(e => e.Id > 0).OrderBy(e => e.Year).ToList();
            Assert.Equal(2, dates.Count);
            Assert.Equal(2000, dates[0].Year);
            Assert.Equal(9, dates[0].Month);
            Assert.Equal(2004, dates[1].Year);
            Assert.Equal(6, dates[1].Month);
        }

        [Fact(DisplayName = "AddDimDates: does not duplicate existing DimDate rows on re-call")]
        public async Task AddDimDates_DoesNotDuplicateDates()
        {
            string dbName = nameof(AddDimDates_DoesNotDuplicateDates);
            using var context = CreateContext(dbName);
            await SeedRequiredData(context);
            var service = CreateService(context);
            string json = LoadFixture("education.json");

            await service.AddDimDates(json, DateTime.UtcNow, LogId);
            int countAfterFirst = context.DimDates.Count(e => e.Id > 0);

            await service.AddDimDates(json, DateTime.UtcNow, LogId);
            int countAfterSecond = context.DimDates.Count(e => e.Id > 0);

            Assert.Equal(2, countAfterFirst);
            Assert.Equal(countAfterFirst, countAfterSecond);
        }
    }
}
