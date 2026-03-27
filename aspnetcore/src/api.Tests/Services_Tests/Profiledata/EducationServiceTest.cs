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
    [Collection("EducationService tests")]
    public class EducationServiceTests
    {
        private TtvContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TtvContext(options);
        }

        private EducationService CreateService(TtvContext context)
        {
            return new EducationService(
                ttvContext: context,
                languageService: new LanguageService(),
                logger: new NullLogger<EducationService>());
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
            var testData = EducationServiceTestData.Create();
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
    }
}