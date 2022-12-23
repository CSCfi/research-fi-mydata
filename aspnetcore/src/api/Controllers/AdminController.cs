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

namespace api.Controllers
{
    /*
     * AdminController implements API for administration commands.
     */
    [ApiController]
    public class AdminController : TtvAdminControllerBase
    {
        private readonly IOrcidApiService _orcidApiService;
        private readonly IUserProfileService _userProfileService;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ILogger<UserProfileController> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IBackgroundProfiledata _backgroundProfiledata;
        public IConfiguration Configuration { get; }

        public AdminController(
            IConfiguration configuration,
            IOrcidApiService orcidApiService,
            IUserProfileService userProfileService,
            IElasticsearchService elasticsearchService,
            ILogger<UserProfileController> logger,
            IMemoryCache memoryCache,
            IBackgroundTaskQueue taskQueue,
            IBackgroundProfiledata backgroundProfiledata,
            IServiceScopeFactory serviceScopeFactory)
        {
            _orcidApiService = orcidApiService;
            _userProfileService = userProfileService;
            _elasticsearchService = elasticsearchService;
            _logger = logger;
            _taskQueue = taskQueue;
            _backgroundProfiledata = backgroundProfiledata;
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
        /// </summary>
        ///         [HttpPost]
        [Route("/[controller]/orcidwebhook/unregister/single/{webhookOrcidId}")]
        public async Task<IActionResult> UnegisterOrcidWebhookForSingleUserprofile(string webhookOrcidId)
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

            string logPrefix = $"AdminController: update Elasticsearch index (dim_user_profile.id={dimUserProfileId}):";

            // Get DimUserProfile
            DimUserProfile dimUserProfile = await _userProfileService.GetUserprofileById(dimUserProfileId);
            // Check that user profile exists
            if (dimUserProfile == null)
            {
                _logger.LogError($"{logPrefix}user profile not found");
                return NotFound();
            }

            // Store ORCID ID for background process
            string orcidId = dimUserProfile.OrcidId;

            // Check if the profile should be updated or deleted in Elasticsearch index
            bool isUserprofilePublished = await _userProfileService.IsUserprofilePublished(dimUserProfileId);

            if (isUserprofilePublished)
            {
                // User profile is published. Update Elasticsearch index.
                // Update Elasticsearch index in a background task.
                // ElasticsearchService is singleton, no need to create local scope.
                if (_elasticsearchService.IsElasticsearchSyncEnabled())
                {
                    await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
                    {
                        _logger.LogInformation($"Elasticsearch index update for {orcidId} started from AdminController {DateTime.UtcNow}");
                        // Get Elasticsearch person entry from profile data.
                        Models.Elasticsearch.ElasticsearchPerson person = await _backgroundProfiledata.GetProfiledataForElasticsearch(orcidId, dimUserProfileId);
                        // Update Elasticsearch person index.
                        await _elasticsearchService.UpdateEntryInElasticsearchPersonIndex(orcidId, person);
                        _logger.LogInformation($"Elasticsearch index update for {orcidId} from AdminController completed {DateTime.UtcNow}");
                    });
                }
            }
            else
            {
                // User profile is not published. Delete from Elasticsearch index.
                // Remove entry from Elasticsearch index in a background task.
                // ElasticsearchService is singleton, no need to create local scope.
                if (_elasticsearchService.IsElasticsearchSyncEnabled())
                {
                    await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
                    {
                        _logger.LogInformation($"Elasticsearch delete for {orcidId} started from AdminController {DateTime.UtcNow}");
                        // Update Elasticsearch person index.
                        bool deleteSuccess = await _elasticsearchService.DeleteEntryFromElasticsearchPersonIndex(orcidId);
                        if (deleteSuccess)
                        {
                            _logger.LogInformation($"Elasticsearch delete for {orcidId} from AdminController completed {DateTime.UtcNow}");
                        }
                        else
                        {
                            _logger.LogError($"Elasticsearch delete for {orcidId} from AdminController failed {DateTime.UtcNow}");
                        }
                    });
                }
            }

            return Ok();
        }
    }
}