using api.Models.Ttv;
using api.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;

namespace api.Tests
{
    public class ProfileDataServiceTestData
    {
        public List<DimSector> DimSectors { get; private set; }
        public List<DimOrganization> DimOrganizations { get; private set; }
        public List<DimRegisteredDataSource> DimRegisteredDataSources { get; private set; }
        public List<DimFieldDisplaySetting> FieldDisplaySettings { get; private set; }
        public List<DimName> DimNames { get; private set; }
        public DimUserProfile UserProfile { get; private set; }
        public List<FactFieldValue> FactFieldValues { get; private set; }

        public static ProfileDataServiceTestData Create()
        {
            var data = new ProfileDataServiceTestData();
            UtilityService utilityService = new UtilityService();
            UserProfileService userProfileService = new UserProfileService(utilityService: utilityService);

            data.UserProfile = new DimUserProfile { Id = 1, SourceId = "Source1" };
            data.DimSectors = new List<DimSector>();
            data.DimOrganizations = new List<DimOrganization>();
            data.DimRegisteredDataSources = new List<DimRegisteredDataSource>();
            data.DimSectors.Add(new DimSector { Id = 1, SectorId = "S1", NameFi = "Sector Fi", NameSv = "Sector Sv", NameEn = "Sector En", SourceId = "Source1" });
            data.DimOrganizations.Add(new DimOrganization { Id = 1, NameFi = "Org name Fi", NameEn = "Org name En", NameSv = "Org name Sv", DimSector = data.DimSectors[0], SourceId = "Source1" });
            data.DimRegisteredDataSources.Add(new DimRegisteredDataSource { Id = 1, Name = "DataSource1", DimOrganization = data.DimOrganizations[0], SourceId = "Source1"});
            data.FactFieldValues = new List<FactFieldValue>();
            DimFieldDisplaySetting dfdsPersonName = new DimFieldDisplaySetting { Id = 1, FieldIdentifier = Constants.FieldIdentifiers.PERSON_NAME, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsPersonOtherNames = new DimFieldDisplaySetting { Id = 2, FieldIdentifier = Constants.FieldIdentifiers.PERSON_OTHER_NAMES, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsPersonExternalIdentifier = new DimFieldDisplaySetting { Id = 3, FieldIdentifier = Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsPersonResearcherDescription = new DimFieldDisplaySetting { Id = 4, FieldIdentifier = Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsPersonKeyword = new DimFieldDisplaySetting { Id = 5, FieldIdentifier = Constants.FieldIdentifiers.PERSON_KEYWORD, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsPersonFieldOfScience = new DimFieldDisplaySetting { Id = 6, FieldIdentifier = Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsPersonEmailAddress = new DimFieldDisplaySetting { Id = 7, FieldIdentifier = Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsPersonTelephoneNumber = new DimFieldDisplaySetting { Id = 8, FieldIdentifier = Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsPersonWebLink = new DimFieldDisplaySetting { Id = 9, FieldIdentifier = Constants.FieldIdentifiers.PERSON_WEB_LINK, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsActivityRoleInResearchCommunity = new DimFieldDisplaySetting { Id = 10, FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_ROLE_IN_RESERCH_COMMUNITY, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsActivityAffiliation = new DimFieldDisplaySetting { Id = 11, FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_AFFILIATION, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsActivityEducation = new DimFieldDisplaySetting { Id = 12, FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_EDUCATION, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsActivityPublication = new DimFieldDisplaySetting { Id = 13, FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsActivityPublicationProfileOnly = new DimFieldDisplaySetting { Id = 14, FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsActivityResearchDataset = new DimFieldDisplaySetting { Id = 15, FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsActivityInfrastructure = new DimFieldDisplaySetting { Id = 16, FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_INFRASTRUCTURE, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsActivityFundingDecision = new DimFieldDisplaySetting { Id = 17, FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsActivityResearchActivity = new DimFieldDisplaySetting { Id = 18, FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_RESEARCH_ACTIVITY, SourceId = "Source1"};
            data.FieldDisplaySettings = new List<DimFieldDisplaySetting>
            {
                dfdsPersonName,
                dfdsPersonOtherNames,
                dfdsPersonExternalIdentifier,
                dfdsPersonResearcherDescription,
                dfdsPersonKeyword,
                dfdsPersonFieldOfScience,
                dfdsPersonEmailAddress,
                dfdsPersonTelephoneNumber,
                dfdsPersonWebLink,
                dfdsActivityRoleInResearchCommunity,
                dfdsActivityAffiliation,
                dfdsActivityEducation,
                dfdsActivityPublication,
                dfdsActivityPublicationProfileOnly,
                dfdsActivityResearchDataset,
                dfdsActivityInfrastructure,
                dfdsActivityFundingDecision,
                dfdsActivityResearchActivity
            };
    

            // DimNames
            data.DimNames = new List<DimName>
            {
                new DimName { Id = 1, FirstNames = "John", LastName = "Doe", FullName = "", SourceId = "Source1" },
                new DimName { Id = 2, FirstNames = "Jack  ", LastName = "Smith  ", FullName = "", SourceId = "Source1" },
                new DimName { Id = 3, FirstNames = "", LastName = "", FullName = "John Doe 2", SourceId = "Source1" },
                new DimName { Id = 4, FirstNames = "", LastName = "", FullName = "Jack Smith 2       ", SourceId = "Source1" }
            };
            FactFieldValue ffvName1 = userProfileService.GetEmptyFactFieldValue();
            ffvName1.DimUserProfileId = data.UserProfile.Id;
            ffvName1.DimUserProfile = data.UserProfile;
            ffvName1.DimFieldDisplaySettingsId = dfdsPersonName.Id; // PERSON_NAME
            ffvName1.DimFieldDisplaySettings = dfdsPersonName;
            ffvName1.DimNameId = data.DimNames[0].Id;
            ffvName1.DimName = data.DimNames[0];
            ffvName1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvName1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvName1.Show = true;
            ffvName1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvName1);

            FactFieldValue ffvName2 = userProfileService.GetEmptyFactFieldValue();
            ffvName2.DimUserProfileId = data.UserProfile.Id;
            ffvName2.DimUserProfile = data.UserProfile;
            ffvName2.DimFieldDisplaySettingsId = dfdsPersonName.Id; // PERSON_NAME
            ffvName2.DimFieldDisplaySettings = dfdsPersonName;
            ffvName2.DimNameId = data.DimNames[1].Id;
            ffvName2.DimName = data.DimNames[1];
            ffvName2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvName2.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvName2.Show = false;
            ffvName2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvName2);

            FactFieldValue ffvOtherName1 = userProfileService.GetEmptyFactFieldValue();
            ffvOtherName1.DimUserProfileId = data.UserProfile.Id;
            ffvOtherName1.DimUserProfile = data.UserProfile;
            ffvOtherName1.DimFieldDisplaySettingsId = dfdsPersonOtherNames.Id; // PERSON_OTHER_NAMES
            ffvOtherName1.DimFieldDisplaySettings = dfdsPersonOtherNames;
            ffvOtherName1.DimNameId = data.DimNames[2].Id;
            ffvOtherName1.DimName = data.DimNames[2];
            ffvOtherName1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvOtherName1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvOtherName1.Show = true;
            ffvOtherName1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvOtherName1);

            FactFieldValue ffvOtherName2 = userProfileService.GetEmptyFactFieldValue();
            ffvOtherName2.DimUserProfileId = data.UserProfile.Id;
            ffvOtherName2.DimUserProfile = data.UserProfile;
            ffvOtherName2.DimFieldDisplaySettingsId = dfdsPersonOtherNames.Id; // PERSON_OTHER_NAMES
            ffvOtherName2.DimFieldDisplaySettings = dfdsPersonOtherNames;
            ffvOtherName2.DimNameId = data.DimNames[3].Id;
            ffvOtherName2.DimName = data.DimNames[3];
            ffvOtherName2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvOtherName2.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvOtherName2.Show = false;
            ffvOtherName2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvOtherName2);

            return data;
        }

        public async Task SeedAsync(TtvContext context)
        {
            context.DimSectors.AddRange(DimSectors);
            context.DimOrganizations.AddRange(DimOrganizations);
            context.DimRegisteredDataSources.AddRange(DimRegisteredDataSources);
            context.DimFieldDisplaySettings.AddRange(FieldDisplaySettings);
            context.DimNames.AddRange(DimNames);
            context.DimUserProfiles.Add(UserProfile);
            context.FactFieldValues.AddRange(FactFieldValues);
            await context.SaveChangesAsync();
        }
    }
}