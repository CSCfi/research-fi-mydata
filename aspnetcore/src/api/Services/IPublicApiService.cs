using ResearchFi.PersonPublicApi;

namespace api.Services
{
    public interface IPublicApiService
    {
        ProfileDataResponse GetProfileDataForPublicApi(string username);
        string GetUsernameFromNationalIdentificationNumber(string nationalIdentificationNumber);
    }
}