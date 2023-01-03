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
        private readonly IElasticsearchService _elasticsearchService;
        private readonly IKeycloakAdminApiService _keycloakAdminApiService;
        private readonly IOrcidApiService _orcidApiService;
        private readonly ILogger<UserProfileController> _logger;
        private readonly IMemoryCache _cache;
        private readonly IBackgroundTaskQueue _taskQueue;

        public UserProfileController(
            IElasticsearchService elasticsearchService,
            IUserProfileService userProfileService,
            IKeycloakAdminApiService keycloakAdminApiService,
            IOrcidApiService orcidApiService,
            ILogger<UserProfileController> logger,
            IMemoryCache memoryCache,
            IBackgroundTaskQueue taskQueue)
        {
            _userProfileService = userProfileService;
            _elasticsearchService = elasticsearchService;
            _keycloakAdminApiService = keycloakAdminApiService;
            _orcidApiService = orcidApiService;
            _logger = logger;
            _cache = memoryCache;
            _taskQueue = taskQueue;
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
                "{@UserIdentification}, {@ApiInfo}",
                this.GetUserIdentification(),
                new ApiInfo(action: LogContent.Action.PROFILE_CREATE, message: "start"));

            // Return immediately, if profile already exist.
            // Log error, but pass silently in user profile API.
            if (await _userProfileService.UserprofileExistsForOrcidId(orcidId: orcidId))
            {
                _logger.LogError(
                    "{@UserIdentification}, {@ApiInfo}",
                    this.GetUserIdentification(),
                    new ApiInfo(action: LogContent.Action.PROFILE_CREATE, success: false, message: "already exists"));
                return Ok(new ApiResponse(success: true));
            }

            // Create profile
            try
            {
                await _userProfileService.CreateProfile(orcidId: orcidId);
            }
            catch
            {
                _logger.LogError(
                    "{@UserIdentification}, {@ApiInfo}",
                    this.GetUserIdentification(),
                    new ApiInfo(action: LogContent.Action.PROFILE_CREATE, success: false, message: "failed"));
                return Ok(new ApiResponse(success: false, reason: "create profile failed"));
            }

            // Register ORCID webhook. Continue profile creation in case of error.
            if (_orcidApiService.IsOrcidWebhookEnabled())
            {
                try
                {
                    await _orcidApiService.RegisterOrcidWebhook(orcidId: orcidId);
                    _logger.LogInformation(
                        "{@UserIdentification}, {@ApiInfo}",
                        this.GetUserIdentification(),
                        new ApiInfo(action: LogContent.Action.ORCID_WEBHOOK_REGISTER, success: true));
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        "{@UserIdentification}, {@ApiInfo}",
                        this.GetUserIdentification(),
                        new ApiInfo(action: LogContent.Action.ORCID_WEBHOOK_REGISTER, success: false, message: ex.ToString()));
                }
            }
            else
            {
                _logger.LogInformation(
                    "{@UserIdentification}, {@ApiInfo}",
                    this.GetUserIdentification(),
                    new ApiInfo(action: LogContent.Action.ORCID_WEBHOOK_REGISTER, message: "disabled in configuration"));
            }

            _logger.LogInformation(
                "{@UserIdentification}, {@ApiInfo}",
                this.GetUserIdentification(),
                new ApiInfo(action: LogContent.Action.PROFILE_CREATE, success: true));
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
            // Log request.
            _logger.LogInformation(
                "{@UserIdentification}, {@ApiInfo}",
                this.GetUserIdentification(),
                new ApiInfo(action: LogContent.Action.PROFILE_DELETE, message: "start processing"));

            // Return immediately, if profile does not exist.
            if (!await _userProfileService.UserprofileExistsForOrcidId(orcidId: orcidId))
            {
                _logger.LogInformation(
                    "{@UserIdentification}, {@ApiInfo}",
                    this.GetUserIdentification(),
                    new ApiInfo(action: LogContent.Action.PROFILE_DELETE, message: "nothing deleted, profile does not exist"));
                return Ok(new ApiResponse(success: true));
            }

            // Get userprofile id.
            int userprofileId = await _userProfileService.GetUserprofileId(orcidId);

            // Delete profile data from database
            bool deleteSuccess = false;
            try
            {
                deleteSuccess = await _userProfileService.DeleteProfileDataAsync(userprofileId: userprofileId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "{@UserIdentification}, {@ApiInfo}",
                    this.GetUserIdentification(),
                    new ApiInfo(action: LogContent.Action.PROFILE_DELETE, success: false, message: ex.ToString()));
            }

            if (deleteSuccess)
            {
                // Log deletion
                _logger.LogInformation(
                    "{@UserIdentification}, {@ApiInfo}",
                    this.GetUserIdentification(),
                    new ApiInfo(action: LogContent.Action.PROFILE_DELETE, success: true));

                // Remove cached profile data response. Cache key is ORCID ID.
                _cache.Remove(orcidId);

                // Remove entry from Elasticsearch index in a background task.
                // ElasticsearchService is singleton, no need to create local scope.
                if (_elasticsearchService.IsElasticsearchSyncEnabled())
                {
                    await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
                    {
                        // Update Elasticsearch person index.
                        bool deleteSuccess = await _elasticsearchService.DeleteEntryFromElasticsearchPersonIndex(orcidId);
                        if (deleteSuccess)
                        {
                            _logger.LogInformation(
                                "{@UserIdentification}, {@ApiInfo}",
                                this.GetUserIdentification(),
                                new ApiInfo(action: LogContent.Action.ELASTICSEARCH_DELETE, success: true));
                        }
                        else
                        {
                            _logger.LogError(
                                "{@UserIdentification}, {@ApiInfo}",
                                this.GetUserIdentification(),
                                new ApiInfo(action: LogContent.Action.ELASTICSEARCH_DELETE, success: false));
                        }
                    });
                }

                // Keycloak: logout user
                await _keycloakAdminApiService.LogoutUser(this.GetBearerTokenFromHttpRequest());

                // Keycloak: remove user
                await _keycloakAdminApiService.RemoveUser(this.GetBearerTokenFromHttpRequest());

                // Unregister ORCID webhook. Continue profile deletion in case of error.
                if (_orcidApiService.IsOrcidWebhookEnabled())
                {
                    try
                    {
                        await _orcidApiService.UnregisterOrcidWebhook(orcidId: orcidId);
                        _logger.LogInformation(
                            "{@UserIdentification}, {@ApiInfo}",
                            this.GetUserIdentification(),
                            new ApiInfo(action: LogContent.Action.ORCID_WEBHOOK_UNREGISTER, success: true));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            "{@UserIdentification}, {@ApiInfo}",
                            this.GetUserIdentification(),
                            new ApiInfo(action: LogContent.Action.ORCID_WEBHOOK_UNREGISTER, success: false, message: ex.ToString()));    
                    }
                }
                else
                {
                    _logger.LogInformation(
                        "{@UserIdentification}, {@ApiInfo}",
                        this.GetUserIdentification(),
                        new ApiInfo(action: LogContent.Action.ORCID_WEBHOOK_UNREGISTER, success: false, message: "disabled in configuration"));
                }

                return Ok(new ApiResponse(success: true));
            }
            else
            {
                // Log error
                string msg = "database delete failed";
                _logger.LogError(
                    "{@UserIdentification}, {@ApiInfo}",
                    this.GetUserIdentification(),
                    new ApiInfo(action: LogContent.Action.PROFILE_DELETE, success: false, message: "database delete failed"));

                return Ok(new ApiResponse(success: false, reason: msg));
            }
        }
    }
}