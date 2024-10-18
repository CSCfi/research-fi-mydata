using api.Services;
using api.Models.Api;
using api.Models.Common;
using api.Models.Ttv;
using api.Models.ProfileEditor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.Logging;
using api.Models.Log;

namespace api.Controllers
{
    /*
     * ProfileSettingsController implements profile editor settings related APIs.
     */
    [Route("api/settings")]
    [ApiController]
    [Authorize(Policy = "RequireScopeApi1AndClaimOrcid")]
    public class ProfileSettingsController : TtvControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ILogger<UserProfileController> _logger;
        private readonly IMemoryCache _cache;

        public ProfileSettingsController(IUserProfileService userProfileService, ILogger<UserProfileController> logger, IMemoryCache memoryCache)
        {
            _userProfileService = userProfileService;
            _logger = logger;
            _cache = memoryCache;
        }

        /// <summary>
        /// Get profile settings
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseProfileSettingsGet), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSettings()
        {
            // Get ORCID id
            string orcidId = GetOrcidId();

            // Get user profile
            DimUserProfile dimUserProfile = await _userProfileService.GetUserprofile(orcidId);

            // Check that userprofile exists.
            if (dimUserProfile == null)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.PROFILE_NOT_FOUND));
            }

            // Cache key
            string cacheKey = _userProfileService.GetCMemoryCacheKey_ProfileSettings(orcidId);

            // Send cached response, if exists.
            if (_cache.TryGetValue(cacheKey, out ProfileSettings cachedProfileSettings))
            {
                return Ok(new ApiResponseProfileSettingsGet(success: true, reason: "", data: cachedProfileSettings, fromCache: true));
            }

            // Get settings data
            ProfileSettings profileSettings = _userProfileService.GetProfileSettings(dimUserProfile);

            // Store data into cache
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromSeconds(Constants.Cache.MEMORY_CACHE_EXPIRATION_SECONDS));
            _cache.Set(cacheKey, profileSettings, cacheEntryOptions);

            return Ok(
                new ApiResponseProfileSettingsGet(
                    success: true,
                    reason: "",
                    data: profileSettings,
                    fromCache: false));
        }

        /// <summary>
        /// Set profile settings.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> SetSettings([FromBody] ProfileSettings profileSettings)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.INVALID_REQUEST));
            }

            // Get ORCID id
            string orcidId = GetOrcidId();

            // Get user profile
            DimUserProfile dimUserProfile = await _userProfileService.GetUserprofileTracking(orcidId);

            // Check that userprofile exists.
            if (dimUserProfile == null)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.PROFILE_NOT_FOUND));
            }

            // Remove cached data
            string cacheKey = _userProfileService.GetCMemoryCacheKey_ProfileSettings(orcidId);
            _cache.Remove(cacheKey);

            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();

            string settingsToLogMessage = $"hidden={profileSettings.Hidden}, publishNewOrcidData={profileSettings.PublishNewOrcidData}";

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.SETTINGS_SET,
                    state: LogContent.ActionState.START,
                    message: settingsToLogMessage));

            // Save settings
            await _userProfileService.SaveProfileSettings(
                orcidId: orcidId,
                dimUserProfile: dimUserProfile,
                profileSettings: profileSettings,
                logUserIdentification: logUserIdentification);

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.SETTINGS_SET,
                    state: LogContent.ActionState.COMPLETE,
                    message: settingsToLogMessage));

            return Ok(new ApiResponse());
        }
    }
}