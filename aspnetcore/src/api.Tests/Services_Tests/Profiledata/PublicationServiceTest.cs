using Xunit;
using api.Services;
using api.Services.Profiledata;
using Microsoft.EntityFrameworkCore;
using api.Models.Ttv;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;

namespace api.Tests.Profiledata
{
    [Collection("PublicationService tests")]
    public class PublicationServiceTests
    {
        private TtvContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TtvContext(options);
        }

        private PublicationService CreateService(TtvContext context)
        {
            return new PublicationService(
                ttvContext: context,
                languageService: new LanguageService(),
                duplicateHandlerService: new DuplicateHandlerService(),
                logger: new NullLogger<PublicationService>());
        }

        [Fact]
        public async Task GetProfileEditorPublications_ReturnsEmpty_WhenNoMatchingUserProfile()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorPublications_ReturnsEmpty_WhenNoMatchingUserProfile));
            var service = CreateService(context);
            var result = await service.GetProfileEditorPublications(userprofileId: 999);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProfileEditorPublications_ReturnsPublications_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorPublications_ReturnsPublications_WhenMatchingUserProfileExists));
            var testData = PublicationServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorPublications(
                userprofileId: 1,
                forElasticsearch: false
            );

            Assert.NotEmpty(result);
            // Expected count is 4.
            // Out of 3 DimPublications 2 of them should be deduplicated based on the same PublicationId. That leaves 2 DimPublications.
            // Out of 3 DimProfileOnlyPublications 1 should be deduplicated with one of the DimPublications base on the same Doi. That leaves 2 DimProfileOnlyPublications.
            // Combined count of deduplicated DimPublications and deduplicated DimProfileOnlyPublications should be 4.
            Assert.Equal(4, result.Count);

            Assert.Equal("DimPublication1 Article number text", result[0].ArticleNumberText);
            Assert.Equal("DimPublication1 Authors text", result[0].AuthorsText);
            Assert.Equal("DimPublication1 Conference name", result[0].ConferenceName);
            Assert.Equal("10.1234/doi_dimpublication_1", result[0].Doi);
            Assert.Equal("DimPublication1 Issue number", result[0].IssueNumber);
            Assert.Equal("DimPublication1 Journal name", result[0].JournalName);
            Assert.Equal(1, result[0].OpenAccess);
            Assert.Equal("123", result[0].PageNumberText);
            Assert.Equal("DimPublication1 Parent publication name", result[0].ParentPublicationName);
            Assert.Equal("DimPublication PublicationId to deduplicate", result[0].PublicationId);
            Assert.Equal("DimPublication1 Publication name", result[0].PublicationName);
            //Assert.Equal("A1", result[0].PublicationTypeCode);
            Assert.Equal(2020, result[0].PublicationYear);
            Assert.Equal("DimPublication1 Publisher name", result[0].PublisherName);
            Assert.Equal("https://example.com/selfarchivedurl1", result[0].SelfArchivedAddress);
            Assert.Equal("1", result[0].SelfArchivedCode);
            Assert.Equal("DimPublication1 Volume number", result[0].Volume);
            // // Item meta
            // Assert.Equal(1, result[0].itemMeta.Id);
            // Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_PUBLICATION, result[0].itemMeta.Type);
            // Assert.True(result[0].itemMeta.Show);
            // Assert.True(result[0].itemMeta.PrimaryValue);
            // // Data sources
            // Assert.Equal(2, result[0].DataSources.Count); // After deduplication by PublicationId, the publication should have combined data sources of the deduplicated publications.
            // Assert.Equal(1, result[0].DataSources[0].Id);
            // Assert.Equal("DataSource1", result[0].DataSources[0].RegisteredDataSource);
            // Assert.Equal("Org name Fi", result[0].DataSources[0].Organization.NameFi);
            // Assert.Equal("Org name En", result[0].DataSources[0].Organization.NameEn);
            // Assert.Equal("Org name Sv", result[0].DataSources[0].Organization.NameSv);
            // Assert.Equal("S1", result[0].DataSources[0].Organization.SectorId);
            // Assert.Single(result[0].DataSources);
            // Assert.Equal(1, result[0].DataSources[0].Id);
            // Assert.Equal("DataSource1", result[0].DataSources[0].RegisteredDataSource);
            // Assert.Equal("Org name Fi", result[0].DataSources[0].Organization.NameFi);
            // Assert.Equal("Org name En", result[0].DataSources[0].Organization.NameEn);
            // Assert.Equal("Org name Sv", result[0].DataSources[0].Organization.NameSv);
            // Assert.Equal("S1", result[0].DataSources[0].Organization.SectorId);

            Assert.Equal("DimPublication3 Article number text", result[1].ArticleNumberText);
            Assert.Equal("DimPublication3 Authors text", result[1].AuthorsText);
            Assert.Equal("DimPublication3 Conference name", result[1].ConferenceName);
            Assert.Equal("10.1234/doi_to_deduplicate", result[1].Doi);
            Assert.Equal("DimPublication3 Issue number", result[1].IssueNumber);
            Assert.Equal("DimPublication3 Journal name", result[1].JournalName);
            Assert.Equal(0, result[1].OpenAccess);
            Assert.Equal("345", result[1].PageNumberText);
            Assert.Equal("DimPublication3 Parent publication name", result[1].ParentPublicationName);
            Assert.Equal("DimPublication3 PublicationId", result[1].PublicationId);
            Assert.Equal("DimPublication3 Publication name", result[1].PublicationName);
            Assert.Equal("", result[1].PublicationTypeCode);
            Assert.Equal(2022, result[1].PublicationYear);
            Assert.Equal("DimPublication3 Publisher name", result[1].PublisherName);
            Assert.Equal("", result[1].SelfArchivedAddress);
            Assert.Equal("0", result[1].SelfArchivedCode);
            Assert.Equal("DimPublication3 Volume number", result[1].Volume);
            Assert.Equal(2, result[1].DataSources.Count); // After deduplication by Doi, the publication should have combined data sources of the deduplicated publications.

            Assert.Equal("DimProfileOnlyPublication2 Article number text", result[2].ArticleNumberText);
            Assert.Equal("DimProfileOnlyPublication2 Authors text", result[2].AuthorsText);
            Assert.Equal("DimProfileOnlyPublication2 Conference name", result[2].ConferenceName);
            Assert.Equal("10.1234/doi_dimprofileonlypublication_2", result[2].Doi);
            Assert.Equal("DimProfileOnlyPublication2 Issue number", result[2].IssueNumber);
            Assert.Equal("", result[2].JournalName);
            Assert.Equal(1, result[2].OpenAccess);
            Assert.Equal("567", result[2].PageNumberText);
            Assert.Equal("DimProfileOnlyPublication2 Parent publication name", result[2].ParentPublicationName);
            Assert.Equal("DimProfileOnlyPublication2 PublicationId", result[2].PublicationId);
            Assert.Equal("DimProfileOnlyPublication2 Publication name", result[2].PublicationName);
            Assert.Equal("", result[2].PublicationTypeCode);
            Assert.Equal(2024, result[2].PublicationYear);
            Assert.Equal("DimProfileOnlyPublication2 Publisher name", result[2].PublisherName);
            Assert.Equal("", result[2].SelfArchivedAddress);
            Assert.Equal("0", result[2].SelfArchivedCode);
            Assert.Equal("DimProfileOnlyPublication2 Volume number", result[2].Volume);
            Assert.Single(result[2].DataSources);

            Assert.Equal("DimProfileOnlyPublication3 Article number text", result[3].ArticleNumberText);
            Assert.Equal("DimProfileOnlyPublication3 Authors text", result[3].AuthorsText);
            Assert.Equal("DimProfileOnlyPublication3 Conference name", result[3].ConferenceName);
            Assert.Equal("", result[3].Doi);
            Assert.Equal("DimProfileOnlyPublication3 Issue number", result[3].IssueNumber);
            Assert.Equal("", result[3].JournalName);
            Assert.Equal(9, result[3].OpenAccess);
            Assert.Equal("678", result[3].PageNumberText);
            Assert.Equal("DimProfileOnlyPublication3 Parent publication name", result[3].ParentPublicationName);
            Assert.Equal("", result[3].PublicationId);
            Assert.Equal( "DimProfileOnlyPublication3 Publication name", result[3].PublicationName);
            Assert.Equal("", result[3].PublicationTypeCode);
            Assert.Equal(2025, result[3].PublicationYear);
            Assert.Equal("DimProfileOnlyPublication3 Publisher name", result[3].PublisherName);
            Assert.Equal("DimProfileOnlyPublication3 Volume number", result[3].Volume);
            Assert.Equal("", result[3].SelfArchivedAddress);
            Assert.Equal("0", result[3].SelfArchivedCode);
            Assert.Single(result[3].DataSources);


            // When forElasticsearch is true, only publications with Show = true should be returned
            result = await service.GetProfileEditorPublications(
                userprofileId: 1,
                forElasticsearch: true
            );
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
        }
    }
}