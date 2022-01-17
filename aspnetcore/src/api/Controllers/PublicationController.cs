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
     * PublicationController implements profile editor API commands for adding and deleting profile's publications.
     */
    [Route("api/publication")]
    [ApiController]
    [Authorize]
    public class PublicationController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;
        private readonly UtilityService _utilityService;
        private readonly LanguageService _languageService;
        private IMemoryCache _cache;
        private readonly ILogger<UserProfileController> _logger;

        public PublicationController(TtvContext ttvContext, UserProfileService userProfileService, UtilityService utilityService, IMemoryCache memoryCache, ILogger<UserProfileController> logger, LanguageService languageService)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _utilityService = utilityService;
            _languageService = languageService;
            _logger = logger;
            _cache = memoryCache;
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
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimRegisteredDataSource)
                            .ThenInclude(drds => drds.DimOrganization).AsNoTracking()
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPublication).AsNoTracking().FirstOrDefaultAsync(dup => dup.Id == userprofileId);

            // TODO: Currently all added publications get the same data source (Tiedejatutkimus.fi)

            // Get Tiedejatutkimus.fi registered data source
            var tiedejatutkimusRegisteredDataSource = await _userProfileService.GetTiedejatutkimusFiRegisteredDataSource();
            // Get DimFieldDisplaySetting for publication
            var dimFieldDisplaySettingsPublication = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION);

            // Registered data source organization name translation
            var nameTranslation_OrganizationName = _languageService.getNameTranslation(
                nameFi: tiedejatutkimusRegisteredDataSource.DimOrganization.NameFi,
                nameSv: tiedejatutkimusRegisteredDataSource.DimOrganization.NameSv,
                nameEn: tiedejatutkimusRegisteredDataSource.DimOrganization.NameEn
            );

            // Response object
            var profileEditorAddPublicationResponse = new ProfileEditorAddPublicationResponse();
            profileEditorAddPublicationResponse.source = new ProfileEditorSource()
            {
                RegisteredDataSource = tiedejatutkimusRegisteredDataSource.Name,
                Organization = new Organization()
                {
                    NameFi = nameTranslation_OrganizationName.NameFi,
                    NameSv = nameTranslation_OrganizationName.NameSv,
                    NameEn = nameTranslation_OrganizationName.NameEn
                }
            };


            // Loop publications
            foreach (ProfileEditorPublicationToAdd publicationToAdd in profileEditorPublicationsToAdd)
            {
                var publicationProcessed = false;
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
                    var dimPublication = await _ttvContext.DimPublications.AsNoTracking().FirstOrDefaultAsync(dp => dp.PublicationId == publicationToAdd.PublicationId);
                    // Check if DimPublication exists
                    if (dimPublication == null)
                    {
                        // Publication does not exist
                        profileEditorAddPublicationResponse.publicationsNotFound.Add(publicationToAdd.PublicationId);
                    }
                    else
                    {
                        // Add FactFieldValue
                        var factFieldValuePublication = _userProfileService.GetEmptyFactFieldValue();
                        factFieldValuePublication.Show = publicationToAdd.Show != null ? publicationToAdd.Show : false;
                        factFieldValuePublication.PrimaryValue = publicationToAdd.PrimaryValue != null ? publicationToAdd.PrimaryValue : false;
                        factFieldValuePublication.DimUserProfileId = dimUserProfile.Id;
                        factFieldValuePublication.DimFieldDisplaySettingsId = dimFieldDisplaySettingsPublication.Id;
                        factFieldValuePublication.DimPublicationId = dimPublication.Id;
                        factFieldValuePublication.DimRegisteredDataSourceId = tiedejatutkimusRegisteredDataSource.Id;
                        factFieldValuePublication.SourceId = Constants.SourceIdentifiers.TIEDEJATUTKIMUS;
                        factFieldValuePublication.Created = _utilityService.getCurrentDateTime();
                        factFieldValuePublication.Modified = _utilityService.getCurrentDateTime();
                        _ttvContext.FactFieldValues.Add(factFieldValuePublication);
                        await _ttvContext.SaveChangesAsync();

                        // Response data
                        var publicationItem = new ProfileEditorItemPublication()
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
                            }
                        };

                        profileEditorAddPublicationResponse.publicationsAdded.Add(publicationItem);
                    }
                }
            }

            // TODO: add Elasticsearch sync?

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);

            return Ok(new ApiResponsePublicationPostMany(success: true, reason:"", data: profileEditorAddPublicationResponse, fromCache: false));
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

            // Return immediately if there is nothing to remove
            if (publicationIds.Count == 0)
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