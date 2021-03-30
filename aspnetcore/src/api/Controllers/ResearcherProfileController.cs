using api.Services;
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
    public class ResearcherProfileController : TtvControllerBase
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
            // Get ORCID ID
            var orcidId = this.GetOrcidId();

            var dimPid = await _ttvContext.DimPids
                .Include(i => i.DimKnownPerson)
                    .ThenInclude(kp => kp.DimUserProfiles).AsNoTracking().FirstOrDefaultAsync(p => p.PidContent == orcidId);

            if (dimPid == null)
            {
                return NotFound();
            }

            if (dimPid.DimKnownPerson == null)
            {
                return NotFound();
            }

            if (dimPid.DimKnownPerson.DimUserProfiles.Count() == 0)
            {
                return NotFound();
            }

            return Ok();
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
                .ThenInclude(i => i.DimUserProfiles).AsNoTracking().FirstOrDefaultAsync(p => p.PidContent == orcidId);

            if (dimPid == null)
            {
                // DimPid was not found.

                // Add new DimPid, add new DimKnownPerson
                dimPid = new DimPid()
                {
                    PidContent = orcidId,
                    PidType = "orcid",
                    DimKnownPerson = new DimKnownPerson(){ Created = DateTime.Now },
                    Created = DateTime.Now
                };
                _ttvContext.DimPids.Add(dimPid);
                await _ttvContext.SaveChangesAsync();
            }
            else if (dimPid.DimKnownPerson == null || dimPid.DimKnownPersonId == -1)
            {
                // DimPid was found but it does not have DimKnownPerson.
                var kp = new DimKnownPerson() { Created = DateTime.Now };
                _ttvContext.DimKnownPeople.Add(kp);
                dimPid.DimKnownPerson = kp;
                await _ttvContext.SaveChangesAsync();
            }


            // Add DimUserProfile
            if (dimPid.DimKnownPerson.DimUserProfiles.FirstOrDefault() == null)
            {
                var userprofile = new DimUserProfile() {
                    DimKnownPersonId = dimPid.DimKnownPerson.Id,
                    Created = DateTime.Now,
                    AllowAllSubscriptions = false
                };
                _ttvContext.DimUserProfiles.Add(userprofile);
                await _ttvContext.SaveChangesAsync();
            }

            return Ok();
        }

        // Delete profile
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            // Get ORCID ID
            var orcidId = this.GetOrcidId();

            // Get DimPid with related DimKnownPerson, DimUserProfile and DimFieldDisplaySettings

            var dimPid = await _ttvContext.DimPids
                .Include(pid => pid.DimKnownPerson)
                    .ThenInclude(knownPerson => knownPerson.DimUserProfiles)
                        .ThenInclude(userProfile => userProfile.FactFieldValues)
                            .ThenInclude(factFieldValues => factFieldValues.DimName)
                        .ThenInclude(userProfile => userProfile.FactFieldValues)
                            .ThenInclude(factFieldValues => factFieldValues.DimWebLink)
                         .ThenInclude(userProfile => userProfile.FactFieldValues)
                            .ThenInclude(factFieldValues => factFieldValues.DimPidIdOrcidPutCode)
                .Where(pid => pid.PidContent == orcidId).FirstOrDefaultAsync();

            // Check that user profile exists and remove related items
            if (dimPid != null && dimPid.DimKnownPerson.DimUserProfiles != null && dimPid.DimKnownPerson.DimUserProfiles.FirstOrDefault() != null)
            {
                _ttvContext.FactFieldValues.RemoveRange(dimPid.DimKnownPerson.DimUserProfiles.First().FactFieldValues);

                /*
                foreach (FactFieldValues ffv in dimPid.DimKnownPerson.DimUserProfile.First().FactFieldValues)
                {
                    
                    // Store reference to related items
                    var orcidPutCode = ffv.DimPidIdOrcidPutCodeNavigation;
                    var name = ffv.DimName;
                    var weblink = ffv.DimWebLink;

                    // Remove FactFieldValue relation
                    ffv.DimPidIdOrcidPutCodeNavigation = null;
                    ffv.DimNameId = -1;
                    ffv.DimWebLinkId = -1;
                    await _ttvContext.SaveChangesAsync();

                    // Remove FactFieldValue
                    _ttvContext.FactFieldValues.Remove(ffv);

                    // Remove related ORCID put code items in table DimPid
                    if (orcidPutCode != null)
                    {
                        _ttvContext.DimPid.Remove(orcidPutCode);
                    }

                    // Remove name
                    if (name != null)
                    {
                        _ttvContext.DimName.Remove(name);
                    }

                    // Remove web links
                    if (weblink != null)
                    {
                        _ttvContext.DimWebLink.Remove(weblink);
                    }

                    // TODO remove other related items
                    
                }
                */

                // Remove DimFieldDisplaySettings
                _ttvContext.DimFieldDisplaySettings.RemoveRange(dimPid.DimKnownPerson.DimUserProfiles.First().DimFieldDisplaySettings);

                // Remove DimUserProfile
                _ttvContext.DimUserProfiles.Remove(dimPid.DimKnownPerson.DimUserProfiles.First());

                //await _ttvContext.SaveChangesAsync();
            }

               
            return Ok();
        }
    }
}