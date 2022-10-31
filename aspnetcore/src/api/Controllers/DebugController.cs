using api.Services;
using api.Models.Api;
using api.Models.Common;
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
    public class DebugController: TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly IUserProfileService _userProfileService;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ILogger<UserProfileController> _logger;
        private readonly IMemoryCache _cache;
        private readonly IBackgroundTaskQueue _taskQueue;
        public IConfiguration Configuration { get; }

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
        }


        /// <summary>
        /// Debug: Get number of user profiles. Requires correct "debugtoken" header value.
        /// </summary>
        [HttpGet]
        [Route("/[controller]/profilecount")]
        public async Task<IActionResult> GetNumberOfProfiles()
        {
            _logger.LogInformation(this.GetLogPrefix() + " DEBUG get number of profiles");

            // Check that "DEBUGTOKEN" is defined and has a value in configuration and that the request header "debugtoken" matches.
            if (Configuration["DEBUGTOKEN"] == null || Configuration["DEBUGTOKEN"] == "" || Request.Headers["debugtoken"] != Configuration["DEBUGTOKEN"])
            {
                return Unauthorized();
            }

            int count = await _ttvContext.DimUserProfiles.Where(up => up.Id != -1).AsNoTracking().CountAsync();
            return Ok(count);
        }


        /// <summary>
        /// Debug: Get list of ORCID ID which have a user profile. Requires correct "debugtoken" header value.
        /// </summary>
        [HttpGet]
        [Route("/[controller]/orcids")]
        public async Task<IActionResult> GetListOfORCIDs()
        {
            _logger.LogInformation(this.GetLogPrefix() + " DEBUG get list of ORCID IDs which have a user profile");

            // Check that "DEBUGTOKEN" is defined and has a value in configuration and that the request header "debugtoken" matches.
            if (Configuration["DEBUGTOKEN"] == null || Configuration["DEBUGTOKEN"] == "" || Request.Headers["debugtoken"] != Configuration["DEBUGTOKEN"])
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
        /// Debug: Get any user profile data. Requires correct "debugtoken" header value.
        /// </summary>
        [HttpGet]
        [Route("/[controller]/profiledata/{orcidId}")]
        [ProducesResponseType(typeof(ApiResponseProfileDataGet), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(string orcidId)
        {
            _logger.LogInformation(this.GetLogPrefix() + " DEBUG get profile data for ORCID ID: " + orcidId);

            // Check that "DEBUGTOKEN" is defined and has a value in configuration and that the request header "debugtoken" matches.
            if (Configuration["DEBUGTOKEN"] == null || Configuration["DEBUGTOKEN"] == "" || Request.Headers["debugtoken"] != Configuration["DEBUGTOKEN"])
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

            // Get profile data
            ProfileEditorDataResponse profileDataResponse = await _userProfileService.GetProfileDataAsync2(userprofileId);

            return Ok(new ApiResponseProfileDataGet(success: true, reason: "", data: profileDataResponse, fromCache: false));
        }


        /// <summary>
        /// Debug: Create user profile for given ORCID ID.
        /// Preconditions: ORCID ID must must already exist in dim_pid and it must be linked to a row in dim_known_person.
        /// Requires correct "debugtoken" header value.
        /// </summary>
        [HttpPost]
        [Route("/[controller]/userprofile/{orcidId}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateProfile(string orcidId)
        {
            _logger.LogInformation(this.GetLogPrefix() + " DEBUG create user profile, ORCID ID: " + orcidId);

            // Check that "DEBUGTOKEN" is defined and has a value in configuration and that the request header "debugtoken" matches.
            if (Configuration["DEBUGTOKEN"] == null || Configuration["DEBUGTOKEN"] == "" || Request.Headers["debugtoken"] != Configuration["DEBUGTOKEN"])
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

            // Create profile
            try
            {
                await _userProfileService.CreateProfile(orcidId: orcidId);
            }
            catch
            {
                string msg = " DEBUG profile creation failed";
                _logger.LogError(this.GetLogPrefix() + msg);
                return Ok(new ApiResponse(success: false, reason: msg));
            }

            _logger.LogInformation(this.GetLogPrefix() + " DEBUG profile created");
            return Ok(new ApiResponse(success: true));
        }


        /// <summary>
        /// Debug: Delete user profile for given ORCID ID. Requires correct "debugtoken" header value.
        /// </summary>
        [HttpDelete]
        [Route("/[controller]/userprofile/{orcidId}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteProfile(string orcidId)
        {
            _logger.LogInformation(this.GetLogPrefix() + " DEBUG delete user profile, ORCID ID: " + orcidId);

            // Check that "DEBUGTOKEN" is defined and has a value in configuration and that the request header "debugtoken" matches.
            if (Configuration["DEBUGTOKEN"] == null || Configuration["DEBUGTOKEN"] == "" || Request.Headers["debugtoken"] != Configuration["DEBUGTOKEN"])
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
                string msg = " DEBUG profile does not exist for ORCID ID: " + orcidId;
                _logger.LogInformation(this.GetLogPrefix() + msg);
                return Ok(new ApiResponse(success: false, reason: msg));
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
            }
            catch
            {
                // Log error
                string msg = " DEBUG profile deletion failed";
                _logger.LogError(this.GetLogPrefix() + msg);
                return Ok(new ApiResponse(success: false, reason: msg));
            }

            // Log deletion
            _logger.LogInformation(this.GetLogPrefix() + " DEBUG profile deleted");

            return Ok(new ApiResponse(success: true));
        }
    }
}