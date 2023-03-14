using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using api.Controllers;
using api.Models.Common;
using api.Models.Orcid;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Nest;
using Serilog.Core;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Constants = api.Models.Common.Constants;

namespace api.Services
{
    /*
     * OrcidImportService imports ORCID data into user profile.
     */
    public class OrcidImportService : IOrcidImportService
    {
        private readonly TtvContext _ttvContext;
        private readonly IUserProfileService _userProfileService;
        private readonly IOrcidJsonParserService _orcidJsonParserService;
        private readonly IOrganizationHandlerService _organizationHandlerService;
        private readonly IDataSourceHelperService _dataSourceHelperService;
        private readonly IUtilityService _utilityService;
        private readonly ILogger<OrcidImportService> _logger;

        public OrcidImportService(TtvContext ttvContext, IUserProfileService userProfileService, IOrcidJsonParserService orcidJsonParserService,
            IOrganizationHandlerService organizationHandlerService, IUtilityService utilityService, IDataSourceHelperService dataSourceHelperService,
            ILogger<OrcidImportService> logger)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _orcidJsonParserService = orcidJsonParserService;
            _organizationHandlerService = organizationHandlerService;
            _utilityService = utilityService;
            _dataSourceHelperService = dataSourceHelperService;
            _logger = logger;
        }


