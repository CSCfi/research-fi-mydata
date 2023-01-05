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
using Microsoft.AspNetCore.Http;
using api.Models.Common;
using Microsoft.Extensions.Logging;
using System;

namespace api.Controllers
{
    /*
     * PublicationController implements profile editor API commands for adding and deleting profile's publications.
     */
    [Route("api/publication")]
    [ApiController]
    [Authorize(Policy = "RequireScopeApi1AndClaimOrcid")]
    public class PublicationController : TtvControllerBase
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

        public PublicationController(TtvContext ttvContext, IUserProfileService userProfileService,
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
        /// Add publicaton(s) to user profile.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponsePublicationPostMany), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostMany([FromBody] List<ProfileEditorPublicationToAdd> profileEditorPublicationsToAdd)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse(success: false, reason: "invalid request data"));
            }

            // Return immediately if there is nothing to add
            if (profileEditorPublicationsToAdd.Count == 0)
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

            DimUserProfile dimUserProfile = await _ttvContext.DimUserProfiles
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimRegisteredDataSource)
                            .ThenInclude(drds => drds.DimOrganization).AsNoTracking()
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPublication).AsNoTracking().FirstOrDefaultAsync(dup => dup.Id == userprofileId);

            // TODO: Currently all added publications get the same data source (Tiedejatutkimus.fi)

            // Get DimFieldDisplaySetting for publication
            DimFieldDisplaySetting dimFieldDisplaySettingsPublication =
                dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION);

            // Registered data source organization name translation
            NameTranslation nameTranslation_OrganizationName = _languageService.GetNameTranslation(
                nameFi: _dataSourceHelperService.DimOrganizationNameFi_TTV,
                nameSv: _dataSourceHelperService.DimOrganizationNameSv_TTV,
                nameEn: _dataSourceHelperService.DimOrganizationNameEn_TTV
            );

            // Data source
            ProfileEditorSource dataSource = new ProfileEditorSource()
            {
                RegisteredDataSource = _dataSourceHelperService.DimRegisteredDataSourceName_TTV,
                Organization = new Organization()
                {
                    NameFi = nameTranslation_OrganizationName.NameFi,
                    NameSv = nameTranslation_OrganizationName.NameSv,
                    NameEn = nameTranslation_OrganizationName.NameEn
                }
            };

            // Response object
            ProfileEditorAddPublicationResponse profileEditorAddPublicationResponse = new()
            {
                source = dataSource
            };


            // Loop publications, ignore possible duplicates
            foreach (ProfileEditorPublicationToAdd publicationToAdd in profileEditorPublicationsToAdd.DistinctBy(p => p.PublicationId))
            {
                bool publicationProcessed = false;
                // Check if userprofile already includes given publication
                foreach (FactFieldValue ffv in dimUserProfile.FactFieldValues.Where(ffv => ffv.DimPublicationId != -1))
                {
                    if (ffv.DimPublication.PublicationId == publicationToAdd.PublicationId)
                    {
                        // Publication is already in profile
                        profileEditorAddPublicationResponse.publicationsAlreadyInProfile.Add(publicationToAdd.PublicationId);
                        publicationProcessed = true;
                        break;
                    }
                }

                if (!publicationProcessed)
                {
                    // Get DimPublication
                    DimPublication dimPublication = await _ttvContext.DimPublications.AsNoTracking().FirstOrDefaultAsync(dp => dp.PublicationId == publicationToAdd.PublicationId);
                    // Check if DimPublication exists
                    if (dimPublication == null)
                    {
                        // Publication does not exist
                        profileEditorAddPublicationResponse.publicationsNotFound.Add(publicationToAdd.PublicationId);
                    }
                    else
                    {
                        // Add FactFieldValue
                        FactFieldValue factFieldValuePublication = _userProfileService.GetEmptyFactFieldValue();
                        factFieldValuePublication.Show = publicationToAdd.Show != null ? publicationToAdd.Show : false;
                        factFieldValuePublication.PrimaryValue = publicationToAdd.PrimaryValue != null ? publicationToAdd.PrimaryValue : false;
                        factFieldValuePublication.DimUserProfileId = dimUserProfile.Id;
                        factFieldValuePublication.DimFieldDisplaySettingsId = dimFieldDisplaySettingsPublication.Id;
                        factFieldValuePublication.DimPublicationId = dimPublication.Id;
                        factFieldValuePublication.DimRegisteredDataSourceId = _dataSourceHelperService.DimRegisteredDataSourceId_TTV;
                        factFieldValuePublication.SourceId = Constants.SourceIdentifiers.TIEDEJATUTKIMUS;
                        factFieldValuePublication.Created = _utilityService.GetCurrentDateTime();
                        factFieldValuePublication.Modified = _utilityService.GetCurrentDateTime();
                        _ttvContext.FactFieldValues.Add(factFieldValuePublication);
                        await _ttvContext.SaveChangesAsync();

                        // Response data
                        ProfileEditorPublication publicationItem = new()
                        {
                            PublicationId = dimPublication.PublicationId,
                            PublicationName = dimPublication.PublicationName,
                            PublicationYear = dimPublication.PublicationYear,
                            Doi = dimPublication.Doi,
                            TypeCode = dimPublication.PublicationTypeCode,
                            itemMeta = new ProfileEditorItemMeta()
                            {
                                Id = dimPublication.Id,
                                Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                                Show = factFieldValuePublication.Show,
                                PrimaryValue = factFieldValuePublication.PrimaryValue
                            },
                            DataSources = new List<ProfileEditorSource>
                            {
                                dataSource
                            }
                        };

                        profileEditorAddPublicationResponse.publicationsAdded.Add(publicationItem);
                    }
                }
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

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);

            return Ok(new ApiResponsePublicationPostMany(success: true, reason:"", data: profileEditorAddPublicationResponse, fromCache: false));
        }

        /// <summary>
        /// Remove publication(s) from user profile.
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

            // Return immediately if there is nothing to remove
            if (publicationIds.Count == 0)
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
            ProfileEditorRemovePublicationResponse profileEditorRemovePublicationResponse = new();

            // Remove FactFieldValues
            foreach(string publicationId in publicationIds.Distinct())
            {
                FactFieldValue factFieldValue = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId && ffv.DimPublicationId != -1 && ffv.DimPublication.PublicationId == publicationId)
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

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);

            return Ok(new ApiResponsePublicationRemoveMany(success: true, reason: "removed", data: profileEditorRemovePublicationResponse, fromCache: false));
        }
    }
}