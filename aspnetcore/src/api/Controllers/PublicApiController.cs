using api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ResearchFi.PersonPublicApi;

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
        public IActionResult GetProfileData([FromBody] ProfileDataRequest request)
        {
            if (!IsPublicApiTokenAuthorized())
            {
                return Unauthorized();
            }

            return Ok(_publicApiService.GetProfileDataForPublicApi(request?.PersonKeyIdentifier));
        }

        // Check that request contains required Public API token, in header "publicapitoken".
        [NonAction]
        private bool IsPublicApiTokenAuthorized()
        {
            return !string.IsNullOrWhiteSpace(_configuration["PUBLICAPITOKEN"]) && Request.Headers["publicapitoken"] == _configuration["PUBLICAPITOKEN"];
        }
    }
}