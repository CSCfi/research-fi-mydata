using api.Services;
using api.Models;
using api.Models.Ttv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;

namespace api.Controllers
{
    /*
     * UserProfileController handles creation, existence check and deletion of userprofile.
     */
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RequireScopeApi1AndClaimOrcid")]
    public class UserProfileController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly DemoDataService _demoDataService;
        private readonly UserProfileService _userProfileService;
        private readonly ElasticsearchService _elasticsearchService;
        private readonly UtilityService _utilityService;
        private readonly KeycloakAdminApiService _keycloakAdminApiService;
        private readonly ILogger<UserProfileController> _logger;
        private readonly IMemoryCache _cache;
        private readonly BackgroundElasticsearchPersonUpdateQueue _backgroundElasticsearchPersonUpdateQueue;

        public UserProfileController(TtvContext ttvContext, DemoDataService demoDataService, ElasticsearchService elasticsearchService, UserProfileService userProfileService, UtilityService utilityService, KeycloakAdminApiService keycloakAdminApiService, ILogger<UserProfileController> logger, IMemoryCache memoryCache, BackgroundElasticsearchPersonUpdateQueue backgroundElasticsearchPersonUpdateQueue)
        {
            _ttvContext = ttvContext;
            _demoDataService = demoDataService;
            _userProfileService = userProfileService;
            _elasticsearchService = elasticsearchService;
            _utilityService = utilityService;
            _keycloakAdminApiService = keycloakAdminApiService;
            _logger = logger;
            _cache = memoryCache;
            _backgroundElasticsearchPersonUpdateQueue = backgroundElasticsearchPersonUpdateQueue;
        }

        /// <summary>
        /// Check if user profile exists.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            // Get ORCID ID.
            var orcidId = this.GetOrcidId();

