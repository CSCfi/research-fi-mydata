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
     * ResearchdataController implements profile editor API commands for adding and deleting profile's research data.
     */
    [Route("api/publication")]
    [ApiController]
    [Authorize]
    public class ResearchdataController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;
        private readonly UtilityService _utilityService;
        private IMemoryCache _cache;
        private readonly ILogger<UserProfileController> _logger;

        public ResearchdataController(TtvContext ttvContext, UserProfileService userProfileService, UtilityService utilityService, IMemoryCache memoryCache, ILogger<UserProfileController> logger)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _utilityService = utilityService;
            _logger = logger;
            _cache = memoryCache;
        }

        /// <summary>
        /// Add research data to user profile.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseResearchdatasetPostMany), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostMany([FromBody] List<ProfileEditorResearchdatasetToAdd> profileEditorResearchdatasetToAdd)
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

            // TODO: FactFieldValues relation to DimResearhDataset
            var dimUserProfile = await _ttvContext.DimUserProfiles
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.BrFieldDisplaySettingsDimRegisteredDataSources)
                        .ThenInclude(br => br.DimRegisteredDataSource)
                            .ThenInclude(drds => drds.DimOrganization).AsNoTracking()
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPublication).AsNoTracking().FirstOrDefaultAsync(dup => dup.Id == userprofileId);

            // TODO: Currently all added publications get the same data source (Tiedejatutkimus.fi)

            // Get Tiedejatutkimus.fi registered data source id
            var tiedejatutkimusRegisteredDataSourceId = await _userProfileService.GetTiedejatutkimusFiRegisteredDataSourceId();
            // Get DimFieldDisplaySetting for Tiedejatutkimus.fi
            var dimFieldDisplaySettingsResearchdataset = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET && dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSourceId == tiedejatutkimusRegisteredDataSourceId);

            // Response object
            var profileEditorAddResearchdatasetResponse = new ProfileEditorAddResearchdatasetResponse();
            profileEditorAddResearchdatasetResponse.source = new ProfileEditorSource()
            {
                Id = dimFieldDisplaySettingsResearchdataset.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                RegisteredDataSource = dimFieldDisplaySettingsResearchdataset.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                Organization = new Organization()
                {
                    NameFi = dimFieldDisplaySettingsResearchdataset.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                    NameEn = dimFieldDisplaySettingsResearchdataset.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                    NameSv = dimFieldDisplaySettingsResearchdataset.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                }
            };


            // Loop publications
            foreach (ProfileEditorResearchdatasetToAdd researchdatasetToAdd in profileEditorResearchdatasetToAdd)
            {
                var researchdatasetProcessed = false;
                //// Check if userprofile already includes given research dataset
                //foreach (FactFieldValue ffv in dimUserProfile.Where(ffv => ffv.DimPublicationId != -1))
                //{
                //    if (ffv.DimPublication.PublicationId == publicationToAdd.PublicationId)
                //    {
                //        // Publication is already in profile
                //        profileEditorAddPublicationResponse.publicationsAlreadyInProfile.Add(publicationToAdd.PublicationId);
                //        publicationProcessed = true;
                //        break;
                //    }
                //}

                if (!researchdatasetProcessed)
                {
                    // Get DimResearchDataset
                    var dimResearchdataset = await _ttvContext.DimResearchDatasets.AsNoTracking().FirstOrDefaultAsync(dp => dp.LocalIdentifier == researchdatasetToAdd.LocalIdentifier);
                    // Check if DimResearchDataset exists
                    if (dimResearchdataset == null)
                    {
                        // DimResearchDataset does not exist
                        profileEditorAddResearchdatasetResponse.researchdatasetNotFound.Add(researchdatasetToAdd.LocalIdentifier);
                    }
                    else
                    {
                        // TODO
                        // Add FactFieldValue
                        var factFieldValuePublication = _userProfileService.GetEmptyFactFieldValue();
                        factFieldValuePublication.Show = researchdatasetToAdd.Show != null ? researchdatasetToAdd.Show : false;
                        factFieldValuePublication.PrimaryValue = researchdatasetToAdd.PrimaryValue != null ? researchdatasetToAdd.PrimaryValue : false;
                        factFieldValuePublication.DimUserProfileId = dimUserProfile.Id;
                        factFieldValuePublication.DimFieldDisplaySettingsId = dimFieldDisplaySettingsResearchdataset.Id;
                        //factFieldValuePublication.DimPublicationId = dimPublication.Id;
                        factFieldValuePublication.SourceId = Constants.SourceIdentifiers.TIEDEJATUTKIMUS;
                        factFieldValuePublication.Created = _utilityService.getCurrentDateTime();
                        factFieldValuePublication.Modified = _utilityService.getCurrentDateTime();
                        _ttvContext.FactFieldValues.Add(factFieldValuePublication);
                        await _ttvContext.SaveChangesAsync();

                        // Response data
                        var researchdatasetItem = new ProfileEditorItemResearchdataset()
                        {
                            LocalIdentifier = dimResearchdataset.LocalIdentifier,
                            NameFi = dimResearchdataset.NameFi,
                            NameSv = dimResearchdataset.NameSv,
                            NameEn = dimResearchdataset.NameEn,
                            itemMeta = new ProfileEditorItemMeta()
                            {
                                Id = dimResearchdataset.Id,
                                Type = Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET,
                                Show = factFieldValuePublication.Show,
                                PrimaryValue = factFieldValuePublication.PrimaryValue
                            }
                        };

                        profileEditorAddResearchdatasetResponse.researchdatasetAdded.Add(researchdatasetItem);
                    }
                }
            }

            // TODO: add Elasticsearch sync?

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);

            return Ok(new ApiResponseResearchdatasetPostMany(success: true, reason:"", data: profileEditorAddResearchdatasetResponse, fromCache: false));
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