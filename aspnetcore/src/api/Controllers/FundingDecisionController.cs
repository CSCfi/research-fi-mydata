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

        public FundingDecisionController(TtvContext ttvContext, UserProfileService userProfileService, UtilityService utilityService, LanguageService languageService, IMemoryCache memoryCache, ILogger<UserProfileController> logger)
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
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                        .ThenInclude(dfd => dfd.DimCallProgramme).AsNoTracking()
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimFundingDecisionId != -1))
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                        .ThenInclude(dfd => dfd.DimOrganizationIdFunderNavigation).AsNoTracking() // DimFundingDecision related DimOrganization (funder organization)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimFundingDecisionId != -1))
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                        .ThenInclude(dfd => dfd.DimDateIdStartNavigation).AsNoTracking() // DimFundingDecision related start date (DimDate)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimFundingDecisionId != -1))
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                        .ThenInclude(dfd => dfd.DimDateIdEndNavigation).AsNoTracking() // DimFundingDecision related end date (DimDate)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimFundingDecisionId != -1))
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                        .ThenInclude(dfd => dfd.DimTypeOfFunding).AsNoTracking() // DimFundingDecision related DimTypeOfFunding
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimFundingDecisionId != -1))
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                        .ThenInclude(dfd => dfd.DimCallProgramme).AsNoTracking().FirstOrDefaultAsync(); // DimFundingDecision related DimCallProgramme

            // TODO: Currently all added funding decisions get the same data source (Tiedejatutkimus.fi)

            // Get Tiedejatutkimus.fi registered data source id
            var tiedejatutkimusRegisteredDataSourceId = await _userProfileService.GetTiedejatutkimusFiRegisteredDataSourceId();
            // Get DimFieldDisplaySetting for Tiedejatutkimus.fi
            var dimFieldDisplaySettingsFundingDecision = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION && dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSourceId == tiedejatutkimusRegisteredDataSourceId);

            // Response object
            var profileEditorAddFundingDecisionResponse = new ProfileEditorAddFundingDecisionResponse();
            profileEditorAddFundingDecisionResponse.source = new ProfileEditorSource()
            {
                Id = dimFieldDisplaySettingsFundingDecision.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                RegisteredDataSource = dimFieldDisplaySettingsFundingDecision.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                Organization = new Organization()
                {
                    NameFi = dimFieldDisplaySettingsFundingDecision.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                    NameEn = dimFieldDisplaySettingsFundingDecision.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                    NameSv = dimFieldDisplaySettingsFundingDecision.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
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

                        // Response data
                        // Name translation service ensures that none of the language fields is empty.
                        var nameTraslationFundingDecision_ProjectName = _languageService.getNameTranslation(
                            nameFi: dimFundingDecision.NameFi,
                            nameSv: dimFundingDecision.NameSv,
                            nameEn: dimFundingDecision.NameEn
                        );
                        var nameTraslationFundingDecision_ProjectDescription = _languageService.getNameTranslation(
                            nameFi: dimFundingDecision.DescriptionFi,
                            nameSv: dimFundingDecision.DescriptionSv,
                            nameEn: dimFundingDecision.DescriptionEn
                        );
                        var nameTraslationFundingDecision_FunderName = _languageService.getNameTranslation(
                            nameFi: dimFundingDecision.DimOrganizationIdFunderNavigation.NameFi,
                            nameSv: dimFundingDecision.DimOrganizationIdFunderNavigation.NameSv,
                            nameEn: dimFundingDecision.DimOrganizationIdFunderNavigation.NameEn
                        );
                        var nameTranslationFundingDecision_TypeOfFunding = _languageService.getNameTranslation(
                            nameFi: dimFundingDecision.DimTypeOfFunding.NameFi,
                            nameSv: dimFundingDecision.DimTypeOfFunding.NameSv,
                            nameEn: dimFundingDecision.DimTypeOfFunding.NameEn
                        );
                        var nameTranslationFundingDecision_CallProgramme = _languageService.getNameTranslation(
                            nameFi: dimFundingDecision.DimCallProgramme.NameFi,
                            nameSv: dimFundingDecision.DimCallProgramme.NameSv,
                            nameEn: dimFundingDecision.DimCallProgramme.NameEn
                        );

                        var fundingItem = new ProfileEditorItemFundingDecision()
                        {
                            FunderProjectNumber = dimFundingDecision.FunderProjectNumber,
                            ProjectAcronym = dimFundingDecision.Acronym,
                            ProjectNameFi = nameTraslationFundingDecision_ProjectName.NameFi,
                            ProjectNameSv = nameTraslationFundingDecision_ProjectName.NameSv,
                            ProjectNameEn = nameTraslationFundingDecision_ProjectName.NameEn,
                            ProjectDescriptionFi = nameTraslationFundingDecision_ProjectDescription.NameFi,
                            ProjectDescriptionSv = nameTraslationFundingDecision_ProjectDescription.NameSv,
                            ProjectDescriptionEn = nameTraslationFundingDecision_ProjectDescription.NameEn,
                            FunderNameFi = nameTraslationFundingDecision_FunderName.NameFi,
                            FunderNameSv = nameTraslationFundingDecision_FunderName.NameSv,
                            FunderNameEn = nameTraslationFundingDecision_FunderName.NameEn,
                            TypeOfFundingNameFi = nameTranslationFundingDecision_TypeOfFunding.NameFi,
                            TypeOfFundingNameSv = nameTranslationFundingDecision_TypeOfFunding.NameSv,
                            TypeOfFundingNameEn = nameTranslationFundingDecision_TypeOfFunding.NameEn,
                            CallProgrammeNameFi = nameTranslationFundingDecision_CallProgramme.NameFi,
                            CallProgrammeNameSv = nameTranslationFundingDecision_CallProgramme.NameSv,
                            CallProgrammeNameEn = nameTranslationFundingDecision_CallProgramme.NameEn,
                            AmountInEur = dimFundingDecision.AmountInEur,
                            itemMeta = new ProfileEditorItemMeta()
                            {
                                Id = dimFundingDecision.Id,
                                Type = Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION,
                                Show = factFieldValueFunding.Show,
                                PrimaryValue = factFieldValueFunding.PrimaryValue
                            }
                        };

                        profileEditorAddFundingDecisionResponse.fundingDecisionsAdded.Add(fundingItem);
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
                var factFieldValue = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId && ffv.DimFundingDecisionId != -1 && ffv.DimFundingDecision.FunderProjectNumber == funderProjectNumber)
                  .Include(ffv => ffv.DimFundingDecision).AsNoTracking().FirstOrDefaultAsync();

                if (factFieldValue != null)
                {
                    profileEditorRemoveFundingDecisionResponse.fundingDecisionsRemoved.Add(funderProjectNumber);
                    _ttvContext.FactFieldValues.Remove(factFieldValue);
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