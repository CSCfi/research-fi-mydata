using api.Services;
using api.Models;
using api.Models.Ttv;
using api.Models.Orcid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace api.Controllers
{
    [Route("api/orcid")]
    [ApiController]
    [Authorize]
    public class OrcidController : TtvControllerBase
    {
        private readonly UserProfileService _userProfileService;
        private readonly OrcidApiService _orcidApiService;
        private readonly OrcidJsonParserService _orcidJsonParserService;
        private readonly TtvContext _ttvContext;

        public OrcidController(UserProfileService userProfileService, OrcidApiService orcidApiService, OrcidJsonParserService orcidJsonParserService, TtvContext ttvContext)
        {
            _userProfileService = userProfileService;
            _orcidApiService = orcidApiService;
            _orcidJsonParserService = orcidJsonParserService;
            _ttvContext = ttvContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Get ORCID ID
            var orcidId = this.GetOrcidId();

            // Get DimPid with related entities
            /*
            var dimPid = await _ttvContext.DimPids
                .Include(i => i.DimKnownPerson)
                  .ThenInclude(i => i.DimUserProfiles)
                    .ThenInclude(i => i.DimFieldDisplaySettings)
                      .ThenInclude(i => i.FactFieldValues)
                        .ThenInclude(i => i.DimWebLink)
                .Include(i => i.DimKnownPerson)
                  .ThenInclude(i => i.DimNameDimKnownPersonidFormerNamesNavigations).AsSplitQuery().FirstOrDefaultAsync(p => p.PidContent == orcidId && p.PidType == "ORCID");
            */
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);

            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Get record JSON from ORCID
            var json = await _orcidApiService.GetJson(orcidId);

            // Get DimUserProfile with related entities
            var dimUserProfile = await _ttvContext.DimUserProfiles.FirstOrDefaultAsync(up => up.Id == userprofileId);

            // Get DimKnownPerson
            var dimKnownPerson = await _ttvContext.DimKnownPeople.AsNoTracking().FirstOrDefaultAsync(dkp => dkp.Id == dimUserProfile.DimKnownPersonId);

            /*
            var dimPid = await _ttvContext.DimPids
                .Include(i => i.DimKnownPerson)
                  .ThenInclude(i => i.DimUserProfiles)
                    .ThenInclude(i => i.DimFieldDisplaySettings)
                      .ThenInclude(i => i.FactFieldValues)
                        .ThenInclude(i => i.DimWebLink)
                .Include(i => i.DimKnownPerson)
                  .ThenInclude(i => i.DimNameDimKnownPersonidFormerNamesNavigations).AsSplitQuery().FirstOrDefaultAsync(p => p.PidContent == orcidId && p.PidType == "ORCID");
            */

            // TODO
            // Names
            // Check if entry for DimName from source "orcid" already exists

            // TODO - check that source is ORCID
            //var dimName = dimPid.DimKnownPerson.DimNameDimKnownPersonIdConfirmedIdentityNavigation.FirstOrDefault(d => d.SourceId == Constants.SourceIdentifiers.ORCID);
            var dimName = dimKnownPerson.DimNameDimKnownPersonIdConfirmedIdentityNavigations.FirstOrDefault();
            if (dimName == null)
            {
                dimName = new DimName()
                {
                    LastName = _orcidJsonParserService.GetFamilyName(json).Value,
                    FirstNames = _orcidJsonParserService.GetGivenNames(json).Value,
                    DimKnownPersonIdConfirmedIdentity = dimKnownPerson.Id,
                    DimKnownPersonidFormerNames = -1,
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    Created = DateTime.Now
                };
                _ttvContext.DimNames.Add(dimName);
            }
            else
            {
                dimName.LastName = _orcidJsonParserService.GetFamilyName(json).Value;
                dimName.FirstNames = _orcidJsonParserService.GetGivenNames(json).Value;
                dimName.Modified = DateTime.Now;
            }
            await _ttvContext.SaveChangesAsync();



            // Create DimFieldDisplaySetting for LastName
            // TODO - check that source is ORCID
            var dimFieldDisplaySettingsLastName = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(d => d.FieldIdentifier == Constants.FieldIdentifiers.LAST_NAME);
            //var dimFieldDisplaySettingsLastName = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(d => d.FieldIdentifier == Constants.FieldIdentifiers.LAST_NAME && d.SourceId == Constants.SourceIdentifiers.ORCID);
            if (dimFieldDisplaySettingsLastName == null)
            {
                dimFieldDisplaySettingsLastName = new DimFieldDisplaySetting()
                {
                    DimUserProfileId = dimUserProfile.Id,
                    FieldIdentifier = Constants.FieldIdentifiers.LAST_NAME,
                    Show = false,
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    Created = DateTime.Now,
                };
                _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySettingsLastName);
            }
            else
            {
                dimFieldDisplaySettingsLastName.Modified = DateTime.Now;
            }

            // Create DimFieldDisplaySetting for FirstNames
            // TODO - check that source is ORCID
            var dimFieldDisplaySettingsFirstNames = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(d => d.FieldIdentifier == Constants.FieldIdentifiers.FIRST_NAMES);
            // var dimFieldDisplaySettingsFirstNames = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(d => d.FieldIdentifier == Constants.FieldIdentifiers.FIRST_NAMES && d.SourceId == Constants.SourceIdentifiers.ORCID);
            if (dimFieldDisplaySettingsFirstNames == null)
            {
                dimFieldDisplaySettingsFirstNames = new DimFieldDisplaySetting()
                {
                    DimUserProfileId = dimUserProfile.Id,
                    FieldIdentifier = Constants.FieldIdentifiers.FIRST_NAMES,
                    Show = false,
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    Created = DateTime.Now,
                };
                _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySettingsFirstNames);
            }
            else
            {
                dimFieldDisplaySettingsFirstNames.Modified = DateTime.Now;
            }

            await _ttvContext.SaveChangesAsync();




            // Create FactFieldValues for LastName
            // TODO - check that source is ORCID
            var factFieldValuesLastName = dimUserProfile.FactFieldValues.FirstOrDefault(f => f.DimFieldDisplaySettingsId == dimFieldDisplaySettingsLastName.Id);
            //var factFieldValuesLastName = dimUserProfile.FactFieldValues.FirstOrDefault(f => f.DimFieldDisplaySettingsId == dimFieldDisplaySettingsLastName.Id && f.SourceId == Constants.SourceIdentifiers.ORCID);
            if (factFieldValuesLastName == null)
            {
                factFieldValuesLastName = new FactFieldValue()
                {
                    DimPidId = -1,
                    DimUserProfileId = dimUserProfile.Id,
                    DimFieldDisplaySettingsId = dimFieldDisplaySettingsLastName.Id,
                    DimNameId = dimName.Id,
                    DimWebLinkId = -1,
                    DimFundingDecisionId = -1,
                    DimPublicationId = -1,
                    DimPidIdOrcidPutCode = -1,
                    DimResearchActivityId = -1,
                    DimEventId = -1,
                    DimEducationId = -1,
                    DimCompetenceId = -1,
                    DimResearchCommunityId = -1,
                    DimTelephoneNumberId = -1,
                    DimEmailAddrressId = -1,
                    DimResearcherDescriptionId = -1,
                    DimIdentifierlessDataId = -1,
                    Show = false,
                    PrimaryValue = false,
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    Created = DateTime.Now,
                };
                _ttvContext.FactFieldValues.Add(factFieldValuesLastName);
            }
            else
            {
                factFieldValuesLastName.Modified = DateTime.Now;
            }

            // Create FactFieldValues for FirstNames
            // TODO - check that source is ORCID
            var factFieldValuesFirstNames = dimUserProfile.FactFieldValues.FirstOrDefault(f => f.DimFieldDisplaySettingsId == dimFieldDisplaySettingsFirstNames.Id);
            //var factFieldValuesFirstNames = dimUserProfile.FactFieldValues.FirstOrDefault(f => f.DimFieldDisplaySettingsId == dimFieldDisplaySettingsFirstNames.Id && f.SourceId == Constants.SourceIdentifiers.ORCID);
            if (factFieldValuesFirstNames == null) {
                factFieldValuesFirstNames = new FactFieldValue()
                {
                    DimPidId = -1,
                    DimUserProfileId = dimUserProfile.Id,
                    DimFieldDisplaySettingsId = dimFieldDisplaySettingsFirstNames.Id,
                    DimNameId = dimName.Id,
                    DimWebLinkId = -1,
                    DimFundingDecisionId = -1,
                    DimPublicationId = -1,
                    DimPidIdOrcidPutCode = -1,
                    DimResearchActivityId = -1,
                    DimEventId = -1,
                    DimEducationId = -1,
                    DimCompetenceId = -1,
                    DimResearchCommunityId = -1,
                    DimTelephoneNumberId = -1,
                    DimEmailAddrressId = -1,
                    DimResearcherDescriptionId = -1,
                    DimIdentifierlessDataId = -1,
                    Show = false,
                    PrimaryValue = false,
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    Created = DateTime.Now,
                };
                _ttvContext.FactFieldValues.Add(factFieldValuesFirstNames);
            }
            else
            {
                factFieldValuesFirstNames.Modified = DateTime.Now;
            }

            await _ttvContext.SaveChangesAsync();




            // Researcher urls
            var researcherUrls = _orcidJsonParserService.GetResearcherUrls(json);

            foreach (OrcidResearcherUrl researchUrl in researcherUrls)
            {
                // Create new weblink
                // TODO - check that source is ORCID
                var dimWebLink = dimKnownPerson.DimWebLinks.FirstOrDefault(d => d.LinkLabel == researchUrl.UrlName && d.Url == researchUrl.Url);
                //var dimWebLink = dimPid.DimKnownPerson.DimWebLink.FirstOrDefault(d => d.LinkLabel == researchUrl.UrlName && d.Url == researchUrl.Url && d.SourceId == Constants.SourceIdentifiers.ORCID);
                if (dimWebLink == null)
                {
                    dimWebLink = new DimWebLink()
                    {
                        Url = researchUrl.Url,
                        LinkLabel = researchUrl.UrlName,
                        DimOrganizationId = -1,
                        DimKnownPersonId = dimKnownPerson.Id,
                        DimCallProgrammeId = -1,
                        DimFundingDecisionId = -1,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        Created = DateTime.Now,
                    };
                    _ttvContext.DimWebLinks.Add(dimWebLink);
                }
                else
                {
                    dimWebLink.Modified = DateTime.Now;
                }
                await _ttvContext.SaveChangesAsync();

                // Check if FactFieldValues already exists for the web link. If yes, then related DimFieldDisplaySetting must also already exist.
                if (dimWebLink.FactFieldValues.Count == 0)
                {
                    // Store Orcid put code into DimPid
                    var dimPidOrcidPutCodeWebLink = new DimPid()
                    {
                        PidContent = researchUrl.PutCode.GetDbValue(),
                        PidType = "orcid put code",
                        DimKnownPersonId = dimKnownPerson.Id,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeWebLink);

                    // Create DimFieldDisplaySetting for weblink
                    var dimFieldDisplaySettingsWebLink = new DimFieldDisplaySetting()
                    {
                        DimUserProfileId = dimUserProfile.Id,
                        FieldIdentifier = Constants.FieldIdentifiers.WEB_LINK,
                        Show = false,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        Created = DateTime.Now,
                    };
                    _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySettingsWebLink);
                    await _ttvContext.SaveChangesAsync();

                    // Create FactFieldValues for weblink
                    var factFieldValuesWebLink = new FactFieldValue()
                    {
                        DimPidId = -1,
                        DimUserProfileId = dimUserProfile.Id,
                        DimFieldDisplaySettingsId = dimFieldDisplaySettingsWebLink.Id,
                        DimNameId = -1,
                        DimWebLinkId = dimWebLink.Id,
                        DimFundingDecisionId = -1,
                        DimPublicationId = -1,
                        DimPidIdOrcidPutCode = dimPidOrcidPutCodeWebLink.Id,
                        DimResearchActivityId = -1,
                        DimEventId = -1,
                        DimEducationId = -1,
                        DimCompetenceId = -1,
                        DimResearchCommunityId = -1,
                        DimTelephoneNumberId = -1,
                        DimEmailAddrressId = -1,
                        DimResearcherDescriptionId = -1,
                        DimIdentifierlessDataId = -1,
                        Show = false,
                        PrimaryValue = false,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        Created = DateTime.Now,
                    };

                    _ttvContext.FactFieldValues.Add(factFieldValuesWebLink);
                    await _ttvContext.SaveChangesAsync();
                }
                else
                {
                    dimWebLink.FactFieldValues.First().Modified = DateTime.Now;
                    dimWebLink.FactFieldValues.First().DimFieldDisplaySettings.Modified = DateTime.Now;
                    await _ttvContext.SaveChangesAsync();
                }
            }
            
            // Biography
            //var biography = _orcidJsonParserService.GetBiography(json);
            //dimPid.DimKnownPerson.ResearchDescription = biography;
            
            await _ttvContext.SaveChangesAsync();

            return Ok(new ApiResponse(success: true));
        }
    }
}