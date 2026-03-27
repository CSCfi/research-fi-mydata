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
    [Collection("AffiliationService tests")]
    public class AffiliationServiceTests
    {
        private TtvContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TtvContext(options);
        }

        private AffiliationService CreateService(TtvContext context)
        {
            return new AffiliationService(
                ttvContext: context,
                languageService: new LanguageService(),
                utilityService: new UtilityService(),
                logger: new NullLogger<AffiliationService>());
        }

        [Fact]
        public async Task GetProfileEditorAffiliations_ReturnsEmpty_WhenNoMatchingUserProfile()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorAffiliations_ReturnsEmpty_WhenNoMatchingUserProfile));
            var service = CreateService(context);
            var result = await service.GetProfileEditorAffiliations(userprofileId: 999);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProfileEditorAffiliations_ReturnsAffiliations_WhenMatchingUserProfileExists()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorAffiliations_ReturnsAffiliations_WhenMatchingUserProfileExists));
            var testData = AffiliationServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorAffiliations(
                userprofileId: 1,
                forElasticsearch: false // Property "sector" should be empty in the result when forElasticsearch is false.
            );

            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("Affiliation 1 organization broader name Fi", result[0].OrganizationNameFi);
            Assert.Equal("Affiliation 1 organization broader name En", result[0].OrganizationNameEn);
            Assert.Equal("Affiliation 1 organization broader name Sv", result[0].OrganizationNameSv);
            Assert.Equal("Affiliation 1 organization name Fi", result[0].DepartmentNameFi);
            Assert.Equal("Affiliation 1 organization name En", result[0].DepartmentNameEn);
            Assert.Equal("Affiliation 1 organization name Sv", result[0].DepartmentNameSv);
            Assert.Equal("Affiliation 1 position name fi", result[0].PositionNameFi);
            Assert.Equal("Affiliation 1 position name en", result[0].PositionNameEn);
            Assert.Equal("Affiliation 1 position name sv", result[0].PositionNameSv);
            Assert.Equal("Affiliation 1 type Fi", result[0].AffiliationTypeFi);
            Assert.Equal("Affiliation 1 type En", result[0].AffiliationTypeEn);
            Assert.Equal("Affiliation 1 type Sv", result[0].AffiliationTypeSv);
            Assert.Equal(2021, result[0].StartDate.Year);
            Assert.Equal(2, result[0].StartDate.Month);
            Assert.Equal(1, result[0].StartDate.Day);
            Assert.Equal(2023, result[0].EndDate.Year);
            Assert.Equal(1, result[0].EndDate.Month);
            Assert.Equal(31, result[0].EndDate.Day);
            Assert.Empty(result[0].sector);
            Assert.Equal(1000, result[0].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_AFFILIATION, result[0].itemMeta.Type);
            Assert.True(result[0].itemMeta.Show);
            Assert.True(result[0].itemMeta.PrimaryValue);
            Assert.Single(result[0].DataSources);
            Assert.Equal(1, result[0].DataSources[0].Id);
            Assert.Equal("DataSource1", result[0].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[0].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[0].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[0].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[0].DataSources[0].Organization.SectorId);
    
            Assert.Equal("Affiliation 2 organization name Fi", result[1].OrganizationNameFi);
            Assert.Equal("Affiliation 2 organization name En", result[1].OrganizationNameEn);
            Assert.Equal("Affiliation 2 organization name Sv", result[1].OrganizationNameSv);
            Assert.Equal("", result[1].DepartmentNameFi);
            Assert.Equal("", result[1].DepartmentNameEn);
            Assert.Equal("", result[1].DepartmentNameSv);    
            Assert.Equal("Affiliation 2 position name en", result[1].PositionNameFi);
            Assert.Equal("Affiliation 2 position name en", result[1].PositionNameEn);
            Assert.Equal("Affiliation 2 position name en", result[1].PositionNameSv);
            Assert.Equal("Affiliation 2 type En", result[1].AffiliationTypeFi);
            Assert.Equal("Affiliation 2 type En", result[1].AffiliationTypeEn);
            Assert.Equal("Affiliation 2 type En", result[1].AffiliationTypeSv);
            Assert.Equal(2020, result[1].StartDate.Year);
            Assert.Equal(3, result[1].StartDate.Month);
            Assert.Equal(13, result[1].StartDate.Day);
            Assert.Equal(2022, result[1].EndDate.Year);
            Assert.Equal(2, result[1].EndDate.Month);
            Assert.Equal(30, result[1].EndDate.Day);
            Assert.Empty(result[1].sector);
            Assert.Equal(1001, result[1].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_AFFILIATION, result[1].itemMeta.Type);
            Assert.False(result[1].itemMeta.Show);
            Assert.False(result[1].itemMeta.PrimaryValue);
            Assert.Single(result[1].DataSources);
            Assert.Equal(2, result[1].DataSources[0].Id);
            Assert.Equal("DataSource2", result[1].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[1].DataSources[0].Organization.SectorId);

            Assert.Equal("Affiliation 3 identifierless data value Fi", result[2].OrganizationNameFi);
            Assert.Equal("Affiliation 3 identifierless data value En", result[2].OrganizationNameEn);
            Assert.Equal("Affiliation 3 identifierless data value Sv", result[2].OrganizationNameSv);
            Assert.Equal("Affiliation 3 identifierless data child value Fi", result[2].DepartmentNameFi);
            Assert.Equal("Affiliation 3 identifierless data child value En", result[2].DepartmentNameEn);
            Assert.Equal("Affiliation 3 identifierless data child value Sv", result[2].DepartmentNameSv);
            Assert.Equal("Affiliation 3 position name fi", result[2].PositionNameFi);
            Assert.Equal("Affiliation 3 position name en", result[2].PositionNameEn);
            Assert.Equal("Affiliation 3 position name sv", result[2].PositionNameSv);
            Assert.Equal("Affiliation 3 type Fi", result[2].AffiliationTypeFi);
            Assert.Equal("Affiliation 3 type En", result[2].AffiliationTypeEn);
            Assert.Equal("Affiliation 3 type Sv", result[2].AffiliationTypeSv);
            Assert.Equal(2020, result[2].StartDate.Year);
            Assert.Equal(3, result[2].StartDate.Month);
            Assert.Equal(13, result[2].StartDate.Day);
            Assert.Equal(2022, result[2].EndDate.Year);
            Assert.Equal(2, result[2].EndDate.Month);
            Assert.Equal(30, result[2].EndDate.Day);
            Assert.Empty(result[2].sector);
            Assert.Equal(1002, result[2].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_AFFILIATION, result[2].itemMeta.Type);
            Assert.False(result[2].itemMeta.Show);
            Assert.False(result[2].itemMeta.PrimaryValue);
            Assert.Single(result[2].DataSources);
            Assert.Equal(2, result[2].DataSources[0].Id);
            Assert.Equal("DataSource2", result[2].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[2].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[2].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[2].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[2].DataSources[0].Organization.SectorId);

            // When forElasticsearch is true, only the name with Show = true should be returned
            result = await service.GetProfileEditorAffiliations(
                userprofileId: 1,
                forElasticsearch: true
            );
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetProfileEditorAffiliations_ReturnsAffiliations_WithSectors_WhenForElasticsearchIsTrue()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileEditorAffiliations_ReturnsAffiliations_WithSectors_WhenForElasticsearchIsTrue));
            var testData = AffiliationServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var result = await service.GetProfileEditorAffiliations(
                userprofileId: 1,
                forElasticsearch: true // Property "sector" should be populated in the result when forElasticsearch is true.
            );

            Assert.NotEmpty(result);
            Assert.Single(result);

            Assert.NotEmpty(result[0].sector);
            Assert.Single(result[0].sector);
            Assert.Equal("S2", result[0].sector[0].sectorId);
            Assert.Equal("Sector 3 Fi", result[0].sector[0].nameFiSector);
            Assert.Equal("Sector 3 En", result[0].sector[0].nameEnSector);
            Assert.Equal("Sector 3 Sv", result[0].sector[0].nameSvSector);
            Assert.NotEmpty(result[0].sector[0].organization);
            Assert.Single(result[0].sector[0].organization);
            Assert.Equal("Affiliation 1 organization organizationId", result[0].sector[0].organization[0].organizationId);
            Assert.Equal("Affiliation 1 organization broader name Fi", result[0].sector[0].organization[0].OrganizationNameFi);
            Assert.Equal("Affiliation 1 organization broader name En", result[0].sector[0].organization[0].OrganizationNameEn);
            Assert.Equal("Affiliation 1 organization broader name Sv", result[0].sector[0].organization[0].OrganizationNameSv);
        }
    }
}