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
    public class OrcidController : ControllerBase
    {
        private readonly OrcidApiService _orcidApiService;
        private readonly OrcidJsonParserService _orcidJsonParserService;
        private readonly TtvContext _ttvContext;

        public OrcidController(OrcidApiService orcidApiService, OrcidJsonParserService orcidJsonParserService, TtvContext ttvContext)
        {
            _orcidApiService = orcidApiService;
            _orcidJsonParserService = orcidJsonParserService;
            _ttvContext = ttvContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Get ORCID ID from user claims
            var orcidId = User.Claims.FirstOrDefault(x => x.Type == "orcid")?.Value;

            // Get DimPid with related entities
            var dimPid = await _ttvContext.DimPid
                .Include(i => i.DimKnownPerson)
                  .ThenInclude(i => i.DimUserProfile)
                    .ThenInclude(i => i.DimFieldDisplaySettings)
                    .ThenInclude(i => i.FactFieldValues)
                    .ThenInclude(i => i.DimWebLink)
                .Include(i => i.DimKnownPerson)
                  .ThenInclude(i => i.DimNameDimKnownPersonIdConfirmedIdentityNavigation).FirstOrDefaultAsync(p => p.PidContent == orcidId);

            // Return if pid was not found
            if (dimPid == null)
            {
                return NotFound();
            }

            // Get record JSON from ORCID
            var json = await _orcidApiService.GetJson(orcidId);

            // Names
            // Check if entry for DimName from source "orcid" already exists

            // TODO
            //var dimName = dimPid.DimKnownPerson.DimNameDimKnownPersonIdConfirmedIdentityNavigation.FirstOrDefault(d => d.SourceId == Constants.SourceIdentifiers.ORCID);
            var dimName = dimPid.DimKnownPerson.DimNameDimKnownPersonIdConfirmedIdentityNavigation.FirstOrDefault();
            if (dimName == null)
            {
                dimName = new DimName()
                {
                    LastName = _orcidJsonParserService.GetFamilyName(json).Value,
                    FirstNames = _orcidJsonParserService.GetGivenNames(json).Value,
                    //SourceId = Constants.SourceIdentifiers.ORCID,
                    DimKnownPersonIdConfirmedIdentity = (int)dimPid.DimKnownPersonId,
                    DimKnownPersonidFormerNames = -1,
                    Created = DateTime.Now
                };
                _ttvContext.DimName.Add(dimName);
            }
            else
            {
                dimName.LastName = _orcidJsonParserService.GetFamilyName(json).Value;
                dimName.FirstNames = _orcidJsonParserService.GetGivenNames(json).Value;
                dimName.Modified = DateTime.Now;
            }
            await _ttvContext.SaveChangesAsync();




            // Create DimFieldDisplaySetting for LastName
            // TODO
            var dimFieldDisplaySettingsLastName = dimPid.DimKnownPerson.DimUserProfile.First().DimFieldDisplaySettings.FirstOrDefault(d => d.FieldIdentifier == Constants.FieldIdentifiers.LAST_NAME);
            //var dimFieldDisplaySettingsLastName = dimPid.DimKnownPerson.DimUserProfile.First().DimFieldDisplaySettings.FirstOrDefault(d => d.FieldIdentifier == Constants.FieldIdentifiers.LAST_NAME && d.SourceId == Constants.SourceIdentifiers.ORCID);
            if (dimFieldDisplaySettingsLastName == null)
            {
                dimFieldDisplaySettingsLastName = new DimFieldDisplaySettings()
                {
                    DimUserProfileId = dimPid.DimKnownPerson.DimUserProfile.First().Id,
                    FieldIdentifier = Constants.FieldIdentifiers.LAST_NAME,
                    Show = false,
                    //SourceId = Constants.SourceIdentifiers.ORCID,
                    Created = DateTime.Now,
                };
                _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySettingsLastName);
            }
            else
            {
                dimFieldDisplaySettingsLastName.Modified = DateTime.Now;
            }

            // Create DimFieldDisplaySetting for FirstNames
            // TODO
            var dimFieldDisplaySettingsFirstNames = dimPid.DimKnownPerson.DimUserProfile.First().DimFieldDisplaySettings.FirstOrDefault(d => d.FieldIdentifier == Constants.FieldIdentifiers.FIRST_NAMES);
            // var dimFieldDisplaySettingsFirstNames = dimPid.DimKnownPerson.DimUserProfile.First().DimFieldDisplaySettings.FirstOrDefault(d => d.FieldIdentifier == Constants.FieldIdentifiers.FIRST_NAMES && d.SourceId == Constants.SourceIdentifiers.ORCID);
            if (dimFieldDisplaySettingsFirstNames == null)
            {
                dimFieldDisplaySettingsFirstNames = new DimFieldDisplaySettings()
                {
                    DimUserProfileId = dimPid.DimKnownPerson.DimUserProfile.First().Id,
                    FieldIdentifier = Constants.FieldIdentifiers.FIRST_NAMES,
                    Show = false,
                    //SourceId = Constants.SourceIdentifiers.ORCID,
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
            // TODO
            var factFieldValuesLastName = dimPid.DimKnownPerson.DimUserProfile.First().FactFieldValues.FirstOrDefault(f => f.DimFieldDisplaySettingsId == dimFieldDisplaySettingsLastName.Id);
            //var factFieldValuesLastName = dimPid.DimKnownPerson.DimUserProfile.First().FactFieldValues.FirstOrDefault(f => f.DimFieldDisplaySettingsId == dimFieldDisplaySettingsLastName.Id && f.SourceId == Constants.SourceIdentifiers.ORCID);
            if (factFieldValuesLastName == null)
            {
                factFieldValuesLastName = new FactFieldValues()
                {
                    DimPid = dimPid,
                    DimUserProfileId = dimPid.DimKnownPerson.DimUserProfile.First().Id,
                    DimFieldDisplaySettingsId = dimFieldDisplaySettingsLastName.Id,
                    DimWebLinkId = -1,
                    DimFundingDecisionId = -1,
                    DimNameId = dimName.Id,
                    DimPublicationId = -1,
                    //SourceId = Constants.SourceIdentifiers.ORCID,
                    Created = DateTime.Now,
                };
                _ttvContext.FactFieldValues.Add(factFieldValuesLastName);
            }
            else
            {
                factFieldValuesLastName.Modified = DateTime.Now;
            }

            // Create FactFieldValues for FirstNames
            // TODO
            var factFieldValuesFirstNames = dimPid.DimKnownPerson.DimUserProfile.First().FactFieldValues.FirstOrDefault(f => f.DimFieldDisplaySettingsId == dimFieldDisplaySettingsFirstNames.Id);
            //var factFieldValuesFirstNames = dimPid.DimKnownPerson.DimUserProfile.First().FactFieldValues.FirstOrDefault(f => f.DimFieldDisplaySettingsId == dimFieldDisplaySettingsFirstNames.Id && f.SourceId == Constants.SourceIdentifiers.ORCID);
            if (factFieldValuesFirstNames == null) {
                factFieldValuesFirstNames = new FactFieldValues()
                {
                    DimPid = dimPid,
                    DimUserProfileId = dimPid.DimKnownPerson.DimUserProfile.First().Id,
                    DimFieldDisplaySettingsId = dimFieldDisplaySettingsFirstNames.Id,
                    DimWebLinkId = -1,
                    DimFundingDecisionId = -1,
                    DimNameId = dimName.Id,
                    DimPublicationId = -1,
                    //SourceId = Constants.SourceIdentifiers.ORCID,
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
                // TODO
                var dimWebLink = dimPid.DimKnownPerson.DimWebLink.FirstOrDefault(d => d.LinkLabel == researchUrl.UrlName && d.Url == researchUrl.Url);
                //var dimWebLink = dimPid.DimKnownPerson.DimWebLink.FirstOrDefault(d => d.LinkLabel == researchUrl.UrlName && d.Url == researchUrl.Url && d.SourceId == Constants.SourceIdentifiers.ORCID);
                if (dimWebLink == null)
                {
                    dimWebLink = new DimWebLink()
                    {
                        Url = researchUrl.Url,
                        LinkLabel = researchUrl.UrlName,
                        DimOrganizationId = -1,
                        DimKnownPersonId = dimPid.DimKnownPersonId,
                        DimCallProgrammeId = -1,
                        DimFundingDecisionId = -1,
                        //SourceId = Constants.SourceIdentifiers.ORCID,
                        Created = DateTime.Now,
                    };
                    _ttvContext.DimWebLink.Add(dimWebLink);
                }
                else
                {
                    dimWebLink.Modified = DateTime.Now;
                }
                await _ttvContext.SaveChangesAsync();

                // Check if FactFieldValues already exists for the web link. If yes, then related DimFieldDisplaySetting must also already exist.
                if (dimWebLink.FactFieldValues.Count == 0)
                {
                    // Create DimFieldDisplaySetting for weblink
                    var dimFieldDisplaySettings = new DimFieldDisplaySettings()
                    {
                        DimUserProfileId = dimPid.DimKnownPerson.DimUserProfile.First().Id,
                        FieldIdentifier = Constants.FieldIdentifiers.WEB_LINK,
                        Show = false,
                        //SourceId = Constants.SourceIdentifiers.ORCID,
                        Created = DateTime.Now,
                    };
                    _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySettings);
                    await _ttvContext.SaveChangesAsync();

                    // Create FactFieldValues for weblink
                    var factFieldValues = new FactFieldValues()
                    {
                        DimPid = dimPid,
                        DimUserProfileId = dimPid.DimKnownPerson.DimUserProfile.First().Id,
                        DimFieldDisplaySettingsId = dimFieldDisplaySettings.Id,
                        DimWebLinkId = dimWebLink.Id,
                        DimFundingDecisionId = -1,
                        DimNameId = -1,
                        DimPublicationId = -1,
                        //SourceId = Constants.SourceIdentifiers.ORCID,
                        Created = DateTime.Now,
                    };
                    _ttvContext.FactFieldValues.Add(factFieldValues);
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

            return Ok(json);
        }
    }
}