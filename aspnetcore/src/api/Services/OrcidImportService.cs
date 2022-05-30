using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Orcid;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    /*
     * OrcidImportService imports ORCID data into user profile.
     */
    public class OrcidImportService
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;
        private readonly OrcidJsonParserService _orcidJsonParserService;
        private readonly OrganizationHandlerService _organizationHandlerService;
        private readonly DataSourceHelperService _dataSourceHelperService;
        private readonly UtilityService _utilityService;

        public OrcidImportService(TtvContext ttvContext, UserProfileService userProfileService, OrcidJsonParserService orcidJsonParserService, OrganizationHandlerService organizationHandlerService, UtilityService utilityService, DataSourceHelperService dataSourceHelperService)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _orcidJsonParserService = orcidJsonParserService;
            _organizationHandlerService = organizationHandlerService;
            _utilityService = utilityService;
            _dataSourceHelperService = dataSourceHelperService;
        }

        /*
         * Import ORCID record json into user profile.
         */
        public async Task<bool> ImportOrcidRecordJsonIntoUserProfile(int userprofileId, string json)
        {
            // Get ORCID registered data source id.
            int orcidRegisteredDataSourceId = _dataSourceHelperService.DimRegisteredDataSourceId_ORCID;

            // Get DimUserProfile and related entities
            DimUserProfile dimUserProfile = await _ttvContext.DimUserProfiles.Where(dup => dup.Id == userprofileId)
                .Include(dup => dup.DimFieldDisplaySettings)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimRegisteredDataSource)
                        .ThenInclude(drds => drds.DimOrganization)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimName)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimWebLink)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimPublication)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimOrcidPublication)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimPid)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimPidIdOrcidPutCodeNavigation)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimResearchActivity)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimEvent)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimEducation)
                        .ThenInclude(de => de.DimStartDateNavigation)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimEducation)
                        .ThenInclude(de => de.DimEndDateNavigation)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimAffiliation)
                        .ThenInclude(da => da.DimOrganization)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimAffiliation)
                        .ThenInclude(da => da.StartDateNavigation)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimAffiliation)
                        .ThenInclude(da => da.EndDateNavigation)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimCompetence)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimResearchCommunity)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimTelephoneNumber)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimEmailAddrress)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimResearcherDescription)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimIdentifierlessData)
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimKeyword).FirstOrDefaultAsync();

            // Get current DateTime
            DateTime currentDateTime = _utilityService.GetCurrentDateTime();

            // Get "-1" DimOrganization
            DimOrganization dimOrganizationNull = await _ttvContext.DimOrganizations.FindAsync(-1);

            // Must use "Constants.SourceIdentifiers.ORCID" as value for "FactFieldValue.SourceId". It is used to identify what data can be deleted when userprofile is deleted.

            // Name
            DimFieldDisplaySetting dimFieldDisplaySettingsName = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dimFieldDisplaysettingsName => dimFieldDisplaysettingsName.FieldIdentifier == Constants.FieldIdentifiers.PERSON_NAME);
            // FactFieldValues
            FactFieldValue factFieldValuesName = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimFieldDisplaySettings.Id == dimFieldDisplaySettingsName.Id && ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId);
            if (factFieldValuesName != null)
            {
                // Update existing DimName
                DimName dimName = factFieldValuesName.DimName;
                dimName.LastName = _orcidJsonParserService.GetFamilyName(json).Value;
                dimName.FirstNames = _orcidJsonParserService.GetGivenNames(json).Value;
                dimName.Modified = _utilityService.GetCurrentDateTime();
                // Update existing FactFieldValue
                factFieldValuesName.Show = true; // ORCID name is selected by default.
                factFieldValuesName.Modified = currentDateTime;
            }
            else
            {
                // Create new DimName
                DimName dimName = new()
                {
                    LastName = _orcidJsonParserService.GetFamilyName(json).Value,
                    FirstNames = _orcidJsonParserService.GetGivenNames(json).Value,
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
                factFieldValuesName.Show = true;
                _ttvContext.FactFieldValues.Add(factFieldValuesName);
            }


            // Other names
            List<OrcidOtherName> otherNames = _orcidJsonParserService.GetOtherNames(json);
            foreach (OrcidOtherName otherName in otherNames)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimPid
                FactFieldValue factFieldValuesOtherName = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimPidIdOrcidPutCode > 0 && ffv.DimPidIdOrcidPutCodeNavigation.PidContent == otherName.PutCode.Value.ToString());

                if (factFieldValuesOtherName != null)
                {
                    // Update existing DimName
                    DimName dimName_otherName = factFieldValuesOtherName.DimName;
                    dimName_otherName.FullName = otherName.Value;
                    dimName_otherName.Modified = currentDateTime;
                    // Update existing FactFieldValue
                    factFieldValuesOtherName.Modified = currentDateTime;
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

                    // Get DimFieldDisplaySettings for other name
                    DimFieldDisplaySetting dimFieldDisplaySettingsOtherName = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsWebLink => dfdsWebLink.FieldIdentifier == Constants.FieldIdentifiers.PERSON_OTHER_NAMES);

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
            List<OrcidResearcherUrl> researcherUrls = _orcidJsonParserService.GetResearcherUrls(json);
            foreach (OrcidResearcherUrl researchUrl in researcherUrls)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimPid
                FactFieldValue factFieldValuesWebLink = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimPidIdOrcidPutCode > 0 && ffv.DimPidIdOrcidPutCodeNavigation.PidContent == researchUrl.PutCode.Value.ToString());

                if (factFieldValuesWebLink != null)
                {
                    // Update existing DimWebLink
                    DimWebLink dimWebLink = factFieldValuesWebLink.DimWebLink;
                    dimWebLink.Url = researchUrl.Url;
                    dimWebLink.LinkLabel = researchUrl.UrlName;
                    dimWebLink.Modified = currentDateTime;

                    // Update existing FactFieldValue
                    factFieldValuesWebLink.Modified = currentDateTime;
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

                    // Get DimFieldDisplaySettings for weblink
                    DimFieldDisplaySetting dimFieldDisplaySettingsWebLink = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsWebLink => dfdsWebLink.FieldIdentifier == Constants.FieldIdentifiers.PERSON_WEB_LINK);

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
            OrcidBiography biography = _orcidJsonParserService.GetBiography(json);
            if (biography != null)
            {
                DimResearcherDescription dimResearcherDescription = await _userProfileService.AddOrUpdateDimResearcherDescription(
                    "",
                    _orcidJsonParserService.GetBiography(json).Value,
                    "",
                    dimUserProfile.DimKnownPersonId,
                    orcidRegisteredDataSourceId
                );

                // Researcher description: DimFieldDisplaySettings
                DimFieldDisplaySetting dimFieldDisplaySettingsResearcherDescription = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dimFieldDisplaySettingsResearcherDescription => dimFieldDisplaySettingsResearcherDescription.FieldIdentifier == Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION);

                // Researcher description: FactFieldValues
                FactFieldValue factFieldValuesResearcherDescription = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimResearcherDescriptionId == dimResearcherDescription.Id);
                if (factFieldValuesResearcherDescription == null)
                {
                    factFieldValuesResearcherDescription = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesResearcherDescription.DimUserProfile = dimUserProfile;
                    factFieldValuesResearcherDescription.DimFieldDisplaySettings = dimFieldDisplaySettingsResearcherDescription;
                    factFieldValuesResearcherDescription.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    factFieldValuesResearcherDescription.DimResearcherDescription = dimResearcherDescription;
                    _ttvContext.FactFieldValues.Add(factFieldValuesResearcherDescription);
                }
                else
                {
                    factFieldValuesResearcherDescription.Modified = currentDateTime;
                }
            }


            // Email
            List<OrcidEmail> emails = _orcidJsonParserService.GetEmails(json);
            foreach (OrcidEmail email in emails)
            {
                // Email: DimEmailAddrressess
                DimEmailAddrress dimEmailAddress = await _userProfileService.AddOrUpdateDimEmailAddress(
                    email.Value,
                    dimUserProfile.DimKnownPersonId,
                    orcidRegisteredDataSourceId
                );

                // Email: DimFieldDisplaySettings
                DimFieldDisplaySetting dimFieldDisplaySettingsEmailAddress = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dimFieldDisplaySettingsEmailAddress => dimFieldDisplaySettingsEmailAddress.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS);

                // Email: FactFieldValues
                FactFieldValue factFieldValuesEmailAddress = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimEmailAddrressId == dimEmailAddress.Id);
                if (factFieldValuesEmailAddress == null)
                {
                    factFieldValuesEmailAddress = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesEmailAddress.DimUserProfile = dimUserProfile;
                    factFieldValuesEmailAddress.DimFieldDisplaySettings = dimFieldDisplaySettingsEmailAddress;
                    factFieldValuesEmailAddress.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    factFieldValuesEmailAddress.DimEmailAddrress = dimEmailAddress;
                    _ttvContext.FactFieldValues.Add(factFieldValuesEmailAddress);
                }
                else
                {
                    factFieldValuesEmailAddress.Modified = currentDateTime;
                }
            }


            // Keyword
            List<OrcidKeyword> keywords = _orcidJsonParserService.GetKeywords(json);
            // Get DimFieldDisplaySettings for keyword
            DimFieldDisplaySetting dimFieldDisplaySettingsKeyword = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsKeyword => dfdsKeyword.FieldIdentifier == Constants.FieldIdentifiers.PERSON_KEYWORD);
            // Collect list of processed FactFieldValues related to keyword. Needed when deleting keywords.
            List<FactFieldValue> processedKeywordFactFieldValues = new();
            foreach (OrcidKeyword keyword in keywords)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimKeyword
                FactFieldValue factFieldValuesKeyword = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimPidIdOrcidPutCode > 0 && ffv.DimPidIdOrcidPutCodeNavigation.PidContent == keyword.PutCode.Value.ToString());

                if (factFieldValuesKeyword != null)
                {
                    // Update existing DimKeyword
                    DimKeyword dimKeyword = factFieldValuesKeyword.DimKeyword;
                    dimKeyword.Keyword = keyword.Value;
                    dimKeyword.Modified = currentDateTime;
                    // Update existing FactFieldValue
                    factFieldValuesKeyword.Modified = currentDateTime;
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
            List<OrcidExternalIdentifier> externalIdentifiers = _orcidJsonParserService.GetExternalIdentifiers(json);
            // Get DimFieldDisplaySettings for keyword
            DimFieldDisplaySetting dimFieldDisplaySettingsExternalIdentifier = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsKeyword => dfdsKeyword.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER);
            foreach (OrcidExternalIdentifier externalIdentifier in externalIdentifiers)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimPid
                FactFieldValue factFieldValuesExternalIdentifier = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimPidIdOrcidPutCode > 0 && ffv.DimPidIdOrcidPutCodeNavigation.PidContent == externalIdentifier.PutCode.Value.ToString());

                if (factFieldValuesExternalIdentifier != null)
                {
                    // Update existing DimPid
                    DimPid dimPid = factFieldValuesExternalIdentifier.DimPid;
                    dimPid.PidContent = externalIdentifier.ExternalIdValue;
                    dimPid.PidType = externalIdentifier.ExternalIdType;
                    dimPid.Modified = currentDateTime;
                    // Update existing FactFieldValue
                    factFieldValuesExternalIdentifier.Modified = currentDateTime;
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
            List<OrcidEducation> educations = _orcidJsonParserService.GetEducations(json);
            foreach (OrcidEducation education in educations)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimEducation
                FactFieldValue factFieldValuesEducation = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimPidIdOrcidPutCode > 0 && ffv.DimPidIdOrcidPutCodeNavigation.PidContent == education.PutCode.Value.ToString());

                // Start date
                DimDate startDate = await _ttvContext.DimDates.FirstOrDefaultAsync(dd => dd.Year == education.StartDate.Year && dd.Month == education.StartDate.Month && dd.Day == education.StartDate.Day);
                if (startDate == null)
                {
                    startDate = new DimDate()
                    {
                        Year = education.StartDate.Year,
                        Month = education.StartDate.Month,
                        Day = education.StartDate.Day,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimDates.Add(startDate);
                }

                // End date
                DimDate endDate = await _ttvContext.DimDates.FirstOrDefaultAsync(ed => ed.Year == education.EndDate.Year && ed.Month == education.EndDate.Month && ed.Day == education.EndDate.Day);
                if (endDate == null)
                {
                    endDate = new DimDate()
                    {
                        Year = education.EndDate.Year,
                        Month = education.EndDate.Month,
                        Day = education.EndDate.Day,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimDates.Add(endDate);
                }

                if (factFieldValuesEducation != null)
                {
                    // Update existing DimEducation
                    DimEducation dimEducation = factFieldValuesEducation.DimEducation;
                    dimEducation.NameEn = education.RoleTitle;
                    dimEducation.DegreeGrantingInstitutionName = education.OrganizationName;
                    dimEducation.DimStartDateNavigation = startDate;
                    dimEducation.DimEndDateNavigation = endDate;
                    dimEducation.Modified = currentDateTime;

                    // Update existing FactFieldValue
                    factFieldValuesEducation.Modified = currentDateTime;
                }
                else
                {
                    // Create new DimEducation
                    DimEducation dimEducation = new()
                    {
                        NameEn = education.RoleTitle,
                        DegreeGrantingInstitutionName = education.OrganizationName,
                        DimStartDateNavigation = startDate,
                        DimEndDateNavigation = endDate,
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

                    // Get DimFieldDisplaySettings for education
                    DimFieldDisplaySetting dimFieldDisplaySettingsEducation = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsEducation => dfdsEducation.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_EDUCATION);

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
            List<OrcidEmployment> employments = _orcidJsonParserService.GetEmployments(json);
            foreach (OrcidEmployment employment in employments)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimAffiliation
                FactFieldValue factFieldValuesAffiliation = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimPidIdOrcidPutCode > 0 && ffv.DimPidIdOrcidPutCodeNavigation.PidContent == employment.PutCode.Value.ToString());

                // Organization handling.
                // Search organization identifier from DimPid based on ORCID's disambiguated-organization-identifier data.
                // If organization identifier is found in DimPid, use the linked DimOrganization.
                // If organization identifier is not found, add organization name into DimIdentifierlessData.
                DimOrganization dimOrganization_affiliation = await _organizationHandlerService.FindOrcidDimOrganizationByOrcidDisambiguatedOrganization(
                        orcidDisambiguatedOrganizationIdentifier: employment.DisambiguatedOrganizationIdentifier,
                        orcidDisambiguationSource: employment.DisambiguationSource
                    );

                // Start date
                DimDate startDate = await _ttvContext.DimDates.FirstOrDefaultAsync(dd => dd.Year == employment.StartDate.Year && dd.Month == employment.StartDate.Month && dd.Day == employment.StartDate.Day);
                if (startDate == null)
                {
                    startDate = new DimDate()
                    {
                        Year = employment.StartDate.Year,
                        Month = employment.StartDate.Month,
                        Day = employment.StartDate.Day,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimDates.Add(startDate);
                }

                // End date
                DimDate endDate = await _ttvContext.DimDates.FirstOrDefaultAsync(dd => dd.Year == employment.EndDate.Year && dd.Month == employment.EndDate.Month && dd.Day == employment.EndDate.Day);
                if (endDate == null)
                {
                    endDate = new DimDate()
                    {
                        Year = employment.EndDate.Year,
                        Month = employment.EndDate.Month,
                        Day = employment.EndDate.Day,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimDates.Add(endDate);
                }

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
                    dimAffiliation_existing.StartDateNavigation = startDate;
                    dimAffiliation_existing.EndDateNavigation = endDate;
                    dimAffiliation_existing.Modified = currentDateTime;

                    /*
                     * Update organization relation or identifierless data for existing affiliation.
                     */
                    if (dimOrganization_affiliation != null && dimOrganization_affiliation.Id > 0)
                    {
                        /*
                         * Affiliation relates directly to DimOrganization.
                         */
                        dimAffiliation_existing.DimOrganization = dimOrganization_affiliation;

                        /*
                         * When affiliation has related DimOrganization, possibly existing DimIdentifierlessData must be removed.
                         */
                        if (factFieldValuesAffiliation.DimIdentifierlessDataId != -1)
                        {
                            _ttvContext.DimIdentifierlessData.Remove(factFieldValuesAffiliation.DimIdentifierlessData);
                        }
                    }
                    else {
                        /*
                         * Affiliation does to relate directly to any DimOrganization.
                         * Update or create relation to DimIdentifierlessData via FactFieldValues.
                         */
                        if (factFieldValuesAffiliation.DimIdentifierlessDataId != -1)
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
                            DimIdentifierlessDatum dimIdentifierlessDatum_organization_name = _organizationHandlerService.CreateIdentifierlessData_OrganizationName(nameFi: "", nameEn: employment.OrganizationName, nameSv: "");
                            _ttvContext.DimIdentifierlessData.Add(dimIdentifierlessDatum_organization_name);
                            factFieldValuesAffiliation.DimIdentifierlessData = dimIdentifierlessDatum_organization_name;
                        }
                    }

                    // Update modified timestamp in FactFieldValue
                    factFieldValuesAffiliation.Modified = currentDateTime;
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
                        StartDateNavigation = startDate,
                        EndDateNavigation = endDate,
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
                    if (dimOrganization_affiliation != null && dimOrganization_affiliation.Id > 0)
                    {
                        dimAffiliation_new.DimOrganization = dimOrganization_affiliation;
                    }
                    _ttvContext.DimAffiliations.Add(dimAffiliation_new);

                    // Add employment (=affiliation) ORCID put code into DimPid
                    DimPid dimPidOrcidPutCodeEmployment = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodeEmployment.PidContent = employment.PutCode.GetDbValue();
                    dimPidOrcidPutCodeEmployment.PidType = Constants.PidTypes.ORCID_PUT_CODE;
                    dimPidOrcidPutCodeEmployment.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodeEmployment.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeEmployment);

                    // Get DimFieldDisplaySettings for affiliation
                    DimFieldDisplaySetting dimFieldDisplaySettingsAffiliation = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsAffiliation => dfdsAffiliation.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_AFFILIATION);

                    // Create FactFieldValues for affiliation
                    factFieldValuesAffiliation = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesAffiliation.DimUserProfile = dimUserProfile;
                    factFieldValuesAffiliation.DimFieldDisplaySettings = dimFieldDisplaySettingsAffiliation;
                    factFieldValuesAffiliation.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    factFieldValuesAffiliation.DimAffiliation = dimAffiliation_new;
                    factFieldValuesAffiliation.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodeEmployment;

                    // If organization was not found, add organization_name into DimIdentifierlessData
                    if (dimOrganization_affiliation == null)
                    {
                        DimIdentifierlessDatum dimIdentifierlessData_oganizationName = _organizationHandlerService.CreateIdentifierlessData_OrganizationName(nameFi: "", nameEn: employment.OrganizationName, nameSv: "");
                        _ttvContext.DimIdentifierlessData.Add(dimIdentifierlessData_oganizationName);
                        factFieldValuesAffiliation.DimIdentifierlessData = dimIdentifierlessData_oganizationName;
                    }

                    _ttvContext.FactFieldValues.Add(factFieldValuesAffiliation);
                }
            }



            // Publication
            List<OrcidPublication> orcidPublications = _orcidJsonParserService.GetPublications(json);
            foreach (OrcidPublication orcidPublication in orcidPublications)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimOrcidPublication
                FactFieldValue factFieldValuesPublication = dimUserProfile.FactFieldValues.FirstOrDefault(ffv => ffv.DimPidIdOrcidPutCode > 0 && ffv.DimPidIdOrcidPutCodeNavigation.PidContent == orcidPublication.PutCode.Value.ToString());

                if (factFieldValuesPublication != null)
                {
                    // Update existing DimOrcidPublication
                    DimOrcidPublication dimOrcidPublication = factFieldValuesPublication.DimOrcidPublication;
                    dimOrcidPublication.OrcidWorkType = orcidPublication.Type;
                    dimOrcidPublication.PublicationName = orcidPublication.PublicationName;
                    dimOrcidPublication.PublicationYear = orcidPublication.PublicationYear;
                    dimOrcidPublication.DoiHandle = orcidPublication.Doi;
                    dimOrcidPublication.Modified = currentDateTime;
                    // Update existing FactFieldValue
                    factFieldValuesPublication.Modified = currentDateTime;
                }
                else
                {
                    // Create new DimOrcidPublication
                    DimOrcidPublication dimOrcidPublication = _userProfileService.GetEmptyDimOrcidPublication();
                    dimOrcidPublication.OrcidWorkType = orcidPublication.Type;
                    dimOrcidPublication.PublicationName = orcidPublication.PublicationName;
                    dimOrcidPublication.PublicationYear = orcidPublication.PublicationYear;
                    dimOrcidPublication.DoiHandle = orcidPublication.Doi;
                    dimOrcidPublication.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    dimOrcidPublication.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    dimOrcidPublication.Created = currentDateTime;
                    _ttvContext.DimOrcidPublications.Add(dimOrcidPublication);

                    // Add publication's ORCID put code into DimPid
                    DimPid dimPidOrcidPutCodePublication = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodePublication.PidContent = orcidPublication.PutCode.GetDbValue();
                    dimPidOrcidPutCodePublication.PidType = Constants.PidTypes.ORCID_PUT_CODE;
                    dimPidOrcidPutCodePublication.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodePublication.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodePublication);

                    // Get DimFieldDisplaySettings for orcid publication
                    DimFieldDisplaySetting dimFieldDisplaySettingsOrcidPublication = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsPublication => dfdsPublication.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID);

                    // Create FactFieldValues for orcid publication
                    factFieldValuesPublication = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesPublication.DimUserProfile = dimUserProfile;
                    factFieldValuesPublication.DimFieldDisplaySettings = dimFieldDisplaySettingsOrcidPublication;
                    factFieldValuesPublication.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    factFieldValuesPublication.DimOrcidPublication = dimOrcidPublication;
                    factFieldValuesPublication.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodePublication;
                    _ttvContext.FactFieldValues.Add(factFieldValuesPublication);
                }
            }

            await _ttvContext.SaveChangesAsync();
            return true;
        }
    }
}