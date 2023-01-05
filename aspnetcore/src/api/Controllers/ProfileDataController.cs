using api.Services;
using api.Models.Api;
using api.Models.Common;
using api.Models.Ttv;
using api.Models.ProfileEditor;
using api.Models.ProfileEditor.Items;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using api.Models.Log;

namespace api.Controllers
{
    /*
     * ProfileDataController implements profile editor API commands, such as getting editor data and setting data visibility.
     */
    [Route("api/profiledata")]
    [ApiController]
    [Authorize(Policy = "RequireScopeApi1AndClaimOrcid")]
    public class ProfileDataController : TtvControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ITtvSqlService _ttvSqlService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<UserProfileController> _logger;
        private readonly IBackgroundProfiledata _backgroundProfiledata;
        private readonly IBackgroundTaskQueue _taskQueue;

        public ProfileDataController(IUserProfileService userProfileService,
            IElasticsearchService elasticsearchService,
            ITtvSqlService ttvSqlService,
            IMemoryCache memoryCache,
            ILogger<UserProfileController> logger,
            IBackgroundProfiledata backgroundProfiledata,
            IBackgroundTaskQueue taskQueue)
        {
            _userProfileService = userProfileService;
            _cache = memoryCache;
            _elasticsearchService = elasticsearchService;
            _ttvSqlService = ttvSqlService;
            _backgroundProfiledata = backgroundProfiledata;
            _logger = logger;
            _taskQueue = taskQueue;
        }

        /// <summary>
        /// Get profile data. New version using different data structure.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseProfileDataGet), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            // Get ORCID id
            string orcidId = GetOrcidId();

            // Check that userprofile exists.
            if (!await _userProfileService.UserprofileExistsForOrcidId(orcidId: orcidId))
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Get userprofile id
            int userprofileId = await _userProfileService.GetUserprofileId(orcidId);

            // Cache key
            string cacheKey = orcidId;

            // Send cached response, if exists.
            if (_cache.TryGetValue(cacheKey, out ProfileEditorDataResponse cachedResponse))
            {
                return Ok(new ApiResponseProfileDataGet(success: true, reason: "", data: cachedResponse, fromCache: true));
            }

            // Get profile data
            ProfileEditorDataResponse profileDataResponse = await _userProfileService.GetProfileDataAsync(userprofileId: userprofileId, logUserIdentification: this.GetLogUserIdentification());

            // Save response in cache
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromSeconds(Constants.Cache.MEMORY_CACHE_EXPIRATION_SECONDS));

            // Save data in cache
            _cache.Set(cacheKey, profileDataResponse, cacheEntryOptions);

            return Ok(new ApiResponseProfileDataGet(success: true, reason: "", data: profileDataResponse, fromCache: false));
        }



        /// <summary>
        /// Modify profile data.
        /// </summary>
        [HttpPatch]
        [ProducesResponseType(typeof(ApiResponseProfileDataPatch), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchMany([FromBody] ProfileEditorDataModificationRequest profileEditorDataModificationRequest)
        {
            // Return immediately if there is nothing to change.
            if (profileEditorDataModificationRequest.groups.Count == 0 && profileEditorDataModificationRequest.items.Count == 0)
            {
                return Ok(new ApiResponse(success: true));
            }

            // Get ORCID id
            string orcidId = GetOrcidId();

            // Check that userprofile exists.
            if (!await _userProfileService.UserprofileExistsForOrcidId(orcidId: orcidId))
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Get userprofile id
            int userprofileId = await _userProfileService.GetUserprofileId(orcidId);

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);


            // Collect information about updated items to a response object, which will be sent in response.
            ProfileEditorDataModificationResponse profileEditorDataModificationResponse = new();

            // Set 'Show' and 'PrimaryValue' in FactFieldValues
            foreach (ProfileEditorItemMeta profileEditorItemMeta in profileEditorDataModificationRequest.items.ToList())
            {
                string updateSql = _ttvSqlService.GetSqlQuery_Update_FactFieldValues(userprofileId, profileEditorItemMeta);
                await _userProfileService.ExecuteRawSql(updateSql);
                profileEditorDataModificationResponse.items.Add(profileEditorItemMeta);
            }
            
            // Update Elasticsearch index in a background task.
            // ElasticsearchService is singleton, no need to create local scope.
            if (_elasticsearchService.IsElasticsearchSyncEnabled())
            {
                LogUserIdentification logUserIdentification = this.GetLogUserIdentification();

                await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
                {
                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.ELASTICSEARCH_UPDATE,
                            state: LogContent.ActionState.START));

                    // Get Elasticsearch person entry from profile data.
                    Models.Elasticsearch.ElasticsearchPerson person =
                        await _backgroundProfiledata.GetProfiledataForElasticsearch(
                            orcidId: orcidId,
                            userprofileId: userprofileId,
                            logUserIdentification: logUserIdentification);

                    // Update Elasticsearch person index.
                    await _elasticsearchService.UpdateEntryInElasticsearchPersonIndex(orcidId, person, logUserIdentification);

                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.ELASTICSEARCH_UPDATE,
                            state: LogContent.ActionState.COMPLETE));
                });
            }

            return Ok(new ApiResponseProfileDataPatch(success: true, reason: "", data: profileEditorDataModificationResponse, fromCache: false));
        }
    }
}