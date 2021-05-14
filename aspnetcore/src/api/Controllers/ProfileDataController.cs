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
                        .ThenInclude(br => br.DimRegisteredDataSource).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimWebLink).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimName).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimWebLink).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimFundingDecision).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimPublication).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimPidIdOrcidPutCodeNavigation).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimResearchActivity).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimEvent).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimEducation).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimCompetence).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearchCommunity).AsNoTracking()
                //.Include(dup => dup.DimFieldDisplaySettings)
                //    .ThenInclude(dfds => dfds.FactFieldValues)
                //        .ThenInclude(ffv => ffv.DimTelephoneNumber).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimEmailAddrress).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimResearcherDescription).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimIdentifierlessData)
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimKeyword).AsNoTracking().AsSplitQuery().FirstOrDefaultAsync(up => up.Id == userprofileId);

            var profileDataResponse = new ProfileEditorDataResponse() {};

            // Collect data from DimFieldDisplaySettings and FactFieldValues entities
            foreach (DimFieldDisplaySetting dfds in dimUserProfile.DimFieldDisplaySettings)
            {
                // FieldIdentifier defines what type of data the field contains.
                switch (dfds.FieldIdentifier)
                {
                    case Constants.FieldIdentifiers.PERSON_FIRST_NAMES:
                        var firstNamesGroup = new ProfileEditorGroupFirstNames()
                        {
                            dataSource = new ProfileEditorDataSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                Name = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                            },
                            items = new List<ProfileEditorItemName>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.PERSON_FIRST_NAMES,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            firstNamesGroup.items.Add(
                                new ProfileEditorItemName()
                                {
                                    Value = ffv.DimName.FirstNames,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimNameId,
                                        Type = Constants.FieldIdentifiers.PERSON_FIRST_NAMES,
                                        Show = ffv.Show
                                    }
                                }
                            );
                        }
                        profileDataResponse.personal.firstNamesGroups.Add(firstNamesGroup);
                        break;
                    case Constants.FieldIdentifiers.PERSON_LAST_NAME:
                        var lastNameGroup = new ProfileEditorGroupLastName()
                        {
                            dataSource = new ProfileEditorDataSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                Name = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                            },
                            items = new List<ProfileEditorItemName>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.PERSON_LAST_NAME,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            lastNameGroup.items.Add(
                                new ProfileEditorItemName()
                                {
                                    Value = ffv.DimName.LastName,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimNameId,
                                        Type = Constants.FieldIdentifiers.PERSON_LAST_NAME,
                                        Show = ffv.Show
                                    }
                                }
                            );
                        }
                        profileDataResponse.personal.lastNameGroups.Add(lastNameGroup);
                        break;
                    case Constants.FieldIdentifiers.PERSON_OTHER_NAMES:
                        var otherNamesGroup = new ProfileEditorGroupOtherNames()
                        {
                            dataSource = new ProfileEditorDataSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                Name = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                            },
                            items = new List<ProfileEditorItemName>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.PERSON_OTHER_NAMES,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            otherNamesGroup.items.Add(
                                new ProfileEditorItemName()
                                {
                                    Value = ffv.DimName.FullName,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimNameId,
                                        Type = Constants.FieldIdentifiers.PERSON_OTHER_NAMES,
                                        Show = ffv.Show
                                    }
                                }
                            );
                        }
                        profileDataResponse.personal.otherNamesGroups.Add(otherNamesGroup);
                        break;
                    case Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION:
                        var researcherDescriptionGroup = new ProfileEditorGroupResearcherDescription()
                        {
                            dataSource = new ProfileEditorDataSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                Name = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                            },
                            items = new List<ProfileEditorItemResearcherDescription>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            researcherDescriptionGroup.items.Add(
                                new ProfileEditorItemResearcherDescription()
                                {
                                    ResearchDescriptionEn = ffv.DimResearcherDescription.ResearchDescriptionEn,
                                    ResearchDescriptionFi = ffv.DimResearcherDescription.ResearchDescriptionFi,
                                    ResearchDescriptionSv = ffv.DimResearcherDescription.ResearchDescriptionSv,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimResearcherDescriptionId,
                                        Type = Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION,
                                        Show = ffv.Show
                                    }
                                }
                            );
                        }
                        profileDataResponse.personal.researcherDescriptionGroups.Add(researcherDescriptionGroup);
                        break;

                    case Constants.FieldIdentifiers.PERSON_WEB_LINK:
                        var webLinkGroup = new ProfileEditorGroupWebLink()
                        {
                            dataSource = new ProfileEditorDataSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                Name = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                            },
                            items = new List<ProfileEditorItemWebLink>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.PERSON_WEB_LINK,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            webLinkGroup.items.Add(
                                new ProfileEditorItemWebLink()
                                {
                                    Url = ffv.DimWebLink.Url,
                                    LinkLabel = ffv.DimWebLink.LinkLabel,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimWebLinkId,
                                        Type = Constants.FieldIdentifiers.PERSON_WEB_LINK,
                                        Show = ffv.Show
                                    }
                                }
                            );
                        }
                        profileDataResponse.personal.webLinkGroups.Add(webLinkGroup);
                        break;

                    case Constants.FieldIdentifiers.PERSON_KEYWORD:
                        var keywordGroup = new ProfileEditorGroupKeyword()
                        {
                            dataSource = new ProfileEditorDataSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                Name = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                            },
                            items = new List<ProfileEditorItemKeyword>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.PERSON_KEYWORD,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            keywordGroup.items.Add(
                                new ProfileEditorItemKeyword()
                                {
                                    Value = ffv.DimKeyword.Keyword,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimKeywordId,
                                        Type = Constants.FieldIdentifiers.PERSON_KEYWORD,
                                        Show = ffv.Show
                                    }
                                }
                            );
                        }
                        profileDataResponse.personal.keywordGroups.Add(keywordGroup);
                        break;
                    default:
                        break;
                }

                //    itemList.Add(item);
            }

            return Ok(new ApiResponse(success: true, reason: "", data: profileDataResponse));
        }



        // PATCH: api/ProfileData/
        [HttpPatch]
        public async Task<IActionResult> PatchMany([FromBody] ProfileEditorDataModificationRequest profileEditorDataModificationRequest)
        {
            // Return immediately if there is nothing to change.
            if (profileEditorDataModificationRequest.groups.Count == 0 && profileEditorDataModificationRequest.items.Count == 0)
            {
                return Ok(new ApiResponse(success: true));
            }

            // Get userprofile
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            var dimUserProfile = await _ttvContext.DimUserProfiles
                .Include(dup => dup.DimFieldDisplaySettings)
                .Include(dup => dup.FactFieldValues).AsSplitQuery().FirstOrDefaultAsync(up => up.Id == userprofileId);


            var profileEditorDataModificationResponse = new ProfileEditorDataModificationResponse();

            // Set 'Show' in DimFieldDisplaySettings
            foreach (ProfileEditorGroupMeta profileEditorGroupMeta in profileEditorDataModificationRequest.groups.ToList())
            {
                var dimFieldDisplaySettings = dimUserProfile.DimFieldDisplaySettings.Where(d => d.Id == profileEditorGroupMeta.Id).FirstOrDefault();
                if (dimFieldDisplaySettings != null)
                {
                    dimFieldDisplaySettings.Show = profileEditorGroupMeta.Show;
                    profileEditorDataModificationResponse.groups.Add(profileEditorGroupMeta);
                }
            }

            // Set 'Show' in FactFieldValues
            foreach (ProfileEditorItemMeta profileEditorItemMeta in profileEditorDataModificationRequest.items.ToList())
            {
                FactFieldValue factFieldValue = null;
                switch (profileEditorItemMeta.Type)
                {
                    case Constants.FieldIdentifiers.PERSON_FIRST_NAMES:
                        factFieldValue = dimUserProfile.FactFieldValues.Where(ffv => ffv.DimNameId == profileEditorItemMeta.Id).FirstOrDefault();
                        break;
                    case Constants.FieldIdentifiers.PERSON_LAST_NAME:
                        factFieldValue = dimUserProfile.FactFieldValues.Where(ffv => ffv.DimNameId == profileEditorItemMeta.Id).FirstOrDefault();
                        break;
                    case Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION:
                        factFieldValue = dimUserProfile.FactFieldValues.Where(ffv => ffv.DimResearcherDescriptionId == profileEditorItemMeta.Id).FirstOrDefault();
                        break;
                    case Constants.FieldIdentifiers.PERSON_WEB_LINK:
                        factFieldValue = dimUserProfile.FactFieldValues.Where(ffv => ffv.DimWebLinkId == profileEditorItemMeta.Id).FirstOrDefault();
                        break;
                    default:
                        break;
                }

                if (factFieldValue != null)
                {
                    factFieldValue.Show = profileEditorItemMeta.Show;
                    profileEditorDataModificationResponse.items.Add(profileEditorItemMeta);
                }
            }

            await _ttvContext.SaveChangesAsync();

            return Ok(new ApiResponse(success: true, reason: "", data: profileEditorDataModificationResponse));
        }



        //// PATCH: api/ProfileData/
        //[HttpPatch]
        //public async Task<IActionResult> PatchMany([FromBody] List<ProfileEditorGroupMeta> profileEditorModificationItemList)
        //{
        //    // Return immediately if there is nothing to change.
        //    if (profileEditorModificationItemList.Count == 0)
        //    {
        //        return Ok(new ApiResponse(success: true));
        //    }

        //    // Get ORCID ID
        //    var orcidId = this.GetOrcidId();

        //    // Get DimPid and related DimFieldDisplaySetting entities
        //    var dimPid = await _ttvContext.DimPids
        //        .Include(i => i.DimKnownPerson)
        //            .ThenInclude(dkp => dkp.DimUserProfiles)
        //                .ThenInclude(dup => dup.DimFieldDisplaySettings).AsSplitQuery().FirstOrDefaultAsync(i => i.PidContent == orcidId);

        //    // Check that DimPid, DimKnownPerson, DimUserProfile and DimFieldDisplaySettings exist
        //    if (dimPid == null || dimPid.DimKnownPerson == null || dimPid.DimKnownPerson.DimUserProfiles.Count() == 0 || dimPid.DimKnownPerson.DimUserProfiles.First().DimFieldDisplaySettings.Count == 0)
        //    {
        //        return Ok(new ApiResponse(success: false, reason: "profile not found"));
        //    }

        //    // Set DimFieldDisplaySettings property Show according to request data
        //    var responseProfileEditorModificationItemList = new List<ProfileEditorGroupMeta> { };
        //    foreach (ProfileEditorGroupMeta profileEditorModificationItem in profileEditorModificationItemList)
        //    {
        //        var dimFieldDisplaySettings = dimPid.DimKnownPerson.DimUserProfiles.First().DimFieldDisplaySettings.Where(d => d.Id == profileEditorModificationItem.Id).FirstOrDefault();
        //        if (dimFieldDisplaySettings != null)
        //        {
        //            dimFieldDisplaySettings.Show = profileEditorModificationItem.Show;
        //            responseProfileEditorModificationItemList.Add(profileEditorModificationItem);
        //        }
        //    }

        //    await _ttvContext.SaveChangesAsync();

        //    return Ok(new ApiResponse(data: responseProfileEditorModificationItemList));
        //}
    }
}