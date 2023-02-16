using api.Services;
using api.Models.Ttv;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using api.Models.Log;

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
        private readonly IElasticsearchService _elasticsearchService;
        private readonly IBackgroundProfiledata _backgroundProfiledata;
        private readonly IMemoryCache _cache;

        public WebhookController(IUserProfileService userProfileService,
            ILogger<OrcidController> logger,
            IBackgroundTaskQueue taskQueue,
            IServiceScopeFactory serviceScopeFactory,
            IElasticsearchService elasticsearchService,
            IBackgroundProfiledata backgroundProfiledata,
            IMemoryCache memoryCache)
        {
            _userProfileService = userProfileService;
            _logger = logger;
            _taskQueue = taskQueue;
            _serviceScopeFactory = serviceScopeFactory;
            _elasticsearchService = elasticsearchService;
            _backgroundProfiledata = backgroundProfiledata;
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

            // Check if the profile is publihed. Published profile should be updated after ORCID import
            bool isUserprofilePublished = await _userProfileService.IsUserprofilePublished(dimUserprofileId);

            // Get ORCID data in a background task.
            await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
            {
                _logger.LogInformation($"{logPrefix}background update for {webhookOrcidId} started {DateTime.UtcNow}");

                bool importSuccess = false;

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
                        importSuccess = await localOrcidImportService.ImportOrcidRecordJsonIntoUserProfile(dimUserprofileId, orcidRecordJson);
                        _logger.LogInformation($"{logPrefix}background import record for {webhookOrcidId} to userprofile OK");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{logPrefix}background import record for {webhookOrcidId} to userprofile failed: {ex}");
                    }
                }

                // If user profile is published, then after successful ORCID import update Elasticsearch index
                if (importSuccess && isUserprofilePublished && _elasticsearchService.IsElasticsearchSyncEnabled())
                {
                    LogUserIdentification logUserIdentification = new(orcid: webhookOrcidId);
                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.ELASTICSEARCH_UPDATE,
                            state: LogContent.ActionState.START));

                    // Get Elasticsearch person entry from profile data.
                    Models.Elasticsearch.ElasticsearchPerson person =
                        await _backgroundProfiledata.GetProfiledataForElasticsearch(
                            orcidId: webhookOrcidId,
                            userprofileId: dimUserprofileId,
                            logUserIdentification: logUserIdentification);
                    // Update Elasticsearch person index.
                    await _elasticsearchService.UpdateEntryInElasticsearchPersonIndex(webhookOrcidId, person, logUserIdentification);

                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.ELASTICSEARCH_UPDATE,
                            state: LogContent.ActionState.COMPLETE));
                }

                _logger.LogInformation($"{logPrefix}background update for {webhookOrcidId} ended {DateTime.UtcNow}");
            });

            return NoContent();
        }
    }
}