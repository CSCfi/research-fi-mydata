using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.Orcid;
using api.Models.ProfileEditor;
using api.Models.Ttv;

namespace api.Services
{
    public interface IUserProfileService
    {
        Task<DimEmailAddrress> AddOrUpdateDimEmailAddress(string emailAddress, int dimKnownPersonId, int dimRegisteredDataSourceId);
        Task<DimName> AddOrUpdateDimName(string lastName, string firstNames, int dimKnownPersonId, int dimRegisteredDataSourceId);
        Task<DimResearcherDescription> AddOrUpdateDimResearcherDescription(string description_fi, string description_en, string description_sv, int dimKnownPersonId, int dimRegisteredDataSourceId);
        Task AddTtvDataToUserProfile(DimKnownPerson dimKnownPerson, DimUserProfile dimUserProfile);
        Task AddTtvPublications(DimKnownPerson dimKnownPerson, DimUserProfile dimUserProfile);
        bool CanDeleteFactFieldValueRelatedData(FactFieldValue ffv);
        Task CreateProfile(string orcidId);
        Task DeleteProfileDataAsync(int userprofileId);
        Task ExecuteRawSql(string sql);
        DimOrcidPublication GetEmptyDimOrcidPublication();
        DimPid GetEmptyDimPid();
        FactFieldValue GetEmptyFactFieldValue();
        FactFieldValue GetEmptyFactFieldValueDemo();
        List<int> GetFieldIdentifiers();
        Task<ProfileEditorDataResponse> GetProfileDataAsync(int userprofileId);
        Task<ProfileEditorDataResponse2> GetProfileDataAsync2(int userprofileId);
        Task<int> GetUserprofileId(string orcidId);
        Task UpdateOrcidTokensInDimUserProfile(int dimUserProfileId, OrcidTokens orcidTokens);
        Task<bool> UserprofileExistsForOrcidId(string orcidId);
    }
}