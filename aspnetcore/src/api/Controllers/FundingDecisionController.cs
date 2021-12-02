﻿using api.Services;
using api.Models;
using api.Models.Ttv;
using api.Models.ProfileEditor;
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

namespace api.Controllers
{
    /*
     * FundingController implements profile editor API commands for adding and deleting profile's fundings.
     */
    [Route("api/fundingdecision")]
    [ApiController]
    [Authorize]
    public class FundingDecisionController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;
        private readonly UtilityService _utilityService;
        private readonly LanguageService _languageService;
        private IMemoryCache _cache;
        private readonly ILogger<UserProfileController> _logger;

        public FundingDecisionController(TtvContext ttvContext, UserProfileService userProfileService, UtilityService utilityService, IMemoryCache memoryCache, ILogger<UserProfileController> logger, LanguageService languageService)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _utilityService = utilityService;
            _languageService = languageService;
            _logger = logger;
            _cache = memoryCache;
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
                return Ok(new ApiResponse(success: false, reason: "invalid request data"));
            }

            // Get userprofile
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                // Userprofile not found
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }
            var dimUserProfile = await _ttvContext.DimUserProfiles.Where(dup => dup.Id == userprofileId)
                .Include(dup => dup.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION))
                    .ThenInclude(dfds => dfds.BrFieldDisplaySettingsDimRegisteredDataSources)
                        .ThenInclude(br => br.DimRegisteredDataSource)
                            .ThenInclude(drds => drds.DimOrganization).AsNoTracking()
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimFundingDecisionId != -1))
                    .ThenInclude(ffv => ffv.DimFundingDecision).FirstOrDefaultAsync();

            // TODO: Currently all added funding decisions get the same data source (Tiedejatutkimus.fi)

            // Get Tiedejatutkimus.fi registered data source id
            var tiedejatutkimusRegisteredDataSourceId = await _userProfileService.GetTiedejatutkimusFiRegisteredDataSourceId();
            // Get DimFieldDisplaySetting for Tiedejatutkimus.fi
            var dimFieldDisplaySettingsFundingDecision = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION && dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSourceId == tiedejatutkimusRegisteredDataSourceId);

            // Organization name translation
            var nameTranslation_OrganizationName = _languageService.getNameTranslation(
                nameFi: dimFieldDisplaySettingsFundingDecision.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                nameSv: dimFieldDisplaySettingsFundingDecision.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv,
                nameEn: dimFieldDisplaySettingsFundingDecision.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn
            );

            // Response object
            var profileEditorAddFundingDecisionResponse = new ProfileEditorAddFundingDecisionResponse();
            profileEditorAddFundingDecisionResponse.source = new ProfileEditorSource()
            {
                Id = dimFieldDisplaySettingsFundingDecision.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                RegisteredDataSource = dimFieldDisplaySettingsFundingDecision.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                Organization = new Organization()
                {
                    NameFi = nameTranslation_OrganizationName.NameFi,
                    NameSv = nameTranslation_OrganizationName.NameSv,
                    NameEn = nameTranslation_OrganizationName.NameEn
                }
            };


            // Loop funding decisions
            foreach (ProfileEditorFundingDecisionToAdd fundingDecisionToAdd in profileEditorFundingDecisionsToAdd)
            {
                var fundingDecisionProcessed = false;
                // Check if userprofile already includes given funding decision
                foreach (FactFieldValue ffv in dimUserProfile.FactFieldValues)
                {
                    if (ffv.DimFundingDecision.FunderProjectNumber == fundingDecisionToAdd.FunderProjectNumber)
                    {
                        // Funding decision is already in profile
                        profileEditorAddFundingDecisionResponse.fundingDecisionsAlreadyInProfile.Add(fundingDecisionToAdd.FunderProjectNumber);
                        fundingDecisionProcessed = true;
                        break;
                    }
                }

                if (!fundingDecisionProcessed)
                {
                    // Get DimFundingDecision
                    var dimFundingDecision = await _ttvContext.DimFundingDecisions.AsNoTracking().FirstOrDefaultAsync(dfd => dfd.FunderProjectNumber == fundingDecisionToAdd.FunderProjectNumber);
                    // Check if exists
                    if (dimFundingDecision == null)
                    {
                        // Does not exist
                        profileEditorAddFundingDecisionResponse.fundingDecisionsNotFound.Add(fundingDecisionToAdd.FunderProjectNumber);
                    }
                    else
                    {
                        // Add FactFieldValue
                        var factFieldValueFunding = _userProfileService.GetEmptyFactFieldValue();
                        factFieldValueFunding.Show = fundingDecisionToAdd.Show != null ? fundingDecisionToAdd.Show : false;
                        factFieldValueFunding.PrimaryValue = fundingDecisionToAdd.PrimaryValue != null ? fundingDecisionToAdd.PrimaryValue : false;
                        factFieldValueFunding.DimUserProfileId = dimUserProfile.Id;
                        factFieldValueFunding.DimFieldDisplaySettingsId = dimFieldDisplaySettingsFundingDecision.Id;
                        factFieldValueFunding.DimFundingDecisionId = dimFundingDecision.Id;
                        factFieldValueFunding.SourceId = Constants.SourceIdentifiers.TIEDEJATUTKIMUS;
                        factFieldValueFunding.Created = _utilityService.getCurrentDateTime();
                        factFieldValueFunding.Modified = _utilityService.getCurrentDateTime();
                        _ttvContext.FactFieldValues.Add(factFieldValueFunding);
                        await _ttvContext.SaveChangesAsync();

                        profileEditorAddFundingDecisionResponse.fundingDecisionsAdded.Add(fundingDecisionToAdd.FunderProjectNumber);
                    }
                }
            }

            // TODO: add Elasticsearch sync?

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
        public async Task<IActionResult> RemoveMany([FromBody] List<string> funderProjectNumbers)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse(success: false, reason: "invalid request data"));
            }

            // Get id of userprofile
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                // Userprofile not found
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Response object
            var profileEditorRemoveFundingDecisionResponse = new ProfileEditorRemoveFundingDecisionResponse();

            // Remove FactFieldValues
            foreach(string funderProjectNumber in funderProjectNumbers.Distinct())
            {
                var factFieldValue = await _ttvContext.FactFieldValues
                    .Where(
                        ffv => ffv.DimUserProfileId == userprofileId &&
                        ffv.DimFundingDecisionId != -1 &&
                        ffv.DimFundingDecision.FunderProjectNumber == funderProjectNumber
                     ).FirstOrDefaultAsync();

                if (factFieldValue != null)
                {
                    _ttvContext.FactFieldValues.Remove(factFieldValue);
                    profileEditorRemoveFundingDecisionResponse.fundingDecisionsRemoved.Add(funderProjectNumber);
                }
                else
                {
                    profileEditorRemoveFundingDecisionResponse.fundingDecisionsNotFound.Add(funderProjectNumber);
                }
            }
            await _ttvContext.SaveChangesAsync();

            // TODO: add Elasticsearch sync?

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);

            return Ok(new ApiResponseFundingDecisionRemoveMany(success: true, reason: "removed", data: profileEditorRemoveFundingDecisionResponse, fromCache: false));
        }
    }
}