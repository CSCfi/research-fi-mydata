using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileDataController : ControllerBase
    {
        private readonly TtvContext _ttvContext;

        public ProfileDataController(TtvContext ttvContext)
        {
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
                .Include(i => i.DimKnownPerson)
                    .ThenInclude(i => i.DimUserProfile)
                        .ThenInclude(i => i.FactFieldDisplayContent)
                            .ThenInclude(i => i.DimWebLink)
                .Include(i => i.DimKnownPerson)
                    .ThenInclude(i => i.DimUserProfile)
                        .ThenInclude(i => i.FactFieldDisplayContent)
                            .ThenInclude(i => i.DimName)
                .Include(i => i.DimKnownPerson)
                  .ThenInclude(i => i.DimNameDimKnownPersonIdConfirmedIdentityNavigation).FirstOrDefaultAsync(i => i.PidContent == orcidId);

            // Return if pid was not found
            if (dimPid == null)
            {
                return NotFound();
            }

            var itemList = new List<ProfileEditorItem> { };

            // Collect data from DimFieldDisplaySettings and FactFieldDisplayContent entities
            foreach (DimFieldDisplaySettings ds in dimPid.DimKnownPerson.DimUserProfile.First().DimFieldDisplaySettings)
            {
                var item = new ProfileEditorItem()
                {
                    Id = ds.Id,
                    FieldIdentifier = ds.FieldIdentifier,
                    Show = ds.Show,
                    SourceId = ds.SourceId,
                    Name = null,
                    WebLink = null
                };

                switch (ds.FieldIdentifier)
                {
                    case Constants.FieldIdentifiers.FIRST_NAMES:
                        item.Name = ds.FactFieldDisplayContent.First().DimName.FirstNames;
                        break;
                    case Constants.FieldIdentifiers.LAST_NAME:
                        item.Name = ds.FactFieldDisplayContent.First().DimName.LastName;
                        break;
                    case Constants.FieldIdentifiers.WEB_LINK:
                        item.WebLink = new ProfileEditorWebLink()
                        {
                            Url = ds.FactFieldDisplayContent.First().DimWebLink.Url,
                            UrlLabel = ds.FactFieldDisplayContent.First().DimWebLink.LinkLabel
                        };
                        break;
                    default:
                        break;
                }

                itemList.Add(item);
            }

            return Ok(itemList);
        }


        // PATCH: api/ProfileData/5
        [HttpPatch("{dimFieldDisplaySettingsId}")]
        public async Task<IActionResult> Patch(int dimFieldDisplaySettingsId, [FromBody] ProfileEditorModificationItem profileEditorModificationItem)
        {
            if (dimFieldDisplaySettingsId == -1)
            {
                return BadRequest();
            }

            // Get ORCID ID from user claims
            var orcidId = User.Claims.FirstOrDefault(x => x.Type == "orcid")?.Value;

            var dimFieldDisplaySettings = await _ttvContext.DimFieldDisplaySettings
                .Include(fds => fds.DimUserProfile)
                    .ThenInclude(dup => dup.DimKnownPerson)
                        .ThenInclude(dkp => dkp.DimPid)
                .FirstOrDefaultAsync(fds => fds.Id == dimFieldDisplaySettingsId);

            if (dimFieldDisplaySettings == null)
            {
                return NotFound();
            }

            var pidFound = false;
            foreach (DimPid pid in dimFieldDisplaySettings.DimUserProfile.DimKnownPerson.DimPid)
            {
                if (pid.PidContent == orcidId)
                    pidFound = true;
            }

            if (!pidFound)
            {
                return Unauthorized();
            }

            dimFieldDisplaySettings.Show = profileEditorModificationItem.Show;
            await _ttvContext.SaveChangesAsync();

            return Ok();
        }



        // PATCH: api/ProfileData/
        [HttpPatch]
        public async Task<IActionResult> PatchMany([FromBody] List<ProfileEditorModificationItem> profileEditorModificationItemList)
        {

            if (profileEditorModificationItemList.Count == 0)
            {
                return Ok();
            }

            // Get ORCID ID from user claims
            var orcidId = User.Claims.FirstOrDefault(x => x.Type == "orcid")?.Value;

            

            // Get DimPid and related DimFieldDisplaySetting entities
            var dimPid = await _ttvContext.DimPid
                .Include(i => i.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimUserProfile)
                        .ThenInclude(dup => dup.DimFieldDisplaySettings).FirstOrDefaultAsync(i => i.PidContent == orcidId);

            if (dimPid == null)
            {
                return NotFound();
            }

            if (dimPid.DimKnownPerson.DimUserProfile.First().DimFieldDisplaySettings.Count == 0)
            {
                return NotFound();
            }

            var responseProfileEditorModificationItemList = new List<ProfileEditorModificationItem> { };

            foreach (ProfileEditorModificationItem profileEditorModificationItem in profileEditorModificationItemList)
            {
                var dimFieldDisplaySettings = dimPid.DimKnownPerson.DimUserProfile.First().DimFieldDisplaySettings.Where(d => d.Id == profileEditorModificationItem.Id).FirstOrDefault();

                if (dimFieldDisplaySettings != null)
                {
                    dimFieldDisplaySettings.Show = profileEditorModificationItem.Show;
                    responseProfileEditorModificationItemList.Add(profileEditorModificationItem);
                }
            }

            await _ttvContext.SaveChangesAsync();

            return Ok(responseProfileEditorModificationItemList);
        }
    }
}