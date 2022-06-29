using System.Threading.Tasks;

namespace api.Services
{
    public interface IOrcidImportService
    {
        Task<bool> ImportOrcidRecordJsonIntoUserProfile(int userprofileId, string json);
    }
}