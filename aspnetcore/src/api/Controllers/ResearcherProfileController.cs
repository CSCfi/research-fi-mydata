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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ResearcherProfileController : ControllerBase
    {
        private readonly OrcidApiService _orcidApiService;
        private readonly TtvContext _ttvContext;

        public ResearcherProfileController(OrcidApiService orcidApiService, TtvContext ttvContext)
        {
            _orcidApiService = orcidApiService;
            _ttvContext = ttvContext;
        }

        // Check if profile exists.
        // Returns 200 if profile exists.
        // Otherwise returns 404.
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Get ORCID ID from user claims.
            var orcid = User.Claims.FirstOrDefault(x => x.Type == "orcid")?.Value;

            var dimPid = await _ttvContext.DimPid
                .Include(i => i.DimKnownPerson)
                    .ThenInclude(kp => kp.DimUserProfile).AsNoTracking().FirstOrDefaultAsync(p => p.PidContent == orcid);

            if (dimPid == null)
            {
                return NotFound();
            }

            if (dimPid.DimKnownPerson == null)
            {
                return NotFound();
            }

            if (dimPid.DimKnownPerson.DimUserProfile.Count() == 0)
            {
                return NotFound();
            }

            return Ok();
        }

        // Create profile
        [HttpPost]
        public async Task<IActionResult> Create()
        {
            // Get ORCID ID from user claims.
            var orcid = User.Claims.FirstOrDefault(x => x.Type == "orcid")?.Value;

            // Check if DimPid and DimKnownPerson already exist.
            var dimPid = await _ttvContext.DimPid
                .Include(i => i.DimKnownPerson)
                .ThenInclude(i => i.DimUserProfile).AsNoTracking().FirstOrDefaultAsync(p => p.PidContent == orcid);

            if (dimPid == null)
            {
                // DimPid was not found.

                //// Add new DimKnownPerson before adding DimPid.
                //var kp = new DimKnownPerson();
                //_ttvContext.DimKnownPerson.Add(kp);

                // Add new DimPid.
                dimPid = new DimPid()
                {
                    PidContent = orcid,
                    PidType = "orcid",
                    DimKnownPerson = new DimKnownPerson(){ SourceId = "orcid", Created = DateTime.Now },
                    SourceId = "orcid",
                    DimOrganizationId = -1,
                    DimPublicationId = -1,
                    DimServiceId = -1,
                    DimInfrastructureId = -1,
                    Created = DateTime.Now
                };
                _ttvContext.DimPid.Add(dimPid);
            }
            else if (dimPid.DimKnownPerson == null || dimPid.DimKnownPersonId == -1)
            {
                // DimPid was found but it does not have DimKnownPerson.
                var kp = new DimKnownPerson() { SourceId = "orcid", Created = DateTime.Now };
                _ttvContext.DimKnownPerson.Add(kp);
                dimPid.DimKnownPerson = kp;
            }

            await _ttvContext.SaveChangesAsync();

            // Add DimUserProfile
            if (dimPid.DimKnownPerson.DimUserProfile.Count() == 0)
            {
                var userprofile = new DimUserProfile() { SourceId = "orcid", Created = DateTime.Now };
                userprofile.DimKnownPerson = dimPid.DimKnownPerson;
                _ttvContext.DimUserProfile.Add(userprofile);
            }

            await _ttvContext.SaveChangesAsync();

            return Ok();
        }

        // Delete profile
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            // Get ORCID ID from user claims
            var orcid = User.Claims.FirstOrDefault(x => x.Type == "orcid")?.Value;

            // Get DimPid with related DimKnownPerson, DimUserProfile and DimFieldDisplaySettings
            var dimPid = await _ttvContext.DimPid
                .Include(i => i.DimKnownPerson)
                    .ThenInclude(kp => kp.DimUserProfile)
                        .ThenInclude(up => up.DimFieldDisplaySettings)
                    .ThenInclude(kp => kp.DimUserProfile)
                        .ThenInclude(up => up.FactFieldDisplayContent)
                .Include(i => i.DimKnownPerson)
                    .ThenInclude(kp => kp.DimNameDimKnownPersonIdConfirmedIdentityNavigation)
                .Include(i => i.DimKnownPerson)
                    .ThenInclude(kp => kp.DimWebLink).FirstOrDefaultAsync(p => p.PidContent == orcid);


            if (dimPid != null)
            {
                // Remove DimFieldDisplaySettings and DimUserProfile
                if (dimPid.DimKnownPerson != null && dimPid.DimKnownPerson.DimUserProfile.Count() > 0)
                {
                    // Remove FactFieldDisplayContent
                    _ttvContext.FactFieldDisplayContent.RemoveRange(dimPid.DimKnownPerson.DimUserProfile.First().FactFieldDisplayContent);

                    // Remove DimFieldDisplaySettings
                    _ttvContext.DimFieldDisplaySettings.RemoveRange(dimPid.DimKnownPerson.DimUserProfile.First().DimFieldDisplaySettings);

                    // Remove DimUserProfile
                    _ttvContext.DimUserProfile.RemoveRange(dimPid.DimKnownPerson.DimUserProfile);
                }

                // Remove DimWebLink
                _ttvContext.DimWebLink.RemoveRange(dimPid.DimKnownPerson.DimWebLink);

                // Remove DimName
                _ttvContext.DimName.RemoveRange(dimPid.DimKnownPerson.DimNameDimKnownPersonIdConfirmedIdentityNavigation);

                // Remove DimPid
                _ttvContext.DimPid.Remove(dimPid);

                // Remove DimKnownPerson
                _ttvContext.DimKnownPerson.Remove(dimPid.DimKnownPerson);

                await _ttvContext.SaveChangesAsync();
            }
               
            return Ok();
        }
    }
}