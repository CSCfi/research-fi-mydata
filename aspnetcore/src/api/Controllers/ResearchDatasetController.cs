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
     * ResearchDatasetController implements profile editor API commands for adding and deleting profile's research dataset.
     */
    [Route("api/researchdataset")]
    [ApiController]
    [Authorize]
    public class ResearchDatasetController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;
        private readonly UtilityService _utilityService;
        private readonly LanguageService _languageService;
        private IMemoryCache _cache;
        private readonly ILogger<UserProfileController> _logger;

        public ResearchDatasetController(TtvContext ttvContext, UserProfileService userProfileService, UtilityService utilityService, IMemoryCache memoryCache, ILogger<UserProfileController> logger, LanguageService languageService)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _utilityService = utilityService;
            _languageService = languageService;
            _logger = logger;
            _cache = memoryCache;
        }

        /// <summary>
        /// Add research dataset to user profile.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseResearchDatasetPostMany), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostMany([FromBody] List<ProfileEditorResearchDatasetToAdd> profileEditorResearchdatasetToAdd)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse(success: false, reason: "invalid request data"));
            }

            // Return immediately if there is nothing to add
            if (profileEditorResearchdatasetToAdd.Count == 0)
            {
                return Ok(new ApiResponse(success: false, reason: "nothing to add"));
            }

            // Get userprofile
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                // Userprofile not found
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // TODO: FactFieldValues relation to DimResearchDataset
            var dimUserProfile = await _ttvContext.DimUserProfiles.Where(dup => dup.Id == userprofileId)
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.BrFieldDisplaySettingsDimRegisteredDataSources)
                        .ThenInclude(br => br.DimRegisteredDataSource)
                            .ThenInclude(drds => drds.DimOrganization).AsNoTracking()
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimResearchDatasetId != -1))
                    .ThenInclude(ffv => ffv.DimResearchDataset).AsNoTracking().FirstOrDefaultAsync();

            // TODO: Currently all added research data get the same data source (Tiedejatutkimus.fi)

            // Get Tiedejatutkimus.fi registered data source id
            var tiedejatutkimusRegisteredDataSourceId = await _userProfileService.GetTiedejatutkimusFiRegisteredDataSourceId();
            // Get DimFieldDisplaySetting for Tiedejatutkimus.fi
            var dimFieldDisplaySettingsResearchDataset = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET && dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSourceId == tiedejatutkimusRegisteredDataSourceId);

            // Organization name translation
            var nameTranslation_OrganizationName = _languageService.getNameTranslation(
                nameFi: dimFieldDisplaySettingsResearchDataset.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                nameSv: dimFieldDisplaySettingsResearchDataset.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv,
                nameEn: dimFieldDisplaySettingsResearchDataset.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn
            );

            // Response object
            var profileEditorAddResearchDatasetResponse = new ProfileEditorAddResearchDatasetResponse();
            profileEditorAddResearchDatasetResponse.source = new ProfileEditorSource()
            {
                Id = dimFieldDisplaySettingsResearchDataset.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                RegisteredDataSource = dimFieldDisplaySettingsResearchDataset.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                Organization = new Organization()
                {
                    NameFi = nameTranslation_OrganizationName.NameFi,
                    NameSv = nameTranslation_OrganizationName.NameSv,
                    NameEn = nameTranslation_OrganizationName.NameEn
                }
            };

            // Loop data sets
            foreach (ProfileEditorResearchDatasetToAdd researchDatasetToAdd in profileEditorResearchdatasetToAdd)
            {
                var researchDatasetProcessed = false;
                // Check if userprofile already includes given research dataset
                foreach (FactFieldValue ffv in dimUserProfile.FactFieldValues)
                {
                    if (ffv.DimResearchDataset.LocalIdentifier == researchDatasetToAdd.LocalIdentifier)
                    {
                        // Research dataset is already in profile
                        profileEditorAddResearchDatasetResponse.researchDatasetAlreadyInProfile.Add(researchDatasetToAdd.LocalIdentifier);
                        researchDatasetProcessed = true;
                        break;
                    }
                }

                if (!researchDatasetProcessed)
                {
                    // Get DimResearchDataset
                    var dimResearchDataset = await _ttvContext.DimResearchDatasets.AsNoTracking().FirstOrDefaultAsync(drd => drd.LocalIdentifier == researchDatasetToAdd.LocalIdentifier);
                    // Check if DimResearchDataset exists
                    if (dimResearchDataset == null)
                    {
                        // DimResearchDataset does not exist
                        profileEditorAddResearchDatasetResponse.researchDatasetNotFound.Add(researchDatasetToAdd.LocalIdentifier);
                    }
                    else
                    {
                        // Add FactFieldValue
                        var factFieldValueResearchDataset = _userProfileService.GetEmptyFactFieldValue();
                        factFieldValueResearchDataset.Show = researchDatasetToAdd.Show != null ? researchDatasetToAdd.Show : false;
                        factFieldValueResearchDataset.PrimaryValue = researchDatasetToAdd.PrimaryValue != null ? researchDatasetToAdd.PrimaryValue : false;
                        factFieldValueResearchDataset.DimUserProfileId = dimUserProfile.Id;
                        factFieldValueResearchDataset.DimFieldDisplaySettingsId = dimFieldDisplaySettingsResearchDataset.Id;
                        factFieldValueResearchDataset.DimResearchDatasetId = dimResearchDataset.Id;
                        factFieldValueResearchDataset.SourceId = Constants.SourceIdentifiers.TIEDEJATUTKIMUS;
                        factFieldValueResearchDataset.Created = _utilityService.getCurrentDateTime();
                        factFieldValueResearchDataset.Modified = _utilityService.getCurrentDateTime();
                        _ttvContext.FactFieldValues.Add(factFieldValueResearchDataset);
                        await _ttvContext.SaveChangesAsync();

                        profileEditorAddResearchDatasetResponse.researchDatasetAdded.Add(researchDatasetToAdd.LocalIdentifier);
                    }
                }
            }

            // TODO: add Elasticsearch sync?

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);

            return Ok(new ApiResponseResearchDatasetPostMany(success: true, reason:"", data: profileEditorAddResearchDatasetResponse, fromCache: false));
        }

        /// <summary>
        /// Remove research dataset(s) from user profile.
        /// </summary>
        [HttpPost]
        [Route("remove")]
        [ProducesResponseType(typeof(ApiResponseResearchDatasetRemoveMany), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveMany([FromBody] List<string> localIdentifiers)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse(success: false, reason: "invalid request data"));
            }

            // Return immediately if there is nothing to remove
            if (localIdentifiers.Count == 0)
            {
                return Ok(new ApiResponse(success: false, reason: "nothing to remove"));
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
            var profileEditorRemoveResearchDatasetResponse = new ProfileEditorRemoveResearchDatasetResponse();

            // Remove FactFieldValues
            foreach(string localIdentifier in localIdentifiers.Distinct())
            {
                var factFieldValue = await _ttvContext.FactFieldValues
                    .Where(
                        ffv => ffv.DimUserProfileId == userprofileId &&
                        ffv.DimResearchDatasetId != -1 &&
                        ffv.DimResearchDataset.LocalIdentifier == localIdentifier
                    ).FirstOrDefaultAsync();

                if (factFieldValue != null)
                {
                    _ttvContext.FactFieldValues.Remove(factFieldValue);
                    profileEditorRemoveResearchDatasetResponse.researchDatasetsRemoved.Add(localIdentifier);
                }
                else
                {
                    profileEditorRemoveResearchDatasetResponse.researchDatasetsNotFound.Add(localIdentifier);
                }
            }
            await _ttvContext.SaveChangesAsync();

            // TODO: add Elasticsearch sync?

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);

            return Ok(new ApiResponseResearchDatasetRemoveMany(success: true, reason: "removed", data: profileEditorRemoveResearchDatasetResponse, fromCache: false));
        }
    }
}