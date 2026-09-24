using api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using api.PublicApiContracts;

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
        /// Mockup endpoint for Public API integration. Returns a greeting for the given username.
        /// Exact request/response shape is out of scope for this phase; see plans/public-api-nuget-contracts.md.
        /// </summary>
        [HttpPost]
        public IActionResult GetDataForPublicApi([FromBody] PublicApiHelloRequest request)
        {
            if (!IsPublicApiTokenAuthorized())
            {
                return Unauthorized();
            }

            return Ok(_publicApiService.GetHelloMessage(request?.Username));
        }

        // Check that request contains required Public API token, in header "publicapitoken".
        [NonAction]
        private bool IsPublicApiTokenAuthorized()
        {
            return !string.IsNullOrWhiteSpace(_configuration["PUBLICAPITOKEN"]) && Request.Headers["publicapitoken"] == _configuration["PUBLICAPITOKEN"];
        }
    }
}