using api.Models.Log;
using Microsoft.AspNetCore.Http;
using System.Threading;
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
        Task UpdateAllUserprofilesInElasticsearch(LogUserIdentification logUserIdentification, string requestScheme, HostString requestHost);
        Task<bool> UpdateUserprofileInElasticsearch(int dimUserProfileId, LogUserIdentification logUserIdentification);
        Task UpdateOrcidDataForAllUserprofiles(LogUserIdentification logUserIdentification, string requestScheme, HostString requestHost);
    }
}