using api.Models.Ttv;
using api.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;

namespace api.Tests.Profiledata
{
    public class ResearcherDescriptionServiceTestData
    {
        public List<DimSector> DimSectors { get; private set; }
        public List<DimOrganization> DimOrganizations { get; private set; }
        public List<DimRegisteredDataSource> DimRegisteredDataSources { get; private set; }
        public List<DimFieldDisplaySetting> FieldDisplaySettings { get; private set; }
        public DimUserProfile UserProfile { get; private set; }
        public List<FactFieldValue> FactFieldValues { get; private set; }

        public static ResearcherDescriptionServiceTestData Create()
        {
            var data = new ResearcherDescriptionServiceTestData();
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
            data.DimOrganizations.Add(new DimOrganization { Id = 3, NameFi = "TTV Fi", NameEn = "TTV En", NameSv = "TTV Sv", DimSector = data.DimSectors[1], SourceId = "Source1" });
            data.DimRegisteredDataSources.Add(new DimRegisteredDataSource { Id = 1, Name = "DataSource1", DimOrganization = data.DimOrganizations[1], SourceId = "Source1"});
            data.DimRegisteredDataSources.Add(new DimRegisteredDataSource { Id = 2, Name = "DataSource2", DimOrganization = data.DimOrganizations[2], SourceId = "Source1"});
            data.DimRegisteredDataSources.Add(new DimRegisteredDataSource { Id = 3, Name = "TTV" , DimOrganization = data.DimOrganizations[3], SourceId = "Source1"});
            data.FactFieldValues = new List<FactFieldValue>();
            DimFieldDisplaySetting dfdsPersonResearcherDescription = new DimFieldDisplaySetting { Id = 4, FieldIdentifier = Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION, SourceId = "Source1"};
            data.FieldDisplaySettings = new List<DimFieldDisplaySetting>
            {
                dfdsPersonResearcherDescription
            };

            // Researcher description 1
            FactFieldValue ffvResearcherDescription1 = userProfileService.GetEmptyFactFieldValue();
            ffvResearcherDescription1.DimUserProfileId = data.UserProfile.Id;
            ffvResearcherDescription1.DimUserProfile = data.UserProfile;
            ffvResearcherDescription1.DimFieldDisplaySettingsId = dfdsPersonResearcherDescription.Id; // PERSON_RESEARCHER_DESCRIPTION
            ffvResearcherDescription1.DimFieldDisplaySettings = dfdsPersonResearcherDescription;
            ffvResearcherDescription1.DimResearcherDescriptionId = 1;
            ffvResearcherDescription1.DimResearcherDescription = new DimResearcherDescription {
                Id = 1,
                ResearchDescriptionFi = "",
                ResearchDescriptionEn = "Researcher description 1",
                ResearchDescriptionSv = "",
                SourceId = "Source1"
            };
            ffvResearcherDescription1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvResearcherDescription1.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvResearcherDescription1.Show = true;
            ffvResearcherDescription1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvResearcherDescription1);

            // Researcher description 2
            FactFieldValue ffvResearcherDescription2 = userProfileService.GetEmptyFactFieldValue();
            ffvResearcherDescription2.DimUserProfileId = data.UserProfile.Id;
            ffvResearcherDescription2.DimUserProfile = data.UserProfile;
            ffvResearcherDescription2.DimFieldDisplaySettingsId = dfdsPersonResearcherDescription.Id; // PERSON_RESEARCHER_DESCRIPTION
            ffvResearcherDescription2.DimFieldDisplaySettings = dfdsPersonResearcherDescription;
            ffvResearcherDescription2.DimResearcherDescriptionId = 2;
            ffvResearcherDescription2.DimResearcherDescription = new DimResearcherDescription {
                Id = 2,
                ResearchDescriptionFi = "Tutkijakuvaus 2 Fi",
                ResearchDescriptionEn = "Researcher description 2 En",
                ResearchDescriptionSv = "Forskarbeskrivning 2 Sv",
                SourceId = "Source1"
            };
            ffvResearcherDescription2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[2].Id; // TTV data source. This test that researcher description from TTV data source is handled correctly.
            ffvResearcherDescription2.DimRegisteredDataSource = data.DimRegisteredDataSources[2];
            ffvResearcherDescription2.Show = false;
            ffvResearcherDescription2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvResearcherDescription2);

            return data;
        }


        public async Task SeedAsync(TtvContext context)
        {
            context.FactFieldValues.AddRange(FactFieldValues);
            await context.SaveChangesAsync();
        }
    }
}