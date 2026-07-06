using Xunit;
using api.Services;
using api.Models.Ttv;
using api.Models.Ai;
using api.Models.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace api.Tests
{
    [Collection("BiographyService tests")]
    public class BiographyServiceTests
    {
        private const int TtvDataSourceId = 99;
        private const int UserProfileId = 1;
        private const string OrcidId = "0000-0001-2345-6789";

        private TtvContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TtvContext(options);
        }

        private BiographyService CreateService(TtvContext context)
        {
            var dataSourceHelper = new DataSourceHelperService();
            dataSourceHelper.DimRegisteredDataSourceId_TTV = TtvDataSourceId;

            var utilityService = new UtilityService();
            var userProfileService = new UserProfileService(utilityService: utilityService);
            var cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

            return new BiographyService(
                ttvContext: context,
                logger: new NullLogger<UserProfileService>(),
                ttvSqlService: new TtvSqlService(),
                dataSourceHelperService: dataSourceHelper,
                utilityService: utilityService,
                userProfileService: userProfileService,
                cache: cache);
        }

        private async Task SeedUserProfileWithDisplaySettingAsync(TtvContext context)
        {
            var userProfile = new DimUserProfile
            {
                Id = UserProfileId,
                OrcidId = OrcidId,
                DimKnownPersonId = 10,
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API
            };
            var displaySetting = new DimFieldDisplaySetting
            {
                Id = 1,
                DimUserProfileId = UserProfileId,
                FieldIdentifier = Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION,
                Show = false,
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API
            };
            context.DimUserProfiles.Add(userProfile);
            context.DimFieldDisplaySettings.Add(displaySetting);
            await context.SaveChangesAsync();
        }

        private async Task SeedExistingBiographyAsync(TtvContext context)
        {
            await SeedUserProfileWithDisplaySettingAsync(context);

            var researcherDescription = new DimResearcherDescription
            {
                Id = 1,
                ResearchDescriptionFi = "Alkuperainen Fi",
                ResearchDescriptionEn = "Original En",
                ResearchDescriptionSv = "Original Sv",
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                DimKnownPersonId = 10,
                DimRegisteredDataSourceId = TtvDataSourceId
            };
            var ffv = new FactFieldValue
            {
                DimUserProfileId = UserProfileId,
                DimResearcherDescriptionId = 1,
                DimRegisteredDataSourceId = TtvDataSourceId,
                DimFieldDisplaySettingsId = 1,
                DimNameId = -1,
                DimWebLinkId = -1,
                DimFundingDecisionId = -1,
                DimPublicationId = -1,
                DimPidId = -1,
                DimPidIdOrcidPutCode = -1,
                DimResearchActivityId = -1,
                DimEventId = -1,
                DimEducationId = -1,
                DimCompetenceId = -1,
                DimResearchCommunityId = -1,
                DimTelephoneNumberId = -1,
                DimEmailAddrressId = -1,
                DimIdentifierlessDataId = -1,
                DimProfileOnlyDatasetId = -1,
                DimProfileOnlyFundingDecisionId = -1,
                DimProfileOnlyPublicationId = -1,
                DimProfileOnlyResearchActivityId = -1,
                DimKeywordId = -1,
                DimAffiliationId = -1,
                DimResearcherToResearchCommunityId = -1,
                DimResearchDatasetId = -1,
                DimReferencedataFieldOfScienceId = -1,
                DimReferencedataActorRoleId = -1,
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Show = false,
                PrimaryValue = false
            };
            context.DimResearcherDescriptions.Add(researcherDescription);
            context.FactFieldValues.Add(ffv);
            await context.SaveChangesAsync();
        }

        [Fact(DisplayName = "CreateOrUpdateBiography creates new biography when none exists")]
        public async Task CreateOrUpdateBiography_CreatesBiography_WhenNoneExists()
        {
            // Arrange
            using var context = CreateInMemoryContext(nameof(CreateOrUpdateBiography_CreatesBiography_WhenNoneExists));
            await SeedUserProfileWithDisplaySettingAsync(context);
            var service = CreateService(context);

            var biography = new Biography { Fi = "Elämäkerta Fi", En = "Biography En", Sv = "Biografi Sv" };

            // Act
            bool result = await service.CreateOrUpdateBiography(UserProfileId, biography);

            // Assert
            Assert.True(result);

            // Verify new DimResearcherDescription was created with correct values
            var createdDescription = await context.DimResearcherDescriptions.FirstOrDefaultAsync();
            Assert.NotNull(createdDescription);
            Assert.Equal("Elämäkerta Fi", createdDescription.ResearchDescriptionFi);
            Assert.Equal("Biography En", createdDescription.ResearchDescriptionEn);
            Assert.Equal("Biografi Sv", createdDescription.ResearchDescriptionSv);
            Assert.Equal(TtvDataSourceId, createdDescription.DimRegisteredDataSourceId);

            // Verify new FactFieldValue was created
            var createdFfv = await context.FactFieldValues.FirstOrDefaultAsync(
                ffv => ffv.DimUserProfileId == UserProfileId && ffv.DimRegisteredDataSourceId == TtvDataSourceId);
            Assert.NotNull(createdFfv);
            Assert.False(createdFfv.Show);
            Assert.False(createdFfv.PrimaryValue);
        }

        [Fact(DisplayName = "CreateOrUpdateBiography updates existing biography when one exists")]
        public async Task CreateOrUpdateBiography_UpdatesBiography_WhenOneExists()
        {
            // Arrange
            using var context = CreateInMemoryContext(nameof(CreateOrUpdateBiography_UpdatesBiography_WhenOneExists));
            await SeedExistingBiographyAsync(context);
            var service = CreateService(context);

            var biography = new Biography { Fi = "Paivitetty Fi", En = "Updated En", Sv = "Uppdaterad Sv" };

            // Act
            bool result = await service.CreateOrUpdateBiography(UserProfileId, biography);

            // Assert
            Assert.True(result);

            // Verify the existing DimResearcherDescription was updated
            var updatedDescription = await context.DimResearcherDescriptions.FindAsync(1);
            Assert.NotNull(updatedDescription);
            Assert.Equal("Paivitetty Fi", updatedDescription.ResearchDescriptionFi);
            Assert.Equal("Updated En", updatedDescription.ResearchDescriptionEn);
            Assert.Equal("Uppdaterad Sv", updatedDescription.ResearchDescriptionSv);

            // Verify no new DimResearcherDescription was created
            var count = await context.DimResearcherDescriptions.CountAsync();
            Assert.Equal(1, count);
        }

        [Fact(DisplayName = "CreateOrUpdateBiography throws when user profile does not exist")]
        public async Task CreateOrUpdateBiography_Throws_WhenUserProfileDoesNotExist()
        {
            // Arrange
            using var context = CreateInMemoryContext(nameof(CreateOrUpdateBiography_Throws_WhenUserProfileDoesNotExist));
            var service = CreateService(context);

            var biography = new Biography { Fi = "Fi", En = "En", Sv = "Sv" };

            // Act & Assert
            // When the user profile does not exist, dimUserProfile will be null.
            // Accessing dimUserProfile.DimFieldDisplaySettings in the create path throws NullReferenceException.
            await Assert.ThrowsAsync<System.NullReferenceException>(
                () => service.CreateOrUpdateBiography(999, biography));
        }

        [Fact(DisplayName = "CreateOrUpdateBiography HTML-encodes malicious input when creating")]
        public async Task CreateOrUpdateBiography_HtmlEncodesMaliciousInput_OnCreate()
        {
            // Arrange
            using var context = CreateInMemoryContext(nameof(CreateOrUpdateBiography_HtmlEncodesMaliciousInput_OnCreate));
            await SeedUserProfileWithDisplaySettingAsync(context);
            var service = CreateService(context);

            var biography = new Biography
            {
                Fi = "<script>alert('xss')</script>Tutkija",
                En = "<b onclick=\"evil()\">Researcher</b>",
                Sv = "Text & <em>markup</em>"
            };

            // Act
            bool result = await service.CreateOrUpdateBiography(UserProfileId, biography);

            // Assert
            Assert.True(result);
            var created = await context.DimResearcherDescriptions.FirstOrDefaultAsync();
            Assert.NotNull(created);
            Assert.Equal("&lt;script&gt;alert(&#39;xss&#39;)&lt;/script&gt;Tutkija", created.ResearchDescriptionFi);
            Assert.Equal("&lt;b onclick=&quot;evil()&quot;&gt;Researcher&lt;/b&gt;", created.ResearchDescriptionEn);
            Assert.Equal("Text &amp; &lt;em&gt;markup&lt;/em&gt;", created.ResearchDescriptionSv);
        }

        [Fact(DisplayName = "CreateOrUpdateBiography HTML-encodes malicious input when updating")]
        public async Task CreateOrUpdateBiography_HtmlEncodesMaliciousInput_OnUpdate()
        {
            // Arrange
            using var context = CreateInMemoryContext(nameof(CreateOrUpdateBiography_HtmlEncodesMaliciousInput_OnUpdate));
            await SeedExistingBiographyAsync(context);
            var service = CreateService(context);

            var biography = new Biography
            {
                Fi = "<img src=x onerror=alert(1)>",
                En = "Safe text",
                Sv = null
            };

            // Act
            bool result = await service.CreateOrUpdateBiography(UserProfileId, biography);

            // Assert
            Assert.True(result);
            var updated = await context.DimResearcherDescriptions.FindAsync(1);
            Assert.NotNull(updated);
            Assert.Equal("&lt;img src=x onerror=alert(1)&gt;", updated.ResearchDescriptionFi);
            Assert.Equal("Safe text", updated.ResearchDescriptionEn);
            Assert.Null(updated.ResearchDescriptionSv);
        }

        [Fact(DisplayName = "CreateOrUpdateBiography preserves null fields without encoding")]
        public async Task CreateOrUpdateBiography_PreservesNullFields()
        {
            // Arrange
            using var context = CreateInMemoryContext(nameof(CreateOrUpdateBiography_PreservesNullFields));
            await SeedUserProfileWithDisplaySettingAsync(context);
            var service = CreateService(context);

            var biography = new Biography { Fi = null, En = null, Sv = null };

            // Act
            bool result = await service.CreateOrUpdateBiography(UserProfileId, biography);

            // Assert
            Assert.True(result);
            var created = await context.DimResearcherDescriptions.FirstOrDefaultAsync();
            Assert.NotNull(created);
            Assert.Null(created.ResearchDescriptionFi);
            Assert.Null(created.ResearchDescriptionEn);
            Assert.Null(created.ResearchDescriptionSv);
        }
    }
}
