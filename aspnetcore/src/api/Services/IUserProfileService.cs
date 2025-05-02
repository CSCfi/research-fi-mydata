using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.Log;
using api.Models.Orcid;
using api.Models.ProfileEditor;
using api.Models.ProfileEditor.Items;
using api.Models.Ttv;

namespace api.Services
{
    public interface IUserProfileService
    {
        Task<DimEmailAddrress> AddOrUpdateDimEmailAddress(string emailAddress, int dimKnownPersonId, int dimRegisteredDataSourceId);
        Task<DimName> AddOrUpdateDimName(string lastName, string firstNames, int dimKnownPersonId, int dimRegisteredDataSourceId);
        Task<DimResearcherDescription> AddOrUpdateDimResearcherDescription(string description_fi, string description_en, string description_sv, int dimKnownPersonId, int dimRegisteredDataSourceId);
        Task AddTtvDataToUserProfile(DimKnownPerson dimKnownPerson, DimUserProfile dimUserProfile, LogUserIdentification logUserIdentification);
        bool CanDeleteFactFieldValueRelatedData(FactFieldValue ffv);
        Task CreateProfile(string orcidId, LogUserIdentification logUserIdentification);
        Task<bool> DeleteProfileDataAsync(int userprofileId, LogUserIdentification logUserIdentification);
        Task ExecuteRawSql(string sql);
        DimProfileOnlyDataset GetEmptyDimProfileOnlyDataset();
        DimProfileOnlyFundingDecision GetEmptyDimProfileOnlyFundingDecision();
        DimProfileOnlyPublication GetEmptyDimProfileOnlyPublication();
        DimProfileOnlyResearchActivity GetEmptyDimProfileOnlyResearchActivity();
        DimPid GetEmptyDimPid();
        FactFieldValue GetEmptyFactFieldValue();
        FactFieldValue GetEmptyFactFieldValueDemo();
        List<int> GetFieldIdentifiers();
        DimKnownPerson GetNewDimKnownPerson(string orcidId, DateTime currentDateTime);
        Task<ProfileEditorDataResponse> GetProfileDataAsync(int userprofileId, LogUserIdentification logUserIdentification, bool forElasticsearch = false);
        Task<DimUserProfile> GetUserprofile(string orcidId);
        Task<DimUserProfile> GetUserprofileTracking(string orcidId);
        Task<DimUserProfile> GetUserprofileById(int Id);
        Task<(bool UserProfileExists, int UserProfileId)> GetUserprofileIdForOrcidId(string orcidId);
        Task<bool> IsUserprofilePublished(int dimUserProfileId);
        Task UpdateOrcidTokensInDimUserProfile(int dimUserProfileId, OrcidTokens orcidTokens);
        Task<bool> DeleteProfileFromElasticsearch(string orcidId, LogUserIdentification logUserIdentification, string logAction = LogContent.Action.ELASTICSEARCH_UPDATE);
        Task<bool> UpdateProfileInElasticsearch(string orcidId, int userprofileId, LogUserIdentification logUserIdentification, string logAction = LogContent.Action.ELASTICSEARCH_UPDATE);
        Task HideProfile(string orcidId, LogUserIdentification logUserIdentification);
        Task RevealProfile(string orcidId, LogUserIdentification logUserIdentification);
        ProfileSettings GetProfileSettings(DimUserProfile dimUserProfile);
        Task SaveProfileSettings(string orcidId, DimUserProfile dimUserProfile, ProfileSettings profileSettings, LogUserIdentification logUserIdentification);
        string GetCMemoryCacheKey_UserProfile(string orcidId);
        string GetCMemoryCacheKey_ProfileSettings(string orcidId);
        string GetCMemoryCacheKey_UserChoices(string orcidId);
        string GetCMemoryCacheKey_SharePurposes();
        string GetCMemoryCacheKey_SharePermissions();
        string GetCMemoryCacheKey_GivenPermissions(string orcidId);
        Task SetModifiedTimestampInUserProfile(int Id);
        bool SetFactFieldValuesShow(FactFieldValue ffv);
    }
}