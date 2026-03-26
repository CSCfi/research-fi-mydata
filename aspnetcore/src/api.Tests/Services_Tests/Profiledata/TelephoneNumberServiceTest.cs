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
    [Collection("TelephoneNumberService tests")]
    public class TelephoneNumberServiceTests
    {
        private TtvContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TtvContext(options);
        }

        private TelephoneNumberService CreateService(TtvContext context)
        {
            return new TelephoneNumberService(
                ttvContext: context,
                languageService: new LanguageService(),
                logger: new NullLogger<TelephoneNumberService>());
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
            var testData = TestProfiledata.Create();
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
    }
}