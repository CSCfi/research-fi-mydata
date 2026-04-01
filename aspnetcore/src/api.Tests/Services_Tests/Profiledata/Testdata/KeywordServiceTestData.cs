using api.Models.Ttv;
using api.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;

namespace api.Tests.Profiledata
{
    public class KeywordServiceTestData
    {
        public List<DimSector> DimSectors { get; private set; }
        public List<DimOrganization> DimOrganizations { get; private set; }
        public List<DimRegisteredDataSource> DimRegisteredDataSources { get; private set; }
        public List<DimFieldDisplaySetting> FieldDisplaySettings { get; private set; }
        public DimUserProfile UserProfile { get; private set; }
        public List<FactFieldValue> FactFieldValues { get; private set; }

        public static KeywordServiceTestData Create()
        {
            var data = new KeywordServiceTestData();
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
            DimFieldDisplaySetting dfdsPersonKeyword = new DimFieldDisplaySetting { Id = 5, FieldIdentifier = Constants.FieldIdentifiers.PERSON_KEYWORD, SourceId = "Source1"};
            data.FieldDisplaySettings = new List<DimFieldDisplaySetting>
            {
                dfdsPersonKeyword
            };

            // Keyword 1
            FactFieldValue ffvKeyword1 = userProfileService.GetEmptyFactFieldValue();
            ffvKeyword1.DimUserProfileId = data.UserProfile.Id;
            ffvKeyword1.DimUserProfile = data.UserProfile;
            ffvKeyword1.DimFieldDisplaySettingsId = dfdsPersonKeyword.Id; // PERSON_KEYWORD
            ffvKeyword1.DimFieldDisplaySettings = dfdsPersonKeyword;
            ffvKeyword1.DimKeywordId = 1;
            ffvKeyword1.DimKeyword = new DimKeyword {
                Id = 1,
                Keyword = "Keyword1",
                SourceId = "Source1"
            };
            ffvKeyword1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvKeyword1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvKeyword1.Show = true;
            ffvKeyword1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvKeyword1);

            // Keyword 2
            FactFieldValue ffvKeyword2 = userProfileService.GetEmptyFactFieldValue();
            ffvKeyword2.DimUserProfileId = data.UserProfile.Id;
            ffvKeyword2.DimUserProfile = data.UserProfile;
            ffvKeyword2.DimFieldDisplaySettingsId = dfdsPersonKeyword.Id; // PERSON_KEYWORD
            ffvKeyword2.DimFieldDisplaySettings = dfdsPersonKeyword;
            ffvKeyword2.DimKeywordId = 2;
            ffvKeyword2.DimKeyword = new DimKeyword {
                Id = 2,
                Keyword = "Keyword2",
                SourceId = "Source1"
            };
            ffvKeyword2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvKeyword2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvKeyword2.Show = false;
            ffvKeyword2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvKeyword2);

            return data;
        }


        public async Task SeedAsync(TtvContext context)
        {
            context.FactFieldValues.AddRange(FactFieldValues);
            await context.SaveChangesAsync();
        }
    }
}