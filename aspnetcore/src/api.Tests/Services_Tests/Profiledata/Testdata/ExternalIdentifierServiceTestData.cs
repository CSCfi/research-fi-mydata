using api.Models.Ttv;
using api.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;

namespace api.Tests.Profiledata
{
    public class ExternalIdentifierServiceTestData
    {
        public List<DimSector> DimSectors { get; private set; }
        public List<DimOrganization> DimOrganizations { get; private set; }
        public List<DimRegisteredDataSource> DimRegisteredDataSources { get; private set; }
        public List<DimFieldDisplaySetting> FieldDisplaySettings { get; private set; }
        public DimUserProfile UserProfile { get; private set; }
        public List<FactFieldValue> FactFieldValues { get; private set; }

        public static ExternalIdentifierServiceTestData Create()
        {
            var data = new ExternalIdentifierServiceTestData();
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
            DimFieldDisplaySetting dfdsPersonExternalIdentifier = new DimFieldDisplaySetting { Id = 3, FieldIdentifier = Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER, SourceId = "Source1"};
            data.FieldDisplaySettings = new List<DimFieldDisplaySetting>
            {
                dfdsPersonExternalIdentifier
            };

            // PID 1
            FactFieldValue ffvPid1 = userProfileService.GetEmptyFactFieldValue();
            ffvPid1.DimUserProfileId = data.UserProfile.Id;
            ffvPid1.DimUserProfile = data.UserProfile;
            ffvPid1.DimFieldDisplaySettingsId = dfdsPersonExternalIdentifier.Id; // PERSON_EXTERNAL_IDENTIFIER
            ffvPid1.DimFieldDisplaySettings = dfdsPersonExternalIdentifier;
            ffvPid1.DimPidId = 1;
            ffvPid1.DimPid = new DimPid {
                Id = 1,
                PidContent = "test-pid-content-1",
                PidType = "test-pid-type-1",
                SourceId = "Source1"
            };
            ffvPid1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvPid1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvPid1.Show = true;
            ffvPid1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvPid1);

            // PID 2
            FactFieldValue ffvPid2 = userProfileService.GetEmptyFactFieldValue();
            ffvPid2.DimUserProfileId = data.UserProfile.Id;
            ffvPid2.DimUserProfile = data.UserProfile;
            ffvPid2.DimFieldDisplaySettingsId = dfdsPersonExternalIdentifier.Id; // PERSON_EXTERNAL_IDENTIFIER
            ffvPid2.DimFieldDisplaySettings = dfdsPersonExternalIdentifier;
            ffvPid2.DimPidId = 2;
            ffvPid2.DimPid = new DimPid {
                Id = 2,
                PidContent = "test-pid-content-2",
                PidType = "test-pid-type-2",
                SourceId = "Source2"
            };
            ffvPid2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvPid2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvPid2.Show = false;
            ffvPid2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvPid2);


            return data;
        }


        public async Task SeedAsync(TtvContext context)
        {
            context.FactFieldValues.AddRange(FactFieldValues);
            await context.SaveChangesAsync();
        }
    }
}