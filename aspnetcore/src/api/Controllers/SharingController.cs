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
        /// Get list of sharing purposes.
        /// </summary>
        [HttpGet]
        [Route("purposes")]
        [ProducesResponseType(typeof(ApiResponseProfileSharingPurposesGet), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPurposes()
        {
            string cacheKey = "share_purposes";

            // Send cached response, if exists.
            if (_cache.TryGetValue(cacheKey, out ProfileEditorSharingPurposesResponse cachedResponse))
            {
                return Ok(new ApiResponseProfileSharingPurposesGet(success: true, reason: "", data: cachedResponse, fromCache: true));
            }

            // Get purposes
            ProfileEditorSharingPurposesResponse profileSharingPurposesResponse = await _sharingService.GetProfileEditorSharingPurposesResponse();

            // Save response in cache. Cache life time can be long, singe the values are the same for all users.
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(Constants.Cache.MEMORY_CACHE_EXPIRATION_SECONDS_LONG));
            _cache.Set(cacheKey, profileSharingPurposesResponse, cacheEntryOptions);

            return Ok(new ApiResponseProfileSharingPurposesGet(success: true, reason: "", data: profileSharingPurposesResponse, fromCache: false));
        }

        /// <summary>
        /// Get list of sharing permissions.
        /// </summary>
        [HttpGet]
        [Route("permissions")]
        [ProducesResponseType(typeof(ApiResponseProfileSharingPermissionsGet), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPermissions()
        {
            string cacheKey = "share_permissions";

            // Send cached response, if exists.
            if (_cache.TryGetValue(cacheKey, out ProfileEditorSharingPermissionsResponse cachedResponse))
            {
                return Ok(new ApiResponseProfileSharingPermissionsGet(success: true, reason: "", data: cachedResponse, fromCache: true));
            }

            // Get permissions
            ProfileEditorSharingPermissionsResponse profileSharingPermissionsResponse = await _sharingService.GetProfileEditorSharingPermissionsResponse();

            // Save response in cache. Cache life time can be long, singe the values are the same for all users.
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(Constants.Cache.MEMORY_CACHE_EXPIRATION_SECONDS_LONG));
            _cache.Set(cacheKey, profileSharingPermissionsResponse, cacheEntryOptions);

            return Ok(new ApiResponseProfileSharingPermissionsGet(success: true, reason: "", data: profileSharingPermissionsResponse, fromCache: false));
        }


        /// <summary>
        /// Get list of given permissions.
        /// </summary>
        [HttpGet]
        [Route("givenpermissions")]
        [ProducesResponseType(typeof(ApiResponseProfileSharingGivenPermissionsGet), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShares()
        {
            // Get ORCID id
            string orcidId = GetOrcidId();

            // Cache key
            string cacheKey = orcidId + "_given_permissions";

            // Check that userprofile exists.
            if (!await _userProfileService.UserprofileExistsForOrcidId(orcidId: orcidId))
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Send cached response, if exists.
            if (_cache.TryGetValue(cacheKey, out ProfileEditorSharingGivenPermissionsResponse cachedResponse))
            {
                return Ok(new ApiResponseProfileSharingGivenPermissionsGet(success: true, reason: "", data: cachedResponse, fromCache: true));
            }

            // Get userprofile id
            int userprofileId = await _userProfileService.GetUserprofileId(orcidId);

            // Get profile data
            ProfileEditorSharingGivenPermissionsResponse profileSharingResponse = await _sharingService.GetProfileEditorSharingResponse(userprofileId);

            // Save response in cache
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(Constants.Cache.MEMORY_CACHE_EXPIRATION_SECONDS));
            _cache.Set(cacheKey, profileSharingResponse, cacheEntryOptions);

            return Ok(new ApiResponseProfileSharingGivenPermissionsGet(success: true, reason: "", data: profileSharingResponse, fromCache: false));
        }

        /// <summary>
        /// Add permission(s).
        /// </summary>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> PostMany([FromBody] List<ProfileEditorSharingPermissionToAddOrDelete> permissionsToAdd)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse(success: false, reason: "invalid request data"));
            }

            // Return immediately if there is nothing to add
            if (permissionsToAdd.Count == 0)
            {
                return Ok(new ApiResponse(success: false, reason: "nothing to add"));
            }

            // Get ORCID id
            string orcidId = GetOrcidId();

            // Check that userprofile exists.
            if (!await _userProfileService.UserprofileExistsForOrcidId(orcidId: orcidId))
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Get userprofile id
            int userprofileId = await _userProfileService.GetUserprofileId(orcidId);

            await _sharingService.AddPermissions(userprofileId, permissionsToAdd);

            await _ttvContext.SaveChangesAsync();

            // Remove cached given permissions list.
            _cache.Remove(orcidId + "_given_permissions");

            return Ok(new ApiResponse(success: true));
        }

        /// <summary>
        /// Remove permission(s).
        /// </summary>
        [HttpPost]
        [Route("remove")]
        public async Task<IActionResult> RemoveMany([FromBody] List<ProfileEditorSharingPermissionToAddOrDelete> permissionsToDelete)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse(success: false, reason: "invalid request data"));
            }

            // Return immediately if there is nothing to delete
            if (permissionsToDelete.Count == 0)
            {
                return Ok(new ApiResponse(success: false, reason: "nothing to remove"));
            }

            // Get ORCID id
            string orcidId = GetOrcidId();

            // Check that userprofile exists.
            if (!await _userProfileService.UserprofileExistsForOrcidId(orcidId: orcidId))
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Get userprofile id
            int userprofileId = await _userProfileService.GetUserprofileId(orcidId);

            await _sharingService.DeletePermissions(userprofileId, permissionsToDelete);

            await _ttvContext.SaveChangesAsync();

            // Remove cached given permissions list.
            _cache.Remove(orcidId + "_given_permissions");

            return Ok(new ApiResponse(success: true));
        }
    }
}