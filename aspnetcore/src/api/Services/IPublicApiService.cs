using api.PublicApiContracts;

namespace api.Services
{
    public interface IPublicApiService
    {
        PublicApiHelloResponse GetHelloMessage(string username);
        string GetUsernameFromNationalIdentificationNumber(string nationalIdentificationNumber);
    }
}