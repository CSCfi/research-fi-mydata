using api.Models.ProfileEditor;
using api.Models;

namespace api.Services
{
    /*
     * TtvSqlService implements utilities for constructing TTV database SQL queries.
     */
    public class TtvSqlService
    {
        public TtvSqlService()
        {
        }

        // Based on field identifier, return FactFieldValues foreign key column name.
        public string getFactFieldValuesFKColumnNameFromFieldIdentifier(int fieldIdentifier)
        {
            var fk_column_name = "";
            switch (fieldIdentifier)
            {
                case Constants.FieldIdentifiers.PERSON_FIRST_NAMES:
                    fk_column_name = "dim_name_id";
                    break;
                case Constants.FieldIdentifiers.PERSON_LAST_NAME:
                    fk_column_name = "dim_name_id";
                    break;
                case Constants.FieldIdentifiers.PERSON_OTHER_NAMES:
                    fk_column_name = "dim_name_id";
                    break;
                case Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION:
                    fk_column_name = "dim_researcher_description_id";
                    break;
                case Constants.FieldIdentifiers.PERSON_WEB_LINK:
                    fk_column_name = "dim_web_link_id";
                    break;
                case Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS:
                    fk_column_name = "dim_email_addrress_id";
                    break;
                case Constants.FieldIdentifiers.PERSON_KEYWORD:
                    fk_column_name = "dim_keyword_id";
                    break;
                case Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER:
                    fk_column_name = "dim_telephone_number_id";
                    break;
                case Constants.FieldIdentifiers.ACTIVITY_AFFILIATION:
                    fk_column_name = "dim_affiliation_id";
                    break;
                case Constants.FieldIdentifiers.ACTIVITY_EDUCATION:
                    fk_column_name = "dim_education_id";
                    break;
                case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION:
                    fk_column_name = "dim_publication_id";
                    break;
                case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID:
                    fk_column_name = "dim_orcid_publication_id";
                    break;
                case Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION:
                    fk_column_name = "dim_funding_decision_id";
                    break;
                case Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET:
                    fk_column_name = "dim_research_dataset_id";
                    break;
                default:
                    break;
            }
            return fk_column_name;
        }

        // Return SQL update statement for updating FactFieldValues.
        public string getSqlQuery_Update_FactFieldValues(int dimUserProfileId, ProfileEditorItemMeta profileEditorItemMeta)
        {
            var fk_column_name = this.getFactFieldValuesFKColumnNameFromFieldIdentifier(profileEditorItemMeta.Type);
            var showToSql = profileEditorItemMeta.Show == true ? "1" : "0";
            var primaryValueToSql = profileEditorItemMeta.PrimaryValue == true ? "1" : "0";

            return $@"UPDATE fact_field_values
                        SET
                            show={showToSql},
                            primary_value={primaryValueToSql},
                            modified=GETDATE()
                        WHERE
                            dim_user_profile_id={dimUserProfileId} AND
                            {fk_column_name}={profileEditorItemMeta.Id}";
        }
    }
}