using api.Models.Ttv;
using api.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;

namespace api.Tests.Profiledata
{
    public class WebLinkServiceTestData
    {
        public List<DimSector> DimSectors { get; private set; }
        public List<DimOrganization> DimOrganizations { get; private set; }
        public List<DimRegisteredDataSource> DimRegisteredDataSources { get; private set; }
        public List<DimFieldDisplaySetting> FieldDisplaySettings { get; private set; }
        public DimUserProfile UserProfile { get; private set; }
        public List<FactFieldValue> FactFieldValues { get; private set; }

        public static WebLinkServiceTestData Create()
        {
            var data = new WebLinkServiceTestData();
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
            DimFieldDisplaySetting dfdsPersonWebLink = new DimFieldDisplaySetting { Id = 9, FieldIdentifier = Constants.FieldIdentifiers.PERSON_WEB_LINK, SourceId = "Source1"};
            data.FieldDisplaySettings = new List<DimFieldDisplaySetting>
            {
                dfdsPersonWebLink,
            };

            // Web link 1
            FactFieldValue ffvWebLink1 = userProfileService.GetEmptyFactFieldValue();
            ffvWebLink1.DimUserProfileId = data.UserProfile.Id;
            ffvWebLink1.DimUserProfile = data.UserProfile;
            ffvWebLink1.DimFieldDisplaySettingsId = dfdsPersonWebLink.Id; // PERSON_WEB_LINK
            ffvWebLink1.DimFieldDisplaySettings = dfdsPersonWebLink;
            ffvWebLink1.DimWebLinkId = 1;
            ffvWebLink1.DimWebLink = new DimWebLink { Id = 1, Url = "https://example1.com", LinkLabel = "Example1", LinkType = "Website1", SourceId = "Source1" };
            ffvWebLink1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvWebLink1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvWebLink1.Show = true;
            ffvWebLink1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvWebLink1);

            // Web link 2
            FactFieldValue ffvWebLink2 = userProfileService.GetEmptyFactFieldValue();
            ffvWebLink2.DimUserProfileId = data.UserProfile.Id;
            ffvWebLink2.DimUserProfile = data.UserProfile;
            ffvWebLink2.DimFieldDisplaySettingsId = dfdsPersonWebLink.Id; // PERSON_WEB_LINK
            ffvWebLink2.DimFieldDisplaySettings = dfdsPersonWebLink;
            ffvWebLink2.DimWebLinkId = 2;
            ffvWebLink2.DimWebLink = new DimWebLink { Id = 2, Url = "https://example2.org", LinkLabel = "Example2", LinkType = "Website2", SourceId = "Source1" };
            ffvWebLink2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvWebLink2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvWebLink2.Show = false;
            ffvWebLink2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvWebLink2);

            return data;
        }


        public async Task SeedAsync(TtvContext context)
        {
            context.FactFieldValues.AddRange(FactFieldValues);
            await context.SaveChangesAsync();
        }
    }
}