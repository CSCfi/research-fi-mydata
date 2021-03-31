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
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Get ORCID ID
            var orcidId = this.GetOrcidId();

            var dimPid = await _ttvContext.DimPids
                .Include(i => i.DimKnownPerson)
                    .ThenInclude(kp => kp.DimUserProfiles).AsNoTracking().FirstOrDefaultAsync(p => p.PidContent == orcidId);

            if (dimPid == null || dimPid.DimKnownPerson == null || dimPid.DimKnownPerson.DimUserProfiles.Count() == 0)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            return Ok(new ApiResponse(success: true));
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

            return Ok(new ApiResponse(success: true));
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
                            .ThenInclude(factFieldValues => factFieldValues.DimPidIdOrcidPutCodeNavigation)
                .Include(pid => pid.DimKnownPerson)
                    .ThenInclude(knownPerson => knownPerson.DimUserProfiles)
                        .ThenInclude(userProfile => userProfile.FactFieldValues)
                            .ThenInclude(factFieldValues => factFieldValues.DimName)
                .Include(pid => pid.DimKnownPerson)
                    .ThenInclude(knownPerson => knownPerson.DimUserProfiles)
                        .ThenInclude(userProfile => userProfile.FactFieldValues)
                            .ThenInclude(factFieldValues => factFieldValues.DimWebLink)
                .Include(pid => pid.DimKnownPerson)
                    .ThenInclude(knownPerson => knownPerson.DimUserProfiles)
                        .ThenInclude(userProfile => userProfile.DimFieldDisplaySettings)
                .Where(pid => pid.PidContent == orcidId).FirstOrDefaultAsync();

            // Check that user profile exists and remove related items
            if (dimPid != null && dimPid.DimKnownPerson.DimUserProfiles != null && dimPid.DimKnownPerson.DimUserProfiles.FirstOrDefault() != null)
            {
                foreach (FactFieldValue ffv in dimPid.DimKnownPerson.DimUserProfiles.First().FactFieldValues)
                {
                    // Remove ORCID put codes
                    if (ffv.DimPidIdOrcidPutCode != -1)
                    {
                        _ttvContext.FactFieldValues.Remove(ffv);
                        _ttvContext.DimPids.Remove(ffv.DimPidIdOrcidPutCodeNavigation);
                    }

                    // Remove names
                    if (ffv.DimNameId != -1)
                    {
                        // DimName can have several related FactFieldValues (for first name, last name, etc). Remove them all.

                        // TODO: Check if all FactFieldValues and DimName can be removed, or should only current FactFieldValue
                        // be removed and DimName only when it does not have any related FactFieldValues left?
                        _ttvContext.FactFieldValues.RemoveRange(ffv.DimName.FactFieldValues);
                        _ttvContext.DimNames.Remove(ffv.DimName);
                    }

                    // Remove web links
                    else if (ffv.DimWebLinkId != -1)
                    {
                        _ttvContext.FactFieldValues.Remove(ffv);
                        _ttvContext.DimWebLinks.Remove(ffv.DimWebLink);
                    }
                }
                await _ttvContext.SaveChangesAsync();


                // Remove DimFieldDisplaySettings
                _ttvContext.DimFieldDisplaySettings.RemoveRange(dimPid.DimKnownPerson.DimUserProfiles.First().DimFieldDisplaySettings);

                // Remove DimUserProfile
                _ttvContext.DimUserProfiles.Remove(dimPid.DimKnownPerson.DimUserProfiles.First());

                // TODO: Should DimKnownPerson be removed?
                // TODO: Should DimPid be removed?

                await _ttvContext.SaveChangesAsync();
            }


            return Ok(new ApiResponse(success: true));
        }
    }
}