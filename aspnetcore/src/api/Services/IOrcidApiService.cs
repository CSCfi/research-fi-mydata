using System.Threading.Tasks;

namespace api.Services
{
    public interface IOrcidApiService
    {
        string GetOrcidRecordPath(string orcidId);
        Task<string> GetRecordFromPublicApi(string orcidId);
        Task<string> GetRecordFromMemberApi(string orcidId, string orcidAccessToken);
    }
}