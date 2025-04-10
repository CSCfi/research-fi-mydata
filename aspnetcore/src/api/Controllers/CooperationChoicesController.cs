using api.Services;
using api.Models.Api;
using api.Models.Common;
using api.Models.ProfileEditor;
using api.Models.Ttv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using AutoMapper;
using Microsoft.Extensions.Logging;
using api.Models.Log;

namespace api.Controllers
{
    /*
     * CooperationChoicesController implements profile editor API for setting user choices for cooperation.
     */
    [Route("api/cooperationchoices")]
    [ApiController]
    [Authorize(Policy = "RequireScopeApi1AndClaimOrcid")]
    public class CooperationChoicesController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly IUserProfileService _userProfileService;
        private readonly ITtvSqlService _ttvSqlService;
        private readonly IUtilityService _utilityService;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly ILogger<UserProfileController> _logger;

        public CooperationChoicesController(TtvContext ttvContext, IUserProfileService userProfileService,
            ITtvSqlService ttvSqlService, IUtilityService utilityService, IMemoryCache memoryCache, IMapper mapper, ILogger<UserProfileController> logger)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _ttvSqlService = ttvSqlService;
            _utilityService = utilityService;
            _cache = memoryCache;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get cooperation selections.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseCooperationGet), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            // Get ORCID id
            string orcidId = GetOrcidId();

            // Check that userprofile exists.
            (bool userprofileExists, int userprofileId) = await _userProfileService.GetUserprofileIdForOrcidId(orcidId);
            if (!userprofileExists)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.PROFILE_NOT_FOUND));
            }

            // Send cached response, if exists.
            string cacheKey = _userProfileService.GetCMemoryCacheKey_UserChoices(orcidId);
            if (_cache.TryGetValue(cacheKey, out List<ProfileEditorCooperationItem> cachedResponse))
            {
                return Ok(new ApiResponseCooperationGet(success: true, reason: "", data: cachedResponse, fromCache: true));
            }

            // Get choices from DimReferencedata by code scheme.
            // MUST NOT use AsNoTracking, because it is possible that DimUserChoise entities will be added.
            List<DimReferencedatum> dimReferenceDataUserChoices = await _ttvContext.DimReferencedata.TagWith("Get user choices").Where(dr => dr.CodeScheme == Constants.ReferenceDataCodeSchemes.USER_CHOICES)
                .Include(dr => dr.DimUserChoices.Where(duc => duc.DimUserProfileId == userprofileId)).ToListAsync();

            // Chech that all available choices have DimUserChoice for this user profile.
            foreach (DimReferencedatum dimReferenceDataUserChoice in dimReferenceDataUserChoices)
            {
                DimUserChoice dimUserChoice = dimReferenceDataUserChoice.DimUserChoices.FirstOrDefault();
                if (dimUserChoice == null)
                {
                    // Add new DimUserChoice
                    dimUserChoice = new DimUserChoice()
                    {
                        UserChoiceValue = false,
                        DimUserProfileId = userprofileId,
                        DimReferencedataIdAsUserChoiceLabelNavigation = dimReferenceDataUserChoice,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = _utilityService.GetCurrentDateTime(),
                        Modified = _utilityService.GetCurrentDateTime()
                    };
                    _ttvContext.DimUserChoices.Add(dimUserChoice);
                }
            }
            await _ttvContext.SaveChangesAsync();

            // Map to API response.
            List<ProfileEditorCooperationItem> cooperationItems = _mapper.Map<List<ProfileEditorCooperationItem>>(dimReferenceDataUserChoices);

            // Save response in cache
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromSeconds(Constants.Cache.MEMORY_CACHE_EXPIRATION_SECONDS));
            _cache.Set(cacheKey, cooperationItems, cacheEntryOptions);

            return Ok(new ApiResponseCooperationGet(success: true, reason: "", data: cooperationItems, fromCache: false));
        }


        /// <summary>
        /// Modify cooperation selections.
        /// </summary>
        [HttpPatch]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchMany([FromBody] List<ProfileEditorCooperationItem> profileEditorCooperationItems)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.INVALID_REQUEST));
            }

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                this.GetLogUserIdentification(),
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_MODIFY_COOPERATION_CHOICES,
                    state: LogContent.ActionState.START));

            // Return immediately if there is nothing to modify.
            if (profileEditorCooperationItems.Count == 0)
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    this.GetLogUserIdentification(),
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_MODIFY_COOPERATION_CHOICES,
                        state: LogContent.ActionState.COMPLETE));

                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.NOTHING_TO_MODIFY));
            }

            // Get ORCID id
            string orcidId = GetOrcidId();

            // Check that userprofile exists.
            (bool userprofileExists, int userprofileId) = await _userProfileService.GetUserprofileIdForOrcidId(orcidId);
            if (!userprofileExists)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.PROFILE_NOT_FOUND));
            }

            // Remove cached profile data response.
            string cacheKey = _userProfileService.GetCMemoryCacheKey_UserChoices(orcidId);
            _cache.Remove(cacheKey);

            // Save cooperation selections
            foreach (ProfileEditorCooperationItem profileEditorCooperationItem in profileEditorCooperationItems)
            {
                DimUserChoice dimUserChoice = await _ttvContext.DimUserChoices.Where(duc => duc.DimUserProfileId == userprofileId && duc.Id == profileEditorCooperationItem.Id).FirstOrDefaultAsync();
                if (dimUserChoice != null)
                {
                    dimUserChoice.UserChoiceValue = profileEditorCooperationItem.Selected;
                }
            }
            await _ttvContext.SaveChangesAsync();

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                this.GetLogUserIdentification(),
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_MODIFY_COOPERATION_CHOICES,
                    state: LogContent.ActionState.COMPLETE));

            // Refresh 'modified' timestamp in user profile
            await _userProfileService.SetModifiedTimestampInUserProfile(userprofileId);

            return Ok(new ApiResponse(success: true));
        }
    }
}