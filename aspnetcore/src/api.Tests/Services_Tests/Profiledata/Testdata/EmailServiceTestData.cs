using api.Models.Ttv;
using api.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;
using System.Linq;

namespace api.Tests.Profiledata
{
    public class EmailServiceTestData
    {
        public List<DimSector> DimSectors { get; private set; }
        public List<DimOrganization> DimOrganizations { get; private set; }
        public List<DimRegisteredDataSource> DimRegisteredDataSources { get; private set; }
        public List<DimFieldDisplaySetting> FieldDisplaySettings { get; private set; }
        public DimUserProfile UserProfile { get; private set; }
        public List<FactFieldValue> FactFieldValues { get; private set; }

        public static EmailServiceTestData Create()
        {
            var data = new EmailServiceTestData();
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
            DimFieldDisplaySetting dfdsPersonEmailAddress = new DimFieldDisplaySetting { Id = 7, FieldIdentifier = Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS, SourceId = "Source1"};
            data.FieldDisplaySettings = new List<DimFieldDisplaySetting>
            {
                dfdsPersonEmailAddress
            };

            // Email 1 
            FactFieldValue ffvEmail1 = userProfileService.GetEmptyFactFieldValue();
            ffvEmail1.DimUserProfileId = data.UserProfile.Id;
            ffvEmail1.DimUserProfile = data.UserProfile;
            ffvEmail1.DimFieldDisplaySettingsId = dfdsPersonEmailAddress.Id; // PERSON_EMAIL_ADDRESS
            ffvEmail1.DimFieldDisplaySettings = dfdsPersonEmailAddress;
            ffvEmail1.DimEmailAddrressId = 1;
            ffvEmail1.DimEmailAddrress = new DimEmailAddrress {
                Id = 1,
                Email = "test1@example.com",
                SourceId = "Source1"
            };
            ffvEmail1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvEmail1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvEmail1.Show = true;
            ffvEmail1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvEmail1);

            // Email 2
            FactFieldValue ffvEmail2 = userProfileService.GetEmptyFactFieldValue();
            ffvEmail2.DimUserProfileId = data.UserProfile.Id;
            ffvEmail2.DimUserProfile = data.UserProfile;
            ffvEmail2.DimFieldDisplaySettingsId = dfdsPersonEmailAddress.Id; // PERSON_EMAIL_ADDRESS
            ffvEmail2.DimFieldDisplaySettings = dfdsPersonEmailAddress;
            ffvEmail2.DimEmailAddrressId = 2;
            ffvEmail2.DimEmailAddrress = new DimEmailAddrress {
                Id = 2,
                Email = "test2@example.com",
                SourceId = "Source1"
            };
            ffvEmail2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvEmail2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvEmail2.Show = false;
            ffvEmail2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvEmail2);

            return data;
        }


        public async Task SeedAsync(TtvContext context)
        {
            context.FactFieldValues.AddRange(FactFieldValues);
            await context.SaveChangesAsync();
        }
    }
}