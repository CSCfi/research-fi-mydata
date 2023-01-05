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
     * AccountDeleteController handles Keycloak user deletion.
     */
    [Route("api/accountdelete")]
    [ApiController]
    [Authorize]
    public class AccountDeleteController : TtvControllerBase
    {
        private readonly IKeycloakAdminApiService _keycloakAdminApiService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<OrcidController> _logger;

        public AccountDeleteController(IKeycloakAdminApiService keycloakAdminApiService, ITokenService tokenService, ILogger<OrcidController> logger)
        {
            _keycloakAdminApiService = keycloakAdminApiService;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// Trigger backend to remove user from Keycloak.
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete()
        {
            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                this.GetLogUserIdentification(),
                new LogApiInfo(
                    action: LogContent.Action.KEYCLOAK_ACCOUNT_DELETE,
                    state: LogContent.ActionState.START));

            // Keycloak: logout user
            await _keycloakAdminApiService.LogoutUser(this.GetBearerTokenFromHttpRequest(), this.GetLogUserIdentification());

            // Keycloak: remove user
            await _keycloakAdminApiService.RemoveUser(this.GetBearerTokenFromHttpRequest(), this.GetLogUserIdentification());

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                this.GetLogUserIdentification(),
                new LogApiInfo(
                    action: LogContent.Action.KEYCLOAK_ACCOUNT_DELETE,
                    state: LogContent.ActionState.COMPLETE));
            return Ok(new ApiResponse(success: true));
        }
    }
}