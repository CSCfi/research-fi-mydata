using api.Models.ProfileEditor.Items;
using api.Models.Common;
using System.Collections.Generic;

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

        // Based on item meta type, return FactFieldValues foreign key column name.
        public string GetFactFieldValuesFKColumnNameFromItemMetaType(int itemMetaType)
        {
            string fk_column_name = "";
            switch (itemMetaType)
            {
                case Constants.ItemMetaTypes.PERSON_NAME:
                    fk_column_name = "dim_name_id";
                    break;
                case Constants.ItemMetaTypes.PERSON_OTHER_NAMES:
                    fk_column_name = "dim_name_id";
                    break;
                case Constants.ItemMetaTypes.PERSON_RESEARCHER_DESCRIPTION:
                    fk_column_name = "dim_researcher_description_id";
                    break;
                case Constants.ItemMetaTypes.PERSON_WEB_LINK:
                    fk_column_name = "dim_web_link_id";
                    break;
                case Constants.ItemMetaTypes.PERSON_EMAIL_ADDRESS:
                    fk_column_name = "dim_email_addrress_id";
                    break;
                case Constants.ItemMetaTypes.PERSON_KEYWORD:
                    fk_column_name = "dim_keyword_id";
                    break;
                case Constants.ItemMetaTypes.PERSON_TELEPHONE_NUMBER:
                    fk_column_name = "dim_telephone_number_id";
                    break;
                case Constants.ItemMetaTypes.ACTIVITY_AFFILIATION:
                    fk_column_name = "dim_affiliation_id";
                    break;
                case Constants.ItemMetaTypes.ACTIVITY_EDUCATION:
                    fk_column_name = "dim_education_id";
                    break;
                case Constants.ItemMetaTypes.ACTIVITY_PUBLICATION:
                    fk_column_name = "dim_publication_id";
                    break;
                case Constants.ItemMetaTypes.ACTIVITY_PUBLICATION_PROFILE_ONLY:
                    fk_column_name = "dim_profile_only_publication_id";
                    break;
                case Constants.ItemMetaTypes.ACTIVITY_FUNDING_DECISION:
                    fk_column_name = "dim_funding_decision_id";
                    break;
                case Constants.ItemMetaTypes.ACTIVITY_FUNDING_DECISION_PROFILE_ONLY:
                    fk_column_name = "dim_profile_only_funding_decision_id";
                    break;
                case Constants.ItemMetaTypes.ACTIVITY_RESEARCH_DATASET:
                    fk_column_name = "dim_research_dataset_id";
                    break;
                case Constants.ItemMetaTypes.ACTIVITY_RESEARCH_DATASET_PROFILE_ONLY:
                    fk_column_name = "dim_profile_only_dataset_id";
                    break;
                case Constants.ItemMetaTypes.ACTIVITY_RESEARCH_ACTIVITY:
                    fk_column_name = "dim_research_activity_id";
                    break;
                case Constants.ItemMetaTypes.ACTIVITY_RESEARCH_ACTIVITY_PROFILE_ONLY:
                    fk_column_name = "dim_profile_only_research_activity_id";
                    break;
                default:
                    break;
            }
            return fk_column_name;
        }

        // Return SQL update statement for updating FactFieldValues.
        public string GetSqlQuery_Update_FactFieldValues(int dimUserProfileId, ProfileEditorItemMeta profileEditorItemMeta)
        {
            string fk_column_name = GetFactFieldValuesFKColumnNameFromItemMetaType(profileEditorItemMeta.Type);
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

        // Return SQL update statement for setting 'modified' timestamp in userprofile.
        public string GetSqlQuery_Update_DimUserProfile_Modified(int dimUserProfileId)
        {
            return $@"UPDATE dim_user_profile SET modified=GETDATE() WHERE id={dimUserProfileId}";
        }

        // Convert list of integers into a comma separated string
        public string ConvertListOfIntsToCommaSeparatedString(List<int> listOfInts)
        {
            return string.Join<int>(",", listOfInts);
        }

        // Convert list of long integers into a comma separated string
        public string ConvertListOfLongsToCommaSeparatedString(List<long> listOfLongs)
        {
            return string.Join<long>(",", listOfLongs);
        }

        /*
         * Return SQL statement for getting profile settings.
         */
        public string GetSqlQuery_ProfileSettings(int userprofileId)
        {
            return $@"SELECT
                        hidden AS 'Hidden',
                        publish_new_orcid_data AS 'PublishNewData',
                        highlight_openess AS 'HighlightOpeness'
                    FROM dim_user_profile
                    WHERE id={userprofileId}";
        }

        /*
         * Return SQL statement for getting profile editor cooperation items.
         * Queried from dim_user_choices and dim_referencedata tables.
         */
        public string GetSqlQuery_ProfileEditorCooperationItems(int userprofileId)
        {
            return $@"SELECT
                        duc.id AS 'Id',
                        dr.name_fi AS 'NameFi',
                        dr.name_en AS 'NameEn',
                        dr.name_sv AS 'NameSv',
                        duc.user_choice_value AS 'Selected',
                        dr.[order] AS 'Order'
                    FROM dim_user_choices AS duc
                    JOIN dim_referencedata AS dr ON duc.dim_referencedata_id_as_user_choice_label=dr.id
                    WHERE
                        duc.dim_user_profile_id={userprofileId}
                    ORDER BY
                        dr.[order]";
        }

        /*
         * Return SQL statement for getting profile data.
         * Parameter forElasticsearch controls if query should be limited to
         * contain only user's published items.
         */
        public string GetSqlQuery_ProfileData(int userprofileId, bool forElasticsearch = false)
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
                    drds_organization_sector.sector_id AS 'DimRegisteredDataSource_DimOrganization_DimSector_SectorId',
                    ffv.dim_user_profile_id AS 'FactFieldValues_DimUserProfileId',
                    ffv.dim_name_id AS 'FactFieldValues_DimNameId',
                    ffv.dim_web_link_id AS 'FactFieldValues_DimWebLinkId',
                    ffv.dim_researcher_description_id AS 'FactFieldValues_DimResearcherDescriptionId',
                    ffv.dim_email_addrress_id AS 'FactFieldValues_DimEmailAddrressId',
                    ffv.dim_telephone_number_id AS 'FactFieldValues_DimTelephoneNumberId',
                    ffv.dim_referencedata_field_of_science_id AS ' FactFieldValues_DimReferencedataFieldOfScienceId',
                    ffv.dim_keyword_id AS 'FactFieldValues_DimKeywordId',
                    ffv.dim_pid_id AS 'FactFieldValues_DimPidId',
                    ffv.dim_affiliation_id AS 'FactFieldValues_DimAffiliationId',
                    ffv.dim_identifierless_data_id AS 'FactFieldValues_DimIdentifierlessDataId',
                    ffv.dim_education_id AS 'FactFieldValues_DimEducationId',
                    ffv.dim_publication_id AS 'FactFieldValues_DimPublicationId',
                    ffv.dim_profile_only_dataset_id AS 'FactFieldValues_DimProfileOnlyDatasetId',
                    ffv.dim_profile_only_funding_decision_id AS 'FactFieldValues_DimProfileOnlyFundingDecisionId',
                    ffv.dim_profile_only_publication_id AS 'FactFieldValues_DimProfileOnlyPublicationId',
                    ffv.dim_profile_only_research_activity_id AS 'FactFieldValues_DimProfileOnlyResearchActivityId',
                    ffv.dim_research_activity_id AS 'FactFieldValues_DimResearchActivityId',
                    ffv.dim_funding_decision_id AS 'FactFieldValues_DimFundingDecisionId',
                    ffv.dim_research_dataset_id AS 'FactFieldValues_DimResearchDatasetId',
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
                    dim_keyword.keyword AS 'DimKeyword_Keyword',
                    dim_pid.pid_type AS 'DimPid_PidType',
                    dim_pid.pid_content AS 'DimPid_PidContent',

                    affiliation_organization.id AS 'DimAffiliation_DimOrganization_Id',
                    affiliation_organization.organization_id AS 'DimAffiliation_DimOrganization_OrganizationId',
                    affiliation_organization.name_fi AS 'DimAffiliation_DimOrganization_NameFi',
                    affiliation_organization.name_en AS 'DimAffiliation_DimOrganization_NameEn',
                    affiliation_organization.name_sv AS 'DimAffiliation_DimOrganization_NameSv',
                    affiliation_organization_sector.sector_id AS 'DimAffiliation_DimOrganization_DimSector_SectorId',
                    affiliation_organization_sector.name_fi AS 'DimAffiliation_DimOrganization_DimSector_NameFi',
                    affiliation_organization_sector.name_en AS 'DimAffiliation_DimOrganization_DimSector_NameEn',
                    affiliation_organization_sector.name_sv AS 'DimAffiliation_DimOrganization_DimSector_NameSv',
                    affiliation_organization_broader.id AS 'DimAffiliation_DimOrganizationBroader_Id',
                    affiliation_organization_broader.organization_id AS 'DimAffiliation_DimOrganizationBroader_OrganizationId',
                    affiliation_organization_broader.name_fi AS 'DimAffiliation_DimOrganizationBroader_NameFi',
                    affiliation_organization_broader.name_en AS 'DimAffiliation_DimOrganizationBroader_NameEn',
                    affiliation_organization_broader.name_sv AS 'DimAffiliation_DimOrganizationBroader_NameSv',
                    affiliation_organization_broader_sector.sector_id AS 'DimAffiliation_DimOrganizationBroader_DimSector_SectorId',
                    affiliation_organization_broader_sector.name_fi AS 'DimAffiliation_DimOrganizationBroader_DimSector_NameFi',
                    affiliation_organization_broader_sector.name_en AS 'DimAffiliation_DimOrganizationBroader_DimSector_NameEn',
                    affiliation_organization_broader_sector.name_sv AS 'DimAffiliation_DimOrganizationBroader_DimSector_NameSv',
                    dim_affiliation.position_name_fi AS 'DimAffiliation_PositionNameFi',
                    dim_affiliation.position_name_en AS 'DimAffiliation_PositionNameEn',
                    dim_affiliation.position_name_sv AS 'DimAffiliation_PositionNameSv',
                    dim_affiliation.affiliation_type_fi AS 'DimAffiliation_AffiliationTypeFi',
                    dim_affiliation.affiliation_type_en AS 'DimAffiliation_AffiliationTypeEn',
                    dim_affiliation.affiliation_type_sv AS 'DimAffiliation_AffiliationTypeSv',
                    affiliation_start_date.year AS 'DimAffiliation_StartDate_Year',
                    affiliation_start_date.month AS 'DimAffiliation_StartDate_Month',
                    affiliation_start_date.day AS 'DimAffiliation_StartDate_Day',
                    affiliation_end_date.year AS 'DimAffiliation_EndDate_Year',
                    affiliation_end_date.month AS 'DimAffiliation_EndDate_Month',
                    affiliation_end_date.day AS 'DimAffiliation_EndDate_Day',
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
                    dim_publication.article_number_text AS 'DimPublication_ArticleNumberText',
                    dim_publication.authors_text AS 'DimPublication_AuthorsText',
                    dim_publication.conference_name AS 'DimPublication_ConferenceName',
                    publication_doi.pid_content AS 'DimPublication_Doi',
                    dim_publication.issue_number AS 'DimPublication_IssueNumber',
                    dim_publication.journal_name AS 'DimPublication_JournalName',
                    dim_publication.open_access_code AS 'DimPublication_DimReferenceData_Id_OpenAccessCode',
                    dim_publication.page_number_text AS 'DimPublication_PageNumberText',
                    dim_publication.parent_publication_name AS 'DimPublication_ParentPublicationName',
                    dim_publication.peer_reviewed AS 'DimPublication_PeerReviewed',
                    dim_publication.publication_id AS 'DimPublication_PublicationId',
                    dim_publication.publication_name AS 'DimPublication_PublicationName',
                    dim_publication.publication_year AS 'DimPublication_PublicationYear',
                    dim_publication.publisher_name AS 'DimPublication_PublisherName',
                    dim_publication_referencedata_type_code.code_value AS 'DimPublication_PublicationTypeCode',
                    dim_publication.self_archived_code AS 'DimPublication_SelfArchivedCode',
                    dim_publication_locally_reported_pub_info.self_archived_url AS 'DimPublication_SelfArchivedAddress',
                    dim_publication_referencedata_open_access_code.code_value AS 'DimPublication_OpenAccessCodeValue',
                    dim_publication.volume AS 'DimPublication_Volume',

                    dim_profile_only_publication.publication_id AS 'DimProfileOnlyPublication_PublicationId',
                    dim_profile_only_publication.publication_name AS 'DimProfileOnlyPublication_PublicationName',
                    dim_profile_only_publication.publication_year AS 'DimProfileOnlyPublication_PublicationYear',
                    dim_profile_only_publication.doi_handle AS 'DimProfileOnlyPublication_Doi',
                    dim_profile_only_publication.peer_reviewed AS 'DimProfileOnlyPublication_PeerReviewed',
                    dim_profile_only_publication.open_access_code AS 'DimProfileOnlyPublication_OpenAccessCode',

                    profile_only_research_activity_organization.id AS 'DimProfileOnlyResearchActivity_DimOrganization_Id',
                    profile_only_research_activity_organization.organization_id AS 'DimProfileOnlyResearchActivity_DimOrganization_OrganizationId',
                    profile_only_research_activity_organization.name_fi AS 'DimProfileOnlyResearchActivity_DimOrganization_NameFi',
                    profile_only_research_activity_organization.name_en AS 'DimProfileOnlyResearchActivity_DimOrganization_NameEn',
                    profile_only_research_activity_organization.name_sv AS 'DimProfileOnlyResearchActivity_DimOrganization_NameSv',
                    profile_only_research_activity_organization_sector.sector_id AS 'DimProfileOnlyResearchActivity_DimOrganization_DimSector_SectorId',
                    profile_only_research_activity_organization_sector.name_fi AS 'DimProfileOnlyResearchActivity_DimOrganization_DimSector_NameFi',
                    profile_only_research_activity_organization_sector.name_en AS 'DimProfileOnlyResearchActivity_DimOrganization_DimSector_NameEn',
                    profile_only_research_activity_organization_sector.name_sv AS 'DimProfileOnlyResearchActivity_DimOrganization_DimSector_NameSv',
                    profile_only_research_activity_organization_broader.id AS 'DimProfileOnlyResearchActivity_DimOrganizationBroader_Id',
                    profile_only_research_activity_organization_broader.organization_id AS 'DimProfileOnlyResearchActivity_DimOrganizationBroader_OrganizationId',
                    profile_only_research_activity_organization_broader.name_fi AS 'DimProfileOnlyResearchActivity_DimOrganizationBroader_NameFi',
                    profile_only_research_activity_organization_broader.name_en AS 'DimProfileOnlyResearchActivity_DimOrganizationBroader_NameEn',
                    profile_only_research_activity_organization_broader.name_sv AS 'DimProfileOnlyResearchActivity_DimOrganizationBroader_NameSv',
                    profile_only_research_activity_organization_broader_sector.sector_id AS 'DimProfileOnlyResearchActivity_DimOrganizationBroader_DimSector_SectorId',
                    profile_only_research_activity_organization_broader_sector.name_fi AS 'DimProfileOnlyResearchActivity_DimOrganizationBroader_DimSector_NameFi',
                    profile_only_research_activity_organization_broader_sector.name_en AS 'DimProfileOnlyResearchActivity_DimOrganizationBroader_DimSector_NameEn',
                    profile_only_research_activity_organization_broader_sector.name_sv AS 'DimProfileOnlyResearchActivity_DimOrganizationBroader_DimSector_NameSv',
                    dim_profile_only_research_activity.name_fi AS 'DimProfileOnlyResearchActivity_NameFi',
                    dim_profile_only_research_activity.name_en AS 'DimProfileOnlyResearchActivity_NameEn',
                    dim_profile_only_research_activity.name_sv AS 'DimProfileOnlyResearchActivity_NameSv',
                    dim_profile_only_research_activity.description_fi AS 'DimProfileOnlyResearchActivity_DescriptionFi',
                    dim_profile_only_research_activity.description_en AS 'DimProfileOnlyResearchActivity_DescriptionEn',
                    dim_profile_only_research_activity.description_sv AS 'DimProfileOnlyResearchActivity_DescriptionSv',
                    dim_profile_only_research_activity_start_date.year AS 'DimProfileOnlyResearchActivity_StartDate_Year',
                    dim_profile_only_research_activity_start_date.month AS 'DimProfileOnlyResearchActivity_StartDate_Month',
                    dim_profile_only_research_activity_start_date.day AS 'DimProfileOnlyResearchActivity_StartDate_Day',
                    dim_profile_only_research_activity_end_date.year AS 'DimProfileOnlyResearchActivity_EndDate_Year',
                    dim_profile_only_research_activity_end_date.month AS 'DimProfileOnlyResearchActivity_EndDate_Month',
                    dim_profile_only_research_activity_end_date.day AS 'DimProfileOnlyResearchActivity_EndDate_Day',
                    profile_only_research_activity_referencedata.code_value AS 'DimProfileOnlyResearchActivity_ActivityRole_CodeValue',
                    profile_only_research_activity_referencedata.name_fi AS 'DimProfileOnlyResearchActivity_ActivityRole_NameFi',
                    profile_only_research_activity_referencedata.name_en AS 'DimProfileOnlyResearchActivity_ActivityRole_NameEn',
                    profile_only_research_activity_referencedata.name_sv AS 'DimProfileOnlyResearchActivity_ActivityRole_NameSv',
                    dim_profile_only_research_activity_web_link.url AS 'DimProfileOnlyResearchActivity_DimWebLink_Url',

                    research_activity_organization.id AS 'DimResearchActivity_DimOrganization_Id',
                    research_activity_organization.organization_id AS 'DimResearchActivity_DimOrganization_OrganizationId',
                    research_activity_organization.name_fi AS 'DimResearchActivity_DimOrganization_NameFi',
                    research_activity_organization.name_en AS 'DimResearchActivity_DimOrganization_NameEn',
                    research_activity_organization.name_sv AS 'DimResearchActivity_DimOrganization_NameSv',
                    research_activity_organization_sector.sector_id AS 'DimResearchActivity_DimOrganization_DimSector_SectorId',
                    research_activity_organization_sector.name_fi AS 'DimResearchActivity_DimOrganization_DimSector_NameFi',
                    research_activity_organization_sector.name_en AS 'DimResearchActivity_DimOrganization_DimSector_NameEn',
                    research_activity_organization_sector.name_sv AS 'DimResearchActivity_DimOrganization_DimSector_NameSv',
                    research_activity_organization_broader.id AS 'DimResearchActivity_DimOrganizationBroader_Id',
                    research_activity_organization_broader.organization_id AS 'DimResearchActivity_DimOrganizationBroader_OrganizationId',
                    research_activity_organization_broader.name_fi AS 'DimResearchActivity_DimOrganizationBroader_NameFi',
                    research_activity_organization_broader.name_en AS 'DimResearchActivity_DimOrganizationBroader_NameEn',
                    research_activity_organization_broader.name_sv AS 'DimResearchActivity_DimOrganizationBroader_NameSv',
                    research_activity_organization_broader_sector.sector_id AS 'DimResearchActivity_DimOrganizationBroader_DimSector_SectorId',
                    research_activity_organization_broader_sector.name_fi AS 'DimResearchActivity_DimOrganizationBroader_DimSector_NameFi',
                    research_activity_organization_broader_sector.name_en AS 'DimResearchActivity_DimOrganizationBroader_DimSector_NameEn',
                    research_activity_organization_broader_sector.name_sv AS 'DimResearchActivity_DimOrganizationBroader_DimSector_NameSv',
                    dim_research_activity.name_fi AS 'DimResearchActivity_NameFi',
                    dim_research_activity.name_en AS 'DimResearchActivity_NameEn',
                    dim_research_activity.name_sv AS 'DimResearchActivity_NameSv',
                    dim_research_activity.description_fi AS 'DimResearchActivity_DescriptionFi',
                    dim_research_activity.description_en AS 'DimResearchActivity_DescriptionEn',
                    dim_research_activity.description_sv AS 'DimResearchActivity_DescriptionSv',
                    dim_research_activity.international_collaboration AS 'DimResearchActivity_InternationalCollaboration',
                    research_activity_start_date.year AS 'DimResearchActivity_StartDate_Year',
                    research_activity_start_date.month AS 'DimResearchActivity_StartDate_Month',
                    research_activity_start_date.day AS 'DimResearchActivity_StartDate_Day',
                    research_activity_end_date.year AS 'DimResearchActivity_EndDate_Year',
                    research_activity_end_date.month AS 'DimResearchActivity_EndDate_Month',
                    research_activity_end_date.day AS 'DimResearchActivity_EndDate_Day',
				    research_activity_fact_contribution_activity_type_dim_referencedata.code_value AS 'DimResearchActivity_ActivityType_CodeValue',
					research_activity_fact_contribution_activity_type_dim_referencedata.name_fi AS 'DimResearchActivity_ActivityType_NameFi',
					research_activity_fact_contribution_activity_type_dim_referencedata.name_en AS 'DimResearchActivity_ActivityType_NameEn',
					research_activity_fact_contribution_activity_type_dim_referencedata.name_sv AS 'DimResearchActivity_ActivityType_NameSv',
					research_activity_fact_contribution_researcher_name_activity_dim_referencedata.code_value AS 'DimResearchActivity_Role_CodeValue',
					research_activity_fact_contribution_researcher_name_activity_dim_referencedata.name_fi AS 'DimResearchActivity_Role_NameFi',
					research_activity_fact_contribution_researcher_name_activity_dim_referencedata.name_en AS 'DimResearchActivity_Role_NameEn',
					research_activity_fact_contribution_researcher_name_activity_dim_referencedata.name_sv AS 'DimResearchActivity_Role_NameSv',

                    dfd.acronym AS 'DimFundingDecision_Acronym',
                    dfd.funder_project_number AS 'DimFundingDecision_FunderProjectNumber',
                    dfd.name_fi AS 'DimFundingDecision_NameFi',
                    dfd.name_en AS 'DimFundingDecision_NameEn',
                    dfd.name_sv AS 'DimFundingDecision_NameSv',
                    dfd.description_fi AS 'DimFundingDecision_DescriptionFi',
                    dfd.description_en AS 'DimFundingDecision_DescriptionEn',
                    dfd.description_sv AS 'DimFundingDecision_DescriptionSv',
                    dfd.amount_in_EUR AS 'DimFundingDecision_AmountInEur',
                    funding_decision_start_date.year AS 'DimFundingDecision_StartDate_Year',
                    funding_decision_end_date.year AS 'DimFundingDecision_EndDate_Year',
                    dim_type_of_funding.name_fi AS 'DimFundingDecision_DimTypeOfFunding_NameFi',
                    dim_type_of_funding.name_en AS 'DimFundingDecision_DimTypeOfFunding_NameEn',
                    dim_type_of_funding.name_sv AS 'DimFundingDecision_DimTypeOfFunding_NameSv',
                    dim_call_programme.name_fi AS 'DimFundingDecision_DimCallProgramme_NameFi',
                    dim_call_programme.name_en AS 'DimFundingDecision_DimCallProgramme_NameEn',
                    dim_call_programme.name_sv AS 'DimFundingDecision_DimCallProgramme_NameSv',
                    dfd_organization.name_fi AS 'DimFundingDecision_Funder_NameFi',
                    dfd_organization.name_en AS 'DimFundingDecision_Funder_NameEn',
                    dfd_organization.name_sv AS 'DimFundingDecision_Funder_NameSv',

                    dpofd.acronym AS 'DimProfileOnlyFundingDecision_Acronym',
                    dpofd.funder_project_number AS 'DimProfileOnlFundingDecision_FunderProjectNumber',
                    dpofd.name_fi AS 'DimProfileOnlyFundingDecision_NameFi',
                    dpofd.name_en AS 'DimProfileOnlyFundingDecision_NameEn',
                    dpofd.name_sv AS 'DimProfileOnlyFundingDecision_NameSv',
                    dpofd.description_fi AS 'DimProfileOnlyFundingDecision_DescriptionFi',
                    dpofd.description_en AS 'DimProfileOnlyFundingDecision_DescriptionEn',
                    dpofd.description_sv AS 'DimProfileOnlyFundingDecision_DescriptionSv',
                    dpofd.amount_in_EUR AS 'DimProfileOnlyFundingDecision_AmountInEur',
                    dpofd.amount_in_funding_decision_currency AS 'DimProfileOnlyFundingDecision_AmountInFundingDecisionCurrency',
                    dpofd.funding_decision_currency_abbreviation AS 'DimProfileOnlyFundingDecision_FundingDecisionCurrencyAbbreviation',
                    profile_only_funding_decision_start_date.year AS 'DimProfileOnlyFundingDecision_StartDate_Year',
                    profile_only_funding_decision_end_date.year AS 'DimProfileOnlyFundingDecision_EndDate_Year',
                    dpofd_organization.id AS 'DimProfileOnlyFundingDecision_DimOrganization_Id',
                    dpofd_organization.name_fi AS 'DimProfileOnlyFundingDecision_DimOrganization_NameFi',
                    dpofd_organization.name_en AS 'DimProfileOnlyFundingDecision_DimOrganization_NameEn',
                    dpofd_organization.name_sv AS 'DimProfileOnlyFundingDecision_DimOrganization_NameSv',
                    profile_only_funding_decision_actor_role.code_value AS 'DimProfileOnlyFundingDecision_Role_CodeValue',
                    profile_only_funding_decision_actor_role.name_fi AS 'DimProfileOnlyFundingDecision_DimTypeOfFunding_NameFi',
                    profile_only_funding_decision_actor_role.name_en AS 'DimProfileOnlyFundingDecision_DimTypeOfFunding_NameEn',
                    profile_only_funding_decision_actor_role.name_sv AS 'DimProfileOnlyFundingDecision_DimTypeOfFunding_NameSv',
                    dim_profile_only_funding_decision_web_link.url AS 'DimProfileOnlyFundingDecision_DimWebLink_Url',

                    dim_research_dataset.local_identifier AS 'DimResearchDataset_LocalIdentifier',
                    dim_research_dataset.name_fi AS 'DimResearchDataset_NameFi',
                    dim_research_dataset.name_en AS 'DimResearchDataset_NameEn',
                    dim_research_dataset.name_sv AS 'DimResearchDataset_NameSv',
                    dim_research_dataset.description_fi AS 'DimResearchDataset_DescriptionFi',
                    dim_research_dataset.description_en AS 'DimResearchDataset_DescriptionEn',
                    dim_research_dataset.description_sv AS 'DimResearchDataset_DescriptionSv',
                    dim_research_dataset.dataset_created AS 'DimResearchDataset_DatasetCreated',
                    dim_research_dataset_referencedata_availability.code_value AS 'DimResearchDataset_AccessType',

                    dim_profile_only_dataset.local_identifier AS 'DimProfileOnlyDataset_LocalIdentifier',
                    dim_profile_only_dataset.name_fi AS 'DimProfileOnlyDataset_NameFi',
                    dim_profile_only_dataset.name_en AS 'DimProfileOnlyDataset_NameEn',
                    dim_profile_only_dataset.name_sv AS 'DimProfileOnlyDataset_NameSv',
                    dim_profile_only_dataset.description_fi AS 'DimProfileOnlyDataset_DescriptionFi',
                    dim_profile_only_dataset.description_en AS 'DimProfileOnlyDataset_DescriptionEn',
                    dim_profile_only_dataset.description_sv AS 'DimProfileOnlyDataset_DescriptionSv',
                    dim_profile_only_dataset.dataset_created AS 'DimProfileOnlyDataset_DatasetCreated',
                    dim_profile_only_dataset_web_link.url AS 'DimProfileOnlyDataset_DimWebLink_Url'

                FROM fact_field_values AS ffv

                JOIN dim_field_display_settings AS dfds ON ffv.dim_field_display_settings_id=dfds.id
                JOIN dim_registered_data_source AS drds ON ffv.dim_registered_data_source_id=drds.id
                JOIN dim_organization AS drds_organization ON drds.dim_organization_id=drds_organization.id
                JOIN dim_sector AS drds_organization_sector ON drds_organization.dim_sectorid=drds_organization_sector.id
                JOIN dim_name ON ffv.dim_name_id=dim_name.id
                JOIN dim_web_link ON ffv.dim_web_link_id=dim_web_link.id
                JOIN dim_researcher_description ON ffv.dim_researcher_description_id=dim_researcher_description.id
                JOIN dim_email_addrress ON ffv.dim_email_addrress_id=dim_email_addrress.id
                JOIN dim_telephone_number ON ffv.dim_telephone_number_id=dim_telephone_number.id
                JOIN dim_keyword ON ffv.dim_keyword_id=dim_keyword.id
                JOIN dim_pid ON ffv.dim_pid_id=dim_pid.id
                LEFT JOIN dim_pid AS publication_doi ON ffv.dim_publication_id=publication_doi.dim_publication_id AND ffv.dim_publication_id!=-1 AND publication_doi.pid_type='doi'

                JOIN dim_affiliation ON ffv.dim_affiliation_id=dim_affiliation.id
                JOIN dim_organization AS affiliation_organization ON dim_affiliation.dim_organization_id=affiliation_organization.id
                LEFT JOIN dim_organization AS affiliation_organization_broader ON affiliation_organization_broader.id=affiliation_organization.dim_organization_broader AND affiliation_organization.dim_organization_broader!=-1
                JOIN dim_sector AS affiliation_organization_sector ON affiliation_organization.dim_sectorid=affiliation_organization_sector.id
                LEFT JOIN dim_sector AS affiliation_organization_broader_sector ON affiliation_organization_broader.dim_sectorid=affiliation_organization_broader_sector.id
                LEFT JOIN dim_date AS affiliation_start_date ON dim_affiliation.start_date=affiliation_start_date.id AND affiliation_start_date.id!=-1
                LEFT JOIN dim_date AS affiliation_end_date ON dim_affiliation.end_date=affiliation_end_date.id AND affiliation_end_date.id!=-1
               
                JOIN dim_identifierless_data AS did ON ffv.dim_identifierless_data_id=did.id
                LEFT JOIN dim_identifierless_data AS did_child ON did_child.dim_identifierless_data_id=did.id AND did_child.dim_identifierless_data_id!=-1

                JOIN dim_education ON ffv.dim_education_id=dim_education.id
                LEFT JOIN dim_date AS education_start_date ON dim_education.dim_start_date=education_start_date.id AND education_start_date.id!=-1
                LEFT JOIN dim_date AS education_end_date ON dim_education.dim_end_date=education_end_date.id AND education_end_date.id!=-1

                JOIN dim_publication ON ffv.dim_publication_id=dim_publication.id
                LEFT JOIN dim_referencedata AS dim_publication_referencedata_type_code ON dim_publication.publication_type_code=dim_publication_referencedata_type_code.id AND dim_publication.publication_type_code!=-1
                LEFT JOIN dim_referencedata AS dim_publication_referencedata_open_access_code ON dim_publication.open_access_code=dim_publication_referencedata_open_access_code.id AND dim_publication.open_access_code!=-1
                LEFT JOIN dim_locally_reported_pub_info AS dim_publication_locally_reported_pub_info ON dim_publication_locally_reported_pub_info.dim_publicationid=dim_publication.id AND dim_publication.id!=-1

                JOIN dim_profile_only_publication ON ffv.dim_profile_only_publication_id=dim_profile_only_publication.id

                JOIN dim_profile_only_research_activity ON ffv.dim_profile_only_research_activity_id=dim_profile_only_research_activity.id
                JOIN dim_organization AS profile_only_research_activity_organization ON dim_profile_only_research_activity.dim_organization_id=profile_only_research_activity_organization.id
                LEFT JOIN dim_organization AS profile_only_research_activity_organization_broader ON profile_only_research_activity_organization_broader.id=profile_only_research_activity_organization.dim_organization_broader AND profile_only_research_activity_organization.dim_organization_broader!=-1
                JOIN dim_sector AS profile_only_research_activity_organization_sector ON profile_only_research_activity_organization.dim_sectorid=profile_only_research_activity_organization_sector.id
                LEFT JOIN dim_sector AS profile_only_research_activity_organization_broader_sector ON profile_only_research_activity_organization_broader.dim_sectorid=profile_only_research_activity_organization_broader_sector.id
                LEFT JOIN dim_referencedata AS profile_only_research_activity_referencedata ON ffv.dim_referencedata_actor_role_id=profile_only_research_activity_referencedata.id
                LEFT JOIN dim_date AS dim_profile_only_research_activity_start_date ON dim_profile_only_research_activity.dim_date_id_start=dim_profile_only_research_activity_start_date.id AND dim_profile_only_research_activity_start_date.id!=-1
                LEFT JOIN dim_date AS dim_profile_only_research_activity_end_date ON dim_profile_only_research_activity.dim_date_id_end=dim_profile_only_research_activity_end_date.id AND dim_profile_only_research_activity_end_date.id!=-1
                LEFT JOIN dim_web_link AS dim_profile_only_research_activity_web_link ON dim_profile_only_research_activity_web_link.dim_profile_only_research_activity_id=dim_profile_only_research_activity.id AND dim_profile_only_research_activity_web_link.dim_profile_only_research_activity_id!=-1

                JOIN dim_research_activity ON ffv.dim_research_activity_id=dim_research_activity.id
                JOIN dim_organization AS research_activity_organization ON dim_research_activity.dim_organization_id=research_activity_organization.id
                LEFT JOIN dim_organization AS research_activity_organization_broader ON research_activity_organization_broader.id=research_activity_organization.dim_organization_broader AND research_activity_organization.dim_organization_broader!=-1
                JOIN dim_sector AS research_activity_organization_sector ON research_activity_organization.dim_sectorid=research_activity_organization_sector.id
                LEFT JOIN dim_sector AS research_activity_organization_broader_sector ON research_activity_organization_broader.dim_sectorid=research_activity_organization_broader_sector.id
                LEFT JOIN dim_date AS research_activity_start_date ON dim_research_activity.dim_start_date=research_activity_start_date.id AND research_activity_start_date.id!=-1
                LEFT JOIN dim_date AS research_activity_end_date ON dim_research_activity.dim_end_date=research_activity_end_date.id AND research_activity_end_date.id!=-1
				
				LEFT JOIN fact_contribution AS research_activity_fact_contribution_activity_type ON dim_research_activity.id=research_activity_fact_contribution_activity_type.dim_research_activity_id AND
					dim_research_activity.id!=-1 AND
					research_activity_fact_contribution_activity_type.contribution_type='activity_type'
				LEFT JOIN dim_referencedata AS research_activity_fact_contribution_activity_type_dim_referencedata ON
					research_activity_fact_contribution_activity_type.dim_referencedata_actor_role_id=research_activity_fact_contribution_activity_type_dim_referencedata.id AND
					research_activity_fact_contribution_activity_type_dim_referencedata.id!=-1

				LEFT JOIN fact_contribution AS research_activity_fact_contribution_researcher_name_activity ON dim_research_activity.id=research_activity_fact_contribution_researcher_name_activity.dim_research_activity_id AND
					dim_research_activity.id!=-1 AND
					research_activity_fact_contribution_researcher_name_activity.contribution_type='researcher_name_activity'
				LEFT JOIN dim_referencedata AS research_activity_fact_contribution_researcher_name_activity_dim_referencedata ON
					research_activity_fact_contribution_researcher_name_activity.dim_referencedata_actor_role_id=research_activity_fact_contribution_researcher_name_activity_dim_referencedata.id AND
					research_activity_fact_contribution_researcher_name_activity_dim_referencedata.id!=-1
              
				JOIN dim_funding_decision AS dfd ON ffv.dim_funding_decision_id=dfd.id
                LEFT JOIN dim_date AS funding_decision_start_date ON dfd.dim_date_id_start=funding_decision_start_date.id AND funding_decision_start_date.id!=-1
                LEFT JOIN dim_date AS funding_decision_end_date ON dfd.dim_date_id_end=funding_decision_end_date.id AND funding_decision_end_date.id!=-1
                LEFT JOIN dim_call_programme ON dim_call_programme.id=dfd.dim_call_programme_id
                LEFT JOIN dim_type_of_funding ON dim_type_of_funding.id=dfd.dim_type_of_funding_id
                LEFT JOIN dim_organization AS dfd_organization ON dfd_organization.id=dfd.dim_organization_id_funder
                
                JOIN dim_profile_only_funding_decision AS dpofd ON ffv.dim_profile_only_funding_decision_id=dpofd.id
                LEFT JOIN dim_organization AS dpofd_organization ON dpofd_organization.id=dpofd.dim_organization_id_funder
                LEFT JOIN dim_date AS profile_only_funding_decision_start_date ON dpofd.dim_date_id_start=profile_only_funding_decision_start_date.id AND profile_only_funding_decision_start_date.id!=-1
                LEFT JOIN dim_date AS profile_only_funding_decision_end_date ON dpofd.dim_date_id_end=profile_only_funding_decision_end_date.id AND profile_only_funding_decision_end_date.id!=-1
                LEFT JOIN dim_referencedata AS profile_only_funding_decision_actor_role ON ffv.dim_referencedata_actor_role_id=profile_only_funding_decision_actor_role.id
                LEFT JOIN dim_web_link AS dim_profile_only_funding_decision_web_link ON dim_profile_only_funding_decision_web_link.dim_profile_only_funding_decision_id=dpofd.id AND dim_profile_only_funding_decision_web_link.dim_profile_only_funding_decision_id!=-1

                JOIN dim_research_dataset ON ffv.dim_research_dataset_id=dim_research_dataset.id
                LEFT JOIN dim_referencedata AS dim_research_dataset_referencedata_availability ON dim_research_dataset.dim_referencedata_availability=dim_research_dataset_referencedata_availability.id AND dim_research_dataset.dim_referencedata_availability!=-1

                JOIN dim_profile_only_dataset ON ffv.dim_profile_only_dataset_id=dim_profile_only_dataset.id
                LEFT JOIN dim_web_link AS dim_profile_only_dataset_web_link ON dim_profile_only_dataset_web_link.dim_profile_only_dataset_id=dim_profile_only_dataset.id AND dim_profile_only_dataset_web_link.dim_profile_only_dataset_id!=-1

                WHERE
                    ffv.dim_user_profile_id={userprofileId} AND {(forElasticsearch ? " ffv.show=1 AND " : "")}
                    (
                        ffv.dim_name_id != -1 OR
                        ffv.dim_web_link_id != -1 OR
                        ffv.dim_researcher_description_id != -1 OR
                        ffv.dim_email_addrress_id != -1 OR
                        ffv.dim_telephone_number_id != -1 OR
                        ffv.dim_keyword_id != -1 OR
                        ffv.dim_pid_id != -1 OR
                        ffv.dim_affiliation_id != -1 OR
                        ffv.dim_identifierless_data_id != -1 OR
                        ffv.dim_education_id != -1 OR
                        ffv.dim_publication_id != -1 OR
                        ffv.dim_profile_only_dataset_id != -1 OR
                        ffv.dim_profile_only_funding_decision_id != -1 OR
                        ffv.dim_profile_only_publication_id != -1 OR
                        ffv.dim_profile_only_research_activity_id != -1 OR
                        ffv.dim_research_activity_id != -1 OR
                        ffv.dim_funding_decision_id != -1 OR
                        ffv.dim_research_dataset_id != -1 OR
                        ffv.dim_referencedata_field_of_science_id != -1 OR
                        ffv.dim_referencedata_actor_role_id != -1
                    )
                ";
        }

        // Return SQL SELECT statement for fact_field_values.
        public string GetSqlQuery_Select_FactFieldValues(int userprofileId)
        {
            return $"SELECT * FROM fact_field_values WHERE dim_user_profile_id={userprofileId}";
        }

        // Return SQL DELETE statement for fact_field_values.
        public string GetSqlQuery_Delete_FactFieldValues(int userprofileId)
        {
            return $"DELETE FROM fact_field_values WHERE dim_user_profile_id={userprofileId}";
        }

        // Return SQL DELETE statement for dim_identifierless_data children
        public string GetSqlQuery_Delete_DimIdentifierlessData_Children(List<int> dimIdentifierlessDataIds)
        {
            return $"DELETE FROM dim_identifierless_data WHERE dim_identifierless_data_id IN ({ConvertListOfIntsToCommaSeparatedString(dimIdentifierlessDataIds)})";
        }

        // Return SQL DELETE statement for dim_identifierless_data parent
        public string GetSqlQuery_Delete_DimIdentifierlessData_Parent(List<int> dimIdentifierlessDataIds)
        {
            return $"DELETE FROM dim_identifierless_data WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimIdentifierlessDataIds)})";
        }

        // Return SQL DELETE statement for dim_affiliation
        public string GetSqlQuery_Delete_DimAffiliations(List<int> dimAffiliationIds)
        {
            return $"DELETE FROM dim_affiliation WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimAffiliationIds)})";
        }

        // Return SQL DELETE statement for dim_competence
        public string GetSqlQuery_Delete_DimCompetences(List<int> dimCompetenceIds)
        {
            return $"DELETE FROM dim_competence WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimCompetenceIds)})";
        }

        // Return SQL DELETE statement for dim_education
        public string GetSqlQuery_Delete_DimEducations(List<int> dimEducationIds)
        {
            return $"DELETE FROM dim_education WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimEducationIds)})";
        }

        // Return SQL DELETE statement for dim_email_addrress
        public string GetSqlQuery_Delete_DimEmailAddrresses(List<int> dimEmailAddrressIds)
        {
            return $"DELETE FROM dim_email_addrress WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimEmailAddrressIds)})";
        }

        // Return SQL DELETE statement for dim_event
        public string GetSqlQuery_Delete_DimEvents(List<int> dimEventIds)
        {
            return $"DELETE FROM dim_event WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimEventIds)})";
        }

        // Return SQL DELETE statement for dim_field_of_science
        public string GetSqlQuery_Delete_DimFieldsOfScience(List<int> dimFieldOfScienceIds)
        {
            return $"DELETE FROM dim_field_of_science WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimFieldOfScienceIds)})";
        }

        // Return SQL DELETE statement for dim_funding_decision
        public string GetSqlQuery_Delete_DimFundingDecisions(List<int> dimFundingDecisionIds)
        {
            return $"DELETE FROM dim_funding_decision WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimFundingDecisionIds)})";
        }

        // Return SQL DELETE statement for dim_keyword
        public string GetSqlQuery_Delete_DimKeyword(List<int> dimKeywordIds)
        {
            return $"DELETE FROM dim_keyword WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimKeywordIds)})";
        }

        // Return SQL DELETE statement for dim_name
        public string GetSqlQuery_Delete_DimNames(List<long> dimNameIds)
        {
            return $"DELETE FROM dim_name WHERE id IN ({ConvertListOfLongsToCommaSeparatedString(dimNameIds)})";
        }

        // Return SQL DELETE statement for dim_profile_dataset
        public string GetSqlQuery_Delete_DimProfileOnlyDatasets(List<int> dimProfileOnlyDatasetIds)
        {
            return $"DELETE FROM dim_profile_only_dataset WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimProfileOnlyDatasetIds)})";
        }

        // Return SQL DELETE statement for dim_profile_funding_decision
        public string GetSqlQuery_Delete_DimProfileOnlyFundingDecisions(List<int> dimProfileOnlyFundingDecisionIds)
        {
            return $"DELETE FROM dim_profile_only_funding_decision WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimProfileOnlyFundingDecisionIds)})";
        }

        // Return SQL DELETE statement for dim_profile_only_publication
        public string GetSqlQuery_Delete_DimProfileOnlyPublications(List<int> dimProfileOnlyPublicationIds)
        {
            return $"DELETE FROM dim_profile_only_publication WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimProfileOnlyPublicationIds)})";
        }

        // Return SQL DELETE statement for dim_profile_only_research_activity
        public string GetSqlQuery_Delete_DimProfileOnlyResearchActivities(List<int> dimProfileOnlyResearchActivityIds)
        {
            return $"DELETE FROM dim_profile_only_research_activity WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimProfileOnlyResearchActivityIds)})";
        }

        // Return SQL DELETE statement for dim_pid
        public string GetSqlQuery_Delete_DimPids(List<int> dimPidIds)
        {
            return $"DELETE FROM dim_pid WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimPidIds)})";
        }

        // Return SQL DELETE statement for dim_research_activity
        public string GetSqlQuery_Delete_DimResearchActivities(List<int> dimResearchActivityIds)
        {
            return $"DELETE FROM dim_research_activity WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimResearchActivityIds)})";
        }

        // Return SQL DELETE statement for dim_research_community
        public string GetSqlQuery_Delete_DimResearchCommunities(List<int> dimResearchCommunityIds)
        {
            return $"DELETE FROM dim_research_community WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimResearchCommunityIds)})";
        }

        // Return SQL DELETE statement for dim_research_dataset
        public string GetSqlQuery_Delete_DimResearchDatasets(List<int> dimResearchDatasetIds)
        {
            return $"DELETE FROM dim_research_dataset WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimResearchDatasetIds)})";
        }

        // Return SQL DELETE statement for dim_researcher_description
        public string GetSqlQuery_Delete_DimResearchDescriptions(List<int> dimResearcherDescriptionIds)
        {
            return $"DELETE FROM dim_researcher_description WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimResearcherDescriptionIds)})";
        }

        // Return SQL DELETE statement for dim_researcher_to_research_community
        public string GetSqlQuery_Delete_DimResearcherToResearchCommunities(List<int> dimResearcherToResearchCommunityIds)
        {
            return $"DELETE FROM dim_researcher_to_research_community WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimResearcherToResearchCommunityIds)})";
        }

        // Return SQL DELETE statement for dim_telephone_number
        public string GetSqlQuery_Delete_DimTelephoneNumbers(List<int> dimTelephoneNumberIds)
        {
            return $"DELETE FROM dim_telephone_number WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimTelephoneNumberIds)})";
        }

        // Return SQL DELETE statement for dim_web_link
        public string GetSqlQuery_Delete_DimWebLinks(List<int> dimWebLinkIds)
        {
            return $"DELETE FROM dim_web_link WHERE id IN ({ConvertListOfIntsToCommaSeparatedString(dimWebLinkIds)})";
        }

        // Return SQL DELETE statement for dim_field_display_settings.
        public string GetSqlQuery_Delete_DimFieldDisplaySettings(int userprofileId)
        {
            return $"DELETE FROM dim_field_display_settings WHERE dim_user_profile_id={userprofileId}";
        }

        // Return SQL DELETE statement for br_granted_permissions.
        public string GetSqlQuery_Delete_BrGrantedPermissions(int userprofileId)
        {
            return $"DELETE FROM br_granted_permissions WHERE dim_user_profile_id={userprofileId}";
        }

        // Return SQL DELETE statement for dim_user_choices.
        public string GetSqlQuery_Delete_DimUserChoices(int userprofileId)
        {
            return $"DELETE FROM dim_user_choices WHERE dim_user_profile_id={userprofileId}";
        }

        // Return SQL DELETE statement for dim_user_profile.
        public string GetSqlQuery_Delete_DimUserProfile(int userprofileId)
        {
            return $"DELETE FROM dim_user_profile WHERE id={userprofileId}";
        }

        // Return SQL SELECT statement for dim_email_addrress
        public string GetSqlQuery_Select_DimEmailAddrress(int dimKnownPersonId, List<int> existingIds)
        {
            string excludeIdsSQL = existingIds.Count > 0 ? $" AND id NOT IN ({ConvertListOfIntsToCommaSeparatedString(existingIds)})" : "";
            return $@"SELECT id as 'Id', dim_registered_data_source_id AS 'DimRegisteredDataSourceId'
                        FROM dim_email_addrress
                        WHERE dim_known_person_id={dimKnownPersonId} AND id!=-1 AND dim_registered_data_source_id!=-1{excludeIdsSQL}";
        }

        // Return SQL SELECT statement for dim_researcher_description
        public string GetSqlQuery_Select_DimResearcherDescription(int dimKnownPersonId, List<int> existingIds)
        {
            string excludeIdsSQL = existingIds.Count > 0 ? $" AND id NOT IN ({ConvertListOfIntsToCommaSeparatedString(existingIds)})" : "";
            return $@"SELECT id as 'Id', dim_registered_data_source_id AS 'DimRegisteredDataSourceId'
                        FROM dim_researcher_description
                        WHERE dim_known_person_id={dimKnownPersonId} AND id!=-1 AND dim_registered_data_source_id!=-1{excludeIdsSQL}";
        }

        // Return SQL SELECT statement for dim_web_link
        // TODO: IS NOT NULL condition can be removed, when table dim_web_link.dim_registered_data_source_id is modified to disallow NULL.
        public string GetSqlQuery_Select_DimWebLink(int dimKnownPersonId, List<int> existingIds)
        {
            string excludeIdsSQL = existingIds.Count > 0 ? $" AND id NOT IN ({ConvertListOfIntsToCommaSeparatedString(existingIds)})" : "";
            return $@"SELECT id as 'Id', dim_registered_data_source_id AS 'DimRegisteredDataSourceId'
                        FROM dim_web_link
                        WHERE dim_known_person_id={dimKnownPersonId} AND id!=-1 AND dim_registered_data_source_id!=-1 AND dim_registered_data_source_id IS NOT NULL{excludeIdsSQL} AND source_id!='{Constants.SourceIdentifiers.PROFILE_API}'";
        }

        // Return SQL SELECT statement for dim_telephone_number
        public string GetSqlQuery_Select_DimTelephoneNumber(int dimKnownPersonId, List<int> existingIds)
        {
            string excludeIdsSQL = existingIds.Count > 0 ? $" AND id NOT IN ({ConvertListOfIntsToCommaSeparatedString(existingIds)})" : "";
            return $@"SELECT id as 'Id', dim_registered_data_source_id AS 'DimRegisteredDataSourceId'
                        FROM dim_telephone_number
                        WHERE dim_known_person_id={dimKnownPersonId} AND id!=-1 AND dim_registered_data_source_id!=-1{excludeIdsSQL}";
        }

        // Return SQL SELECT statement for dim_affiliation
        public string GetSqlQuery_Select_DimAffiliation(int dimKnownPersonId, List<int> existingIds)
        {
            string excludeIdsSQL = existingIds.Count > 0 ? $" AND id NOT IN ({ConvertListOfIntsToCommaSeparatedString(existingIds)})" : "";
            return $@"SELECT id as 'Id', dim_registered_data_source_id AS 'DimRegisteredDataSourceId'
                        FROM dim_affiliation
                        WHERE dim_known_person_id={dimKnownPersonId} AND id!=-1 AND dim_registered_data_source_id!=-1{excludeIdsSQL}";
        }

        // Return SQL SELECT statement for dim_education
        public string GetSqlQuery_Select_DimEducation(int dimKnownPersonId, List<int> existingIds)
        {
            string excludeIdsSQL = existingIds.Count > 0 ? $" AND id NOT IN ({ConvertListOfIntsToCommaSeparatedString(existingIds)})" : "";
            return $@"SELECT id as 'Id', dim_registered_data_source_id AS 'DimRegisteredDataSourceId'
                        FROM dim_education
                        WHERE dim_known_person_id={dimKnownPersonId} AND id!=-1 AND dim_registered_data_source_id!=-1{excludeIdsSQL}";
        }

        // Return SQL SELECT statement for fact_contribution
        public string GetSqlQuery_Select_FactContribution(long dimNameId)
        {
            return $@"SELECT DISTINCT
                        fc.dim_research_activity_id AS 'DimResearchActivityId',
                        fc.dim_research_dataset_id AS 'DimResearchDatasetId',
                        fc.dim_publication_id AS 'DimPublicationId',
                        COALESCE(dp.dim_publication_id, -1) AS 'CoPublication_Parent_DimPublicationId'
                    FROM
                        fact_contribution AS fc
                    JOIN
                        dim_publication AS dp ON fc.dim_publication_id=dp.id
                    WHERE
                        fc.dim_name_id = {dimNameId} AND
                        (
                            fc.dim_research_activity_id!=-1 OR fc.dim_research_dataset_id!=-1 OR fc.dim_publication_id!=-1
                        )";
        }

        // Return SQL SELECT statement for br_participates_in_funding_group
        public string GetSqlQuery_Select_BrParticipatesInFundingGroup(long dimNameId, List<int> existingFundingDecisionIds)
        {
            string excludeFundingDecisionIdsSQL =
                existingFundingDecisionIds.Count > 0 ? $" AND dim_funding_decisionid NOT IN ({ConvertListOfIntsToCommaSeparatedString(existingFundingDecisionIds)})" : "";
            return $@"SELECT dim_funding_decisionid as 'DimFundingDecisionId'
                        FROM br_participates_in_funding_group
                        WHERE dim_name_id = {dimNameId}{excludeFundingDecisionIdsSQL}";
        }

        // Return SQL SELECT statement for getting property "hidden" in userprofile
        public string GetSqlQuery_Select_GetHiddenInUserprofile(int dimUserProfileId)
        {
            return $"SELECT hidden as 'Hidden' FROM dim_user_profile WHERE id={dimUserProfileId}";
        }

        // Return SQL SELECT statement for counting number of published items in userprofile
        public string GetSqlQuery_Select_CountPublishedItemsInUserprofile(int dimUserProfileId)
        {
            return $@"SELECT COUNT(ffv.show) AS 'PublishedCount'
                        FROM fact_field_values AS ffv
                        JOIN dim_user_profile AS dup ON ffv.dim_user_profile_id=dup.id
                        WHERE dup.id={dimUserProfileId} AND ffv.show=1";
        }

        // Return SQL SELECT statement for matching ORCID and TTV publications by Doi
        public string GetSqlQuery_Select_PublicationDoiMatching(int dimUserProfileId)
        {
            return $@"SELECT
                        pop.doi_handle AS DimProfileOnlyPublication_Doi,
                        pop.publication_name AS DimProfileOnlyPublication_PublicationName,
                        pub.id AS DimPublication_Id,
                        pub.publication_id AS DimPublication_PublicationId,
                        pub.publication_name AS DimPublication_PublicationName,
                        pid.pid_content AS DimPublication_Doi,
                        type_code.code_value AS DimPublication_TypeCode,
                        ffv.show AS FactFieldValues_Show
                    FROM fact_field_values AS ffv
                    INNER JOIN dim_user_profile AS dup ON dup.id=ffv.dim_user_profile_id
                    INNER JOIN dim_profile_only_publication AS pop ON pop.id=ffv.dim_profile_only_publication_id
                    INNER JOIN dim_pid AS pid ON pid.pid_type='doi' AND pid.pid_content=pop.doi_handle
                    INNER JOIN dim_publication AS pub ON pub.id=pid.dim_publication_id
                    INNER JOIN dim_referencedata AS type_code ON type_code.id=pub.publication_type_code
                    WHERE
                        dup.id={dimUserProfileId} AND
                        ffv.dim_profile_only_publication_id > 0 AND
                        pop.doi_handle IS NOT NULL AND pop.doi_handle <> '' AND
                        pid.dim_publication_id>0 AND
                        (pub.dim_publication_id<0 OR pub.dim_publication_id IS NULL) AND
                        NOT EXISTS (
                            SELECT 1
                            FROM fact_field_values ffv2
                            WHERE
                                ffv2.dim_user_profile_id={dimUserProfileId} AND
                                ffv2.dim_publication_id = pub.id
                        )";
        }
    }
}