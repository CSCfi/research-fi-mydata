using Xunit;
using api.Services;
using api.Models;
using api.Models.ProfileEditor;
using System.Text.RegularExpressions;
using System;

namespace api.Tests
{
    [Collection("Get SQL query from TtvSqlService")]
    public class TtvSqlServiceTests
    {
        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_name_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_name_id()
        {
            var ttvSqlService = new TtvSqlService();
            // First names
            Assert.Equal(
                "dim_name_id", ttvSqlService.getFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.PERSON_FIRST_NAMES)
            );
            // Last name
            Assert.Equal(
                "dim_name_id", ttvSqlService.getFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.PERSON_LAST_NAME)
            );
            // Other names
            Assert.Equal(
                "dim_name_id", ttvSqlService.getFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.PERSON_OTHER_NAMES)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_researcher_description_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_researcher_description_id()
        {
            var ttvSqlService = new TtvSqlService();
            // Researcer description
            Assert.Equal(
                "dim_researcher_description_id", ttvSqlService.getFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_web_link_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_web_link_id()
        {
            var ttvSqlService = new TtvSqlService();
            // Web link
            Assert.Equal(
                "dim_web_link_id", ttvSqlService.getFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.PERSON_WEB_LINK)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_email_addrress_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_email_addrress_id()
        {
            var ttvSqlService = new TtvSqlService();
            // Web link
            Assert.Equal(
                "dim_email_addrress_id", ttvSqlService.getFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_keyword_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_keyword_id()
        {
            var ttvSqlService = new TtvSqlService();
            // Web link
            Assert.Equal(
                "dim_keyword_id", ttvSqlService.getFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.PERSON_KEYWORD)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_telephone_number_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_telephone_number_id()
        {
            var ttvSqlService = new TtvSqlService();
            // Web link
            Assert.Equal(
                "dim_telephone_number_id", ttvSqlService.getFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_affiliation_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_affiliation_id()
        {
            var ttvSqlService = new TtvSqlService();
            // Web link
            Assert.Equal(
                "dim_affiliation_id", ttvSqlService.getFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.ACTIVITY_AFFILIATION)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_education_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_education_id()
        {
            var ttvSqlService = new TtvSqlService();
            // Web link
            Assert.Equal(
                "dim_education_id", ttvSqlService.getFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.ACTIVITY_EDUCATION)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_publication_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_publication_id()
        {
            var ttvSqlService = new TtvSqlService();
            // Web link
            Assert.Equal(
                "dim_publication_id", ttvSqlService.getFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.ACTIVITY_PUBLICATION)
            );
        }

        [Fact(DisplayName = "Get FactFieldValues FK column name - dim_orcid_publication_id")]
        public void getFactFieldValuesFKColumnNameFromFieldIdentifier_dim_orcid_publication_id()
        {
            var ttvSqlService = new TtvSqlService();
            // Web link
            Assert.Equal(
                "dim_orcid_publication_id", ttvSqlService.getFactFieldValuesFKColumnNameFromFieldIdentifier(Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID)
            );
        }

        [Fact(DisplayName = "Get SQL query for updating FactFieldValues, first name")]
        public void Test_getSqlQuery_Update_FactFieldValues_first_name()
        {
            var ttvSqlService = new TtvSqlService();
            var userProfileId = 80;
            var profileEditorItemMeta = new ProfileEditorItemMeta()
            {
                Id = 321,
                Type = Constants.FieldIdentifiers.PERSON_FIRST_NAMES,
                PrimaryValue = false,
                Show = true
            };

            var expectedSqlString = @"UPDATE fact_field_values
                                SET
                                    show=1,
                                    primary_value=0,
                                    modified=GETDATE()
                                WHERE
                                    dim_user_profile_id=80 AND
                                    dim_name_id=321";
            var actualSqlString = ttvSqlService.getSqlQuery_Update_FactFieldValues(userProfileId, profileEditorItemMeta);

            Assert.Equal(
                expectedSqlString.Replace("\n", String.Empty).Replace(" ", String.Empty),
                actualSqlString.Replace("\n", String.Empty).Replace(" ", String.Empty)
            );
        }

        [Fact(DisplayName = "Get SQL query for updating FactFieldValues, researcher description")]
        public void Test_getSqlQuery_Update_FactFieldValues_researcher_description()
        {
            var ttvSqlService = new TtvSqlService();
            var userProfileId = 5678;
            var profileEditorItemMeta = new ProfileEditorItemMeta()
            {
                Id = 254,
                Type = Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION,
                PrimaryValue = true,
                Show = false
            };

            var expectedSqlString = @"UPDATE fact_field_values
                                SET
                                    show=0,
                                    primary_value=1,
                                    modified=GETDATE()
                                WHERE
                                    dim_user_profile_id=5678 AND
                                    dim_researcher_description_id=254";
            var actualSqlString = ttvSqlService.getSqlQuery_Update_FactFieldValues(userProfileId, profileEditorItemMeta);

            Assert.Equal(
                expectedSqlString.Replace("\n", String.Empty).Replace(" ", String.Empty),
                actualSqlString.Replace("\n", String.Empty).Replace(" ", String.Empty)
            );
        }
    }
}