        /*
         *  Add DimDates entities needed in ORCID record.
         */
        public async Task AddDimDates(string orcidRecordJson, DateTime currentDateTime)
        {
            // Education DimDates
            List<OrcidEducation> educations = _orcidJsonParserService.GetEducations(orcidRecordJson);
            foreach (OrcidEducation education in educations)
            {
                // Start date
                DimDate educationStartDate =
                    await _ttvContext.DimDates.FirstOrDefaultAsync(
                        dd => dd.Year == education.StartDate.Year &&
                        dd.Month == education.StartDate.Month &&
                        dd.Day == education.StartDate.Day);
                if (educationStartDate == null)
                {
                    educationStartDate = new DimDate()
                    {
                        Year = education.StartDate.Year,
                        Month = education.StartDate.Month,
                        Day = education.StartDate.Day,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimDates.Add(educationStartDate);
                    await _ttvContext.SaveChangesAsync();
                }

                // End date
                DimDate educationEndDate =
                    await _ttvContext.DimDates.FirstOrDefaultAsync(
                        ed => ed.Year == education.EndDate.Year &&
                        ed.Month == education.EndDate.Month &&
                        ed.Day == education.EndDate.Day);
                if (educationEndDate == null)
                {
                    educationEndDate = new DimDate()
                    {
                        Year = education.EndDate.Year,
                        Month = education.EndDate.Month,
                        Day = education.EndDate.Day,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimDates.Add(educationEndDate);
                    await _ttvContext.SaveChangesAsync();
                }
            }

            // Employment DimDates
            List<OrcidEmployment> employments = _orcidJsonParserService.GetEmployments(orcidRecordJson);
            foreach (OrcidEmployment employment in employments)
            {
                // Start date
                DimDate employmentStartDate =
                    await _ttvContext.DimDates.FirstOrDefaultAsync(
                        dd => dd.Year == employment.StartDate.Year &&
                        dd.Month == employment.StartDate.Month &&
                        dd.Day == employment.StartDate.Day);
                if (employmentStartDate == null)
                {
                    employmentStartDate = new DimDate()
                    {
                        Year = employment.StartDate.Year,
                        Month = employment.StartDate.Month,
                        Day = employment.StartDate.Day,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimDates.Add(employmentStartDate);
                    await _ttvContext.SaveChangesAsync();
                }

                // End date
                DimDate employmentEndDate = await _ttvContext.DimDates.FirstOrDefaultAsync(
                    dd => dd.Year == employment.EndDate.Year &&
                    dd.Month == employment.EndDate.Month &&
                    dd.Day == employment.EndDate.Day);
                if (employmentEndDate == null)
                {
                    employmentEndDate = new DimDate()
                    {
                        Year = employment.EndDate.Year,
                        Month = employment.EndDate.Month,
                        Day = employment.EndDate.Day,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimDates.Add(employmentEndDate);
                    await _ttvContext.SaveChangesAsync();
                }
            }
        }


        /*
         * Import ORCID record json into user profile.
         */
        public async Task<bool> ImportOrcidRecordJsonIntoUserProfile(int userprofileId, string orcidRecordJson)
        {
            // Get ORCID registered data source id.
            int orcidRegisteredDataSourceId = _dataSourceHelperService.DimRegisteredDataSourceId_ORCID;

            // Get DimUserProfile and related entities
            string queryTag = $"Insert ORCID data, dim_user_profile.id={userprofileId}";
            DimUserProfile dimUserProfile = await _ttvContext.DimUserProfiles.TagWith(queryTag).Where(dup => dup.Id == userprofileId)
                .Include(dup => dup.DimFieldDisplaySettings)
                // DimRegisteredDataSource
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimRegisteredDataSource)
                        .ThenInclude(drds => drds.DimOrganization)
                // DimName
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimName)
                // DimWebLink
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimWebLink)
                // DimFundingDecision
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                // DimProfileOnlyPublication
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimProfileOnlyPublication)
                // DimPid
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimPid)
                // DimPidIdOrcidPutCodeNavigation
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimPidIdOrcidPutCodeNavigation)
                // DimResearchActivity
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimResearchActivity)
                // DimEducation
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimEducation)
                        .ThenInclude(de => de.DimStartDateNavigation)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimEducation)
                        .ThenInclude(de => de.DimEndDateNavigation)
                // DimAffiliation
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimAffiliation)
                        .ThenInclude(da => da.DimOrganization)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimAffiliation)
                        .ThenInclude(da => da.StartDateNavigation)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimAffiliation)
                        .ThenInclude(da => da.EndDateNavigation)
                // DimTelephoneNumber
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimTelephoneNumber)
                // DimEmailAddrress
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimEmailAddrress)
                // DimResearcherDescription
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimResearcherDescription)
                // DimIdentifierlessData
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimIdentifierlessData)
                        .ThenInclude(did => did.InverseDimIdentifierlessData) // DimIdentifierlessData can have a child entity.
                // DimKeyword
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimKeyword).FirstOrDefaultAsync();

            // Get current DateTime
            DateTime currentDateTime = _utilityService.GetCurrentDateTime();

            // Add DimDates.
            await AddDimDates(orcidRecordJson, currentDateTime);

            // Helper object to store processed IDs, used when deciding what data needs to be removed.
            OrcidImportHelper orcidImportHelper = new();

            // Name
            DimFieldDisplaySetting dimFieldDisplaySettingsName =
                dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dimFieldDisplaysettingsName => dimFieldDisplaysettingsName.FieldIdentifier == Constants.FieldIdentifiers.PERSON_NAME);
            // FactFieldValues
            FactFieldValue factFieldValuesName =
                dimUserProfile.FactFieldValues.FirstOrDefault(
                    ffv => ffv.DimFieldDisplaySettings.Id == dimFieldDisplaySettingsName.Id && ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId);
            if (factFieldValuesName != null)
            {
                // Update existing DimName
                DimName dimName = factFieldValuesName.DimName;
                dimName.LastName = _orcidJsonParserService.GetFamilyName(orcidRecordJson).Value;
                dimName.FirstNames = _orcidJsonParserService.GetGivenNames(orcidRecordJson).Value;
                dimName.Modified = currentDateTime;
                // Update existing FactFieldValue
                factFieldValuesName.Modified = currentDateTime;
                // Mark as processed
                orcidImportHelper.dimNameIds.Add(factFieldValuesName.DimName.Id);
            }
            else
            {
                // Create new DimName
                DimName dimName = new()
                {
                    LastName = _orcidJsonParserService.GetFamilyName(orcidRecordJson).Value,
                    FirstNames = _orcidJsonParserService.GetGivenNames(orcidRecordJson).Value,
                    DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                    DimRegisteredDataSourceId = orcidRegisteredDataSourceId,
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = currentDateTime,
                    Modified = currentDateTime
                };
                _ttvContext.DimNames.Add(dimName);
                // Create FactFieldValues for name
                factFieldValuesName = _userProfileService.GetEmptyFactFieldValue();
                factFieldValuesName.DimUserProfile = dimUserProfile;
                factFieldValuesName.DimFieldDisplaySettings = dimFieldDisplaySettingsName;
                factFieldValuesName.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                factFieldValuesName.DimName = dimName;
                factFieldValuesName.Show = true; // ORCID name is selected by default.
                _ttvContext.FactFieldValues.Add(factFieldValuesName);
            }


            // Other names
            List<OrcidOtherName> otherNames = _orcidJsonParserService.GetOtherNames(orcidRecordJson);
            // Get DimFieldDisplaySettings for other name
            DimFieldDisplaySetting dimFieldDisplaySettingsOtherName =
                dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsOtherName => dfdsOtherName.FieldIdentifier == Constants.FieldIdentifiers.PERSON_OTHER_NAMES);
            foreach (OrcidOtherName otherName in otherNames)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimPid
                FactFieldValue factFieldValuesOtherName =
                    dimUserProfile.FactFieldValues.FirstOrDefault(ffv =>
                        ffv.DimFieldDisplaySettings == dimFieldDisplaySettingsOtherName &&
                        ffv.DimPidIdOrcidPutCode > 0 &&
                        ffv.DimPidIdOrcidPutCodeNavigation.PidContent == otherName.PutCode.Value.ToString());

                if (factFieldValuesOtherName != null)
                {
                    // Update existing DimName
                    DimName dimName_otherName = factFieldValuesOtherName.DimName;
                    dimName_otherName.FullName = otherName.Value;
                    dimName_otherName.Modified = currentDateTime;
                    // Update existing FactFieldValue
                    factFieldValuesOtherName.Modified = currentDateTime;
                    // Mark as processed
                    orcidImportHelper.dimNameIds.Add(factFieldValuesOtherName.DimName.Id);
                }
                else
                {
                    // Create new DimName for other name
                    DimName dimName_otherName = new()
                    {
                        FullName = otherName.Value,
                        DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimNames.Add(dimName_otherName);

                    // Add other name ORCID put code into DimPid
                    DimPid dimPidOrcidPutCodeOtherName = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodeOtherName.PidContent = otherName.PutCode.GetDbValue();
                    dimPidOrcidPutCodeOtherName.PidType = Constants.PidTypes.ORCID_PUT_CODE;
                    dimPidOrcidPutCodeOtherName.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodeOtherName.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeOtherName);

                    // Create FactFieldValues for other name
                    factFieldValuesOtherName = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesOtherName.DimUserProfile = dimUserProfile;
                    factFieldValuesOtherName.DimFieldDisplaySettings = dimFieldDisplaySettingsOtherName;
                    factFieldValuesOtherName.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    factFieldValuesOtherName.DimName = dimName_otherName;
                    factFieldValuesOtherName.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodeOtherName;
                    _ttvContext.FactFieldValues.Add(factFieldValuesOtherName);
                }
            }


            // Researcher urls
            List<OrcidResearcherUrl> researcherUrls = _orcidJsonParserService.GetResearcherUrls(orcidRecordJson);
            // Get DimFieldDisplaySettings for weblink
            DimFieldDisplaySetting dimFieldDisplaySettingsWebLink =
                dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsWebLink => dfdsWebLink.FieldIdentifier == Constants.FieldIdentifiers.PERSON_WEB_LINK);
            foreach (OrcidResearcherUrl researchUrl in researcherUrls)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimPid
                FactFieldValue factFieldValuesWebLink =
                    dimUserProfile.FactFieldValues.FirstOrDefault(ffv =>
                        ffv.DimFieldDisplaySettings == dimFieldDisplaySettingsWebLink &&
                        ffv.DimPidIdOrcidPutCode > 0 &&
                        ffv.DimPidIdOrcidPutCodeNavigation.PidContent == researchUrl.PutCode.Value.ToString());

                if (factFieldValuesWebLink != null)
                {
                    // Update existing DimWebLink
                    DimWebLink dimWebLink = factFieldValuesWebLink.DimWebLink;
                    dimWebLink.Url = researchUrl.Url;
                    dimWebLink.LinkLabel = researchUrl.UrlName;
                    dimWebLink.Modified = currentDateTime;
                    // Update existing FactFieldValue
                    factFieldValuesWebLink.Modified = currentDateTime;
                    // Mark as processed
                    orcidImportHelper.dimWebLinkIds.Add(factFieldValuesWebLink.DimWebLink.Id);
                }
                else
                {
                    // Create new DimWebLink
                    DimWebLink dimWebLink = new()
                    {
                        Url = researchUrl.Url,
                        LinkLabel = researchUrl.UrlName,
                        DimOrganizationId = -1,
                        DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                        DimCallProgrammeId = -1,
                        DimFundingDecisionId = -1,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimWebLinks.Add(dimWebLink);

                    // Add web link ORCID put code into DimPid
                    DimPid dimPidOrcidPutCodeWebLink = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodeWebLink.PidContent = researchUrl.PutCode.GetDbValue();
                    dimPidOrcidPutCodeWebLink.PidType = Constants.PidTypes.ORCID_PUT_CODE;
                    dimPidOrcidPutCodeWebLink.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodeWebLink.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeWebLink);

                    // Create FactFieldValues for weblink
                    factFieldValuesWebLink = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesWebLink.DimUserProfile = dimUserProfile;
                    factFieldValuesWebLink.DimFieldDisplaySettings = dimFieldDisplaySettingsWebLink;
                    factFieldValuesWebLink.DimWebLink = dimWebLink;
                    factFieldValuesWebLink.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    factFieldValuesWebLink.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodeWebLink;
                    _ttvContext.FactFieldValues.Add(factFieldValuesWebLink);
                }
            }


            // Researcher description
            OrcidBiography biography = _orcidJsonParserService.GetBiography(orcidRecordJson);
            // Get DimFieldDisplaySettings for researcher description
            DimFieldDisplaySetting dimFieldDisplaySettingsResearcherDescription =
                dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(
                    dimFieldDisplaySettingsResearcherDescription => dimFieldDisplaySettingsResearcherDescription.FieldIdentifier == Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION);
            if (biography != null)
            {
                // Check if FactFieldValues contains entry pointing to DimResearcherDescriptions, which has ORCID as data source
                FactFieldValue factFieldValuesResearcherDescription =
                    dimUserProfile.FactFieldValues.FirstOrDefault(ffv =>
                        ffv.DimFieldDisplaySettings == dimFieldDisplaySettingsResearcherDescription &&
                        ffv.DimResearcherDescriptionId > 0 &&
                        ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId);

                if (factFieldValuesResearcherDescription != null)
                {
                    // Update existing DimResearcherDescription
                    factFieldValuesResearcherDescription.DimResearcherDescription.ResearchDescriptionEn = biography.Value;
                    factFieldValuesResearcherDescription.DimResearcherDescription.Modified = currentDateTime;
                    // Update existing FactFieldValue
                    factFieldValuesResearcherDescription.Modified = currentDateTime;
                    // Mark as processed
                    orcidImportHelper.dimResearcherDescriptionIds.Add(factFieldValuesResearcherDescription.DimResearcherDescription.Id);
                }
                else
                {   // Create new DimResearcherDescription
                    DimResearcherDescription dimResearcherDescription = new ()
                    {
                        ResearchDescriptionFi = "",
                        ResearchDescriptionEn = biography.Value,
                        ResearchDescriptionSv = "",
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime,
                        DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId
                    };
                    _ttvContext.DimResearcherDescriptions.Add(dimResearcherDescription);

                    // Create FactFieldValues for researcher description
                    factFieldValuesResearcherDescription = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesResearcherDescription.DimUserProfile = dimUserProfile;
                    factFieldValuesResearcherDescription.DimFieldDisplaySettings = dimFieldDisplaySettingsResearcherDescription;
                    factFieldValuesResearcherDescription.DimResearcherDescription = dimResearcherDescription;
                    factFieldValuesResearcherDescription.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    _ttvContext.FactFieldValues.Add(factFieldValuesResearcherDescription);
                }
            }


            // Email
            List<OrcidEmail> emails = _orcidJsonParserService.GetEmails(orcidRecordJson);
            // Email: DimFieldDisplaySettings
            DimFieldDisplaySetting dimFieldDisplaySettingsEmailAddress =
                dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(
                    dimFieldDisplaySettingsEmailAddress => dimFieldDisplaySettingsEmailAddress.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS);
            foreach (OrcidEmail email in emails)
            {
                // Check if email already exists
                FactFieldValue factFieldValuesEmail =
                    dimUserProfile.FactFieldValues.FirstOrDefault(ffv =>
                        ffv.DimFieldDisplaySettings == dimFieldDisplaySettingsEmailAddress &&
                        ffv.DimEmailAddrress.Email == email.Value);

                if (factFieldValuesEmail != null)
                {
                    // Email address is matched by value, so no modification is made here.

                    // Update existing FactFieldValue
                    factFieldValuesEmail.Modified = currentDateTime;
                    // Mark as processed
                    orcidImportHelper.dimEmailAddressIds.Add(factFieldValuesEmail.DimEmailAddrressId);
                }
                else
                {
                    // Create new DimEmailAddrress
                    DimEmailAddrress dimEmailAddress = new()
                    {
                        Email = email.Value,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime,
                        DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId
                    };
                    _ttvContext.DimEmailAddrresses.Add(dimEmailAddress);

                    // Email: FactFieldValues
                    factFieldValuesEmail = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesEmail.DimUserProfile = dimUserProfile;
                    factFieldValuesEmail.DimFieldDisplaySettings = dimFieldDisplaySettingsEmailAddress;
                    factFieldValuesEmail.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    factFieldValuesEmail.DimEmailAddrress = dimEmailAddress;

                    // Add email address ORCID put code into DimPid.
                    // In ORCID data the email has field put code, but it seems to be always null.
                    if (email.PutCode.Value != null)
                    {
                        DimPid dimPidOrcidPutCodeEmail = _userProfileService.GetEmptyDimPid();
                        dimPidOrcidPutCodeEmail.PidContent = email.PutCode.GetDbValue();
                        dimPidOrcidPutCodeEmail.PidType = Constants.PidTypes.ORCID_PUT_CODE;
                        dimPidOrcidPutCodeEmail.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                        dimPidOrcidPutCodeEmail.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                        _ttvContext.DimPids.Add(dimPidOrcidPutCodeEmail);

                        factFieldValuesEmail.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodeEmail;
                    }

                    _ttvContext.FactFieldValues.Add(factFieldValuesEmail);
                }
            }


            // Keyword
            List<OrcidKeyword> keywords = _orcidJsonParserService.GetKeywords(orcidRecordJson);
            // Get DimFieldDisplaySettings for keyword
            DimFieldDisplaySetting dimFieldDisplaySettingsKeyword =
                dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsKeyword => dfdsKeyword.FieldIdentifier == Constants.FieldIdentifiers.PERSON_KEYWORD);
            // Collect list of processed FactFieldValues related to keyword. Needed when deleting keywords.
            List<FactFieldValue> processedKeywordFactFieldValues = new();
            foreach (OrcidKeyword keyword in keywords)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimKeyword
                FactFieldValue factFieldValuesKeyword =
                    dimUserProfile.FactFieldValues.FirstOrDefault(ffv =>
                        ffv.DimFieldDisplaySettings == dimFieldDisplaySettingsKeyword &&
                        ffv.DimPidIdOrcidPutCode > 0 &&
                        ffv.DimPidIdOrcidPutCodeNavigation.PidContent == keyword.PutCode.Value.ToString());

                if (factFieldValuesKeyword != null)
                {
                    // Update existing DimKeyword
                    DimKeyword dimKeyword = factFieldValuesKeyword.DimKeyword;
                    dimKeyword.Keyword = keyword.Value;
                    dimKeyword.Modified = currentDateTime;
                    // Update existing FactFieldValue
                    factFieldValuesKeyword.Modified = currentDateTime;
                    // Mark as processed
                    orcidImportHelper.dimKeywordIds.Add(factFieldValuesKeyword.DimKeywordId);
                }
                else
                {
                    // Create new DimKeyword
                    DimKeyword dimKeyword = new()
                    {
                        Keyword = keyword.Value,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimKeywords.Add(dimKeyword);

                    // Add keyword ORCID put code into DimPid
                    DimPid dimPidOrcidPutCodeKeyword = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodeKeyword.PidContent = keyword.PutCode.GetDbValue();
                    dimPidOrcidPutCodeKeyword.PidType = Constants.PidTypes.ORCID_PUT_CODE;
                    dimPidOrcidPutCodeKeyword.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodeKeyword.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeKeyword);

                    // Create FactFieldValues for keyword
                    factFieldValuesKeyword = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesKeyword.DimUserProfile = dimUserProfile;
                    factFieldValuesKeyword.DimFieldDisplaySettings = dimFieldDisplaySettingsKeyword;
                    factFieldValuesKeyword.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    factFieldValuesKeyword.DimKeyword = dimKeyword;
                    factFieldValuesKeyword.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodeKeyword;
                    _ttvContext.FactFieldValues.Add(factFieldValuesKeyword);
                }
                processedKeywordFactFieldValues.Add(factFieldValuesKeyword);
            }



            // External identifier (=DimPid)
            List<OrcidExternalIdentifier> externalIdentifiers = _orcidJsonParserService.GetExternalIdentifiers(orcidRecordJson);
            // Get DimFieldDisplaySettings for keyword
            DimFieldDisplaySetting dimFieldDisplaySettingsExternalIdentifier =
                dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsKeyword => dfdsKeyword.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER);
            foreach (OrcidExternalIdentifier externalIdentifier in externalIdentifiers)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimPid
                FactFieldValue factFieldValuesExternalIdentifier =
                    dimUserProfile.FactFieldValues.FirstOrDefault(ffv =>
                        ffv.DimFieldDisplaySettings == dimFieldDisplaySettingsExternalIdentifier &&
                        ffv.DimPidIdOrcidPutCode > 0 &&
                        ffv.DimPidIdOrcidPutCodeNavigation.PidContent == externalIdentifier.PutCode.Value.ToString());

                if (factFieldValuesExternalIdentifier != null)
                {
                    // Update existing DimPid
                    DimPid dimPid = factFieldValuesExternalIdentifier.DimPid;
                    dimPid.PidContent = externalIdentifier.ExternalIdValue;
                    dimPid.PidType = externalIdentifier.ExternalIdType;
                    dimPid.Modified = currentDateTime;
                    // Update existing FactFieldValue
                    factFieldValuesExternalIdentifier.Modified = currentDateTime;
                    // Mark as processed
                    orcidImportHelper.dimPidIds.Add(factFieldValuesExternalIdentifier.DimPidId);
                }
                else
                {
                    // Create new DimPid (external identifier is stored into DimPid)
                    DimPid dimPid = _userProfileService.GetEmptyDimPid();
                    dimPid.PidContent = externalIdentifier.ExternalIdValue;
                    dimPid.PidType = externalIdentifier.ExternalIdType;
                    dimPid.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPid.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    _ttvContext.DimPids.Add(dimPid);

                    // Add ORCID put code into DimPid
                    DimPid dimPidOrcidPutCodeExternalIdentifier = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodeExternalIdentifier.PidContent = externalIdentifier.PutCode.GetDbValue();
                    dimPidOrcidPutCodeExternalIdentifier.PidType = Constants.PidTypes.ORCID_PUT_CODE;
                    dimPidOrcidPutCodeExternalIdentifier.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodeExternalIdentifier.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeExternalIdentifier);

                    // Create FactFieldValues for external identifier
                    factFieldValuesExternalIdentifier = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesExternalIdentifier.DimUserProfile = dimUserProfile;
                    factFieldValuesExternalIdentifier.DimFieldDisplaySettings = dimFieldDisplaySettingsExternalIdentifier;
                    factFieldValuesExternalIdentifier.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    factFieldValuesExternalIdentifier.DimPid = dimPid;
                    factFieldValuesExternalIdentifier.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodeExternalIdentifier;
                    _ttvContext.FactFieldValues.Add(factFieldValuesExternalIdentifier);
                }
            }


            // Education
            List<OrcidEducation> educations = _orcidJsonParserService.GetEducations(orcidRecordJson);
            // Get DimFieldDisplaySettings for education
            DimFieldDisplaySetting dimFieldDisplaySettingsEducation =
                dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsEducation => dfdsEducation.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_EDUCATION);
            foreach (OrcidEducation education in educations)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimEducation
                FactFieldValue factFieldValuesEducation =
                    dimUserProfile.FactFieldValues.FirstOrDefault(ffv =>
                        ffv.DimFieldDisplaySettings == dimFieldDisplaySettingsEducation &&
                        ffv.DimPidIdOrcidPutCode > 0 &&
                        ffv.DimPidIdOrcidPutCodeNavigation.PidContent == education.PutCode.Value.ToString());

                // Start date
                DimDate educationStartDate = await _ttvContext.DimDates
                    .FirstOrDefaultAsync(dd => dd.Year == education.StartDate.Year && dd.Month == education.StartDate.Month && dd.Day == education.StartDate.Day);

                // End date
                DimDate educationEndDate = await _ttvContext.DimDates
                    .FirstOrDefaultAsync(ed => ed.Year == education.EndDate.Year && ed.Month == education.EndDate.Month && ed.Day == education.EndDate.Day);

                if (factFieldValuesEducation != null)
                {
                    // Update existing DimEducation
                    DimEducation dimEducation = factFieldValuesEducation.DimEducation;
                    dimEducation.NameEn = education.RoleTitle;
                    dimEducation.DegreeGrantingInstitutionName = education.OrganizationName;
                    dimEducation.DimStartDateNavigation = educationStartDate;
                    dimEducation.DimEndDateNavigation = educationEndDate;
                    dimEducation.Modified = currentDateTime;
                    // Update existing FactFieldValue
                    factFieldValuesEducation.Modified = currentDateTime;
                    // Mark as processed
                    orcidImportHelper.dimEducationIds.Add(factFieldValuesEducation.DimEducationId);
                }
                else
                {
                    // Create new DimEducation
                    DimEducation dimEducation = new()
                    {
                        NameEn = education.RoleTitle,
                        DegreeGrantingInstitutionName = education.OrganizationName,
                        DimStartDateNavigation = educationStartDate,
                        DimEndDateNavigation = educationEndDate,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimEducations.Add(dimEducation);

                    // Add education ORCID put code into DimPid
                    DimPid dimPidOrcidPutCodeEducation = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodeEducation.PidContent = education.PutCode.GetDbValue();
                    dimPidOrcidPutCodeEducation.PidType = Constants.PidTypes.ORCID_PUT_CODE;
                    dimPidOrcidPutCodeEducation.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodeEducation.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeEducation);

                    // Create FactFieldValues for education
                    factFieldValuesEducation = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesEducation.DimUserProfile = dimUserProfile;
                    factFieldValuesEducation.DimFieldDisplaySettings = dimFieldDisplaySettingsEducation;
                    factFieldValuesEducation.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    factFieldValuesEducation.DimEducation = dimEducation;
                    factFieldValuesEducation.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodeEducation;
                    _ttvContext.FactFieldValues.Add(factFieldValuesEducation);
                }
            }



            // Employment (Affiliation in Ttv database)
            List<OrcidEmployment> employments = _orcidJsonParserService.GetEmployments(orcidRecordJson);
            // Get DimFieldDisplaySettings for affiliation
            DimFieldDisplaySetting dimFieldDisplaySettingsAffiliation =
                dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsAffiliation => dfdsAffiliation.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_AFFILIATION);
            foreach (OrcidEmployment employment in employments)
            {
                /*
                 * Organization handling.
                 * Search organization identifier from DimPid based on ORCID's disambiguated-organization-identifier data.
                 * If organization identifier is found in DimPid, use the linked DimOrganization.
                 * If organization identifier is not found, add organization name into DimIdentifierlessData as type 'organization_name'.
                 * 
                 * Department name handling.
                 * ORCID employment may contain 'department-name'. Add deparment-name into DimIdentifierlessData as type 'organization_unit'.
                 */

                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimAffiliation
                FactFieldValue factFieldValuesAffiliation =
                    dimUserProfile.FactFieldValues.FirstOrDefault(ffv =>
                        ffv.DimFieldDisplaySettings == dimFieldDisplaySettingsAffiliation &&
                        ffv.DimPidIdOrcidPutCode > 0 &&
                        ffv.DimPidIdOrcidPutCodeNavigation.PidContent == employment.PutCode.Value.ToString());

                // Search organization identifier from DimPid based on ORCID's disambiguated-organization-identifier data.
                int? dimOrganization_id_affiliation = await _organizationHandlerService.FindOrganizationIdByOrcidDisambiguationIdentifier(
                        orcidDisambiguatedOrganizationIdentifier: employment.DisambiguatedOrganizationIdentifier,
                        orcidDisambiguationSource: employment.DisambiguationSource
                    );

                // Start date
                DimDate employmentStartDate = await _ttvContext.DimDates
                    .FirstOrDefaultAsync(dd => dd.Year == employment.StartDate.Year && dd.Month == employment.StartDate.Month && dd.Day == employment.StartDate.Day);

                // End date
                DimDate employmentEndDate = await _ttvContext.DimDates
                    .FirstOrDefaultAsync(dd => dd.Year == employment.EndDate.Year && dd.Month == employment.EndDate.Month && dd.Day == employment.EndDate.Day);

                /*
                 * Check if affiliation already exists in profile.
                 */
                if (factFieldValuesAffiliation != null)
                {
                    /*
                     * Affiliation already exists in profile, update.
                     */
                    DimAffiliation dimAffiliation_existing = factFieldValuesAffiliation.DimAffiliation;
                    dimAffiliation_existing.PositionNameEn = employment.RoleTitle;
                    dimAffiliation_existing.StartDateNavigation = employmentStartDate;
                    dimAffiliation_existing.EndDateNavigation = employmentEndDate;
                    dimAffiliation_existing.Modified = currentDateTime;

                    /*
                     * Update organization relation or identifierless data for existing affiliation.
                     */
                    if (dimOrganization_id_affiliation != null && dimOrganization_id_affiliation > 0)
                    {
                        /*
                         * Affiliation relates directly to DimOrganization.
                         */
                        dimAffiliation_existing.DimOrganizationId = (int)dimOrganization_id_affiliation;

                        /*
                         * When affiliation has related DimOrganization, possibly existing DimIdentifierlessData of type organization_name must be removed.
                         */
                        if (factFieldValuesAffiliation.DimIdentifierlessDataId != -1 && factFieldValuesAffiliation.DimIdentifierlessData.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                        {
                            _ttvContext.DimIdentifierlessData.Remove(factFieldValuesAffiliation.DimIdentifierlessData);
                        }
                    }
                    else
                    {
                        /*
                         * Affiliation does to relate directly to any DimOrganization.
                         * Update or create relation to DimIdentifierlessData via FactFieldValues.
                         */
                        if (factFieldValuesAffiliation.DimIdentifierlessDataId != -1 && factFieldValuesAffiliation.DimIdentifierlessData.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                        {
                            /*
                             * Update organization name in existing DimIdentifierlessData.
                             */
                            factFieldValuesAffiliation.DimIdentifierlessData.ValueEn = employment.OrganizationName;
                        }
                        else
                        {
                            /*
                             * Create new DimIdentifierlessData for organization name.
                             */
                            DimIdentifierlessDatum dimIdentifierlessDatum_organization_name =
                                _organizationHandlerService.CreateIdentifierlessData_OrganizationName(nameFi: "", nameEn: employment.OrganizationName, nameSv: "");
                            _ttvContext.DimIdentifierlessData.Add(dimIdentifierlessDatum_organization_name);
                            factFieldValuesAffiliation.DimIdentifierlessData = dimIdentifierlessDatum_organization_name;
                        }
                    }

                    // Update modified timestamp in FactFieldValue
                    factFieldValuesAffiliation.Modified = currentDateTime;

                    // Mark as processed
                    orcidImportHelper.dimAffiliationIds.Add(factFieldValuesAffiliation.DimAffiliationId);
                }
                else
                {
                    /*
                     * Affiliation does not yet exists in profile. Create new.
                     * TODO: AffiliationType handling
                     */
                    DimAffiliation dimAffiliation_new = new()
                    {
                        DimOrganizationId = -1,
                        StartDateNavigation = employmentStartDate,
                        EndDateNavigation = employmentEndDate,
                        PositionNameEn = employment.RoleTitle,
                        AffiliationType = -1,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };

                    // If organization was found, add relation
                    if (dimOrganization_id_affiliation != null && dimOrganization_id_affiliation > 0)
                    {
                        dimAffiliation_new.DimOrganizationId = (int)dimOrganization_id_affiliation;
                    }
                    _ttvContext.DimAffiliations.Add(dimAffiliation_new);

                    // Add employment (=affiliation) ORCID put code into DimPid
                    DimPid dimPidOrcidPutCodeEmployment = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodeEmployment.PidContent = employment.PutCode.GetDbValue();
                    dimPidOrcidPutCodeEmployment.PidType = Constants.PidTypes.ORCID_PUT_CODE;
                    dimPidOrcidPutCodeEmployment.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodeEmployment.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeEmployment);

                    // Create FactFieldValues for affiliation
                    factFieldValuesAffiliation = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesAffiliation.DimUserProfile = dimUserProfile;
                    factFieldValuesAffiliation.DimFieldDisplaySettings = dimFieldDisplaySettingsAffiliation;
                    factFieldValuesAffiliation.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    factFieldValuesAffiliation.DimAffiliation = dimAffiliation_new;
                    factFieldValuesAffiliation.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodeEmployment;

                    // If organization was not found, add organization_name into DimIdentifierlessData
                    if (dimOrganization_id_affiliation == null || dimOrganization_id_affiliation == -1)
                    {
                        DimIdentifierlessDatum dimIdentifierlessData_oganizationName =
                            _organizationHandlerService.CreateIdentifierlessData_OrganizationName(nameFi: "", nameEn: employment.OrganizationName, nameSv: "");
                        _ttvContext.DimIdentifierlessData.Add(dimIdentifierlessData_oganizationName);
                        factFieldValuesAffiliation.DimIdentifierlessData = dimIdentifierlessData_oganizationName;
                    }

                    _ttvContext.FactFieldValues.Add(factFieldValuesAffiliation);
                }

                /*
                 * Affiliation department name handling
                 */
                if (employment.DepartmentName != "")
                {
                    // ORCID employment contains 'department-name'

                    // Check if FactFieldValue has related DimIdentifierlessData.
                    //     If exists, check if type is 'organization_name' or 'organization_unit'
                    //         If type is 'organization_name', check if it has related DimIdentifierlessData of type 'organization_unit'. If exists, update value. If does not exist, create new.
                    //         If type is 'organization_unit, update value.
                    //     If does not exist, create new using type 'organization_unit'.
                    if (factFieldValuesAffiliation.DimIdentifierlessData != null)
                    {
                        // DimIdentifierlessData exists
                        // Check type.
                        if (factFieldValuesAffiliation.DimIdentifierlessData.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                        {
                            // Type is 'organization_name'. Check if it has related DimIdentifierlessData of type 'organization_unit'
                            if (
                                factFieldValuesAffiliation.DimIdentifierlessData.InverseDimIdentifierlessData.Count > 0 &&
                                factFieldValuesAffiliation.DimIdentifierlessData.InverseDimIdentifierlessData.First().Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT
                            )
                            {
                                // Has related DimIdentifierlessData of type 'organization_unit'. Update.
                                factFieldValuesAffiliation.DimIdentifierlessData.InverseDimIdentifierlessData.First().ValueEn = employment.DepartmentName;
                                factFieldValuesAffiliation.DimIdentifierlessData.InverseDimIdentifierlessData.First().ValueFi = "";
                                factFieldValuesAffiliation.DimIdentifierlessData.InverseDimIdentifierlessData.First().ValueSv = "";
                                factFieldValuesAffiliation.DimIdentifierlessData.InverseDimIdentifierlessData.First().Modified = currentDateTime;
                            }
                            else
                            {
                                // Does not have related DimIdentifierlessData of type 'organization_unit'. Add new. Set as child of DimIdentifierlessData of type 'organization_name'
                                DimIdentifierlessDatum dimIdentifierlessData_organizationUnit =
                                    _organizationHandlerService.CreateIdentifierlessData_OrganizationUnit(
                                        parentDimIdentifierlessData: factFieldValuesAffiliation.DimIdentifierlessData,
                                        nameFi: "",
                                        nameEn: employment.DepartmentName,
                                        nameSv: ""
                                    );
                                _ttvContext.DimIdentifierlessData.Add(dimIdentifierlessData_organizationUnit);
                            }
                        }
                        else if (factFieldValuesAffiliation.DimIdentifierlessData.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                        {
                            // Type is 'organization_unit'. Update
                            factFieldValuesAffiliation.DimIdentifierlessData.ValueEn = employment.DepartmentName;
                            factFieldValuesAffiliation.DimIdentifierlessData.ValueFi = "";
                            factFieldValuesAffiliation.DimIdentifierlessData.ValueSv = "";
                            factFieldValuesAffiliation.DimIdentifierlessData.Modified = currentDateTime;
                        }
                    }
                    else
                    {
                        // DimIdentifierlessData does not exist. Create new. Do not set parent DimIdentifierlessData, instead link to FactFieldValue
                        DimIdentifierlessDatum dimIdentifierlessData_organizationUnit =
                            _organizationHandlerService.CreateIdentifierlessData_OrganizationUnit(
                                parentDimIdentifierlessData: null,
                                nameFi: "",
                                nameEn: employment.DepartmentName,
                                nameSv: ""
                            );
                        _ttvContext.DimIdentifierlessData.Add(dimIdentifierlessData_organizationUnit);
                        factFieldValuesAffiliation.DimIdentifierlessData = dimIdentifierlessData_organizationUnit;
                    }
                }
            }



            // Publication
            List<OrcidPublication> orcidPublications = _orcidJsonParserService.GetPublications(orcidRecordJson);
            // Get DimFieldDisplaySettings for orcid publication
            DimFieldDisplaySetting dimFieldDisplaySettingsOrcidPublication =
                dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsPublication => dfdsPublication.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY);
            foreach (OrcidPublication orcidPublication in orcidPublications)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimOrcidPublication
                FactFieldValue factFieldValuesPublication =
                    dimUserProfile.FactFieldValues.FirstOrDefault(ffv =>
                        ffv.DimFieldDisplaySettings == dimFieldDisplaySettingsOrcidPublication &&
                        ffv.DimPidIdOrcidPutCode > 0 &&
                        ffv.DimPidIdOrcidPutCodeNavigation.PidContent == orcidPublication.PutCode.Value.ToString());

                if (factFieldValuesPublication != null)
                {
                    // Update existing DimProfileOnlyPublication
                    DimProfileOnlyPublication dimProfileOnlyPublication = factFieldValuesPublication.DimProfileOnlyPublication;
                    dimProfileOnlyPublication.OrcidWorkType = orcidPublication.Type;
                    dimProfileOnlyPublication.PublicationName = orcidPublication.PublicationName;
                    dimProfileOnlyPublication.PublicationYear = orcidPublication.PublicationYear;
                    dimProfileOnlyPublication.DoiHandle = orcidPublication.Doi;
                    dimProfileOnlyPublication.Modified = currentDateTime;
                    // Update existing FactFieldValue
                    factFieldValuesPublication.Modified = currentDateTime;
                    // Mark as processed
                    orcidImportHelper.dimPublicationIds.Add(factFieldValuesPublication.DimProfileOnlyPublicationId);
                }
                else
                {
                    // Create new DimProfileOnlyPublication
                    DimProfileOnlyPublication dimProfileOnlyPublication = _userProfileService.GetEmptyDimProfileOnlyPublication();
                    dimProfileOnlyPublication.OrcidWorkType = orcidPublication.Type;
                    dimProfileOnlyPublication.PublicationName = orcidPublication.PublicationName;
                    dimProfileOnlyPublication.PublicationYear = orcidPublication.PublicationYear;
                    dimProfileOnlyPublication.DoiHandle = orcidPublication.Doi;
                    dimProfileOnlyPublication.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    dimProfileOnlyPublication.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    dimProfileOnlyPublication.Created = currentDateTime;
                    _ttvContext.DimProfileOnlyPublications.Add(dimProfileOnlyPublication);

                    // Add publication's ORCID put code into DimPid
                    DimPid dimPidOrcidPutCodePublication = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodePublication.PidContent = orcidPublication.PutCode.GetDbValue();
                    dimPidOrcidPutCodePublication.PidType = Constants.PidTypes.ORCID_PUT_CODE;
                    dimPidOrcidPutCodePublication.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodePublication.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodePublication);

                    // Create FactFieldValues for orcid publication
                    factFieldValuesPublication = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesPublication.DimUserProfile = dimUserProfile;
                    factFieldValuesPublication.DimFieldDisplaySettings = dimFieldDisplaySettingsOrcidPublication;
                    factFieldValuesPublication.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    factFieldValuesPublication.DimProfileOnlyPublication = dimProfileOnlyPublication;
                    factFieldValuesPublication.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodePublication;
                    _ttvContext.FactFieldValues.Add(factFieldValuesPublication);
                }
            }

            // Remove names, which user has deleted in ORCID
            List<FactFieldValue> removableFfvDimNames =
                dimUserProfile.FactFieldValues.Where(ffv =>
                    ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId &&
                    ffv.DimNameId > 0 &&
                    !orcidImportHelper.dimNameIds.Contains(ffv.DimNameId)).ToList();
            foreach (FactFieldValue removableFfvDimName in removableFfvDimNames.Distinct())
            {
                _ttvContext.FactFieldValues.Remove(removableFfvDimName);
                _ttvContext.DimNames.Remove(removableFfvDimName.DimName);
                if (removableFfvDimName.DimPidIdOrcidPutCode > 0)
                {
                    _ttvContext.DimPids.Remove(removableFfvDimName.DimPidIdOrcidPutCodeNavigation);
                }
            }

            // Remove web links, which user has deleted in ORCID
            List<FactFieldValue> removableFfvWebLinks =
                dimUserProfile.FactFieldValues.Where(ffv =>
                    ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId &&
                    ffv.DimWebLinkId > 0 &&
                    !orcidImportHelper.dimWebLinkIds.Contains(ffv.DimWebLinkId)).ToList();
            foreach (FactFieldValue removableFfvWebLink in removableFfvWebLinks.Distinct())
            {
                _ttvContext.FactFieldValues.Remove(removableFfvWebLink);
                _ttvContext.DimWebLinks.Remove(removableFfvWebLink.DimWebLink);
                if (removableFfvWebLink.DimPidIdOrcidPutCode > 0)
                {
                    _ttvContext.DimPids.Remove(removableFfvWebLink.DimPidIdOrcidPutCodeNavigation);
                }
            }

            // Remove researcher descriptions, which user has deleted in ORCID
            List<FactFieldValue> removableFfvResearcherDescriptions =
                dimUserProfile.FactFieldValues.Where(ffv =>
                    ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId &&
                    ffv.DimResearcherDescriptionId > 0 &&
                    !orcidImportHelper.dimResearcherDescriptionIds.Contains(ffv.DimResearcherDescriptionId)).ToList();
            foreach (FactFieldValue removableFfvResearcherDescription in removableFfvResearcherDescriptions.Distinct())
            {
                _ttvContext.FactFieldValues.Remove(removableFfvResearcherDescription);
                _ttvContext.DimResearcherDescriptions.Remove(removableFfvResearcherDescription.DimResearcherDescription);
                if (removableFfvResearcherDescription.DimPidIdOrcidPutCode > 0)
                {
                    _ttvContext.DimPids.Remove(removableFfvResearcherDescription.DimPidIdOrcidPutCodeNavigation);
                }
            }

            // Remove email addresses, which user has deleted in ORCID
            List<FactFieldValue> removableFfvEmails =
                dimUserProfile.FactFieldValues.Where(ffv =>
                    ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId &&
                    ffv.DimEmailAddrressId > 0 &&
                    !orcidImportHelper.dimEmailAddressIds.Contains(ffv.DimEmailAddrressId)).ToList();
            foreach (FactFieldValue removableFfvEmail in removableFfvEmails.Distinct())
            {
                _ttvContext.FactFieldValues.Remove(removableFfvEmail);
                _ttvContext.DimEmailAddrresses.Remove(removableFfvEmail.DimEmailAddrress);
                if (removableFfvEmail.DimPidIdOrcidPutCode > 0)
                {
                    _ttvContext.DimPids.Remove(removableFfvEmail.DimPidIdOrcidPutCodeNavigation);
                }
            }

            // Remove keywords, which user has deleted in ORCID
            List<FactFieldValue> removableFfvKeywords =
                dimUserProfile.FactFieldValues.Where(ffv =>
                    ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId &&
                    ffv.DimKeywordId > 0 &&
                    !orcidImportHelper.dimKeywordIds.Contains(ffv.DimKeywordId)).ToList();
            foreach (FactFieldValue removableFfvKeyword in removableFfvKeywords.Distinct())
            {
                _ttvContext.FactFieldValues.Remove(removableFfvKeyword);
                _ttvContext.DimKeywords.Remove(removableFfvKeyword.DimKeyword);
                if (removableFfvKeyword.DimPidIdOrcidPutCode > 0)
                {
                    _ttvContext.DimPids.Remove(removableFfvKeyword.DimPidIdOrcidPutCodeNavigation);
                }
            }

            // Remove external identifiers, which user has deleted in ORCID
            List<FactFieldValue> removableFfvExternalIdentifiers =
                dimUserProfile.FactFieldValues.Where(ffv =>
                    ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId &&
                    ffv.DimPidId > 0 &&
                    !orcidImportHelper.dimPidIds.Contains(ffv.DimPidId)).ToList();
            foreach (FactFieldValue removableFfvExternalIdentifier in removableFfvExternalIdentifiers.Distinct())
            {
                _ttvContext.FactFieldValues.Remove(removableFfvExternalIdentifier);
                _ttvContext.DimPids.Remove(removableFfvExternalIdentifier.DimPid);
                if (removableFfvExternalIdentifier.DimPidIdOrcidPutCode > 0)
                {
                    _ttvContext.DimPids.Remove(removableFfvExternalIdentifier.DimPidIdOrcidPutCodeNavigation);
                }
            }

            // Remove educations, which user has deleted in ORCID
            List<FactFieldValue> removableFfvEducations =
                dimUserProfile.FactFieldValues.Where(ffv =>
                    ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId &&
                    ffv.DimEducationId > 0 &&
                    !orcidImportHelper.dimEducationIds.Contains(ffv.DimEducationId)).ToList();
            foreach (FactFieldValue removableFfvEducation in removableFfvEducations.Distinct())
            {
                _ttvContext.FactFieldValues.Remove(removableFfvEducation);
                _ttvContext.DimEducations.Remove(removableFfvEducation.DimEducation);
                if (removableFfvEducation.DimPidIdOrcidPutCode > 0)
                {
                    _ttvContext.DimPids.Remove(removableFfvEducation.DimPidIdOrcidPutCodeNavigation);
                }
            }

            // Remove affiliations, which user has deleted in ORCID
            List<FactFieldValue> removableFfvAffiliations =
                dimUserProfile.FactFieldValues.Where(ffv =>
                    ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId &&
                    ffv.DimAffiliationId > 0 &&
                    !orcidImportHelper.dimAffiliationIds.Contains(ffv.DimAffiliationId)).ToList();
            foreach (FactFieldValue removableFfvAffiliation in removableFfvAffiliations.Distinct())
            {
                _ttvContext.FactFieldValues.Remove(removableFfvAffiliation);
                _ttvContext.DimAffiliations.Remove(removableFfvAffiliation.DimAffiliation);
                if (removableFfvAffiliation.DimPidIdOrcidPutCode > 0)
                {
                    _ttvContext.DimPids.Remove(removableFfvAffiliation.DimPidIdOrcidPutCodeNavigation);
                }
                // Affiliation organization can be stored in DimIdentifierlessData
                if (removableFfvAffiliation.DimIdentifierlessDataId > 0)
                {
                    // DimIdentifierlessData can have child entity
                    _ttvContext.DimIdentifierlessData.RemoveRange(removableFfvAffiliation.DimIdentifierlessData.InverseDimIdentifierlessData);
                    _ttvContext.DimIdentifierlessData.Remove(removableFfvAffiliation.DimIdentifierlessData);
                }
            }

            // Remove publications, which user has deleted in ORCID
            List<FactFieldValue> removableFfvPublications =
                dimUserProfile.FactFieldValues.Where(ffv =>
                    ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId &&
                    ffv.DimProfileOnlyPublicationId > 0 &&
                    !orcidImportHelper.dimPublicationIds.Contains(ffv.DimProfileOnlyPublicationId)).ToList();
            foreach (FactFieldValue removableFfvPublication in removableFfvPublications.Distinct())
            {
                _ttvContext.FactFieldValues.Remove(removableFfvPublication);
                _ttvContext.DimProfileOnlyPublications.Remove(removableFfvPublication.DimProfileOnlyPublication);
                if (removableFfvPublication.DimPidIdOrcidPutCode > 0)
                {
                    _ttvContext.DimPids.Remove(removableFfvPublication.DimPidIdOrcidPutCodeNavigation);
                }
            }

            try
            {
                await _ttvContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ORCID import failed for dim_user_profile.id={userprofileId}: {ex}");
            }

            return false;
        }
    }
}