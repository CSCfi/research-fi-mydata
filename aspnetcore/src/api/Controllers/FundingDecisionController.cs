using api.Services;
using api.Models.Api;
using api.Models.Log;
using api.Models.Ttv;
using api.Models.ProfileEditor;
using api.Models.ProfileEditor.Items;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using api.Models.Common;
using System;

namespace api.Controllers
{
    /*
     * FundingController implements profile editor API commands for adding and deleting profile's fundings.
     */
    [Route("api/fundingdecision")]
    [ApiController]
    [Authorize(Policy = "RequireScopeApi1AndClaimOrcid")]
    public class FundingDecisionController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly IUserProfileService _userProfileService;
        private readonly IUtilityService _utilityService;
        private readonly IDataSourceHelperService _dataSourceHelperService;
        private readonly ILanguageService _languageService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<UserProfileController> _logger;

        public FundingDecisionController(TtvContext ttvContext, IUserProfileService userProfileService,
            IUtilityService utilityService, IDataSourceHelperService dataSourceHelperService,
            IMemoryCache memoryCache, ILanguageService languageService,
            ILogger<UserProfileController> logger,
            IBackgroundProfiledata backgroundProfiledata,
            IBackgroundTaskQueue taskQueue)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _utilityService = utilityService;
            _dataSourceHelperService = dataSourceHelperService;
            _languageService = languageService;
            _cache = memoryCache;
            _logger = logger;
        }

        /// <summary>
        /// Add funding decision(s) to user profile.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseFundingDecisionPostMany), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostMany([FromBody] List<ProfileEditorFundingDecisionToAdd> profileEditorFundingDecisionsToAdd)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.INVALID_REQUEST));
            }

            // Return immediately if there is nothing to add
            if (profileEditorFundingDecisionsToAdd.Count == 0)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.NOTHING_TO_ADD));
            }

            // Get ORCID id
            string orcidId = GetOrcidId();

            // Check that userprofile exists.
            (bool userprofileExists, int userprofileId) = await _userProfileService.GetUserprofileIdForOrcidId(orcidId);
            if (!userprofileExists)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.PROFILE_NOT_FOUND));
            }

            DimUserProfile dimUserProfile = await _ttvContext.DimUserProfiles.Where(dup => dup.Id == userprofileId)
                .Include(dup => dup.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION))
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimRegisteredDataSource)
                            .ThenInclude(drds => drds.DimOrganization).AsNoTracking()
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimFundingDecisionId != -1))
                    .ThenInclude(ffv => ffv.DimFundingDecision).FirstOrDefaultAsync();

            // TODO: Currently all added funding decisions get the same data source (Tiedejatutkimus.fi)

            // Get DimFieldDisplaySetting for funding decision
            DimFieldDisplaySetting dimFieldDisplaySettingsFundingDecision =
                dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION);

            // Registered data source organization name translation
            NameTranslation nameTranslation_OrganizationName = _languageService.GetNameTranslation(
                nameFi: _dataSourceHelperService.DimOrganizationNameFi_TTV,
                nameSv: _dataSourceHelperService.DimOrganizationNameSv_TTV,
                nameEn: _dataSourceHelperService.DimOrganizationNameEn_TTV
            );

            // Response object
            ProfileEditorAddFundingDecisionResponse profileEditorAddFundingDecisionResponse = new()
            {
                source = new ProfileEditorSource()
                {
                    RegisteredDataSource = _dataSourceHelperService.DimRegisteredDataSourceName_TTV,
                    Organization = new Organization()
                    {
                        NameFi = nameTranslation_OrganizationName.NameFi,
                        NameSv = nameTranslation_OrganizationName.NameSv,
                        NameEn = nameTranslation_OrganizationName.NameEn
                    }
                }
            };


            // Loop funding decisions, ignore possible duplicates
            foreach (ProfileEditorFundingDecisionToAdd fundingDecisionToAdd in profileEditorFundingDecisionsToAdd.DistinctBy(f => f.ProjectId))
            {
                bool fundingDecisionProcessed = false;
                // Check if userprofile already includes given funding decision
                foreach (FactFieldValue ffv in dimUserProfile.FactFieldValues)
                {
                    if (ffv.DimFundingDecision.Id == fundingDecisionToAdd.ProjectId)
                    {
                        // Funding decision is already in profile
                        profileEditorAddFundingDecisionResponse.fundingDecisionsAlreadyInProfile.Add(fundingDecisionToAdd.ProjectId);
                        fundingDecisionProcessed = true;
                        break;
                    }
                }

                if (!fundingDecisionProcessed)
                {
                    // Get DimFundingDecision
                    DimFundingDecision dimFundingDecision =
                        await _ttvContext.DimFundingDecisions.AsNoTracking().FirstOrDefaultAsync(dfd => dfd.Id == fundingDecisionToAdd.ProjectId);
                    // Check if exists
                    if (dimFundingDecision == null)
                    {
                        // Does not exist
                        profileEditorAddFundingDecisionResponse.fundingDecisionsNotFound.Add(fundingDecisionToAdd.ProjectId);
                    }
                    else
                    {
                        // Add FactFieldValue
                        FactFieldValue factFieldValueFunding = _userProfileService.GetEmptyFactFieldValue();
                        factFieldValueFunding.Show = fundingDecisionToAdd.Show != null ? fundingDecisionToAdd.Show : false;
                        factFieldValueFunding.PrimaryValue = fundingDecisionToAdd.PrimaryValue != null ? fundingDecisionToAdd.PrimaryValue : false;
                        factFieldValueFunding.DimUserProfileId = dimUserProfile.Id;
                        factFieldValueFunding.DimFieldDisplaySettingsId = dimFieldDisplaySettingsFundingDecision.Id;
                        factFieldValueFunding.DimFundingDecisionId = dimFundingDecision.Id;
                        factFieldValueFunding.DimRegisteredDataSourceId = _dataSourceHelperService.DimRegisteredDataSourceId_TTV;
                        factFieldValueFunding.SourceId = Constants.SourceIdentifiers.TIEDEJATUTKIMUS;
                        factFieldValueFunding.Created = _utilityService.GetCurrentDateTime();
                        factFieldValueFunding.Modified = _utilityService.GetCurrentDateTime();
                        _ttvContext.FactFieldValues.Add(factFieldValueFunding);
                        await _ttvContext.SaveChangesAsync();

                        profileEditorAddFundingDecisionResponse.fundingDecisionsAdded.Add(fundingDecisionToAdd.ProjectId);
                    }
                }
            }

            // Update Elasticsearch index.
            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();
            await _userProfileService.UpdateProfileInElasticsearch(
                orcidId: orcidId,
                userprofileId: dimUserProfile.Id,
                logUserIdentification: logUserIdentification);

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);

            return Ok(new ApiResponseFundingDecisionPostMany(success: true, reason:"", data: profileEditorAddFundingDecisionResponse, fromCache: false));
        }

        /// <summary>
        /// Remove funding decision(s) from user profile.
        /// </summary>
        [HttpPost]
        [Route("remove")]
        [ProducesResponseType(typeof(ApiResponseFundingDecisionRemoveMany), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveMany([FromBody] List<int> projectIds)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.INVALID_REQUEST));
            }

            // Return immediately if there is nothing to remove
            if (projectIds.Count == 0)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.NOTHING_TO_REMOVE));
            }

            // Get ORCID id
            string orcidId = GetOrcidId();

            // Check that userprofile exists.
            (bool userprofileExists, int userprofileId) = await _userProfileService.GetUserprofileIdForOrcidId(orcidId);
            if (!userprofileExists)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.PROFILE_NOT_FOUND));
            }

            // Response object
            ProfileEditorRemoveFundingDecisionResponse profileEditorRemoveFundingDecisionResponse = new();

            // Remove FactFieldValues
            foreach(int projectId in projectIds.Distinct())
            {
                FactFieldValue factFieldValue = await _ttvContext.FactFieldValues
                    .Where(
                        ffv => ffv.DimUserProfileId == userprofileId &&
                        ffv.DimFundingDecisionId != -1 &&
                        ffv.DimFundingDecision.Id == projectId
                     ).FirstOrDefaultAsync();

                if (factFieldValue != null)
                {
                    _ttvContext.FactFieldValues.Remove(factFieldValue);
                    profileEditorRemoveFundingDecisionResponse.fundingDecisionsRemoved.Add(projectId);
                }
                else
                {
                    profileEditorRemoveFundingDecisionResponse.fundingDecisionsNotFound.Add(projectId);
                }
            }
            await _ttvContext.SaveChangesAsync();

            // Update Elasticsearch index.
            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();
            await _userProfileService.UpdateProfileInElasticsearch(
                orcidId: orcidId,
                userprofileId: userprofileId,
                logUserIdentification: logUserIdentification);

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);

            return Ok(new ApiResponseFundingDecisionRemoveMany(success: true, reason: "removed", data: profileEditorRemoveFundingDecisionResponse, fromCache: false));
        }
    }
}