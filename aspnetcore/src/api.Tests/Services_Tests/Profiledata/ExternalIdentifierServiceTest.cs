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
    [Collection("ExternalIdentifierService tests")]
    public class ExternalIdentifierServiceTests
    {
        private TtvContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TtvContext(options);
        }

        private ExternalIdentifierService CreateService(TtvContext context)
        {
            return new ExternalIdentifierService(
                ttvContext: context,
                languageService: new LanguageService(),
                logger: new NullLogger<ExternalIdentifierService>());
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
            var testData = ExternalIdentifierServiceTestData.Create();
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
            Assert.NotNull(result[0].itemMeta);
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
            Assert.NotNull(result[1].itemMeta);
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
        }

        [Fact]
        public async Task GetProfileEditorExternalIdentifiers_ReturnsExternalIdentifiers_ForElasticsearch_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorExternalIdentifiers_ReturnsExternalIdentifiers_ForElasticsearch_WhenMatchingUserProfileExists));
            var testData = ExternalIdentifierServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorExternalIdentifiers(
                userprofileId: 1,
                forElasticsearch: true
            );
            Assert.NotEmpty(result);
            Assert.Single(result);
        }
    }
}