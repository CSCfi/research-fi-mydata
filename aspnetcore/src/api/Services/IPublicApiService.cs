using System.Threading.Tasks;
using ResearchFi.ProfilePublicApi;

namespace api.Services
{
    public interface IPublicApiService
    {
        Task<ProfileDataResponse> GetProfileDataForPublicApi(ProfileDataRequest request);
        // Maps published items without visibility or consent checks; callers must authorize access.
        Task<PersonProfileData> GetPersonProfileData(int userProfileId, string orcidId);
        string GetUsernameFromNationalIdentificationNumber(string nationalIdentificationNumber);
    }
}