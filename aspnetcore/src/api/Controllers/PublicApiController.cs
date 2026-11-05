using api.Models.Log;
using api.Services;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ResearchFi.ProfilePublicApi;

namespace api.Controllers
{
    /*
     * PublicApiController implements endpoint for Public API.
     */
    [Route("api/publicapi")]
    [ApiController]
    public class PublicApiController : TtvControllerBase
    {
        private readonly ILogger<PublicApiController> _logger;
        private readonly IPublicApiService _publicApiService;
        private readonly IConfiguration _configuration;

        public PublicApiController(ILogger<PublicApiController> logger, IPublicApiService publicApiService, IConfiguration configuration)
        {
            _logger = logger;
            _publicApiService = publicApiService;
            _configuration = configuration;
        }

        /// <summary>
        /// Profile data endpoint for Public API.
        /// </summary>
        [HttpPost]
        [Route("profile")]
        public async Task<IActionResult> GetProfileData([FromBody] ProfileDataRequest request)
        {
            string clientId = GetPublicApiClientId();

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                new LogUserIdentification(),
                new LogApiInfo(
                    action: LogContent.Action.PUBLIC_API_PROFILE_REQUEST,
                    state: LogContent.ActionState.START,
                    message: $"Public API clientId={clientId}"));

            if (!IsPublicApiTokenAuthorized())
            {
                string errorMessage = "Invalid token from Public API";
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    new LogUserIdentification(),
                    new LogApiInfo(
                        action: LogContent.Action.PUBLIC_API_PROFILE_REQUEST,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: $"{errorMessage}. Public API clientId={clientId}"));
                return Unauthorized();
            }

            // Schema query_for_consent_status: personIdentifiers has 1-2 items and controllerROR is required.
            if (request == null)
            {
                return BadRequest("A request body is required.");
            }

            if (request.PersonIdentifiers == null || request.PersonIdentifiers.Count == 0)
            {
                return BadRequest("At least one person identifier is required.");
            }

            if (request.PersonIdentifiers.Count > 2)
            {
                return BadRequest("No more than two person identifiers may be provided.");
            }

            if (string.IsNullOrWhiteSpace(request.ControllerROR))
            {
                return BadRequest("ControllerROR is required.");
            }

            if (request.PersonIdentifiers.Any(identifier => identifier == null))
            {
                return BadRequest("Each person identifier must be provided.");
            }

            if (request.PersonIdentifiers.Any(identifier => string.IsNullOrWhiteSpace(identifier.PidContent)))
            {
                return BadRequest("PidContent is required for each person identifier.");
            }

            if (request.PersonIdentifiers.Any(identifier =>
                    !string.Equals(identifier.PidType, "ORCID", System.StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(identifier.PidType, "HETU", System.StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("PidType must be either ORCID or HETU.");
            }

            return Ok(await _publicApiService.GetProfileDataForPublicApi(request));
        }

        // Check that request contains required Public API token, in header "publicapitoken".
        [NonAction]
        private bool IsPublicApiTokenAuthorized()
        {
            return !string.IsNullOrWhiteSpace(_configuration["PUBLICAPITOKEN"]) && Request.Headers["publicapitoken"] == _configuration["PUBLICAPITOKEN"];
        }

        // Get calling Public API client's client credentials clientid, sent in header "clientid".
        [NonAction]
        private string GetPublicApiClientId()
        {
            return Request.Headers["public-api-clientid"].ToString();
        }
    }
}