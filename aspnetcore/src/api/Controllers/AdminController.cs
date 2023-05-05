using api.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using api.Models.Ttv;
using api.Models.Log;

namespace api.Controllers
{
    /*
     * AdminController implements API for administration commands.
     */
    [ApiController]
    public class AdminController : TtvAdminControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IOrcidApiService _orcidApiService;
        private readonly IUserProfileService _userProfileService;
        private readonly ILogger<UserProfileController> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public IConfiguration Configuration { get; }

        public AdminController(
            IAdminService adminService,
            IConfiguration configuration,
            IOrcidApiService orcidApiService,
            IUserProfileService userProfileService,
            ILogger<UserProfileController> logger,
            IMemoryCache memoryCache,
            IBackgroundTaskQueue taskQueue,
            IServiceScopeFactory serviceScopeFactory)
        {
            _adminService = adminService;
            _orcidApiService = orcidApiService;
            _userProfileService = userProfileService;
            _logger = logger;
            _taskQueue = taskQueue;
            _serviceScopeFactory = serviceScopeFactory;
            Configuration = configuration;
        }


        /// <summary>
        /// Admin: Register webhook for a single user profile.
        /// </summary>
        [HttpPost]
        [Route("/[controller]/orcidwebhook/register/single/{webhookOrcidId}")]
        public async Task<IActionResult> RegisterOrcidWebhookForSingleUserprofile(string webhookOrcidId)
        {
            // Validate request data
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // Check admin token authorization
            if (!IsAdminTokenAuthorized(Configuration))
            {
                return Unauthorized();
            }

            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();

            // Check that ORCID webhook feature is enabled
            if (!_orcidApiService.IsOrcidWebhookEnabled())
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ADMIN_WEBHOOK_ORCID_REGISTER,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: "disabled in configuration"));
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            // Register webhook in a background task
            await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ADMIN_WEBHOOK_ORCID_REGISTER,
                        state: LogContent.ActionState.START,
                        message: $"(ORCID={webhookOrcidId})"));

                // Create service scope and get required services.
                // Do not use services from controller scope in a background task.
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IAdminService localAdminService = scope.ServiceProvider.GetRequiredService<IAdminService>();
                await localAdminService.RegisterOrcidWebhookForSingleUserprofile(webhookOrcidId);


                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ADMIN_WEBHOOK_ORCID_REGISTER,
                        state: LogContent.ActionState.COMPLETE,
                        message: $"(ORCID={webhookOrcidId})"));
            });

            return Ok();
        }

        /// <summary>
        /// Admin: Unregister webhook for a single user profile.
        /// </summary>
        [HttpPost]
        [Route("/[controller]/orcidwebhook/unregister/single/{webhookOrcidId}")]
        public async Task<IActionResult> UnregisterOrcidWebhookForSingleUserprofile(string webhookOrcidId)
        {
            // Validate request data
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // Check admin token authorization
            if (!IsAdminTokenAuthorized(Configuration))
            {
                return Unauthorized();
            }

            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();

            // Check that ORCID webhook feature is enabled
            if (!_orcidApiService.IsOrcidWebhookEnabled())
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ADMIN_WEBHOOK_ORCID_UNREGISTER,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: "disabled in configuration"));
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            // Register webhook in a background task
            await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ADMIN_WEBHOOK_ORCID_UNREGISTER,
                        state: LogContent.ActionState.START,
                        message: $"ORCID={webhookOrcidId}"));

                // Create service scope and get required services.
                // Do not use services from controller scope in a background task.
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IAdminService localAdminService = scope.ServiceProvider.GetRequiredService<IAdminService>();
                await localAdminService.UnregisterOrcidWebhookForSingleUserprofile(webhookOrcidId);

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ADMIN_WEBHOOK_ORCID_UNREGISTER,
                        state: LogContent.ActionState.COMPLETE,
                        message: $"ORCID={webhookOrcidId}"));
            });

            return Ok();
        }

        /// <summary>
        /// Admin: Register webhook for all user profiles.
        /// </summary>
        [HttpPost]
        [Route("/[controller]/orcidwebhook/register/all")]
        public async Task<IActionResult> RegisterOrcidWebhookForAllUserprofiles()
        {
            // Check admin token authorization
            if (!IsAdminTokenAuthorized(Configuration))
            {
                return Unauthorized();
            }

            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();

            // Check that ORCID webhook feature is enabled
            if (!_orcidApiService.IsOrcidWebhookEnabled())
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ADMIN_WEBHOOK_ORCID_REGISTER_ALL,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: "disabled in configuration"));
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            // Register webhooks in a background task
            await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ADMIN_WEBHOOK_ORCID_REGISTER_ALL,
                        state: LogContent.ActionState.START));

                // Create service scope and get required services.
                // Do not use services from controller scope in a background task.
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IAdminService localAdminService = scope.ServiceProvider.GetRequiredService<IAdminService>();
                await localAdminService.RegisterOrcidWebhookForAllUserprofiles();

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ADMIN_WEBHOOK_ORCID_REGISTER_ALL,
                        state: LogContent.ActionState.COMPLETE));
            });

            return Ok();
        }

        /// <summary>
        /// Admin: Unregister webhook for all user profiles.
        /// </summary>
        [HttpPost]
        [Route("/[controller]/orcidwebhook/unregister/all")]
        public async Task<IActionResult> UnregisterOrcidWebhookForAllUserprofiles()
        {
            // Check admin token authorization
            if (!IsAdminTokenAuthorized(Configuration))
            {
                return Unauthorized();
            }

            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();

            // Check that ORCID webhook feature is enabled
            if (!_orcidApiService.IsOrcidWebhookEnabled())
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ADMIN_WEBHOOK_ORCID_UNREGISTER_ALL,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: "disabled in configuration"));
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            // Unregister webhooks in a background task
            await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ADMIN_WEBHOOK_ORCID_UNREGISTER_ALL,
                        state: LogContent.ActionState.START));

                // Create service scope and get required services.
                // Do not use services from controller scope in a background task.
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IAdminService localAdminService = scope.ServiceProvider.GetRequiredService<IAdminService>();
                await localAdminService.UnregisterOrcidWebhookForAllUserprofiles();

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ADMIN_WEBHOOK_ORCID_UNREGISTER_ALL,
                        state: LogContent.ActionState.COMPLETE));
            });

            return Ok();
        }

        /// <summary>
        /// Admin: Update user profile in Elasticsearch.
        /// </summary>
        [HttpPost]
        [Route("/[controller]/elasticsearch/updateprofile/{dimUserProfileId}")]
        public async Task<IActionResult> UpdateUserprofileInElasticsearch(int dimUserProfileId)
        {
            // Validate request data
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // Check admin token authorization
            if (!IsAdminTokenAuthorized(Configuration))
            {
                return Unauthorized();
            }

            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();
            await _adminService.UpdateUserprofileInElasticsearch(dimUserProfileId: dimUserProfileId, logUserIdentification: logUserIdentification);
            return Ok();
        }



        /// <summary>
        /// Admin: Update all user profiles in Elasticsearch.
        /// </summary>
        [HttpPost]
        [Route("/[controller]/elasticsearch/updateprofile/all")]
        public async Task<IActionResult> UpdateAllUserprofilesInElasticsearch()
        {
            // Validate request data
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // Check admin token authorization
            if (!IsAdminTokenAuthorized(Configuration))
            {
                return Unauthorized();
            }

            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();
            await _adminService.UpdateAllUserprofilesInElasticsearch(logUserIdentification);
            return Ok();
        }




        /// <summary>
        /// Admin: Add new TTV data in user profile.
        /// </summary>
        [HttpPost]
        [Route("/[controller]/userprofile/addttvdata/{dimUserProfileId}")]
        public async Task<IActionResult> AddNewTtvDataInUserProfile(int dimUserProfileId)
        {
            // Validate request data
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // Check admin token authorization
            if (!IsAdminTokenAuthorized(Configuration))
            {
                return Unauthorized();
            }

            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();

            await _adminService.AddNewTtvDataInUserProfileBackground(dimUserProfileId, logUserIdentification);

            return Ok();
        }
    }
}