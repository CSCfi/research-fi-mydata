using api.Models.Log;
using System.Threading.Tasks;

namespace api.Services
{
    public interface IAdminService
    {
        Task<bool> AddNewTtvDataInUserProfileBackground(int dimUserProfileId, LogUserIdentification logUserIdentification);
        Task RegisterOrcidWebhookForAllUserprofiles();
        Task RegisterOrcidWebhookForSingleUserprofile(string webhookOrcidId);
        Task UnregisterOrcidWebhookForAllUserprofiles();
        Task UnregisterOrcidWebhookForSingleUserprofile(string webhookOrcidId);
    }
}