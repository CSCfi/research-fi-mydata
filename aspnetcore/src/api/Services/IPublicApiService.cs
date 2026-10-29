using System.Threading.Tasks;
using ResearchFi.ProfilePublicApi;

namespace api.Services
{
    public interface IPublicApiService
    {
        Task<ProfileDataResponse> GetProfileDataForPublicApi(ProfileDataRequest request);
        string GetUsernameFromNationalIdentificationNumber(string nationalIdentificationNumber);
    }
}