using api.Services;
using api.Models.Api;
using api.Models.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace api.Controllers
{
    /*
     * UserProfileController handles creation, existence check and deletion of userprofile.
     */
    [Route("api/userprofile")]
    [ApiController]
    [Authorize(Policy = "RequireScopeApi1AndClaimOrcid")]
    public class UserProfileController : TtvControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IKeycloakAdminApiService _keycloakAdminApiService;
        private readonly IOrcidApiService _orcidApiService;
        private readonly ILogger<UserProfileController> _logger;
        private readonly IMemoryCache _cache;

        public UserProfileController(
            IUserProfileService userProfileService,
            IKeycloakAdminApiService keycloakAdminApiService,
            IOrcidApiService orcidApiService,
            ILogger<UserProfileController> logger,
            IMemoryCache memoryCache)
        {
            _userProfileService = userProfileService;
            _keycloakAdminApiService = keycloakAdminApiService;
            _orcidApiService = orcidApiService;
            _logger = logger;
            _cache = memoryCache;
        }

        /// <summary>
        /// Check if user profile exists.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            if (await _userProfileService.UserprofileExistsForOrcidId(orcidId: GetOrcidId()))
            {
                return Ok(new ApiResponse(success: true));
            }
            else
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }
        }

        /// <summary>
        /// Create user profile.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create()
        {
            // Get ORCID id.
            string orcidId = GetOrcidId();
            // Log request
            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                this.GetLogUserIdentification(),
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_CREATE,
                    state: LogContent.ActionState.START));

            // Return immediately, if profile already exist.
            // Log error, but pass silently in user profile API.
            if (await _userProfileService.UserprofileExistsForOrcidId(orcidId: orcidId))
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    this.GetLogUserIdentification(),
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_CREATE,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: "already exists"));
                return Ok(new ApiResponse(success: true));
            }

            // Create profile
            try
            {
                await _userProfileService.CreateProfile(orcidId: orcidId, logUserIdentification: this.GetLogUserIdentification());
            }
            catch
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    this.GetLogUserIdentification(),
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_CREATE,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: LogContent.ActionState.FAILED));
                return Ok(new ApiResponse(success: false, reason: "create profile failed"));
            }

            // Register ORCID webhook. Continue profile creation in case of error.
            if (_orcidApiService.IsOrcidWebhookEnabled())
            {
                try
                {
                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        this.GetLogUserIdentification(),
                        new LogApiInfo(
                            action: LogContent.Action.ORCID_WEBHOOK_REGISTER,
                            state: LogContent.ActionState.START));

                    await _orcidApiService.RegisterOrcidWebhook(orcidId: orcidId);

                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        this.GetLogUserIdentification(),
                        new LogApiInfo(
                            action: LogContent.Action.ORCID_WEBHOOK_REGISTER,
                            state: LogContent.ActionState.COMPLETE));
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        LogContent.MESSAGE_TEMPLATE,
                        this.GetLogUserIdentification(),
                        new LogApiInfo(
                            action: LogContent.Action.ORCID_WEBHOOK_REGISTER,
                            state: LogContent.ActionState.FAILED,
                            error: true,
                            message: ex.ToString()));
                }
            }
            else
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    this.GetLogUserIdentification(),
                    new LogApiInfo(
                        action: LogContent.Action.ORCID_WEBHOOK_REGISTER,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: "disabled in configuration"));
            }

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                this.GetLogUserIdentification(),
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_CREATE,
                    state: LogContent.ActionState.COMPLETE));
            return Ok(new ApiResponse(success: true));
        }


        /// <summary>
        /// Delete user profile.
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete()
        {
            // Get ORCID id.
            string orcidId = GetOrcidId();

            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();

            // Log request.
            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_DELETE,
                    state: LogContent.ActionState.START));

            // Return immediately, if profile does not exist.
            if (!await _userProfileService.UserprofileExistsForOrcidId(orcidId: orcidId))
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_DELETE,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: "profile does not exist"));
                return Ok(new ApiResponse(success: true));
            }

            // Get userprofile id.
            int userprofileId = await _userProfileService.GetUserprofileId(orcidId);

            // Delete profile data from database
            bool deleteSuccess = false;
            try
            {
                deleteSuccess = await _userProfileService.DeleteProfileDataAsync(userprofileId: userprofileId, logUserIdentification: this.GetLogUserIdentification());
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_DELETE,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: ex.ToString()));
            }

            if (deleteSuccess)
            {
                // Log deletion
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_DELETE,
                        state: LogContent.ActionState.START));

                // Remove cached profile data response. Cache key is ORCID ID.
                _cache.Remove(orcidId);

                // Remove entry from Elasticsearch index.
                await _userProfileService.DeleteProfileFromElasticsearch(
                    orcidId: orcidId,
                    logUserIdentification: logUserIdentification);

                // Keycloak: logout user
                await _keycloakAdminApiService.LogoutUser(this.GetBearerTokenFromHttpRequest(), logUserIdentification);

                // Keycloak: remove user
                await _keycloakAdminApiService.RemoveUser(this.GetBearerTokenFromHttpRequest(), logUserIdentification);

                // Unregister ORCID webhook. Continue profile deletion in case of error.
                if (_orcidApiService.IsOrcidWebhookEnabled())
                {
                    try
                    {
                        _logger.LogInformation(
                            LogContent.MESSAGE_TEMPLATE,
                            logUserIdentification,
                            new LogApiInfo(
                                action: LogContent.Action.ORCID_WEBHOOK_UNREGISTER,
                                state: LogContent.ActionState.START));

                        await _orcidApiService.UnregisterOrcidWebhook(orcidId: orcidId);

                        _logger.LogInformation(
                            LogContent.MESSAGE_TEMPLATE,
                            logUserIdentification,
                            new LogApiInfo(
                                action: LogContent.Action.ORCID_WEBHOOK_UNREGISTER,
                                state: LogContent.ActionState.COMPLETE));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            LogContent.MESSAGE_TEMPLATE,
                            logUserIdentification,
                            new LogApiInfo(
                                action: LogContent.Action.ORCID_WEBHOOK_UNREGISTER,
                                state: LogContent.ActionState.FAILED,
                                error: true,
                                message: ex.ToString()));    
                    }
                }
                else
                {
                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.ORCID_WEBHOOK_UNREGISTER,
                            state: LogContent.ActionState.FAILED,
                            error: true,
                            message: "disabled in configuration"));
                }

                return Ok(new ApiResponse(success: true));
            }
            else
            {
                // Log error
                string msg = "database delete failed";
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_DELETE,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: msg));

                return Ok(new ApiResponse(success: false, reason: msg));
            }
        }
    }
}