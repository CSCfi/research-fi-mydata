using api.Services;
using api.Models.Api;
using api.Models.Common;
using api.Models.Log;
using api.Models.Ttv;
using api.Models.ProfileEditor.Items;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace api.Controllers
{
    /*
     * DebugController implements API for debugging.
     */
    [ApiController]
    public class DebugController: TtvAdminControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly IUserProfileService _userProfileService;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ILogger<UserProfileController> _logger;
        private readonly IMemoryCache _cache;
        private readonly IBackgroundTaskQueue _taskQueue;
        public IConfiguration Configuration { get; }
        public string logPrefix;

        public DebugController(
            IConfiguration configuration,
            TtvContext ttvContext,
            IUserProfileService userProfileService,
            ILogger<UserProfileController> logger,
            IMemoryCache memoryCache,
            IBackgroundTaskQueue taskQueue,
            IElasticsearchService elasticsearchService)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _logger = logger;
            _cache = memoryCache;
            _taskQueue = taskQueue;
            _elasticsearchService = elasticsearchService;
            Configuration = configuration;

            logPrefix = "DebugController: ";
        }


        /// <summary>
        /// Debug: Get number of user profiles.
        /// </summary>
        [HttpGet]
        [Route("/[controller]/profilecount")]
        public async Task<IActionResult> GetNumberOfProfiles()
        {
            _logger.LogInformation($"{logPrefix}get number of profiles");

            // Check admin token authorization
            if (!IsAdminTokenAuthorized(Configuration))
            {
                return Unauthorized();
            }

            int count = await _ttvContext.DimUserProfiles.Where(up => up.Id != -1).AsNoTracking().CountAsync();
            return Ok(count);
        }


        /// <summary>
        /// Debug: Get list of ORCID ID which have a user profile.
        /// </summary>
        [HttpGet]
        [Route("/[controller]/orcids")]
        public async Task<IActionResult> GetListOfORCIDs()
        {
            _logger.LogInformation($"{logPrefix}get list of ORCID IDs which have a user profile");

            // Check admin token authorization
            if (!IsAdminTokenAuthorized(Configuration))
            {
                return Unauthorized();
            }

            List<string> orcidIds = new();
            foreach (DimUserProfile up in await _ttvContext.DimUserProfiles.Where(up => up.Id != -1).AsNoTracking().ToListAsync())
            {
                orcidIds.Add(up.OrcidId);
            }

            return Ok(orcidIds);
        }
            

        /// <summary>
        /// Debug: Get any user profile data.
        /// </summary>
        [HttpGet]
        [Route("/[controller]/profiledata/{orcidId}")]
        [ProducesResponseType(typeof(ApiResponseProfileDataGet), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(string orcidId)
        {
            _logger.LogInformation($"{logPrefix}get profile data for ORCID ID: " + orcidId);

            // Check admin token authorization
            if (!IsAdminTokenAuthorized(Configuration))
            {
                return Unauthorized();
            }

            // Validate request data
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse(success: false, reason: "invalid request data"));
            }

            // Check that user profile exists.
            int userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // User identification object for logging
            LogUserIdentification logUserIdentification = new LogUserIdentification(orcid: orcidId);

            // Get profile data
            ProfileEditorDataResponse profileDataResponse = await _userProfileService.GetProfileDataAsync(userprofileId: userprofileId, logUserIdentification: logUserIdentification);

            return Ok(new ApiResponseProfileDataGet(success: true, reason: "", data: profileDataResponse, fromCache: false));
        }


        /// <summary>
        /// Debug: Create user profile for given ORCID ID.
        /// Preconditions: ORCID ID must must already exist in dim_pid and it must be linked to a row in dim_known_person.
        /// </summary>
        [HttpPost]
        [Route("/[controller]/userprofile/{orcidId}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateProfile(string orcidId)
        {
            _logger.LogInformation($"{logPrefix}create user profile, ORCID ID: " + orcidId);

            // Check admin token authorization
            if (!IsAdminTokenAuthorized(Configuration))
            {
                return Unauthorized();
            }

            // Validate request data
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse(success: false, reason: "invalid request data"));
            }

            // Check if user profile already exists.
            if (await _userProfileService.UserprofileExistsForOrcidId(orcidId: orcidId))
            {
                return Ok(new ApiResponse(success: true, reason: "profile already exists"));
            }

            // Check that ORCID ID exists in DimPid and is linked to DimKnownPerson.
            // New rows must not be added to DimPid or DimKnownPerson.
            DimPid dimPid = await _ttvContext.DimPids
                .FirstOrDefaultAsync(dp => dp.PidContent == orcidId && dp.PidType == Constants.PidTypes.ORCID && dp.DimKnownPersonId != -1);

            if (dimPid == null)
            {
                return Ok(new ApiResponse(success: false, reason: "DEBUG Either ORCID ID does not exist in dim_pid, or it is not linked to any dim_known_person."));
            }

            // User identification object for logging
            LogUserIdentification logUserIdentification = new LogUserIdentification(orcid: orcidId);

            // Create profile
            try
            {
                await _userProfileService.CreateProfile(orcidId: orcidId, logUserIdentification: logUserIdentification);
            }
            catch
            {
                string msg = "profile creation failed";
                _logger.LogError($"{logPrefix}{msg}");
                return Ok(new ApiResponse(success: false, reason: msg));
            }

            _logger.LogInformation($"{logPrefix}profile created");
            return Ok(new ApiResponse(success: true));
        }


        /// <summary>
        /// Debug: Delete user profile for given ORCID ID.
        /// </summary>
        [HttpDelete]
        [Route("/[controller]/userprofile/{orcidId}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteProfile(string orcidId)
        {
            _logger.LogInformation($"{logPrefix}delete user profile, ORCID ID: " + orcidId);

            // Check admin token authorization
            if (!IsAdminTokenAuthorized(Configuration))
            {
                return Unauthorized();
            }

            // Validate request data
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse(success: false, reason: "invalid request data"));
            }

            // Return immediately, if profile does not exist.
            if (!await _userProfileService.UserprofileExistsForOrcidId(orcidId: orcidId))
            {
                string msg = "profile does not exist for ORCID ID: " + orcidId;
                _logger.LogInformation($"{logPrefix}{msg}");
                return Ok(new ApiResponse(success: false, reason: msg));
            }

            // Get userprofile id.
            int userprofileId = await _userProfileService.GetUserprofileId(orcidId);

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);

            // User identification object for logging
            LogUserIdentification logUserIdentification = new LogUserIdentification(orcid: orcidId);

            // Remove entry from Elasticsearch index.
            await _userProfileService.DeleteProfileFromElasticsearch(
                orcidId: orcidId,
                logUserIdentification: logUserIdentification);

            // Delete profile data from database
            try
            {
                await _userProfileService.DeleteProfileDataAsync(userprofileId: userprofileId, logUserIdentification: logUserIdentification);
            }
            catch
            {
                // Log error
                string msg = "profile deletion failed";
                _logger.LogError($"{logPrefix}{msg}");
                return Ok(new ApiResponse(success: false, reason: msg));
            }

            // Log deletion
            _logger.LogInformation($"{logPrefix}profile deleted");

            return Ok(new ApiResponse(success: true));
        }
    }
}