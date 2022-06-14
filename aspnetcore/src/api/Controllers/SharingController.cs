using api.Services;
using api.Models;
using api.Models.Ttv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using api.Models.ProfileEditor;
using System;

namespace api.Controllers
{
    /*
     * SharingController implements profile editor API commands for sharing settings.
     */
    [Route("api/sharing")]
    [ApiController]
    [Authorize(Policy = "RequireScopeApi1AndClaimOrcid")]
    public class SharingController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;
        private readonly SharingService _sharingService;
        private readonly UtilityService _utilityService;
        private readonly DataSourceHelperService _dataSourceHelperService;
        private readonly IMemoryCache _cache;

        public SharingController(TtvContext ttvContext,
            UserProfileService userProfileService,
            SharingService sharingService,
            UtilityService utilityService,
            DataSourceHelperService dataSourceHelperService,
            IMemoryCache memoryCache)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _sharingService = sharingService;
            _utilityService = utilityService;
            _dataSourceHelperService = dataSourceHelperService;
            _cache = memoryCache;
        }

        /// <summary>
        /// Get sharing settings.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseProfileSharingGet), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            // Get ORCID id
            string orcidId = GetOrcidId();
            string cacheKey = orcidId + "_sharing";

            // Check that userprofile exists.
            if (!await _userProfileService.UserprofileExistsForOrcidId(orcidId: orcidId))
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Send cached response, if exists.
            if (_cache.TryGetValue(cacheKey, out ProfileEditorDataResponse cachedResponse))
            {
                return Ok(new ApiResponseProfileDataGet(success: true, reason: "", data: cachedResponse, fromCache: true));
            }

            // Get userprofile id
            int userprofileId = await _userProfileService.GetUserprofileId(orcidId);

            // Get profile data
            ProfileEditorSharingResponse profileSharingResponse = await _sharingService.GetProfileEditorSharingResponse(userprofileId);

            // Save response in cache
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromSeconds(Constants.Cache.MEMORY_CACHE_EXPIRATION_SECONDS));
            _cache.Set(cacheKey, profileSharingResponse, cacheEntryOptions);

            return Ok(new ApiResponseProfileSharingGet(success: true, reason: "", data: profileSharingResponse, fromCache: false));
        }
    }
}