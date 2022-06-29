using api.Services;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;

namespace api.Controllers
{
    /*
     * UserProfileController handles creation, existence check and deletion of userprofile.
     */
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RequireScopeApi1AndClaimOrcid")]
    public class UserProfileController : TtvControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly IKeycloakAdminApiService _keycloakAdminApiService;
        private readonly ILogger<UserProfileController> _logger;
        private readonly IMemoryCache _cache;
        private readonly IBackgroundTaskQueue _taskQueue;

        public UserProfileController(
            IElasticsearchService elasticsearchService,
            IUserProfileService userProfileService,
            IKeycloakAdminApiService keycloakAdminApiService,
            ILogger<UserProfileController> logger,
            IMemoryCache memoryCache,
            IBackgroundTaskQueue taskQueue)
        {
            _userProfileService = userProfileService;
            _elasticsearchService = elasticsearchService;
            _keycloakAdminApiService = keycloakAdminApiService;
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
            _logger.LogInformation(this.GetLogPrefix() + " create profile request");

            // Return immediately, if profile already exist.
            if (await _userProfileService.UserprofileExistsForOrcidId(orcidId: orcidId))
            {
                _logger.LogInformation(this.GetLogPrefix() + " profile already exists");
                return Ok(new ApiResponse(success: true));
            }

            // Create profile
            try
            {
                await _userProfileService.CreateProfile(orcidId: orcidId);
            }
            catch
            {
                _logger.LogError(this.GetLogPrefix() + " profile creation failed");
                return Ok(new ApiResponse(success: false, reason: "profile creation failed"));
            }

            _logger.LogInformation(this.GetLogPrefix() + " profile created");
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
            _logger.LogInformation(this.GetLogPrefix() + " delete profile request");

            // Return immediately, if profile does not exist.
            if (!await _userProfileService.UserprofileExistsForOrcidId(orcidId: orcidId))
            {
                _logger.LogInformation(this.GetLogPrefix() + " nothing deleted, profile does not exist");
                return Ok(new ApiResponse(success: true));
            }

            // Get userprofile id.
            int userprofileId = await _userProfileService.GetUserprofileId(orcidId);

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);

            // Remove entry from Elasticsearch index in a background task.
            if (_elasticsearchService.IsElasticsearchSyncEnabled())
            {
                await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
                {
                    _logger.LogInformation($"Elasticsearch removal of {orcidId} started {DateTime.UtcNow}");
                    // Update Elasticsearch person index.
                    await _elasticsearchService.DeleteEntryFromElasticsearchPersonIndex(orcidId);
                    _logger.LogInformation($"Elasticsearch removal of {orcidId} completed {DateTime.UtcNow}");
                });
            }

            // Delete profile data from database
            try
            {
                await _userProfileService.DeleteProfileDataAsync(userprofileId: userprofileId);
            } catch
            {
                // Log error
                _logger.LogError(this.GetLogPrefix() + " profile deletion failed");
                return Ok(new ApiResponse(success: false, reason: "profile deletion failed"));
            }

            // Log deletion
            _logger.LogInformation(this.GetLogPrefix() + " profile deleted");

            // Keycloak: logout user
            await _keycloakAdminApiService.LogoutUser(this.GetBearerTokenFromHttpRequest());

            // Keycloak: remove user
            await _keycloakAdminApiService.RemoveUser(this.GetBearerTokenFromHttpRequest());

            return Ok(new ApiResponse(success: true));
        }
    }
}