using api.Services;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

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
        private readonly KeycloakAdminApiService _keycloakAdminApiService;
        private readonly TokenService _tokenService;
        private readonly ILogger<OrcidController> _logger;

        public AccountLinkController(KeycloakAdminApiService keycloakAdminApiService, TokenService tokenService, ILogger<OrcidController> logger)
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
            // Log request.
            _logger.LogInformation(this.GetLogPrefix() + " set ORCID ID attribute in Keycloak.");

            // Decode JWT.
            System.IdentityModel.Tokens.Jwt.JwtSecurityToken kcJwt = _tokenService.GetJwtFromString(this.GetBearerTokenFromHttpRequest());
            // Return immediately if JWT already contains ORCID ID.
            if (TokenService.JwtContainsOrcid(kcJwt))
            {
                return Ok(new ApiResponse(success: true));
            }
            // Set ORCID ID as a user attribute in Keycloak.
            bool setOrcidAttributeSuccess = await _keycloakAdminApiService.SetOrcidAttributedInKeycloak(kcJwt);
            return Ok(new ApiResponse(success: setOrcidAttributeSuccess));
        }
    }
}