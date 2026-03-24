using Xunit;
using api.Services;
using Microsoft.EntityFrameworkCore;
using api.Models.Ttv;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;
using api.Models.Common;
using api.Models.ProfileEditor.Items;

namespace api.Tests
{
    [Collection("Profile data service tests")]
    public class ProfileDataServiceTests
    {
        private TtvContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TtvContext(options);
        }

        private ProfileDataService CreateService(TtvContext context)
        {
            DataSourceHelperService dataSourceHelperService = new DataSourceHelperService();
            LanguageService languageService = new LanguageService();
            UtilityService utilityService = new UtilityService();
            return new ProfileDataService(
                ttvContext: context,
                dataSourceHelperService: dataSourceHelperService,
                languageService: languageService,
                utilityService: utilityService,
                duplicateHandlerService: new DuplicateHandlerService(),
                logger: new NullLogger<ProfileDataService>());
        }

        [Fact]
        public async Task GetProfileEditorNames_ReturnsEmpty_WhenNoMatchingUserProfile()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorNames_ReturnsEmpty_WhenNoMatchingUserProfile));
            var service = CreateService(context);

            var result = await service.GetProfileEditorNames(userprofileId: 999);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProfileEditorNames_ReturnsNames_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorNames_ReturnsNames_WhenMatchingUserProfileExists));
            var testData = ProfileDataServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorNames(
                userprofileId: 1,
                forElasticsearch: false
            );

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("John", result[0].FirstNames);
            Assert.Equal("Doe", result[0].LastName);
            Assert.Equal("Doe John", result[0].FullName);
            Assert.Equal(1, result[0].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.PERSON_NAME, result[0].itemMeta.Type);
            Assert.True(result[0].itemMeta.Show);
            Assert.True(result[0].itemMeta.PrimaryValue);
            Assert.Single(result[0].DataSources);
            Assert.Equal(1, result[0].DataSources[0].Id);
            Assert.Equal("DataSource1", result[0].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[0].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[0].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[0].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[0].DataSources[0].Organization.SectorId);

            Assert.Equal("Jack", result[1].FirstNames);
            Assert.Equal("Smith", result[1].LastName);
            Assert.Equal("Smith Jack", result[1].FullName);
            Assert.Equal(2, result[1].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.PERSON_NAME, result[1].itemMeta.Type);
            Assert.False(result[1].itemMeta.Show);
            Assert.False(result[1].itemMeta.PrimaryValue);
            Assert.Single(result[1].DataSources);
            Assert.Equal(2, result[1].DataSources[0].Id);
            Assert.Equal("DataSource2", result[1].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[1].DataSources[0].Organization.SectorId);

            // When forElasticsearch is true, only the name with Show = true should be returned
            result = await service.GetProfileEditorNames(
                userprofileId: 1,
                forElasticsearch: true
            );
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetProfileEditorOtherNames_ReturnsEmpty_WhenNoMatchingUserProfile()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorOtherNames_ReturnsEmpty_WhenNoMatchingUserProfile));
            var service = CreateService(context);
            var result = await service.GetProfileEditorOtherNames(userprofileId: 999);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProfileEditorOtherNames_ReturnsOtherNames_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorOtherNames_ReturnsOtherNames_WhenMatchingUserProfileExists));
            var testData = ProfileDataServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorOtherNames(
                userprofileId: 1,
                forElasticsearch: false
            );

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("", result[0].FirstNames);
            Assert.Equal("", result[0].LastName);
            Assert.Equal("John Doe 2", result[0].FullName);
            Assert.Equal(3, result[0].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.PERSON_OTHER_NAMES, result[0].itemMeta.Type);
            Assert.True(result[0].itemMeta.Show);
            Assert.True(result[0].itemMeta.PrimaryValue);
            Assert.Single(result[0].DataSources);
            Assert.Equal(1, result[0].DataSources[0].Id);
            Assert.Equal("DataSource1", result[0].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[0].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[0].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[0].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[0].DataSources[0].Organization.SectorId);

            Assert.Equal("", result[1].FirstNames);
            Assert.Equal("", result[1].LastName);
            Assert.Equal("Jack Smith 2", result[1].FullName);
            Assert.Equal(4, result[1].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.PERSON_OTHER_NAMES, result[1].itemMeta.Type);
            Assert.False(result[1].itemMeta.Show);
            Assert.False(result[1].itemMeta.PrimaryValue);
            Assert.Single(result[1].DataSources);
            Assert.Equal(2, result[1].DataSources[0].Id);
            Assert.Equal("DataSource2", result[1].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[1].DataSources[0].Organization.SectorId);

            // When forElasticsearch is true, only the name with Show = true should be returned
            result = await service.GetProfileEditorOtherNames(
                userprofileId: 1,
                forElasticsearch: true
            );
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetProfileEditorEmails_ReturnsEmpty_WhenNoMatchingUserProfile()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorEmails_ReturnsEmpty_WhenNoMatchingUserProfile));
            var service = CreateService(context);
            var result = await service.GetProfileEditorEmails(userprofileId: 999);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProfileEditorEmails_ReturnsEmails_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorEmails_ReturnsEmails_WhenMatchingUserProfileExists));
            var testData = ProfileDataServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorEmails(
                userprofileId: 1,
                forElasticsearch: false
            );

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("test1@example.com", result[0].Value);
            Assert.Equal(1, result[0].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.PERSON_EMAIL_ADDRESS, result[0].itemMeta.Type);
            Assert.True(result[0].itemMeta.Show);
            Assert.True(result[0].itemMeta.PrimaryValue);
            Assert.Single(result[0].DataSources);
            Assert.Equal(1, result[0].DataSources[0].Id);
            Assert.Equal("DataSource1", result[0].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[0].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[0].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[0].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[0].DataSources[0].Organization.SectorId);

            Assert.Equal("test2@example.com", result[1].Value);
            Assert.Equal(Constants.ItemMetaTypes.PERSON_EMAIL_ADDRESS, result[1].itemMeta.Type);
            Assert.False(result[1].itemMeta.Show);
            Assert.False(result[1].itemMeta.PrimaryValue);
            Assert.Single(result[1].DataSources);
            Assert.Equal(2, result[1].DataSources[0].Id);
            Assert.Equal("DataSource2", result[1].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[1].DataSources[0].Organization.SectorId);

            // When forElasticsearch is true, only the name with Show = true should be returned
            result = await service.GetProfileEditorEmails(
                userprofileId: 1,
                forElasticsearch: true
            );
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetProfileEditorTelephoneNumbers_ReturnsEmpty_WhenNoMatchingUserProfile()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorTelephoneNumbers_ReturnsEmpty_WhenNoMatchingUserProfile));
            var service = CreateService(context);
            var result = await service.GetProfileEditorTelephoneNumbers(userprofileId: 999);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProfileEditorTelephoneNumbers_ReturnsTelephoneNumbers_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorTelephoneNumbers_ReturnsTelephoneNumbers_WhenMatchingUserProfileExists));
            var testData = ProfileDataServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorTelephoneNumbers(
                userprofileId: 1,
                forElasticsearch: false
            );

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("+358501234567", result[0].Value);
            Assert.Equal(1, result[0].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.PERSON_TELEPHONE_NUMBER, result[0].itemMeta.Type);
            Assert.True(result[0].itemMeta.Show);
            Assert.True(result[0].itemMeta.PrimaryValue);
            Assert.Single(result[0].DataSources);
            Assert.Equal(1, result[0].DataSources[0].Id);
            Assert.Equal("DataSource1", result[0].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[0].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[0].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[0].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[0].DataSources[0].Organization.SectorId);

            Assert.Equal("+358501234568", result[1].Value);
            Assert.Equal(Constants.ItemMetaTypes.PERSON_TELEPHONE_NUMBER, result[1].itemMeta.Type);
            Assert.False(result[1].itemMeta.Show);
            Assert.False(result[1].itemMeta.PrimaryValue);
            Assert.Single(result[1].DataSources);
            Assert.Equal(2, result[1].DataSources[0].Id);
            Assert.Equal("DataSource2", result[1].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[1].DataSources[0].Organization.SectorId);

            // When forElasticsearch is true, only the name with Show = true should be returned
            result = await service.GetProfileEditorTelephoneNumbers(
                userprofileId: 1,
                forElasticsearch: true
            );
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetProfileEditorWebLinks_ReturnsEmpty_WhenNoMatchingUserProfile()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorWebLinks_ReturnsEmpty_WhenNoMatchingUserProfile));
            var service = CreateService(context);
            var result = await service.GetProfileEditorWebLinks(userprofileId: 999);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProfileEditorWebLinks_ReturnsWebLinks_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorWebLinks_ReturnsWebLinks_WhenMatchingUserProfileExists));
            var testData = ProfileDataServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorWebLinks(
                userprofileId: 1,
                forElasticsearch: false
            );

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("https://example1.com", result[0].Url);
            Assert.Equal("Example1", result[0].LinkLabel);
            Assert.Equal("Website1", result[0].LinkType);
            Assert.Equal(1, result[0].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.PERSON_WEB_LINK, result[0].itemMeta.Type);
            Assert.True(result[0].itemMeta.Show);
            Assert.True(result[0].itemMeta.PrimaryValue);
            Assert.Single(result[0].DataSources);
            Assert.Equal(1, result[0].DataSources[0].Id);
            Assert.Equal("DataSource1", result[0].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[0].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[0].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[0].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[0].DataSources[0].Organization.SectorId);

            Assert.Equal("https://example2.org", result[1].Url);
            Assert.Equal("Example2", result[1].LinkLabel);
            Assert.Equal("Website2", result[1].LinkType);
            Assert.Equal(Constants.ItemMetaTypes.PERSON_WEB_LINK, result[1].itemMeta.Type);
            Assert.False(result[1].itemMeta.Show);
            Assert.False(result[1].itemMeta.PrimaryValue);
            Assert.Single(result[1].DataSources);
            Assert.Equal(2, result[1].DataSources[0].Id);
            Assert.Equal("DataSource2", result[1].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[1].DataSources[0].Organization.SectorId);

            // When forElasticsearch is true, only the name with Show = true should be returned
            result = await service.GetProfileEditorWebLinks(
                userprofileId: 1,
                forElasticsearch: true
            );
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetProfileEditorKeywords_ReturnsEmpty_WhenNoMatchingUserProfile()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorKeywords_ReturnsEmpty_WhenNoMatchingUserProfile));
            var service = CreateService(context);
            var result = await service.GetProfileEditorKeywords(userprofileId: 999);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProfileEditorKeywords_ReturnsKeywords_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorKeywords_ReturnsKeywords_WhenMatchingUserProfileExists));
            var testData = ProfileDataServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorKeywords(
                userprofileId: 1,
                forElasticsearch: false
            );

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Keyword1", result[0].Value);
            Assert.Equal(1, result[0].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.PERSON_KEYWORD, result[0].itemMeta.Type);
            Assert.True(result[0].itemMeta.Show);
            Assert.True(result[0].itemMeta.PrimaryValue);
            Assert.Single(result[0].DataSources);
            Assert.Equal(1, result[0].DataSources[0].Id);
            Assert.Equal("DataSource1", result[0].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[0].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[0].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[0].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[0].DataSources[0].Organization.SectorId);

            Assert.Equal("Keyword2", result[1].Value);
            Assert.Equal(2, result[1].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.PERSON_KEYWORD, result[1].itemMeta.Type);
            Assert.False(result[1].itemMeta.Show);
            Assert.False(result[1].itemMeta.PrimaryValue);
            Assert.Single(result[1].DataSources);
            Assert.Equal(2, result[1].DataSources[0].Id);
            Assert.Equal("DataSource2", result[1].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[1].DataSources[0].Organization.SectorId);

            // When forElasticsearch is true, only the name with Show = true should be returned
            result = await service.GetProfileEditorKeywords(
                userprofileId: 1,
                forElasticsearch: true
            );
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetProfileEditorResearcherDescriptions_ReturnsEmpty_WhenNoMatchingUserProfile()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorResearcherDescriptions_ReturnsEmpty_WhenNoMatchingUserProfile));
            var service = CreateService(context);
            var result = await service.GetProfileEditorResearcherDescriptions(userprofileId: 999);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProfileEditorResearcherDescriptions_ReturnsResearcherDescriptions_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorResearcherDescriptions_ReturnsResearcherDescriptions_WhenMatchingUserProfileExists));
            var testData = ProfileDataServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorResearcherDescriptions(
                userprofileId: 1,
                forElasticsearch: false
            );

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Researcher description 1", result[0].ResearchDescriptionFi);
            Assert.Equal("Researcher description 1", result[0].ResearchDescriptionEn);
            Assert.Equal("Researcher description 1", result[0].ResearchDescriptionSv);
            Assert.Equal(1, result[0].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.PERSON_RESEARCHER_DESCRIPTION, result[0].itemMeta.Type);
            Assert.True(result[0].itemMeta.Show);
            Assert.True(result[0].itemMeta.PrimaryValue);
            Assert.Single(result[0].DataSources);
            Assert.Equal(2, result[0].DataSources[0].Id);
            Assert.Equal("DataSource2", result[0].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[0].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[0].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[0].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[0].DataSources[0].Organization.SectorId);

            Assert.Equal("Tutkijakuvaus 2 Fi", result[1].ResearchDescriptionFi);
            Assert.Equal("Researcher description 2 En", result[1].ResearchDescriptionEn);
            Assert.Equal("Forskarbeskrivning 2 Sv", result[1].ResearchDescriptionSv);
            Assert.Equal(2, result[1].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.PERSON_RESEARCHER_DESCRIPTION, result[1].itemMeta.Type);
            Assert.False(result[1].itemMeta.Show);
            Assert.False(result[1].itemMeta.PrimaryValue);
            Assert.Single(result[1].DataSources);
            Assert.Equal(3, result[1].DataSources[0].Id);
            Assert.Equal("TTV", result[1].DataSources[0].RegisteredDataSource);
            Assert.Equal("TTV Fi", result[1].DataSources[0].Organization.NameFi);
            Assert.Equal("TTV En", result[1].DataSources[0].Organization.NameEn);
            Assert.Equal("TTV Sv", result[1].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[1].DataSources[0].Organization.SectorId);

            // When forElasticsearch is true, only the name with Show = true should be returned
            result = await service.GetProfileEditorResearcherDescriptions(
                userprofileId: 1,
                forElasticsearch: true
            );
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetProfileEditorExternalIdentifiers_ReturnsEmpty_WhenNoMatchingUserProfile()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorExternalIdentifiers_ReturnsEmpty_WhenNoMatchingUserProfile));
            var service = CreateService(context);
            var result = await service.GetProfileEditorExternalIdentifiers(userprofileId: 999);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProfileEditorExternalIdentifiers_ReturnsExternalIdentifiers_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorExternalIdentifiers_ReturnsExternalIdentifiers_WhenMatchingUserProfileExists));
            var testData = ProfileDataServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorExternalIdentifiers(
                userprofileId: 1,
                forElasticsearch: false
            );

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("test-pid-content-1", result[0].PidContent);
            Assert.Equal("test-pid-type-1", result[0].PidType);
            Assert.Equal(1, result[0].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.PERSON_EXTERNAL_IDENTIFIER, result[0].itemMeta.Type);
            Assert.True(result[0].itemMeta.Show);
            Assert.True(result[0].itemMeta.PrimaryValue);
            Assert.Single(result[0].DataSources);
            Assert.Equal(1, result[0].DataSources[0].Id);
            Assert.Equal("DataSource1", result[0].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[0].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[0].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[0].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[0].DataSources[0].Organization.SectorId);

            Assert.Equal("test-pid-content-2", result[1].PidContent);
            Assert.Equal("test-pid-type-2", result[1].PidType);
            Assert.Equal(2, result[1].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.PERSON_EXTERNAL_IDENTIFIER, result[1].itemMeta.Type);
            Assert.False(result[1].itemMeta.Show);
            Assert.False(result[1].itemMeta.PrimaryValue);
            Assert.Single(result[1].DataSources);
            Assert.Equal(2, result[1].DataSources[0].Id);
            Assert.Equal("DataSource2", result[1].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[1].DataSources[0].Organization.SectorId);

            // When forElasticsearch is true, only the name with Show = true should be returned
            result = await service.GetProfileEditorExternalIdentifiers(
                userprofileId: 1,
                forElasticsearch: true
            );
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetProfileEditorEducations_ReturnsEmpty_WhenNoMatchingUserProfile()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorEducations_ReturnsEmpty_WhenNoMatchingUserProfile));
            var service = CreateService(context);
            var result = await service.GetProfileEditorEducations(userprofileId: 999);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProfileEditorEducations_ReturnsEducations_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorEducations_ReturnsEducations_WhenMatchingUserProfileExists));
            var testData = ProfileDataServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorEducations(
                userprofileId: 1,
                forElasticsearch: false
            );
            Assert.Equal(2, result.Count);

            Assert.Equal("Education 1 name Fi", result[0].NameFi);
            Assert.Equal("Education 1 name En", result[0].NameEn);
            Assert.Equal("Education 1 name Sv", result[0].NameSv);
            Assert.Equal("Test institution name 1", result[0].DegreeGrantingInstitutionName);
            Assert.Equal(2020, result[0].StartDate.Year);
            Assert.Equal(1, result[0].StartDate.Month);
            Assert.Equal(15, result[0].StartDate.Day);
            Assert.Equal(2022, result[0].EndDate.Year);
            Assert.Equal(6, result[0].EndDate.Month);
            Assert.Equal(30, result[0].EndDate.Day);
            Assert.Equal(1, result[0].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_EDUCATION, result[0].itemMeta.Type);
            Assert.True(result[0].itemMeta.Show);
            Assert.True(result[0].itemMeta.PrimaryValue);
            Assert.Single(result[0].DataSources);
            Assert.Equal(1, result[0].DataSources[0].Id);
            Assert.Equal("DataSource1", result[0].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[0].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[0].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[0].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[0].DataSources[0].Organization.SectorId);

            Assert.Equal("Education 2 name", result[1].NameFi);
            Assert.Equal("Education 2 name", result[1].NameEn);
            Assert.Equal("Education 2 name", result[1].NameSv);
            Assert.Equal("Test institution name 2", result[1].DegreeGrantingInstitutionName);
            Assert.Equal(2018, result[1].StartDate.Year);
            Assert.Equal(9, result[1].StartDate.Month);
            Assert.Equal(1, result[1].StartDate.Day);
            Assert.Equal(2020, result[1].EndDate.Year);
            Assert.Equal(5, result[1].EndDate.Month);
            Assert.Equal(31, result[1].EndDate.Day);
            Assert.Equal(2, result[1].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_EDUCATION, result[1].itemMeta.Type);
            Assert.False(result[1].itemMeta.Show);
            Assert.False(result[1].itemMeta.PrimaryValue);
            Assert.Single(result[1].DataSources);
            Assert.Equal(2, result[1].DataSources[0].Id);
            Assert.Equal("DataSource2", result[1].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[1].DataSources[0].Organization.SectorId);

            // When forElasticsearch is true, only the name with Show = true should be returned
            result = await service.GetProfileEditorEducations(
                userprofileId: 1,
                forElasticsearch: true
            );
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetProfileEditorAffiliations_ReturnsEmpty_WhenNoMatchingUserProfile()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorAffiliations_ReturnsEmpty_WhenNoMatchingUserProfile));
            var service = CreateService(context);
            var result = await service.GetProfileEditorAffiliations(userprofileId: 999);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProfileEditorAffiliations_ReturnsAffiliations_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorAffiliations_ReturnsAffiliations_WhenMatchingUserProfileExists));
            var testData = ProfileDataServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorAffiliations(
                userprofileId: 1,
                forElasticsearch: false // Property "sector" should be empty in the result when forElasticsearch is false.
            );

            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("Affiliation 1 organization broader name Fi", result[0].OrganizationNameFi);
            Assert.Equal("Affiliation 1 organization broader name En", result[0].OrganizationNameEn);
            Assert.Equal("Affiliation 1 organization broader name Sv", result[0].OrganizationNameSv);
            Assert.Equal("Affiliation 1 organization name Fi", result[0].DepartmentNameFi);
            Assert.Equal("Affiliation 1 organization name En", result[0].DepartmentNameEn);
            Assert.Equal("Affiliation 1 organization name Sv", result[0].DepartmentNameSv);
            Assert.Equal("Affiliation 1 position name fi", result[0].PositionNameFi);
            Assert.Equal("Affiliation 1 position name en", result[0].PositionNameEn);
            Assert.Equal("Affiliation 1 position name sv", result[0].PositionNameSv);
            Assert.Equal("Affiliation 1 type Fi", result[0].AffiliationTypeFi);
            Assert.Equal("Affiliation 1 type En", result[0].AffiliationTypeEn);
            Assert.Equal("Affiliation 1 type Sv", result[0].AffiliationTypeSv);
            Assert.Equal(2021, result[0].StartDate.Year);
            Assert.Equal(2, result[0].StartDate.Month);
            Assert.Equal(1, result[0].StartDate.Day);
            Assert.Equal(2023, result[0].EndDate.Year);
            Assert.Equal(1, result[0].EndDate.Month);
            Assert.Equal(31, result[0].EndDate.Day);
            Assert.Empty(result[0].sector);
    
            Assert.Equal("Affiliation 2 organization name Fi", result[1].OrganizationNameFi);
            Assert.Equal("Affiliation 2 organization name En", result[1].OrganizationNameEn);
            Assert.Equal("Affiliation 2 organization name Sv", result[1].OrganizationNameSv);
            Assert.Equal("", result[1].DepartmentNameFi);
            Assert.Equal("", result[1].DepartmentNameEn);
            Assert.Equal("", result[1].DepartmentNameSv);    
            Assert.Equal("Affiliation 2 position name en", result[1].PositionNameFi);
            Assert.Equal("Affiliation 2 position name en", result[1].PositionNameEn);
            Assert.Equal("Affiliation 2 position name en", result[1].PositionNameSv);
            Assert.Equal("Affiliation 2 type En", result[1].AffiliationTypeFi);
            Assert.Equal("Affiliation 2 type En", result[1].AffiliationTypeEn);
            Assert.Equal("Affiliation 2 type En", result[1].AffiliationTypeSv);
            Assert.Equal(2020, result[1].StartDate.Year);
            Assert.Equal(3, result[1].StartDate.Month);
            Assert.Equal(13, result[1].StartDate.Day);
            Assert.Equal(2022, result[1].EndDate.Year);
            Assert.Equal(2, result[1].EndDate.Month);
            Assert.Equal(30, result[1].EndDate.Day);
            Assert.Empty(result[1].sector);

            Assert.Equal("Affiliation 3 identifierless data value Fi", result[2].OrganizationNameFi);
            Assert.Equal("Affiliation 3 identifierless data value En", result[2].OrganizationNameEn);
            Assert.Equal("Affiliation 3 identifierless data value Sv", result[2].OrganizationNameSv);
            Assert.Equal("Affiliation 3 identifierless data child value Fi", result[2].DepartmentNameFi);
            Assert.Equal("Affiliation 3 identifierless data child value En", result[2].DepartmentNameEn);
            Assert.Equal("Affiliation 3 identifierless data child value Sv", result[2].DepartmentNameSv);
            Assert.Equal("Affiliation 3 position name fi", result[2].PositionNameFi);
            Assert.Equal("Affiliation 3 position name en", result[2].PositionNameEn);
            Assert.Equal("Affiliation 3 position name sv", result[2].PositionNameSv);
            Assert.Equal("Affiliation 3 type Fi", result[2].AffiliationTypeFi);
            Assert.Equal("Affiliation 3 type En", result[2].AffiliationTypeEn);
            Assert.Equal("Affiliation 3 type Sv", result[2].AffiliationTypeSv);
            Assert.Equal(2020, result[2].StartDate.Year);
            Assert.Equal(3, result[2].StartDate.Month);
            Assert.Equal(13, result[2].StartDate.Day);
            Assert.Equal(2022, result[2].EndDate.Year);
            Assert.Equal(2, result[2].EndDate.Month);
            Assert.Equal(30, result[2].EndDate.Day);
            Assert.Empty(result[2].sector);

            // When forElasticsearch is true, only the name with Show = true should be returned
            result = await service.GetProfileEditorAffiliations(
                userprofileId: 1,
                forElasticsearch: true
            );
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetProfileEditorAffiliations_ReturnsAffiliations_WithSectors_WhenForElasticsearchIsTrue()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorAffiliations_ReturnsAffiliations_WithSectors_WhenForElasticsearchIsTrue));
            var testData = ProfileDataServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorAffiliations(
                userprofileId: 1,
                forElasticsearch: true // Property "sector" should be populated in the result when forElasticsearch is true.
            );

            Assert.NotEmpty(result);
            Assert.Single(result);

            Assert.NotEmpty(result[0].sector);
            Assert.Single(result[0].sector);
            Assert.Equal("S2", result[0].sector[0].sectorId);
            Assert.Equal("Sector 3 Fi", result[0].sector[0].nameFiSector);
            Assert.Equal("Sector 3 En", result[0].sector[0].nameEnSector);
            Assert.Equal("Sector 3 Sv", result[0].sector[0].nameSvSector);
            Assert.NotEmpty(result[0].sector[0].organization);
            Assert.Single(result[0].sector[0].organization);
            Assert.Equal("Affiliation 1 organization organizationId", result[0].sector[0].organization[0].organizationId);
            Assert.Equal("Affiliation 1 organization broader name Fi", result[0].sector[0].organization[0].OrganizationNameFi);
            Assert.Equal("Affiliation 1 organization broader name En", result[0].sector[0].organization[0].OrganizationNameEn);
            Assert.Equal("Affiliation 1 organization broader name Sv", result[0].sector[0].organization[0].OrganizationNameSv);
        }

        [Fact]
        public async Task GetProfileEditorPublications_ReturnsEmpty_WhenNoMatchingUserProfile()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorPublications_ReturnsEmpty_WhenNoMatchingUserProfile));
            var service = CreateService(context);
            var result = await service.GetProfileEditorPublications(userprofileId: 999);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProfileEditorPublications_ReturnsPublications_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorPublications_ReturnsPublications_WhenMatchingUserProfileExists));
            var testData = ProfileDataServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorPublications(
                userprofileId: 1,
                forElasticsearch: false
            );

            Assert.NotEmpty(result);
            // Expected count is 4.
            // Out of 3 DimPublications 2 of them should be deduplicated based on the same PublicationId. That leaves 2 DimPublications.
            // Out of 3 DimProfileOnlyPublications 1 should be deduplicated with one of the DimPublications base on the same Doi. That leaves 2 DimProfileOnlyPublications.
            // Combined count of deduplicated DimPublications and deduplicated DimProfileOnlyPublications should be 4.
            Assert.Equal(4, result.Count);

            Assert.Equal("DimPublication1 Article number text", result[0].ArticleNumberText);
            Assert.Equal("DimPublication1 Authors text", result[0].AuthorsText);
            Assert.Equal("DimPublication1 Conference name", result[0].ConferenceName);
            Assert.Equal("10.1234/doi_dimpublication_1", result[0].Doi);
            Assert.Equal("DimPublication1 Issue number", result[0].IssueNumber);
            Assert.Equal("DimPublication1 Journal name", result[0].JournalName);
            Assert.Equal(1, result[0].OpenAccess);
            Assert.Equal("123", result[0].PageNumberText);
            Assert.Equal("DimPublication1 Parent publication name", result[0].ParentPublicationName);
            Assert.Equal("DimPublication PublicationId to deduplicate", result[0].PublicationId);
            Assert.Equal("DimPublication1 Publication name", result[0].PublicationName);
            Assert.Equal(2020, result[0].PublicationYear);
            Assert.Equal("DimPublication1 Publisher name", result[0].PublisherName);
            Assert.Equal("https://example.com/selfarchivedurl1", result[0].SelfArchivedAddress);
            Assert.Equal("1", result[0].SelfArchivedCode);
            Assert.Equal("DimPublication1 Volume number", result[0].Volume);
            Assert.Equal(2, result[0].DataSources.Count); // After deduplication by PublicationId, the publication should have combined data sources of the deduplicated publications.

            Assert.Equal("DimPublication3 Article number text", result[1].ArticleNumberText);
            Assert.Equal("DimPublication3 Authors text", result[1].AuthorsText);
            Assert.Equal("DimPublication3 Conference name", result[1].ConferenceName);
            Assert.Equal("10.1234/doi_to_deduplicate", result[1].Doi);
            Assert.Equal("DimPublication3 Issue number", result[1].IssueNumber);
            Assert.Equal("DimPublication3 Journal name", result[1].JournalName);
            Assert.Equal(0, result[1].OpenAccess);
            Assert.Equal("345", result[1].PageNumberText);
            Assert.Equal("DimPublication3 Parent publication name", result[1].ParentPublicationName);
            Assert.Equal("DimPublication3 PublicationId", result[1].PublicationId);
            Assert.Equal("DimPublication3 Publication name", result[1].PublicationName);
            Assert.Equal(2022, result[1].PublicationYear);
            Assert.Equal("DimPublication3 Publisher name", result[1].PublisherName);
            Assert.Equal("", result[1].SelfArchivedAddress);
            Assert.Equal("0", result[1].SelfArchivedCode);
            Assert.Equal("DimPublication3 Volume number", result[1].Volume);
            Assert.Equal(2, result[1].DataSources.Count); // After deduplication by Doi, the publication should have combined data sources of the deduplicated publications.

            Assert.Equal("DimProfileOnlyPublication2 Article number text", result[2].ArticleNumberText);
            Assert.Equal("DimProfileOnlyPublication2 Authors text", result[2].AuthorsText);
            Assert.Equal("DimProfileOnlyPublication2 Conference name", result[2].ConferenceName);
            Assert.Equal("10.1234/doi_dimprofileonlypublication_2", result[2].Doi);
            Assert.Equal("DimProfileOnlyPublication2 Issue number", result[2].IssueNumber);
            Assert.Equal("", result[2].JournalName);
            Assert.Equal(1, result[2].OpenAccess);
            Assert.Equal("567", result[2].PageNumberText);
            Assert.Equal("DimProfileOnlyPublication2 Parent publication name", result[2].ParentPublicationName);
            Assert.Equal("DimProfileOnlyPublication2 PublicationId", result[2].PublicationId);
            Assert.Equal("DimProfileOnlyPublication2 Publication name", result[2].PublicationName);
            Assert.Equal(2024, result[2].PublicationYear);
            Assert.Equal("DimProfileOnlyPublication2 Publisher name", result[2].PublisherName);
            Assert.Equal("", result[2].SelfArchivedAddress);
            Assert.Equal("0", result[2].SelfArchivedCode);
            Assert.Equal("DimProfileOnlyPublication2 Volume number", result[2].Volume);
            Assert.Single(result[2].DataSources);

            Assert.Equal("DimProfileOnlyPublication3 Article number text", result[3].ArticleNumberText);
            Assert.Equal("DimProfileOnlyPublication3 Authors text", result[3].AuthorsText);
            Assert.Equal("DimProfileOnlyPublication3 Conference name", result[3].ConferenceName);
            Assert.Equal("", result[3].Doi);
            Assert.Equal("DimProfileOnlyPublication3 Issue number", result[3].IssueNumber);
            Assert.Equal("", result[3].JournalName);
            Assert.Equal(9, result[3].OpenAccess);
            Assert.Equal("678", result[3].PageNumberText);
            Assert.Equal("DimProfileOnlyPublication3 Parent publication name", result[3].ParentPublicationName);
            Assert.Equal("", result[3].PublicationId);
            Assert.Equal( "DimProfileOnlyPublication3 Publication name", result[3].PublicationName);
            Assert.Equal(2025, result[3].PublicationYear);
            Assert.Equal("DimProfileOnlyPublication3 Publisher name", result[3].PublisherName);
            Assert.Equal("DimProfileOnlyPublication3 Volume number", result[3].Volume);
            Assert.Equal("", result[3].SelfArchivedAddress);
            Assert.Equal("0", result[3].SelfArchivedCode);
            Assert.Single(result[3].DataSources);


            // When forElasticsearch is true, only publications with Show = true should be returned
            result = await service.GetProfileEditorPublications(
                userprofileId: 1,
                forElasticsearch: true
            );
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
        }
    }
}