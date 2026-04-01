using api.Models.Ttv;
using api.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;

namespace api.Tests.Profiledata
{
    public class ResearchActivityServiceTestData
    {
        public List<DimSector> DimSectors { get; private set; }
        public List<DimOrganization> DimOrganizations { get; private set; }
        public List<DimRegisteredDataSource> DimRegisteredDataSources { get; private set; }
        public List<DimFieldDisplaySetting> FieldDisplaySettings { get; private set; }
        public DimUserProfile UserProfile { get; private set; }
        public List<FactFieldValue> FactFieldValues { get; private set; }

        public static ResearchActivityServiceTestData Create()
        {
            var data = new ResearchActivityServiceTestData();
            UtilityService utilityService = new UtilityService();
            DataSourceHelperService dataSourceHelperService = new DataSourceHelperService();
            
            UserProfileService userProfileService = new UserProfileService(utilityService: utilityService);

            data.UserProfile = new DimUserProfile { Id = 1, SourceId = "Source1" };
            data.DimSectors = new List<DimSector>();
            data.DimOrganizations = new List<DimOrganization>();
            data.DimRegisteredDataSources = new List<DimRegisteredDataSource>();
            data.DimSectors.Add(new DimSector { Id = -1, SectorId = "", NameFi = "", NameSv = "", NameEn = "", SourceId = "Source1" });
            data.DimSectors.Add(new DimSector { Id = 1, SectorId = "S1", NameFi = "Sector Fi", NameSv = "Sector Sv", NameEn = "Sector En", SourceId = "Source1" });
            data.DimOrganizations.Add(new DimOrganization { Id = -1, NameFi = "", NameEn = "", NameSv = "", DimSector = data.DimSectors[0], SourceId = "Source1" });
            data.DimOrganizations.Add(new DimOrganization { Id = 1, NameFi = "Org name Fi", NameEn = "Org name En", NameSv = "Org name Sv", DimSector = data.DimSectors[1], SourceId = "Source1" });
            data.DimOrganizations.Add(new DimOrganization { Id = 2, NameFi = "Org name", NameEn = "", NameSv = "", DimSector = data.DimSectors[1], SourceId = "Source1" });
            data.DimRegisteredDataSources.Add(new DimRegisteredDataSource { Id = 1, Name = "DataSource1", DimOrganization = data.DimOrganizations[1], SourceId = "Source1"});
            data.DimRegisteredDataSources.Add(new DimRegisteredDataSource { Id = 2, Name = "DataSource2", DimOrganization = data.DimOrganizations[2], SourceId = "Source1"});
            data.FactFieldValues = new List<FactFieldValue>();
            DimFieldDisplaySetting dfdsResearchActivity = new DimFieldDisplaySetting { Id = 1, FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_RESEARCH_ACTIVITY, SourceId = "Source1"};
            data.FieldDisplaySettings = new List<DimFieldDisplaySetting>
            {
                dfdsResearchActivity
            };
    
            // DimResearchActivity 1
            FactFieldValue ffvResearchActivity1 = userProfileService.GetEmptyFactFieldValue();
            ffvResearchActivity1.DimResearchActivityId = 1;
            ffvResearchActivity1.DimResearchActivity = new DimResearchActivity {
                Id = 1,
                DescriptionFi = "dimResearchActivity1 description fi",
                DescriptionSv = "dimResearchActivity1 description sv",
                DescriptionEn = "dimResearchActivity1 description en",
                DimEndDate = 400,
                DimEndDateNavigation = new DimDate { Id = 400, Year = 2020, Month = 2, Day = 22, SourceId = "Source1" },
                DimOrganizationId = data.DimOrganizations[1].Id,
                DimOrganization = data.DimOrganizations[1],
                DimPids = new List<DimPid>(),
                DimStartDate = 401,
                DimStartDateNavigation = new DimDate { Id = 401, Year = 2019, Month = 1, Day = 11, SourceId = "Source1" },
                DimWebLinks = new List<DimWebLink>()
                {
                    new DimWebLink { Id = 1, Url = "https://example.com/research-activity-1-wl1", LinkLabel = "Research Activity 1 Web Link 1", LinkType = "ProfileEditorWebLink", SourceId = "Source1" },
                    new DimWebLink { Id = 2, Url = "https://example.com/research-activity-1-wl2", LinkLabel = "Research Activity 1 Web Link 2", LinkType = "ProfileEditorWebLink", SourceId = "Source1" }
                },
                InternationalCollaboration = true,
                LocalIdentifier = "dimResearchActivity1 local identifier",
                NameFi = "dimResearchActivity1 name fi",
                NameSv = "dimResearchActivity1 name sv",
                NameEn = "dimResearchActivity1 name en",
                SourceId = "Source1"
            };
            ffvResearchActivity1.DimUserProfileId = data.UserProfile.Id;
            ffvResearchActivity1.DimUserProfile = data.UserProfile;
            ffvResearchActivity1.DimFieldDisplaySettingsId = dfdsResearchActivity.Id; // RESEARCH_ACTIVITY
            ffvResearchActivity1.DimFieldDisplaySettings = dfdsResearchActivity;
            ffvResearchActivity1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvResearchActivity1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvResearchActivity1.Show = true;
            ffvResearchActivity1.PrimaryValue = true;
            ffvResearchActivity1.DimIdentifierlessDataId = -1;
            ffvResearchActivity1.DimIdentifierlessData = new DimIdentifierlessDatum { Id = -1, SourceId = "Source1" };
            data.FactFieldValues.Add(ffvResearchActivity1);
            return data;
        }


        public async Task SeedAsync(TtvContext context)
        {
            context.FactFieldValues.AddRange(FactFieldValues);
            await context.SaveChangesAsync();
        }
    }
}