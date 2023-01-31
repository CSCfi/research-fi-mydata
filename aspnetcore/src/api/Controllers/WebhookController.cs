using api.Services;
using api.Models.Ttv;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using api.Models.Api;
using api.Models.Orcid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using static api.Models.Common.Constants;

namespace api.Controllers
{
    /*
     * WebhookController handles webhook related actions:
     * - ORCID notification webhook: update ORCID data in a background task.
     * 
     * This controller does not require authorization, since the endpoints must
     * be accessible from 3rd party services. Each endpoint must prevent misuse
     * by, for example, requiring presence of predefined token in the webhook url.
     */
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ILogger<OrcidController> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMemoryCache _cache;

        public WebhookController(IUserProfileService userProfileService,
            ILogger<OrcidController> logger,
            IBackgroundTaskQueue taskQueue,
            IServiceScopeFactory serviceScopeFactory,
            IMemoryCache memoryCache)
        {
            _userProfileService = userProfileService;
            _logger = logger;
            _taskQueue = taskQueue;
            _serviceScopeFactory = serviceScopeFactory;
            _cache = memoryCache;
        }


        /// <summary>
        /// Handle ORCID webhook
        /// 204 No Content should be returned if webhook is successfully received.
        /// </summary>
        [HttpPost]
        [Route("api/webhook/orcid/{webhookOrcidId}")]
        public async Task<IActionResult> HandleOrcidWebhook(string webhookOrcidId)
        {
            string logPrefix = "ORCID webhook: ";
            string orcidAccessToken = "";
            int dimUserprofileId = -1;

            // Validate request data
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _logger.LogInformation($"{logPrefix}received for {webhookOrcidId}");

            // Check that userprofile exists.
            DimUserProfile dimUserProfile = await _userProfileService.GetUserprofile(orcidId: webhookOrcidId);
            if (dimUserProfile == null)
            {
                _logger.LogError($"{logPrefix}user profile not found: {webhookOrcidId}");
                return NoContent();
            }

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(webhookOrcidId);

            // Store values to be used in background task.
            orcidAccessToken = dimUserProfile.OrcidAccessToken;
            dimUserprofileId = dimUserProfile.Id;

            // Get ORCID data in a background task.
            await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
            {
                _logger.LogInformation($"{logPrefix}background update for {webhookOrcidId} started {DateTime.UtcNow}");

                // Create service scope and get required services.
                // Do not use services from controller scope in a background task.
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IOrcidApiService localOrcidApiService = scope.ServiceProvider.GetRequiredService<IOrcidApiService>();
                IOrcidImportService localOrcidImportService = scope.ServiceProvider.GetRequiredService<IOrcidImportService>();

                // Get record json from ORCID member API
                string orcidRecordJson = "";
                try
                {
                    orcidRecordJson = await localOrcidApiService.GetRecordFromMemberApi(webhookOrcidId, orcidAccessToken);
                    _logger.LogInformation($"{logPrefix}background get record for {webhookOrcidId} OK");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{logPrefix}background get record for {webhookOrcidId} failed: {ex}");
                }

                // Import record json into userprofile
                if (orcidRecordJson != "")
                {
                    try
                    {
                        await localOrcidImportService.ImportOrcidRecordJsonIntoUserProfile(dimUserprofileId, orcidRecordJson);
                        _logger.LogInformation($"{logPrefix}background import record for {webhookOrcidId} to userprofile OK");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{logPrefix}background import record for {webhookOrcidId} to userprofile failed: {ex}");
                    }
                }

                _logger.LogInformation($"{logPrefix}background update for {webhookOrcidId} ended {DateTime.UtcNow}");
            });

            return NoContent();
        }
    }
}