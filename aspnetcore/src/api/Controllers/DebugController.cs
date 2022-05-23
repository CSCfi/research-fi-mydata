using api.Services;
using api.Models;
using api.Models.Ttv;
using api.Models.ProfileEditor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public IConfiguration Configuration { get; }

        public DebugController(IConfiguration configuration, TtvContext ttvContext, UserProfileService userProfileService)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            Configuration = configuration;
        }


        /// <summary>
        /// Debug: Get number of user profiles. Requires correct "debugtoken" header value.
        /// </summary>
        [HttpGet]
        [Route("/[controller]/profilecount")]
        public async Task<IActionResult> GetNumberOfProfiles()
        {
            // Check that "DEBUGTOKEN" is defined and has a value in configuration and that the request header "debugtoken" matches.
            if (Configuration["DEBUGTOKEN"] == null || Configuration["DEBUGTOKEN"] == "" || Request.Headers["debugtoken"] != Configuration["DEBUGTOKEN"])
            {
                return Unauthorized();
            }

            List<DimPid> dimPids = await _ttvContext.DimPids.Where(dp => dp.PidType == Constants.PidTypes.ORCID && dp.SourceId == Constants.SourceIdentifiers.PROFILE_API).AsNoTracking().ToListAsync();
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
            if (Configuration["DEBUGTOKEN"] == null || Configuration["DEBUGTOKEN"] == "" || Request.Headers["debugtoken"] != Configuration["DEBUGTOKEN"])
            {
                return Unauthorized();
            }

            List<string> orcidIds = new();
            List<DimPid> dimPids = await _ttvContext.DimPids.Where(dp => dp.PidType == Constants.PidTypes.ORCID && dp.SourceId == Constants.SourceIdentifiers.PROFILE_API).AsNoTracking().ToListAsync();

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
            if (Configuration["DEBUGTOKEN"] == null || Configuration["DEBUGTOKEN"] == "" || Request.Headers["debugtoken"] != Configuration["DEBUGTOKEN"])
            {
                return Unauthorized();
            }

            // Check that user profile exists.
            int userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Get profile data
            ProfileEditorDataResponse profileDataResponse = await _userProfileService.GetProfileDataAsync(userprofileId);

            return Ok(new ApiResponseProfileDataGet(success: true, reason: "", data: profileDataResponse, fromCache: false));
        }
    }
}