using api.Models.Ttv;
using api.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;
using System;

namespace api.Tests
{
    public class ProfileDataServiceTestData
    {
        public List<DimSector> DimSectors { get; private set; }
        public List<DimOrganization> DimOrganizations { get; private set; }
        public List<DimRegisteredDataSource> DimRegisteredDataSources { get; private set; }
        public List<DimFieldDisplaySetting> FieldDisplaySettings { get; private set; }
        public List<DimName> DimNames { get; private set; }
        public List<DimEmailAddrress> DimEmailAddresses { get; private set; }
        public List<DimTelephoneNumber> DimTelephoneNumbers { get; private set; }
        public List<DimWebLink> DimWebLinks { get; private set; }
        public List<DimKeyword> DimKeywords { get; private set; }
        public List<DimResearcherDescription> DimResearcherDescriptions { get; private set; }
        public List<DimPid> DimPids { get; private set; }
        public List<DimEducation> DimEducations { get; private set; }
        public List<DimAffiliation> DimAffiliations { get; private set; }
        public DimUserProfile UserProfile { get; private set; }
        public List<FactFieldValue> FactFieldValues { get; private set; }

        public static ProfileDataServiceTestData Create()
        {
            var data = new ProfileDataServiceTestData();
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
    
            /*
             * DimNames
             */
            data.DimNames = new List<DimName>
            {
                new DimName { Id = 1, FirstNames = "John", LastName = "Doe", FullName = "", SourceId = "Source1" },
                new DimName { Id = 2, FirstNames = "Jack  ", LastName = "Smith  ", FullName = "", SourceId = "Source1" },
                new DimName { Id = 3, FirstNames = "", LastName = "", FullName = "John Doe 2", SourceId = "Source1" },
                new DimName { Id = 4, FirstNames = "", LastName = "", FullName = "Jack Smith 2       ", SourceId = "Source1" }
            };
            // FactFieldValue - Name 1
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
            // FactFieldValue - Name 2
            FactFieldValue ffvName2 = userProfileService.GetEmptyFactFieldValue();
            ffvName2.DimUserProfileId = data.UserProfile.Id;
            ffvName2.DimUserProfile = data.UserProfile;
            ffvName2.DimFieldDisplaySettingsId = dfdsPersonName.Id; // PERSON_NAME
            ffvName2.DimFieldDisplaySettings = dfdsPersonName;
            ffvName2.DimNameId = data.DimNames[1].Id;
            ffvName2.DimName = data.DimNames[1];
            ffvName2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id; // This data source has only FI name. This tests that datasource name translation is working.
            ffvName2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvName2.Show = false;
            ffvName2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvName2);
            // FactFieldValue - Other name 1
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
            // FactFieldValue - Other name 2
            FactFieldValue ffvOtherName2 = userProfileService.GetEmptyFactFieldValue();
            ffvOtherName2.DimUserProfileId = data.UserProfile.Id;
            ffvOtherName2.DimUserProfile = data.UserProfile;
            ffvOtherName2.DimFieldDisplaySettingsId = dfdsPersonOtherNames.Id; // PERSON_OTHER_NAMES
            ffvOtherName2.DimFieldDisplaySettings = dfdsPersonOtherNames;
            ffvOtherName2.DimNameId = data.DimNames[3].Id;
            ffvOtherName2.DimName = data.DimNames[3];
            ffvOtherName2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvOtherName2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvOtherName2.Show = false;
            ffvOtherName2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvOtherName2);

