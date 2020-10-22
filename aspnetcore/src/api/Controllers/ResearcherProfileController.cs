using api.Services;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<string> Create()
        {
            // Get ORCID ID from user claims
            var orcid = User.Claims.FirstOrDefault(x => x.Type == "orcid")?.Value;

            // Create entry into table DimKnownPerson
            var knownPerson = new DimKnownPerson();
            _ttvContext.DimKnownPerson.Add(knownPerson);
            // Save immediately to get knownPerson.Id
            await _ttvContext.SaveChangesAsync();


            // Create entry into table DimPid
            var pid = new DimPid()
            {
                PidContent = orcid,
                DimKnownPersonId = knownPerson.Id,
            };
            _ttvContext.DimPid.Add(pid);


            // Create entry into table DimUserProfile
            var userprofile = new DimUserProfile();
            userprofile.DimKnownPerson = knownPerson;
            _ttvContext.DimUserProfile.Add(userprofile);

            await _ttvContext.SaveChangesAsync();

            return "OK";
        }

        [HttpDelete]
        public async Task<string> Delete()
        {
            // Get ORCID ID from user claims
            var orcid = User.Claims.FirstOrDefault(x => x.Type == "orcid")?.Value;

            // Get DimPid
            var dimPid = await _ttvContext.DimPid.FindAsync(orcid);

            // Get DimKnownPerson
            var dimKnownPerson = _ttvContext.DimKnownPerson.FirstOrDefault(x => x.Id == dimPid.DimKnownPersonId);

            // Get DimUserProfile
            var dimUserProfile = _ttvContext.DimUserProfile.FirstOrDefault(x => x.DimKnownPersonId == dimPid.DimKnownPersonId);

            // Remove DimUserProfile
            _ttvContext.DimUserProfile.Remove(dimUserProfile);

            // Remove DimPid
            _ttvContext.DimPid.Remove(dimPid);

            // Remove DimKnownPerson
            _ttvContext.DimKnownPerson.Remove(dimKnownPerson);

            await _ttvContext.SaveChangesAsync();

            return "OK";
        }
    }
}