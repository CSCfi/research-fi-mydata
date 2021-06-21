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

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly DemoDataService _demoDataService;
        private readonly OrcidApiService _orcidApiService;
        private readonly UserProfileService _userProfileService;

        public UserProfileController(TtvContext ttvContext, DemoDataService demoDataService, OrcidApiService orcidApiService, UserProfileService userProfileService)
        {
            _ttvContext = ttvContext;
            _demoDataService = demoDataService;
            _orcidApiService = orcidApiService;
            _userProfileService = userProfileService;
        }

        // Check if profile exists.
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
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
            // Get ORCID ID
            var orcidId = this.GetOrcidId();

            // Check if DimPid and DimKnownPerson already exist.
            var dimPid = await _ttvContext.DimPids
                .Include(i => i.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimTelephoneNumbers).AsNoTracking()
                .Include(i => i.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimUserProfiles).AsNoTracking().AsSplitQuery().FirstOrDefaultAsync(p => p.PidContent == orcidId && p.PidType == "ORCID");

            if (dimPid == null)
            {
                // DimPid was not found.

                // Add new DimPid, add new DimKnownPerson
                dimPid = new DimPid()
                {
                    PidContent = orcidId,
                    PidType = "ORCID",
                    DimKnownPerson = new DimKnownPerson(){
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        Created = DateTime.Now
                    },
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    Created = DateTime.Now
                };
                _ttvContext.DimPids.Add(dimPid);
                await _ttvContext.SaveChangesAsync();
            }
            else if (dimPid.DimKnownPerson == null || dimPid.DimKnownPersonId == -1)
            {
                // DimPid was found but it does not have DimKnownPerson.
                var kp = new DimKnownPerson()
                {
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    Created = DateTime.Now
                };
                _ttvContext.DimKnownPeople.Add(kp);
                dimPid.DimKnownPerson = kp;
                await _ttvContext.SaveChangesAsync();
            }


            // Add DimUserProfile
            var userprofile = dimPid.DimKnownPerson.DimUserProfiles.FirstOrDefault();
            if (userprofile == null)
            {
                userprofile = new DimUserProfile() {
                    DimKnownPersonId = dimPid.DimKnownPerson.Id,
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    Created = DateTime.Now,
                    AllowAllSubscriptions = false
                };
                _ttvContext.DimUserProfiles.Add(userprofile);
                await _ttvContext.SaveChangesAsync();


                // Add DimFieldDisplaySettings for data sources ORCID and DEMO
                var orcidRegisteredDataSourceId = await _userProfileService.GetOrcidRegisteredDataSourceId();
                var demoOrganization1RegisteredDataSource = await _demoDataService.GetOrganization1RegisteredDataSourceAsync();
                var demoOrganization2RegisteredDataSource = await _demoDataService.GetOrganization2RegisteredDataSourceAsync();
                var demoOrganization3RegisteredDataSource = await _demoDataService.GetOrganization3RegisteredDataSourceAsync();

                // TODO: enumerate Constants.FieldIdentifiers
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
                    Constants.FieldIdentifiers.ACTIVITY_ROLE_IN_RESERCH_COMMUNITY,
                    Constants.FieldIdentifiers.ACTIVITY_AFFILIATION,
                    Constants.FieldIdentifiers.ACTIVITY_EDUCATION,
                    Constants.FieldIdentifiers.ACTIVITY_PUBLICATION
                };

                // DimFieldDisplaySettings for ORCID registered data source
                foreach (int fieldIdentifier in fieldIdentifiers)
                {
                    var dimFieldDisplaySetting = new DimFieldDisplaySetting()
                    {
                        DimUserProfileId = userprofile.Id,
                        FieldIdentifier = fieldIdentifier,
                        Show = false,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        Created = DateTime.Now
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
                        DimUserProfileId = userprofile.Id,
                        FieldIdentifier = fieldIdentifier,
                        Show = false,
                        SourceId = Constants.SourceIdentifiers.DEMO,
                        SourceDescription = _demoDataService.GetDemoOrganization1Name(),
                        Created = DateTime.Now
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
                        DimUserProfileId = userprofile.Id,
                        FieldIdentifier = fieldIdentifier,
                        Show = false,
                        SourceId = Constants.SourceIdentifiers.DEMO,
                        SourceDescription = _demoDataService.GetDemoOrganization2Name(),
                        Created = DateTime.Now
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
                    DimUserProfileId = userprofile.Id,
                    FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                    Show = false,
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    SourceDescription = _demoDataService.GetDemoOrganization3Name(),
                    Created = DateTime.Now
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
                await _demoDataService.AddDemoDataToUserProfile(userprofile);
            }

            // Add Ttv data: telephone number
            //await _userProfileService.AddTtvTelephoneNumbers(dimPid.DimKnownPerson);

            return Ok(new ApiResponse(success: true));
        }

        // Delete profile
        // TODO: Delete only ORCID originated data
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            // Get userprofile
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Get DimUserProfile and related entities
            var dimUserProfile = await _ttvContext.DimUserProfiles
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.BrFieldDisplaySettingsDimRegisteredDataSources)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimName)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimWebLink)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPublication)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPid)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPidIdOrcidPutCodeNavigation)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearchActivity)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearcherToResearchCommunity)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEvent)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimAffiliation)
                        .ThenInclude(da => da.DimOrganization)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEducation)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimCompetence)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearchCommunity)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimTelephoneNumber)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEmailAddrress)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearcherDescription)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimIdentifierlessData)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimOrcidPublication)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimKeyword)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFieldOfScience).AsSplitQuery().FirstOrDefaultAsync(up => up.Id == userprofileId);

            foreach (FactFieldValue ffv in dimUserProfile.FactFieldValues)
            {
                // ORCID put code
                if (ffv.DimPidIdOrcidPutCode != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimPids.Remove(ffv.DimPidIdOrcidPutCodeNavigation);
                }

                // DimName
                if (ffv.DimNameId != -1)
                {
                    _ttvContext.FactFieldValues.RemoveRange(ffv);
                    if (ffv.SourceId == Constants.SourceIdentifiers.ORCID || ffv.SourceId == Constants.SourceIdentifiers.DEMO)
                    {
                        _ttvContext.DimNames.Remove(ffv.DimName);
                    }
                }

                // DimPid
                if (ffv.DimPidId != -1)
                {
                    _ttvContext.FactFieldValues.RemoveRange(ffv);
                    if (ffv.SourceId == Constants.SourceIdentifiers.ORCID || ffv.SourceId == Constants.SourceIdentifiers.DEMO)
                    {
                        _ttvContext.DimPids.Remove(ffv.DimPid);
                    }
                }

                // DimWebLink
                else if (ffv.DimWebLinkId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    if (ffv.SourceId == Constants.SourceIdentifiers.ORCID || ffv.SourceId == Constants.SourceIdentifiers.DEMO)
                    {
                        _ttvContext.DimWebLinks.Remove(ffv.DimWebLink);
                    }
                }

                // DimAffiliation
                else if (ffv.DimAffiliationId != -1)
                {
                    var dimOrganization = ffv.DimAffiliation.DimOrganization;
                    _ttvContext.FactFieldValues.Remove(ffv);

                    if (ffv.SourceId == Constants.SourceIdentifiers.ORCID || ffv.SourceId == Constants.SourceIdentifiers.DEMO)
                    {
                        _ttvContext.DimAffiliations.Remove(ffv.DimAffiliation);
                    }

                    // Remove related DimOrganization
                    // TODO: Removal of DimOrganization only in demo version, if sourceId is ORCID
                    if (dimOrganization.SourceId == Constants.SourceIdentifiers.ORCID)
                    {
                        _ttvContext.DimOrganizations.Remove(dimOrganization);
                    }
                }

                // DimOrcidPublication
                else if (ffv.DimOrcidPublicationId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    if (ffv.SourceId == Constants.SourceIdentifiers.ORCID || ffv.SourceId == Constants.SourceIdentifiers.DEMO)
                    {
                        _ttvContext.DimOrcidPublications.Remove(ffv.DimOrcidPublication);
                    }
                }

                // DimKeyword
                else if (ffv.DimKeywordId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    if (ffv.SourceId == Constants.SourceIdentifiers.ORCID || ffv.SourceId == Constants.SourceIdentifiers.DEMO)
                    {
                        _ttvContext.DimKeywords.Remove(ffv.DimKeyword);
                    }
                }

                // DimEducation
                else if (ffv.DimEducationId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    if (ffv.SourceId == Constants.SourceIdentifiers.ORCID || ffv.SourceId == Constants.SourceIdentifiers.DEMO)
                    {
                        _ttvContext.DimEducations.Remove(ffv.DimEducation);
                    }
                }

                // DimEmail
                else if (ffv.DimEmailAddrressId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    if (ffv.SourceId == Constants.SourceIdentifiers.ORCID || ffv.SourceId == Constants.SourceIdentifiers.DEMO)
                    {
                        _ttvContext.DimEmailAddrresses.Remove(ffv.DimEmailAddrress);
                    }
                }

                // DimResearcherDescription
                else if (ffv.DimResearcherDescriptionId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    if (ffv.SourceId == Constants.SourceIdentifiers.ORCID || ffv.SourceId == Constants.SourceIdentifiers.DEMO)
                    {
                        _ttvContext.DimResearcherDescriptions.Remove(ffv.DimResearcherDescription);
                    }
                }

                // DimTelephoneNumber
                else if (ffv.DimTelephoneNumberId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    if (ffv.SourceId == Constants.SourceIdentifiers.ORCID || ffv.SourceId == Constants.SourceIdentifiers.DEMO)
                    {
                        _ttvContext.DimTelephoneNumbers.Remove(ffv.DimTelephoneNumber);
                    }
                }

                // DimFundingDecision
                else if (ffv.DimFundingDecisionId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                }

                // DimPublication
                else if (ffv.DimPublicationId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                }

                // DimResearchActivity
                else if (ffv.DimResearchActivityId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                }

                // DimEvent
                else if (ffv.DimEventId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                }

                // DimCompetence
                else if (ffv.DimCompetenceId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimCompetences.Remove(ffv.DimCompetence);
                }

                // DimResearcherCommunity
                else if (ffv.DimResearchCommunityId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimResearchCommunities.Remove(ffv.DimResearchCommunity);
                }

                // DimIdentifierlessData
                else if (ffv.DimIdentifierlessDataId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                }

                // DimFieldOfScience
                else if (ffv.DimFieldOfScienceId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                }
            }
            await _ttvContext.SaveChangesAsync();

            // Remove DimFieldDisplaySettings relation to DimRegisteredDataSource
            foreach (DimFieldDisplaySetting dimFieldDisplaySetting in dimUserProfile.DimFieldDisplaySettings)
            {
                _ttvContext.BrFieldDisplaySettingsDimRegisteredDataSources.RemoveRange(dimFieldDisplaySetting.BrFieldDisplaySettingsDimRegisteredDataSources);
            }

            // Remove DimFieldDisplaySettings
            _ttvContext.DimFieldDisplaySettings.RemoveRange(dimUserProfile.DimFieldDisplaySettings);

            // Remove DimUserProfile
            _ttvContext.DimUserProfiles.Remove(dimUserProfile);

            // TODO: Should DimKnownPerson be removed?
            // TODO: Should DimPid be removed?

            await _ttvContext.SaveChangesAsync();
            


            return Ok(new ApiResponse(success: true));
        }
    }
}