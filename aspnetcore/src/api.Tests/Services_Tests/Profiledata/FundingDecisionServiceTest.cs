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
    [Collection("FundingDecisionService tests")]
    public class FundingDecisionServiceTests
    {
        private TtvContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TtvContext(options);
        }

        private FundingDecisionService CreateService(TtvContext context)
        {
            return new FundingDecisionService(
                ttvContext: context,
                languageService: new LanguageService(),
                logger: new NullLogger<FundingDecisionService>());
        }

        [Fact]
        public async Task GetProfileEditorFundingDecisions_ReturnsEmpty_WhenNoMatchingUserProfile()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorFundingDecisions_ReturnsEmpty_WhenNoMatchingUserProfile));
            var service = CreateService(context);
            var result = await service.GetProfileEditorFundingDecisions(userprofileId: 999);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProfileEditorFundingDecisions_ReturnsFundingDecisions_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorFundingDecisions_ReturnsFundingDecisions_WhenMatchingUserProfileExists));
            var testData = FundingDecisionServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorFundingDecisions(
                userprofileId: 1,
                forElasticsearch: false
            );

            Assert.NotEmpty(result);
            Assert.Equal(4, result.Count);

            // result[0] from dimFundingDecision1
            Assert.NotNull(result[0].itemMeta);
            Assert.Equal(123, result[0].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_FUNDING_DECISION, result[0].itemMeta.Type);
            Assert.True(result[0].itemMeta.Show);
            Assert.True(result[0].itemMeta.PrimaryValue);
            Assert.Single(result[0].DataSources);
            Assert.Equal(1, result[0].DataSources[0].Id);
            Assert.Equal("DataSource1", result[0].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[0].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[0].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[0].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[0].DataSources[0].Organization.SectorId);
            Assert.Equal(123, result[0].ProjectId);
            Assert.Equal("dimFundingDecision1 acronym", result[0].ProjectAcronym);
            Assert.Equal("dimFundingDecision1 name", result[0].ProjectNameFi);
            Assert.Equal("dimFundingDecision1 name", result[0].ProjectNameEn);
            Assert.Equal("dimFundingDecision1 name", result[0].ProjectNameSv);
            Assert.Equal("dimFundingDecision1 description", result[0].ProjectDescriptionFi);
            Assert.Equal("dimFundingDecision1 description", result[0].ProjectDescriptionEn);
            Assert.Equal("dimFundingDecision1 description", result[0].ProjectDescriptionSv);
            Assert.Equal("Org name Fi", result[0].FunderNameFi);
            Assert.Equal("Org name En", result[0].FunderNameEn);
            Assert.Equal("Org name Sv", result[0].FunderNameSv);
            Assert.Equal("dimFundingDecision1 funder project number", result[0].FunderProjectNumber);
            Assert.Equal("dimFundingDecision1 type of funding name", result[0].TypeOfFundingNameFi);
            Assert.Equal("dimFundingDecision1 type of funding name", result[0].TypeOfFundingNameEn);
            Assert.Equal("dimFundingDecision1 type of funding name", result[0].TypeOfFundingNameSv);
            Assert.Equal("dimFundingDecision1 call programme", result[0].CallProgrammeNameFi);
            Assert.Equal("dimFundingDecision1 call programme", result[0].CallProgrammeNameEn);
            Assert.Equal("dimFundingDecision1 call programme", result[0].CallProgrammeNameSv);
            Assert.Equal(2019, result[0].FundingStartYear);
            Assert.Equal(2020, result[0].FundingEndYear);
            Assert.Equal(123.45m, result[0].AmountInEur);
            Assert.Null(result[0].AmountInFundingDecisionCurrency);
            Assert.Null(result[0].FundingDecisionCurrencyAbbreviation);
            Assert.Equal("", result[0].Url);

            // result[1] from dimFundingDecision2
            Assert.NotNull(result[1].itemMeta);
            Assert.Equal(124, result[1].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_FUNDING_DECISION, result[1].itemMeta.Type);
            Assert.False(result[1].itemMeta.Show);
            Assert.False(result[1].itemMeta.PrimaryValue);
            Assert.Single(result[1].DataSources);
            Assert.Equal(2, result[1].DataSources[0].Id);
            Assert.Equal("DataSource2", result[1].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[1].DataSources[0].Organization.SectorId);
            Assert.Equal(124, result[1].ProjectId);
            Assert.Equal("dimFundingDecision2 acronym", result[1].ProjectAcronym);
            Assert.Equal("dimFundingDecision2 name fi", result[1].ProjectNameFi);
            Assert.Equal("dimFundingDecision2 name en", result[1].ProjectNameEn);
            Assert.Equal("dimFundingDecision2 name sv", result[1].ProjectNameSv);
            Assert.Equal("dimFundingDecision2 description fi", result[1].ProjectDescriptionFi);
            Assert.Equal("dimFundingDecision2 description en", result[1].ProjectDescriptionEn);
            Assert.Equal("dimFundingDecision2 description sv", result[1].ProjectDescriptionSv);
            Assert.Equal("Org name", result[1].FunderNameFi);
            Assert.Equal("Org name", result[1].FunderNameEn);
            Assert.Equal("Org name", result[1].FunderNameSv);
            Assert.Equal("dimFundingDecision2 funder project number", result[1].FunderProjectNumber);
            Assert.Equal("dimFundingDecision2 type of funding name fi", result[1].TypeOfFundingNameFi);
            Assert.Equal("dimFundingDecision2 type of funding name en", result[1].TypeOfFundingNameEn);
            Assert.Equal("dimFundingDecision2 type of funding name sv", result[1].TypeOfFundingNameSv);
            Assert.Equal("dimFundingDecision2 call programme name fi", result[1].CallProgrammeNameFi);
            Assert.Equal("dimFundingDecision2 call programme name en", result[1].CallProgrammeNameEn);
            Assert.Equal("dimFundingDecision2 call programme name sv", result[1].CallProgrammeNameSv);
            Assert.Equal(2020, result[1].FundingStartYear);
            Assert.Equal(2021, result[1].FundingEndYear);
            Assert.Equal(124.45m, result[1].AmountInEur);
            Assert.Null(result[1].AmountInFundingDecisionCurrency);
            Assert.Null(result[1].FundingDecisionCurrencyAbbreviation);
            Assert.Equal("", result[1].Url);


            // result[2] from dimProfileOnlyFundingDecision1
            Assert.NotNull(result[2].itemMeta);
            Assert.Equal(234, result[2].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_FUNDING_DECISION_PROFILE_ONLY, result[2].itemMeta.Type);
            Assert.True(result[2].itemMeta.Show);
            Assert.True(result[2].itemMeta.PrimaryValue);
            Assert.Single(result[2].DataSources);
            Assert.Equal(1, result[2].DataSources[0].Id);
            Assert.Equal("DataSource1", result[2].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[2].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[2].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[2].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[2].DataSources[0].Organization.SectorId);
            Assert.Equal(-1, result[2].ProjectId);
            Assert.Equal("dimProfileOnlyFundingDecision1 acronym", result[2].ProjectAcronym);
            Assert.Equal("dimProfileOnlyFundingDecision1 name", result[2].ProjectNameFi);
            Assert.Equal("dimProfileOnlyFundingDecision1 name", result[2].ProjectNameEn);
            Assert.Equal("dimProfileOnlyFundingDecision1 name", result[2].ProjectNameSv);
            Assert.Equal("dimProfileOnlyFundingDecision1 description", result[2].ProjectDescriptionFi);
            Assert.Equal("dimProfileOnlyFundingDecision1 description", result[2].ProjectDescriptionEn);
            Assert.Equal("dimProfileOnlyFundingDecision1 description", result[2].ProjectDescriptionSv);
            Assert.Equal("Org name Fi", result[2].FunderNameFi);
            Assert.Equal("Org name En", result[2].FunderNameEn);
            Assert.Equal("Org name Sv", result[2].FunderNameSv);
            Assert.Equal("dimProfileOnlyFundingDecision1 funder project number", result[2].FunderProjectNumber);
            Assert.Equal("dimProfileOnlyFundingDecision1 dimReferencedataActorRole name fi", result[2].TypeOfFundingNameFi);
            Assert.Equal("dimProfileOnlyFundingDecision1 dimReferencedataActorRole name en", result[2].TypeOfFundingNameEn);
            Assert.Equal("dimProfileOnlyFundingDecision1 dimReferencedataActorRole name sv", result[2].TypeOfFundingNameSv);
            Assert.Equal("", result[2].CallProgrammeNameFi);
            Assert.Equal("", result[2].CallProgrammeNameEn);
            Assert.Equal("", result[2].CallProgrammeNameSv);
            Assert.Equal(1995, result[2].FundingStartYear);
            Assert.Equal(1996, result[2].FundingEndYear);
            Assert.Equal(234.56m, result[2].AmountInEur);
            Assert.Equal(112233.44m, result[2].AmountInFundingDecisionCurrency);
            Assert.Equal("EUR", result[2].FundingDecisionCurrencyAbbreviation);
            Assert.Equal("https://example.com/profile_only_fundingdecision1", result[2].Url);

            // result[3] from dimProfileOnlyFundingDecision2
            Assert.NotNull(result[3].itemMeta);
            Assert.Equal(235, result[3].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_FUNDING_DECISION_PROFILE_ONLY, result[3].itemMeta.Type);
            Assert.False(result[3].itemMeta.Show);
            Assert.False(result[3].itemMeta.PrimaryValue);
            Assert.Single(result[3].DataSources);
            Assert.Equal(2, result[3].DataSources[0].Id);
            Assert.Equal("DataSource2", result[3].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[3].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[3].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[3].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[3].DataSources[0].Organization.SectorId);
            Assert.Equal(-1, result[3].ProjectId);
            Assert.Equal("dimProfileOnlyFundingDecision2 acronym", result[3].ProjectAcronym);
            Assert.Equal("dimProfileOnlyFundingDecision2 name fi", result[3].ProjectNameFi);
            Assert.Equal("dimProfileOnlyFundingDecision2 name en", result[3].ProjectNameEn);
            Assert.Equal("dimProfileOnlyFundingDecision2 name sv", result[3].ProjectNameSv);
            Assert.Equal("dimProfileOnlyFundingDecision2 description fi", result[3].ProjectDescriptionFi);
            Assert.Equal("dimProfileOnlyFundingDecision2 description en", result[3].ProjectDescriptionEn);
            Assert.Equal("dimProfileOnlyFundingDecision2 description sv", result[3].ProjectDescriptionSv);
            Assert.Equal("dimProfileOnlyFundingDecision2 DimIdentifierlessData value fi", result[3].FunderNameFi);
            Assert.Equal("dimProfileOnlyFundingDecision2 DimIdentifierlessData value en", result[3].FunderNameEn);
            Assert.Equal("dimProfileOnlyFundingDecision2 DimIdentifierlessData value sv", result[3].FunderNameSv);
            Assert.Equal("dimProfileOnlyFundingDecision2 funder project number", result[3].FunderProjectNumber);
            Assert.Equal("dimProfileOnlyFundingDecision2 dimReferencedataActorRole name", result[3].TypeOfFundingNameFi);
            Assert.Equal("dimProfileOnlyFundingDecision2 dimReferencedataActorRole name", result[3].TypeOfFundingNameEn);
            Assert.Equal("dimProfileOnlyFundingDecision2 dimReferencedataActorRole name", result[3].TypeOfFundingNameSv);
            Assert.Equal("", result[3].CallProgrammeNameFi);
            Assert.Equal("", result[3].CallProgrammeNameEn);
            Assert.Equal("", result[3].CallProgrammeNameSv);
            Assert.Equal(2001, result[3].FundingStartYear);
            Assert.Equal(2002, result[3].FundingEndYear);
            Assert.Equal(345.67m, result[3].AmountInEur);
            Assert.Equal(223344.55m, result[3].AmountInFundingDecisionCurrency);
            Assert.Equal("USD", result[3].FundingDecisionCurrencyAbbreviation);
            Assert.Equal("https://example.com/profile_only_fundingdecision2", result[3].Url);
        }

        [Fact]
        public async Task GetProfileEditorFundingDecisions_ReturnsFundingDecisions_ForElasticsearch_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorFundingDecisions_ReturnsFundingDecisions_ForElasticsearch_WhenMatchingUserProfileExists));
            var testData = FundingDecisionServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorFundingDecisions(
                userprofileId: 1,
                forElasticsearch: true
            );
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
        }
    }
}