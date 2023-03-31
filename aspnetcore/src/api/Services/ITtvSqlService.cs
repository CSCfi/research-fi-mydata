using System.Collections.Generic;
using api.Models.ProfileEditor.Items;

namespace api.Services
{
    public interface ITtvSqlService
    {
        string ConvertListOfIntsToCommaSeparatedString(List<int> listOfInts);
        string GetFactFieldValuesFKColumnNameFromFieldIdentifier(int fieldIdentifier);
        string GetSqlQuery_Delete_BrGrantedPermissions(int userprofileId);
        string GetSqlQuery_Delete_DimAffiliations(List<int> dimAffiliationIds);
        string GetSqlQuery_Delete_DimCompetences(List<int> dimCompetenceIds);
        string GetSqlQuery_Delete_DimEducations(List<int> dimEducationIds);
        string GetSqlQuery_Delete_DimEmailAddrresses(List<int> dimEmailAddrressIds);
        string GetSqlQuery_Delete_DimEvents(List<int> dimEventIds);
        string GetSqlQuery_Delete_DimFieldDisplaySettings(int userprofileId);
        string GetSqlQuery_Delete_DimFieldsOfScience(List<int> dimFieldOfScienceIds);
        string GetSqlQuery_Delete_DimFundingDecisions(List<int> dimFundingDecisionIds);
        string GetSqlQuery_Delete_DimIdentifierlessData_Children(int dimIdentifierlessDataId);
        string GetSqlQuery_Delete_DimIdentifierlessData_Parent(int id);
        string GetSqlQuery_Delete_DimKeyword(List<int> dimKeywordIds);
        string GetSqlQuery_Delete_DimNames(List<int> dimNameIds);
        string GetSqlQuery_Delete_DimProfileOnlyPublications(List<int> dimProfileOnlyPublicationIds);
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
        string GetSqlQuery_Select_BrParticipatesInFundingGroup(int dimNameId);
        string GetSqlQuery_Select_CountPublishedItemsInUserprofile(int dimUserProfileId);
        string GetSqlQuery_Select_DimAffiliation(int dimKnownPersonId, List<int> existingIds);
        string GetSqlQuery_Select_DimEducation(int dimKnownPersonId, List<int> existingIds);
        string GetSqlQuery_Select_DimEmailAddrress(int dimKnownPersonId, List<int> existingIds);
        string GetSqlQuery_Select_DimResearcherDescription(int dimKnownPersonId, List<int> existingIds);
        string GetSqlQuery_Select_DimTelephoneNumber(int dimKnownPersonId, List<int> existingIds);
        string GetSqlQuery_Select_DimWebLink(int dimKnownPersonId, List<int> existingIds);
        string GetSqlQuery_Select_FactContribution(int dimNameId);
        string GetSqlQuery_Select_FactFieldValues(int userprofileId);
        string GetSqlQuery_Update_FactFieldValues(int dimUserProfileId, ProfileEditorItemMeta profileEditorItemMeta);
    }
}