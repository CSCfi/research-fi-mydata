using api.Services;
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
    [Route("api/funding")]
    [ApiController]
    [Authorize]
    public class FundingController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;
        private readonly UtilityService _utilityService;
        private IMemoryCache _cache;
        private readonly ILogger<UserProfileController> _logger;

        public FundingController(TtvContext ttvContext, UserProfileService userProfileService, UtilityService utilityService, IMemoryCache memoryCache, ILogger<UserProfileController> logger)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _utilityService = utilityService;
            _logger = logger;
            _cache = memoryCache;
        }

        /// <summary>
        /// Add funding(s) to user profile.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseFundingPostMany), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostMany([FromBody] List<ProfileEditorFundingToAdd> profileEditorFundingsToAdd)
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
            var dimUserProfile = await _ttvContext.DimUserProfiles
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.BrFieldDisplaySettingsDimRegisteredDataSources)
                        .ThenInclude(br => br.DimRegisteredDataSource)
                            .ThenInclude(drds => drds.DimOrganization).AsNoTracking()
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFundingDecision).AsNoTracking().FirstOrDefaultAsync(dup => dup.Id == userprofileId);

            // TODO: Currently all added fundings get the same data source (Tiedejatutkimus.fi)

            // Get Tiedejatutkimus.fi registered data source id
            var tiedejatutkimusRegisteredDataSourceId = await _userProfileService.GetTiedejatutkimusFiRegisteredDataSourceId();
            // Get DimFieldDisplaySetting for Tiedejatutkimus.fi
            var dimFieldDisplaySettingsFunding = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION && dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSourceId == tiedejatutkimusRegisteredDataSourceId);

            // Response object
            var profileEditorAddFundingResponse = new ProfileEditorAddFundingResponse();
            profileEditorAddFundingResponse.source = new ProfileEditorSource()
            {
                Id = dimFieldDisplaySettingsFunding.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                RegisteredDataSource = dimFieldDisplaySettingsFunding.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                Organization = new Organization()
                {
                    NameFi = dimFieldDisplaySettingsFunding.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                    NameEn = dimFieldDisplaySettingsFunding.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                    NameSv = dimFieldDisplaySettingsFunding.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                }
            };


            // Loop fundings
            foreach (ProfileEditorFundingToAdd fundingToAdd in profileEditorFundingsToAdd)
            {
                var fundingProcessed = false;
                // Check if userprofile already includes given funding
                foreach (FactFieldValue ffv in dimUserProfile.FactFieldValues.Where(ffv => ffv.DimFundingDecisionId != -1))
                {
                    if (ffv.DimFundingDecision.Id == fundingToAdd.Id)
                    {
                        // Funding is already in profile
                        profileEditorAddFundingResponse.fundingsAlreadyInProfile.Add(fundingToAdd.Id);
                        fundingProcessed = true;
                        break;
                    }
                }

                if (!fundingProcessed)
                {
                    // Get DimPublication
                    var dimFunding = await _ttvContext.DimFundingDecisions.AsNoTracking().FirstOrDefaultAsync(dp => dp.Id == fundingToAdd.Id);
                    // Check if DimPublication exists
                    if (dimFunding == null)
                    {
                        // Funding does not exist
                        profileEditorAddFundingResponse.fundingsNotFound.Add(fundingToAdd.Id);
                    }
                    else
                    {
                        // Add FactFieldValue
                        var factFieldValueFunding = _userProfileService.GetEmptyFactFieldValue();
                        factFieldValueFunding.Show = fundingToAdd.Show != null ? fundingToAdd.Show : false;
                        factFieldValueFunding.PrimaryValue = fundingToAdd.PrimaryValue != null ? fundingToAdd.PrimaryValue : false;
                        factFieldValueFunding.DimUserProfileId = dimUserProfile.Id;
                        factFieldValueFunding.DimFieldDisplaySettingsId = dimFieldDisplaySettingsFunding.Id;
                        factFieldValueFunding.DimPublicationId = dimFunding.Id;
                        factFieldValueFunding.SourceId = Constants.SourceIdentifiers.TIEDEJATUTKIMUS;
                        factFieldValueFunding.Created = _utilityService.getCurrentDateTime();
                        factFieldValueFunding.Modified = _utilityService.getCurrentDateTime();
                        _ttvContext.FactFieldValues.Add(factFieldValueFunding);
                        await _ttvContext.SaveChangesAsync();

                        // Response data
                        var fundingItem = new ProfileEditorItemFunding()
                        {
                            Id = dimFunding.Id,
                            itemMeta = new ProfileEditorItemMeta()
                            {
                                Id = dimFunding.Id,
                                Type = Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION,
                                Show = factFieldValueFunding.Show,
                                PrimaryValue = factFieldValueFunding.PrimaryValue
                            }
                        };

                        profileEditorAddFundingResponse.fundingsAdded.Add(fundingItem);
                    }
                }
            }

            // TODO: add Elasticsearch sync?

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);

            return Ok(new ApiResponseFundingPostMany(success: true, reason:"", data: profileEditorAddFundingResponse, fromCache: false));
        }

        /// <summary>
        /// Remove publicaton(s) from user profile.
        /// </summary>
        [HttpPost]
        [Route("remove")]
        [ProducesResponseType(typeof(ApiResponsePublicationRemoveMany), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveMany([FromBody] List<string> publicationIds)
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
            var profileEditorRemovePublicationResponse = new ProfileEditorRemovePublicationResponse();

            // Remove FactFieldValues
            foreach(string publicationId in publicationIds.Distinct())
            {
                var factFieldValue = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId && ffv.DimPublicationId != -1 && ffv.DimPublication.PublicationId == publicationId)
                  .Include(ffv => ffv.DimPublication).AsNoTracking().FirstOrDefaultAsync();

                if (factFieldValue != null)
                {
                    profileEditorRemovePublicationResponse.publicationsRemoved.Add(publicationId);
                    _ttvContext.FactFieldValues.Remove(factFieldValue);
                }
                else
                {
                    profileEditorRemovePublicationResponse.publicationsNotFound.Add(publicationId);
                }
            }
            await _ttvContext.SaveChangesAsync();

            // TODO: add Elasticsearch sync?

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);

            return Ok(new ApiResponsePublicationRemoveMany(success: true, reason: "removed", data: profileEditorRemovePublicationResponse, fromCache: false));
        }
    }
}