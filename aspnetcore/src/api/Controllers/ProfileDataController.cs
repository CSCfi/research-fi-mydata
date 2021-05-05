using api.Services;
using api.Models;
using api.Models.Ttv;
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
    public class ProfileDataController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;

        public ProfileDataController(TtvContext ttvContext, UserProfileService userProfileService)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Get userprofile
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Get DimUserProfile and related entities
            var dimUserProfile = await _ttvContext.DimUserProfiles
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.BrFieldDisplaySettingsDimRegisteredDataSources)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimName)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimWebLink)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPublication)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPidIdOrcidPutCodeNavigation)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearchActivity)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEvent)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEducation)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimCompetence)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearchCommunity)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimTelephoneNumber)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEmailAddrress)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearcherDescription)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimIdentifierlessData)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimWebLink).AsSplitQuery().FirstOrDefaultAsync(up => up.Id == userprofileId);



            // Collect data from DimFieldDisplaySettings and FactFieldValues entities
            var itemList = new List<ProfileEditorItem> { };
            foreach (DimFieldDisplaySetting ds in dimUserProfile.DimFieldDisplaySettings)
            {
                var item = new ProfileEditorItem()
                {
                    Id = ds.Id,
                    FieldIdentifier = ds.FieldIdentifier,
                    Show = ds.Show,
                    //SourceId = ds.,
                    Name = null,
                    WebLink = null
                };

                // FieldIdentifier defines what type of data the field contains.
                switch (ds.FieldIdentifier)
                {
                    case Constants.FieldIdentifiers.FIRST_NAMES:
                        item.Name = ds.FactFieldValues.First().DimName.FirstNames;
                        break;
                    case Constants.FieldIdentifiers.LAST_NAME:
                        item.Name = ds.FactFieldValues.First().DimName.LastName;
                        break;
                    // case Constants.FieldIdentifiers.RESEARCHER_DESCRIPTION:
                    case Constants.FieldIdentifiers.WEB_LINK:
                        item.WebLink = new ProfileEditorWebLink()
                        {
                            Url = ds.FactFieldValues.First().DimWebLink.Url,
                            UrlLabel = ds.FactFieldValues.First().DimWebLink.LinkLabel
                        };
                        break;
                    default:
                        break;
                }

                itemList.Add(item);
            }

            return Ok(new ApiResponse(data: itemList));
        }



        // PATCH: api/ProfileData/
        [HttpPatch]
        public async Task<IActionResult> PatchMany([FromBody] List<ProfileEditorModificationItem> profileEditorModificationItemList)
        {
            // Return immediately if there is nothing to change.
            if (profileEditorModificationItemList.Count == 0)
            {
                return Ok(new ApiResponse(success: true));
            }

            // Get ORCID ID
            var orcidId = this.GetOrcidId();

            // Get DimPid and related DimFieldDisplaySetting entities
            var dimPid = await _ttvContext.DimPids
                .Include(i => i.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimUserProfiles)
                        .ThenInclude(dup => dup.DimFieldDisplaySettings).AsSplitQuery().FirstOrDefaultAsync(i => i.PidContent == orcidId);

            // Check that DimPid, DimKnownPerson, DimUserProfile and DimFieldDisplaySettings exist
            if (dimPid == null || dimPid.DimKnownPerson == null || dimPid.DimKnownPerson.DimUserProfiles.Count() == 0 || dimPid.DimKnownPerson.DimUserProfiles.First().DimFieldDisplaySettings.Count == 0)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Set DimFieldDisplaySettings property Show according to request data
            var responseProfileEditorModificationItemList = new List<ProfileEditorModificationItem> { };
            foreach (ProfileEditorModificationItem profileEditorModificationItem in profileEditorModificationItemList)
            {
                var dimFieldDisplaySettings = dimPid.DimKnownPerson.DimUserProfiles.First().DimFieldDisplaySettings.Where(d => d.Id == profileEditorModificationItem.Id).FirstOrDefault();
                if (dimFieldDisplaySettings != null)
                {
                    dimFieldDisplaySettings.Show = profileEditorModificationItem.Show;
                    responseProfileEditorModificationItemList.Add(profileEditorModificationItem);
                }
            }

            await _ttvContext.SaveChangesAsync();

            return Ok(new ApiResponse(data: responseProfileEditorModificationItemList));
        }
    }
}