using api.Services;
using api.Models.Api;
using api.Models.Common;
using api.Models.Ttv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using api.Models.ProfileEditor;
using System;
using Microsoft.Extensions.Logging;
using api.Models.Log;

namespace api.Controllers
{
    /*
     * ProfileSettingsController implements profile editor settings related APIs.
     */
    [Route("api/settings")]
    [ApiController]
    [Authorize(Policy = "RequireScopeApi1AndClaimOrcid")]
    public class ProfileSettingsController : TtvControllerBase
    {
        private readonly IElasticsearchService _elasticsearchService;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<UserProfileController> _logger;

        public ProfileSettingsController(IElasticsearchService elasticsearchService, ILogger<UserProfileController> logger, IBackgroundTaskQueue taskQueue)
        {
            _elasticsearchService = elasticsearchService;
            _taskQueue = taskQueue;
            _logger = logger;
        }

        /// <summary>
        /// Hide user profile from portal by removing the profile from Elasticsearch index.
        /// User profile is not deleted from database.
        /// To restore the profile in portal, the user has to publish the profile again.
        /// </summary>
        [HttpGet]
        [Route("hideprofile")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> HideProfileFromPortal()
        {
            // Get ORCID id
            string orcidId = GetOrcidId();

            _logger.LogInformation(
                        "{@UserIdentification}, {@ApiInfo}",
                        this.GetUserIdentification(),
                        new ApiInfo(action: LogContent.Action.ELASTICSEARCH_DELETE, message: "ProfileSettingsController: hide profile"));

            // Remove entry from Elasticsearch index in a background task.
            // ElasticsearchService is singleton, no need to create local scope.
            if (_elasticsearchService.IsElasticsearchSyncEnabled())
            {
                await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
                {
                    // Update Elasticsearch person index.
                    bool deleteSuccess = await _elasticsearchService.DeleteEntryFromElasticsearchPersonIndex(orcidId);
                    if (!deleteSuccess)
                    {
                        _logger.LogError(
                            "{@UserIdentification}, {@ApiInfo}",
                            this.GetUserIdentification(),
                            new ApiInfo(action: LogContent.Action.ELASTICSEARCH_DELETE, success: false));
                    }
                });
            }

            return Ok(new ApiResponse());
        }
    }
}