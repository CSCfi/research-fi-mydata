using api.Models.ProfileEditor;
using api.Models;
using api.Models.Ttv;

namespace api.Services
{
    /*
     * TtvSqlService implements utilities for constructing TTV database SQL queries.
     */
    public class TtvSqlService : ITtvSqlService
    {
        public TtvSqlService()
        {
        }

        // Based on field identifier, return FactFieldValues foreign key column name.
        public string GetFactFieldValuesFKColumnNameFromFieldIdentifier(int fieldIdentifier)
        {
            string fk_column_name = "";
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
        public string GetSqlQuery_Update_FactFieldValues(int dimUserProfileId, ProfileEditorItemMeta profileEditorItemMeta)
        {
            string fk_column_name = GetFactFieldValuesFKColumnNameFromFieldIdentifier(profileEditorItemMeta.Type);
            string showToSql = profileEditorItemMeta.Show == true ? "1" : "0";
            string primaryValueToSql = profileEditorItemMeta.PrimaryValue == true ? "1" : "0";

            return $@"UPDATE fact_field_values
                        SET
                            show={showToSql},
                            primary_value={primaryValueToSql},
                            modified=GETDATE()
                        WHERE
                            dim_user_profile_id={dimUserProfileId} AND
                            {fk_column_name}={profileEditorItemMeta.Id}";
        }

        // Return SQL statement for getting profile data
        public string GetSqlQuery_ProfileData(int userprofileId)
        {
            return $@"
                SELECT
                    dfds.id AS 'DimFieldDisplaySettings_Id',
                    dfds.field_identifier AS 'DimFieldDisplaySettings_FieldIdentifier',
                    dfds.show AS 'DimFieldDisplaySettings_Show',
                    ffv.show AS 'FactFieldValues_Show',
                    ffv.primary_value AS 'FactFieldValues_PrimaryValue',
                    drds.id AS 'DimRegisteredDataSource_Id',
                    drds.name AS 'DimRegisteredDataSource_Name',
                    drds_organization.name_fi AS 'DimRegisteredDataSource_DimOrganization_NameFi',
                    drds_organization.name_en AS 'DimRegisteredDataSource_DimOrganization_NameEn',
                    drds_organization.name_sv AS 'DimRegisteredDataSource_DimOrganization_NameSv',
                    ffv.dim_user_profile_id AS 'FactFieldValues_DimUserProfileId',
                    ffv.dim_name_id AS 'FactFieldValues_DimNameId',
                    ffv.dim_web_link_id AS 'FactFieldValues_DimWebLinkId',
                    ffv.dim_researcher_description_id AS 'FactFieldValues_DimResearcherDescriptionId',
                    ffv.dim_email_addrress_id AS 'FactFieldValues_DimEmailAddrressId',
                    ffv.dim_telephone_number_id AS 'FactFieldValues_DimTelephoneNumberId',
                    ffv.dim_field_of_science_id AS ' FactFieldValues_DimFieldOfScienceId',
                    ffv.dim_keyword_id AS 'FactFieldValues_DimKeywordId',
                    ffv.dim_pid_id AS 'FactFieldValues_DimPidId',
                    ffv.dim_affiliation_id AS 'FactFieldValues_DimAffiliationId',
                    ffv.dim_identifierless_data_id AS 'FactFieldValues_DimIdentifierlessDataId',
                    ffv.dim_education_id AS 'FactFieldValues_DimEducationId',
                    ffv.dim_publication_id AS 'FactFieldValues_DimPublicationId',
                    ffv.dim_orcid_publication_id AS 'FactFieldValues_DimOrcidPublicationId',
                    dim_name.lASt_name AS 'DimName_LastName',
                    dim_name.first_names AS 'DimName_FirstNames',
                    dim_name.full_name AS 'DimName_FullName',
                    dim_web_link.url AS 'DimWebLink_Url',
                    dim_web_link.link_label AS 'DimWebLink_LinkLabel',
                    dim_researcher_description.research_description_fi AS 'DimResearcherDescription_ResearchDescriptionFi',
                    dim_researcher_description.research_description_en AS 'DimResearcherDescription_ResearchDescriptionEn',
                    dim_researcher_description.research_description_sv AS 'DimResearcherDescription_ResearchDescriptionSv',
                    dim_email_addrress.email AS 'DimEmailAddrress_Email',
                    dim_telephone_number.telephone_number AS 'DimTelephoneNumber_TelephoneNumber',
                    dim_field_of_science.name_fi AS 'DimFieldOfScience_NameFi',
                    dim_field_of_science.name_en AS 'DimFieldOfScience_NameEn',
                    dim_field_of_science.name_sv AS 'DimFieldOfScience_NameSv',
                    dim_keyword.keyword AS 'DimKeyword_Keyword',
                    dim_pid.pid_type AS 'DimPid_PidType',
                    dim_pid.pid_content AS 'DimPid_PidContent',
                    affiliation_organization.id AS 'DimAffiliation_DimOrganization_Id',
                    affiliation_organization.name_fi AS 'DimAffiliation_DimOrganization_NameFi',
                    affiliation_organization.name_en AS 'DimAffiliation_DimOrganization_NameEn',
                    affiliation_organization.name_sv AS 'DimAffiliation_DimOrganization_NameSv',
                    dim_affiliation.position_name_fi AS 'DimAffiliation_PositionNameFi',
                    dim_affiliation.position_name_en AS 'DimAffiliation_PositionNameEn',
                    dim_affiliation.position_name_sv AS 'DimAffiliation_PositionNameSv',
                    affiliation_start_date.year AS 'DimAffiliation_StartDate_Year',
                    affiliation_start_date.month AS 'DimAffiliation_StartDate_Month',
                    affiliation_start_date.day AS 'DimAffiliation_StartDate_Day',
                    affiliation_end_date.year AS 'DimAffiliation_EndDate_Year',
                    affiliation_end_date.month AS 'DimAffiliation_EndDate_Month',
                    affiliation_end_date.day AS 'DimAffiliation_EndDate_Day',
                    affiliation_type.name_fi AS 'DimAffiliation_DimReferenceData_NameFi',
                    did.type AS 'DimIdentifierlessData_Type',
                    did.value_fi AS 'DimIdentifierlessData_ValueFi',
                    did.value_en AS 'DimIdentifierlessData_ValueEn',
                    did.value_sv AS 'DimIdentifierlessData_ValueSv',
                    did.unlinked_identifier AS 'DimIdentifierlessData_UnlinkedIdentifier',
                    did_child.type AS 'DimIdentifierlessData_Child_Type',
                    did_child.value_fi AS 'DimIdentifierlessData_Child_ValueFi',
                    did_child.value_en AS 'DimIdentifierlessData_Child_ValueEn',
                    did_child.value_sv AS 'DimIdentifierlessData_Child_ValueSv',
                    did_child.unlinked_identifier AS 'DimIdentifierlessData_Child_UnlinkedIdentifier',
                    dim_education.name_fi AS 'DimEducation_NameFi',
                    dim_education.name_en AS 'DimEducation_NameEn',
                    dim_education.name_sv AS 'DimEducation_NameSv',
                    dim_education.degree_granting_institution_name AS 'DimEducation_DegreeGrantingInstitutionName',
                    education_start_date.year AS 'DimEducation_StartDate_Year',
                    education_start_date.month AS 'DimEducation_StartDate_Month',
                    education_start_date.day AS 'DimEducation_StartDate_Day',
                    education_end_date.year AS 'DimEducation_EndDate_Year',
                    education_end_date.month AS 'DimEducation_EndDate_Month',
                    education_end_date.day AS 'DimEducation_EndDate_Day',
                    dim_publication.publication_id AS 'DimPublication_PublicationId',
                    dim_publication.publication_name AS 'DimPublication_PublicationName',
                    dim_publication.publication_year AS 'DimPublication_PublicationYear',
                    dim_publication.doi AS 'DimPublication_Doi',
                    dim_publication.publication_type_code AS 'DimPublication_PublicationTypeCode',
                    dim_orcid_publication.publication_id AS 'DimOrcidPublication_PublicationId',
                    dim_orcid_publication.publication_name AS 'DimOrcidPublication_PublicationName',
                    dim_orcid_publication.publication_year AS 'DimOrcidPublication_PublicationYear',
                    dim_orcid_publication.doi_handle AS 'DimOrcidPublication_Doi'

                FROM fact_field_values AS ffv

                JOIN dim_field_display_settings AS dfds ON ffv.dim_field_display_settings_id=dfds.id
                JOIN dim_registered_data_source AS drds ON ffv.dim_registered_data_source_id=drds.id
                JOIN dim_organization AS drds_organization ON drds.dim_organization_id=drds_organization.id
                JOIN dim_name ON ffv.dim_name_id=dim_name.id
                JOIN dim_web_link ON ffv.dim_web_link_id=dim_web_link.id
                JOIN dim_researcher_description ON ffv.dim_researcher_description_id=dim_researcher_description.id
                JOIN dim_email_addrress ON ffv.dim_email_addrress_id=dim_email_addrress.id
                JOIN dim_telephone_number ON ffv.dim_telephone_number_id=dim_telephone_number.id
                JOIN dim_field_of_science ON ffv.dim_field_of_science_id=dim_field_of_science.id
                JOIN dim_keyword ON ffv.dim_keyword_id=dim_keyword.id
                JOIN dim_pid ON ffv.dim_pid_id=dim_pid.id
                JOIN dim_affiliation ON ffv.dim_affiliation_id=dim_affiliation.id
                JOIN dim_organization AS affiliation_organization ON dim_affiliation.dim_organization_id=affiliation_organization.id
                LEFT JOIN dim_date AS affiliation_start_date ON dim_affiliation.start_date=affiliation_start_date.id AND affiliation_start_date.id!=-1
                LEFT JOIN dim_date AS affiliation_end_date ON dim_affiliation.end_date=affiliation_end_date.id AND affiliation_end_date.id!=-1
                JOIN dim_referencedata AS affiliation_type ON dim_affiliation.affiliation_type=affiliation_type.id
                JOIN dim_identifierless_data AS did ON ffv.dim_identifierless_data_id=did.id
                LEFT JOIN dim_identifierless_data AS did_child ON did_child.dim_identifierless_data_id=did.id AND did_child.dim_identifierless_data_id!=-1
                JOIN dim_education ON ffv.dim_education_id=dim_education.id
                LEFT JOIN dim_date AS education_start_date ON dim_education.dim_start_date=education_start_date.id AND education_start_date.id!=-1
                LEFT JOIN dim_date AS education_end_date ON dim_education.dim_end_date=education_end_date.id AND education_end_date.id!=-1
                JOIN dim_publication ON ffv.dim_publication_id=dim_publication.id
                JOIN dim_orcid_publication ON ffv.dim_orcid_publication_id=dim_orcid_publication.id

                WHERE
                    ffv.dim_user_profile_id={userprofileId} AND
                    (
                        ffv.dim_name_id != -1 OR
                        ffv.dim_web_link_id != -1 OR
                        ffv.dim_researcher_description_id != -1 OR
                        ffv.dim_email_addrress_id != -1 OR
                        ffv.dim_telephone_number_id != -1 OR
                        ffv.dim_field_of_science_id != -1 OR
                        ffv.dim_keyword_id != -1 OR
                        ffv.dim_pid_id != -1 OR
                        ffv.dim_affiliation_id != -1 OR
                        ffv.dim_identifierless_data_id != -1 OR
                        ffv.dim_education_id != -1 OR
                        ffv.dim_publication_id != -1 OR
                        ffv.dim_orcid_publication_id != -1
                    )
                ";
        }


        public string GetSqlQuery_Delete_FactFieldValueRelatedData(FactFieldValue ffv)
        {
            if (ffv.DimAffiliationId != -1) return $"DELETE FROM dim_affiliation WHERE id={ffv.DimAffiliationId}";
            if (ffv.DimCompetenceId != -1) return $"DELETE FROM dim_competence WHERE id={ffv.DimCompetenceId}";
            if (ffv.DimEducationId != -1) return $"DELETE FROM dim_education WHERE id={ffv.DimEducationId}";
            if (ffv.DimEmailAddrressId != -1) return $"DELETE FROM dim_email_addrress WHERE id={ffv.DimEmailAddrressId}";
            if (ffv.DimEventId != -1) return $"DELETE FROM dim_event WHERE id={ffv.DimEventId}";
            if (ffv.DimFieldOfScienceId != -1) return $"DELETE FROM dim_field_of_science WHERE id={ffv.DimFieldOfScienceId}";
            if (ffv.DimFundingDecisionId != -1) return $"DELETE FROM dim_funding_decision WHERE id={ffv.DimFundingDecisionId}";
            if (ffv.DimIdentifierlessDataId != -1) return $"DELETE FROM dim_identifierless_data WHERE id={ffv.DimIdentifierlessDataId}";
            if (ffv.DimKeywordId != -1) return $"DELETE FROM dim_keyword WHERE id={ffv.DimKeywordId}";
            if (ffv.DimNameId != -1) return $"DELETE FROM dim_name WHERE id={ffv.DimNameId}";
            if (ffv.DimOrcidPublicationId != -1) return $"DELETE FROM dim_orcid_publication WHERE id={ffv.DimOrcidPublicationId}";
            if (ffv.DimPidId != -1) return $"DELETE FROM dim_pid WHERE id={ffv.DimPidId}";
            if (ffv.DimPublicationId != -1) return $"DELETE FROM dim_publication WHERE id={ffv.DimPublicationId}";
            if (ffv.DimResearchActivityId != -1) return $"DELETE FROM dim_research_activity WHERE id={ffv.DimResearchActivityId}";
            if (ffv.DimResearchCommunityId != -1) return $"DELETE FROM dim_research_community WHERE id={ffv.DimResearchCommunityId}";
            if (ffv.DimResearchDatasetId != -1) return $"DELETE FROM dim_research_dataset WHERE id={ffv.DimResearchDatasetId}";
            if (ffv.DimResearcherDescriptionId != -1) return $"DELETE FROM dim_researcher_description WHERE id={ffv.DimResearcherDescriptionId}";
            if (ffv.DimTelephoneNumberId != -1) return $"DELETE FROM dim_telephone_number WHERE id={ffv.DimTelephoneNumberId}";
            if (ffv.DimWebLinkId != -1) return $"DELETE FROM dim_web_link WHERE id={ffv.DimWebLinkId}";
            return "";
        }
    }
}