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
        // TODO: Currently adding and updating ORCID data works, but detecting deleted ORCID data and deleting them is TTV database is not implemented.
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
                    .ThenInclude(ffv => ffv.DimOrcidPublication)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPid)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPidIdOrcidPutCodeNavigation)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearchActivity)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEvent)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEducation)
                        .ThenInclude(de => de.DimStartDateNavigation)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEducation)
                        .ThenInclude(de => de.DimEndDateNavigation)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimAffiliation)
                        .ThenInclude(da => da.DimOrganization)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimAffiliation)
                        .ThenInclude(da => da.StartDateNavigation)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimAffiliation)
                        .ThenInclude(da => da.EndDateNavigation)
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
                    .ThenInclude(ffv => ffv.DimWebLink)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimKeyword).AsSplitQuery().FirstOrDefaultAsync(up => up.Id == userprofileId);

            // Get DimKnownPerson
            var dimKnownPerson = await _ttvContext.DimKnownPeople
                .Include(dkp => dkp.DimNames).AsSplitQuery().AsNoTracking().FirstOrDefaultAsync(dkp => dkp.Id == dimUserProfile.DimKnownPersonId);

            // Get ORCID registered data source id
            var orcidRegisteredDataSourceId = await _userProfileService.GetOrcidRegisteredDataSourceId();


            // Must use "Constants.SourceIdentifiers.ORCID" as value for "FactFieldValue.SourceId". It is used to identify what data can be deleted when user profile is deleted.


            // Name
            var dimFieldDisplaySettingsName = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dimFieldDisplaysettingsName => dimFieldDisplaysettingsName.FieldIdentifier == Constants.FieldIdentifiers.PERSON_NAME && dimFieldDisplaysettingsName.SourceId == Constants.SourceIdentifiers.ORCID);
            // FactFieldValues
            var factFieldValuesName = dimUserProfile.FactFieldValues.FirstOrDefault(factFieldValuesName => factFieldValuesName.DimFieldDisplaySettingsId == dimFieldDisplaySettingsName.Id);
            if (factFieldValuesName != null)
            {
                // Update existing DimName
                var dimName = factFieldValuesName.DimName;
                dimName.LastName = _orcidJsonParserService.GetFamilyName(json).Value;
                dimName.FirstNames = _orcidJsonParserService.GetGivenNames(json).Value;
                dimName.Modified = DateTime.Now;
                _ttvContext.Entry(dimName).State = EntityState.Modified;
                // Update existing FactFieldValue
                factFieldValuesName.Modified = DateTime.Now;
                await _ttvContext.SaveChangesAsync();
            }
            else
            {
                // Create new DimName
                var dimName = new DimName()
                {
                    LastName = _orcidJsonParserService.GetFamilyName(json).Value,
                    FirstNames = _orcidJsonParserService.GetGivenNames(json).Value,
                    DimKnownPersonIdConfirmedIdentity = dimKnownPerson.Id,
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = DateTime.Now,
                    DimRegisteredDataSourceId = orcidRegisteredDataSourceId
                };
                _ttvContext.DimNames.Add(dimName);
                await _ttvContext.SaveChangesAsync();

                factFieldValuesName = _userProfileService.GetEmptyFactFieldValueOrcid();
                factFieldValuesName.DimUserProfileId = dimUserProfile.Id;
                factFieldValuesName.DimFieldDisplaySettingsId = dimFieldDisplaySettingsName.Id;
                factFieldValuesName.DimNameId = dimName.Id;
                _ttvContext.FactFieldValues.Add(factFieldValuesName);
                await _ttvContext.SaveChangesAsync();
            }


            // Other names
            var otherNames = _orcidJsonParserService.GetOtherNames(json);
            foreach (OrcidOtherName otherName in otherNames)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimPid
                var factFieldValuesOtherName = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimPidIdOrcidPutCode > 0 && ffv.DimPidIdOrcidPutCodeNavigation.PidContent == otherName.PutCode.Value.ToString());

                if (factFieldValuesOtherName != null)
                {
                    // Update existing DimName
                    var dimName_otherName = factFieldValuesOtherName.DimName;
                    dimName_otherName.FullName = otherName.Value;
                    dimName_otherName.Modified = DateTime.Now;
                    _ttvContext.Entry(dimName_otherName).State = EntityState.Modified;
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
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
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
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeOtherName);
                    await _ttvContext.SaveChangesAsync();

                    // Get DimFieldDisplaySettings for other name
                    var dimFieldDisplaySettingsOtherName = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsWebLink => dfdsWebLink.FieldIdentifier == Constants.FieldIdentifiers.PERSON_OTHER_NAMES && dfdsWebLink.SourceId == Constants.SourceIdentifiers.ORCID);

                    // Create FactFieldValues for other name
                    factFieldValuesOtherName = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesOtherName.DimUserProfileId = dimUserProfile.Id;
                    factFieldValuesOtherName.DimFieldDisplaySettingsId = dimFieldDisplaySettingsOtherName.Id;
                    factFieldValuesOtherName.DimNameId = dimName_otherName.Id;
                    factFieldValuesOtherName.DimPidIdOrcidPutCode = dimPidOrcidPutCodeOtherName.Id;
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
                    var dimWebLink = factFieldValuesWebLink.DimWebLink;
                    dimWebLink.Url = researchUrl.Url;
                    dimWebLink.LinkLabel = researchUrl.UrlName;
                    dimWebLink.Modified = DateTime.Now;
                    _ttvContext.Entry(dimWebLink).State = EntityState.Modified;

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
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
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
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeWebLink);
                    await _ttvContext.SaveChangesAsync();

                    // Get DimFieldDisplaySettings for weblink
                    var dimFieldDisplaySettingsWebLink = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsWebLink => dfdsWebLink.FieldIdentifier == Constants.FieldIdentifiers.PERSON_WEB_LINK && dfdsWebLink.SourceId == Constants.SourceIdentifiers.ORCID);

                    // Create FactFieldValues for weblink
                    factFieldValuesWebLink = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesWebLink.DimUserProfileId = dimUserProfile.Id;
                    factFieldValuesWebLink.DimFieldDisplaySettingsId = dimFieldDisplaySettingsWebLink.Id;
                    factFieldValuesWebLink.DimWebLinkId = dimWebLink.Id;
                    factFieldValuesWebLink.DimPidIdOrcidPutCode = dimPidOrcidPutCodeWebLink.Id;
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
                    factFieldValuesResearcherDescription = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesResearcherDescription.DimUserProfileId = dimUserProfile.Id;
                    factFieldValuesResearcherDescription.DimFieldDisplaySettingsId = dimFieldDisplaySettingsResearcherDescription.Id;
                    factFieldValuesResearcherDescription.DimResearcherDescriptionId = dimResearcherDescription.Id;
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
                    factFieldValuesEmailAddress = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesEmailAddress.DimUserProfileId = dimUserProfile.Id;
                    factFieldValuesEmailAddress.DimFieldDisplaySettingsId = dimFieldDisplaySettingsEmailAddress.Id;
                    factFieldValuesEmailAddress.DimEmailAddrressId = dimEmailAddress.Id;
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
            // Get DimFieldDisplaySettings for keyword
            var dimFieldDisplaySettingsKeyword = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsKeyword => dfdsKeyword.FieldIdentifier == Constants.FieldIdentifiers.PERSON_KEYWORD && dfdsKeyword.SourceId == Constants.SourceIdentifiers.ORCID);
            // Collect list of processed FactFieldValues related to keyword. Needed when deleting keywords.
            var processedKeywordFactFieldValues = new List<FactFieldValue> ();
            foreach (OrcidKeyword keyword in keywords)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimKeyword
                var factFieldValuesKeyword = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimPidIdOrcidPutCode > 0 && ffv.DimPidIdOrcidPutCodeNavigation.PidContent == keyword.PutCode.Value.ToString());

                if (factFieldValuesKeyword != null)
                {
                    // Update existing DimKeyword
                    var dimKeyword = factFieldValuesKeyword.DimKeyword;
                    dimKeyword.Keyword = keyword.Value;
                    dimKeyword.Modified = DateTime.Now;
                    _ttvContext.Entry(dimKeyword).State = EntityState.Modified;
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
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
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
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeKeyword);
                    await _ttvContext.SaveChangesAsync();

                    // Create FactFieldValues for keyword
                    factFieldValuesKeyword = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesKeyword.DimUserProfileId = dimUserProfile.Id;
                    factFieldValuesKeyword.DimFieldDisplaySettingsId = dimFieldDisplaySettingsKeyword.Id;
                    factFieldValuesKeyword.DimKeywordId = dimKeyword.Id;
                    factFieldValuesKeyword.DimPidIdOrcidPutCode = dimPidOrcidPutCodeKeyword.Id;
                    _ttvContext.FactFieldValues.Add(factFieldValuesKeyword);
                    await _ttvContext.SaveChangesAsync();
                }
                processedKeywordFactFieldValues.Add(factFieldValuesKeyword);
            }
            // Remove existing keywords which were not in ORCID data.
            foreach (FactFieldValue ffvKeyword in dimFieldDisplaySettingsKeyword.FactFieldValues)
            {
                if (!processedKeywordFactFieldValues.Contains(ffvKeyword))
                {
                    _ttvContext.FactFieldValues.Remove(ffvKeyword);
                    _ttvContext.DimKeywords.Remove(ffvKeyword.DimKeyword);
                }
            }
            await _ttvContext.SaveChangesAsync();



            // External identifier (=DimPid)
            var externalIdentifiers = _orcidJsonParserService.GetExternalIdentifiers(json);
            // Get DimFieldDisplaySettings for keyword
            var dimFieldDisplaySettingsExternalIdentifier = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsKeyword => dfdsKeyword.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER && dfdsKeyword.SourceId == Constants.SourceIdentifiers.ORCID);
            foreach (OrcidExternalIdentifier externalIdentifier in externalIdentifiers)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimPid
                var factFieldValuesExternalIdentifier = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimPidIdOrcidPutCode > 0 && ffv.DimPidIdOrcidPutCodeNavigation.PidContent == externalIdentifier.PutCode.Value.ToString());

                if (factFieldValuesExternalIdentifier != null)
                {
                    // Update existing DimPid
                    var dimPid = factFieldValuesExternalIdentifier.DimPid;
                    dimPid.PidContent = externalIdentifier.ExternalIdValue;
                    dimPid.PidType = externalIdentifier.ExternalIdType;
                    dimPid.Modified = DateTime.Now;
                    _ttvContext.Entry(dimPid).State = EntityState.Modified;
                    // Update existing FactFieldValue
                    factFieldValuesExternalIdentifier.Modified = DateTime.Now;
                    await _ttvContext.SaveChangesAsync();
                }
                else
                {
                    // Create new DimPid (external identifier is stored into DimPid)
                    var dimPid = new DimPid()
                    {
                        PidContent = externalIdentifier.ExternalIdValue,
                        PidType = externalIdentifier.ExternalIdType,
                        DimKnownPersonId = dimKnownPerson.Id,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimPids.Add(dimPid);
                    await _ttvContext.SaveChangesAsync();

                    // Add ORCID put code into DimPid
                    var dimPidOrcidPutCodeExternalIdentifier = new DimPid()
                    {
                        PidContent = externalIdentifier.PutCode.GetDbValue(),
                        PidType = "ORCID put code",
                        DimKnownPersonId = dimKnownPerson.Id,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeExternalIdentifier);
                    await _ttvContext.SaveChangesAsync();

                    // Create FactFieldValues for external identifier
                    factFieldValuesExternalIdentifier = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesExternalIdentifier.DimUserProfileId = dimUserProfile.Id;
                    factFieldValuesExternalIdentifier.DimFieldDisplaySettingsId = dimFieldDisplaySettingsExternalIdentifier.Id;
                    factFieldValuesExternalIdentifier.DimPidId = dimPid.Id;
                    factFieldValuesExternalIdentifier.DimPidIdOrcidPutCode = dimPidOrcidPutCodeExternalIdentifier.Id;
                    _ttvContext.FactFieldValues.Add(factFieldValuesExternalIdentifier);
                    await _ttvContext.SaveChangesAsync();
                }
            }



            // Education
            var educations = _orcidJsonParserService.GetEducations(json);
            foreach (OrcidEducation education in educations)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimEducation
                var factFieldValuesEducation = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimPidIdOrcidPutCode > 0 && ffv.DimPidIdOrcidPutCodeNavigation.PidContent == education.PutCode.Value.ToString());

                // Start date
                var startDate = await _ttvContext.DimDates.FirstOrDefaultAsync(dd => dd.Year == education.StartDate.Year && dd.Month == education.StartDate.Month && dd.Day == education.StartDate.Day);
                if (startDate == null)
                {
                    startDate = new DimDate()
                    {
                        Year = education.StartDate.Year,
                        Month = education.StartDate.Month,
                        Day = education.StartDate.Day,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimDates.Add(startDate);
                    await _ttvContext.SaveChangesAsync();
                }
               
                // End date
                var endDate = await _ttvContext.DimDates.FirstOrDefaultAsync(ed => ed.Year == education.EndDate.Year && ed.Month == education.EndDate.Month && ed.Day == education.EndDate.Day);
                if (endDate == null)
                {
                    endDate = new DimDate()
                    {
                        Year = education.EndDate.Year,
                        Month = education.EndDate.Month,
                        Day = education.EndDate.Day,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimDates.Add(endDate);
                    await _ttvContext.SaveChangesAsync();
                }

                if (factFieldValuesEducation != null)
                {
                    // Update existing DimEducation
                    var dimEducation = factFieldValuesEducation.DimEducation; 
                    dimEducation.NameEn = education.RoleTitle;
                    dimEducation.DegreeGrantingInstitutionName = education.OrganizationName;
                    dimEducation.DimStartDate = startDate.Id;
                    dimEducation.DimEndDate = endDate.Id;
                    _ttvContext.Entry(dimEducation).State = EntityState.Modified;
                    dimEducation.Modified = DateTime.Now;

                    // Update existing FactFieldValue
                    factFieldValuesEducation.Modified = DateTime.Now;

                    await _ttvContext.SaveChangesAsync();
                }
                else
                {
                    // Create new DimEducation
                    var dimEducation = new DimEducation()
                    {
                        NameEn = education.RoleTitle,
                        DegreeGrantingInstitutionName = education.OrganizationName,
                        DimStartDate = startDate.Id,
                        DimEndDate = endDate.Id,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        DimKnownPersonId = dimKnownPerson.Id,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimEducations.Add(dimEducation);
                    await _ttvContext.SaveChangesAsync();

                    // Add education ORCID put code into DimPid
                    var dimPidOrcidPutCodeEducation = new DimPid()
                    {
                        PidContent = education.PutCode.GetDbValue(),
                        PidType = "ORCID put code",
                        DimKnownPersonId = dimKnownPerson.Id,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeEducation);
                    await _ttvContext.SaveChangesAsync();

                    // Get DimFieldDisplaySettings for education
                    var dimFieldDisplaySettingsEducation = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsEducation => dfdsEducation.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_EDUCATION && dfdsEducation.SourceId == Constants.SourceIdentifiers.ORCID);

                    // Create FactFieldValues for education
                    factFieldValuesEducation = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesEducation.DimUserProfileId = dimUserProfile.Id;
                    factFieldValuesEducation.DimFieldDisplaySettingsId = dimFieldDisplaySettingsEducation.Id;
                    factFieldValuesEducation.DimEducationId = dimEducation.Id;
                    factFieldValuesEducation.DimPidIdOrcidPutCode = dimPidOrcidPutCodeEducation.Id;
                    _ttvContext.FactFieldValues.Add(factFieldValuesEducation);
                    await _ttvContext.SaveChangesAsync();
                }
            }


            // Employment (Affiliation in Ttv database)
            // TODO: Handling of relations DimOrganization and AffiliationType
            var employments = _orcidJsonParserService.GetEmployments(json);
            foreach (OrcidEmployment employment in employments)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimAffiliation
                var factFieldValuesAffiliation = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimPidIdOrcidPutCode > 0 && ffv.DimPidIdOrcidPutCodeNavigation.PidContent == employment.PutCode.Value.ToString());

                // Start date
                var startDate = await _ttvContext.DimDates.FirstOrDefaultAsync(dd => dd.Year == employment.StartDate.Year && dd.Month == employment.StartDate.Month && dd.Day == employment.StartDate.Day);
                if (startDate == null)
                {
                    startDate = new DimDate()
                    {
                        Year = employment.StartDate.Year,
                        Month = employment.StartDate.Month,
                        Day = employment.StartDate.Day,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimDates.Add(startDate);
                    await _ttvContext.SaveChangesAsync();
                }

                // End date
                var endDate = await _ttvContext.DimDates.FirstOrDefaultAsync(dd => dd.Year == employment.EndDate.Year && dd.Month == employment.EndDate.Month && dd.Day == employment.EndDate.Day);
                if (endDate == null)
                {
                    endDate = new DimDate()
                    {
                        Year = employment.EndDate.Year,
                        Month = employment.EndDate.Month,
                        Day = employment.EndDate.Day,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimDates.Add(endDate);
                    await _ttvContext.SaveChangesAsync();
                }

                // TODO: DimOrganization handling

                if (factFieldValuesAffiliation != null)
                {
                    // Update existing DimAffiliation
                    var dimAffiliation = factFieldValuesAffiliation.DimAffiliation;
                    dimAffiliation.PositionNameEn = employment.RoleTitle;
                    dimAffiliation.StartDate = startDate.Id;
                    dimAffiliation.EndDate = endDate.Id;
                    _ttvContext.Entry(dimAffiliation).State = EntityState.Modified;
                    dimAffiliation.Modified = DateTime.Now;

                    // Update related DimOrganization
                    // TODO: DimOrganization handling
                    var dimOrganization = dimAffiliation.DimOrganization;
                    dimOrganization.NameEn = employment.OrganizationName;
                    _ttvContext.Entry(dimOrganization).State = EntityState.Modified;

                    // Update existing FactFieldValue
                    factFieldValuesAffiliation.Modified = DateTime.Now;

                    await _ttvContext.SaveChangesAsync();
                }
                else
                {
                    // Create new related DimOrganization
                    // TODO: DimOrganization handling
                    var dimOrganization = new DimOrganization()
                    {
                        DimSectorid = -1,
                        NameEn = employment.OrganizationName,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId
                    };

                    _ttvContext.DimOrganizations.Add(dimOrganization);
                    await _ttvContext.SaveChangesAsync();


                    // Create new DimAffiliation
                    var dimAffiliation = new DimAffiliation()
                    {
                        DimOrganizationId = dimOrganization.Id,
                        StartDate = startDate.Id,
                        EndDate = endDate.Id,
                        PositionNameEn = employment.RoleTitle,
                        AffiliationType = -1,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        DimKnownPersonId = dimKnownPerson.Id,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimAffiliations.Add(dimAffiliation);
                    await _ttvContext.SaveChangesAsync();

                    // Add employment (=affiliation) ORCID put code into DimPid
                    var dimPidOrcidPutCodeEmployment = new DimPid()
                    {
                        PidContent = employment.PutCode.GetDbValue(),
                        PidType = "ORCID put code",
                        DimKnownPersonId = dimKnownPerson.Id,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeEmployment);
                    await _ttvContext.SaveChangesAsync();

                    // Get DimFieldDisplaySettings for affiliation
                    var dimFieldDisplaySettingsAffiliation = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsAffiliation => dfdsAffiliation.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_AFFILIATION && dfdsAffiliation.SourceId == Constants.SourceIdentifiers.ORCID);

                    // Create FactFieldValues for affiliation
                    factFieldValuesAffiliation = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesAffiliation.DimUserProfileId = dimUserProfile.Id;
                    factFieldValuesAffiliation.DimFieldDisplaySettingsId = dimFieldDisplaySettingsAffiliation.Id;
                    factFieldValuesAffiliation.DimAffiliationId = dimAffiliation.Id;
                    factFieldValuesAffiliation.DimPidIdOrcidPutCode = dimPidOrcidPutCodeEmployment.Id;
                    _ttvContext.FactFieldValues.Add(factFieldValuesAffiliation);
                    await _ttvContext.SaveChangesAsync();
                }
            }


            // Publication
            var orcidPublications = _orcidJsonParserService.GetPublications(json);
            foreach (OrcidPublication orcidPublication in orcidPublications)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimOrcidPublication
                var factFieldValuesPublication = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimPidIdOrcidPutCode > 0 && ffv.DimPidIdOrcidPutCodeNavigation.PidContent == orcidPublication.PutCode.Value.ToString());

                if (factFieldValuesPublication != null)
                {
                    // Update existing DimOrcidPublication
                    var dimOrcidPublication = factFieldValuesPublication.DimOrcidPublication;
                    dimOrcidPublication.OrcidWorkType = orcidPublication.Type;
                    dimOrcidPublication.PublicationName = orcidPublication.PublicationName;
                    dimOrcidPublication.PublicationYear = orcidPublication.PublicationYear;
                    dimOrcidPublication.DoiHandle = orcidPublication.DoiHandle;
                    dimOrcidPublication.Modified = DateTime.Now;
                    _ttvContext.Entry(dimOrcidPublication).State = EntityState.Modified;
                    // Update existing FactFieldValue
                    factFieldValuesPublication.Modified = DateTime.Now;
                    await _ttvContext.SaveChangesAsync();
                }
                else
                {
                    // Create new DimOrcidPublication
                    var dimOrcidPublication = _userProfileService.GetEmptyDimOrcidPublication();
                    dimOrcidPublication.OrcidWorkType = orcidPublication.Type;
                    dimOrcidPublication.PublicationName = orcidPublication.PublicationName;
                    dimOrcidPublication.PublicationYear = orcidPublication.PublicationYear;
                    dimOrcidPublication.DoiHandle = orcidPublication.DoiHandle;
                    dimOrcidPublication.SourceId = Constants.SourceIdentifiers.ORCID;
                    dimOrcidPublication.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    dimOrcidPublication.Created = DateTime.Now;
                    _ttvContext.DimOrcidPublications.Add(dimOrcidPublication);
                    await _ttvContext.SaveChangesAsync();

                    // Add publication's ORCID put code into DimPid
                    var dimPidOrcidPutCodePublication = new DimPid()
                    {
                        PidContent = orcidPublication.PutCode.GetDbValue(),
                        PidType = "ORCID put code",
                        DimKnownPersonId = dimKnownPerson.Id,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = DateTime.Now
                    };
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodePublication);
                    await _ttvContext.SaveChangesAsync();

                    // Get DimFieldDisplaySettings for orcid publication
                    var dimFieldDisplaySettingsPublication = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsPublication => dfdsPublication.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION && dfdsPublication.SourceId == Constants.SourceIdentifiers.ORCID);

                    // Create FactFieldValues for orcid publication
                    factFieldValuesPublication = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesPublication.DimUserProfileId = dimUserProfile.Id;
                    factFieldValuesPublication.DimFieldDisplaySettingsId = dimFieldDisplaySettingsPublication.Id;
                    factFieldValuesPublication.DimOrcidPublicationId = dimOrcidPublication.Id;
                    factFieldValuesPublication.DimPidIdOrcidPutCode = dimPidOrcidPutCodePublication.Id;
                    _ttvContext.FactFieldValues.Add(factFieldValuesPublication);
                    await _ttvContext.SaveChangesAsync();
                }
            }

            return Ok(new ApiResponse(success: true));
        }
    }
}