using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.Orcid;
using api.Models.ProfileEditor.Items;
using api.Models.Ttv;

namespace api.Services
{
    public interface IUserProfileService
    {
        Task<DimEmailAddrress> AddOrUpdateDimEmailAddress(string emailAddress, int dimKnownPersonId, int dimRegisteredDataSourceId);
        Task<DimName> AddOrUpdateDimName(string lastName, string firstNames, int dimKnownPersonId, int dimRegisteredDataSourceId);
        Task<DimResearcherDescription> AddOrUpdateDimResearcherDescription(string description_fi, string description_en, string description_sv, int dimKnownPersonId, int dimRegisteredDataSourceId);
        Task AddTtvDataToUserProfile(DimKnownPerson dimKnownPerson, DimUserProfile dimUserProfile);
        bool CanDeleteFactFieldValueRelatedData(FactFieldValue ffv);
        Task CreateProfile(string orcidId);
        Task<bool> DeleteProfileDataAsync(int userprofileId);
        Task ExecuteRawSql(string sql);
        DimOrcidPublication GetEmptyDimOrcidPublication();
        DimPid GetEmptyDimPid();
        FactFieldValue GetEmptyFactFieldValue();
        FactFieldValue GetEmptyFactFieldValueDemo();
        List<int> GetFieldIdentifiers();
        DimKnownPerson GetNewDimKnownPerson(string orcidId, DateTime currentDateTime);
        Task<ProfileEditorDataResponse> GetProfileDataAsync(int userprofileId, bool forElasticsearch = false);
        Task<int> GetUserprofileId(string orcidId);
        Task UpdateOrcidTokensInDimUserProfile(int dimUserProfileId, OrcidTokens orcidTokens);
        Task<bool> UserprofileExistsForOrcidId(string orcidId);
    }
}