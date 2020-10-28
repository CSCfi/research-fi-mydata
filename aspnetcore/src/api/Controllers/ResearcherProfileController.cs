using api.Services;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ResearcherProfileController : ControllerBase
    {
        private readonly OrcidService _orcidService;
        private readonly TtvContext _ttvContext;

        public ResearcherProfileController(OrcidService orcidService, TtvContext ttvContext)
        {
            _orcidService = orcidService;
            _ttvContext = ttvContext;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok();
        }

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
                    DimKnownPerson = new DimKnownPerson(),
                };
                _ttvContext.DimPid.Add(dimPid);
            }
            else if (dimPid.DimKnownPerson == null || dimPid.DimKnownPersonId == -1)
            {
                // DimPid was found but it does not have DimKnownPerson.
                var kp = new DimKnownPerson();
                _ttvContext.DimKnownPerson.Add(kp);
                dimPid.DimKnownPerson = kp;
            }

            await _ttvContext.SaveChangesAsync();

            // Add DimUserProfile
            if (dimPid.DimKnownPerson.DimUserProfile.Count() == 0)
            {
                var userprofile = new DimUserProfile();
                userprofile.DimKnownPerson = dimPid.DimKnownPerson;
                _ttvContext.DimUserProfile.Add(userprofile);
            }

            await _ttvContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            // Get ORCID ID from user claims
            var orcid = User.Claims.FirstOrDefault(x => x.Type == "orcid")?.Value;

            // Get DimPid with related DimKnownPerson, DimUserProfile and DimFieldDisplaySettings
            var dimPid = await _ttvContext.DimPid
                .Include(i => i.DimKnownPerson)
                  .ThenInclude(i => i.DimUserProfile)
                    .ThenInclude(i => i.DimFieldDisplaySettings).FirstOrDefaultAsync(p => p.PidContent == orcid);

            if (dimPid != null)
            {
                // Remove DimFieldDisplaySettings and DimUserProfile
                if (dimPid.DimKnownPerson != null && dimPid.DimKnownPerson.DimUserProfile.Count() > 0)
                {
                    // Remove DimFieldDisplaySettings
                    _ttvContext.DimFieldDisplaySettings.RemoveRange(dimPid.DimKnownPerson.DimUserProfile.First().DimFieldDisplaySettings);

                    // Remove DimUserProfile
                    _ttvContext.DimUserProfile.RemoveRange(dimPid.DimKnownPerson.DimUserProfile);
                }

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