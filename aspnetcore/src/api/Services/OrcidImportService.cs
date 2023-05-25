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
using Microsoft.EntityFrameworkCore.Storage;
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
        private readonly IOrcidApiService _orcidApiService;
        private readonly IOrcidJsonParserService _orcidJsonParserService;
        private readonly IOrganizationHandlerService _organizationHandlerService;
        private readonly IDataSourceHelperService _dataSourceHelperService;
        private readonly IUtilityService _utilityService;

        public OrcidImportService(
            TtvContext ttvContext, IUserProfileService userProfileService, IOrcidApiService orcidApiService, IOrcidJsonParserService orcidJsonParserService,
            IOrganizationHandlerService organizationHandlerService, IUtilityService utilityService, IDataSourceHelperService dataSourceHelperService)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _orcidApiService = orcidApiService;
            _orcidJsonParserService = orcidJsonParserService;
            _organizationHandlerService = organizationHandlerService;
            _utilityService = utilityService;
            _dataSourceHelperService = dataSourceHelperService;
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

            // Funding DimDates
            List<OrcidFunding> fundings = _orcidJsonParserService.GetFundings(orcidRecordJson);
            foreach (OrcidFunding funding in fundings)
            {
                // Start data
                DimDate fundingStartDate =
                    await _ttvContext.DimDates.FirstOrDefaultAsync(
                        dd => dd.Year == funding.StartDate.Year &&
                        dd.Month == funding.StartDate.Month &&
                        dd.Day == funding.StartDate.Day);
                if (fundingStartDate == null)
                {
                    fundingStartDate = new DimDate()
                    {
                        Year = funding.StartDate.Year,
                        Month = funding.StartDate.Month,
                        Day = funding.StartDate.Day,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimDates.Add(fundingStartDate);
                    await _ttvContext.SaveChangesAsync();
                }

                // End date
                DimDate fundingEndDate = await _ttvContext.DimDates.FirstOrDefaultAsync(
                    dd => dd.Year == funding.EndDate.Year &&
                    dd.Month == funding.EndDate.Month &&
                    dd.Day == funding.EndDate.Day);
                if (fundingEndDate == null)
                {
                    fundingEndDate = new DimDate()
                    {
                        Year = funding.EndDate.Year,
                        Month = funding.EndDate.Month,
                        Day = funding.EndDate.Day,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimDates.Add(fundingEndDate);
                    await _ttvContext.SaveChangesAsync();
                }
            }


            // Research activity DimDates - invited position & distinction
            List <OrcidResearchActivity> orcidResearchActivity_invitedPositionsAndDistinctionsMembershipsServices =
                _orcidJsonParserService.GetProfileOnlyResearchActivityItems(orcidRecordJson);
            foreach (OrcidResearchActivity researchActivity in orcidResearchActivity_invitedPositionsAndDistinctionsMembershipsServices)
            {
                // Start date
                DimDate researchActivityStartDate =
                    await _ttvContext.DimDates.FirstOrDefaultAsync(
                        dd => dd.Year == researchActivity.StartDate.Year &&
                        dd.Month == researchActivity.StartDate.Month &&
                        dd.Day == researchActivity.StartDate.Day);
                if (researchActivityStartDate == null)
                {
                    researchActivityStartDate = new DimDate()
                    {
                        Year = researchActivity.StartDate.Year,
                        Month = researchActivity.StartDate.Month,
                        Day = researchActivity.StartDate.Day,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimDates.Add(researchActivityStartDate);
                    await _ttvContext.SaveChangesAsync();
                }

                // End date
                DimDate researchActivityEndDate =
                    await _ttvContext.DimDates.FirstOrDefaultAsync(
                        dd => dd.Year == researchActivity.EndDate.Year &&
                        dd.Month == researchActivity.EndDate.Month &&
                        dd.Day == researchActivity.EndDate.Day);
                if (researchActivityEndDate == null)
                {
                    researchActivityEndDate = new DimDate()
                    {
                        Year = researchActivity.EndDate.Year,
                        Month = researchActivity.EndDate.Month,
                        Day = researchActivity.EndDate.Day,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimDates.Add(researchActivityEndDate);
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
                // DimProfileOnlyFundingDecision
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimProfileOnlyFundingDecision)
                        .ThenInclude(fd => fd.DimOrganizationIdFunderNavigation)
                // DimProfileOnlyDataset
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimProfileOnlyDataset)
                // DimProfileOnlyResearchActivity
                .Include(dup => dup.FactFieldValues.Where(ffv => ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId))
                    .ThenInclude(ffv => ffv.DimProfileOnlyResearchActivity)
                        .ThenInclude(ra => ra.DimOrganization)
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
                         * That will change the primary key of FactFieldValue, which must be recreated.
                         */
                        if (factFieldValuesAffiliation.DimIdentifierlessDataId != -1 && factFieldValuesAffiliation.DimIdentifierlessData.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                        {
                            // Create new FactFieldValues for affiliation and delete old
                            FactFieldValue factFieldValuesAffiliationNew = _userProfileService.GetEmptyFactFieldValue();
                            factFieldValuesAffiliationNew.DimIdentifierlessDataId = -1;
                            factFieldValuesAffiliationNew.DimUserProfile = factFieldValuesAffiliation.DimUserProfile;
                            factFieldValuesAffiliationNew.DimFieldDisplaySettings = factFieldValuesAffiliation.DimFieldDisplaySettings;
                            factFieldValuesAffiliationNew.DimRegisteredDataSourceId = factFieldValuesAffiliation.DimRegisteredDataSourceId;
                            factFieldValuesAffiliationNew.DimAffiliation = factFieldValuesAffiliation.DimAffiliation;
                            factFieldValuesAffiliationNew.DimPidIdOrcidPutCodeNavigation = factFieldValuesAffiliation.DimPidIdOrcidPutCodeNavigation;
                            factFieldValuesAffiliationNew.Show = factFieldValuesAffiliation.Show;
                            factFieldValuesAffiliationNew.PrimaryValue = factFieldValuesAffiliation.PrimaryValue;
                            factFieldValuesAffiliationNew.SourceId = factFieldValuesAffiliation.SourceId;
                            factFieldValuesAffiliationNew.SourceDescription = factFieldValuesAffiliation.SourceDescription;
                            _ttvContext.Add(factFieldValuesAffiliationNew);
                            _ttvContext.FactFieldValues.Remove(factFieldValuesAffiliation);
                            _ttvContext.DimIdentifierlessData.Remove(factFieldValuesAffiliation.DimIdentifierlessData);
                        }
                    }
                    else
                    {
                        /*
                         * Affiliation does not relate directly to any DimOrganization.
                         * Update or create relation to DimIdentifierlessData via FactFieldValues.
                         */
                        if (factFieldValuesAffiliation.DimIdentifierlessDataId != -1 && factFieldValuesAffiliation.DimIdentifierlessData.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                        {
                            /*
                             * Update organization name in existing DimIdentifierlessData.
                             */
                            factFieldValuesAffiliation.DimIdentifierlessData.ValueEn = employment.OrganizationName;
                            factFieldValuesAffiliation.DimIdentifierlessData.UnlinkedIdentifier =
                                _organizationHandlerService.GetUnlinkedIdentifierFromOrcidDisambiguation(
                                    orcidDisambiguationSource: employment.DisambiguationSource,
                                    orcidDisambiguatedOrganizationIdentifier: employment.DisambiguatedOrganizationIdentifier);
                        }
                        else
                        {
                            /*
                             * Create new DimIdentifierlessData for organization name.
                             * That will change the primary key of FactFieldValue, which must be recreated.
                             */

                            // Make sure DimAffiliation does not reference DimOrganization
                            factFieldValuesAffiliation.DimAffiliation.DimOrganizationId = -1;

                            DimIdentifierlessDatum dimIdentifierlessDatum_affiliation_organization_name =
                                _organizationHandlerService.CreateIdentifierlessData_OrganizationName(
                                    nameFi: "",
                                    nameEn: employment.OrganizationName,
                                    nameSv: "",
                                    orcidDisambiguationSource: employment.DisambiguationSource,
                                    orcidDisambiguatedOrganizationIdentifier: employment.DisambiguatedOrganizationIdentifier);
                            _ttvContext.DimIdentifierlessData.Add(dimIdentifierlessDatum_affiliation_organization_name);


                            // Create new FactFieldValues for affiliation and delete old
                            FactFieldValue factFieldValuesAffiliationNew = _userProfileService.GetEmptyFactFieldValue();
                            factFieldValuesAffiliationNew.DimIdentifierlessData = dimIdentifierlessDatum_affiliation_organization_name;
                            factFieldValuesAffiliationNew.DimUserProfile = factFieldValuesAffiliation.DimUserProfile;
                            factFieldValuesAffiliationNew.DimFieldDisplaySettings = factFieldValuesAffiliation.DimFieldDisplaySettings;
                            factFieldValuesAffiliationNew.DimRegisteredDataSourceId = factFieldValuesAffiliation.DimRegisteredDataSourceId;
                            factFieldValuesAffiliationNew.DimAffiliation = factFieldValuesAffiliation.DimAffiliation;
                            factFieldValuesAffiliationNew.DimPidIdOrcidPutCodeNavigation = factFieldValuesAffiliation.DimPidIdOrcidPutCodeNavigation;
                            factFieldValuesAffiliationNew.Show = factFieldValuesAffiliation.Show;
                            factFieldValuesAffiliationNew.PrimaryValue = factFieldValuesAffiliation.PrimaryValue;
                            factFieldValuesAffiliationNew.SourceId = factFieldValuesAffiliation.SourceId;
                            factFieldValuesAffiliationNew.SourceDescription = factFieldValuesAffiliation.SourceDescription;

                            _ttvContext.Add(factFieldValuesAffiliationNew);
                            _ttvContext.FactFieldValues.Remove(factFieldValuesAffiliation);
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
                     * Affiliation does not yet exist in profile. Create new.
                     */
                    DimAffiliation dimAffiliation_new = new()
                    {
                        DimOrganizationId = -1,
                        StartDateNavigation = employmentStartDate,
                        EndDateNavigation = employmentEndDate,
                        PositionNameEn = employment.RoleTitle,
                        AffiliationTypeFi = "",
                        AffiliationTypeEn = "",
                        AffiliationTypeSv = "",
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

                    // Add affiliation ORCID put code into DimPid
                    DimPid dimPidOrcidPutCodeAffiliation = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodeAffiliation.PidContent = employment.PutCode.GetDbValue();
                    dimPidOrcidPutCodeAffiliation.PidType = Constants.PidTypes.ORCID_PUT_CODE;
                    dimPidOrcidPutCodeAffiliation.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodeAffiliation.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeAffiliation);

                    // Create FactFieldValues for affiliation
                    factFieldValuesAffiliation = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesAffiliation.DimUserProfile = dimUserProfile;
                    factFieldValuesAffiliation.DimFieldDisplaySettings = dimFieldDisplaySettingsAffiliation;
                    factFieldValuesAffiliation.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    factFieldValuesAffiliation.DimAffiliation = dimAffiliation_new;
                    factFieldValuesAffiliation.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodeAffiliation;

                    // If organization was not found, add organization_name into DimIdentifierlessData
                    if (dimOrganization_id_affiliation == null || dimOrganization_id_affiliation == -1)
                    {
                        DimIdentifierlessDatum dimIdentifierlessData_oganizationName =
                            _organizationHandlerService.CreateIdentifierlessData_OrganizationName(
                                nameFi: "",
                                nameEn: employment.OrganizationName,
                                nameSv: "",
                                orcidDisambiguationSource: employment.DisambiguationSource,
                                orcidDisambiguatedOrganizationIdentifier: employment.DisambiguatedOrganizationIdentifier);
                        _ttvContext.DimIdentifierlessData.Add(dimIdentifierlessData_oganizationName);
                        factFieldValuesAffiliation.DimIdentifierlessData = dimIdentifierlessData_oganizationName;
                    }

                    _ttvContext.FactFieldValues.Add(factFieldValuesAffiliation);
                }

                /*
                 * Affiliation department name handling
                 */
                if (!string.IsNullOrWhiteSpace(employment.DepartmentName))
                {
                    _organizationHandlerService.DepartmentNameHandling(
                        ffv: factFieldValuesAffiliation,
                        departmentNameFi: "",
                        departmentNameEn: employment.DepartmentName,
                        departmentNameSv: "");
                }
            }



            // Publication
            List<OrcidPublication> orcidPublications = _orcidJsonParserService.GetPublications(orcidRecordJson);
            // Get DimFieldDisplaySettings for orcid publication
            DimFieldDisplaySetting dimFieldDisplaySettingsOrcidPublication =
                dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsPublication => dfdsPublication.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY);
            foreach (OrcidPublication orcidPublication in orcidPublications)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimProfileOnlyPublication
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
                    orcidImportHelper.dimProfileOnlyPublicationIds.Add(factFieldValuesPublication.DimProfileOnlyPublicationId);
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




            // Dataset
            List<OrcidDataset> orcidDatasets = _orcidJsonParserService.GetDatasets(orcidRecordJson);
            // Get DimFieldDisplaySettings for orcid dataset
            DimFieldDisplaySetting dimFieldDisplaySettingsProfileOnlyDataset =
                dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsPublication => dfdsPublication.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET);
            foreach (OrcidDataset orcidDataset in orcidDatasets)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimProfileOnlyDataset
                FactFieldValue factFieldValuesProfileOnlyDataset =
                    dimUserProfile.FactFieldValues.FirstOrDefault(ffv =>
                        ffv.DimFieldDisplaySettings == dimFieldDisplaySettingsProfileOnlyDataset &&
                        ffv.DimPidIdOrcidPutCode > 0 &&
                        ffv.DimPidIdOrcidPutCodeNavigation.PidContent == orcidDataset.PutCode.Value.ToString());

                DateTime? datasetCreated;
                // Data set created
                if (orcidDataset.DatasetDate.Year == 0)
                {
                    datasetCreated = null;
                }
                else
                {
                    int datasetYear = orcidDataset.DatasetDate.Year;
                    int datasetMonth = (orcidDataset.DatasetDate.Month > 0 ? orcidDataset.DatasetDate.Month : 1);
                    int datasetDay = (orcidDataset.DatasetDate.Day > 0 ? orcidDataset.DatasetDate.Day : 1);
                    datasetCreated = new DateTime(year: datasetYear, month: datasetMonth, day: datasetDay);
                }

                if (factFieldValuesProfileOnlyDataset != null)
                {
                    // Update existing DimProfileOnlyDataset
                    DimProfileOnlyDataset dimProfileOnlyDataset = factFieldValuesProfileOnlyDataset.DimProfileOnlyDataset;
                    dimProfileOnlyDataset.OrcidWorkType = orcidDataset.Type;
                    dimProfileOnlyDataset.NameEn = orcidDataset.DatasetName;
                    dimProfileOnlyDataset.DatasetCreated = datasetCreated;
                    dimProfileOnlyDataset.Modified = currentDateTime;
                    // Update existing FactFieldValue
                    dimProfileOnlyDataset.Modified = currentDateTime;
                    // Mark as processed
                    orcidImportHelper.dimProfileOnlyDatasetIds.Add(factFieldValuesProfileOnlyDataset.DimProfileOnlyDatasetId);
                }
                else
                {
                    // Create new DimProfileOnlyDataset
                    DimProfileOnlyDataset dimProfileOnlyDataset = _userProfileService.GetEmptyDimProfileOnlyDataset();
                    dimProfileOnlyDataset.OrcidWorkType = orcidDataset.Type;
                    dimProfileOnlyDataset.NameEn = orcidDataset.DatasetName;
                    dimProfileOnlyDataset.DatasetCreated = datasetCreated;
                    dimProfileOnlyDataset.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    dimProfileOnlyDataset.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    dimProfileOnlyDataset.Created = currentDateTime;
                    _ttvContext.DimProfileOnlyDatasets.Add(dimProfileOnlyDataset);

                    // Add dataset's ORCID put code into DimPid
                    DimPid dimPidOrcidPutCodeDataset = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodeDataset.PidContent = orcidDataset.PutCode.GetDbValue();
                    dimPidOrcidPutCodeDataset.PidType = Constants.PidTypes.ORCID_PUT_CODE;
                    dimPidOrcidPutCodeDataset.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodeDataset.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeDataset);

                    // Create FactFieldValues for ORCID dataset
                    factFieldValuesProfileOnlyDataset = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesProfileOnlyDataset.DimUserProfile = dimUserProfile;
                    factFieldValuesProfileOnlyDataset.DimFieldDisplaySettings = dimFieldDisplaySettingsProfileOnlyDataset;
                    factFieldValuesProfileOnlyDataset.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    factFieldValuesProfileOnlyDataset.DimProfileOnlyDataset = dimProfileOnlyDataset;
                    factFieldValuesProfileOnlyDataset.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodeDataset;
                    _ttvContext.FactFieldValues.Add(factFieldValuesProfileOnlyDataset);
                }
            }



            // Funding
            List<OrcidFunding> orcidFundings = _orcidJsonParserService.GetFundings(orcidRecordJson);
            // Get DimFieldDisplaySettings for orcid publication
            DimFieldDisplaySetting dimFieldDisplaySettingsOrcidFunding =
                dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsPublication => dfdsPublication.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION);
            // Reference data
            DimReferencedatum dimReferencedata_award =
                await _ttvContext.DimReferencedata.Where(dr => dr.CodeScheme == Constants.ReferenceDataCodeSchemes.ORCID_FUNDING && dr.CodeValue == Constants.OrcidFundingType_To_ReferenceDataCodeValue.AWARD).AsNoTracking().FirstOrDefaultAsync();
            DimReferencedatum dimReferencedata_contract =
                await _ttvContext.DimReferencedata.Where(dr => dr.CodeScheme == Constants.ReferenceDataCodeSchemes.ORCID_FUNDING && dr.CodeValue == Constants.OrcidFundingType_To_ReferenceDataCodeValue.CONTRACT).AsNoTracking().FirstOrDefaultAsync();
            DimReferencedatum dimReferencedata_grant =
                await _ttvContext.DimReferencedata.Where(dr => dr.CodeScheme == Constants.ReferenceDataCodeSchemes.ORCID_FUNDING && dr.CodeValue == Constants.OrcidFundingType_To_ReferenceDataCodeValue.GRANT).AsNoTracking().FirstOrDefaultAsync();
            DimReferencedatum dimReferencedata_salaryAward =
                await _ttvContext.DimReferencedata.Where(dr => dr.CodeScheme == Constants.ReferenceDataCodeSchemes.ORCID_FUNDING && dr.CodeValue == Constants.OrcidFundingType_To_ReferenceDataCodeValue.SALARY_AWARD).AsNoTracking().FirstOrDefaultAsync();
            foreach (OrcidFunding orcidFunding in orcidFundings)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimProfileOnlyFundingDecision
                FactFieldValue factFieldValuesProfileOnlyFundingDecision =
                    dimUserProfile.FactFieldValues.FirstOrDefault(ffv =>
                        ffv.DimFieldDisplaySettings == dimFieldDisplaySettingsOrcidFunding &&
                        ffv.DimPidIdOrcidPutCode > 0 &&
                        ffv.DimPidIdOrcidPutCodeNavigation.PidContent == orcidFunding.PutCode.Value.ToString());

                // Search organization identifier from DimPid based on ORCID's disambiguated-organization-identifier data.
                int? dimOrganization_id_funding = await _organizationHandlerService.FindOrganizationIdByOrcidDisambiguationIdentifier(
                        orcidDisambiguatedOrganizationIdentifier: orcidFunding.DisambiguatedOrganizationIdentifier,
                        orcidDisambiguationSource: orcidFunding.DisambiguationSource
                    );

                // Start date
                DimDate fundingStartDate = await _ttvContext.DimDates
                    .FirstOrDefaultAsync(dd => dd.Year == orcidFunding.StartDate.Year && dd.Month == orcidFunding.StartDate.Month && dd.Day == orcidFunding.StartDate.Day);

                // End date
                DimDate fundingEndDate = await _ttvContext.DimDates
                    .FirstOrDefaultAsync(dd => dd.Year == orcidFunding.EndDate.Year && dd.Month == orcidFunding.EndDate.Month && dd.Day == orcidFunding.EndDate.Day);

                if (factFieldValuesProfileOnlyFundingDecision != null)
                {
                    // Update existing DimProfileOnlyFundingDecision
                    DimProfileOnlyFundingDecision dimProfileOnlyFundingDecision = factFieldValuesProfileOnlyFundingDecision.DimProfileOnlyFundingDecision;
                    dimProfileOnlyFundingDecision.NameEn = orcidFunding.Name;
                    dimProfileOnlyFundingDecision.DimDateIdStartNavigation = fundingStartDate;
                    dimProfileOnlyFundingDecision.DimDateIdStartNavigation = fundingEndDate;
                    dimProfileOnlyFundingDecision.SourceDescription = orcidFunding.Path;
               
                    /*
                     * Update organization relation or identifierless data for existing funding.
                     */
                    if (dimOrganization_id_funding != null && dimOrganization_id_funding > 0)
                    {
                        /*
                         * Funding relates directly to DimOrganization.
                         */
                        dimProfileOnlyFundingDecision.DimOrganizationIdFunder = (int)dimOrganization_id_funding;

                        /*
                         * When funding has related DimOrganization, possibly existing DimIdentifierlessData of type organization_name must be removed.
                         */
                        if (factFieldValuesProfileOnlyFundingDecision.DimIdentifierlessDataId != -1 && factFieldValuesProfileOnlyFundingDecision.DimIdentifierlessData.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                        {
                            _ttvContext.DimIdentifierlessData.Remove(factFieldValuesProfileOnlyFundingDecision.DimIdentifierlessData);
                        }
                    }
                    else
                    {
                        /*
                         * Funding does not relate directly to any DimOrganization.
                         * Update or create relation to DimIdentifierlessData via FactFieldValues.
                         */
                        if (factFieldValuesProfileOnlyFundingDecision.DimIdentifierlessDataId != -1 && factFieldValuesProfileOnlyFundingDecision.DimIdentifierlessData.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                        {
                            /*
                             * Update organization name in existing DimIdentifierlessData.
                             */
                            factFieldValuesProfileOnlyFundingDecision.DimIdentifierlessData.ValueEn = orcidFunding.OrganizationName;
                        }
                        else
                        {
                            /*
                             * Create new DimIdentifierlessData for organization name.
                             */
                            DimIdentifierlessDatum dimIdentifierlessDatum_funding_organization_name =
                                _organizationHandlerService.CreateIdentifierlessData_OrganizationName(
                                    nameFi: "",
                                    nameEn: orcidFunding.OrganizationName,
                                    nameSv: "",
                                    orcidDisambiguationSource: orcidFunding.DisambiguationSource,
                                    orcidDisambiguatedOrganizationIdentifier: orcidFunding.DisambiguatedOrganizationIdentifier);
                            _ttvContext.DimIdentifierlessData.Add(dimIdentifierlessDatum_funding_organization_name);
                            factFieldValuesProfileOnlyFundingDecision.DimIdentifierlessData = dimIdentifierlessDatum_funding_organization_name;
                        }
                    }

                    // Update existing FactFieldValue
                    factFieldValuesProfileOnlyFundingDecision.Modified = currentDateTime;
                    // Mark as processed
                    orcidImportHelper.dimProfileOnlyFundingDecisionIds.Add(factFieldValuesProfileOnlyFundingDecision.DimProfileOnlyFundingDecisionId);
                }
                else
                {
                    // Create new DimProfileOnlyFundingDecision
                    DimProfileOnlyFundingDecision dimProfileOnlyFundingDecision = _userProfileService.GetEmptyDimProfileOnlyFundingDecision();
                    dimProfileOnlyFundingDecision.NameEn = orcidFunding.Name;
                    dimProfileOnlyFundingDecision.DescriptionEn = orcidFunding.Description;
                    dimProfileOnlyFundingDecision.DimDateIdStartNavigation = fundingStartDate;
                    dimProfileOnlyFundingDecision.DimDateIdEndNavigation = fundingEndDate;
                    dimProfileOnlyFundingDecision.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    dimProfileOnlyFundingDecision.Created = currentDateTime;
                    dimProfileOnlyFundingDecision.SourceDescription = orcidFunding.Path;
                    // If organization was found, add relation
                    if (dimOrganization_id_funding != null && dimOrganization_id_funding > 0)
                    {
                        dimProfileOnlyFundingDecision.DimOrganizationIdFunder = (int)dimOrganization_id_funding;
                    }
                    _ttvContext.DimProfileOnlyFundingDecisions.Add(dimProfileOnlyFundingDecision);

                    // Add funding's ORCID put code into DimPid
                    DimPid dimPidOrcidPutCodePublication = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodePublication.PidContent = orcidFunding.PutCode.GetDbValue();
                    dimPidOrcidPutCodePublication.PidType = Constants.PidTypes.ORCID_PUT_CODE;
                    dimPidOrcidPutCodePublication.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodePublication.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodePublication);

                    // Create FactFieldValues for orcid funding
                    factFieldValuesProfileOnlyFundingDecision = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesProfileOnlyFundingDecision.DimUserProfile = dimUserProfile;
                    factFieldValuesProfileOnlyFundingDecision.DimFieldDisplaySettings = dimFieldDisplaySettingsOrcidFunding;
                    factFieldValuesProfileOnlyFundingDecision.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    factFieldValuesProfileOnlyFundingDecision.DimProfileOnlyFundingDecision = dimProfileOnlyFundingDecision;
                    factFieldValuesProfileOnlyFundingDecision.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodePublication;

                    // If organization was not found, add organization_name into DimIdentifierlessData
                    if (dimOrganization_id_funding == null || dimOrganization_id_funding == -1)
                    {
                        DimIdentifierlessDatum dimIdentifierlessData_oganizationName =
                            _organizationHandlerService.CreateIdentifierlessData_OrganizationName(
                                nameFi: "",
                                nameEn: orcidFunding.OrganizationName,
                                nameSv: "",
                                orcidDisambiguationSource: orcidFunding.DisambiguationSource,
                                orcidDisambiguatedOrganizationIdentifier: orcidFunding.DisambiguatedOrganizationIdentifier);
                        _ttvContext.DimIdentifierlessData.Add(dimIdentifierlessData_oganizationName);
                        factFieldValuesProfileOnlyFundingDecision.DimIdentifierlessData = dimIdentifierlessData_oganizationName;
                    }

                    // Set correct DimReferenceDatum based on ORCID funding type
                    switch (orcidFunding.Type)
                    {
                        case Constants.OrcidFundingTypes.AWARD:
                            factFieldValuesProfileOnlyFundingDecision.DimReferencedataActorRoleId = dimReferencedata_award.Id;
                            break;
                        case Constants.OrcidFundingTypes.CONTRACT:
                            factFieldValuesProfileOnlyFundingDecision.DimReferencedataActorRoleId = dimReferencedata_contract.Id;
                            break;
                        case Constants.OrcidFundingTypes.GRANT:
                            factFieldValuesProfileOnlyFundingDecision.DimReferencedataActorRoleId = dimReferencedata_grant.Id;
                            break;
                        case Constants.OrcidFundingTypes.SALARY_AWARD:
                            factFieldValuesProfileOnlyFundingDecision.DimReferencedataActorRoleId = dimReferencedata_salaryAward.Id;
                            break;
                    }

                    _ttvContext.FactFieldValues.Add(factFieldValuesProfileOnlyFundingDecision);
                }
            }



            // Invited positions, distinctions, memberships and services => Research activity
            List<OrcidResearchActivity> orcidResearchActivity_invitedPositionsAndDistinctions = _orcidJsonParserService.GetProfileOnlyResearchActivityItems(orcidRecordJson);
            // Get DimFieldDisplaySettings for research activity
            DimFieldDisplaySetting dimFieldDisplaySettingsResearchActivity =
                dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfdsResearchActivity => dfdsResearchActivity.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_RESEARCH_ACTIVITY);
            // Reference data
            DimReferencedatum dimReferencedata_invitedPosition =
                await _ttvContext.DimReferencedata.Where(dr => dr.CodeScheme == Constants.ReferenceDataCodeSchemes.ORCID_RESEARCH_ACTIVITY && dr.CodeValue == Constants.OrcidResearchActivityType_To_ReferenceDataCodeValue.INVITED_POSITION).AsNoTracking().FirstOrDefaultAsync();
            DimReferencedatum dimReferencedata_distinction =
                await _ttvContext.DimReferencedata.Where(dr => dr.CodeScheme == Constants.ReferenceDataCodeSchemes.ORCID_RESEARCH_ACTIVITY && dr.CodeValue == Constants.OrcidResearchActivityType_To_ReferenceDataCodeValue.DISTINCTION).AsNoTracking().FirstOrDefaultAsync();
            DimReferencedatum dimReferencedata_membership =
                await _ttvContext.DimReferencedata.Where(dr => dr.CodeScheme == Constants.ReferenceDataCodeSchemes.ORCID_RESEARCH_ACTIVITY && dr.CodeValue == Constants.OrcidResearchActivityType_To_ReferenceDataCodeValue.MEMBERSHIP).AsNoTracking().FirstOrDefaultAsync();
            DimReferencedatum dimReferencedata_qualification =
                await _ttvContext.DimReferencedata.Where(dr => dr.CodeScheme == Constants.ReferenceDataCodeSchemes.ORCID_RESEARCH_ACTIVITY && dr.CodeValue == Constants.OrcidResearchActivityType_To_ReferenceDataCodeValue.QUALIFICATION).AsNoTracking().FirstOrDefaultAsync();
            DimReferencedatum dimReferencedata_service =
                await _ttvContext.DimReferencedata.Where(dr => dr.CodeScheme == Constants.ReferenceDataCodeSchemes.ORCID_RESEARCH_ACTIVITY && dr.CodeValue == Constants.OrcidResearchActivityType_To_ReferenceDataCodeValue.SERVICE).AsNoTracking().FirstOrDefaultAsync();


            foreach (OrcidResearchActivity orcidResearchActivity in orcidResearchActivity_invitedPositionsAndDistinctions)
            {
                // Check if FactFieldValues contains entry, which points to ORCID put code value in DimProfileOnlyResearchActivity
                FactFieldValue factFieldValuesDimProfileOnlyResearchActivity =
                    dimUserProfile.FactFieldValues.FirstOrDefault(ffv =>
                        ffv.DimFieldDisplaySettings == dimFieldDisplaySettingsResearchActivity &&
                        ffv.DimPidIdOrcidPutCode > 0 &&
                        ffv.DimPidIdOrcidPutCodeNavigation.PidContent == orcidResearchActivity.PutCode.Value.ToString());

                // Search organization identifier from DimPid based on ORCID's disambiguated-organization-identifier data.
                int? dimOrganization_id_research_activity = await _organizationHandlerService.FindOrganizationIdByOrcidDisambiguationIdentifier(
                        orcidDisambiguatedOrganizationIdentifier: orcidResearchActivity.DisambiguatedOrganizationIdentifier,
                        orcidDisambiguationSource: orcidResearchActivity.DisambiguationSource
                    );

                // Start date
                DimDate researchActivityStartDate = await _ttvContext.DimDates
                    .FirstOrDefaultAsync(dd => dd.Year == orcidResearchActivity.StartDate.Year && dd.Month == orcidResearchActivity.StartDate.Month && dd.Day == orcidResearchActivity.StartDate.Day);

                // End date
                DimDate researchActivityEndDate = await _ttvContext.DimDates
                    .FirstOrDefaultAsync(dd => dd.Year == orcidResearchActivity.EndDate.Year && dd.Month == orcidResearchActivity.EndDate.Month && dd.Day == orcidResearchActivity.EndDate.Day);

                if (factFieldValuesDimProfileOnlyResearchActivity != null)
                {
                    // Update existing DimProfileOnlyResearchActivity
                    DimProfileOnlyResearchActivity dimProfileOnlyResearchActivity_existing = factFieldValuesDimProfileOnlyResearchActivity.DimProfileOnlyResearchActivity;
                    dimProfileOnlyResearchActivity_existing.DimDateIdStartNavigation = researchActivityStartDate;
                    dimProfileOnlyResearchActivity_existing.DimDateIdEndNavigation = researchActivityEndDate;
                    dimProfileOnlyResearchActivity_existing.NameEn = orcidResearchActivity.RoleTitle.Length > 255 ? orcidResearchActivity.RoleTitle.Substring(0, 255) : orcidResearchActivity.RoleTitle; // Database size 255

                    /*
                     * Update organization relation or identifierless data for existing affiliation.
                     */
                    if (dimOrganization_id_research_activity != null && dimOrganization_id_research_activity > 0)
                    {
                        /*
                         * Affiliation relates directly to DimOrganization.
                         */
                        dimProfileOnlyResearchActivity_existing.DimOrganizationId = (int)dimOrganization_id_research_activity;

                        /*
                         * When affiliation has related DimOrganization, possibly existing DimIdentifierlessData of type organization_name must be removed.
                         */
                        if (factFieldValuesDimProfileOnlyResearchActivity.DimIdentifierlessDataId != -1 && factFieldValuesDimProfileOnlyResearchActivity.DimIdentifierlessData.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                        {
                            _ttvContext.DimIdentifierlessData.Remove(factFieldValuesDimProfileOnlyResearchActivity.DimIdentifierlessData);
                        }
                    }
                    else
                    {
                        /*
                         * Affiliation does to relate directly to any DimOrganization.
                         * Update or create relation to DimIdentifierlessData via FactFieldValues.
                         */
                        if (factFieldValuesDimProfileOnlyResearchActivity.DimIdentifierlessDataId != -1 && factFieldValuesDimProfileOnlyResearchActivity.DimIdentifierlessData.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                        {
                            /*
                             * Update organization name in existing DimIdentifierlessData.
                             */
                            factFieldValuesDimProfileOnlyResearchActivity.DimIdentifierlessData.ValueEn = orcidResearchActivity.OrganizationName;
                        }
                        else
                        {
                            /*
                             * Create new DimIdentifierlessData for organization name.
                             */
                            DimIdentifierlessDatum dimIdentifierlessDatum_research_activity_organization_name =
                                _organizationHandlerService.CreateIdentifierlessData_OrganizationName(
                                    nameFi: "",
                                    nameEn: orcidResearchActivity.OrganizationName,
                                    nameSv: "",
                                    orcidDisambiguationSource: orcidResearchActivity.DisambiguationSource,
                                    orcidDisambiguatedOrganizationIdentifier: orcidResearchActivity.DisambiguatedOrganizationIdentifier);
                            _ttvContext.DimIdentifierlessData.Add(dimIdentifierlessDatum_research_activity_organization_name);
                            factFieldValuesDimProfileOnlyResearchActivity.DimIdentifierlessData = dimIdentifierlessDatum_research_activity_organization_name;
                        }
                    }

                    dimProfileOnlyResearchActivity_existing.Modified = currentDateTime;
                    // Update existing FactFieldValue
                    factFieldValuesDimProfileOnlyResearchActivity.Modified = currentDateTime;
                    // Mark as processed
                    orcidImportHelper.dimProfileOnlyResearchActivityIds.Add(factFieldValuesDimProfileOnlyResearchActivity.DimProfileOnlyResearchActivityId);
                }
                else
                {
                    // Create new DimProfileOnlyResearchActivity
                    DimProfileOnlyResearchActivity dimProfileOnlyResearchActivity_new = _userProfileService.GetEmptyDimProfileOnlyResearchActivity();
                    dimProfileOnlyResearchActivity_new.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    dimProfileOnlyResearchActivity_new.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    dimProfileOnlyResearchActivity_new.NameEn = orcidResearchActivity.RoleTitle.Length > 255 ? orcidResearchActivity.RoleTitle.Substring(0,255) : orcidResearchActivity.RoleTitle; // Database size 255
                    dimProfileOnlyResearchActivity_new.DimDateIdStartNavigation = researchActivityStartDate;
                    dimProfileOnlyResearchActivity_new.DimDateIdEndNavigation = researchActivityEndDate;
                    dimProfileOnlyResearchActivity_new.Created = currentDateTime;
                    // If organization was found, add relation
                    if (dimOrganization_id_research_activity != null && dimOrganization_id_research_activity > 0)
                    {
                        dimProfileOnlyResearchActivity_new.DimOrganizationId = (int)dimOrganization_id_research_activity;
                    }
                    _ttvContext.DimProfileOnlyResearchActivities.Add(dimProfileOnlyResearchActivity_new);

                    // Add research activity's ORCID put code into DimPid
                    DimPid dimPidOrcidPutCodeResearchActivity = _userProfileService.GetEmptyDimPid();
                    dimPidOrcidPutCodeResearchActivity.PidContent = orcidResearchActivity.PutCode.GetDbValue();
                    dimPidOrcidPutCodeResearchActivity.PidType = Constants.PidTypes.ORCID_PUT_CODE;
                    dimPidOrcidPutCodeResearchActivity.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
                    dimPidOrcidPutCodeResearchActivity.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                    _ttvContext.DimPids.Add(dimPidOrcidPutCodeResearchActivity);

                    // Create FactFieldValues for research activity
                    factFieldValuesDimProfileOnlyResearchActivity = _userProfileService.GetEmptyFactFieldValue();
                    factFieldValuesDimProfileOnlyResearchActivity.DimUserProfile = dimUserProfile;
                    factFieldValuesDimProfileOnlyResearchActivity.DimFieldDisplaySettings = dimFieldDisplaySettingsResearchActivity;
                    factFieldValuesDimProfileOnlyResearchActivity.DimRegisteredDataSourceId = orcidRegisteredDataSourceId;
                    factFieldValuesDimProfileOnlyResearchActivity.DimProfileOnlyResearchActivity = dimProfileOnlyResearchActivity_new;
                    factFieldValuesDimProfileOnlyResearchActivity.DimPidIdOrcidPutCodeNavigation = dimPidOrcidPutCodeResearchActivity;

                    // Set correct DimReferenceDatum based on ORCID activity type
                    switch (orcidResearchActivity.OrcidActivityType)
                    {
                        case Constants.OrcidResearchActivityTypes.INVITED_POSITION:
                            factFieldValuesDimProfileOnlyResearchActivity.DimReferencedataActorRoleId = dimReferencedata_invitedPosition.Id;
                            break;
                        case Constants.OrcidResearchActivityTypes.DISTINCTION:
                            factFieldValuesDimProfileOnlyResearchActivity.DimReferencedataActorRoleId = dimReferencedata_distinction.Id;
                            break;
                        case Constants.OrcidResearchActivityTypes.MEMBERSHIP:
                            factFieldValuesDimProfileOnlyResearchActivity.DimReferencedataActorRoleId = dimReferencedata_membership.Id;
                            break;
                        case Constants.OrcidResearchActivityTypes.QUALIFICATION:
                            factFieldValuesDimProfileOnlyResearchActivity.DimReferencedataActorRoleId = dimReferencedata_qualification.Id;
                            break;
                        case Constants.OrcidResearchActivityTypes.SERVICE:
                            factFieldValuesDimProfileOnlyResearchActivity.DimReferencedataActorRoleId = dimReferencedata_service.Id;
                            break;
                    }
                    _ttvContext.FactFieldValues.Add(factFieldValuesDimProfileOnlyResearchActivity);

                    // If organization was not found, add organization_name into DimIdentifierlessData
                    if (dimOrganization_id_research_activity == null || dimOrganization_id_research_activity == -1)
                    {
                        DimIdentifierlessDatum dimIdentifierlessData_oganizationName =
                            _organizationHandlerService.CreateIdentifierlessData_OrganizationName(
                                nameFi: "",
                                nameEn: orcidResearchActivity.OrganizationName,
                                nameSv: "",
                                orcidDisambiguationSource: orcidResearchActivity.DisambiguationSource,
                                orcidDisambiguatedOrganizationIdentifier: orcidResearchActivity.DisambiguatedOrganizationIdentifier);
                        _ttvContext.DimIdentifierlessData.Add(dimIdentifierlessData_oganizationName);
                        factFieldValuesDimProfileOnlyResearchActivity.DimIdentifierlessData = dimIdentifierlessData_oganizationName;
                    }
                }

                /*
                 * Research activity department name handling
                 */
                if (!string.IsNullOrWhiteSpace(orcidResearchActivity.DepartmentName))
                {
                    _organizationHandlerService.DepartmentNameHandling(
                        ffv: factFieldValuesDimProfileOnlyResearchActivity,
                        departmentNameFi: "",
                        departmentNameEn: orcidResearchActivity.DepartmentName,
                        departmentNameSv: "");
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
                    !orcidImportHelper.dimProfileOnlyPublicationIds.Contains(ffv.DimProfileOnlyPublicationId)).ToList();
            foreach (FactFieldValue removableFfvPublication in removableFfvPublications.Distinct())
            {
                _ttvContext.FactFieldValues.Remove(removableFfvPublication);
                _ttvContext.DimProfileOnlyPublications.Remove(removableFfvPublication.DimProfileOnlyPublication);
                if (removableFfvPublication.DimPidIdOrcidPutCode > 0)
                {
                    _ttvContext.DimPids.Remove(removableFfvPublication.DimPidIdOrcidPutCodeNavigation);
                }
            }

            // Remove datasets, which user has deleted in ORCID
            List<FactFieldValue> removableFfvDatasets =
                dimUserProfile.FactFieldValues.Where(ffv =>
                    ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId &&
                    ffv.DimProfileOnlyDatasetId > 0 &&
                    !orcidImportHelper.dimProfileOnlyDatasetIds.Contains(ffv.DimProfileOnlyDatasetId)).ToList();
            foreach (FactFieldValue removableFfvDataset in removableFfvDatasets.Distinct())
            {
                _ttvContext.FactFieldValues.Remove(removableFfvDataset);
                _ttvContext.DimProfileOnlyDatasets.Remove(removableFfvDataset.DimProfileOnlyDataset);
                if (removableFfvDataset.DimPidIdOrcidPutCode > 0)
                {
                    _ttvContext.DimPids.Remove(removableFfvDataset.DimPidIdOrcidPutCodeNavigation);
                }
            }

            // Remove research activities, which user has deleted in ORCID
            List<FactFieldValue> removableFfvResearchActivities =
                dimUserProfile.FactFieldValues.Where(ffv =>
                    ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId &&
                    ffv.DimProfileOnlyResearchActivityId > 0 &&
                    !orcidImportHelper.dimProfileOnlyResearchActivityIds.Contains(ffv.DimProfileOnlyResearchActivityId)).ToList();
            foreach (FactFieldValue removableFfvResearchActivity in removableFfvResearchActivities.Distinct())
            {
                _ttvContext.FactFieldValues.Remove(removableFfvResearchActivity);
                _ttvContext.DimProfileOnlyResearchActivities.Remove(removableFfvResearchActivity.DimProfileOnlyResearchActivity);
                if (removableFfvResearchActivity.DimPidIdOrcidPutCode > 0)
                {
                    _ttvContext.DimPids.Remove(removableFfvResearchActivity.DimPidIdOrcidPutCodeNavigation);
                }
            }

            // Remove fundings, which user has deleted in ORCID
            List<FactFieldValue> removableFfvFundings =
                dimUserProfile.FactFieldValues.Where(ffv =>
                    ffv.DimRegisteredDataSourceId == orcidRegisteredDataSourceId &&
                    ffv.DimProfileOnlyFundingDecisionId > 0 &&
                    !orcidImportHelper.dimProfileOnlyFundingDecisionIds.Contains(ffv.DimProfileOnlyFundingDecisionId)).ToList();
            foreach (FactFieldValue removableFfvFunding in removableFfvFundings.Distinct())
            {
                _ttvContext.FactFieldValues.Remove(removableFfvFunding);
                _ttvContext.DimProfileOnlyFundingDecisions.Remove(removableFfvFunding.DimProfileOnlyFundingDecision);
                if (removableFfvFunding.DimPidIdOrcidPutCode > 0)
                {
                    _ttvContext.DimPids.Remove(removableFfvFunding.DimPidIdOrcidPutCodeNavigation);
                }
                // Funding organization can be stored in DimIdentifierlessData
                if (removableFfvFunding.DimIdentifierlessDataId > 0)
                {
                    // DimIdentifierlessData can have child entity
                    _ttvContext.DimIdentifierlessData.RemoveRange(removableFfvFunding.DimIdentifierlessData.InverseDimIdentifierlessData);
                    _ttvContext.DimIdentifierlessData.Remove(removableFfvFunding.DimIdentifierlessData);
                }
            }

            await _ttvContext.SaveChangesAsync();

            return true;
        }



        /*
         * Import additional data by making ORCID API requests to item specific endpoints.
         * The main ORCID record contains most of the required data, but for some items
         * additional info must be requested separately. Main ORCID record indicates detail
         * path, which is used to fetch the details.
         *
         * Implemented for:
         *   - fundings
         */
        public async Task<bool> ImportAdditionalData(List<FactFieldValue> factFieldValues, String orcidAccessToken)
        {
            foreach (FactFieldValue ffv in factFieldValues)
            {
                if (ffv.DimProfileOnlyFundingDecisionId > 0)
                {
                    string result = await _orcidApiService.GetDataFromMemberApi(path: ffv.DimProfileOnlyFundingDecision.SourceDescription, orcidAccessToken: orcidAccessToken);
                    OrcidFunding orcidFunding = _orcidJsonParserService.GetFundingDetail(fundingDetailJson: result);

                    ffv.DimProfileOnlyFundingDecision.OrcidWorkType = orcidFunding.Type;
                    ffv.DimProfileOnlyFundingDecision.NameEn = orcidFunding.Name;
                    ffv.DimProfileOnlyFundingDecision.DescriptionEn = orcidFunding.Description;
                    ffv.DimProfileOnlyFundingDecision.AmountInFundingDecisionCurrency =
                        _utilityService.StringToNullableDecimal(orcidFunding.Amount);
                    ffv.DimProfileOnlyFundingDecision.FundingDecisionCurrencyAbbreviation = orcidFunding.CurrencyCode;

                    // Set EUR value
                    if (orcidFunding.CurrencyCode == "EUR" && ffv.DimProfileOnlyFundingDecision.AmountInFundingDecisionCurrency != null)
                    {
                        ffv.DimProfileOnlyFundingDecision.AmountInEur = (decimal)ffv.DimProfileOnlyFundingDecision.AmountInFundingDecisionCurrency;
                    }
                }
            }

            return false;
        }
    }
}