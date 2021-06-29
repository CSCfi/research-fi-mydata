using api.Services;
using api.Models;
using api.Models.Ttv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace api.Controllers
{
    [Route("api/publication")]
    [ApiController]
    [Authorize]
    public class PublicationController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;

        public PublicationController(TtvContext ttvContext, UserProfileService userProfileService)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;          
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProfileEditorPublicationId profileEditorPublicationId)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse(success: false, reason: "invalid publicationId", data: profileEditorPublicationId));
            }

            // Get userprofile
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                // Userprofile not found
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }
            var dimUserProfile = await _ttvContext.DimUserProfiles
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.BrFieldDisplaySettingsDimRegisteredDataSources)
                        .ThenInclude(br => br.DimRegisteredDataSource)
                            .ThenInclude(drds => drds.DimOrganization).AsNoTracking()
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPublication).AsNoTracking().AsSplitQuery().FirstOrDefaultAsync(dup => dup.Id == userprofileId);

            // Check if userprofile already includes given publication
            foreach (FactFieldValue ffv in dimUserProfile.FactFieldValues)
            {
                if (ffv.DimPublicationId != -1 && ffv.DimPublication.PublicationId == profileEditorPublicationId.PublicationId)
                {
                    return Ok(new ApiResponse(success: true, reason: "publication is already included", data: profileEditorPublicationId));
                }
            }

            // Get DimPublication
            var dimPublication = await _ttvContext.DimPublications.AsNoTracking().FirstOrDefaultAsync(dp => dp.PublicationId == profileEditorPublicationId.PublicationId);
            // Return if DimPublication does not exist
            if (dimPublication == null)
            {
                return Ok(new ApiResponse(success: false, reason: "publication not found", data: profileEditorPublicationId));
            }

            // Get Tiedejatutkimus.fi registered data source id
            var tiedejatutkimusRegisteredDataSourceId = await _userProfileService.GetTiedejatutkimusFiRegisteredDataSourceId();

            // Get DimFieldDisplaySetting for Tiedejatutkimus.fi
            var dimFieldDisplaySettingsPublication = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION && dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSourceId == tiedejatutkimusRegisteredDataSourceId);

            // Add FactFieldValue
            var factFieldValuePublication = _userProfileService.GetEmptyFactFieldValue();
            factFieldValuePublication.DimUserProfileId = dimUserProfile.Id;
            factFieldValuePublication.DimFieldDisplaySettingsId = dimFieldDisplaySettingsPublication.Id;
            factFieldValuePublication.DimPublicationId = dimPublication.Id;
            factFieldValuePublication.SourceId = Constants.SourceIdentifiers.TIEDEJATUTKIMUS;
            factFieldValuePublication.Created = System.DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValuePublication);
            await _ttvContext.SaveChangesAsync();

            // Response
            var publicationGroup = new ProfileEditorGroupPublication()
            {
                source = new ProfileEditorSource()
                {
                    Id = dimFieldDisplaySettingsPublication.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                    RegisteredDataSource = dimFieldDisplaySettingsPublication.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                    Organization = new ProfileEditorSourceOrganization()
                    {
                        NameFi = dimFieldDisplaySettingsPublication.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                        NameEn = dimFieldDisplaySettingsPublication.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                        NameSv = dimFieldDisplaySettingsPublication.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                    }
                },
                items = new List<ProfileEditorItemPublication>() {
                    new ProfileEditorItemPublication()
                    {
                        PublicationId = dimPublication.PublicationId,
                        PublicationName = dimPublication.PublicationName,
                        PublicationYear = dimPublication.PublicationYear,
                        DoiHandle = dimPublication.DoiHandle,
                        itemMeta = new ProfileEditorItemMeta()
                        {
                            Id = dimPublication.Id,
                            Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                            Show = factFieldValuePublication.Show,
                            PrimaryValue = factFieldValuePublication.PrimaryValue
                        }
                    }
                },
                groupMeta = new ProfileEditorGroupMeta()
                {
                    Id = dimFieldDisplaySettingsPublication.Id,
                    Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                    Show = dimFieldDisplaySettingsPublication.Show
                }
            };

            return Ok(new ApiResponse(success: true, data: publicationGroup));
        }


        [HttpDelete("{publicationId}")]
        public async Task<IActionResult> DeletePublicationFromProfile(string publicationId)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse(success: false, reason: "invalid publicationId", data: publicationId));
            }

            // Get userprofile
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                // Userprofile not found
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            var factFieldValue = await _ttvContext.FactFieldValues
                .Include(ffv => ffv.DimPublication).AsNoTracking().AsSplitQuery().FirstOrDefaultAsync(ffv => ffv.DimUserProfileId == userprofileId && ffv.DimPublicationId != -1 && ffv.DimPublication.PublicationId == publicationId);

            if (factFieldValue == null)
            {
                return Ok(new ApiResponse(success: false, reason: "publication not found", data: publicationId));
            }

            _ttvContext.FactFieldValues.Remove(factFieldValue);
            await _ttvContext.SaveChangesAsync();

            return Ok(new ApiResponse(success: true, reason: "removed", data: publicationId));
        }
    }
}