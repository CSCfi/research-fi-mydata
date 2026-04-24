using Xunit;
using api.Services;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;

namespace api.Tests
{
    public class SettingsServiceTestData
    {
        public DimUserProfile UserProfile { get; private set; }

        public static SettingsServiceTestData Create()
        {
            var data = new SettingsServiceTestData();
            data.UserProfile = new DimUserProfile {
                Id = 1,
                SourceId = "Source1",
                PublishNewOrcidData = true,
                HighlightOpeness = true,
                Hidden = true,
                DimKnownPersonId = 5,
                DimKnownPerson = new DimKnownPerson
                {
                    Id = 5,
                    SourceId = "Source1"
                },
                AllowAllSubscriptions = false
            };
            return data;
        }

        public async Task SeedAsync(TtvContext context)
        {
            context.DimUserProfiles.Add(UserProfile);
            context.DimKnownPeople.Add(UserProfile.DimKnownPerson);
            await context.SaveChangesAsync();
        }
    }


    [Collection("Settings service tests")]
    public class SettingsServiceTests
    {
        private TtvContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TtvContext(options);
        }

        private SettingsService CreateService(TtvContext context)
        {
            return new SettingsService(
                ttvContext: context,
                logger: new NullLogger<SettingsService>());
        }

        [Fact(DisplayName = "GetProfileSettings retrieves correct settings for user profile")]
        public async Task GetProfileSettings_RetrievesCorrectSettings()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileSettings_RetrievesCorrectSettings));
            var testData = SettingsServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var results = await service.GetProfileSettings(
                userprofileId: 1,
                forElasticsearch: false
            );

            Assert.NotNull(results);
            Assert.True(results.PublishNewData);
            Assert.True(results.HighlightOpeness);
            Assert.True(results.Hidden);
        }
    }
}