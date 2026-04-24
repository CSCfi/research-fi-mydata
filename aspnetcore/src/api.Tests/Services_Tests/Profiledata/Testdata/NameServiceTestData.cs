using api.Models.Ttv;
using api.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;

namespace api.Tests.Profiledata
{
    public class NameServiceTestData
    {
        public List<DimSector> DimSectors { get; private set; }
        public List<DimOrganization> DimOrganizations { get; private set; }
        public List<DimRegisteredDataSource> DimRegisteredDataSources { get; private set; }
        public List<DimFieldDisplaySetting> FieldDisplaySettings { get; private set; }
        public DimUserProfile UserProfile { get; private set; }
        public List<FactFieldValue> FactFieldValues { get; private set; }

        public static NameServiceTestData Create()
        {
            var data = new NameServiceTestData();
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
            DimFieldDisplaySetting dfdsPersonName = new DimFieldDisplaySetting { Id = 1, FieldIdentifier = Constants.FieldIdentifiers.PERSON_NAME, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsPersonOtherNames = new DimFieldDisplaySetting { Id = 2, FieldIdentifier = Constants.FieldIdentifiers.PERSON_OTHER_NAMES, SourceId = "Source1"};
            data.FieldDisplaySettings = new List<DimFieldDisplaySetting>
            {
                dfdsPersonName,
                dfdsPersonOtherNames
            };
    
            // Name 1
            FactFieldValue ffvName1 = userProfileService.GetEmptyFactFieldValue();
            ffvName1.DimNameId = 1;
            ffvName1.DimName = new DimName {
                Id = 1,
                FirstNames = "John",
                LastName = "Doe",
                FullName = "",
                SourceId = "Source1"
            };
            ffvName1.DimUserProfileId = data.UserProfile.Id;
            ffvName1.DimUserProfile = data.UserProfile;
            ffvName1.DimFieldDisplaySettingsId = dfdsPersonName.Id; // PERSON_NAME
            ffvName1.DimFieldDisplaySettings = dfdsPersonName;
            ffvName1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvName1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvName1.Show = true;
            ffvName1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvName1);

            // Name 2
            FactFieldValue ffvName2 = userProfileService.GetEmptyFactFieldValue();
            ffvName2.DimNameId = 2;
            ffvName2.DimName = new DimName {
                Id = 2,
                FirstNames = "Jack  ", // Trailing whitespaces should be trimmed
                LastName = "Smith  ", // Trailing whitespaces should be trimmed
                FullName = "",
                SourceId = "Source1"
            };
            ffvName2.DimUserProfileId = data.UserProfile.Id;
            ffvName2.DimUserProfile = data.UserProfile;
            ffvName2.DimFieldDisplaySettingsId = dfdsPersonName.Id; // PERSON_NAME
            ffvName2.DimFieldDisplaySettings = dfdsPersonName;
            ffvName2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id; // This data source has only FI name. This tests that datasource name translation is working.
            ffvName2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvName2.Show = false;
            ffvName2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvName2);

            // Other name 1
            FactFieldValue ffvOtherName1 = userProfileService.GetEmptyFactFieldValue();
            ffvOtherName1.DimNameId = 3;
            ffvOtherName1.DimName = new DimName {
                Id = 3,
                FirstNames = "",
                LastName = "",
                FullName = "John Doe 2",
                SourceId = "Source1"
            };
            ffvOtherName1.DimUserProfileId = data.UserProfile.Id;
            ffvOtherName1.DimUserProfile = data.UserProfile;
            ffvOtherName1.DimFieldDisplaySettingsId = dfdsPersonOtherNames.Id; // PERSON_OTHER_NAMES
            ffvOtherName1.DimFieldDisplaySettings = dfdsPersonOtherNames;
            ffvOtherName1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvOtherName1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvOtherName1.Show = true;
            ffvOtherName1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvOtherName1);

            // Other name 2
            FactFieldValue ffvOtherName2 = userProfileService.GetEmptyFactFieldValue();
            ffvOtherName2.DimNameId = 4;
            ffvOtherName2.DimName = new DimName {
                Id = 4,
                FirstNames = "",
                LastName = "",
                FullName = "Jack Smith 2       ", // Trailing whitespaces should be trimmed
                SourceId = "Source1"
            };
            ffvOtherName2.DimUserProfileId = data.UserProfile.Id;
            ffvOtherName2.DimUserProfile = data.UserProfile;
            ffvOtherName2.DimFieldDisplaySettingsId = dfdsPersonOtherNames.Id; // PERSON_OTHER_NAMES
            ffvOtherName2.DimFieldDisplaySettings = dfdsPersonOtherNames;
            ffvOtherName2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvOtherName2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvOtherName2.Show = false;
            ffvOtherName2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvOtherName2);

            return data;
        }


        public async Task SeedAsync(TtvContext context)
        {
            context.FactFieldValues.AddRange(FactFieldValues);
            await context.SaveChangesAsync();
        }
    }
}