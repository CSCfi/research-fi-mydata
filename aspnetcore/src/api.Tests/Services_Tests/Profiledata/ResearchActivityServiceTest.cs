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
        }
    }
}