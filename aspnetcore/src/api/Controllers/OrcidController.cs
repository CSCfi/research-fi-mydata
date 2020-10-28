using api.Services;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace api.Controllers
{
    [Route("api/orcid")]
    [ApiController]
    [Authorize]
    public class OrcidController : ControllerBase
    {
        private readonly OrcidService _orcidService;
        private readonly TtvContext _ttvContext;

        public OrcidController(OrcidService orcidService, TtvContext ttvContext)
        {
            _orcidService = orcidService;
            _ttvContext = ttvContext;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            // Get ORCID ID from user claims
            var orcid = User.Claims.FirstOrDefault(x => x.Type == "orcid")?.Value;



            //// Create entry into table DimKnownPerson
            //var knownPerson = new DimKnownPerson();
            //_ttvContext.DimKnownPerson.Add(knownPerson);
            //await _ttvContext.SaveChangesAsync();



            //// Create entry into table DimPid
            //var pid = new DimPid()
            //{
            //    PidContent = orcid,
            //    DimKnownPersonId = knownPerson.Id,
            //};
            //_ttvContext.DimPid.Add(pid);
            //await _ttvContext.SaveChangesAsync();



            //// Create entry into table DimUserProfile
            //var userprofile = new DimUserProfile();
            //userprofile.DimKnownPerson = knownPerson;
            //userprofile.DimKnownPerson = knownPerson;
            //_ttvContext.DimUserProfile.Add(userprofile);
            //await _ttvContext.SaveChangesAsync();

            // Get record JSON from ORCID and return it as response
            return await _orcidService.GetRecord(orcid);
        }
    }

    [Route("api/weblink")]
    [ApiController]
    [Authorize]
    public class WeblinkController : ControllerBase
    {
        private readonly TtvContext _ttvContext;

        public WeblinkController(TtvContext ttvContext)
        {
            _ttvContext = ttvContext;
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            // Get ORCID ID from user claims
            var orcid = User.Claims.FirstOrDefault(x => x.Type == "orcid")?.Value;

            // Get DimPid with related DimKnownPerson and DimUserProfile
            var dimPid = await _ttvContext.DimPid
                .Include(i => i.DimKnownPerson)
                  .ThenInclude(i => i.DimUserProfile).AsNoTracking().FirstOrDefaultAsync(p => p.PidContent == orcid);

            // Return if pid was not found
            if (dimPid == null)
            {
                return NotFound();
            }

            // Create new weblink
            var dimWebLink = new DimWebLink()
            {
                Url = "https://www.csc.fi",
                LinkLabel = "CSC",
                DimOrganizationId = -1,
                DimKnownPersonId = dimPid.DimKnownPersonId,
                DimCallProgrammeId = -1,
                DimFundingDecisionId = -1,
                SourceId = "ORCID",
                SourceDescription = "Researcher profile API",
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
                SourceId = "ORCID",
                SourceDescription = "Researcher profile API",
                Created = DateTime.Now,
            };
            _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySettings);
            await _ttvContext.SaveChangesAsync();

            //// Create FactFieldDisplayContent
            //var factFieldDisplayContent = new FactFieldDisplayContent()
            //{
            //    DimUserProfileId = dimUserProfile.Id,
            //    DimFieldDisplaySettingsId = dimFieldDisplaySettings.Id,
            //    DimWebLinkId = dimWebLink.Id,
            //    SourceId = "ORCID",
            //    SourceDescription = "Researcher profile API",
            //    Created = DateTime.Now,
            //};
            //_ttvContext.FactFieldDisplayContent.Add(factFieldDisplayContent);
            //await _ttvContext.SaveChangesAsync();

            return Ok();
        }
    }
}