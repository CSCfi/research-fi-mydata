using api.Services;
using api.Models;
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

            // Get DimPid with related DimKnownPerson and DimUserProfile
            var dimPid = await _ttvContext.DimPid
                .Include(i => i.DimKnownPerson)
                  .ThenInclude(i => i.DimUserProfile).FirstOrDefaultAsync(p => p.PidContent == orcidId);

            // Return if pid was not found
            if (dimPid == null)
            {
                return NotFound();
            }

            // Get record JSON from ORCID
            var json = await _orcidApiService.GetJson(orcidId);

            // Researcher urls
            var researcherUrls = _orcidJsonParserService.GetResearcherUrls(json);

            foreach ((string UrlName, string Url) researchUrl in researcherUrls)
            {
                // Create new weblink
                var dimWebLink = new DimWebLink()
                {
                    Url = researchUrl.Url,
                    LinkLabel = researchUrl.UrlName,
                    DimOrganizationId = -1,
                    DimKnownPersonId = dimPid.DimKnownPersonId,
                    DimCallProgrammeId = -1,
                    DimFundingDecisionId = -1,
                    SourceId = "orcid",
                    Created = DateTime.Now,
                };
                _ttvContext.DimWebLink.Add(dimWebLink);
                await _ttvContext.SaveChangesAsync();

                // Create DimFieldDisplaySetting
                var dimFieldDisplaySettings = new DimFieldDisplaySettings()
                {
                    DimUserProfileId = dimPid.DimKnownPerson.DimUserProfile.First().Id,
                    FieldIdentifier = 1,
                    Show = false,
                    SourceId = "orcid",
                    Created = DateTime.Now,
                };
                _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySettings);
                await _ttvContext.SaveChangesAsync();

                // Create FactFieldDisplayContent
                var factFieldDisplayContent = new FactFieldDisplayContent()
                {
                    DimUserProfileId = dimPid.DimKnownPerson.DimUserProfile.First().Id,
                    DimFieldDisplaySettingsId = dimFieldDisplaySettings.Id,
                    DimWebLinkId = dimWebLink.Id,
                    DimPidPidContent = " ",
                    DimFundingDecisionId = -1,
                    DimNameId = -1,
                    DimPublicationId = -1,
                    SourceId = "orcid",
                    Created = DateTime.Now,
                };
                _ttvContext.FactFieldDisplayContent.Add(factFieldDisplayContent);
                await _ttvContext.SaveChangesAsync();
            }
            
            // Biography
            //var biography = _orcidJsonParserService.GetBiography(json);
            //dimPid.DimKnownPerson.ResearchDescription = biography;
            
            await _ttvContext.SaveChangesAsync();

            return Ok(json);
        }
    }
}