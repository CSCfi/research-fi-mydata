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
            Assert.Equal(6, result.Count);

            // result[0] from DimResearchActivity1
            Assert.Equal("ResearchActivity to deduplicate name fi", result[0].NameFi);
            Assert.Equal("ResearchActivity to deduplicate name en", result[0].NameEn);
            Assert.Equal("ResearchActivity to deduplicate name sv", result[0].NameSv);
            Assert.Equal("DimResearchActivity1 description fi", result[0].DescriptionFi);
            Assert.Equal("DimResearchActivity1 description en", result[0].DescriptionEn);
            Assert.Equal("DimResearchActivity1 description sv", result[0].DescriptionSv);
            Assert.Equal("DimResearchActivity1 organization broader name fi", result[0].OrganizationNameFi);
            Assert.Equal("DimResearchActivity1 organization broader name en", result[0].OrganizationNameEn);
            Assert.Equal("DimResearchActivity1 organization broader name sv", result[0].OrganizationNameSv);
            Assert.Equal("DimResearchActivity1 organization name fi", result[0].DepartmentNameFi);
            Assert.Equal("DimResearchActivity1 organization name en", result[0].DepartmentNameEn);
            Assert.Equal("DimResearchActivity1 organization name sv", result[0].DepartmentNameSv);
            Assert.Equal("DimResearchActivity1 FactContribution activity_type DimReferenceData CodeValue", result[0].ActivityTypeCode);
            Assert.Equal("DimResearchActivity1 FactContribution activity_type DimReferenceData NameFi", result[0].ActivityTypeNameFi);
            Assert.Equal("DimResearchActivity1 FactContribution activity_type DimReferenceData NameEn", result[0].ActivityTypeNameEn);
            Assert.Equal("DimResearchActivity1 FactContribution activity_type DimReferenceData NameSv", result[0].ActivityTypeNameSv);
            Assert.Equal("DimResearchActivity1 FactContribution researcher_name_activity DimReferenceData CodeValue", result[0].RoleCode);
            Assert.Equal("DimResearchActivity1 FactContribution researcher_name_activity DimReferenceData NameFi", result[0].RoleNameFi);
            Assert.Equal("DimResearchActivity1 FactContribution researcher_name_activity DimReferenceData NameEn", result[0].RoleNameEn);
            Assert.Equal("DimResearchActivity1 FactContribution researcher_name_activity DimReferenceData NameSv", result[0].RoleNameSv);
            Assert.Equal(2019, result[0].StartDate.Year);
            Assert.Equal(1, result[0].StartDate.Month);
            Assert.Equal(11, result[0].StartDate.Day);
            Assert.Equal(2020, result[0].EndDate.Year);
            Assert.Equal(2, result[0].EndDate.Month);
            Assert.Equal(22, result[0].EndDate.Day);
            Assert.Empty(result[0].sector);
            Assert.NotNull(result[0].itemMeta);
            Assert.Equal(100, result[0].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_RESEARCH_ACTIVITY, result[0].itemMeta.Type);
            Assert.True(result[0].itemMeta.Show);
            Assert.True(result[0].itemMeta.PrimaryValue);
            Assert.Single(result[0].DataSources);
            Assert.Equal(1, result[0].DataSources[0].Id);
            Assert.Equal("DataSource1", result[0].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[0].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[0].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[0].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[0].DataSources[0].Organization.SectorId);
            Assert.Equal("https://example.com/dim-research-activity-1-wl1", result[0].Url);
            Assert.Equal(2, result[0].WebLinks.Count);
            Assert.Equal("https://example.com/dim-research-activity-1-wl1", result[0].WebLinks[0].Url);
            Assert.Equal("DimResearchActivity1 Web Link 1", result[0].WebLinks[0].LinkLabel);
            Assert.Equal("ProfileEditorWebLink", result[0].WebLinks[0].LinkType);
            Assert.Equal("https://example.com/dim-research-activity-1-wl2", result[0].WebLinks[1].Url);
            Assert.Equal("DimResearchActivity1 Web Link 2", result[0].WebLinks[1].LinkLabel);
            Assert.Equal("ProfileEditorWebLink", result[0].WebLinks[1].LinkType);

            // result[1] from DimResearchActivity2
            Assert.Equal("DimResearchActivity2 name", result[1].NameFi);
            Assert.Equal("DimResearchActivity2 name", result[1].NameEn);
            Assert.Equal("DimResearchActivity2 name", result[1].NameSv);
            Assert.Equal("DimResearchActivity2 description", result[1].DescriptionFi);
            Assert.Equal("DimResearchActivity2 description", result[1].DescriptionEn);
            Assert.Equal("DimResearchActivity2 description", result[1].DescriptionSv);
            Assert.Equal("DimResearchActivity2 organization name", result[1].OrganizationNameFi);
            Assert.Equal("DimResearchActivity2 organization name", result[1].OrganizationNameEn);
            Assert.Equal("DimResearchActivity2 organization name", result[1].OrganizationNameSv);
            Assert.Equal("", result[1].DepartmentNameFi);
            Assert.Equal("", result[1].DepartmentNameEn);
            Assert.Equal("", result[1].DepartmentNameSv);
            Assert.Equal("DimResearchActivity2 FactContribution activity_type DimReferenceData CodeValue", result[1].ActivityTypeCode);
            Assert.Equal("DimResearchActivity2 FactContribution activity_type DimReferenceData NameFi", result[1].ActivityTypeNameFi);
            Assert.Equal("DimResearchActivity2 FactContribution activity_type DimReferenceData NameEn", result[1].ActivityTypeNameEn);
            Assert.Equal("DimResearchActivity2 FactContribution activity_type DimReferenceData NameSv", result[1].ActivityTypeNameSv);
            Assert.Equal("DimResearchActivity2 FactContribution researcher_name_activity DimReferenceData CodeValue", result[1].RoleCode);
            Assert.Equal("DimResearchActivity2 FactContribution researcher_name_activity DimReferenceData NameFi", result[1].RoleNameFi);
            Assert.Equal("DimResearchActivity2 FactContribution researcher_name_activity DimReferenceData NameEn", result[1].RoleNameEn);
            Assert.Equal("DimResearchActivity2 FactContribution researcher_name_activity DimReferenceData NameSv", result[1].RoleNameSv);
            Assert.Equal(2021, result[1].StartDate.Year);
            Assert.Equal(2, result[1].StartDate.Month);
            Assert.Equal(12, result[1].StartDate.Day);
            Assert.Equal(2022, result[1].EndDate.Year);
            Assert.Equal(3, result[1].EndDate.Month);
            Assert.Equal(23, result[1].EndDate.Day);
            Assert.Empty(result[1].sector);
            Assert.NotNull(result[1].itemMeta);
            Assert.Equal(200, result[1].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_RESEARCH_ACTIVITY, result[1].itemMeta.Type);
            Assert.False(result[1].itemMeta.Show);
            Assert.False(result[1].itemMeta.PrimaryValue);
            Assert.Single(result[1].DataSources);
            Assert.Equal(2, result[1].DataSources[0].Id);
            Assert.Equal("DataSource2", result[1].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[1].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[1].DataSources[0].Organization.SectorId);

            // result[2] from DimresearchActivity3
            Assert.Equal("DimResearchActivity3 name fi", result[2].NameFi);
            Assert.Equal("DimResearchActivity3 name en", result[2].NameEn);
            Assert.Equal("DimResearchActivity3 name sv", result[2].NameSv);
            Assert.Equal("DimResearchActivity3 description fi", result[2].DescriptionFi);
            Assert.Equal("DimResearchActivity3 description en", result[2].DescriptionEn);
            Assert.Equal("DimResearchActivity3 description sv", result[2].DescriptionSv);
            Assert.Equal("DimResearchActivity3 identifierless data value fi", result[2].OrganizationNameFi);
            Assert.Equal("DimResearchActivity3 identifierless data value en", result[2].OrganizationNameEn);
            Assert.Equal("DimResearchActivity3 identifierless data value sv", result[2].OrganizationNameSv);
            Assert.Equal("DimResearchActivity3 identifierless data child value fi", result[2].DepartmentNameFi);
            Assert.Equal("DimResearchActivity3 identifierless data child value en", result[2].DepartmentNameEn);
            Assert.Equal("DimResearchActivity3 identifierless data child value sv", result[2].DepartmentNameSv);
            Assert.Equal("DimResearchActivity3 FactContribution activity_type DimReferenceData CodeValue", result[2].ActivityTypeCode);
            Assert.Equal("DimResearchActivity3 FactContribution activity_type DimReferenceData", result[2].ActivityTypeNameFi);
            Assert.Equal("DimResearchActivity3 FactContribution activity_type DimReferenceData", result[2].ActivityTypeNameEn);
            Assert.Equal("DimResearchActivity3 FactContribution activity_type DimReferenceData", result[2].ActivityTypeNameSv);
            Assert.Equal("DimResearchActivity3 FactContribution researcher_name_activity DimReferenceData CodeValue", result[2].RoleCode);
            Assert.Equal("DimResearchActivity3 FactContribution researcher_name_activity DimReferenceData", result[2].RoleNameFi);
            Assert.Equal("DimResearchActivity3 FactContribution researcher_name_activity DimReferenceData", result[2].RoleNameEn);
            Assert.Equal("DimResearchActivity3 FactContribution researcher_name_activity DimReferenceData", result[2].RoleNameSv);
            Assert.Equal(2023, result[2].StartDate.Year);
            Assert.Equal(4, result[2].StartDate.Month);
            Assert.Equal(15, result[2].StartDate.Day);
            Assert.Equal(2024, result[2].EndDate.Year);
            Assert.Equal(5, result[2].EndDate.Month);
            Assert.Equal(16, result[2].EndDate.Day);
            Assert.Empty(result[2].sector);
            Assert.NotNull(result[2].itemMeta);
            Assert.Equal(300, result[2].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_RESEARCH_ACTIVITY, result[2].itemMeta.Type);
            Assert.True(result[2].itemMeta.Show);
            Assert.True(result[2].itemMeta.PrimaryValue);
            Assert.Single(result[2].DataSources);
            Assert.Equal(1, result[2].DataSources[0].Id);
            Assert.Equal("DataSource1", result[2].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[2].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[2].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[2].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[2].DataSources[0].Organization.SectorId);

            // result[3] from DimProfileOnlyResearchActivity1
            Assert.Equal("DimProfileOnlyResearchActivity1 name fi", result[3].NameFi);
            Assert.Equal("DimProfileOnlyResearchActivity1 name en", result[3].NameEn);
            Assert.Equal("DimProfileOnlyResearchActivity1 name sv", result[3].NameSv);
            Assert.Equal("DimProfileOnlyResearchActivity1 description fi", result[3].DescriptionFi);
            Assert.Equal("DimProfileOnlyResearchActivity1 description en", result[3].DescriptionEn);
            Assert.Equal("DimProfileOnlyResearchActivity1 description sv", result[3].DescriptionSv);
            Assert.Equal("DimProfileOnlyResearchActivity1 organization broader name fi", result[3].OrganizationNameFi);
            Assert.Equal("DimProfileOnlyResearchActivity1 organization broader name en", result[3].OrganizationNameEn);
            Assert.Equal("DimProfileOnlyResearchActivity1 organization broader name sv", result[3].OrganizationNameSv);
            Assert.Equal("DimProfileOnlyResearchActivity1 organization name fi", result[3].DepartmentNameFi);
            Assert.Equal("DimProfileOnlyResearchActivity1 organization name en", result[3].DepartmentNameEn);
            Assert.Equal("DimProfileOnlyResearchActivity1 organization name sv", result[3].DepartmentNameSv);
            Assert.Equal("DimProfileOnlyResearchActivity1 activity_type DimReferenceData CodeValue", result[3].ActivityTypeCode);
            Assert.Equal("DimProfileOnlyResearchActivity1 activity_type DimReferenceData NameFi", result[3].ActivityTypeNameFi);
            Assert.Equal("DimProfileOnlyResearchActivity1 activity_type DimReferenceData NameEn", result[3].ActivityTypeNameEn);
            Assert.Equal("DimProfileOnlyResearchActivity1 activity_type DimReferenceData NameSv", result[3].ActivityTypeNameSv);
            Assert.Null(result[3].RoleCode);
            Assert.Equal("", result[3].RoleNameFi);
            Assert.Equal("", result[3].RoleNameEn);
            Assert.Equal("", result[3].RoleNameSv);
            Assert.Equal(1970, result[3].StartDate.Year);
            Assert.Equal(5, result[3].StartDate.Month);
            Assert.Equal(15, result[3].StartDate.Day);
            Assert.Equal(1980, result[3].EndDate.Year);
            Assert.Equal(4, result[3].EndDate.Month);
            Assert.Equal(14, result[3].EndDate.Day);
            Assert.Empty(result[3].sector);
            Assert.NotNull(result[3].itemMeta);
            Assert.Equal(1000, result[3].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_RESEARCH_ACTIVITY_PROFILE_ONLY, result[3].itemMeta.Type);
            Assert.True(result[3].itemMeta.Show);
            Assert.True(result[3].itemMeta.PrimaryValue);
            Assert.Single(result[3].DataSources);
            Assert.Equal(1, result[3].DataSources[0].Id);
            Assert.Equal("DataSource1", result[3].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name Fi", result[3].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name En", result[3].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name Sv", result[3].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[3].DataSources[0].Organization.SectorId);
            Assert.Equal("https://example.com/dim-profile-only-research-activity-1-wl1", result[3].Url);
            Assert.Equal(2, result[3].WebLinks.Count);
            Assert.Equal("https://example.com/dim-profile-only-research-activity-1-wl1", result[3].WebLinks[0].Url);
            Assert.Equal("DimProfileOnlyResearchActivity1 Web Link 1", result[3].WebLinks[0].LinkLabel);
            Assert.Equal("ProfileEditorWebLink", result[3].WebLinks[0].LinkType);
            Assert.Equal("https://example.com/dim-profile-only-research-activity-1-wl2", result[3].WebLinks[1].Url);
            Assert.Equal("DimProfileOnlyResearchActivity1 Web Link 2", result[3].WebLinks[1].LinkLabel);
            Assert.Equal("ProfileEditorWebLink", result[3].WebLinks[1].LinkType);

            // result[4] from DimProfileOnlyResearchActivity2
            Assert.Equal("DimProfileOnlyResearchActivity2 name", result[4].NameFi);
            Assert.Equal("DimProfileOnlyResearchActivity2 name", result[4].NameEn);
            Assert.Equal("DimProfileOnlyResearchActivity2 name", result[4].NameSv);
            Assert.Equal("DimProfileOnlyResearchActivity2 description", result[4].DescriptionFi);
            Assert.Equal("DimProfileOnlyResearchActivity2 description", result[4].DescriptionEn);
            Assert.Equal("DimProfileOnlyResearchActivity2 description", result[4].DescriptionSv);
            Assert.Equal("DimProfileOnlyResearchActivity2 organization name", result[4].OrganizationNameFi);
            Assert.Equal("DimProfileOnlyResearchActivity2 organization name", result[4].OrganizationNameEn);
            Assert.Equal("DimProfileOnlyResearchActivity2 organization name", result[4].OrganizationNameSv);
            Assert.Equal("", result[4].DepartmentNameFi);
            Assert.Equal("", result[4].DepartmentNameEn);
            Assert.Equal("", result[4].DepartmentNameSv);
            Assert.Equal("DimProfileOnlyResearchActivity2 activity_type DimReferenceData CodeValue", result[4].ActivityTypeCode);
            Assert.Equal("DimProfileOnlyResearchActivity2 activity_type DimReferenceData", result[4].ActivityTypeNameFi);
            Assert.Equal("DimProfileOnlyResearchActivity2 activity_type DimReferenceData", result[4].ActivityTypeNameEn);
            Assert.Equal("DimProfileOnlyResearchActivity2 activity_type DimReferenceData", result[4].ActivityTypeNameSv);
            Assert.Null(result[4].RoleCode);
            Assert.Equal("", result[4].RoleNameFi);
            Assert.Equal("", result[4].RoleNameEn);
            Assert.Equal("", result[4].RoleNameSv);
            Assert.Equal(1960, result[4].StartDate.Year);
            Assert.Equal(6, result[4].StartDate.Month);
            Assert.Equal(16, result[4].StartDate.Day);
            Assert.Equal(1965, result[4].EndDate.Year);
            Assert.Equal(7, result[4].EndDate.Month);
            Assert.Equal(17, result[4].EndDate.Day);
            Assert.Empty(result[4].sector);
            Assert.NotNull(result[4].itemMeta);
            Assert.Equal(1001, result[4].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_RESEARCH_ACTIVITY_PROFILE_ONLY, result[4].itemMeta.Type);
            Assert.False(result[4].itemMeta.Show);
            Assert.False(result[4].itemMeta.PrimaryValue);
            Assert.Single(result[4].DataSources);
            Assert.Equal(2, result[4].DataSources[0].Id);
            Assert.Equal("DataSource2", result[4].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[4].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[4].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[4].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[4].DataSources[0].Organization.SectorId);

            // result[5] from DimProfileOnlyResearchActivity3
            Assert.Equal("DimProfileOnlyResearchActivity3 name", result[5].NameFi);
            Assert.Equal("DimProfileOnlyResearchActivity3 name", result[5].NameEn);
            Assert.Equal("DimProfileOnlyResearchActivity3 name", result[5].NameSv);
            Assert.Equal("DimProfileOnlyResearchActivity3 description", result[5].DescriptionFi);
            Assert.Equal("DimProfileOnlyResearchActivity3 description", result[5].DescriptionEn);
            Assert.Equal("DimProfileOnlyResearchActivity3 description", result[5].DescriptionSv);
            Assert.Equal("DimProfileOnlyResearchActivity3 identifierless data value fi", result[5].OrganizationNameFi);
            Assert.Equal("DimProfileOnlyResearchActivity3 identifierless data value en", result[5].OrganizationNameEn);
            Assert.Equal("DimProfileOnlyResearchActivity3 identifierless data value sv", result[5].OrganizationNameSv);
            Assert.Equal("DimProfileOnlyResearchActivity3 identifierless data child value fi", result[5].DepartmentNameFi);
            Assert.Equal("DimProfileOnlyResearchActivity3 identifierless data child value en", result[5].DepartmentNameEn);
            Assert.Equal("DimProfileOnlyResearchActivity3 identifierless data child value sv", result[5].DepartmentNameSv);
            Assert.Equal("DimProfileOnlyResearchActivity3 activity_type DimReferenceData CodeValue", result[5].ActivityTypeCode);
            Assert.Equal("DimProfileOnlyResearchActivity3 activity_type DimReferenceData", result[5].ActivityTypeNameFi);
            Assert.Equal("DimProfileOnlyResearchActivity3 activity_type DimReferenceData", result[5].ActivityTypeNameEn);
            Assert.Equal("DimProfileOnlyResearchActivity3 activity_type DimReferenceData", result[5].ActivityTypeNameSv);
            Assert.Null(result[5].RoleCode);
            Assert.Equal("", result[5].RoleNameFi);
            Assert.Equal("", result[5].RoleNameEn);
            Assert.Equal("", result[5].RoleNameSv);
            Assert.Equal(1950, result[5].StartDate.Year);
            Assert.Equal(8, result[5].StartDate.Month);
            Assert.Equal(18, result[5].StartDate.Day);
            Assert.Equal(1955, result[5].EndDate.Year);
            Assert.Equal(9, result[5].EndDate.Month);
            Assert.Equal(19, result[5].EndDate.Day);
            Assert.Empty(result[5].sector);
            Assert.NotNull(result[5].itemMeta);
            Assert.Equal(1002, result[5].itemMeta.Id);
            Assert.Equal(Constants.ItemMetaTypes.ACTIVITY_RESEARCH_ACTIVITY_PROFILE_ONLY, result[5].itemMeta.Type);
            Assert.False(result[5].itemMeta.Show);
            Assert.False(result[5].itemMeta.PrimaryValue);
            Assert.Single(result[5].DataSources);
            Assert.Equal(2, result[5].DataSources[0].Id);
            Assert.Equal("DataSource2", result[5].DataSources[0].RegisteredDataSource);
            Assert.Equal("Org name", result[5].DataSources[0].Organization.NameFi);
            Assert.Equal("Org name", result[5].DataSources[0].Organization.NameEn);
            Assert.Equal("Org name", result[5].DataSources[0].Organization.NameSv);
            Assert.Equal("S1", result[5].DataSources[0].Organization.SectorId);
        }

        [Fact(DisplayName = "Research activity hash key")]
        public void ResearchActivityHashKey()
        {
            using var context = CreateInMemoryContext(nameof(ResearchActivityHashKey));
            ResearchActivityService service = new(context, null, null);

            Assert.Equal(
                "2020_researchactivitynamefi_researchactivitynameen_researchactivitynamesv",
                service.ComputeKey(
                    startYear: 2020,
                    nameFi: "ResearchActivity Name Fi",
                    nameEn: " ResearchActivity Name En",
                    nameSv: "ResearchActivity Name  Sv "
                )
            );
        }
    }
}