            /*
             * DimEmailAddresses
             */
            data.DimEmailAddresses = new List<DimEmailAddrress>
            {
                new DimEmailAddrress { Id = 1, Email = "test1@example.com", SourceId = "Source1" },
                new DimEmailAddrress { Id = 2, Email = "test2@example.com", SourceId = "Source1" }
            };
            // FactFieldValue - Email 1 
            FactFieldValue ffvEmail1 = userProfileService.GetEmptyFactFieldValue();
            ffvEmail1.DimUserProfileId = data.UserProfile.Id;
            ffvEmail1.DimUserProfile = data.UserProfile;
            ffvEmail1.DimFieldDisplaySettingsId = dfdsPersonEmailAddress.Id; // PERSON_EMAIL_ADDRESS
            ffvEmail1.DimFieldDisplaySettings = dfdsPersonEmailAddress;
            ffvEmail1.DimEmailAddrressId = data.DimEmailAddresses[0].Id;
            ffvEmail1.DimEmailAddrress = data.DimEmailAddresses[0];
            ffvEmail1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvEmail1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvEmail1.Show = true;
            ffvEmail1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvEmail1);
            // FactFieldValue - Email 2
            FactFieldValue ffvEmail2 = userProfileService.GetEmptyFactFieldValue();
            ffvEmail2.DimUserProfileId = data.UserProfile.Id;
            ffvEmail2.DimUserProfile = data.UserProfile;
            ffvEmail2.DimFieldDisplaySettingsId = dfdsPersonEmailAddress.Id; // PERSON_EMAIL_ADDRESS
            ffvEmail2.DimFieldDisplaySettings = dfdsPersonEmailAddress;
            ffvEmail2.DimEmailAddrressId = data.DimEmailAddresses[1].Id;
            ffvEmail2.DimEmailAddrress = data.DimEmailAddresses[1];
            ffvEmail2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvEmail2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvEmail2.Show = false;
            ffvEmail2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvEmail2);

            /*
             * DimTelephoneNumbers
             */
            data.DimTelephoneNumbers = new List<DimTelephoneNumber>
            {
                new DimTelephoneNumber { Id = 1, TelephoneNumber = "+358501234567", SourceId = "Source1" },
                new DimTelephoneNumber { Id = 2, TelephoneNumber = "+358501234568", SourceId = "Source1" }
            };
            // FactFieldValue - Telephone 1
            FactFieldValue ffvTelephone1 = userProfileService.GetEmptyFactFieldValue();
            ffvTelephone1.DimUserProfileId = data.UserProfile.Id;
            ffvTelephone1.DimUserProfile = data.UserProfile;
            ffvTelephone1.DimFieldDisplaySettingsId = dfdsPersonTelephoneNumber.Id; // PERSON_TELEPHONE_NUMBER
            ffvTelephone1.DimFieldDisplaySettings = dfdsPersonTelephoneNumber;
            ffvTelephone1.DimTelephoneNumberId = data.DimTelephoneNumbers[0].Id;
            ffvTelephone1.DimTelephoneNumber = data.DimTelephoneNumbers[0];
            ffvTelephone1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvTelephone1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvTelephone1.Show = true;
            ffvTelephone1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvTelephone1);
            // FactFieldValue - Telephone 2
            FactFieldValue ffvTelephone2 = userProfileService.GetEmptyFactFieldValue();
            ffvTelephone2.DimUserProfileId = data.UserProfile.Id;
            ffvTelephone2.DimUserProfile = data.UserProfile;
            ffvTelephone2.DimFieldDisplaySettingsId = dfdsPersonTelephoneNumber.Id; // PERSON_TELEPHONE_NUMBER
            ffvTelephone2.DimFieldDisplaySettings = dfdsPersonTelephoneNumber;
            ffvTelephone2.DimTelephoneNumberId = data.DimTelephoneNumbers[1].Id;
            ffvTelephone2.DimTelephoneNumber = data.DimTelephoneNumbers[1];
            ffvTelephone2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvTelephone2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvTelephone2.Show = false;
            ffvTelephone2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvTelephone2);

            /*
             * DimWebLinks
             */
            data.DimWebLinks = new List<DimWebLink>
            {
                new DimWebLink { Id = 1, Url = "https://example1.com", LinkLabel = "Example1", LinkType = "Website1", SourceId = "Source1" },
                new DimWebLink { Id = 2, Url = "https://example2.org", LinkLabel = "Example2", LinkType = "Website2", SourceId = "Source1" } 
            };
            // FactFieldValue - Web link 1
            FactFieldValue ffvWebLink1 = userProfileService.GetEmptyFactFieldValue();
            ffvWebLink1.DimUserProfileId = data.UserProfile.Id;
            ffvWebLink1.DimUserProfile = data.UserProfile;
            ffvWebLink1.DimFieldDisplaySettingsId = dfdsPersonWebLink.Id; // PERSON_WEB_LINK
            ffvWebLink1.DimFieldDisplaySettings = dfdsPersonWebLink;
            ffvWebLink1.DimWebLinkId = data.DimWebLinks[0].Id;
            ffvWebLink1.DimWebLink = data.DimWebLinks[0];
            ffvWebLink1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvWebLink1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvWebLink1.Show = true;
            ffvWebLink1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvWebLink1);
            // FactFieldValue - Web link 2
            FactFieldValue ffvWebLink2 = userProfileService.GetEmptyFactFieldValue();
            ffvWebLink2.DimUserProfileId = data.UserProfile.Id;
            ffvWebLink2.DimUserProfile = data.UserProfile;
            ffvWebLink2.DimFieldDisplaySettingsId = dfdsPersonWebLink.Id; // PERSON_WEB_LINK
            ffvWebLink2.DimFieldDisplaySettings = dfdsPersonWebLink;
            ffvWebLink2.DimWebLinkId = data.DimWebLinks[1].Id;
            ffvWebLink2.DimWebLink = data.DimWebLinks[1];
            ffvWebLink2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvWebLink2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvWebLink2.Show = false;
            ffvWebLink2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvWebLink2);

            /*
             * DimKeywords
             */
            data.DimKeywords = new List<DimKeyword>
            {
                new DimKeyword { Id = 1, Keyword = "Keyword1", SourceId = "Source1" },
                new DimKeyword { Id = 2, Keyword = "Keyword2", SourceId = "Source1" }
            };
            // FactFieldValue - Keyword 1
            FactFieldValue ffvKeyword1 = userProfileService.GetEmptyFactFieldValue();
            ffvKeyword1.DimUserProfileId = data.UserProfile.Id;
            ffvKeyword1.DimUserProfile = data.UserProfile;
            ffvKeyword1.DimFieldDisplaySettingsId = dfdsPersonKeyword.Id; // PERSON_KEYWORD
            ffvKeyword1.DimFieldDisplaySettings = dfdsPersonKeyword;
            ffvKeyword1.DimKeywordId = data.DimKeywords[0].Id;
            ffvKeyword1.DimKeyword = data.DimKeywords[0];
            ffvKeyword1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvKeyword1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvKeyword1.Show = true;
            ffvKeyword1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvKeyword1);
            // FactFieldValue - Keyword 2
            FactFieldValue ffvKeyword2 = userProfileService.GetEmptyFactFieldValue();
            ffvKeyword2.DimUserProfileId = data.UserProfile.Id;
            ffvKeyword2.DimUserProfile = data.UserProfile;
            ffvKeyword2.DimFieldDisplaySettingsId = dfdsPersonKeyword.Id; // PERSON_KEYWORD
            ffvKeyword2.DimFieldDisplaySettings = dfdsPersonKeyword;
            ffvKeyword2.DimKeywordId = data.DimKeywords[1].Id;
            ffvKeyword2.DimKeyword = data.DimKeywords[1];
            ffvKeyword2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvKeyword2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvKeyword2.Show = false;
            ffvKeyword2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvKeyword2);

            /*
             * DimResearcherDescriptions
             */
            data.DimResearcherDescriptions = new List<DimResearcherDescription>
            {
                new DimResearcherDescription { Id = 1, ResearchDescriptionFi = "", ResearchDescriptionEn = "Researcher description 1", ResearchDescriptionSv = "", SourceId = "Source1" },
                new DimResearcherDescription { Id = 2, ResearchDescriptionFi = "Tutkijakuvaus 2 Fi", ResearchDescriptionEn = "Researcher description 2 En", ResearchDescriptionSv = "Forskarbeskrivning 2 Sv", SourceId = "Source1" }
            };
            // FactFieldValue - Researcher description 1
            FactFieldValue ffvResearcherDescription1 = userProfileService.GetEmptyFactFieldValue();
            ffvResearcherDescription1.DimUserProfileId = data.UserProfile.Id;
            ffvResearcherDescription1.DimUserProfile = data.UserProfile;
            ffvResearcherDescription1.DimFieldDisplaySettingsId = dfdsPersonResearcherDescription.Id; // PERSON_RESEARCHER_DESCRIPTION
            ffvResearcherDescription1.DimFieldDisplaySettings = dfdsPersonResearcherDescription;
            ffvResearcherDescription1.DimResearcherDescriptionId = data.DimResearcherDescriptions[0].Id;
            ffvResearcherDescription1.DimResearcherDescription = data.DimResearcherDescriptions[0];
            ffvResearcherDescription1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvResearcherDescription1.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvResearcherDescription1.Show = true;
            ffvResearcherDescription1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvResearcherDescription1);
            // FactFieldValue - Researcher description 2
            FactFieldValue ffvResearcherDescription2 = userProfileService.GetEmptyFactFieldValue();
            ffvResearcherDescription2.DimUserProfileId = data.UserProfile.Id;
            ffvResearcherDescription2.DimUserProfile = data.UserProfile;
            ffvResearcherDescription2.DimFieldDisplaySettingsId = dfdsPersonResearcherDescription.Id; // PERSON_RESEARCHER_DESCRIPTION
            ffvResearcherDescription2.DimFieldDisplaySettings = dfdsPersonResearcherDescription;
            ffvResearcherDescription2.DimResearcherDescriptionId = data.DimResearcherDescriptions[1].Id;
            ffvResearcherDescription2.DimResearcherDescription = data.DimResearcherDescriptions[1];
            ffvResearcherDescription2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[2].Id; // TTV data source. This test that researcher description from TTV data source is handled correctly.
            ffvResearcherDescription2.DimRegisteredDataSource = data.DimRegisteredDataSources[2];
            ffvResearcherDescription2.Show = false;
            ffvResearcherDescription2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvResearcherDescription2);

            /*
             * DimPids
             */
            data.DimPids = new List<DimPid>
            {
                new DimPid { Id = 1, PidContent = "test-pid-content-1", PidType = "test-pid-type-1", SourceId = "Source1" },
                new DimPid { Id = 2, PidContent = "test-pid-content-2", PidType = "test-pid-type-2", SourceId = "Source2" }
            };
            // FactFieldValue - PID 1
            FactFieldValue ffvPid1 = userProfileService.GetEmptyFactFieldValue();
            ffvPid1.DimUserProfileId = data.UserProfile.Id;
            ffvPid1.DimUserProfile = data.UserProfile;
            ffvPid1.DimFieldDisplaySettingsId = dfdsPersonExternalIdentifier.Id; // PERSON_EXTERNAL_IDENTIFIER
            ffvPid1.DimFieldDisplaySettings = dfdsPersonExternalIdentifier;
            ffvPid1.DimPidId = data.DimPids[0].Id;
            ffvPid1.DimPid = data.DimPids[0];
            ffvPid1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvPid1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvPid1.Show = true;
            ffvPid1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvPid1);
            // FactFieldValue - PID 2
            FactFieldValue ffvPid2 = userProfileService.GetEmptyFactFieldValue();
            ffvPid2.DimUserProfileId = data.UserProfile.Id;
            ffvPid2.DimUserProfile = data.UserProfile;
            ffvPid2.DimFieldDisplaySettingsId = dfdsPersonExternalIdentifier.Id; // PERSON_EXTERNAL_IDENTIFIER
            ffvPid2.DimFieldDisplaySettings = dfdsPersonExternalIdentifier;
            ffvPid2.DimPidId = data.DimPids[1].Id;
            ffvPid2.DimPid = data.DimPids[1];
            ffvPid2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvPid2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvPid2.Show = false;
            ffvPid2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvPid2);

            /*
             * DimEducations
             */
            data.DimEducations = new List<DimEducation>
            {
                new DimEducation {
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
                },
                new DimEducation {
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
                }
            };
            // FactFieldValue - Education 1
            FactFieldValue ffvEducation1 = userProfileService.GetEmptyFactFieldValue();
            ffvEducation1.DimUserProfileId = data.UserProfile.Id;
            ffvEducation1.DimUserProfile = data.UserProfile;
            ffvEducation1.DimFieldDisplaySettingsId = dfdsActivityEducation.Id; // ACTIVITY_EDUCATION
            ffvEducation1.DimFieldDisplaySettings = dfdsActivityEducation;
            ffvEducation1.DimEducationId = data.DimEducations[0].Id;
            ffvEducation1.DimEducation = data.DimEducations[0];
            ffvEducation1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvEducation1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvEducation1.Show = true;
            ffvEducation1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvEducation1);
            // FactFieldValue - Education 2
            FactFieldValue ffvEducation2 = userProfileService.GetEmptyFactFieldValue();
            ffvEducation2.DimUserProfileId = data.UserProfile.Id;
            ffvEducation2.DimUserProfile = data.UserProfile;
            ffvEducation2.DimFieldDisplaySettingsId = dfdsActivityEducation.Id; // ACTIVITY_EDUCATION
            ffvEducation2.DimFieldDisplaySettings = dfdsActivityEducation;
            ffvEducation2.DimEducationId = data.DimEducations[1].Id;
            ffvEducation2.DimEducation = data.DimEducations[1];
            ffvEducation2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvEducation2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvEducation2.Show = false;
            ffvEducation2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvEducation2);

            /*
             * DimAffiliations
             *
             * 1st affiliation
             *   - has DimOrganization => OrganizationBroader
             *   - has all language versions populated
             * 2nd affiliation
             *   - has DimOrganization, but that organization does not have OrganizationBroader. 
             * 3rd affiliation
             *   - does not have related DimOrganization. It has related DimIdentifierlessData.
             */
            data.DimAffiliations = new List<DimAffiliation>
            {
                new DimAffiliation {
                    Id = 1000,
                    PositionNameFi = "Affiliation 1 position name Fi",
                    PositionNameEn = "Affiliation 1 position name En",
                    PositionNameSv = "Affiliation 1 position name Sv",
                    AffiliationTypeFi = "Affiliation 1 type Fi",
                    AffiliationTypeEn = "Affiliation 1 type En",
                    AffiliationTypeSv = "Affiliation 1 type Sv",
                    StartDate = 1000,
                    StartDateNavigation = new DimDate { Id = 1000, Year = 2021, Month = 2, Day = 1, SourceId = "Source1" },
                    EndDate = 1001,
                    EndDateNavigation = new DimDate { Id = 1001, Year = 2023, Month = 1, Day = 31, SourceId = "Source1" },
                    DimOrganizationId = 1000,
                    DimOrganization = new DimOrganization {
                        Id = 1000,
                        NameFi = "Affiliation 1 organization name Fi",
                        NameEn = "Affiliation 1 organization name En",
                        NameSv = "Affiliation 1 organization name Sv",
                        SourceId = "Source1",
                        DimSectorid = 1000,
                        DimSector = new DimSector { Id = 1000, SectorId = "S2", NameFi = "Sector 2 Fi", NameSv = "Sector 2 Sv", NameEn = "Sector 2 En", SourceId = "Source1" },
                        DimOrganizationBroader = 1001,
                        DimOrganizationBroaderNavigation = new DimOrganization {
                            Id = 1001,
                            NameFi = "Affiliation 1 organization broader name Fi",
                            NameEn = "Affiliation 1 organization broader name En",
                            NameSv = "Affiliation 1 organization broader name Sv",
                            SourceId = "Source1",
                            DimSectorid = 1001,
                            DimSector = new DimSector { Id = 1001, SectorId = "S3", NameFi = "Sector 3 Fi", NameSv = "Sector 3 Sv", NameEn = "Sector 3 En", SourceId = "Source1" }
                        }
                    },
                    SourceId = "Source1"
                },
                new DimAffiliation {
                    Id = 1001,
                    PositionNameFi = "",
                    PositionNameEn = "Affiliation 2 position name En",
                    PositionNameSv = "",
                    AffiliationTypeFi = "",
                    AffiliationTypeEn = "Affiliation 2 type En",
                    AffiliationTypeSv = "",
                    StartDate = 1002,
                    StartDateNavigation = new DimDate { Id = 1002, Year = 2020, Month = 3, Day = 13, SourceId = "Source1" },
                    EndDate = 1003,
                    EndDateNavigation = new DimDate { Id = 1003, Year = 2022, Month = 2, Day = 30, SourceId = "Source1" },
                    DimOrganizationId = 1002,
                    DimOrganization = new DimOrganization {
                        Id = 1002,
                        NameFi = "Affiliation 2 organization name Fi",
                        NameEn = "Affiliation 2 organization name En",
                        NameSv = "Affiliation 2 organization name Sv",
                        SourceId = "Source1",
                        DimSectorid = 1002,
                        DimSector = new DimSector { Id = 1002, SectorId = "S2", NameFi = "Sector 2 Fi", NameSv = "Sector 2 Sv", NameEn = "Sector 2 En", SourceId = "Source1" }
                    },
                    SourceId = "Source1"
                },
                new DimAffiliation {
                    Id = 1002,
                    PositionNameFi = "Affiliation 3 position name Fi",
                    PositionNameEn = "Affiliation 3 position name En",
                    PositionNameSv = "Affiliation 3 position name Sv",
                    AffiliationTypeFi = "Affiliation 3 type Fi",
                    AffiliationTypeEn = "Affiliation 3 type En",
                    AffiliationTypeSv = "Affiliation 3 type Sv",
                    StartDate = 1004,
                    StartDateNavigation = new DimDate { Id = 1004, Year = 2020, Month = 3, Day = 13, SourceId = "Source1" },
                    EndDate = 1005,
                    EndDateNavigation = new DimDate { Id = 1005, Year = 2022, Month = 2, Day = 30, SourceId = "Source1" },
                    DimOrganizationId = -1,
                    DimOrganization = data.DimOrganizations[0],
                    SourceId = "Source1"
                }
            };

            // FactFieldValue - Affiliation 1
            FactFieldValue ffvAffiliation1 = userProfileService.GetEmptyFactFieldValue();
            ffvAffiliation1.DimUserProfileId = data.UserProfile.Id;
            ffvAffiliation1.DimUserProfile = data.UserProfile;
            ffvAffiliation1.DimFieldDisplaySettingsId = dfdsActivityAffiliation.Id; // ACTIVITY_AFFILIATION
            ffvAffiliation1.DimFieldDisplaySettings = dfdsActivityAffiliation;
            ffvAffiliation1.DimAffiliationId = data.DimAffiliations[0].Id;
            ffvAffiliation1.DimAffiliation = data.DimAffiliations[0];
            ffvAffiliation1.DimIdentifierlessDataId = -1;
            ffvAffiliation1.DimIdentifierlessData = new DimIdentifierlessDatum { Id = -1, SourceId = "Source1" };
            ffvAffiliation1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvAffiliation1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvAffiliation1.Show = true;
            ffvAffiliation1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvAffiliation1);
            // FactFieldValue - Affiliation 2
            FactFieldValue ffvAffiliation2 = userProfileService.GetEmptyFactFieldValue();
            ffvAffiliation2.DimUserProfileId = data.UserProfile.Id;
            ffvAffiliation2.DimUserProfile = data.UserProfile;
            ffvAffiliation2.DimFieldDisplaySettingsId = dfdsActivityAffiliation.Id; // ACTIVITY_AFFILIATION
            ffvAffiliation2.DimFieldDisplaySettings = dfdsActivityAffiliation;
            ffvAffiliation2.DimAffiliationId = data.DimAffiliations[1].Id;
            ffvAffiliation2.DimAffiliation = data.DimAffiliations[1];
            ffvAffiliation1.DimIdentifierlessDataId = -1;
            ffvAffiliation1.DimIdentifierlessData = new DimIdentifierlessDatum { Id = -1, SourceId = "Source1" };
            ffvAffiliation2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvAffiliation2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvAffiliation2.Show = false;
            ffvAffiliation2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvAffiliation2);
            // FactFieldValue - Affiliation 3
            FactFieldValue ffvAffiliation3 = userProfileService.GetEmptyFactFieldValue();
            ffvAffiliation3.DimUserProfileId = data.UserProfile.Id;
            ffvAffiliation3.DimUserProfile = data.UserProfile;
            ffvAffiliation3.DimFieldDisplaySettingsId = dfdsActivityAffiliation.Id; // ACTIVITY_AFFILIATION
            ffvAffiliation3.DimFieldDisplaySettings = dfdsActivityAffiliation;
            ffvAffiliation3.DimAffiliationId = data.DimAffiliations[2].Id;
            ffvAffiliation3.DimAffiliation = data.DimAffiliations[2];
            ffvAffiliation3.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvAffiliation3.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvAffiliation3.Show = false;
            ffvAffiliation3.PrimaryValue = false;
            ffvAffiliation3.DimIdentifierlessDataId = 1000;
            ffvAffiliation3.DimIdentifierlessData = new DimIdentifierlessDatum {
                Id = 1000,
                Type = Constants.IdentifierlessDataTypes.ORGANIZATION_NAME,
                ValueFi = "Affiliation 3 identifierless data value Fi",
                ValueEn = "Affiliation 3 identifierless data value En",
                ValueSv = "Affiliation 3 identifierless data value Sv",
                DimIdentifierlessDataId = 1001,
                DimIdentifierlessData = new DimIdentifierlessDatum {
                    Id = 1001,
                    Type = Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT,
                    ValueFi = "Affiliation 3 identifierless data child value Fi",
                    ValueEn = "Affiliation 3 identifierless data child value En",
                    ValueSv = "Affiliation 3 identifierless data child value Sv",
                    SourceId = "Source1"
                },
                SourceId = "Source1"
            };
            data.FactFieldValues.Add(ffvAffiliation3);

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