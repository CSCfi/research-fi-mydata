using api.Services;
using api.Models;
using api.Models.Ttv;
using api.Models.Orcid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace api.Controllers
{
    [Route("api/orcid")]
    [ApiController]
    [Authorize]
    public class OrcidController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;
        private readonly OrcidApiService _orcidApiService;
        private readonly OrcidJsonParserService _orcidJsonParserService;

        public OrcidController(TtvContext ttvContext, UserProfileService userProfileService, OrcidApiService orcidApiService, OrcidJsonParserService orcidJsonParserService)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _orcidApiService = orcidApiService;
            _orcidJsonParserService = orcidJsonParserService;            
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Get userprofile
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                // Userprofile not found
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Get record JSON from ORCID
            var json = await _orcidApiService.GetRecord(orcidId);

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

            // Get DimKnownPerson
            var dimKnownPerson = await _ttvContext.DimKnownPeople
                .Include(dkp => dkp.DimNameDimKnownPersonIdConfirmedIdentityNavigations).AsSplitQuery().AsNoTracking().FirstOrDefaultAsync(dkp => dkp.Id == dimUserProfile.DimKnownPersonId);

            // Get ORCID registered data source id
            var orcidRegisteredDataSourceId = await _userProfileService.GetOrcidRegisteredDataSourceId();


            // DimName
            var dimName = await _userProfileService.AddOrUpdateDimName(
                _orcidJsonParserService.GetFamilyName(json).Value,
                _orcidJsonParserService.GetGivenNames(json).Value,
                dimKnownPerson.Id,
                orcidRegisteredDataSourceId
            );

            // LastName: DimFieldDisplaySettings
            var dimFieldDisplaySettingsLastName = dimUserProfile.DimFieldDisplaySettings
                .FirstOrDefault(dimFieldDisplaysettingsLastName => dimFieldDisplaysettingsLastName.FieldIdentifier == Constants.FieldIdentifiers.LAST_NAME && dimFieldDisplaysettingsLastName.BrFieldDisplaySettingsDimRegisteredDataSources.Any(br => br.DimFieldDisplaySettingsId == dimFieldDisplaysettingsLastName.Id && br.DimRegisteredDataSourceId == orcidRegisteredDataSourceId));

            if (dimFieldDisplaySettingsLastName == null)
            {
                dimFieldDisplaySettingsLastName = new DimFieldDisplaySetting()
                {
                    DimUserProfileId = dimUserProfile.Id,
                    FieldIdentifier = Constants.FieldIdentifiers.LAST_NAME,
                    Show = false,
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    Created = DateTime.Now
                };
                dimFieldDisplaySettingsLastName.BrFieldDisplaySettingsDimRegisteredDataSources.Add(
                    new BrFieldDisplaySettingsDimRegisteredDataSource()
                    {
                        DimFieldDisplaySettingsId = dimFieldDisplaySettingsLastName.Id,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId
                    }
                );
                _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySettingsLastName);
            }
            else
            {
                dimFieldDisplaySettingsLastName.Modified = DateTime.Now;
            }
            await _ttvContext.SaveChangesAsync();

            // LastName: FactFieldValues
            var factFieldValuesLastName = dimUserProfile.FactFieldValues.FirstOrDefault(factFieldValuesLastName => factFieldValuesLastName.DimFieldDisplaySettingsId == dimFieldDisplaySettingsLastName.Id);
            if (factFieldValuesLastName == null)
            {
                factFieldValuesLastName = new FactFieldValue()
                {
                    DimPidId = -1,
                    DimUserProfileId = dimUserProfile.Id,
                    DimFieldDisplaySettingsId = dimFieldDisplaySettingsLastName.Id,
                    DimNameId = dimName.Id,
                    DimWebLinkId = -1,
                    DimFundingDecisionId = -1,
                    DimPublicationId = -1,
                    DimPidIdOrcidPutCode = -1,
                    DimResearchActivityId = -1,
                    DimEventId = -1,
                    DimEducationId = -1,
                    DimCompetenceId = -1,
                    DimResearchCommunityId = -1,
                    DimTelephoneNumberId = -1,
                    DimEmailAddrressId = -1,
                    DimResearcherDescriptionId = -1,
                    DimIdentifierlessDataId = -1,
                    Show = false,
                    PrimaryValue = false,
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    Created = DateTime.Now,
                };
                _ttvContext.FactFieldValues.Add(factFieldValuesLastName);
            }
            else
            {
                factFieldValuesLastName.Modified = DateTime.Now;
            }
            await _ttvContext.SaveChangesAsync();


            // FirstNames: DimFieldDisplaySettings
            var dimFieldDisplaySettingsFirstNames = dimUserProfile.DimFieldDisplaySettings
                .FirstOrDefault(dimFieldDisplaysettingsFirstNames => dimFieldDisplaysettingsFirstNames.FieldIdentifier == Constants.FieldIdentifiers.FIRST_NAMES && dimFieldDisplaysettingsFirstNames.BrFieldDisplaySettingsDimRegisteredDataSources.Any(br => br.DimFieldDisplaySettingsId == dimFieldDisplaysettingsFirstNames.Id && br.DimRegisteredDataSourceId == orcidRegisteredDataSourceId));
            if (dimFieldDisplaySettingsFirstNames == null)
            {
                dimFieldDisplaySettingsFirstNames = new DimFieldDisplaySetting()
                {
                    DimUserProfileId = dimUserProfile.Id,
                    FieldIdentifier = Constants.FieldIdentifiers.FIRST_NAMES,
                    Show = false,
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    Created = DateTime.Now,
                };
                dimFieldDisplaySettingsFirstNames.BrFieldDisplaySettingsDimRegisteredDataSources.Add(
                    new BrFieldDisplaySettingsDimRegisteredDataSource()
                    {
                        DimFieldDisplaySettingsId = dimFieldDisplaySettingsFirstNames.Id,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId
                    }
                );
                _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySettingsFirstNames);
            }
            else
            {
                dimFieldDisplaySettingsFirstNames.Modified = DateTime.Now;
            }
            await _ttvContext.SaveChangesAsync();


            // FirstNames: FactFieldValues
            var factFieldValuesFirstNames = dimUserProfile.FactFieldValues.FirstOrDefault(factFieldValuesFirstNames => factFieldValuesFirstNames.DimFieldDisplaySettingsId == dimFieldDisplaySettingsFirstNames.Id);
            if (factFieldValuesFirstNames == null)
            {
                factFieldValuesFirstNames = new FactFieldValue()
                {
                    DimPidId = -1,
                    DimUserProfileId = dimUserProfile.Id,
                    DimFieldDisplaySettingsId = dimFieldDisplaySettingsFirstNames.Id,
                    DimNameId = dimName.Id,
                    DimWebLinkId = -1,
                    DimFundingDecisionId = -1,
                    DimPublicationId = -1,
                    DimPidIdOrcidPutCode = -1,
                    DimResearchActivityId = -1,
                    DimEventId = -1,
                    DimEducationId = -1,
                    DimCompetenceId = -1,
                    DimResearchCommunityId = -1,
                    DimTelephoneNumberId = -1,
                    DimEmailAddrressId = -1,
                    DimResearcherDescriptionId = -1,
                    DimIdentifierlessDataId = -1,
                    Show = false,
                    PrimaryValue = false,
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    Created = DateTime.Now,
                };
                _ttvContext.FactFieldValues.Add(factFieldValuesFirstNames);
            }
            else
            {
                factFieldValuesFirstNames.Modified = DateTime.Now;
            }
            await _ttvContext.SaveChangesAsync();




            // Researcher urls
            var researcherUrls = _orcidJsonParserService.GetResearcherUrls(json);

            foreach (OrcidResearcherUrl researchUrl in researcherUrls)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimPid
                var factFieldValueWebLink = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimPidIdOrcidPutCode > 0 && ffv.DimPidIdOrcidPutCodeNavigation.PidContent == researchUrl.PutCode.Value.ToString());

                if (factFieldValueWebLink != null)
                {
                    // Update existing DimWebLink
                    factFieldValueWebLink.DimWebLink.Url = researchUrl.Url;
                    factFieldValueWebLink.DimWebLink.LinkLabel = researchUrl.UrlName;
                    factFieldValueWebLink.DimWebLink.Modified = DateTime.Now;

                    // Update existing FactFieldValue
                    factFieldValueWebLink.Modified = DateTime.Now;

                    await _ttvContext.SaveChangesAsync();
                }
                else
                {
                    // Create new DimWebLink
                    var dimWebLink = new DimWebLink()
                    {
                        Url = researchUrl.Url,
                        LinkLabel = researchUrl.UrlName,
                        DimOrganizationId = -1,
                        DimKnownPersonId = dimKnownPerson.Id,
                        DimCallProgrammeId = -1,
                        DimFundingDecisionId = -1,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        Created = DateTime.Now,
                    };
                    _ttvContext.DimWebLinks.Add(dimWebLink);
                    await _ttvContext.SaveChangesAsync();

                    // Add web link ORCID put code into DimPid
                    var dimPidOrcidPutCodeWebLink = new DimPid()
                    {
                        PidContent = researchUrl.PutCode.GetDbValue(),
                        PidType = "ORCID put code",
                        DimKnownPersonId = dimKnownPerson.Id,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeWebLink);

                    // Create DimFieldDisplaySettings for weblink
                    var dimFieldDisplaySettingsWebLink = new DimFieldDisplaySetting()
                    {
                        DimUserProfileId = dimUserProfile.Id,
                        FieldIdentifier = Constants.FieldIdentifiers.WEB_LINK,
                        Show = false,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        Created = DateTime.Now,
                    };
                    // Set ORCID as data source
                    dimFieldDisplaySettingsWebLink.BrFieldDisplaySettingsDimRegisteredDataSources.Add(
                        new BrFieldDisplaySettingsDimRegisteredDataSource()
                        {
                            DimFieldDisplaySettingsId = dimFieldDisplaySettingsWebLink.Id,
                            DimRegisteredDataSourceId = orcidRegisteredDataSourceId
                        }
                    );
                    _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySettingsWebLink);
                    await _ttvContext.SaveChangesAsync();

                    // Create FactFieldValues for weblink
                    _ttvContext.FactFieldValues.Add(
                        new FactFieldValue()
                        {
                            DimPidId = -1,
                            DimUserProfileId = dimUserProfile.Id,
                            DimFieldDisplaySettingsId = dimFieldDisplaySettingsWebLink.Id,
                            DimNameId = -1,
                            DimWebLinkId = dimWebLink.Id,
                            DimFundingDecisionId = -1,
                            DimPublicationId = -1,
                            DimPidIdOrcidPutCode = dimPidOrcidPutCodeWebLink.Id,
                            DimResearchActivityId = -1,
                            DimEventId = -1,
                            DimEducationId = -1,
                            DimCompetenceId = -1,
                            DimResearchCommunityId = -1,
                            DimTelephoneNumberId = -1,
                            DimEmailAddrressId = -1,
                            DimResearcherDescriptionId = -1,
                            DimIdentifierlessDataId = -1,
                            Show = false,
                            PrimaryValue = false,
                            SourceId = Constants.SourceIdentifiers.ORCID,
                            Created = DateTime.Now,
                        }
                    );
                    await _ttvContext.SaveChangesAsync();
                }
            }

            // Researcher description
            var dimResearcherDescription = await _userProfileService.AddOrUpdateDimResearcherDescription(
                "",
                _orcidJsonParserService.GetBiography(json).Value,
                "",
                dimKnownPerson.Id,
                orcidRegisteredDataSourceId
            );

            // Researcher description: DimFieldDisplaySettings
            var dimFieldDisplaySettingsResearcherDescription = dimUserProfile.DimFieldDisplaySettings
                .FirstOrDefault(dimFieldDisplaySettingsResearcherDescription => dimFieldDisplaySettingsResearcherDescription.FieldIdentifier == Constants.FieldIdentifiers.RESEARCHER_DESCRIPTION && dimFieldDisplaySettingsResearcherDescription.BrFieldDisplaySettingsDimRegisteredDataSources.Any(br => br.DimFieldDisplaySettingsId == dimFieldDisplaySettingsResearcherDescription.Id && br.DimRegisteredDataSourceId == orcidRegisteredDataSourceId));

            if (dimFieldDisplaySettingsResearcherDescription == null)
            {
                dimFieldDisplaySettingsResearcherDescription = new DimFieldDisplaySetting()
                {
                    DimUserProfileId = dimUserProfile.Id,
                    FieldIdentifier = Constants.FieldIdentifiers.RESEARCHER_DESCRIPTION,
                    Show = false,
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    Created = DateTime.Now
                };
                dimFieldDisplaySettingsResearcherDescription.BrFieldDisplaySettingsDimRegisteredDataSources.Add(
                    new BrFieldDisplaySettingsDimRegisteredDataSource()
                    {
                        DimFieldDisplaySettingsId = dimFieldDisplaySettingsResearcherDescription.Id,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId
                    }
                );
                _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySettingsResearcherDescription);
            }
            else
            {
                dimFieldDisplaySettingsResearcherDescription.Modified = DateTime.Now;
            }
            await _ttvContext.SaveChangesAsync();

            // Researcher description: FactFieldValues
            var factFieldValuesResearcherDescription = dimUserProfile.FactFieldValues.FirstOrDefault(factFieldValuesResearcherDescription => factFieldValuesResearcherDescription.DimFieldDisplaySettingsId == dimFieldDisplaySettingsResearcherDescription.Id);
            if (factFieldValuesResearcherDescription == null)
            {
                factFieldValuesResearcherDescription = new FactFieldValue()
                {
                    DimPidId = -1,
                    DimUserProfileId = dimUserProfile.Id,
                    DimFieldDisplaySettingsId = dimFieldDisplaySettingsResearcherDescription.Id,
                    DimNameId = -1,
                    DimWebLinkId = -1,
                    DimFundingDecisionId = -1,
                    DimPublicationId = -1,
                    DimPidIdOrcidPutCode = -1,
                    DimResearchActivityId = -1,
                    DimEventId = -1,
                    DimEducationId = -1,
                    DimCompetenceId = -1,
                    DimResearchCommunityId = -1,
                    DimTelephoneNumberId = -1,
                    DimEmailAddrressId = -1,
                    DimResearcherDescriptionId = dimResearcherDescription.Id,
                    DimIdentifierlessDataId = -1,
                    Show = false,
                    PrimaryValue = false,
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    Created = DateTime.Now,
                };
                _ttvContext.FactFieldValues.Add(factFieldValuesResearcherDescription);
            }
            else
            {
                factFieldValuesResearcherDescription.Modified = DateTime.Now;
            }
            await _ttvContext.SaveChangesAsync();

            return Ok(new ApiResponse(success: true));
        }
    }
}