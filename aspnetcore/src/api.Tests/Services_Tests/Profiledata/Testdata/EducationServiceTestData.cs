using api.Models.Ttv;
using api.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;

namespace api.Tests.Profiledata
{
    public class EducationServiceTestData
    {
        public List<DimSector> DimSectors { get; private set; }
        public List<DimOrganization> DimOrganizations { get; private set; }
        public List<DimRegisteredDataSource> DimRegisteredDataSources { get; private set; }
        public List<DimFieldDisplaySetting> FieldDisplaySettings { get; private set; }
        public DimUserProfile UserProfile { get; private set; }
        public List<FactFieldValue> FactFieldValues { get; private set; }

        public static EducationServiceTestData Create()
        {
            var data = new EducationServiceTestData();
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
            DimFieldDisplaySetting dfdsActivityEducation = new DimFieldDisplaySetting { Id = 12, FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_EDUCATION, SourceId = "Source1"};
            data.FieldDisplaySettings = new List<DimFieldDisplaySetting>
            {
                dfdsActivityEducation
            };
    
         

            // Education 1
            FactFieldValue ffvEducation1 = userProfileService.GetEmptyFactFieldValue();
            ffvEducation1.DimUserProfileId = data.UserProfile.Id;
            ffvEducation1.DimUserProfile = data.UserProfile;
            ffvEducation1.DimFieldDisplaySettingsId = dfdsActivityEducation.Id; // ACTIVITY_EDUCATION
            ffvEducation1.DimFieldDisplaySettings = dfdsActivityEducation;
            ffvEducation1.DimEducationId = 1;
            ffvEducation1.DimEducation = new DimEducation {
                Id = 1,
                NameFi = "Education 1 name Fi",
                NameEn = "Education 1 name En",
                NameSv = "Education 1 name Sv",
                DegreeGrantingInstitutionName = "Test institution name 1",
                DimStartDate = 1,
                DimStartDateNavigation = new DimDate { Id = 1, Year = 2020, Month = 1, Day = 15, SourceId = "Source1" },
                DimEndDate = 2,
                DimEndDateNavigation = new DimDate { Id = 2, Year = 2022, Month = 6, Day = 30, SourceId = "Source1" },
                SourceId = "Source1"
            };
            ffvEducation1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvEducation1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvEducation1.Show = true;
            ffvEducation1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvEducation1);

            // Education 2
            FactFieldValue ffvEducation2 = userProfileService.GetEmptyFactFieldValue();
            ffvEducation2.DimUserProfileId = data.UserProfile.Id;
            ffvEducation2.DimUserProfile = data.UserProfile;
            ffvEducation2.DimFieldDisplaySettingsId = dfdsActivityEducation.Id; // ACTIVITY_EDUCATION
            ffvEducation2.DimFieldDisplaySettings = dfdsActivityEducation;
            ffvEducation2.DimEducationId = 2;
            ffvEducation2.DimEducation = new DimEducation {
                Id = 2,
                NameFi = "",
                NameEn = "Education 2 name", // Test that education name translation is working.
                NameSv = "",
                DegreeGrantingInstitutionName = "Test institution name 2",
                DimStartDate = 3,
                DimStartDateNavigation = new DimDate { Id = 3, Year = 2018, Month = 9, Day = 1, SourceId = "Source1" },
                DimEndDate = 4,
                DimEndDateNavigation = new DimDate { Id = 4, Year = 2020, Month = 5, Day = 31, SourceId = "Source1" },
                SourceId = "Source1"
            };
            ffvEducation2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvEducation2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvEducation2.Show = false;
            ffvEducation2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvEducation2);

            return data;
        }


        public async Task SeedAsync(TtvContext context)
        {
            context.FactFieldValues.AddRange(FactFieldValues);
            await context.SaveChangesAsync();
        }
    }
}