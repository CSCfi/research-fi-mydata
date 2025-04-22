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
using System.Security.AccessControl;

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

        public PublicationController(TtvContext ttvContext, IUserProfileService userProfileService,
            IUtilityService utilityService, IDataSourceHelperService dataSourceHelperService,
            IMemoryCache memoryCache, ILanguageService languageService,
            ILogger<UserProfileController> logger)
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
        /// Add publicaton(s) to user profile.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponsePublicationPostMany), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostMany([FromBody] List<ProfileEditorPublicationToAdd> profileEditorPublicationsToAdd)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.INVALID_REQUEST));
            }

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                this.GetLogUserIdentification(),
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_MODIFY_PUBLICATION_ADD,
                    state: LogContent.ActionState.START,
                    message: $"IDs: {string.Join(", ", profileEditorPublicationsToAdd.Select(p => p.PublicationId))}"));

            // Return immediately if there is nothing to add
            if (profileEditorPublicationsToAdd.Count == 0)
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    this.GetLogUserIdentification(),
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_MODIFY_PUBLICATION_ADD,
                        state: LogContent.ActionState.COMPLETE));
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

            DimUserProfile dimUserProfile = await _ttvContext.DimUserProfiles
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimRegisteredDataSource)
                            .ThenInclude(drds => drds.DimOrganization).AsNoTracking()
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPublication)
                        .ThenInclude(publication => publication.DimPids.Where(pid => pid.PidType == "doi")).AsNoTracking().FirstOrDefaultAsync(dup => dup.Id == userprofileId);

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
                    ProfileEditorPublication profileEditorPublication = await _ttvContext.DimPublications.Select(dp => new ProfileEditorPublication()
                    {
                        PublicationId = dp.PublicationId,
                        PublicationName = dp.PublicationName,
                        PublicationYear = dp.PublicationYear,
                        Doi = dp.DimPids.Select(pid => pid.PidContent).FirstOrDefault(),
                        TypeCode = dp.PublicationTypeCodeNavigation.CodeValue,
                        AuthorsText = dp.AuthorsText,
                        JournalName = dp.JournalName,
                        ConferenceName = dp.ConferenceName,
                        ParentPublicationName = dp.ParentPublicationName,
                        DataSources = new List<ProfileEditorSource> { dataSource },
                        itemMeta = new ProfileEditorItemMeta(
                            dp.Id,
                            Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                            publicationToAdd.Show != null ? publicationToAdd.Show : false,
                            publicationToAdd.PrimaryValue != null ? publicationToAdd.PrimaryValue : false
                        )
                    }).AsNoTracking().FirstOrDefaultAsync(dp => dp.PublicationId == publicationToAdd.PublicationId);
                    
                    // Check if DimPublication exists
                    if (profileEditorPublication == null)
                    {
                        // Publication does not exist
                        profileEditorAddPublicationResponse.publicationsNotFound.Add(publicationToAdd.PublicationId);
                    }
                    else
                    {
                        // Add FactFieldValue
                        FactFieldValue factFieldValuePublication = _userProfileService.GetEmptyFactFieldValue();
                        factFieldValuePublication.Show = profileEditorPublication.itemMeta.Show;
                        factFieldValuePublication.PrimaryValue = profileEditorPublication.itemMeta.PrimaryValue;
                        factFieldValuePublication.DimUserProfileId = dimUserProfile.Id;
                        factFieldValuePublication.DimFieldDisplaySettingsId = dimFieldDisplaySettingsPublication.Id;
                        factFieldValuePublication.DimPublicationId = (int)profileEditorPublication.itemMeta.Id;
                        factFieldValuePublication.DimRegisteredDataSourceId = _dataSourceHelperService.DimRegisteredDataSourceId_TTV;
                        factFieldValuePublication.SourceId = Constants.SourceIdentifiers.TIEDEJATUTKIMUS;
                        factFieldValuePublication.Created = _utilityService.GetCurrentDateTime();
                        factFieldValuePublication.Modified = _utilityService.GetCurrentDateTime();
                        _ttvContext.FactFieldValues.Add(factFieldValuePublication);
                        await _ttvContext.SaveChangesAsync();

                        profileEditorAddPublicationResponse.publicationsAdded.Add(profileEditorPublication);
                    }
                }
            }

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                this.GetLogUserIdentification(),
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_MODIFY_PUBLICATION_ADD,
                    state: LogContent.ActionState.COMPLETE,
                    message: $"IDs: {string.Join(", ", profileEditorAddPublicationResponse.publicationsAdded.Select(p => p.PublicationId))}"));

            // Refresh 'modified' timestamp in user profile
            await _userProfileService.SetModifiedTimestampInUserProfile(userprofileId);

            // Update Elasticsearch index.
            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();
            await _userProfileService.UpdateProfileInElasticsearch(
                orcidId: orcidId,
                userprofileId: dimUserProfile.Id,
                logUserIdentification: logUserIdentification);

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
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.INVALID_REQUEST));
            }

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                this.GetLogUserIdentification(),
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_MODIFY_PUBLICATION_DELETE,
                    state: LogContent.ActionState.START,
                    message: $"IDs: {string.Join(", ", publicationIds)}"));

            // Return immediately if there is nothing to remove
            if (publicationIds.Count == 0)
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    this.GetLogUserIdentification(),
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_MODIFY_PUBLICATION_DELETE,
                        state: LogContent.ActionState.COMPLETE
                    ));
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

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                this.GetLogUserIdentification(),
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_MODIFY_PUBLICATION_DELETE,
                    state: LogContent.ActionState.COMPLETE,
                    message: $"IDs: {string.Join(", ", profileEditorRemovePublicationResponse.publicationsRemoved)}"
                ));

            // Refresh 'modified' timestamp in user profile
            await _userProfileService.SetModifiedTimestampInUserProfile(userprofileId);

            // Update Elasticsearch index.
            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();
            await _userProfileService.UpdateProfileInElasticsearch(
                orcidId: orcidId,
                userprofileId: userprofileId,
                logUserIdentification: logUserIdentification);

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);

            return Ok(new ApiResponsePublicationRemoveMany(success: true, reason: "removed", data: profileEditorRemovePublicationResponse, fromCache: false));
        }
    }
}