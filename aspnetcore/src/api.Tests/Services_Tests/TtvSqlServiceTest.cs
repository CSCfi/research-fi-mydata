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
                DimReferencedataFieldOfScienceId = -1,
                DimFundingDecisionId = -1,
                DimIdentifierlessDataId = -1,
                DimKeywordId = -1,
                DimNameId = -1,
                DimProfileOnlyPublicationId = -1,
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
                DimWebLinkId = -1,
                DimReferencedataActorRoleId = -1
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

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_profile_only_publication_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_profile_only_publication_id()
        {
            TtvSqlService ttvSqlService = new();
            // Web link
            Assert.Equal(
                "dim_profile_only_publication_id", ttvSqlService.GetFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY)
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

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_research_activity_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_research_activity_id()
        {
            TtvSqlService ttvSqlService = new();
            // Research activity
            Assert.Equal(
                "dim_research_activity_id", ttvSqlService.GetFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.ACTIVITY_RESEARCH_ACTIVITY)
            );
        }

        [Fact(DisplayName = "Test that list of integers is converted to a comma separated string")]
        public void Test_ConvertListOfIntsToCommaSeparatedString()
        {
            // Arrange
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
            // Act
            string actualString = ttvSqlService.ConvertListOfIntsToCommaSeparatedString(listOfInts);
            // Assert
            Assert.Equal(
                expectedString,
                actualString
            );
        }

        [Fact(DisplayName = "Get SQL query for updating FactFieldValues, first name")]
        public void Test_getSqlQuery_Update_FactFieldValues_first_name()
        {
            // Arrange
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
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Update_FactFieldValues(userProfileId, profileEditorItemMeta);
            // Assert
            Assert.Equal(
                expectedSqlString.Replace("\n", String.Empty).Replace(" ", String.Empty),
                actualSqlString.Replace("\n", String.Empty).Replace(" ", String.Empty)
            );
        }

        [Fact(DisplayName = "Get SQL query for updating FactFieldValues, researcher description")]
        public void Test_getSqlQuery_Update_FactFieldValues_researcher_description()
        {
            // Arrange
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
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Update_FactFieldValues(userProfileId, profileEditorItemMeta);
            // Assert
            Assert.Equal(
                expectedSqlString.Replace("\n", String.Empty).Replace(" ", String.Empty),
                actualSqlString.Replace("\n", String.Empty).Replace(" ", String.Empty)
            );
        }





        [Fact(DisplayName = "Get SQL query for selecting adding TTV data, email")]
        public void Test_getSqlQuery_Select_DimEmailAddrress()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            int dimKnownPersonId = 9999;
            List<int> existingIds = new() { 111, 222, 333 };
            string expectedSqlString =
                    @"SELECT id as 'Id', dim_registered_data_source_id AS 'DimRegisteredDataSourceId'
                        FROM dim_email_addrress
                        WHERE dim_known_person_id=9999 AND id!=-1 AND dim_registered_data_source_id!=-1 AND id NOT IN (111,222,333)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Select_DimEmailAddrress(dimKnownPersonId, existingIds);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for selecting adding TTV data, researcher description")]
        public void Test_getSqlQuery_Select_DimResearcherDescription()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            int dimKnownPersonId = 9998;
            List<int> existingIds = new() { 222, 333, 444 };
            string expectedSqlString =
                    @"SELECT id as 'Id', dim_registered_data_source_id AS 'DimRegisteredDataSourceId'
                        FROM dim_researcher_description
                        WHERE dim_known_person_id=9998 AND id!=-1 AND dim_registered_data_source_id!=-1 AND id NOT IN (222,333,444)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Select_DimResearcherDescription(dimKnownPersonId, existingIds);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for selecting adding TTV data, web link")]
        public void Test_getSqlQuery_Select_DimWebLink()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            int dimKnownPersonId = 9997;
            List<int> existingIds = new() { 333, 444, 555 };
            string expectedSqlString =
                    @"SELECT id as 'Id', dim_registered_data_source_id AS 'DimRegisteredDataSourceId'
                        FROM dim_web_link
                        WHERE dim_known_person_id=9997 AND id!=-1 AND dim_registered_data_source_id!=-1 AND dim_registered_data_source_id IS NOT NULL AND id NOT IN (333,444,555)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Select_DimWebLink(dimKnownPersonId, existingIds);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for selecting adding TTV data, telephone number")]
        public void Test_getSqlQuery_Select_DimTelephoneNumber()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            int dimKnownPersonId = 9996;
            List<int> existingIds = new() { 444, 555, 666 };
            string expectedSqlString =
                    @"SELECT id as 'Id', dim_registered_data_source_id AS 'DimRegisteredDataSourceId'
                        FROM dim_telephone_number
                        WHERE dim_known_person_id=9996 AND id!=-1 AND dim_registered_data_source_id!=-1 AND id NOT IN (444,555,666)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Select_DimTelephoneNumber(dimKnownPersonId, existingIds);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for selecting adding TTV data, affiliation")]
        public void Test_getSqlQuery_Select_DimAffiliation()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            int dimKnownPersonId = 9995;
            List<int> existingIds = new() { 555, 666, 777 };
            string expectedSqlString =
                    @"SELECT id as 'Id', dim_registered_data_source_id AS 'DimRegisteredDataSourceId'
                        FROM dim_affiliation
                        WHERE dim_known_person_id=9995 AND id!=-1 AND dim_registered_data_source_id!=-1 AND id NOT IN (555,666,777)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Select_DimAffiliation(dimKnownPersonId, existingIds);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for selecting adding TTV data, education")]
        public void Test_getSqlQuery_Select_DimEducation()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            int dimKnownPersonId = 9994;
            List<int> existingIds = new() { 666, 777, 888 };
            string expectedSqlString =
                    @"SELECT id as 'Id', dim_registered_data_source_id AS 'DimRegisteredDataSourceId'
                        FROM dim_education
                        WHERE dim_known_person_id=9994 AND id!=-1 AND dim_registered_data_source_id!=-1 AND id NOT IN (666,777,888)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Select_DimEducation(dimKnownPersonId, existingIds);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }



        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, affiliation")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_affiliation()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 12, 23, 34};
            string expectedSqlString = "DELETE FROM dim_affiliation WHERE id IN (12,23,34)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimAffiliations(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, competence")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_competence()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 23, 34, 45 };
            string expectedSqlString = "DELETE FROM dim_competence WHERE id IN (23,34,45)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimCompetences(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, education")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_education()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 34, 45, 56 };
            string expectedSqlString = "DELETE FROM dim_education WHERE id IN (34,45,56)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimEducations(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, email")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_email()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 45, 56, 67 };
            string expectedSqlString = "DELETE FROM dim_email_addrress WHERE id IN (45,56,67)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimEmailAddrresses(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, event")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_event()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 56, 67, 78 };
            string expectedSqlString = "DELETE FROM dim_event WHERE id IN (56,67,78)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimEvents(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, field of science")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_field_of_science()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 67, 78, 89 };
            string expectedSqlString = "DELETE FROM dim_field_of_science WHERE id IN (67,78,89)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimFieldsOfScience(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, funding decision")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_funding_decision()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 78, 89, 90 };
            string expectedSqlString = "DELETE FROM dim_funding_decision WHERE id IN (78,89,90)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimFundingDecisions(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, keyword")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_keyword()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 100, 101, 102 };
            string expectedSqlString = "DELETE FROM dim_keyword WHERE id IN (100,101,102)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimKeyword(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, name")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_name()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 101, 102, 103 };
            string expectedSqlString = "DELETE FROM dim_name WHERE id IN (101,102,103)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimNames(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, profile only publication")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_profile_only_publication()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 102, 103, 104 };
            string expectedSqlString = "DELETE FROM dim_profile_only_publication WHERE id IN (102,103,104)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimProfileOnlyPublications(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, pid")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_pid()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 103, 104, 105 };
            string expectedSqlString = "DELETE FROM dim_pid WHERE id IN (103,104,105)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimPids(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, research activity")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_research_activity()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 104, 105, 106 };
            string expectedSqlString = "DELETE FROM dim_research_activity WHERE id IN (104,105,106)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimResearchActivities(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, research community")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_research_community()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 105, 106, 107 };
            string expectedSqlString = "DELETE FROM dim_research_community WHERE id IN (105,106,107)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimResearchCommunities(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, research dataset")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_research_dataset()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 106, 107, 108 };
            string expectedSqlString = "DELETE FROM dim_research_dataset WHERE id IN (106,107,108)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimResearchDatasets(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, researcher description")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_researcher_description()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 107, 108, 109 };
            string expectedSqlString = "DELETE FROM dim_researcher_description WHERE id IN (107,108,109)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimResearchDescriptions(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, researcher to research community")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_researcher_to_research_community()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 108, 109, 110 };
            string expectedSqlString = "DELETE FROM dim_researcher_to_research_community WHERE id IN (108,109,110)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimResearcherToResearchCommunities(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, telephone number")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_telephone_number()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 109, 110, 111 };
            string expectedSqlString = "DELETE FROM dim_telephone_number WHERE id IN (109,110,111)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimTelephoneNumbers(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, web link")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_web_link()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            List<int> ids = new() { 110, 111, 112 };
            string expectedSqlString = "DELETE FROM dim_telephone_number WHERE id IN (110,111,112)";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimTelephoneNumbers(ids);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }
        
        [Fact(DisplayName = "Get SQL query for selecting fact_field_values")]
        public void Test_GetSqlQuery_Select_FactFieldValues()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            string expectedSqlString = "SELECT * FROM fact_field_values WHERE dim_user_profile_id=332211";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Select_FactFieldValues(332211);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting fact_field_values")]
        public void Test_GetSqlQuery_Delete_FactFieldValues()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            string expectedSqlString = "DELETE FROM fact_field_values WHERE dim_user_profile_id=443322";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValues(443322);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting dim_identifierless_data children")]
        public void Test_GetSqlQuery_Delete_DimIdentifierlessData_Children()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            string expectedSqlString = "DELETE FROM dim_identifierless_data where dim_identifierless_data_id=554433";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimIdentifierlessData_Children(554433);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting dim_identifierless_data parent")]
        public void Test_GetSqlQuery_Delete_DimIdentifierlessData_Parent()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            string expectedSqlString = "DELETE FROM dim_identifierless_data where id=665544";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimIdentifierlessData_Parent(665544);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }
      
        [Fact(DisplayName = "Get SQL query for deleting dim_field_display_settings")]
        public void Test_GetSqlQuery_Delete_DimFieldDisplaySettings()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            string expectedSqlString = "DELETE FROM dim_field_display_settings WHERE dim_user_profile_id=887766";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimFieldDisplaySettings(887766);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting br_granted_permissions")]
        public void Test_GetSqlQuery_Delete_BrGrantedPermissions()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            string expectedSqlString = "DELETE FROM br_granted_permissions WHERE dim_user_profile_id=998877";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_BrGrantedPermissions(998877);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting dim_user_choices")]
        public void Test_GetSqlQuery_Delete_DimUserChoices()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            string expectedSqlString = "DELETE FROM dim_user_choices WHERE dim_user_profile_id=119988";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimUserChoices(119988);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting dim_user_profile")]
        public void Test_GetSqlQuery_Delete_DimUserProfile()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            string expectedSqlString = "DELETE FROM dim_user_profile WHERE id=221199";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_DimUserProfile(221199);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for counting number of published items in userprofile")]
        public void GetSqlQuery_Select_CountPublishedItemsInUserprofile()
        {
            // Arrange
            TtvSqlService ttvSqlService = new();
            string expectedSqlString = $@"SELECT COUNT(ffv.show) AS 'PublishedCount'
                        FROM fact_field_values AS ffv
                        JOIN dim_user_profile AS dup ON ffv.dim_user_profile_id=dup.id
                        WHERE dup.id=335577 AND ffv.show=1";
            // Act
            string actualSqlString = ttvSqlService.GetSqlQuery_Select_CountPublishedItemsInUserprofile(335577);
            // Assert
            Assert.Equal(expectedSqlString, actualSqlString);
        }
    }
}