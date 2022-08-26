using Xunit;
using api.Services;
using api.Models;
using api.Models.Ttv;
using api.Models.ProfileEditor;
using System;

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
            // First names
            Assert.Equal(
                "dim_name_id", ttvSqlService.GetFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.PERSON_FIRST_NAMES)
            );
            // Last name
            Assert.Equal(
                "dim_name_id", ttvSqlService.GetFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.PERSON_LAST_NAME)
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

        [Fact(DisplayName = "Get SQL query for updating FactFieldValues, first name")]
        public void Test_getSqlQuery_Update_FactFieldValues_first_name()
        {
            TtvSqlService ttvSqlService = new();
            int userProfileId = 80;
            ProfileEditorItemMeta profileEditorItemMeta = new()
            {
                Id = 321,
                Type = Constants.FieldIdentifiers.PERSON_FIRST_NAMES,
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
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimAffiliationId = 1234;
            string expectedSqlString = "DELETE FROM dim_affiliation WHERE id=1234";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, competence")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_competence()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimCompetenceId = 2345;
            string expectedSqlString = "DELETE FROM dim_competence WHERE id=2345";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, education")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_education()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimEducationId = 3456;
            string expectedSqlString = "DELETE FROM dim_education WHERE id=3456";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, email")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_email()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimEmailAddrressId = 4567;
            string expectedSqlString = "DELETE FROM dim_email_addrress WHERE id=4567";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, event")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_event()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimEventId = 5678;
            string expectedSqlString = "DELETE FROM dim_event WHERE id=5678";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, field of science")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_field_of_science()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimFieldOfScienceId = 6789;
            string expectedSqlString = "DELETE FROM dim_field_of_science WHERE id=6789";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, funding decision")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_funding_decision()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimFundingDecisionId = 7890;
            string expectedSqlString = "DELETE FROM dim_funding_decision WHERE id=7890";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, keyword")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_keyword()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimKeywordId = 9012;
            string expectedSqlString = "DELETE FROM dim_keyword WHERE id=9012";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, name")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_name()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimNameId = 5566;
            string expectedSqlString = "DELETE FROM dim_name WHERE id=5566";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, ORCID publication")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_ORCID_publication()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimOrcidPublicationId = 12345;
            string expectedSqlString = "DELETE FROM dim_orcid_publication WHERE id=12345";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, pid")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_pid()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimPidId = 23456;
            string expectedSqlString = "DELETE FROM dim_pid WHERE id=23456";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, pid ORCID putcode")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_pid_ORCID_putcode()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimPidIdOrcidPutCode = 776655;
            string expectedSqlString = "DELETE FROM dim_pid WHERE id=776655";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, publication")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_publication()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimPublicationId = 34567;
            string expectedSqlString = "DELETE FROM dim_publication WHERE id=34567";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, research activity")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_research_activity()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimResearchActivityId = 45678;
            string expectedSqlString = "DELETE FROM dim_research_activity WHERE id=45678";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, research community")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_research_community()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimResearchCommunityId = 56789;
            string expectedSqlString = "DELETE FROM dim_research_community WHERE id=56789";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, research dataset")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_research_dataset()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimResearchDatasetId = 67890;
            string expectedSqlString = "DELETE FROM dim_research_dataset WHERE id=67890";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, researcher description")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_researcher_description()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimResearcherDescriptionId = 78901;
            string expectedSqlString = "DELETE FROM dim_researcher_description WHERE id=78901";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, researcher to research community")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_researcher_to_research_community()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimResearcherToResearchCommunityId = 887766;
            string expectedSqlString = "DELETE FROM dim_researcher_to_research_community WHERE id=887766";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, telephone number")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_telephone_number()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimTelephoneNumberId = 89012;
            string expectedSqlString = "DELETE FROM dim_telephone_number WHERE id=89012";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }

        [Fact(DisplayName = "Get SQL query for deleting FactFieldValues related data, web link")]
        public void Test_getSqlQuery_Delete_FactFieldValues_related_web_link()
        {
            TtvSqlService ttvSqlService = new();
            FactFieldValue ffv = GetFactFieldValueForTest();
            ffv.DimWebLinkId = 90123;
            string expectedSqlString = "DELETE FROM dim_web_link WHERE id=90123";
            string actualSqlString = ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(ffv);
            Assert.Equal(expectedSqlString, actualSqlString);
        }
    }
}