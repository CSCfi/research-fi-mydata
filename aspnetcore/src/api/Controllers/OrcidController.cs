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
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace api.Controllers
{
    /*
     * OrcidController handles ORCID api related actions, such as getting ORCID record and saving ORCID data into database.
     */
    [Route("api/orcid")]
    [ApiController]
    [Authorize]
    public class OrcidController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;
        private readonly OrcidApiService _orcidApiService;
        private readonly OrcidJsonParserService _orcidJsonParserService;
        private readonly UtilityService _utilityService;
        private readonly ILogger<OrcidController> _logger;

        public OrcidController(TtvContext ttvContext, UserProfileService userProfileService, OrcidApiService orcidApiService, OrcidJsonParserService orcidJsonParserService, ILogger<OrcidController> logger, UtilityService utilityService)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _orcidApiService = orcidApiService;
            _orcidJsonParserService = orcidJsonParserService;
            _utilityService = utilityService;
            _logger = logger;
        }

        /// <summary>
        /// Trigger backend to get ORCID record and save data into TTV database.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        // TODO: Currently adding and updating ORCID data works, but detecting deleted ORCID data and deleting them is TTV database is not implemented.
        public async Task<IActionResult> Get()
        {
            // Get ORCID id.
            var orcidId = this.GetOrcidId();

            // Log request.
            _logger.LogInformation(this.GetLogPrefix() + " get ORCID data request");

            // Check that userprofile exists.
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                // Userprofile not found
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Get record JSON from ORCID
            var json = await _orcidApiService.GetRecord(orcidId);

            // Get DimUserProfile and related entities
            var dimUserProfile = await _ttvContext.DimUserProfiles.Where(dup => dup.Id == userprofileId)
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
                    .ThenInclude(ffv => ffv.DimKeyword).FirstOrDefaultAsync();


            // Get ORCID registered data source id
            var orcidRegisteredDataSourceId = await _userProfileService.GetOrcidRegisteredDataSourceId();


            // Get current DateTime
            DateTime currentDateTime = _utilityService.getCurrentDateTime();

            // Must use "Constants.SourceIdentifiers.ORCID" as value for "FactFieldValue.SourceId". It is used to identify what data can be deleted when userprofile is deleted.


            // Name
            var dimFieldDisplaySettingsName = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dimFieldDisplaysettingsName => dimFieldDisplaysettingsName.FieldIdentifier == Constants.FieldIdentifiers.PERSON_NAME && dimFieldDisplaysettingsName.SourceId == Constants.SourceIdentifiers.ORCID);
            // FactFieldValues
            var factFieldValuesName = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimFieldDisplaySettings == dimFieldDisplaySettingsName);
            if (factFieldValuesName != null)
            {
                // Update existing DimName
                var dimName = factFieldValuesName.DimName;
                dimName.LastName = _orcidJsonParserService.GetFamilyName(json).Value;
                dimName.FirstNames = _orcidJsonParserService.GetGivenNames(json).Value;
                dimName.Modified = _utilityService.getCurrentDateTime();
                _ttvContext.Entry(dimName).State = EntityState.Modified;
                // Update existing FactFieldValue
                factFieldValuesName.Show = true; // ORCID name is selected by default.
                factFieldValuesName.Modified = currentDateTime;
                _ttvContext.Entry(factFieldValuesName).State = EntityState.Modified;
            }
            else
            {
                // Create new DimName
                var dimName = new DimName()
                {
                    LastName = _orcidJsonParserService.GetFamilyName(json).Value,
                    FirstNames = _orcidJsonParserService.GetGivenNames(json).Value,
                    DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                    DimKnownPersonidFormerNames = -1,
                    SourceId = Constants.SourceIdentifiers.ORCID,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = currentDateTime,
                    Modified = currentDateTime,
                    DimRegisteredDataSourceId = orcidRegisteredDataSourceId
                };
                _ttvContext.DimNames.Add(dimName);

                factFieldValuesName = _userProfileService.GetEmptyFactFieldValueOrcid();
                factFieldValuesName.DimUserProfile = dimUserProfile;
                factFieldValuesName.DimFieldDisplaySettings = dimFieldDisplaySettingsName;
                factFieldValuesName.DimName = dimName;
                factFieldValuesName.Show = true;
                _ttvContext.FactFieldValues.Add(factFieldValuesName);
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
                    dimName_otherName.Modified = currentDateTime;
                    _ttvContext.Entry(dimName_otherName).State = EntityState.Modified;
                    // Update existing FactFieldValue
                    factFieldValuesOtherName.Modified = currentDateTime;
                    _ttvContext.Entry(factFieldValuesOtherName).State = EntityState.Modified;
                }
                else
                {
                    // Create new DimName for other name
                    var dimName_otherName = new DimName()
                    {
                        FullName = otherName.Value,
                        DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                        DimKnownPersonidFormerNames = -1,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimNames.Add(dimName_otherName);

                    // Add other name ORCID put code into DimPid
                    var dimPidOrcidPutCodeOtherName = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodeOtherName.PidContent = otherName.PutCode.GetDbValue();
                    dimPidOrcidPutCodeOtherName.PidType = "ORCID put code";
                    dimPidOrcidPutCodeOtherName.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodeOtherName.SourceId = Constants.SourceIdentifiers.ORCID;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeOtherName);

                    // Get DimFieldDisplaySettings for other name
                    var dimFieldDisplaySettingsOtherName = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsWebLink => dfdsWebLink.FieldIdentifier == Constants.FieldIdentifiers.PERSON_OTHER_NAMES && dfdsWebLink.SourceId == Constants.SourceIdentifiers.ORCID);

                    // Create FactFieldValues for other name
                    factFieldValuesOtherName = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesOtherName.DimUserProfile = dimUserProfile;
                    factFieldValuesOtherName.DimFieldDisplaySettings = dimFieldDisplaySettingsOtherName;
                    factFieldValuesOtherName.DimName = dimName_otherName;
                    factFieldValuesOtherName.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodeOtherName;
                    _ttvContext.FactFieldValues.Add(factFieldValuesOtherName);
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
                    dimWebLink.Modified = currentDateTime;
                    _ttvContext.Entry(dimWebLink).State = EntityState.Modified;

                    // Update existing FactFieldValue
                    factFieldValuesWebLink.Modified = currentDateTime;
                    _ttvContext.Entry(factFieldValuesWebLink).State = EntityState.Modified;
                }
                else
                {
                    // Create new DimWebLink
                    var dimWebLink = new DimWebLink()
                    {
                        Url = researchUrl.Url,
                        LinkLabel = researchUrl.UrlName,
                        DimOrganizationId = -1,
                        DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                        DimCallProgrammeId = -1,
                        DimFundingDecisionId = -1,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimWebLinks.Add(dimWebLink);

                    // Add web link ORCID put code into DimPid
                    var dimPidOrcidPutCodeWebLink = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodeWebLink.PidContent = researchUrl.PutCode.GetDbValue();
                    dimPidOrcidPutCodeWebLink.PidType = "ORCID put code";
                    dimPidOrcidPutCodeWebLink.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodeWebLink.SourceId = Constants.SourceIdentifiers.ORCID;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeWebLink);

                    // Get DimFieldDisplaySettings for weblink
                    var dimFieldDisplaySettingsWebLink = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsWebLink => dfdsWebLink.FieldIdentifier == Constants.FieldIdentifiers.PERSON_WEB_LINK && dfdsWebLink.SourceId == Constants.SourceIdentifiers.ORCID);

                    // Create FactFieldValues for weblink
                    factFieldValuesWebLink = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesWebLink.DimUserProfile = dimUserProfile;
                    factFieldValuesWebLink.DimFieldDisplaySettings = dimFieldDisplaySettingsWebLink;
                    factFieldValuesWebLink.DimWebLink = dimWebLink;
                    factFieldValuesWebLink.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodeWebLink;
                    _ttvContext.FactFieldValues.Add(factFieldValuesWebLink);
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
                    dimUserProfile.DimKnownPersonId,
                    orcidRegisteredDataSourceId
                );

                // Researcher description: DimFieldDisplaySettings
                var dimFieldDisplaySettingsResearcherDescription = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dimFieldDisplaySettingsResearcherDescription => dimFieldDisplaySettingsResearcherDescription.FieldIdentifier == Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION && dimFieldDisplaySettingsResearcherDescription.SourceId == Constants.SourceIdentifiers.ORCID);

                // Researcher description: FactFieldValues
                var factFieldValuesResearcherDescription = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimFieldDisplaySettingsId == dimFieldDisplaySettingsResearcherDescription.Id);
                if (factFieldValuesResearcherDescription == null)
                {
                    factFieldValuesResearcherDescription = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesResearcherDescription.DimUserProfile = dimUserProfile;
                    factFieldValuesResearcherDescription.DimFieldDisplaySettings = dimFieldDisplaySettingsResearcherDescription;
                    factFieldValuesResearcherDescription.DimResearcherDescription = dimResearcherDescription;
                    _ttvContext.FactFieldValues.Add(factFieldValuesResearcherDescription);
                }
                else
                {
                    factFieldValuesResearcherDescription.Modified = currentDateTime;
                    _ttvContext.Entry(factFieldValuesResearcherDescription).State = EntityState.Modified;
                }
            }



            // Email
            var emails = _orcidJsonParserService.GetEmails(json);
            foreach (OrcidEmail email in emails)
            {
                // Email: DimEmailAddrressess
                var dimEmailAddress = await _userProfileService.AddOrUpdateDimEmailAddress(
                    email.Value,
                    dimUserProfile.DimKnownPersonId,
                    orcidRegisteredDataSourceId
                );

                // Email: DimFieldDisplaySettings
                var dimFieldDisplaySettingsEmailAddress = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dimFieldDisplaySettingsEmailAddress => dimFieldDisplaySettingsEmailAddress.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS && dimFieldDisplaySettingsEmailAddress.SourceId == Constants.SourceIdentifiers.ORCID);

                // Email: FactFieldValues
                var factFieldValuesEmailAddress = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimFieldDisplaySettingsId == dimFieldDisplaySettingsEmailAddress.Id);
                if (factFieldValuesEmailAddress == null)
                {
                    factFieldValuesEmailAddress = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesEmailAddress.DimUserProfile = dimUserProfile;
                    factFieldValuesEmailAddress.DimFieldDisplaySettings = dimFieldDisplaySettingsEmailAddress;
                    factFieldValuesEmailAddress.DimEmailAddrress = dimEmailAddress;
                    _ttvContext.FactFieldValues.Add(factFieldValuesEmailAddress);
                }
                else
                {
                    factFieldValuesEmailAddress.Modified = currentDateTime;
                    _ttvContext.Entry(factFieldValuesEmailAddress).State = EntityState.Modified;
                }
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
                    dimKeyword.Modified = currentDateTime;
                    _ttvContext.Entry(dimKeyword).State = EntityState.Modified;
                    // Update existing FactFieldValue
                    factFieldValuesKeyword.Modified = currentDateTime;
                    _ttvContext.Entry(factFieldValuesKeyword).State = EntityState.Modified;
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
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimKeywords.Add(dimKeyword);

                    // Add keyword ORCID put code into DimPid
                    var dimPidOrcidPutCodeKeyword = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodeKeyword.PidContent = keyword.PutCode.GetDbValue();
                    dimPidOrcidPutCodeKeyword.PidType = "ORCID put code";
                    dimPidOrcidPutCodeKeyword.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodeKeyword.SourceId = Constants.SourceIdentifiers.ORCID;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeKeyword);

                    // Create FactFieldValues for keyword
                    factFieldValuesKeyword = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesKeyword.DimUserProfile = dimUserProfile;
                    factFieldValuesKeyword.DimFieldDisplaySettings = dimFieldDisplaySettingsKeyword;
                    factFieldValuesKeyword.DimKeyword = dimKeyword;
                    factFieldValuesKeyword.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodeKeyword;
                    _ttvContext.FactFieldValues.Add(factFieldValuesKeyword);
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
                    dimPid.Modified = currentDateTime;
                    _ttvContext.Entry(dimPid).State = EntityState.Modified;
                    // Update existing FactFieldValue
                    factFieldValuesExternalIdentifier.Modified = currentDateTime;
                    _ttvContext.Entry(factFieldValuesExternalIdentifier).State = EntityState.Modified;
                }
                else
                {
                    // Create new DimPid (external identifier is stored into DimPid)
                    var dimPid = _userProfileService.GetEmptyDimPid();
                    dimPid.PidContent = externalIdentifier.ExternalIdValue;
                    dimPid.PidType = externalIdentifier.ExternalIdType;
                    dimPid.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPid.SourceId = Constants.SourceIdentifiers.ORCID;
                    _ttvContext.DimPids.Add(dimPid);

                    // Add ORCID put code into DimPid
                    var dimPidOrcidPutCodeExternalIdentifier = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodeExternalIdentifier.PidContent = externalIdentifier.PutCode.GetDbValue();
                    dimPidOrcidPutCodeExternalIdentifier.PidType = "ORCID put code";
                    dimPidOrcidPutCodeExternalIdentifier.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodeExternalIdentifier.SourceId = Constants.SourceIdentifiers.ORCID;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeExternalIdentifier);

                    // Create FactFieldValues for external identifier
                    factFieldValuesExternalIdentifier = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesExternalIdentifier.DimUserProfile = dimUserProfile;
                    factFieldValuesExternalIdentifier.DimFieldDisplaySettings = dimFieldDisplaySettingsExternalIdentifier;
                    factFieldValuesExternalIdentifier.DimPid = dimPid;
                    factFieldValuesExternalIdentifier.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodeExternalIdentifier;
                    _ttvContext.FactFieldValues.Add(factFieldValuesExternalIdentifier);
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
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimDates.Add(startDate);
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
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimDates.Add(endDate);
                }

                if (factFieldValuesEducation != null)
                {
                    // Update existing DimEducation
                    var dimEducation = factFieldValuesEducation.DimEducation; 
                    dimEducation.NameEn = education.RoleTitle;
                    dimEducation.DegreeGrantingInstitutionName = education.OrganizationName;
                    dimEducation.DimStartDateNavigation = startDate;
                    dimEducation.DimEndDateNavigation = endDate;
                    dimEducation.Modified = currentDateTime;
                    _ttvContext.Entry(dimEducation).State = EntityState.Modified;

                    // Update existing FactFieldValue
                    factFieldValuesEducation.Modified = currentDateTime;
                    _ttvContext.Entry(factFieldValuesEducation).State = EntityState.Modified;

                }
                else
                {
                    // Create new DimEducation
                    var dimEducation = new DimEducation()
                    {
                        NameEn = education.RoleTitle,
                        DegreeGrantingInstitutionName = education.OrganizationName,
                        DimStartDateNavigation = startDate,
                        DimEndDateNavigation = endDate,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimEducations.Add(dimEducation);

                    // Add education ORCID put code into DimPid
                    var dimPidOrcidPutCodeEducation = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodeEducation.PidContent = education.PutCode.GetDbValue();
                    dimPidOrcidPutCodeEducation.PidType = "ORCID put code";
                    dimPidOrcidPutCodeEducation.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodeEducation.SourceId = Constants.SourceIdentifiers.ORCID;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeEducation);

                    // Get DimFieldDisplaySettings for education
                    var dimFieldDisplaySettingsEducation = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsEducation => dfdsEducation.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_EDUCATION && dfdsEducation.SourceId == Constants.SourceIdentifiers.ORCID);

                    // Create FactFieldValues for education
                    factFieldValuesEducation = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesEducation.DimUserProfile = dimUserProfile;
                    factFieldValuesEducation.DimFieldDisplaySettings = dimFieldDisplaySettingsEducation;
                    factFieldValuesEducation.DimEducation = dimEducation;
                    factFieldValuesEducation.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodeEducation;
                    _ttvContext.FactFieldValues.Add(factFieldValuesEducation);
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
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimDates.Add(startDate);
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
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimDates.Add(endDate);
                }

                // TODO: DimOrganization handling

                if (factFieldValuesAffiliation != null)
                {
                    // Update existing DimAffiliation
                    var dimAffiliation = factFieldValuesAffiliation.DimAffiliation;
                    dimAffiliation.PositionNameEn = employment.RoleTitle;
                    dimAffiliation.StartDateNavigation = startDate;
                    dimAffiliation.EndDateNavigation = endDate;
                    _ttvContext.Entry(dimAffiliation).State = EntityState.Modified;
                    dimAffiliation.Modified = currentDateTime;

                    // Update related DimOrganization
                    // TODO: DimOrganization handling
                    var dimOrganization = dimAffiliation.DimOrganization;
                    dimOrganization.NameEn = employment.OrganizationName;
                    _ttvContext.Entry(dimOrganization).State = EntityState.Modified;

                    // Update existing FactFieldValue
                    factFieldValuesAffiliation.Modified = currentDateTime;
                    _ttvContext.Entry(factFieldValuesAffiliation).State = EntityState.Modified;
                }
                else
                {
                    // Create new related DimOrganization.
                    // For demo: include department name in field NameUnd.
                    // TODO: DimOrganization handling
                    var dimOrganization = new DimOrganization()
                    {
                        DimSectorid = -1,
                        NameEn = employment.OrganizationName,
                        NameUnd = employment.DepartmentName, // TODO: this is a temporary solution for demo.
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };

                    _ttvContext.DimOrganizations.Add(dimOrganization);


                    // Create new DimAffiliation
                    var dimAffiliation = new DimAffiliation()
                    {
                        DimOrganization = dimOrganization,
                        StartDateNavigation = startDate,
                        EndDateNavigation = endDate,
                        PositionNameEn = employment.RoleTitle,
                        AffiliationType = -1,
                        SourceId = Constants.SourceIdentifiers.ORCID,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimAffiliations.Add(dimAffiliation);

                    // Add employment (=affiliation) ORCID put code into DimPid
                    var dimPidOrcidPutCodeEmployment = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodeEmployment.PidContent = employment.PutCode.GetDbValue();
                    dimPidOrcidPutCodeEmployment.PidType = "ORCID put code";
                    dimPidOrcidPutCodeEmployment.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodeEmployment.SourceId = Constants.SourceIdentifiers.ORCID;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeEmployment);

                    // Get DimFieldDisplaySettings for affiliation
                    var dimFieldDisplaySettingsAffiliation = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsAffiliation => dfdsAffiliation.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_AFFILIATION && dfdsAffiliation.SourceId == Constants.SourceIdentifiers.ORCID);

                    // Create FactFieldValues for affiliation
                    factFieldValuesAffiliation = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesAffiliation.DimUserProfile = dimUserProfile;
                    factFieldValuesAffiliation.DimFieldDisplaySettings = dimFieldDisplaySettingsAffiliation;
                    factFieldValuesAffiliation.DimAffiliation = dimAffiliation;
                    factFieldValuesAffiliation.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodeEmployment;
                    _ttvContext.FactFieldValues.Add(factFieldValuesAffiliation);
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
                    dimOrcidPublication.Modified = currentDateTime;
                    _ttvContext.Entry(dimOrcidPublication).State = EntityState.Modified;
                    // Update existing FactFieldValue
                    factFieldValuesPublication.Modified = currentDateTime;
                    _ttvContext.Entry(factFieldValuesPublication).State = EntityState.Modified;
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
                    dimOrcidPublication.Created = currentDateTime;
                    _ttvContext.DimOrcidPublications.Add(dimOrcidPublication);

                    // Add publication's ORCID put code into DimPid
                    var dimPidOrcidPutCodePublication = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodePublication.PidContent = orcidPublication.PutCode.GetDbValue();
                    dimPidOrcidPutCodePublication.PidType = "ORCID put code";
                    dimPidOrcidPutCodePublication.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodePublication.SourceId = Constants.SourceIdentifiers.ORCID;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodePublication);

                    // Get DimFieldDisplaySettings for orcid publication
                    var dimFieldDisplaySettingsOrcidPublication = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsPublication => dfdsPublication.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID && dfdsPublication.SourceId == Constants.SourceIdentifiers.ORCID);

                    // Create FactFieldValues for orcid publication
                    factFieldValuesPublication = _userProfileService.GetEmptyFactFieldValueOrcid();
                    factFieldValuesPublication.DimUserProfile = dimUserProfile;
                    factFieldValuesPublication.DimFieldDisplaySettings = dimFieldDisplaySettingsOrcidPublication;
                    factFieldValuesPublication.DimOrcidPublication = dimOrcidPublication;
                    factFieldValuesPublication.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodePublication;
                    _ttvContext.FactFieldValues.Add(factFieldValuesPublication);
                }
            }
            
            await _ttvContext.SaveChangesAsync();

            _logger.LogInformation(this.GetLogPrefix() + " get ORCID data success");

            return Ok(new ApiResponse(success: true));
        }
    }
}