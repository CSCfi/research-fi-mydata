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
    [Collection("WebLinkService tests")]
    public class WebLinkServiceTests
    {
        private TtvContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TtvContext(options);
        }

        private WebLinkService CreateService(TtvContext context)
        {
            return new WebLinkService(
                ttvContext: context,
                languageService: new LanguageService(),
                logger: new NullLogger<WebLinkService>());
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
            var testData = WebLinkServiceTestData.Create();
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
            Assert.NotNull(result[0].itemMeta);
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
            Assert.NotNull(result[1].itemMeta);
            Assert.Equal(2, result[1].itemMeta.Id);
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
    }
}