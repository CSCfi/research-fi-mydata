using api.Services;
using api.Models.Api;
using api.Models.Log;
using api.Models.Ttv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Reflection.Metadata;
using api.Models.Common;
using Nest;

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

            // Check that userprofile exists.
            (bool userprofileExists, int userprofileId) = await _userProfileService.GetUserprofileIdForOrcidId(GetOrcidId());
            if (userprofileExists)
            {
                return Ok(new ApiResponse(success: true));
            }
            else
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.PROFILE_NOT_FOUND));
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

            // Return immediately, if profile already exists.
            // Log error, but pass silently in user profile API.
            (bool userprofileExists, int userprofileId) = await _userProfileService.GetUserprofileIdForOrcidId(GetOrcidId());
            if (userprofileExists)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    this.GetLogUserIdentification(),
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_CREATE,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: "profile already exists"));
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
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.PROFILE_CREATE_FAILED));
            }

            // Register ORCID webhook. Continue profile creation in case of error.
            if (_orcidApiService.IsOrcidWebhookEnabled())
            {
                try
                {
                    await _orcidApiService.RegisterOrcidWebhook(orcidId: orcidId);
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
                        state: LogContent.ActionState.CANCELLED,
                        error: false,
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
            (bool userprofileExists, int userprofileId) = await _userProfileService.GetUserprofileIdForOrcidId(GetOrcidId());
            if (!userprofileExists)
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

            // Get userprofile
            DimUserProfile userProfile = await _userProfileService.GetUserprofile(orcidId);

            // Delete profile data from database
            // Start stopwatch to measure time taken for database delete
            var deleteProfileDataStopwatch = System.Diagnostics.Stopwatch.StartNew();
            bool deleteSuccess = false;
            try
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_DELETE_DATABASE,
                        state: LogContent.ActionState.START));
                deleteSuccess = await _userProfileService.DeleteProfileDataAsync(userprofileId: userProfile.Id, logUserIdentification: this.GetLogUserIdentification());
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_DELETE_DATABASE,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: ex.ToString()));
            }

            // Stop stopwatch after delete operation
            deleteProfileDataStopwatch.Stop();

            if (deleteSuccess)
            {
                // Log successful database delete
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_DELETE_DATABASE,
                        state: LogContent.ActionState.COMPLETE,
                        message: $"took {deleteProfileDataStopwatch.ElapsedMilliseconds} ms"));

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
                        await _orcidApiService.UnregisterOrcidWebhook(orcidId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            LogContent.MESSAGE_TEMPLATE,
                            logUserIdentification,
                            new LogApiInfo(
                                action: LogContent.Action.ADMIN_WEBHOOK_ORCID_UNREGISTER,
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
                            state: LogContent.ActionState.CANCELLED,
                            error: false,
                            message: "disabled in configuration"));
                }

                // Revoke ORCID access token
                try
                {
                    await _orcidApiService.RevokeToken(logUserIdentification, userProfile.OrcidRefreshToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.ORCID_REVOKE_TOKEN,
                            state: LogContent.ActionState.FAILED,
                            error: true,
                            message: ex.ToString()));
                }

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_DELETE,
                        state: LogContent.ActionState.COMPLETE));

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