            // Get userprofile id from ORCID id.
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);

            // Userprofile id must be positive.
            if (userprofileId > 0)
            {
                return Ok(new ApiResponse(success: true));
            }
            return Ok(new ApiResponse(success: false, reason: "profile not found"));
        }

        /// <summary>
        /// Create user profile.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create()
        {
            // Get ORCID id.
            var orcidId = this.GetOrcidId();
            // Log request
            _logger.LogInformation(this.GetLogPrefix() + " create profile request");

            // Get DimPid by ORCID id.
            // Also get related entities. Needed when searching existing data that should be automatically included in profile.
            var dimPid = await _ttvContext.DimPids
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimNames)
                        .ThenInclude(dn => dn.FactContributions).AsNoTracking()
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimNames)
                        .ThenInclude(dn => dn.DimRegisteredDataSource).AsNoTracking()
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimTelephoneNumbers)
                        .ThenInclude(dtn => dtn.DimRegisteredDataSource).AsNoTracking()
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimUserProfiles).AsNoTracking().FirstOrDefaultAsync(p => p.PidContent == orcidId && p.PidType == Constants.PidTypes.ORCID);

            // Get current DateTime
            DateTime currentDateTime = _utilityService.getCurrentDateTime();

            // Check if DimPid 
            if (dimPid == null)
            {
                // DimPid was not found.

                // Add new DimPid, add new DimKnownPerson
                dimPid = _userProfileService.GetEmptyDimPid();
                dimPid.PidContent = orcidId;
                dimPid.PidType = Constants.PidTypes.ORCID;

                // Since new DimPid is added, then new DimKnownPerson must be added
                dimPid.DimKnownPerson = new DimKnownPerson()
                {
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = currentDateTime,
                    Modified = currentDateTime
                };
                dimPid.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                _ttvContext.DimPids.Add(dimPid);
                await _ttvContext.SaveChangesAsync();
            }
            else if (dimPid.DimKnownPerson == null || dimPid.DimKnownPersonId == -1)
            {
                // DimPid was found but it does not have related DimKnownPerson.
                var kp = new DimKnownPerson()
                {
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = currentDateTime,
                    Modified = currentDateTime
                };
                _ttvContext.DimKnownPeople.Add(kp);
                dimPid.DimKnownPerson = kp;
                await _ttvContext.SaveChangesAsync();
            }


            // Add DimUserProfile
            var dimUserProfile = dimPid.DimKnownPerson.DimUserProfiles.FirstOrDefault();
            if (dimUserProfile == null)
            {
                // Add DimUserProfile
                dimUserProfile = new DimUserProfile()
                {
                    DimKnownPersonId = dimPid.DimKnownPerson.Id,
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = currentDateTime,
                    AllowAllSubscriptions = false
                };
                _ttvContext.DimUserProfiles.Add(dimUserProfile);
                await _ttvContext.SaveChangesAsync();

                // Add DimFieldDisplaySettings
                foreach (int fieldIdentifier in _userProfileService.GetFieldIdentifiers())
                {
                    var dimFieldDisplaySetting = new DimFieldDisplaySetting()
                    {
                        DimUserProfileId = dimUserProfile.Id,
                        FieldIdentifier = fieldIdentifier,
                        Show = false,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySetting);
                }
                await _ttvContext.SaveChangesAsync();


                // Demo data can be added to every user profile by uncommenting the following line
                // await _demoDataService.AddDemoDataToUserProfile(orcidId, dimUserProfile);
                await _userProfileService.AddTtvDataToUserProfile(dimPid.DimKnownPerson, dimUserProfile);
            }

            _logger.LogInformation(this.GetLogPrefix() + " profile created");
            return Ok(new ApiResponse(success: true));
        }


        /// <summary>
        /// Delete user profile.
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete()
        {
            // Get ORCID id.
            var orcidId = this.GetOrcidId();
            // Log request.
            _logger.LogInformation(this.GetLogPrefix() + " delete profile request");

            // Check that userprofile exists.
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);

            // Remove entry from Elasticsearch index in a background task.
            _backgroundElasticsearchPersonUpdateQueue.QueueBackgroundWorkItem(async token =>
            {
                _logger.LogInformation($"Background task for removing {orcidId} from Elasticsearch person index started at {DateTime.UtcNow}");
                // Update Elasticsearch person index.
                await _elasticsearchService.DeleteEntryFromElasticsearchPersonIndex(orcidId);
                _logger.LogInformation($"Background task for removing {orcidId} from Elasticseach person index ended at {DateTime.UtcNow}");
            });


            // Get DimUserProfile and related data that should be removed. 
            var dimUserProfile = await _ttvContext.DimUserProfiles
                .Include(dup => dup.DimFieldDisplaySettings)
                .Include(dup => dup.DimUserChoices)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimRegisteredDataSource)
                        .ThenInclude(drds => drds.DimOrganization)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimName)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimWebLink)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPid)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPidIdOrcidPutCodeNavigation)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimAffiliation)
                        .ThenInclude(da => da.DimOrganization)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEducation)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimTelephoneNumber)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEmailAddrress)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearcherDescription)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimOrcidPublication)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimKeyword)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearchDataset)
                .FirstOrDefaultAsync(up => up.Id == userprofileId);

            foreach (FactFieldValue ffv in dimUserProfile.FactFieldValues.Where(ffv => ffv.DimNameId == -1))
            {
                // DimAffiliation. In demo version the related DimOrganization is removed.
                if (ffv.DimAffiliationId != -1)
                {
                    var dimOrganization = ffv.DimAffiliation.DimOrganization;
                    _ttvContext.FactFieldValues.Remove(ffv);

                    if (_userProfileService.CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimAffiliations.Remove(ffv.DimAffiliation);
                    }

                    // Remove related DimOrganization
                    // TODO: Removal of DimOrganization only in demo version, if sourceId is ORCID
                    if (dimOrganization.SourceId == Constants.SourceIdentifiers.PROFILE_API || dimOrganization.SourceId == Constants.SourceIdentifiers.DEMO)
                    {
                        _ttvContext.DimOrganizations.Remove(dimOrganization);
                    }
                }
                else
                {
                    // Always remove FactFieldValue
                    _ttvContext.FactFieldValues.Remove(ffv);
                }

                // ORCID put code
                if (ffv.DimPidIdOrcidPutCode != -1)
                {
                    _ttvContext.DimPids.Remove(ffv.DimPidIdOrcidPutCodeNavigation);
                }

                // DimPid
                // DimPids related to FactFieldValue store person's external identifiers 
                if (ffv.DimPidId != -1)
                {
                    if (_userProfileService.CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimPids.Remove(ffv.DimPid);
                    }
                }

                // DimWebLink
                else if (ffv.DimWebLinkId != -1)
                {
                    if (_userProfileService.CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimWebLinks.Remove(ffv.DimWebLink);
                    }
                }

                // DimOrcidPublication
                else if (ffv.DimOrcidPublicationId != -1)
                {
                    if (_userProfileService.CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimOrcidPublications.Remove(ffv.DimOrcidPublication);
                    }
                }

                // DimKeyword
                else if (ffv.DimKeywordId != -1)
                {
                    if (_userProfileService.CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimKeywords.Remove(ffv.DimKeyword);
                    }
                }

                // DimEducation
                else if (ffv.DimEducationId != -1)
                {
                    if (_userProfileService.CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimEducations.Remove(ffv.DimEducation);
                    }
                }

                // DimEmail
                else if (ffv.DimEmailAddrressId != -1)
                {
                    if (_userProfileService.CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimEmailAddrresses.Remove(ffv.DimEmailAddrress);
                    }
                }

                // DimResearcherDescription
                else if (ffv.DimResearcherDescriptionId != -1)
                {
                    if (_userProfileService.CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimResearcherDescriptions.Remove(ffv.DimResearcherDescription);
                    }
                }

                // DimTelephoneNumber
                else if (ffv.DimTelephoneNumberId != -1)
                {
                    if (_userProfileService.CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimTelephoneNumbers.Remove(ffv.DimTelephoneNumber);
                    }
                }

                // DimResarchDataset
                else if (ffv.DimResearchDatasetId != -1)
                {
                    if (_userProfileService.CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimResearchDatasets.Remove(ffv.DimResearchDataset);
                    }

                    // DEMO: remove test data from FactContribution
                    var factContributions = await _ttvContext.FactContributions.Where(fc => fc.DimResearchDatasetId == ffv.DimResearchDatasetId && fc.SourceId == Constants.SourceIdentifiers.DEMO).ToListAsync();
                    foreach (FactContribution fc in factContributions)
                    {
                        _ttvContext.FactContributions.Remove(fc);
                        _ttvContext.Entry(fc).State = EntityState.Deleted;
                    }

                    // DEMO: remove test data from DimPids
                    var dimPids = await _ttvContext.DimPids.Where(dp => dp.DimResearchDatasetId == ffv.DimResearchDatasetId && dp.SourceId == Constants.SourceIdentifiers.DEMO).ToListAsync();
                    foreach (DimPid dp in dimPids)
                    {
                        _ttvContext.DimPids.Remove(dp);
                        _ttvContext.Entry(dp).State = EntityState.Deleted;
                    }
                }
            }
            await _ttvContext.SaveChangesAsync();


            // Remove DimName
            foreach (FactFieldValue ffv in dimUserProfile.FactFieldValues.Where(ffv => ffv.DimNameId != -1))
            {
                _ttvContext.FactFieldValues.Remove(ffv);

                if (ffv.DimPidIdOrcidPutCode != -1)
                {
                    _ttvContext.DimPids.Remove(ffv.DimPidIdOrcidPutCodeNavigation);
                }

                if (_userProfileService.CanDeleteFactFieldValueRelatedData(ffv))
                {
                    _ttvContext.DimNames.Remove(ffv.DimName);
                }
            }
            await _ttvContext.SaveChangesAsync();

            // Remove DimFieldDisplaySettings
            _ttvContext.DimFieldDisplaySettings.RemoveRange(dimUserProfile.DimFieldDisplaySettings);

            // Remove cooperation user choices
            _ttvContext.DimUserChoices.RemoveRange(dimUserProfile.DimUserChoices);

            // Remove DimUserProfile
            _ttvContext.DimUserProfiles.Remove(dimUserProfile);

            // Must not remove DimKnownPerson.
            // Must not remove DimPid (ORCID ID).

            await _ttvContext.SaveChangesAsync();

            // Log deletion
            _logger.LogInformation(this.GetLogPrefix() + " profile deleted");

            // Keycloak: logout user
            await _keycloakAdminApiService.LogoutUser(this.GetBearerTokenFromHttpRequest());

            // Keycloak: remove user
            await _keycloakAdminApiService.RemoveUser(this.GetBearerTokenFromHttpRequest());

            return Ok(new ApiResponse(success: true));
        }
    }
}