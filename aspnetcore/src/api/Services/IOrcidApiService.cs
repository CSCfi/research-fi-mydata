using System.Threading.Tasks;

namespace api.Services
{
    public interface IOrcidApiService
    {
        string GetOrcidRecordPath(string orcidId);
        Task<string> GetRecord(string orcidId, string orcidAccessToken);
    }
}