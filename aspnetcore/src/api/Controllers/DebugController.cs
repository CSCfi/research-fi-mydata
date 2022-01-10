using api.Services;
using api.Models;
using api.Models.Ttv;
using api.Models.Common;
using api.Models.ProfileEditor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace api.Controllers
{
    /*
     * DebugController implements API for debugging.
     */
    [ApiController]
    public class DebugController: ControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;
        private readonly TtvSqlService _ttvSqlService;
        private readonly LanguageService _languageService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<UserProfileController> _logger;
        public IConfiguration _configuration { get; }

        public DebugController(IConfiguration configuration, TtvContext ttvContext, UserProfileService userProfileService, TtvSqlService ttvSqlService, LanguageService languageService, IMemoryCache memoryCache, ILogger<UserProfileController> logger)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _cache = memoryCache;
            _ttvSqlService = ttvSqlService;
            _languageService = languageService;
            _logger = logger;
            _configuration = configuration;
        }


        /// <summary>
        /// Debug: Get number of user profiles. Requires correct "debugtoken" header value.
        /// </summary>
        [HttpGet]
        [Route("/[controller]/profilecount")]
        public async Task<IActionResult> GetNumberOfProfiles()
        {
            // Check that "DEBUGTOKEN" is defined and has a value in configuration and that the request header "debugtoken" matches.
            if (_configuration["DEBUGTOKEN"] == null || _configuration["DEBUGTOKEN"] == "" || Request.Headers["debugtoken"] != _configuration["DEBUGTOKEN"])
            {
                return Unauthorized();
            }

            var dimPids = await _ttvContext.DimPids.Where(dp => dp.PidType == Constants.PidTypes.ORCID && dp.SourceId == Constants.SourceIdentifiers.PROFILE_API).AsNoTracking().ToListAsync();
            return Ok(dimPids.Count);
        }


        /// <summary>
        /// Debug: Get list of ORCID IDs. Requires correct "debugtoken" header value.
        /// </summary>
        [HttpGet]
        [Route("/[controller]/orcids")]
        public async Task<IActionResult> GetListOfORCIDs()
        {
            // Check that "DEBUGTOKEN" is defined and has a value in configuration and that the request header "debugtoken" matches.
            if (_configuration["DEBUGTOKEN"] == null || _configuration["DEBUGTOKEN"] == "" || Request.Headers["debugtoken"] != _configuration["DEBUGTOKEN"])
            {
                return Unauthorized();
            }

            var orcidIds = new List<string>();
            var dimPids = await _ttvContext.DimPids.Where(dp => dp.PidType == Constants.PidTypes.ORCID && dp.SourceId == Constants.SourceIdentifiers.PROFILE_API).AsNoTracking().ToListAsync();

            foreach (DimPid dp in dimPids)
            {
                orcidIds.Add(dp.PidContent);
            }

            return Ok(orcidIds);
        }


        /// <summary>
        /// Debug: Get any profile data. Requires correct "debugtoken" header value.
        /// </summary>
        [HttpGet]
        [Route("/[controller]/profiledata/{orcidId}")]
        [ProducesResponseType(typeof(ApiResponseProfileDataGet), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(string orcidId)
        {
            // Check that "DEBUGTOKEN" is defined and has a value in configuration and that the request header "debugtoken" matches.
            if (_configuration["DEBUGTOKEN"] == null || _configuration["DEBUGTOKEN"] == "" || Request.Headers["debugtoken"] != _configuration["DEBUGTOKEN"])
            {
                return Unauthorized();
            }

            // Check that user profile exists.
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Get profile data
            var profileDataResponse = await _userProfileService.GetProfileDataAsync(userprofileId);

            return Ok(new ApiResponseProfileDataGet(success: true, reason: "", data: profileDataResponse, fromCache: false));
        }
    }
}