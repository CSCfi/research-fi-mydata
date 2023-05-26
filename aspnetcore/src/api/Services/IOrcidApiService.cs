using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace api.Services
{
    public interface IOrcidApiService
    {
        IConfiguration Configuration { get; }

        Task<string> GetDataFromMemberApi(String path, String orcidAccessToken);
        Task<string> GetDataFromPublicApi(String path, String orcidAccessToken = "");
        string GetOrcidRecordPath(string orcidId);
        string GetOrcidWebhookAccessToken();
        string GetOrcidWebhookCallbackUri(string orcidId);
        string GetOrcidWebhookRegistrationUri(string orcidId);
        Task<string> GetRecordFromMemberApi(string orcidId, string orcidAccessToken);
        Task<string> GetRecordFromPublicApi(string orcidId);
        string GetUrlEncodedString(string uriToEncode);
        bool IsOrcidWebhookEnabled();
        Task<bool> RegisterOrcidWebhook(string orcidId);
        Task<bool> UnregisterOrcidWebhook(string orcidId);
    }
}