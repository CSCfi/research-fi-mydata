using api.Models.Ttv;
using api.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;

namespace api.Tests.Profiledata
{
    public class TelephoneNumberServiceTestData
    {
        public List<DimSector> DimSectors { get; private set; }
        public List<DimOrganization> DimOrganizations { get; private set; }
        public List<DimRegisteredDataSource> DimRegisteredDataSources { get; private set; }
        public List<DimFieldDisplaySetting> FieldDisplaySettings { get; private set; }
        public DimUserProfile UserProfile { get; private set; }
        public List<FactFieldValue> FactFieldValues { get; private set; }

        public static TelephoneNumberServiceTestData Create()
        {
            var data = new TelephoneNumberServiceTestData();
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
            DimFieldDisplaySetting dfdsPersonTelephoneNumber = new DimFieldDisplaySetting { Id = 8, FieldIdentifier = Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER, SourceId = "Source1"};
            data.FieldDisplaySettings = new List<DimFieldDisplaySetting>
            {
                dfdsPersonTelephoneNumber
            };
    
            // Telephone 1
            FactFieldValue ffvTelephone1 = userProfileService.GetEmptyFactFieldValue();
            ffvTelephone1.DimUserProfileId = data.UserProfile.Id;
            ffvTelephone1.DimUserProfile = data.UserProfile;
            ffvTelephone1.DimFieldDisplaySettingsId = dfdsPersonTelephoneNumber.Id; // PERSON_TELEPHONE_NUMBER
            ffvTelephone1.DimFieldDisplaySettings = dfdsPersonTelephoneNumber;
            ffvTelephone1.DimTelephoneNumberId = 1;
            ffvTelephone1.DimTelephoneNumber = new DimTelephoneNumber {
                Id = 1,
                TelephoneNumber = "+358501234567",
                SourceId = "Source1"
            };
            ffvTelephone1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvTelephone1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvTelephone1.Show = true;
            ffvTelephone1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvTelephone1);

            // Telephone 2
            FactFieldValue ffvTelephone2 = userProfileService.GetEmptyFactFieldValue();
            ffvTelephone2.DimUserProfileId = data.UserProfile.Id;
            ffvTelephone2.DimUserProfile = data.UserProfile;
            ffvTelephone2.DimFieldDisplaySettingsId = dfdsPersonTelephoneNumber.Id; // PERSON_TELEPHONE_NUMBER
            ffvTelephone2.DimFieldDisplaySettings = dfdsPersonTelephoneNumber;
            ffvTelephone2.DimTelephoneNumberId = 2;
            ffvTelephone2.DimTelephoneNumber = new DimTelephoneNumber {
                Id = 2,
                TelephoneNumber = "+358501234568",
                SourceId = "Source1"
            };
            ffvTelephone2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvTelephone2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvTelephone2.Show = false;
            ffvTelephone2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvTelephone2);

            return data;
        }


        public async Task SeedAsync(TtvContext context)
        {
            context.FactFieldValues.AddRange(FactFieldValues);
            await context.SaveChangesAsync();
        }
    }
}