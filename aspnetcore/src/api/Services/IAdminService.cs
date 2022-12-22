using System.Threading.Tasks;

namespace api.Services
{
    public interface IAdminService
    {
        Task RegisterOrcidWebhookForAllUserprofiles();
        Task RegisterOrcidWebhookForSingleUserprofile(string webhookOrcidId);
        Task UnregisterOrcidWebhookForAllUserprofiles();
        Task UnregisterOrcidWebhookForSingleUserprofile(string webhookOrcidId);
    }
}