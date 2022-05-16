﻿using api.Services;
using api.Models;
using api.Models.Ttv;
using api.Models.ProfileEditor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace api.Controllers
{
    /*
     * ProfileDataController2 implements profile editor API commands, such as getting editor data and setting data visibility.
     */
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RequireScopeApi1AndClaimOrcid")]
    public class ProfileDataController2 : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;
        private readonly ElasticsearchService _elasticsearchService;
        private readonly TtvSqlService _ttvSqlService;
        private readonly LanguageService _languageService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<UserProfileController> _logger;
        private readonly BackgroundElasticsearchPersonUpdateQueue _backgroundElasticsearchPersonUpdateQueue;
        private readonly BackgroundProfiledata _backgroundProfiledata;

        public ProfileDataController2(TtvContext ttvContext, UserProfileService userProfileService, ElasticsearchService elasticsearchService, TtvSqlService ttvSqlService, LanguageService languageService, IMemoryCache memoryCache, ILogger<UserProfileController> logger, BackgroundElasticsearchPersonUpdateQueue backgroundElasticsearchPersonUpdateQueue, BackgroundProfiledata backgroundProfiledata)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _cache = memoryCache;
            _elasticsearchService = elasticsearchService;
            _ttvSqlService = ttvSqlService;
            _languageService = languageService;
            _backgroundElasticsearchPersonUpdateQueue = backgroundElasticsearchPersonUpdateQueue;
            _backgroundProfiledata = backgroundProfiledata;
            _logger = logger;
        }

        /// <summary>
        /// Get profile data. New version using different data structure.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseProfileDataGet), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            // Check that user profile exists
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }
            // Cache key
            var cacheKey = orcidId + "_2";

            // Send cached response, if exists.
            ProfileEditorDataResponse cachedResponse;
            if (_cache.TryGetValue(cacheKey, out cachedResponse))
            {
                return Ok(new ApiResponseProfileDataGet(success: true, reason: "", data: cachedResponse, fromCache: true));
            }

            // Get profile data
            var profileDataResponse = await _userProfileService.GetProfileDataAsync2(userprofileId);

            // Save response in cache
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromSeconds(Constants.Cache.MEMORY_CACHE_EXPIRATION_SECONDS));

            // Save data in cache
            _cache.Set(cacheKey, profileDataResponse, cacheEntryOptions);

            return Ok(new ApiResponseProfileDataGet2(success: true, reason: "", data: profileDataResponse, fromCache: false));
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

            // Check that user profile exists.
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);


            // Collect information about updated items to a response object, which will be sent in response.
            var profileEditorDataModificationResponse = new ProfileEditorDataModificationResponse();

            // Set 'Show' and 'PrimaryValue' in FactFieldValues
            foreach (ProfileEditorItemMeta profileEditorItemMeta in profileEditorDataModificationRequest.items.ToList())
            {
                var updateSql = _ttvSqlService.getSqlQuery_Update_FactFieldValues(userprofileId, profileEditorItemMeta);
                await _ttvContext.Database.ExecuteSqlRawAsync(updateSql);
                profileEditorDataModificationResponse.items.Add(profileEditorItemMeta);
            }

            // Update Elasticsearch index in a background task.
            _backgroundElasticsearchPersonUpdateQueue.QueueBackgroundWorkItem(async token =>
            {
                _logger.LogInformation($"Background task for updating {orcidId} started at {DateTime.UtcNow}");
                // Get Elasticsearch person entry from profile data.
                var person = await _backgroundProfiledata.GetProfiledataForElasticsearch(orcidId, userprofileId);
                // Update Elasticsearch person index.
                await _elasticsearchService.UpdateEntryInElasticsearchPersonIndex(orcidId, person);
                _logger.LogInformation($"Background task for updating {orcidId} ended at {DateTime.UtcNow}");
            });

            return Ok(new ApiResponseProfileDataPatch(success: true, reason: "", data: profileEditorDataModificationResponse, fromCache: false));
        }
    }
}