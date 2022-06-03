using api.Services;
using api.Models;
using api.Models.Orcid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace api.Controllers
{
    /*
     * OrcidController handles ORCID api related actions, such as getting ORCID record and saving ORCID data into database.
     */
    [Route("api/orcid")]
    [ApiController]
    [Authorize(Policy = "RequireScopeApi1AndClaimOrcid")]
    public class OrcidController : TtvControllerBase
    {
        private readonly UserProfileService _userProfileService;
        private readonly OrcidApiService _orcidApiService;
        private readonly OrcidImportService _orcidImportService;
        private readonly TokenService _tokenService;
        private readonly ILogger<OrcidController> _logger;

        public OrcidController(UserProfileService userProfileService, OrcidApiService orcidApiService, OrcidImportService orcidImportService, ILogger<OrcidController> logger, TokenService tokenService)
        {
            _userProfileService = userProfileService;
            _orcidApiService = orcidApiService;
            _orcidImportService = orcidImportService;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// Trigger backend to get ORCID record and save data into TTV database.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        // TODO: Currently adding and updating ORCID data works, but detecting deleted ORCID data and deleting them is TTV database is not implemented.
        public async Task<IActionResult> Get()
        {
            // Get ORCID id.
            string orcidId = this.GetOrcidId();

            // Log request.
            _logger.LogInformation(this.GetLogPrefix() + " get ORCID data request");

            // Check that userprofile exists.
            if (!await _userProfileService.UserprofileExistsForOrcidId(orcidId: GetOrcidId()))
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Get userprofile id
            int userprofileId = await _userProfileService.GetUserprofileId(orcidId);

            // User's ORCID access token handling
            OrcidTokens orcidTokens;
            try
            {
                // Get ORCID access token from Keycloak
                string orcidTokensJson = await _tokenService.GetOrcidTokensJsonFromKeycloak(this.GetBearerTokenFromHttpRequest());
                // Parse json from Keycloak into EF model
                orcidTokens = _tokenService.ParseOrcidTokensJson(orcidTokensJson);
                // Update ORCID tokens in TTV database. 
                await _userProfileService.UpdateOrcidTokensInDimUserProfile(userprofileId, orcidTokens);
            }
            catch
            {
                _logger.LogError(this.GetLogPrefix() + " failed getting user's ORCID tokens from Keycloak");
                return Ok(new ApiResponse(success: false));
            }


            // Get record json from ORCID
            _logger.LogInformation(this.GetLogPrefix() + " get ORCID record json from ORCID API");
            string orcidRecordJson;
            try
            {
                orcidRecordJson = await _orcidApiService.GetRecord(orcidId, orcidTokens.AccessToken);
            }
            catch
            {
                _logger.LogError(this.GetLogPrefix() + " failed getting ORCID record json from ORCID API");
                return Ok(new ApiResponse(success: false));
            }


            // Import record json into userprofile
            _logger.LogInformation(this.GetLogPrefix() + " import ORCID record json into userprofile");
            try
            {
                await _orcidImportService.ImportOrcidRecordJsonIntoUserProfile(userprofileId, orcidRecordJson);
            }
            catch
            {
                _logger.LogError(this.GetLogPrefix() + " ORCID record import to userprofile failed");
                return Ok(new ApiResponse(success: false));
            }

            _logger.LogInformation(this.GetLogPrefix() + " ORCID data imported successfully");
            return Ok(new ApiResponse(success: true));
        }
    }
}