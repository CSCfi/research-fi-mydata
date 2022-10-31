using Xunit;
using api.Services;
using api.Models.Common;
using api.Models.Ttv;
using api.Models.ProfileEditor.Items;
using System;
using System.Collections.Generic;

namespace api.Tests
{
    [Collection("Get SQL query from TtvSqlService")]
    public class TtvSqlServiceTests
    {
        public FactFieldValue GetFactFieldValueForTest()
        {
            // Get FactFieldValue for test
            return new FactFieldValue()
            {
                DimAffiliationId = -1,
                DimCompetenceId = -1,
                DimEducationId = -1,
                DimEmailAddrressId = -1,
                DimEventId = -1,
                DimFieldDisplaySettingsId = -1,
                DimFieldOfScienceId = -1,
                DimFundingDecisionId = -1,
                DimIdentifierlessDataId = -1,
                DimKeywordId = -1,
                DimNameId = -1,
                DimOrcidPublicationId = -1,
                DimPidId = -1,
                DimPidIdOrcidPutCode = -1,
                DimPublicationId = -1,
                DimResearchActivityId = -1,
                DimResearchCommunityId = -1,
                DimResearchDatasetId = -1,
                DimResearcherDescriptionId = -1,
                DimResearcherToResearchCommunityId = -1,
                DimTelephoneNumberId = -1,
                DimUserProfileId = -1,
                DimWebLinkId = -1
            };
        }


        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_name_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_name_id()
        {
            TtvSqlService ttvSqlService = new();
            // Names
            Assert.Equal(
                "dim_name_id", ttvSqlService.GetFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.PERSON_NAME)
            );
            // Other names
            Assert.Equal(
                "dim_name_id", ttvSqlService.GetFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.PERSON_OTHER_NAMES)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_researcher_description_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_researcher_description_id()
        {
            TtvSqlService ttvSqlService = new();
            // Researcer description
            Assert.Equal(
                "dim_researcher_description_id", ttvSqlService.GetFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_web_link_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_web_link_id()
        {
            TtvSqlService ttvSqlService = new();
            // Web link
            Assert.Equal(
                "dim_web_link_id", ttvSqlService.GetFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.PERSON_WEB_LINK)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_email_addrress_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_email_addrress_id()
        {
            TtvSqlService ttvSqlService = new();
            // Web link
            Assert.Equal(
                "dim_email_addrress_id", ttvSqlService.GetFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_keyword_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_keyword_id()
        {
            TtvSqlService ttvSqlService = new();
            // Web link
            Assert.Equal(
                "dim_keyword_id", ttvSqlService.GetFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.PERSON_KEYWORD)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_telephone_number_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_telephone_number_id()
        {
            TtvSqlService ttvSqlService = new();
            // Web link
            Assert.Equal(
                "dim_telephone_number_id", ttvSqlService.GetFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_affiliation_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_affiliation_id()
        {
            TtvSqlService ttvSqlService = new();
            // Web link
            Assert.Equal(
                "dim_affiliation_id", ttvSqlService.GetFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.ACTIVITY_AFFILIATION)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_education_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_education_id()
        {
            TtvSqlService ttvSqlService = new();
            // Web link
            Assert.Equal(
                "dim_education_id", ttvSqlService.GetFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.ACTIVITY_EDUCATION)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_publication_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_publication_id()
        {
            TtvSqlService ttvSqlService = new();
            // Web link
            Assert.Equal(
                "dim_publication_id", ttvSqlService.GetFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.ACTIVITY_PUBLICATION)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_orcid_publication_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_orcid_publication_id()
        {
            TtvSqlService ttvSqlService = new();
            // Web link
            Assert.Equal(
                "dim_orcid_publication_id", ttvSqlService.GetFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_funding_decision_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_funding_decision_id()
        {
            TtvSqlService ttvSqlService = new();
            // Funding decision
            Assert.Equal(
                "dim_funding_decision_id", ttvSqlService.GetFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_research_dataset_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_research_dataset_id()
        {
            TtvSqlService ttvSqlService = new();
            // Research dataset
            Assert.Equal(
                "dim_research_dataset_id", ttvSqlService.GetFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET)
            );
        }

        [Fact(DisplayName = "Test that list of integers is converted to a comma separated string")]
        public void Test_ConvertListOfIntsToCommaSeparatedString()
        {
            TtvSqlService ttvSqlService = new();
            List<int> listOfInts = new()
            {
                3489,
                432523,
                2,
                45,
                5345,
                98752,
                1
            };
            string expectedString = "3489,432523,2,45,5345,98752,1";
            Assert.Equal(
                expectedString,
                ttvSqlService.ConvertListOfIntsToCommaSeparatedString(listOfInts)
            );
        }

        [Fact(DisplayName = "Get SQL query for updating FactFieldValues, first name")]
        public void Test_getSqlQuery_Update_FactFieldValues_first_name()
        {
            TtvSqlService ttvSqlService = new();
            int userProfileId = 80;
            ProfileEditorItemMeta profileEditorItemMeta = new()
            {
                Id = 321,
                Type = Constants.FieldIdentifiers.PERSON_NAME,
                PrimaryValue = false,
                Show = true
            };

            string expectedSqlString = @"UPDATE fact_field_values
                                SET
                                    show=1,
                                    primary_value=0,
                                    modified=GETDATE()
                                WHERE
                                    dim_user_profile_id=80 AND
                                    dim_name_id=321";
            string actualSqlString = ttvSqlService.GetSqlQuery_Update_FactFieldValues(userProfileId, profileEditorItemMeta);

            Assert.Equal(
                expectedSqlString.Replace("\n", String.Empty).Replace(" ", String.Empty),
                actualSqlString.Replace("\n", String.Empty).Replace(" ", String.Empty)
            );
        }

        [Fact(DisplayName = "Get SQL query for updating FactFieldValues, researcher description")]
        public void Test_getSqlQuery_Update_FactFieldValues_researcher_description()
        {
            TtvSqlService ttvSqlService = new();
            int userProfileId = 5678;
            ProfileEditorItemMeta profileEditorItemMeta = new()
            {
                Id = 254,
                Type = Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION,
                PrimaryValue = true,
                Show = false
            };

            string expectedSqlString = @"UPDATE fact_field_values
                                SET
                                    show=0,
                                    primary_value=1,
                                    modified=GETDATE()
                                WHERE
                                    dim_user_profile_id=5678 AND
                                    dim_researcher_description_id=254";
            string actualSqlString = ttvSqlService.GetSqlQuery_Update_FactFieldValues(userProfileId, profileEditorItemMeta);

            Assert.Equal(
                expectedSqlString.Replace("\n", String.Empty).Replace(" ", String.Empty),
                actualSqlString.Replace("\n", String.Empty).Replace(" ", String.Empty)
            );
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, affiliation")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_affiliation()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 12, 23, 34};
            string expectedSqlString = "DELETE FROM dim_affiliation WHERE id IN (12,23,34)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimAffiliations(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, competence")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_competence()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 23, 34, 45 };
            string expectedSqlString = "DELETE FROM dim_competence WHERE id IN (23,34,45)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimCompetences(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, education")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_education()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 34, 45, 56 };
            string expectedSqlString = "DELETE FROM dim_education WHERE id IN (34,45,56)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimEducations(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, email")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_email()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 45, 56, 67 };
            string expectedSqlString = "DELETE FROM dim_email_addrress WHERE id IN (45,56,67)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimEmailAddrresses(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, event")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_event()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 56, 67, 78 };
            string expectedSqlString = "DELETE FROM dim_event WHERE id IN (56,67,78)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimEvents(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, field of science")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_field_of_science()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 67, 78, 89 };
            string expectedSqlString = "DELETE FROM dim_field_of_science WHERE id IN (67,78,89)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimFieldsOfScience(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, funding decision")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_funding_decision()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 78, 89, 90 };
            string expectedSqlString = "DELETE FROM dim_funding_decision WHERE id IN (78,89,90)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimFundingDecisions(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, keyword")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_keyword()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 100, 101, 102 };
            string expectedSqlString = "DELETE FROM dim_keyword WHERE id IN (100,101,102)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimKeyword(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, name")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_name()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 101, 102, 103 };
            string expectedSqlString = "DELETE FROM dim_name WHERE id IN (101,102,103)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimNames(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, ORCID publication")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_ORCID_publication()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 102, 103, 104 };
            string expectedSqlString = "DELETE FROM dim_orcid_publication WHERE id IN (102,103,104)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimOrcidPublications(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, pid")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_pid()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 103, 104, 105 };
            string expectedSqlString = "DELETE FROM dim_pid WHERE id IN (103,104,105)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimPids(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, research activity")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_research_activity()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 104, 105, 106 };
            string expectedSqlString = "DELETE FROM dim_research_activity WHERE id IN (104,105,106)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimResearchActivities(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, research community")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_research_community()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 105, 106, 107 };
            string expectedSqlString = "DELETE FROM dim_research_community WHERE id IN (105,106,107)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimResearchCommunities(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, research dataset")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_research_dataset()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 106, 107, 108 };
            string expectedSqlString = "DELETE FROM dim_research_dataset WHERE id IN (106,107,108)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimResearchDatasets(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, researcher description")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_researcher_description()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 107, 108, 109 };
            string expectedSqlString = "DELETE FROM dim_researcher_description WHERE id IN (107,108,109)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimResearchDescriptions(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, researcher to research community")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_researcher_to_research_community()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 108, 109, 110 };
            string expectedSqlString = "DELETE FROM dim_researcher_to_research_community WHERE id IN (108,109,110)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimResearcherToResearchCommunities(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, telephone number")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_telephone_number()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 109, 110, 111 };
            string expectedSqlString = "DELETE FROM dim_telephone_number WHERE id IN (109,110,111)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimTelephoneNumbers(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, web link")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_web_link()
        {
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 110, 111, 112 };
            string expectedSqlString = "DELETE FROM dim_telephone_number WHERE id IN (110,111,112)";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimTelephoneNumbers(ids);
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for selecting fact_field_values")]
        public void Test_GetSqlQuery_Select_FactFieldValues()
        {
            TtvSqlService ttvSqlService = new();
            string expectedSqlString = "SELECT * FROM fact_field_values WHERE dim_user_profile_id=332211";
            string actualSqlString = ttvSqlService.GetSqlQuery_Select_FactFieldValues(332211);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting fact_field_values")]
        public void Test_GetSqlQuery_Delete_FactFieldValues()
        {
            TtvSqlService ttvSqlService = new();
            string expectedSqlString = "DELETE FROM fact_field_values WHERE dim_user_profile_id=443322";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValues(443322);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting dim_identifierless_data children")]
        public void Test_GetSqlQuery_Delete_DimIdentifierlessData_Children()
        {
            TtvSqlService ttvSqlService = new();
            string expectedSqlString = "DELETE FROM dim_identifierless_data where dim_identifierless_data_id=554433";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimIdentifierlessData_Children(554433);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting dim_identifierless_data parent")]
        public void Test_GetSqlQuery_Delete_DimIdentifierlessData_Parent()
        {
            TtvSqlService ttvSqlService = new();
            string expectedSqlString = "DELETE FROM dim_identifierless_data where id=665544";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimIdentifierlessData_Parent(665544);
            Assert.Equal(expectedSqlString, actualSqlString);
        }
      
        [Fact(DisplayName = "Get SQL query for deleting dim_field_display_settings")]
        public void Test_GetSqlQuery_Delete_DimFieldDisplaySettings()
        {
            TtvSqlService ttvSqlService = new();
            string expectedSqlString = "DELETE FROM dim_field_display_settings WHERE dim_user_profile_id=887766";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimFieldDisplaySettings(887766);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting br_granted_permissions")]
        public void Test_GetSqlQuery_Delete_BrGrantedPermissions()
        {
            TtvSqlService ttvSqlService = new();
            string expectedSqlString = "DELETE FROM br_granted_permissions WHERE dim_user_profile_id=998877";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_BrGrantedPermissions(998877);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting dim_user_choices")]
        public void Test_GetSqlQuery_Delete_DimUserChoices()
        {
            TtvSqlService ttvSqlService = new();
            string expectedSqlString = "DELETE FROM dim_user_choices WHERE dim_user_profile_id=119988";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimUserChoices(119988);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting dim_user_profile")]
        public void Test_GetSqlQuery_Delete_DimUserProfile()
        {
            TtvSqlService ttvSqlService = new();
            string expectedSqlString = "DELETE FROM dim_user_profile WHERE id=221199";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimUserProfile(221199);
            Assert.Equal(expectedSqlString, actualSqlString);
        }
    }
}