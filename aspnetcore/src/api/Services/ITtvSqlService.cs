using System.Collections.Generic;
using api.Models.ProfileEditor.Items;

namespace api.Services
{
    public interface ITtvSqlService
    {
        string ConvertListOfIntsToCommaSeparatedString(List<int> listOfInts);
        string ConvertListOfLongsToCommaSeparatedString(List<long> listOfInts);
        string GetFactFieldValuesFKColumnNameFromItemMetaType(int itemMetaType);
        string GetSqlQuery_Delete_BrGrantedPermissions(int userprofileId);
        string GetSqlQuery_Delete_DimAffiliations(List<int> dimAffiliationIds);
        string GetSqlQuery_Delete_DimCompetences(List<int> dimCompetenceIds);
        string GetSqlQuery_Delete_DimEducations(List<int> dimEducationIds);
        string GetSqlQuery_Delete_DimEmailAddrresses(List<int> dimEmailAddrressIds);
        string GetSqlQuery_Delete_DimEvents(List<int> dimEventIds);
        string GetSqlQuery_Delete_DimFieldDisplaySettings(int userprofileId);
        string GetSqlQuery_Delete_DimFieldsOfScience(List<int> dimFieldOfScienceIds);
        string GetSqlQuery_Delete_DimFundingDecisions(List<int> dimFundingDecisionIds);
        string GetSqlQuery_Delete_DimIdentifierlessData_Children(List<int> dimIdentifierlessDataIds);
        string GetSqlQuery_Delete_DimIdentifierlessData_Parent(List<int> dimIdentifierlessDataIds);
        string GetSqlQuery_Delete_DimKeyword(List<int> dimKeywordIds);
        string GetSqlQuery_Delete_DimNames(List<long> dimNameIds);
        string GetSqlQuery_Delete_DimProfileOnlyDatasets(List<int> dimProfileOnlyDatasetIds);
        string GetSqlQuery_Delete_DimProfileOnlyFundingDecisions(List<int> dimProfileOnlyFundingDecisionIds);
        string GetSqlQuery_Delete_DimProfileOnlyPublications(List<int> dimProfileOnlyPublicationIds);
        string GetSqlQuery_Delete_DimProfileOnlyResearchActivities(List<int> dimProfileOnlyResearchActivityIds);
        string GetSqlQuery_Delete_DimPids(List<int> dimPidIds);
        string GetSqlQuery_Delete_DimResearchActivities(List<int> dimResearchActivityIds);
        string GetSqlQuery_Delete_DimResearchCommunities(List<int> dimResearchCommunityIds);
        string GetSqlQuery_Delete_DimResearchDatasets(List<int> dimResearchDatasetIds);
        string GetSqlQuery_Delete_DimResearchDescriptions(List<int> dimResearcherDescriptionIds);
        string GetSqlQuery_Delete_DimResearcherToResearchCommunities(List<int> dimResearcherToResearchCommunityIds);
        string GetSqlQuery_Delete_DimTelephoneNumbers(List<int> dimTelephoneNumberIds);
        string GetSqlQuery_Delete_DimUserChoices(int userprofileId);
        string GetSqlQuery_Delete_DimUserProfile(int userprofileId);
        string GetSqlQuery_Delete_DimWebLinks(List<int> dimWebLinkIds);
        string GetSqlQuery_Delete_FactFieldValues(int userprofileId);
        string GetSqlQuery_ProfileData(int userprofileId, bool forElasticsearch = false);
        string GetSqlQuery_Select_BrParticipatesInFundingGroup(long dimNameId, List<int> existingFundingDecisionIds);
        string GetSqlQuery_Select_CountPublishedItemsInUserprofile(int dimUserProfileId);
        string GetSqlQuery_Select_DimAffiliation(int dimKnownPersonId, List<int> existingIds);
        string GetSqlQuery_Select_DimEducation(int dimKnownPersonId, List<int> existingIds);
        string GetSqlQuery_Select_DimEmailAddrress(int dimKnownPersonId, List<int> existingIds);
        string GetSqlQuery_Select_DimResearcherDescription(int dimKnownPersonId, List<int> existingIds);
        string GetSqlQuery_Select_DimTelephoneNumber(int dimKnownPersonId, List<int> existingIds);
        string GetSqlQuery_Select_DimWebLink(int dimKnownPersonId, List<int> existingIds);
        string GetSqlQuery_Select_FactContribution(long dimNameId);
        string GetSqlQuery_Select_FactFieldValues(int userprofileId);
        string GetSqlQuery_Select_GetHiddenInUserprofile(int dimUserProfileId);
        string GetSqlQuery_Update_FactFieldValues(int dimUserProfileId, ProfileEditorItemMeta profileEditorItemMeta);
        string GetSqlQuery_Update_DimUserProfile_Modified(int dimUserProfileId);
        string GetSqlQuery_ProfileEditorCooperationItems(int userprofileId);
        string GetSqlQuery_ProfileSettings(int userprofileId);
    }
}