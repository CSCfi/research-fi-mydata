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
    [Collection("NameService tests")]
    public class NameServiceTests
    {
        private TtvContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TtvContext(options);
        }

        private NameService CreateService(TtvContext context)
        {
            return new NameService(
                ttvContext: context,
                languageService: new LanguageService(),
                logger: new NullLogger<NameService>());
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
            var testData = TestProfiledata.Create();
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
            var testData = TestProfiledata.Create();
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
    }
}