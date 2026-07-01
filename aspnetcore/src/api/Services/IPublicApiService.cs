using System.Threading.Tasks;
using api.Models.Ai;

namespace api.Services
{
    public interface IPublicApiService
    {
        string GetProfileDataForPublicApi(string nationalIdentificationNumber);
        string GetUsernameFromNationalIdentificationNumber(string nationalIdentificationNumber);
        Task<string?> GetOrcidIdFromKeycloak(string username);
    }
}