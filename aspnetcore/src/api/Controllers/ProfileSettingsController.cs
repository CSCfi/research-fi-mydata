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

            _logger.LogInformation(this.GetLogPrefix() + " hide profile request. Delete from Elasticsearch index.");

            // Remove entry from Elasticsearch index in a background task.
            if (_elasticsearchService.IsElasticsearchSyncEnabled())
            {
                await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
                {
                    // Update Elasticsearch person index.
                    bool deleteSuccess = await _elasticsearchService.DeleteEntryFromElasticsearchPersonIndex(orcidId);
                    if (deleteSuccess)
                    {
                        _logger.LogInformation($"Elasticsearch: {orcidId} delete OK.");
                    }
                    else
                    {
                        _logger.LogError($"Elasticsearch: {orcidId} delete failed.");
                    }
                });
            }

            return Ok(new ApiResponse());
        }
    }
}