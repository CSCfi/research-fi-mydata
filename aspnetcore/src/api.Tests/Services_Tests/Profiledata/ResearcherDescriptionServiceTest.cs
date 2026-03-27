using Xunit;
using api.Services;
using api.Services.Profiledata;
using Microsoft.EntityFrameworkCore;
using api.Models.Ttv;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;
using api.Models.Common;

namespace api.Tests.Profiledata
{
    [Collection("ResearcherDescriptionService tests")]
    public class ResearcherDescriptionServiceTests
    {
        private TtvContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TtvContext(options);
        }

        private ResearcherDescriptionService CreateService(TtvContext context)
        {
            return new ResearcherDescriptionService(
                ttvContext: context,
                dataSourceHelperService: new DataSourceHelperService(),
                languageService: new LanguageService(),
                logger: new NullLogger<ResearcherDescriptionService>());
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
            var testData = ResearcherDescriptionServiceTestData.Create();
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
    }
}