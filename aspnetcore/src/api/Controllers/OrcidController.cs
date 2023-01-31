using api.Services;
using api.Models.Api;
using api.Models.Orcid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Hosting;
using api.Models.Log;
using Microsoft.Extensions.Caching.Memory;

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
        private readonly IUserProfileService _userProfileService;
        private readonly IOrcidApiService _orcidApiService;
        private readonly IOrcidImportService _orcidImportService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<OrcidController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMemoryCache _cache;

        public OrcidController(IUserProfileService userProfileService,
            IOrcidApiService orcidApiService,
            IOrcidImportService orcidImportService,
            ILogger<OrcidController> logger,
            ITokenService tokenService,
            IWebHostEnvironment webHostEnvironment,
            IMemoryCache memoryCache)
        {
            _userProfileService = userProfileService;
            _orcidApiService = orcidApiService;
            _orcidImportService = orcidImportService;
            _tokenService = tokenService;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _cache = memoryCache;
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
            //_logger.LogInformation(this.GetLogPrefix() + " get ORCID data request");

            // Check that userprofile exists.
            if (!await _userProfileService.UserprofileExistsForOrcidId(orcidId: GetOrcidId()))
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Get userprofile id
            int userprofileId = await _userProfileService.GetUserprofileId(orcidId);

            // Get ORCID record from ORCID member or public API.
            // If environment is not "Production" and user access token has claim "use_orcid_public_api",
            // then the record is requested from public API.
            // In all other cases the ORCID member API will be used.
            string orcidRecordJson = "";

            if (_webHostEnvironment.EnvironmentName!="Production" && this.GetOrcidPublicApiFlag() != null)
            {
                // ORCID public API should be used
                try
                {
                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        this.GetLogUserIdentification(),
                        new LogApiInfo(
                            action: LogContent.Action.ORCID_RECORD_GET_PUBLIC_API,
                            state: LogContent.ActionState.START));

                    orcidRecordJson = await _orcidApiService.GetRecordFromPublicApi(orcidId);

                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        this.GetLogUserIdentification(),
                        new LogApiInfo(
                            action: LogContent.Action.ORCID_RECORD_GET_PUBLIC_API,
                            state: LogContent.ActionState.COMPLETE));
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        LogContent.MESSAGE_TEMPLATE,
                        this.GetLogUserIdentification(),
                        new LogApiInfo(
                            action: LogContent.Action.ORCID_RECORD_GET_PUBLIC_API,
                            state: LogContent.ActionState.FAILED,
                            error: true,
                            message: ex.ToString()));

                    return Ok(new ApiResponse(success: false));
                }
            }
            else
            {
                // ORCID member API should be used

                // User's ORCID access token handling
                OrcidTokens orcidTokens;
                try
                {
                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        this.GetLogUserIdentification(),
                        new LogApiInfo(
                            action: LogContent.Action.KEYCLOAK_GET_ORCID_TOKENS,
                            state: LogContent.ActionState.START));

                    // Get ORCID access token from Keycloak
                    string orcidTokensJson = await _tokenService.GetOrcidTokensJsonFromKeycloak(this.GetBearerTokenFromHttpRequest());
                    // Parse json from Keycloak into EF model
                    orcidTokens = _tokenService.ParseOrcidTokensJson(orcidTokensJson);
                    // Update ORCID tokens in TTV database. 
                    await _userProfileService.UpdateOrcidTokensInDimUserProfile(userprofileId, orcidTokens);

                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        this.GetLogUserIdentification(),
                        new LogApiInfo(
                            action: LogContent.Action.KEYCLOAK_GET_ORCID_TOKENS,
                            state: LogContent.ActionState.COMPLETE));
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        LogContent.MESSAGE_TEMPLATE,
                        this.GetLogUserIdentification(),
                        new LogApiInfo(
                            action: LogContent.Action.KEYCLOAK_GET_ORCID_TOKENS,
                            state: LogContent.ActionState.FAILED,
                            error: true,
                            message: ex.ToString()));
                    return Ok(new ApiResponse(success: false));
                }

                // Get record json from ORCID member API
                try
                {
                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        this.GetLogUserIdentification(),
                        new LogApiInfo(
                            action: LogContent.Action.ORCID_RECORD_GET_MEMBER_API,
                            state: LogContent.ActionState.START));

                    orcidRecordJson = await _orcidApiService.GetRecordFromMemberApi(orcidId, orcidTokens.AccessToken);

                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        this.GetLogUserIdentification(),
                        new LogApiInfo(
                            action: LogContent.Action.ORCID_RECORD_GET_MEMBER_API,
                            state: LogContent.ActionState.COMPLETE));
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        LogContent.MESSAGE_TEMPLATE,
                        this.GetLogUserIdentification(),
                        new LogApiInfo(
                            action: LogContent.Action.ORCID_RECORD_GET_MEMBER_API,
                            state: LogContent.ActionState.FAILED,
                            error: true,
                            message: ex.ToString()));
                    return Ok(new ApiResponse(success: false));
                }
            }

            // Import record json into userprofile
            try
            {
                _logger.LogInformation(
                                    LogContent.MESSAGE_TEMPLATE,
                                    this.GetLogUserIdentification(),
                                    new LogApiInfo(
                                        action: LogContent.Action.ORCID_RECORD_IMPORT,
                                        state: LogContent.ActionState.START));

                await _orcidImportService.ImportOrcidRecordJsonIntoUserProfile(userprofileId, orcidRecordJson);

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    this.GetLogUserIdentification(),
                    new LogApiInfo(
                        action: LogContent.Action.ORCID_RECORD_IMPORT,
                        state: LogContent.ActionState.COMPLETE));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    this.GetLogUserIdentification(),
                    new LogApiInfo(
                        action: LogContent.Action.ORCID_RECORD_IMPORT,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: ex.ToString()));

                return Ok(new ApiResponse(success: false));
            }

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);

            return Ok(new ApiResponse(success: true));
        }
    }
}