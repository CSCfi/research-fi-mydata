using api.Services;
using api.Models;
using api.Models.Ttv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    /*
     * UserProfileController handles creation, existence check and deletion of userprofile.
     */
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly DemoDataService _demoDataService;
        private readonly UserProfileService _userProfileService;
        private readonly ElasticsearchService _elasticsearchService;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(TtvContext ttvContext, DemoDataService demoDataService, ElasticsearchService elasticsearchService, UserProfileService userProfileService, ILogger<UserProfileController> logger)
        {
            _ttvContext = ttvContext;
            _demoDataService = demoDataService;
            _userProfileService = userProfileService;
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        // Check if profile exists.
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Get ORCID id.
            var orcidId = this.GetOrcidId();
            // Log request
            _logger.LogInformation(this.GetLogPrefix() + " check profile exists");

            // Get userprofile id from ORCID id.
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);

            // Userprofile id must be positive.
            if (userprofileId > 0)
            {
                return Ok(new ApiResponse(success: true));
            }
            return Ok(new ApiResponse(success: false, reason: "profile not found"));
        }

        // Create profile
        [HttpPost]
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
                    .ThenInclude(dkp => dkp.DimNameDimKnownPersonIdConfirmedIdentityNavigations)
                        .ThenInclude(dn => dn.FactContributions).AsNoTracking()
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimNameDimKnownPersonIdConfirmedIdentityNavigations)
                        .ThenInclude(dn => dn.DimRegisteredDataSource).AsNoTracking()
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimTelephoneNumbers)
                        .ThenInclude(dtn => dtn.DimRegisteredDataSource).AsNoTracking()
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimUserProfiles).AsNoTracking().AsSplitQuery().FirstOrDefaultAsync(p => p.PidContent == orcidId && p.PidType == Constants.PidTypes.ORCID);

            // Check if DimPid 
            if (dimPid == null)
            {
                // DimPid was not found.

                // Add new DimPid, add new DimKnownPerson
                dimPid = _userProfileService.GetEmptyDimPid();
                dimPid.PidContent = orcidId;
                dimPid.PidType = Constants.PidTypes.ORCID;

                // Since new DimPid is added, then new DimKnownPerson must be added
                dimPid.DimKnownPerson = new DimKnownPerson() {
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };
                dimPid.SourceId = Constants.SourceIdentifiers.ORCID;
                _ttvContext.DimPids.Add(dimPid);
                await _ttvContext.SaveChangesAsync();
            }
            else if (dimPid.DimKnownPerson == null || dimPid.DimKnownPersonId == -1)
            {
                // DimPid was found but it does not have related DimKnownPerson.
                var kp = new DimKnownPerson()
                {
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };
                _ttvContext.DimKnownPeople.Add(kp);
                dimPid.DimKnownPerson = kp;
                await _ttvContext.SaveChangesAsync();
            }


            // Add DimUserProfile
            var dimUserProfile = dimPid.DimKnownPerson.DimUserProfiles.FirstOrDefault();
            if (dimUserProfile == null)
            {
                dimUserProfile = new DimUserProfile() {
                    DimKnownPersonId = dimPid.DimKnownPerson.Id,
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = DateTime.Now,
                    AllowAllSubscriptions = false
                };
                _ttvContext.DimUserProfiles.Add(dimUserProfile);
                await _ttvContext.SaveChangesAsync();


                // Add DimFieldDisplaySettings for data sources ORCID and DEMO
                var orcidRegisteredDataSourceId = await _userProfileService.GetOrcidRegisteredDataSourceId();
                var demoOrganization1RegisteredDataSource = await _demoDataService.GetOrganization1RegisteredDataSourceAsync();
                var demoOrganization2RegisteredDataSource = await _demoDataService.GetOrganization2RegisteredDataSourceAsync();
                var demoOrganization3RegisteredDataSource = await _demoDataService.GetOrganization3RegisteredDataSourceAsync();

                // TODO: Field identifiers used in demo. In production this list should be extended.
                var fieldIdentifiers = new List<int>
                {
                    Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS,
                    Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER,
                    Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE,
                    Constants.FieldIdentifiers.PERSON_KEYWORD,
                    Constants.FieldIdentifiers.PERSON_NAME,
                    Constants.FieldIdentifiers.PERSON_OTHER_NAMES,
                    Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION,
                    Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER,
                    Constants.FieldIdentifiers.PERSON_WEB_LINK,
                    Constants.FieldIdentifiers.ACTIVITY_AFFILIATION,
                    Constants.FieldIdentifiers.ACTIVITY_EDUCATION,
                    Constants.FieldIdentifiers.ACTIVITY_PUBLICATION
                };

                // DimFieldDisplaySettings for ORCID registered data source
                foreach (int fieldIdentifier in fieldIdentifiers)
                {
                    var dimFieldDisplaySetting = new DimFieldDisplaySetting()
                    {
                        DimUserProfileId = dimUserProfile.Id,
                        FieldIdentifier = fieldIdentifier,
                        Show = false,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = DateTime.Now,
                        Modified = DateTime.Now
                    };
                    dimFieldDisplaySetting.BrFieldDisplaySettingsDimRegisteredDataSources.Add(
                        new BrFieldDisplaySettingsDimRegisteredDataSource()
                        {
                            DimFieldDisplaySettingsId = dimFieldDisplaySetting.Id,
                            DimRegisteredDataSourceId = orcidRegisteredDataSourceId
                        }
                    );
                    _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySetting);
                }
                await _ttvContext.SaveChangesAsync();


                // DimFieldDisplaySettings for demo: Organization 1 registered data source
                foreach (int fieldIdentifier in fieldIdentifiers)
                {
                    var dimFieldDisplaySetting = new DimFieldDisplaySetting()
                    {
                        DimUserProfileId = dimUserProfile.Id,
                        FieldIdentifier = fieldIdentifier,
                        Show = false,
                        SourceId = Constants.SourceIdentifiers.DEMO,
                        SourceDescription = _demoDataService.GetDemoOrganization1Name(), // In demo must use Org1 name here
                        Created = DateTime.Now,
                        Modified = DateTime.Now
                    };
                    dimFieldDisplaySetting.BrFieldDisplaySettingsDimRegisteredDataSources.Add(
                        new BrFieldDisplaySettingsDimRegisteredDataSource()
                        {
                            DimFieldDisplaySettingsId = dimFieldDisplaySetting.Id,
                            DimRegisteredDataSourceId = demoOrganization1RegisteredDataSource.Id
                        }
                    );
                    _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySetting);
                }
                await _ttvContext.SaveChangesAsync();

                // DimFieldDisplaySettings for demo: Organization 2 registered data source
                foreach (int fieldIdentifier in fieldIdentifiers)
                {
                    var dimFieldDisplaySetting = new DimFieldDisplaySetting()
                    {
                        DimUserProfileId = dimUserProfile.Id,
                        FieldIdentifier = fieldIdentifier,
                        Show = false,
                        SourceId = Constants.SourceIdentifiers.DEMO,
                        SourceDescription = _demoDataService.GetDemoOrganization2Name(), // In demo must use Org2 name here
                        Created = DateTime.Now,
                        Modified = DateTime.Now
                    };
                    dimFieldDisplaySetting.BrFieldDisplaySettingsDimRegisteredDataSources.Add(
                        new BrFieldDisplaySettingsDimRegisteredDataSource()
                        {
                            DimFieldDisplaySettingsId = dimFieldDisplaySetting.Id,
                            DimRegisteredDataSourceId = demoOrganization2RegisteredDataSource.Id
                        }
                    );
                    _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySetting);
                }
                await _ttvContext.SaveChangesAsync();

                // DimFieldDisplaySettings for demo: Organization 3 (Tiedejatutkimus.fi) publications
                var dimFieldDisplaySettingDemoOrganization3 = new DimFieldDisplaySetting()
                {
                    DimUserProfileId = dimUserProfile.Id,
                    FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                    Show = false,
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };
                dimFieldDisplaySettingDemoOrganization3.BrFieldDisplaySettingsDimRegisteredDataSources.Add(
                    new BrFieldDisplaySettingsDimRegisteredDataSource()
                    {
                        DimFieldDisplaySettingsId = dimFieldDisplaySettingDemoOrganization3.Id,
                        DimRegisteredDataSourceId = demoOrganization3RegisteredDataSource.Id
                    }
                );
                _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySettingDemoOrganization3);
                await _ttvContext.SaveChangesAsync();

                // Add demo data
                await _demoDataService.AddDemoDataToUserProfile(dimUserProfile);
                await _userProfileService.AddTtvDataToUserProfile(dimPid.DimKnownPerson, dimUserProfile);
            }

            _logger.LogInformation(this.GetLogPrefix() + " profile created");
            return Ok(new ApiResponse(success: true));
        }


        // Delete profile
        [HttpDelete]
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

            // Remove entry from Elasticsearch index
            // TODO use BackgroundService to handle Elasticsearch API call.
            if (_elasticsearchService.IsElasticsearchSyncEnabled())
            {
                await _elasticsearchService.DeleteEntryFromElasticsearchPersonIndex(orcidId);
            }

            // Get DimUserProfile and related data that should be removed. 
            var dimUserProfile = await _ttvContext.DimUserProfiles
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.BrFieldDisplaySettingsDimRegisteredDataSources)
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
                    .ThenInclude(ffv => ffv.DimKeyword).AsSplitQuery().FirstOrDefaultAsync(up => up.Id == userprofileId);

            foreach (FactFieldValue ffv in dimUserProfile.FactFieldValues)
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
                    if (dimOrganization.SourceId == Constants.SourceIdentifiers.ORCID)
                    {
                        _ttvContext.DimOrganizations.Remove(dimOrganization);
                    }
                } else
                {
                    // Always remove FactFieldValue
                    _ttvContext.FactFieldValues.Remove(ffv);
                }

                // ORCID put code
                if (ffv.DimPidIdOrcidPutCode != -1)
                {
                    _ttvContext.DimPids.Remove(ffv.DimPidIdOrcidPutCodeNavigation);
                }

                // DimName
                if (ffv.DimNameId != -1)
                {
                    if (_userProfileService.CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimNames.Remove(ffv.DimName);
                    }
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
            }
            await _ttvContext.SaveChangesAsync();

            // Remove profile's DimFieldDisplaySettings relation to DimRegisteredDataSource
            foreach (DimFieldDisplaySetting dimFieldDisplaySetting in dimUserProfile.DimFieldDisplaySettings)
            {
                _ttvContext.BrFieldDisplaySettingsDimRegisteredDataSources.RemoveRange(dimFieldDisplaySetting.BrFieldDisplaySettingsDimRegisteredDataSources);
            }

            // Remove DimFieldDisplaySettings
            _ttvContext.DimFieldDisplaySettings.RemoveRange(dimUserProfile.DimFieldDisplaySettings);

            // Remove DimUserProfile
            _ttvContext.DimUserProfiles.Remove(dimUserProfile);

            // Must not remove DimKnownPerson.
            // Must not remove DimPid.

            await _ttvContext.SaveChangesAsync();

            // Log deletion
            _logger.LogInformation(this.GetLogPrefix() + " profile deleted");

            return Ok(new ApiResponse(success: true));
        }
    }
}