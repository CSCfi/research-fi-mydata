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

            // DimPid was not found
            if (dimPid == null)
            {
                return NotFound();
            }

            // DimKnownPerson was not found
            if (dimPid.DimKnownPerson == null)
            {
                return NotFound();
            }

            // DimUserProfile was not found
            if (dimPid.DimKnownPerson.DimUserProfile.Count() == 0)
            {
                return NotFound();
            }

            // Collect data from DimFieldDisplaySettings and FactFieldDisplayContent entities
            var itemList = new List<ProfileEditorItem> { };
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

                // FieldIdentifier defines what type of data the field contains.
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



        // PATCH: api/ProfileData/
        [HttpPatch]
        public async Task<IActionResult> PatchMany([FromBody] List<ProfileEditorModificationItem> profileEditorModificationItemList)
        {
            // Return immediately if there is nothing to change.
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

            // Check that DimPid exists
            if (dimPid == null)
            {
                return NotFound();
            }

            // Check that DimKnownPerson exists
            if (dimPid.DimKnownPerson == null)
            {
                return NotFound();
            }

            // Check that DimUserProfile exists
            if (dimPid.DimKnownPerson.DimUserProfile.Count() == 0)
            {
                return NotFound();
            }

            // Check that DimFieldDisplaySettings exist
            if (dimPid.DimKnownPerson.DimUserProfile.First().DimFieldDisplaySettings.Count == 0)
            {
                return NotFound();
            }

            // Set DimFieldDisplaySettings property Show according to request data
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