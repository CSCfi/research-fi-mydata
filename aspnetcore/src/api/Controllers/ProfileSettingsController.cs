using api.Services;
using api.Models.Api;
using api.Models.Common;
using api.Models.Ttv;
using api.Models.ProfileEditor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.Logging;
using api.Models.Log;
using api.Models.ProfileEditor.Items;
using static api.Models.Common.Constants;

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

            // Cache key
            string cacheKey = _userProfileService.GetCMemoryCacheKey_ProfileSettings(orcidId);

            // Send cached response, if exists.
            if (_cache.TryGetValue(cacheKey, out ProfileSettings cachedProfileSettings))
            {
                return Ok(new ApiResponseProfileSettingsGet(success: true, reason: "", data: cachedProfileSettings, fromCache: true));
            }

            // Cached response was not found, get data
            DimUserProfile dimUserProfile;
            ProfileSettings profileSettings = new();
            try
            {
                dimUserProfile = await _userProfileService.GetUserprofile(orcidId);
                profileSettings.Hidden = dimUserProfile.Hidden;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    this.GetLogUserIdentification(),
                    new LogApiInfo(
                        action: LogContent.Action.SETTINGS_GET,
                        error: true,
                        state: LogContent.ActionState.FAILED,
                        message: $"{ex.ToString()}"));
                return Ok(new ApiResponse(success: false, reason: "profile not found", fromCache: false));
            }

            // Store data into cache
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromSeconds(Constants.Cache.MEMORY_CACHE_EXPIRATION_SECONDS));
            _cache.Set(cacheKey, profileSettings, cacheEntryOptions);

            return Ok(new ApiResponseProfileSettingsGet(success: true, reason: "", data: profileSettings, fromCache: false));
        }

        /// <summary>
        /// Set profile settings.
        /// Setting "hidden" to true removes profile from Elasticsearch and disables Elasticsearch sync for the profile.
        /// Setting "hidden" to false adds profile to Elasticsearch and enables Elasticsearch sync for the profile.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> SetSettings([FromBody] ProfileSettings profileSettings)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse(success: false, reason: "invalid request data"));
            }

            // Get ORCID id
            string orcidId = GetOrcidId();

            // Remove cached data
            string cacheKey = _userProfileService.GetCMemoryCacheKey_ProfileSettings(orcidId);
            _cache.Remove(cacheKey);

            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();

            // Handle setting "hidden"
            if (profileSettings.Hidden != null)
            {
                if (profileSettings.Hidden == true)
                {
                    // Hide profile
                    await _userProfileService.HideProfile(orcidId: orcidId, logUserIdentification: logUserIdentification);
                }
                else if (profileSettings.Hidden == false)
                {
                    // Reveal profile
                    await _userProfileService.RevealProfile(orcidId: orcidId, logUserIdentification: logUserIdentification);
                }
            }

            return Ok(new ApiResponse());
        }



        /// <summary>
        /// DEPRECATED
        /// Set profile state to "hidden".
        /// Profile is removed from Elasticsearch index.
        /// </summary>
        [HttpGet]
        [Route("hideprofile")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> HideProfileFromPortal()
        {
            // Get ORCID id
            string orcidId = GetOrcidId();

            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();
            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_HIDE,
                    state: LogContent.ActionState.START));

            // Set profile state to "hidden"
            await _userProfileService.HideProfile(orcidId: orcidId, logUserIdentification: logUserIdentification);

            return Ok(new ApiResponse());
        }
    }
}