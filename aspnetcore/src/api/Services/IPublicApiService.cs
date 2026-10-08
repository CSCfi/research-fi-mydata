using System.Threading.Tasks;
using ResearchFi.PersonPublicApi;

namespace api.Services
{
    public interface IPublicApiService
    {
        Task<ProfileDataResponse> GetProfileDataForPublicApi(string personKeyIdentifier);
        string GetUsernameFromNationalIdentificationNumber(string nationalIdentificationNumber);
    }
}