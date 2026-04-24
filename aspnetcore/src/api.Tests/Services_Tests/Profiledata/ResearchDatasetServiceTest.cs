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
    [Collection("ResearchDatasetService tests")]
    public class ResearchDatasetServiceTests
    {
        private TtvContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TtvContext(options);
        }

        private ResearchDatasetService CreateService(TtvContext context)
        {
            return new ResearchDatasetService(
                ttvContext: context,
                languageService: new LanguageService(),
                logger: new NullLogger<ResearchDatasetService>());
        }

        [Fact]
        public async Task GetProfileEditorResearchDatasets_ReturnsEmpty_WhenNoMatchingUserProfile()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorResearchDatasets_ReturnsEmpty_WhenNoMatchingUserProfile));
            var service = CreateService(context);

            var result = await service.GetProfileEditorResearchDatasets(userprofileId: 999);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProfileEditorResearchDatasets_ReturnsResearchDatasets_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorResearchDatasets_ReturnsResearchDatasets_WhenMatchingUserProfileExists));
            var testData = ResearchDatasetServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorResearchDatasets(
                userprofileId: 1,
                forElasticsearch: false
            );

            Assert.NotEmpty(result);
            Assert.Equal(4, result.Count);

            // result[0] from dimResearchDataset1
            Assert.NotNull(result[0].itemMeta);
            Assert.Equal(123, result[0].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_RESEARCH_DATASET, result[0].itemMeta.Type);
            Assert.True(result[0].itemMeta.Show);
            Assert.True(result[0].itemMeta.PrimaryValue);
            Assert.Single(result[0].DataSources);
            Assert.Equal(1, result[0].DataSources[0].Id);
            Assert.Equal("DataSource1", result[0].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[0].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[0].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[0].DataSources[0].Organization.NameSv);
            Assert.Equal("dimResearchDataset1 DimReferencedataAvailability CodeValue", result[0].AccessType);
            Assert.Equal(1999, result[0].DatasetCreated);
            Assert.Equal("dimResearchDataset1 name fi", result[0].NameFi);
            Assert.Equal("dimResearchDataset1 name en", result[0].NameEn);
            Assert.Equal("dimResearchDataset1 name sv", result[0].NameSv);
            Assert.Equal("dimResearchDataset1 description fi", result[0].DescriptionFi);
            Assert.Equal("dimResearchDataset1 description en", result[0].DescriptionEn);
            Assert.Equal("dimResearchDataset1 description sv", result[0].DescriptionSv);
            Assert.Equal("https://etsin.fairdata.fi/dataset/dimResearchDataset1 LocalIdentifier", result[0].FairdataUrl);
            Assert.Equal("dimResearchDataset1 LocalIdentifier", result[0].Identifier);
            Assert.NotNull(result[0].PreferredIdentifiers);
            Assert.Equal(2, result[0].PreferredIdentifiers.Count);
            Assert.Equal("dimResearchDataset1 DimPid1 type", result[0].PreferredIdentifiers[0].PidType);
            Assert.Equal("dimResearchDataset1 DimPid1 content", result[0].PreferredIdentifiers[0].PidContent);
            Assert.Equal("dimResearchDataset1 DimPid2 type", result[0].PreferredIdentifiers[1].PidType);
            Assert.Equal("dimResearchDataset1 DimPid2 content", result[0].PreferredIdentifiers[1].PidContent);
            Assert.Equal("", result[0].Url);

            // result[1] from dimResearchDataset2
            Assert.NotNull(result[1].itemMeta);
            Assert.Equal(124, result[1].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_RESEARCH_DATASET, result[1].itemMeta.Type);
            Assert.True(result[1].itemMeta.Show);
            Assert.True(result[1].itemMeta.PrimaryValue);
            Assert.Single(result[1].DataSources);
            Assert.Equal(2, result[1].DataSources[0].Id);
            Assert.Equal("DataSource2", result[1].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameSv);
            Assert.Equal("dimResearchDataset2 DimReferencedataAvailability CodeValue", result[1].AccessType);
            Assert.Null(result[1].DatasetCreated);
            Assert.Equal("dimResearchDataset2 name", result[1].NameFi);
            Assert.Equal("dimResearchDataset2 name", result[1].NameEn);
            Assert.Equal("dimResearchDataset2 name", result[1].NameSv);
            Assert.Equal("dimResearchDataset2 description", result[1].DescriptionFi);
            Assert.Equal("dimResearchDataset2 description", result[1].DescriptionEn);
            Assert.Equal("dimResearchDataset2 description", result[1].DescriptionSv);
            Assert.Equal("https://etsin.fairdata.fi/dataset/dimResearchDataset2 LocalIdentifier", result[1].FairdataUrl);
            Assert.Equal("dimResearchDataset2 LocalIdentifier", result[1].Identifier);
            Assert.Empty(result[1].PreferredIdentifiers);
            Assert.Equal("", result[0].Url);

            // result[2] from dimProfileOnlyDataset1
            Assert.NotNull(result[2].itemMeta);
            Assert.Equal(234, result[2].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_RESEARCH_DATASET_PROFILE_ONLY, result[2].itemMeta.Type);
            Assert.True(result[2].itemMeta.Show);
            Assert.True(result[2].itemMeta.PrimaryValue);
            Assert.Single(result[2].DataSources);
            Assert.Equal(1, result[2].DataSources[0].Id);
            Assert.Equal("DataSource1", result[2].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[2].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[2].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[2].DataSources[0].Organization.NameSv);
            Assert.Equal(2005, result[2].DatasetCreated);
            Assert.Equal("dimProfileOnlyDataset1 name fi", result[2].NameFi);
            Assert.Equal("dimProfileOnlyDataset1 name en", result[2].NameEn);
            Assert.Equal("dimProfileOnlyDataset1 name sv", result[2].NameSv);
            Assert.Equal("dimProfileOnlyDataset1 description fi", result[2].DescriptionFi);
            Assert.Equal("dimProfileOnlyDataset1 description en", result[2].DescriptionEn);
            Assert.Equal("dimProfileOnlyDataset1 description sv", result[2].DescriptionSv);
            Assert.Equal("", result[2].FairdataUrl);
            Assert.Equal("dimProfileOnlyDataset1 LocalIdentifier", result[2].Identifier);
            Assert.Empty(result[2].PreferredIdentifiers);
            Assert.Equal("https://example.com/dimProfileOnlyDataset1", result[2].Url);

            // result[3] from dimProfileOnlyDataset2
            Assert.NotNull(result[3].itemMeta);
            Assert.Equal(235, result[3].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_RESEARCH_DATASET_PROFILE_ONLY, result[3].itemMeta.Type);
            Assert.False(result[3].itemMeta.Show);
            Assert.False(result[3].itemMeta.PrimaryValue);
            Assert.Single(result[3].DataSources);
            Assert.Equal(2, result[3].DataSources[0].Id);
            Assert.Equal("DataSource2", result[3].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[3].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[3].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[3].DataSources[0].Organization.NameSv);
            Assert.Null(result[3].DatasetCreated);
            Assert.Equal("dimProfileOnlyDataset2 name", result[3].NameFi);
            Assert.Equal("dimProfileOnlyDataset2 name", result[3].NameEn);
            Assert.Equal("dimProfileOnlyDataset2 name", result[3].NameSv);
            Assert.Equal("dimProfileOnlyDataset2 description", result[3].DescriptionFi);
            Assert.Equal("dimProfileOnlyDataset2 description", result[3].DescriptionEn);
            Assert.Equal("dimProfileOnlyDataset2 description", result[3].DescriptionSv);
            Assert.Equal("", result[3].FairdataUrl);
            Assert.Equal("dimProfileOnlyDataset2 LocalIdentifier", result[3].Identifier);
            Assert.Empty(result[3].PreferredIdentifiers);
            Assert.Equal("", result[3].Url);
        }
    }
}