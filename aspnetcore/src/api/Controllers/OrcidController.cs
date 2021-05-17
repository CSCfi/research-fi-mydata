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
                    .ThenInclude(dfds => dfds.BrFieldDisplaySettingsDimRegisteredDataSources).AsNoTracking()
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
                .Include(dkp => dkp.DimNames).AsSplitQuery().AsNoTracking().FirstOrDefaultAsync(dkp => dkp.Id == dimUserProfile.DimKnownPersonId);

            // Get ORCID registered data source id
            var orcidRegisteredDataSourceId = await _userProfileService.GetOrcidRegisteredDataSourceId();


            // FirstNames and LastName
            var dimName = await _userProfileService.AddOrUpdateDimName(
                _orcidJsonParserService.GetFamilyName(json).Value,
                _orcidJsonParserService.GetGivenNames(json).Value,
                dimKnownPerson.Id,
                orcidRegisteredDataSourceId
            );

            // LastName: DimFieldDisplaySettings
            var dimFieldDisplaySettingsLastName = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dimFieldDisplaysettingsLastName => dimFieldDisplaysettingsLastName.FieldIdentifier == Constants.FieldIdentifiers.PERSON_LAST_NAME && dimFieldDisplaysettingsLastName.SourceId == Constants.SourceIdentifiers.ORCID);
            // LastName: FactFieldValues
            var factFieldValuesLastName = dimUserProfile.FactFieldValues.FirstOrDefault(factFieldValuesLastName => factFieldValuesLastName.DimFieldDisplaySettingsId == dimFieldDisplaySettingsLastName.Id);
            if (factFieldValuesLastName == null)
            {
                factFieldValuesLastName = _userProfileService.GetEmptyFactFieldValue();
                factFieldValuesLastName.DimUserProfileId = dimUserProfile.Id;
                factFieldValuesLastName.DimFieldDisplaySettingsId = dimFieldDisplaySettingsLastName.Id;
                factFieldValuesLastName.DimNameId = dimName.Id;
                factFieldValuesLastName.SourceId = Constants.SourceIdentifiers.ORCID;
                _ttvContext.FactFieldValues.Add(factFieldValuesLastName);
            }
            else
            {
                factFieldValuesLastName.Modified = DateTime.Now;
            }
            await _ttvContext.SaveChangesAsync();


            // FirstNames: DimFieldDisplaySettings
            var dimFieldDisplaySettingsFirstNames = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dimFieldDisplaysettingsFirstNames => dimFieldDisplaysettingsFirstNames.FieldIdentifier == Constants.FieldIdentifiers.PERSON_FIRST_NAMES && dimFieldDisplaysettingsFirstNames.SourceId == Constants.SourceIdentifiers.ORCID);
            // FirstNames: FactFieldValues
            var factFieldValuesFirstNames = dimUserProfile.FactFieldValues.FirstOrDefault(factFieldValuesFirstNames => factFieldValuesFirstNames.DimFieldDisplaySettingsId == dimFieldDisplaySettingsFirstNames.Id);
            if (factFieldValuesFirstNames == null)
            {
                factFieldValuesFirstNames = _userProfileService.GetEmptyFactFieldValue();
                factFieldValuesFirstNames.DimUserProfileId = dimUserProfile.Id;
                factFieldValuesFirstNames.DimFieldDisplaySettingsId = dimFieldDisplaySettingsFirstNames.Id;
                factFieldValuesFirstNames.DimNameId = dimName.Id;
                factFieldValuesFirstNames.SourceId = Constants.SourceIdentifiers.ORCID;
                _ttvContext.FactFieldValues.Add(factFieldValuesFirstNames);
            }
            else
            {
                factFieldValuesFirstNames.Modified = DateTime.Now;
            }
            await _ttvContext.SaveChangesAsync();


            // Other names
            var otherNames = _orcidJsonParserService.GetOtherNames(json);
            foreach (OrcidOtherName otherName in otherNames)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimPid
                var factFieldValuesOtherName = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimPidIdOrcidPutCode > 0 && ffv.DimPidIdOrcidPutCodeNavigation.PidContent == otherName.PutCode.Value.ToString());

                if (factFieldValuesOtherName != null)
                {
                    // Update existing DimWebLink
                    factFieldValuesOtherName.DimName.FullName = otherName.Value;
                    factFieldValuesOtherName.DimWebLink.Modified = DateTime.Now;

                    // Update existing FactFieldValue
                    factFieldValuesOtherName.Modified = DateTime.Now;

                    await _ttvContext.SaveChangesAsync();
                }
                else
                {
                    // Create new DimName for other name
                    var dimName_otherName = new DimName()
                    {
                        FullName = otherName.Value,
                        DimKnownPersonIdConfirmedIdentity = dimKnownPerson.Id,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimNames.Add(dimName_otherName);
                    await _ttvContext.SaveChangesAsync();

                    // Add other name ORCID put code into DimPid
                    var dimPidOrcidPutCodeOtherName = new DimPid()
                    {
                        PidContent = otherName.PutCode.GetDbValue(),
                        PidType = "ORCID put code",
                        DimKnownPersonId = dimKnownPerson.Id,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeOtherName);
                    await _ttvContext.SaveChangesAsync();

                    // Get DimFieldDisplaySettings for other name
                    var dimFieldDisplaySettingsOtherName = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsWebLink => dfdsWebLink.FieldIdentifier == Constants.FieldIdentifiers.PERSON_OTHER_NAMES && dfdsWebLink.SourceId == Constants.SourceIdentifiers.ORCID);

                    // Create FactFieldValues for weblink
                    factFieldValuesOtherName = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesOtherName.DimUserProfileId = dimUserProfile.Id;
                    factFieldValuesOtherName.DimFieldDisplaySettingsId = dimFieldDisplaySettingsOtherName.Id;
                    factFieldValuesOtherName.DimNameId = dimName_otherName.Id;
                    factFieldValuesOtherName.DimPidIdOrcidPutCode = dimPidOrcidPutCodeOtherName.Id;
                    factFieldValuesOtherName.SourceId = Constants.SourceIdentifiers.ORCID;
                    _ttvContext.FactFieldValues.Add(factFieldValuesOtherName);
                    await _ttvContext.SaveChangesAsync();
                }
            }


            // Researcher urls
            var researcherUrls = _orcidJsonParserService.GetResearcherUrls(json);
            foreach (OrcidResearcherUrl researchUrl in researcherUrls)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimPid
                var factFieldValuesWebLink = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimPidIdOrcidPutCode > 0 && ffv.DimPidIdOrcidPutCodeNavigation.PidContent == researchUrl.PutCode.Value.ToString());

                if (factFieldValuesWebLink != null)
                {
                    // Update existing DimWebLink
                    factFieldValuesWebLink.DimWebLink.Url = researchUrl.Url;
                    factFieldValuesWebLink.DimWebLink.LinkLabel = researchUrl.UrlName;
                    factFieldValuesWebLink.DimWebLink.Modified = DateTime.Now;

                    // Update existing FactFieldValue
                    factFieldValuesWebLink.Modified = DateTime.Now;

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
                    await _ttvContext.SaveChangesAsync();

                    // Get DimFieldDisplaySettings for weblink
                    var dimFieldDisplaySettingsWebLink = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsWebLink => dfdsWebLink.FieldIdentifier == Constants.FieldIdentifiers.PERSON_WEB_LINK && dfdsWebLink.SourceId == Constants.SourceIdentifiers.ORCID);

                    // Create FactFieldValues for weblink
                    factFieldValuesWebLink = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesWebLink.DimUserProfileId = dimUserProfile.Id;
                    factFieldValuesWebLink.DimFieldDisplaySettingsId = dimFieldDisplaySettingsWebLink.Id;
                    factFieldValuesWebLink.DimWebLinkId = dimWebLink.Id;
                    factFieldValuesWebLink.DimPidIdOrcidPutCode = dimPidOrcidPutCodeWebLink.Id;
                    factFieldValuesWebLink.SourceId = Constants.SourceIdentifiers.ORCID;
                    _ttvContext.FactFieldValues.Add(factFieldValuesWebLink);
                    await _ttvContext.SaveChangesAsync();
                }
            }

            // Researcher description
            var biography = _orcidJsonParserService.GetBiography(json);
            if (biography != null)
            { 
                var dimResearcherDescription = await _userProfileService.AddOrUpdateDimResearcherDescription(
                    "",
                    _orcidJsonParserService.GetBiography(json).Value,
                    "",
                    dimKnownPerson.Id,
                    orcidRegisteredDataSourceId
                );

                // Researcher description: DimFieldDisplaySettings
                var dimFieldDisplaySettingsResearcherDescription = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dimFieldDisplaySettingsResearcherDescription => dimFieldDisplaySettingsResearcherDescription.FieldIdentifier == Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION && dimFieldDisplaySettingsResearcherDescription.SourceId == Constants.SourceIdentifiers.ORCID);

                // Researcher description: FactFieldValues
                var factFieldValuesResearcherDescription = dimUserProfile.FactFieldValues.FirstOrDefault(factFieldValuesResearcherDescription => factFieldValuesResearcherDescription.DimFieldDisplaySettingsId == dimFieldDisplaySettingsResearcherDescription.Id);
                if (factFieldValuesResearcherDescription == null)
                {
                    factFieldValuesResearcherDescription = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesResearcherDescription.DimUserProfileId = dimUserProfile.Id;
                    factFieldValuesResearcherDescription.DimFieldDisplaySettingsId = dimFieldDisplaySettingsResearcherDescription.Id;
                    factFieldValuesResearcherDescription.DimResearcherDescriptionId = dimResearcherDescription.Id;
                    factFieldValuesResearcherDescription.SourceId = Constants.SourceIdentifiers.ORCID;
                    _ttvContext.FactFieldValues.Add(factFieldValuesResearcherDescription);
                }
                else
                {
                    factFieldValuesResearcherDescription.Modified = DateTime.Now;
                }
                await _ttvContext.SaveChangesAsync();
            }


            // Email
            var emails = _orcidJsonParserService.GetEmails(json);
            foreach (OrcidEmail email in emails)
            {
                // Email: DimEmailAddrressess
                var dimEmailAddress = await _userProfileService.AddOrUpdateDimEmailAddress(
                    email.Value,
                    dimKnownPerson.Id,
                    orcidRegisteredDataSourceId
                );

                // Email: DimFieldDisplaySettings
                var dimFieldDisplaySettingsEmailAddress = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dimFieldDisplaySettingsEmailAddress => dimFieldDisplaySettingsEmailAddress.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS && dimFieldDisplaySettingsEmailAddress.SourceId == Constants.SourceIdentifiers.ORCID);

                // Email: FactFieldValues
                var factFieldValuesEmailAddress = dimUserProfile.FactFieldValues.FirstOrDefault(factFieldValuesEmailAddress => factFieldValuesEmailAddress.DimFieldDisplaySettingsId == dimFieldDisplaySettingsEmailAddress.Id);
                if (factFieldValuesEmailAddress == null)
                {
                    factFieldValuesEmailAddress = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesEmailAddress.DimUserProfileId = dimUserProfile.Id;
                    factFieldValuesEmailAddress.DimFieldDisplaySettingsId = dimFieldDisplaySettingsEmailAddress.Id;
                    factFieldValuesEmailAddress.DimEmailAddrressId = dimEmailAddress.Id;
                    factFieldValuesEmailAddress.SourceId = Constants.SourceIdentifiers.ORCID;
                    _ttvContext.FactFieldValues.Add(factFieldValuesEmailAddress);
                }
                else
                {
                    factFieldValuesEmailAddress.Modified = DateTime.Now;
                }
                await _ttvContext.SaveChangesAsync();
            }

            // Keyword
            var keywords = _orcidJsonParserService.GetKeywords(json);
            foreach (OrcidKeyword keyword in keywords)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimKeyword
                var factFieldValuesKeyword = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimPidIdOrcidPutCode > 0 && ffv.DimPidIdOrcidPutCodeNavigation.PidContent == keyword.PutCode.Value.ToString());

                if (factFieldValuesKeyword != null)
                {
                    // Update existing DimKeywork
                    factFieldValuesKeyword.DimKeyword.Keyword = keyword.Value;
                    factFieldValuesKeyword.DimKeyword.Modified = DateTime.Now;

                    // Update existing FactFieldValue
                    factFieldValuesKeyword.Modified = DateTime.Now;

                    await _ttvContext.SaveChangesAsync();
                }
                else
                {
                    // Create new DimKeyword
                    var dimKeyword = new DimKeyword()
                    {
                        Keyword = keyword.Value,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimKeywords.Add(dimKeyword);
                    await _ttvContext.SaveChangesAsync();

                    // Add keyword ORCID put code into DimPid
                    var dimPidOrcidPutCodeKeyword = new DimPid()
                    {
                        PidContent = keyword.PutCode.GetDbValue(),
                        PidType = "ORCID put code",
                        DimKnownPersonId = dimKnownPerson.Id,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeKeyword);
                    await _ttvContext.SaveChangesAsync();

                    // Get DimFieldDisplaySettings for keyword
                    var dimFieldDisplaySettingsKeyword = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsKeyword => dfdsKeyword.FieldIdentifier == Constants.FieldIdentifiers.PERSON_KEYWORD && dfdsKeyword.SourceId == Constants.SourceIdentifiers.ORCID);

                    // Create FactFieldValues for weblink
                    factFieldValuesKeyword = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesKeyword.DimUserProfileId = dimUserProfile.Id;
                    factFieldValuesKeyword.DimFieldDisplaySettingsId = dimFieldDisplaySettingsKeyword.Id;
                    factFieldValuesKeyword.DimKeywordId = dimKeyword.Id;
                    factFieldValuesKeyword.DimPidIdOrcidPutCode = dimPidOrcidPutCodeKeyword.Id;
                    factFieldValuesKeyword.SourceId = Constants.SourceIdentifiers.ORCID;
                    _ttvContext.FactFieldValues.Add(factFieldValuesKeyword);
                    await _ttvContext.SaveChangesAsync();
                }
            }

            return Ok(new ApiResponse(success: true));
        }
    }
}