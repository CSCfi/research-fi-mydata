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
    [Collection("ResearchActivityService tests")]
    public class ResearchActivityServiceTests
    {
        private TtvContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TtvContext(options);
        }

        private ResearchActivityService CreateService(TtvContext context)
        {
            return new ResearchActivityService(
                ttvContext: context,
                languageService: new LanguageService(),
                logger: new NullLogger<ResearchActivityService>());
        }

        [Fact]
        public async Task GetProfileEditorActiviesAndRewards_ReturnsEmpty_WhenNoMatchingUserProfile()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorActiviesAndRewards_ReturnsEmpty_WhenNoMatchingUserProfile));
            var service = CreateService(context);

            var result = await service.GetProfileEditorActiviesAndRewards(userprofileId: 999);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProfileEditorActiviesAndRewards_ReturnsData_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorActiviesAndRewards_ReturnsData_WhenMatchingUserProfileExists));
            var testData = ResearchActivityServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorActiviesAndRewards(
                userprofileId: 1,
                forElasticsearch: false
            );

            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);

            // result[0] from DimFundingDecision1
            Assert.Equal("DimResearchActivity1 name fi", result[0].NameFi);
            Assert.Equal("DimResearchActivity1 name en", result[0].NameEn);
            Assert.Equal("DimResearchActivity1 name sv", result[0].NameSv);
            Assert.Equal("DimResearchActivity1 description fi", result[0].DescriptionFi);
            Assert.Equal("DimResearchActivity1 description en", result[0].DescriptionEn);
            Assert.Equal("DimResearchActivity1 description sv", result[0].DescriptionSv);
            Assert.Equal("DimResearchActivity1 organization broader name fi", result[0].OrganizationNameFi);
            Assert.Equal("DimResearchActivity1 organization broader name en", result[0].OrganizationNameEn);
            Assert.Equal("DimResearchActivity1 organization broader name sv", result[0].OrganizationNameSv);
            Assert.Equal("DimResearchActivity1 organization name fi", result[0].DepartmentNameFi);
            Assert.Equal("DimResearchActivity1 organization name en", result[0].DepartmentNameEn);
            Assert.Equal("DimResearchActivity1 organization name sv", result[0].DepartmentNameSv);
            Assert.Equal(2019, result[0].StartDate.Year);
            Assert.Equal(1, result[0].StartDate.Month);
            Assert.Equal(11, result[0].StartDate.Day);
            Assert.Equal(2020, result[0].EndDate.Year);
            Assert.Equal(2, result[0].EndDate.Month);
            Assert.Equal(22, result[0].EndDate.Day);
            Assert.Empty(result[0].sector);
            Assert.NotNull(result[0].itemMeta);
            Assert.Equal(100, result[0].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_RESEARCH_ACTIVITY, result[0].itemMeta.Type);
            Assert.True(result[0].itemMeta.Show);
            Assert.True(result[0].itemMeta.PrimaryValue);
            Assert.Single(result[0].DataSources);
            Assert.Equal(1, result[0].DataSources[0].Id);
            Assert.Equal("DataSource1", result[0].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[0].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[0].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[0].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[0].DataSources[0].Organization.SectorId);

            // result[1] from DimFundingDecision2
            Assert.Equal("DimResearchActivity2 name", result[1].NameFi);
            Assert.Equal("DimResearchActivity2 name", result[1].NameEn);
            Assert.Equal("DimResearchActivity2 name", result[1].NameSv);
            Assert.Equal("DimResearchActivity2 description", result[1].DescriptionFi);
            Assert.Equal("DimResearchActivity2 description", result[1].DescriptionEn);
            Assert.Equal("DimResearchActivity2 description", result[1].DescriptionSv);
            Assert.Equal("DimResearchActivity2 organization name", result[1].OrganizationNameFi);
            Assert.Equal("DimResearchActivity2 organization name", result[1].OrganizationNameEn);
            Assert.Equal("DimResearchActivity2 organization name", result[1].OrganizationNameSv);
            Assert.Equal("", result[1].DepartmentNameFi);
            Assert.Equal("", result[1].DepartmentNameEn);
            Assert.Equal("", result[1].DepartmentNameSv);
            Assert.Equal(2021, result[1].StartDate.Year);
            Assert.Equal(2, result[1].StartDate.Month);
            Assert.Equal(12, result[1].StartDate.Day);
            Assert.Equal(2022, result[1].EndDate.Year);
            Assert.Equal(3, result[1].EndDate.Month);
            Assert.Equal(23, result[1].EndDate.Day);
            Assert.Empty(result[1].sector);
            Assert.NotNull(result[1].itemMeta);
            Assert.Equal(200, result[1].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_RESEARCH_ACTIVITY, result[1].itemMeta.Type);
            Assert.False(result[1].itemMeta.Show);
            Assert.False(result[1].itemMeta.PrimaryValue);
            Assert.Single(result[1].DataSources);
            Assert.Equal(2, result[1].DataSources[0].Id);
            Assert.Equal("DataSource2", result[1].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[1].DataSources[0].Organization.SectorId);

            // result[2] from DimFundingDecision3
            Assert.Equal("DimResearchActivity3 name fi", result[2].NameFi);
            Assert.Equal("DimResearchActivity3 name en", result[2].NameEn);
            Assert.Equal("DimResearchActivity3 name sv", result[2].NameSv);
            Assert.Equal("DimResearchActivity3 description fi", result[2].DescriptionFi);
            Assert.Equal("DimResearchActivity3 description en", result[2].DescriptionEn);
            Assert.Equal("DimResearchActivity3 description sv", result[2].DescriptionSv);
            Assert.Equal("DimResearchActivity3 identifierless data value fi", result[2].OrganizationNameFi);
            Assert.Equal("DimResearchActivity3 identifierless data value en", result[2].OrganizationNameEn);
            Assert.Equal("DimResearchActivity3 identifierless data value sv", result[2].OrganizationNameSv);
            Assert.Equal("DimResearchActivity3 identifierless data child value fi", result[2].DepartmentNameFi);
            Assert.Equal("DimResearchActivity3 identifierless data child value en", result[2].DepartmentNameEn);
            Assert.Equal("DimResearchActivity3 identifierless data child value sv", result[2].DepartmentNameSv);
            Assert.Equal(2023, result[2].StartDate.Year);
            Assert.Equal(4, result[2].StartDate.Month);
            Assert.Equal(15, result[2].StartDate.Day);
            Assert.Equal(2024, result[2].EndDate.Year);
            Assert.Equal(5, result[2].EndDate.Month);
            Assert.Equal(16, result[2].EndDate.Day);
            Assert.Empty(result[2].sector);
            Assert.NotNull(result[2].itemMeta);
            Assert.Equal(300, result[2].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_RESEARCH_ACTIVITY, result[2].itemMeta.Type);
            Assert.True(result[2].itemMeta.Show);
            Assert.True(result[2].itemMeta.PrimaryValue);
            Assert.Single(result[2].DataSources);
            Assert.Equal(1, result[2].DataSources[0].Id);
            Assert.Equal("DataSource1", result[2].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[2].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[2].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[2].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[2].DataSources[0].Organization.SectorId);
        }
    }
}