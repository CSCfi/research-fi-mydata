using api.Services;
using api.Models.Api;
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
using Microsoft.AspNetCore.Http;
using api.Models.Common;
using Microsoft.Extensions.Logging;
using System;

namespace api.Controllers
{
    /*
     * ResearchDatasetController implements profile editor API commands for adding and deleting profile's research dataset.
     */
    [Route("api/researchdataset")]
    [ApiController]
    [Authorize(Policy = "RequireScopeApi1AndClaimOrcid")]
    public class ResearchDatasetController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly IUserProfileService _userProfileService;
        private readonly IUtilityService _utilityService;
        private readonly IDataSourceHelperService _dataSourceHelperService;
        private readonly ILanguageService _languageService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<UserProfileController> _logger;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly IBackgroundProfiledata _backgroundProfiledata;
        private readonly IBackgroundTaskQueue _taskQueue;

        public ResearchDatasetController(TtvContext ttvContext, IUserProfileService userProfileService,
            IUtilityService utilityService, IDataSourceHelperService dataSourceHelperService,
            IMemoryCache memoryCache, ILanguageService languageService,
            IElasticsearchService elasticsearchService,
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
            _elasticsearchService = elasticsearchService;
            _backgroundProfiledata = backgroundProfiledata;
            _logger = logger;
            _taskQueue = taskQueue;
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

            // Get ORCID id
            string orcidId = GetOrcidId();

            // Check that userprofile exists.
            if (!await _userProfileService.UserprofileExistsForOrcidId(orcidId: orcidId))
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Get userprofile id
            int userprofileId = await _userProfileService.GetUserprofileId(orcidId);

            // TODO: FactFieldValues relation to DimResearchDataset
            DimUserProfile dimUserProfile = await _ttvContext.DimUserProfiles.Where(dup => dup.Id == userprofileId)
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimRegisteredDataSource)
                            .ThenInclude(drds => drds.DimOrganization).AsNoTracking()
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimResearchDatasetId != -1))
                    .ThenInclude(ffv => ffv.DimResearchDataset).AsNoTracking().FirstOrDefaultAsync();

            // TODO: Currently all added research data get the same data source (Tiedejatutkimus.fi)

            // Get DimFieldDisplaySetting for Tiedejatutkimus.fi
            DimFieldDisplaySetting dimFieldDisplaySettingsResearchDataset =
                dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET);

            // Registered data source organization name translation
            NameTranslation nameTranslation_OrganizationName = _languageService.GetNameTranslation(
                nameFi: _dataSourceHelperService.DimOrganizationNameFi_TTV,
                nameSv: _dataSourceHelperService.DimOrganizationNameSv_TTV,
                nameEn: _dataSourceHelperService.DimOrganizationNameEn_TTV
            );

            // Response object
            ProfileEditorAddResearchDatasetResponse profileEditorAddResearchDatasetResponse = new()
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

            // Loop data sets, ignore possible duplicates
            foreach (ProfileEditorResearchDatasetToAdd researchDatasetToAdd in profileEditorResearchdatasetToAdd.DistinctBy(r => r.LocalIdentifier))
            {
                bool researchDatasetProcessed = false;
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
                    DimResearchDataset dimResearchDataset =
                        await _ttvContext.DimResearchDatasets.AsNoTracking().FirstOrDefaultAsync(drd => drd.LocalIdentifier == researchDatasetToAdd.LocalIdentifier);
                    // Check if DimResearchDataset exists
                    if (dimResearchDataset == null)
                    {
                        // DimResearchDataset does not exist
                        profileEditorAddResearchDatasetResponse.researchDatasetNotFound.Add(researchDatasetToAdd.LocalIdentifier);
                    }
                    else
                    {
                        // Add FactFieldValue
                        FactFieldValue factFieldValueResearchDataset = _userProfileService.GetEmptyFactFieldValue();
                        factFieldValueResearchDataset.Show = researchDatasetToAdd.Show != null ? researchDatasetToAdd.Show : false;
                        factFieldValueResearchDataset.PrimaryValue = researchDatasetToAdd.PrimaryValue != null ? researchDatasetToAdd.PrimaryValue : false;
                        factFieldValueResearchDataset.DimUserProfileId = dimUserProfile.Id;
                        factFieldValueResearchDataset.DimFieldDisplaySettingsId = dimFieldDisplaySettingsResearchDataset.Id;
                        factFieldValueResearchDataset.DimResearchDatasetId = dimResearchDataset.Id;
                        factFieldValueResearchDataset.DimRegisteredDataSourceId = _dataSourceHelperService.DimRegisteredDataSourceId_TTV;
                        factFieldValueResearchDataset.SourceId = Constants.SourceIdentifiers.TIEDEJATUTKIMUS;
                        factFieldValueResearchDataset.Created = _utilityService.GetCurrentDateTime();
                        factFieldValueResearchDataset.Modified = _utilityService.GetCurrentDateTime();
                        _ttvContext.FactFieldValues.Add(factFieldValueResearchDataset);
                        await _ttvContext.SaveChangesAsync();

                        profileEditorAddResearchDatasetResponse.researchDatasetAdded.Add(researchDatasetToAdd.LocalIdentifier);
                    }
                }
            }

            // Update Elasticsearch index in a background task.
            if (_elasticsearchService.IsElasticsearchSyncEnabled())
            {
                await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
                {
                    _logger.LogInformation($"Elasticsearch index update for {orcidId} started from ResearchDatasetController {DateTime.UtcNow}");
                    // Get Elasticsearch person entry from profile data.
                    Models.Elasticsearch.ElasticsearchPerson person = await _backgroundProfiledata.GetProfiledataForElasticsearch(orcidId, userprofileId);
                    // Update Elasticsearch person index.
                    await _elasticsearchService.UpdateEntryInElasticsearchPersonIndex(orcidId, person);
                    _logger.LogInformation($"Elasticsearch index update for {orcidId} from ResearchDatasetController completed {DateTime.UtcNow}");
                });
            }

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

            // Get ORCID id
            string orcidId = GetOrcidId();

            // Check that userprofile exists.
            if (!await _userProfileService.UserprofileExistsForOrcidId(orcidId: orcidId))
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Get userprofile id
            int userprofileId = await _userProfileService.GetUserprofileId(orcidId);

            // Response object
            ProfileEditorRemoveResearchDatasetResponse profileEditorRemoveResearchDatasetResponse = new();

            // Remove FactFieldValues
            foreach(string localIdentifier in localIdentifiers.Distinct())
            {
                FactFieldValue factFieldValue = await _ttvContext.FactFieldValues
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

            // Update Elasticsearch index in a background task.
            if (_elasticsearchService.IsElasticsearchSyncEnabled())
            {
                await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
                {
                    _logger.LogInformation($"Elasticsearch index update for {orcidId} started from ResearchDatasetController {DateTime.UtcNow}");
                    // Get Elasticsearch person entry from profile data.
                    Models.Elasticsearch.ElasticsearchPerson person = await _backgroundProfiledata.GetProfiledataForElasticsearch(orcidId, userprofileId);
                    // Update Elasticsearch person index.
                    await _elasticsearchService.UpdateEntryInElasticsearchPersonIndex(orcidId, person);
                    _logger.LogInformation($"Elasticsearch index update for {orcidId} from ResearchDatasetController completed {DateTime.UtcNow}");
                });
            }

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);

            return Ok(new ApiResponseResearchDatasetRemoveMany(success: true, reason: "removed", data: profileEditorRemoveResearchDatasetResponse, fromCache: false));
        }
    }
}