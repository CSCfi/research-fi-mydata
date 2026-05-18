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