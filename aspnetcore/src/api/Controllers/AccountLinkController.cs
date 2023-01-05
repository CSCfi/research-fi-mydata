using api.Services;
using api.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using api.Models.Log;

namespace api.Controllers
{
    /*
     * AccountLinkController handles Suomi.fi and ORCID identity linking related functionality.
     */
    [Route("api/accountlink")]
    [ApiController]
    [Authorize]
    public class AccountLinkController : TtvControllerBase
    {
        private readonly IKeycloakAdminApiService _keycloakAdminApiService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountLinkController> _logger;

        public AccountLinkController(IKeycloakAdminApiService keycloakAdminApiService, ITokenService tokenService, ILogger<AccountLinkController> logger)
        {
            _keycloakAdminApiService = keycloakAdminApiService;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// Trigger backend to set ORCID ID as a user attribute in Keycloak.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                this.GetLogUserIdentification(),
                new LogApiInfo(
                    action: LogContent.Action.KEYCLOAK_LINK_ORCID,
                    state: LogContent.ActionState.START));

            // Decode JWT.
            System.IdentityModel.Tokens.Jwt.JwtSecurityToken kcJwt = _tokenService.GetJwtFromString(this.GetBearerTokenFromHttpRequest());
            // Return immediately if JWT already contains ORCID ID.
            if (TokenService.JwtContainsOrcid(kcJwt))
            {
                return Ok(new ApiResponse(success: true));
            }

            // Set ORCID ID as a user attribute in Keycloak.
            bool setOrcidAttributeSuccess = await _keycloakAdminApiService.SetOrcidAttributedInKeycloak(kcJwt, this.GetLogUserIdentification());

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                this.GetLogUserIdentification(),
                new LogApiInfo(
                    action: LogContent.Action.KEYCLOAK_LINK_ORCID,
                    state: LogContent.ActionState.COMPLETE));
            return Ok(new ApiResponse(success: setOrcidAttributeSuccess));
        }
    }
}