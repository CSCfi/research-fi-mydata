using api.Services;
using api.Models.Ttv;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using api.Models.Log;
using api.Models.Orcid;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
        private readonly ITtvSqlService _ttvSqlService;
        private readonly ILogger<OrcidController> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMemoryCache _cache;

        public WebhookController(IUserProfileService userProfileService,
            ITtvSqlService ttvSqlService,
            ILogger<OrcidController> logger,
            IBackgroundTaskQueue taskQueue,
            IServiceScopeFactory serviceScopeFactory,
            IElasticsearchService elasticsearchService,
            IBackgroundProfiledata backgroundProfiledata,
            IMemoryCache memoryCache)
        {
            _userProfileService = userProfileService;
            _ttvSqlService = ttvSqlService;
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
            string orcidAccessToken = "";
            int dimUserprofileId = -1;
            LogUserIdentification logUserIdentification = new(orcid: webhookOrcidId);

            // Validate request data
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.ORCID_WEBHOOK_RECEIVED,
                    state: LogContent.ActionState.START));

            // Check that userprofile exists.
            DimUserProfile dimUserProfile = await _userProfileService.GetUserprofile(orcidId: webhookOrcidId);
            if (dimUserProfile == null)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ORCID_WEBHOOK_RECEIVED,
                        error: true,
                        message: LogContent.ErrorMessage.USER_PROFILE_NOT_FOUND,
                        state: LogContent.ActionState.START));

                return NoContent();
            }

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(webhookOrcidId);

            // Refresh 'modified' timestamp in table dim_user_profile
            string setModifiedSql = _ttvSqlService.GetSqlQuery_Update_DimUserProfile_Modified(dimUserProfile.Id);
            await _userProfileService.ExecuteRawSql(setModifiedSql); 

            // Store values to be used in background task.
            orcidAccessToken = dimUserProfile.OrcidAccessToken;
            dimUserprofileId = dimUserProfile.Id;

            // Get ORCID data in a background task.
            await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.BACKGROUND_UPDATE,
                        state: LogContent.ActionState.START));

                bool importSuccess = false;

                // Create service scope and get required services.
                // Do not use services from controller scope in a background task.
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IOrcidApiService localOrcidApiService = scope.ServiceProvider.GetRequiredService<IOrcidApiService>();
                IOrcidImportService localOrcidImportService = scope.ServiceProvider.GetRequiredService<IOrcidImportService>();
                IUserProfileService localUserProfileService = scope.ServiceProvider.GetRequiredService<IUserProfileService>();
                TtvContext localTtvContext = scope.ServiceProvider.GetRequiredService<TtvContext>();

                // Get record json from ORCID member API
                string orcidRecordJson = "";
                try
                {
                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.ORCID_RECORD_GET_MEMBER_API,
                            state: LogContent.ActionState.START));

                    orcidRecordJson = await localOrcidApiService.GetRecordFromMemberApi(webhookOrcidId, orcidAccessToken);

                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.ORCID_RECORD_GET_MEMBER_API,
                            state: LogContent.ActionState.COMPLETE));
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.ORCID_RECORD_GET_MEMBER_API,
                            error: true,
                            message: ex.ToString(),
                            state: LogContent.ActionState.FAILED));
                }

                // Import record json into userprofile
                if (orcidRecordJson != "")
                {
                    try
                    {
                        _logger.LogInformation(
                            LogContent.MESSAGE_TEMPLATE,
                            logUserIdentification,
                            new LogApiInfo(
                                action: LogContent.Action.ORCID_RECORD_IMPORT,
                                state: LogContent.ActionState.START));

                        importSuccess = await localOrcidImportService.ImportOrcidRecordJsonIntoUserProfile(dimUserprofileId, orcidRecordJson, logUserIdentification);

                        _logger.LogInformation(
                            LogContent.MESSAGE_TEMPLATE,
                            logUserIdentification,
                            new LogApiInfo(
                                action: LogContent.Action.ORCID_RECORD_IMPORT,
                                state: LogContent.ActionState.COMPLETE));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            LogContent.MESSAGE_TEMPLATE,
                            logUserIdentification,
                            new LogApiInfo(
                                action: LogContent.Action.ORCID_RECORD_IMPORT,
                                error: true,
                                message: ex.ToString(),
                                state: LogContent.ActionState.FAILED));
                    }
                }

                // Import additional data
                if (importSuccess)
                {
                    try
                    {
                        _logger.LogInformation(
                            LogContent.MESSAGE_TEMPLATE,
                            logUserIdentification,
                            new LogApiInfo(
                                action: LogContent.Action.ORCID_RECORD_IMPORT_ADDITIONAL,
                                state: LogContent.ActionState.START));

                        List<FactFieldValue> ffvs = await localTtvContext.FactFieldValues.Where(
                                ffv =>
                                    ffv.DimUserProfileId == dimUserprofileId &&
                                    ffv.DimPidIdOrcidPutCode > 0 &&
                                    ffv.DimProfileOnlyFundingDecisionId > 0
                                )
                            .Include(ffv => ffv.DimProfileOnlyFundingDecision)
                            .Include(ffv => ffv.DimPidIdOrcidPutCodeNavigation).ToListAsync();

                        await localOrcidImportService.ImportAdditionalData(factFieldValues: ffvs, orcidAccessToken: orcidAccessToken, logUserIdentification: logUserIdentification);
                        await localTtvContext.SaveChangesAsync();

                        _logger.LogInformation(
                            LogContent.MESSAGE_TEMPLATE,
                            logUserIdentification,
                            new LogApiInfo(
                                action: LogContent.Action.ORCID_RECORD_IMPORT_ADDITIONAL,
                                state: LogContent.ActionState.COMPLETE));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            LogContent.MESSAGE_TEMPLATE,
                            logUserIdentification,
                            new LogApiInfo(
                                action: LogContent.Action.ORCID_RECORD_IMPORT_ADDITIONAL,
                                error: true,
                                message: ex.ToString(),
                                state: LogContent.ActionState.FAILED));
                    }
                }

                // After successful ORCID import update Elasticsearch index
                if (importSuccess)
                {
                    await localUserProfileService.UpdateProfileInElasticsearch(
                        orcidId: webhookOrcidId,
                        userprofileId: dimUserProfile.Id,
                        logUserIdentification: logUserIdentification);
                }

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.BACKGROUND_UPDATE,
                        state: LogContent.ActionState.COMPLETE));
            });

            return NoContent();
        }
    }
}