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
        private readonly UserProfileService _userProfileService;
        private readonly OrcidApiService _orcidApiService;

        public UserProfileController(TtvContext ttvContext, UserProfileService userProfileService, OrcidApiService orcidApiService)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _orcidApiService = orcidApiService;
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
                    .ThenInclude(i => i.DimUserProfiles).AsNoTracking().AsSplitQuery().FirstOrDefaultAsync(p => p.PidContent == orcidId && p.PidType == "ORCID");

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
            if (dimPid.DimKnownPerson.DimUserProfiles.FirstOrDefault() == null)
            {
                var userprofile = new DimUserProfile() {
                    DimKnownPersonId = dimPid.DimKnownPerson.Id,
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    Created = DateTime.Now,
                    AllowAllSubscriptions = false
                };
                _ttvContext.DimUserProfiles.Add(userprofile);
                await _ttvContext.SaveChangesAsync();


                // Add DimFieldDisplaySettings for data source ORCID
                var orcidRegisteredDataSourceId = await _userProfileService.GetOrcidRegisteredDataSourceId();
                // TODO: enumerate Constants.FieldIdentifiers
                var fieldIdentifiers = new List<int>
                {
                    Constants.FieldIdentifiers.PERSON_NAME,
                    Constants.FieldIdentifiers.PERSON_OTHER_NAMES,
                    Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION,
                    Constants.FieldIdentifiers.PERSON_WEB_LINK,
                    Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS,
                    Constants.FieldIdentifiers.PERSON_KEYWORD,
                    Constants.FieldIdentifiers.ACTIVITY_AFFILIATION,
                    Constants.FieldIdentifiers.ACTIVITY_EDUCATION,
                    Constants.FieldIdentifiers.ACTIVITY_PUBLICATION
                };

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
                
            }
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
                    .ThenInclude(ffv => ffv.DimPidIdOrcidPutCodeNavigation)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearchActivity)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEvent)
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
                    .ThenInclude(ffv => ffv.DimKeyword).AsSplitQuery().FirstOrDefaultAsync(up => up.Id == userprofileId);

            foreach (FactFieldValue ffv in dimUserProfile.FactFieldValues)
            {
                // Remove ORCID put code
                if (ffv.DimPidIdOrcidPutCode != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimPids.Remove(ffv.DimPidIdOrcidPutCodeNavigation);
                }

                // Remove related DimName
                if (ffv.DimNameId != -1)
                {
                    // DimName can have several related FactFieldValues (for first name, last name, etc). Remove them all.

                    // TODO: Check if all FactFieldValues and DimName can be removed, or should only current FactFieldValue
                    // be removed and DimName only when it does not have any related FactFieldValues left?
                    _ttvContext.FactFieldValues.RemoveRange(ffv.DimName.FactFieldValues);
                    _ttvContext.DimNames.Remove(ffv.DimName);
                }

                // Remove related DimWebLink
                else if (ffv.DimWebLinkId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimWebLinks.Remove(ffv.DimWebLink);
                }

                // Remove related DimFundingDecision
                else if (ffv.DimFundingDecisionId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimFundingDecisions.Remove(ffv.DimFundingDecision);
                }

                // Remove related DimPublication
                else if (ffv.DimPublicationId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimPublications.Remove(ffv.DimPublication);
                }

                // Remove related DimResearchActivity
                else if (ffv.DimResearchActivityId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimResearchActivities.Remove(ffv.DimResearchActivity);
                }

                // Remove related DimEvent
                else if (ffv.DimEventId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimEvents.Remove(ffv.DimEvent);
                }

                // Remove related DimEducation
                else if (ffv.DimEducationId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimEducations.Remove(ffv.DimEducation);
                }

                // Remove related DimCompetence
                else if (ffv.DimCompetenceId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimCompetences.Remove(ffv.DimCompetence);
                }

                // Remove related DimResearcherCommunity
                else if (ffv.DimResearchCommunityId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimResearchCommunities.Remove(ffv.DimResearchCommunity);
                }

                // Remove related DimTelephoneNumber
                else if (ffv.DimTelephoneNumberId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimTelephoneNumbers.Remove(ffv.DimTelephoneNumber);
                }

                // Remove related DimEmail
                else if (ffv.DimEmailAddrressId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimEmailAddrresses.Remove(ffv.DimEmailAddrress);
                }

                // Remove related DimResearcherDescription
                else if (ffv.DimResearcherDescriptionId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimResearcherDescriptions.Remove(ffv.DimResearcherDescription);
                }

                // Remove related DimIdentifierlessData
                else if (ffv.DimIdentifierlessDataId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimIdentifierlessData.Remove(ffv.DimIdentifierlessData);
                }

                // Remove related DimOrcidPublication
                else if (ffv.DimOrcidPublicationId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimOrcidPublications.Remove(ffv.DimOrcidPublication);
                }

                // Remove related DimKeyword
                else if (ffv.DimKeywordId != -1)
                {
                    _ttvContext.FactFieldValues.Remove(ffv);
                    _ttvContext.DimKeywords.Remove(ffv.DimKeyword);
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