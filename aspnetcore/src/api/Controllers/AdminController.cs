using api.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    /*
     * AdminController implements API for administration commands.
     */
    [ApiController]
    public class AdminController : TtvControllerBase
    {
        private readonly IOrcidApiService _orcidApiService;
        private readonly ILogger<UserProfileController> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public IConfiguration Configuration { get; }

        public AdminController(
            IOrcidApiService orcidApiService,
            IConfiguration configuration,
            IUserProfileService userProfileService,
            ILogger<UserProfileController> logger,
            IMemoryCache memoryCache,
            IBackgroundTaskQueue taskQueue,
            IServiceScopeFactory serviceScopeFactory)
        {
            _orcidApiService = orcidApiService;
            _logger = logger;
            _taskQueue = taskQueue;
            _serviceScopeFactory = serviceScopeFactory;
            Configuration = configuration;
        }


        /// <summary>
        /// Admin: Register webhook for a single user profile.
        /// Requires correct "debugtoken" header value.
        /// </summary>
        [HttpPost]
        [Route("/[controller]/orcidwebhook/register/single/{webhookOrcidId}")]
        public async Task<IActionResult> RegisterOrcidWebhookForSingleUserprofile(string webhookOrcidId)
        {
            // Check that "DEBUGTOKEN" is defined and has a value in configuration and that the request header "debugtoken" matches.
            if (string.IsNullOrWhiteSpace(Configuration["DEBUGTOKEN"]) || Request.Headers["debugtoken"] != Configuration["DEBUGTOKEN"])
            {
                return Unauthorized();
            }

            string logPrefix = $"AdminController: register ORCID webhook for {webhookOrcidId}: ";

            // Check that ORCID webhook feature is enabled
            if (!_orcidApiService.IsOrcidWebhookEnabled())
            {
                _logger.LogError($"{logPrefix}failed: ORCID webhook feature disabled in configuration");
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            // Register webhook in a background task
            await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
            {
                _logger.LogInformation($"{logPrefix}background task started {DateTime.UtcNow}");

                // Create service scope and get required services.
                // Do not use services from controller scope in a background task.
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IAdminService localAdminService = scope.ServiceProvider.GetRequiredService<IAdminService>();
                await localAdminService.RegisterOrcidWebhookForSingleUserprofile(webhookOrcidId);

                _logger.LogInformation($"{logPrefix}background task ended {DateTime.UtcNow}");
            });

            return Ok();
        }

        /// <summary>
        /// Admin: Unregister webhook for a single user profile.
        /// Requires correct "debugtoken" header value.
        /// </summary>
        ///         [HttpPost]
        [Route("/[controller]/orcidwebhook/unregister/single/{webhookOrcidId}")]
        public async Task<IActionResult> UnegisterOrcidWebhookForSingleUserprofile(string webhookOrcidId)
        {
            // Check that "DEBUGTOKEN" is defined and has a value in configuration and that the request header "debugtoken" matches.
            if (string.IsNullOrWhiteSpace(Configuration["DEBUGTOKEN"]) || Request.Headers["debugtoken"] != Configuration["DEBUGTOKEN"])
            {
                return Unauthorized();
            }

            string logPrefix = $"AdminController: unregister ORCID webhook for {webhookOrcidId}: ";

            // Check that ORCID webhook feature is enabled
            if (!_orcidApiService.IsOrcidWebhookEnabled())
            {
                _logger.LogError($"{logPrefix}failed: ORCID webhook feature disabled in configuration");
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            // Register webhook in a background task
            await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
            {
                _logger.LogInformation($"{logPrefix}background task started {DateTime.UtcNow}");

                // Create service scope and get required services.
                // Do not use services from controller scope in a background task.
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IAdminService localAdminService = scope.ServiceProvider.GetRequiredService<IAdminService>();
                await localAdminService.UnregisterOrcidWebhookForSingleUserprofile(webhookOrcidId);

                _logger.LogInformation($"{logPrefix}background task ended {DateTime.UtcNow}");
            });

            return Ok();
        }

        /// <summary>
        /// Admin: Register webhook for all user profiles.
        /// Requires correct "debugtoken" header value.
        /// </summary>
        [HttpPost]
        [Route("/[controller]/orcidwebhook/register/all")]
        public async Task<IActionResult> RegisterOrcidWebhookForAllUserprofiles()
        {
            // Check that "DEBUGTOKEN" is defined and has a value in configuration and that the request header "debugtoken" matches.
            if (string.IsNullOrWhiteSpace(Configuration["DEBUGTOKEN"]) || Request.Headers["debugtoken"] != Configuration["DEBUGTOKEN"])
            {
                return Unauthorized();
            }

            string logPrefix = "AdminController: register ORCID webhook for all user profiles: ";

            // Check that ORCID webhook feature is enabled
            if (!_orcidApiService.IsOrcidWebhookEnabled())
            {
                _logger.LogError($"{logPrefix}failed: ORCID webhook feature disabled in configuration");
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            // Register webhooks in a background task
            await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
            {
                _logger.LogInformation($"{logPrefix}background task started {DateTime.UtcNow}");

                // Create service scope and get required services.
                // Do not use services from controller scope in a background task.
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IAdminService localAdminService = scope.ServiceProvider.GetRequiredService<IAdminService>();
                await localAdminService.RegisterOrcidWebhookForAllUserprofiles();

                _logger.LogInformation($"{logPrefix}background task ended {DateTime.UtcNow}");
            });
 
            return Ok();
        }

        /// <summary>
        /// Admin: Unregister webhook for all user profiles.
        /// Requires correct "debugtoken" header value.
        /// </summary>
        [HttpPost]
        [Route("/[controller]/orcidwebhook/unregister/all")]
        public async Task<IActionResult> UnregisterOrcidWebhookForAllUserprofiles()
        {
            // Check that "DEBUGTOKEN" is defined and has a value in configuration and that the request header "debugtoken" matches.
            if (string.IsNullOrWhiteSpace(Configuration["DEBUGTOKEN"]) || Request.Headers["debugtoken"] != Configuration["DEBUGTOKEN"])
            {
                return Unauthorized();
            }

            string logPrefix = "AdminController: unregister ORCID webhook for all user profiles: ";

            // Check that ORCID webhook feature is enabled
            if (!_orcidApiService.IsOrcidWebhookEnabled())
            {
                _logger.LogError($"{logPrefix}failed: ORCID webhook feature disabled in configuration");
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            // Unregister webhooks in a background task
            await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
            {
                _logger.LogInformation($"{logPrefix}background task started {DateTime.UtcNow}");

                // Create service scope and get required services.
                // Do not use services from controller scope in a background task.
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IAdminService localAdminService = scope.ServiceProvider.GetRequiredService<IAdminService>();
                await localAdminService.UnregisterOrcidWebhookForAllUserprofiles();

                _logger.LogInformation($"{logPrefix}background task ended {DateTime.UtcNow}");
            });

            return Ok();
        }
    }
}