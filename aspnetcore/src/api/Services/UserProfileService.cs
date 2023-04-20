using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Ttv;
using api.Models.ProfileEditor;
using api.Models.ProfileEditor.Items;
using Microsoft.EntityFrameworkCore;
using api.Models.Common;
using api.Models.Orcid;
using api.Models.Log;
using Dapper;
using System.Transactions;
using api.Controllers;
using Microsoft.Extensions.Logging;
using api.Models.Elasticsearch;
using Elasticsearch.Net;
using api.Models.Api;
using Serilog;
using System.Text.Json;
using static api.Models.Common.Constants;

namespace api.Services
{
    /*
     * UserProfileService implements utilities, which simplify handling of userprofile and related data.
     */
    public class UserProfileService : IUserProfileService
    {
        private readonly TtvContext _ttvContext;
        private readonly IDataSourceHelperService _dataSourceHelperService;
        private readonly IUtilityService _utilityService;
        private readonly ILanguageService _languageService;
        private readonly IDuplicateHandlerService _duplicateHandlerService;
        private readonly ISharingService _sharingService;
        private readonly ITtvSqlService _ttvSqlService;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(TtvContext ttvContext,
            IDataSourceHelperService dataSourceHelperService,
            IUtilityService utilityService,
            ILanguageService languageService,
            IDuplicateHandlerService duplicateHandlerService,
            ISharingService sharingService,
            ITtvSqlService ttvSqlService,
            ILogger<UserProfileService> logger)
        {
            _ttvContext = ttvContext;
            _dataSourceHelperService = dataSourceHelperService;
            _utilityService = utilityService;
            _languageService = languageService;
            _duplicateHandlerService = duplicateHandlerService;
            _sharingService = sharingService;
            _ttvSqlService = ttvSqlService;
            _logger = logger;
        }

        public UserProfileService(
            TtvContext ttvContext,
            ITtvSqlService ttvSqlService,
            ILogger<UserProfileService> logger)
        {
            _ttvContext = ttvContext;
            _ttvSqlService = ttvSqlService;
            _logger = logger;
        }

        public UserProfileService() { }
        public UserProfileService(IUtilityService utilityService) {
            _utilityService = utilityService;
        }
        public UserProfileService(IDataSourceHelperService dataSourceHelperService)
        {
            _dataSourceHelperService = dataSourceHelperService;
        }

        /*
         * Get FieldIdentifiers.
         */
        public List<int> GetFieldIdentifiers()
        {
            return new List<int>()
            {
                Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS,
                Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER,
                Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE,
                Constants.FieldIdentifiers.PERSON_KEYWORD,
                Constants.FieldIdentifiers.PERSON_NAME,
                Constants.FieldIdentifiers.PERSON_OTHER_NAMES,
                Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION,
                Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER,
                Constants.FieldIdentifiers.PERSON_WEB_LINK,
                Constants.FieldIdentifiers.ACTIVITY_AFFILIATION,
                Constants.FieldIdentifiers.ACTIVITY_EDUCATION,
                Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY,
                Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION,
                Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET,
                Constants.FieldIdentifiers.ACTIVITY_RESEARCH_ACTIVITY
            };
        }

        /*
         * Update ORCID tokens in DimUserProfile
         */
        public async Task UpdateOrcidTokensInDimUserProfile(int dimUserProfileId, OrcidTokens orcidTokens)
        {
            DimUserProfile dimUserProfile = await _ttvContext.DimUserProfiles.FindAsync(dimUserProfileId);
            dimUserProfile.OrcidAccessToken = orcidTokens.AccessToken;
            dimUserProfile.OrcidRefreshToken = orcidTokens.RefreshToken;
            dimUserProfile.OrcidTokenScope = orcidTokens.Scope;
            dimUserProfile.OrcidTokenExpires = orcidTokens.ExpiresDatetime;
            await _ttvContext.SaveChangesAsync();
        }

        /*
         * Check if data related to FactFieldValue can be removed.
         * Data from registered data source ORCID can be removed.
         */
        public bool CanDeleteFactFieldValueRelatedData(FactFieldValue ffv)
        {
            return ffv.DimRegisteredDataSourceId == _dataSourceHelperService.DimRegisteredDataSourceId_ORCID;
        }

        /*
         * Get DimUserProfile based on ORCID Id.
         */
        public async Task<DimUserProfile> GetUserprofile(string orcidId)
        {
            return await _ttvContext.DimUserProfiles.Where(dup => dup.OrcidId == orcidId).AsNoTracking().FirstOrDefaultAsync();
        }

        /*
         * Get DimUserProfile based on Id.
         */
        public async Task<DimUserProfile> GetUserprofileById(int Id)
        {
            return await _ttvContext.DimUserProfiles.Where(dup => dup.Id == Id).AsNoTracking().FirstOrDefaultAsync();
        }

        /*
         * Get Id of DimUserProfile based on ORCID Id.
         */
        public async Task<int> GetUserprofileId(string orcidId)
        {
            DimUserProfile dimUserProfile = await _ttvContext.DimUserProfiles.Where(dup => dup.OrcidId == orcidId).AsNoTracking().FirstOrDefaultAsync();
            if (dimUserProfile == null)
            {
                return -1;
            }
            else
            {
                return dimUserProfile.Id;
            }
        }

        /*
         * Check if user profile exists for ORCID Id.
         */
        public async Task<bool> UserprofileExistsForOrcidId(string orcidId)
        {
            int userProfileId = await GetUserprofileId(orcidId: orcidId);
            return userProfileId > -1;
        }

        /*
         * Add or update DimName.
         */
        public async Task<DimName> AddOrUpdateDimName(String lastName, String firstNames, int dimKnownPersonId, int dimRegisteredDataSourceId)
        {
            DimName dimName = await _ttvContext.DimNames.FirstOrDefaultAsync(dn => dn.DimKnownPersonIdConfirmedIdentityNavigation.Id == dimKnownPersonId && dn.DimRegisteredDataSourceId == dimRegisteredDataSourceId);
            if (dimName == null)
            {
                dimName = new DimName()
                {
                    LastName = lastName,
                    FirstNames = firstNames,
                    DimKnownPersonIdConfirmedIdentity = dimKnownPersonId,
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.GetCurrentDateTime(),
                    Modified = _utilityService.GetCurrentDateTime(),
                    DimRegisteredDataSourceId = dimRegisteredDataSourceId
                };
                _ttvContext.DimNames.Add(dimName);
            }
            else
            {
                dimName.LastName = lastName;
                dimName.FirstNames = firstNames;
                dimName.Modified = _utilityService.GetCurrentDateTime();
            }
            await _ttvContext.SaveChangesAsync();
            return dimName;
        }

        /*
         * Add or update DimResearcherDescription.
         */
        public async Task<DimResearcherDescription> AddOrUpdateDimResearcherDescription(String description_fi, String description_en, String description_sv, int dimKnownPersonId, int dimRegisteredDataSourceId)
        {
            DimResearcherDescription dimResearcherDescription = await _ttvContext.DimResearcherDescriptions.FirstOrDefaultAsync(dr => dr.DimKnownPersonId == dimKnownPersonId && dr.DimRegisteredDataSourceId == dimRegisteredDataSourceId);
            if (dimResearcherDescription == null)
            {
                dimResearcherDescription = new DimResearcherDescription()
                {
                    ResearchDescriptionFi = description_fi,
                    ResearchDescriptionEn = description_en,
                    ResearchDescriptionSv = description_sv,
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.GetCurrentDateTime(),
                    Modified = _utilityService.GetCurrentDateTime(),
                    DimKnownPersonId = dimKnownPersonId,
                    DimRegisteredDataSourceId = dimRegisteredDataSourceId
                };
                _ttvContext.DimResearcherDescriptions.Add(dimResearcherDescription);
            }
            else
            {
                dimResearcherDescription.ResearchDescriptionFi = description_fi;
                dimResearcherDescription.ResearchDescriptionEn = description_en;
                dimResearcherDescription.ResearchDescriptionSv = description_sv;
                dimResearcherDescription.Modified = _utilityService.GetCurrentDateTime();
            }
            await _ttvContext.SaveChangesAsync();
            return dimResearcherDescription;
        }

        /*
         * Add or update DimEmailAddrress.
         */
        public async Task<DimEmailAddrress> AddOrUpdateDimEmailAddress(string emailAddress, int dimKnownPersonId, int dimRegisteredDataSourceId)
        {
            DimEmailAddrress dimEmailAddress = await _ttvContext.DimEmailAddrresses.FirstOrDefaultAsync(dr => dr.Email == emailAddress && dr.DimKnownPersonId == dimKnownPersonId && dr.DimRegisteredDataSourceId == dimRegisteredDataSourceId);
            if (dimEmailAddress == null)
            {
                dimEmailAddress = new DimEmailAddrress()
                {
                    Email = emailAddress,
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.GetCurrentDateTime(),
                    Modified = _utilityService.GetCurrentDateTime(),
                    DimKnownPersonId = dimKnownPersonId,
                    DimRegisteredDataSourceId = dimRegisteredDataSourceId
                };
                _ttvContext.DimEmailAddrresses.Add(dimEmailAddress);
            }
            else
            {
                dimEmailAddress.Modified = _utilityService.GetCurrentDateTime();
            }
            await _ttvContext.SaveChangesAsync();
            return dimEmailAddress;
        }

        /*
         * Get new DimKnownPerson.
         * - ORCID ID must be used as a source_id.
         * - Registered data source must point to ORCID.
         */
        public DimKnownPerson GetNewDimKnownPerson(string orcidId, DateTime currentDateTime)
        {
            return new DimKnownPerson()
            {
                SourceId = orcidId, // ORCID ID must be used in dim_known_person.source_id
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = currentDateTime,
                Modified = currentDateTime,
                DimRegisteredDataSourceId = _dataSourceHelperService.DimRegisteredDataSourceId_ORCID
            };
        }

        /*
         * Get empty FactFieldValue.
         * Must use -1 in required foreign keys.
         */
        public FactFieldValue GetEmptyFactFieldValue()
        {
            return new FactFieldValue()
            {
                DimUserProfileId = -1,
                DimFieldDisplaySettingsId = -1,
                DimNameId = -1,
                DimWebLinkId = -1,
                DimFundingDecisionId = -1,
                DimPublicationId = -1,
                DimPidId = -1,
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
                DimProfileOnlyDatasetId = -1,
                DimProfileOnlyFundingDecisionId = -1,
                DimProfileOnlyPublicationId = -1,
                DimProfileOnlyResearchActivityId = -1,
                DimKeywordId = -1,
                DimAffiliationId = -1,
                DimResearcherToResearchCommunityId = -1,
                DimResearchDatasetId = -1,
                DimReferencedataFieldOfScienceId = -1,
                DimReferencedataActorRoleId = -1,
                Show = false,
                PrimaryValue = false,
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.GetCurrentDateTime(),
                Modified = _utilityService.GetCurrentDateTime()
            };
        }

        /*
         * Get empty FactFieldValue. Set SourceID to DEMO.
         */
        public FactFieldValue GetEmptyFactFieldValueDemo()
        {
            FactFieldValue factFieldValue = this.GetEmptyFactFieldValue();
            factFieldValue.SourceId = Constants.SourceIdentifiers.DEMO;
            return factFieldValue;
        }

        /*
         * Get empty DimProfileOnlyPublication.
         * Must use -1 in required foreign keys.
         */
        public DimProfileOnlyPublication GetEmptyDimProfileOnlyPublication()
        {
            return new DimProfileOnlyPublication()
            {
                DimProfileOnlyPublicationId = null,
                ParentTypeClassificationCode = -1,
                TypeClassificationCode = -1,
                PublicationFormatCode = -1,
                ArticleTypeCode = -1,
                TargetAudienceCode = -1,
                OrcidWorkType = null,
                PublicationName = "",
                ConferenceName = null,
                ShortDescription = null,
                PublicationYear = null,
                PublicationId = "",
                AuthorsText = "",
                NumberOfAuthors = null,
                PageNumberText = null,
                ArticleNumberText = null,
                IssueNumber = null,
                Volume = null,
                PublicationCountryCode = null,
                PublisherName = null,
                PublisherLocation = null,
                ParentPublicationName = null,
                ParentPublicationEditors = null,
                LicenseCode = null,
                LanguageCode = -1,
                OpenAccessCode = null,
                OriginalPublicationId = null,
                PeerReviewed = null,
                Report = null,
                ThesisTypeCode = null,
                DoiHandle = null,
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = null,
                Modified = null,
                DimKnownPersonId = -1,
                DimRegisteredDataSourceId = -1
            };
        }

        /*
         * Get empty DimProfileOnlyResearchActivity.
         * Must use -1 in required foreign keys.
         */
        public DimProfileOnlyResearchActivity GetEmptyDimProfileOnlyResearchActivity()
        {
            return new DimProfileOnlyResearchActivity()
            {
                DimDateIdStart = -1,
                DimDateIdEnd = -1,
                DimGeoIdCountry = null,
                DimOrganizationId = -1,
                DimEventId = -1,
                LocalIdentifier = "",
                OrcidWorkType = "",
                NameFi = "",
                NameSv = "",
                NameEn = "",
                NameUnd = "",
                DescriptionFi = "",
                DescriptionEn = "",
                DescriptionSv = "",
                IndentifierlessTargetOrg = "",
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = null,
                Modified = null,
                DimRegisteredDataSourceId = -1
            };
        }

        /*
         * Get empty DimPid.
         * Must use -1 in required foreign keys.
         */
        public DimPid GetEmptyDimPid()
        {
            return new DimPid()
            {
                PidContent = "",
                PidType = "",
                DimOrganizationId = -1,
                DimKnownPersonId = -1,
                DimPublicationId = -1,
                DimServiceId = -1,
                DimInfrastructureId = -1,
                DimPublicationChannelId = -1,
                DimResearchDatasetId = -1,
                DimFundingDecisionId = -1,
                DimResearchDataCatalogId = -1,
                DimResearchActivityId = -1,
                DimEventId = -1,
                DimProfileOnlyDatasetId = -1,
                DimProfileOnlyFundingDecisionId = -1,
                DimProfileOnlyPublicationId = -1,
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.GetCurrentDateTime(),
                Modified = _utilityService.GetCurrentDateTime()
            };
        }


        /*
         * Check if a DimName can be included in user profile.
         * Exclude DimName, which are already included in profile.
         * Exclude DimNames, whose registered data source is any of the following:
         * - virta
         * - metax
         * - sftp_funding
         */
        public bool CanIncludeDimNameInUserProfile(List<int> existingIDs, DimName dimName)
        {
            return
                !existingIDs.Contains(dimName.Id) &&
                !(
                    dimName.DimRegisteredDataSource.Name == "virta" ||
                    dimName.DimRegisteredDataSource.Name == "metax" ||
                    dimName.DimRegisteredDataSource.Name == "sftp_funding"
                );
        }


        /*
         * Search and add data from TTV database.
         * This is data that is already linked to the ORCID id in DimPid and it's related DimKnownPerson.
         * ProfileOnly* items must be excluded in these queries.
         */
        public async Task AddTtvDataToUserProfile(DimKnownPerson dimKnownPerson, DimUserProfile dimUserProfile, LogUserIdentification logUserIdentification)
        {
            _logger.LogError(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_ADD_TTV_DATA,
                    state: LogContent.ActionState.START));


            // Get FactFieldValues
            List<FactFieldValue> ffvs = await _ttvContext.FactFieldValues.Where(f => f.DimUserProfileId == dimUserProfile.Id).AsNoTracking().ToListAsync();
            // Collect lists of IDs, which are already included in the profile.
            // They are used in SQL where condition to filter out duplicates.
            List<int> existingEmailIds = new();
            List<int> existingWebLinkIds = new();
            List<int> existingTelephoneNumberIds = new();
            List<int> existingResearcherDescriptionIds = new();
            List<int> existingAffiliationIds = new();
            List<int> existingEducationIds = new();
            List<int> existingNameIds = new();
            List<int> existingPublicationIds = new();
            List<int> existingResearchActivityIds = new();
            List<int> existingResearchDatasetIds = new();
            List<int> existingFundingDecisionIds = new();
            if (ffvs != null)
            {
                existingEmailIds = ffvs.Where(ffv => ffv.DimEmailAddrressId != -1).Select(ffv => ffv.DimEmailAddrressId).Distinct().ToList<int>();
                existingWebLinkIds = ffvs.Where(ffv => ffv.DimWebLinkId != -1).Select(ffv => ffv.DimWebLinkId).Distinct().ToList<int>();
                existingTelephoneNumberIds = ffvs.Where(ffv => ffv.DimTelephoneNumberId != -1).Select(ffv => ffv.DimTelephoneNumberId).Distinct().ToList<int>();
                existingResearcherDescriptionIds = ffvs.Where(ffv => ffv.DimResearcherDescriptionId != -1).Select(ffv => ffv.DimResearcherDescriptionId).Distinct().ToList<int>();
                existingAffiliationIds = ffvs.Where(ffv => ffv.DimAffiliationId != -1).Select(ffv => ffv.DimAffiliationId).Distinct().ToList<int>();
                existingEducationIds = ffvs.Where(ffv => ffv.DimEducationId != -1).Select(ffv => ffv.DimEducationId).Distinct().ToList<int>();
                existingNameIds = ffvs.Where(ffv => ffv.DimNameId != -1).Select(ffv => ffv.DimNameId).Distinct().ToList<int>();
                existingPublicationIds = ffvs.Where(ffv => ffv.DimPublicationId != -1).Select(ffv => ffv.DimPublicationId).Distinct().ToList<int>();
                existingResearchActivityIds = ffvs.Where(ffv => ffv.DimResearchActivityId != -1).Select(ffv => ffv.DimResearchActivityId).Distinct().ToList<int>();
                existingResearchDatasetIds = ffvs.Where(ffv => ffv.DimResearchDatasetId != -1).Select(ffv => ffv.DimResearchDatasetId).Distinct().ToList<int>();
                existingFundingDecisionIds = ffvs.Where(ffv => ffv.DimFundingDecisionId != -1).Select(ffv => ffv.DimFundingDecisionId).Distinct().ToList<int>();
            }

            using (var connection = _ttvContext.Database.GetDbConnection())
            {                

                // email
                try
                {
                    string emailSql = _ttvSqlService.GetSqlQuery_Select_DimEmailAddrress(dimKnownPerson.Id, existingEmailIds);
                    List<DimTableMinimalDTO> emails = (await connection.QueryAsync<DimTableMinimalDTO>(emailSql)).ToList();
                    DimFieldDisplaySetting dimFieldDisplaySetting_emailAddress =
                        dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS).First();
                    foreach (DimTableMinimalDTO email in emails)
                    {
                        FactFieldValue factFieldValueEmailAddress = this.GetEmptyFactFieldValue();
                        factFieldValueEmailAddress.DimUserProfileId = dimUserProfile.Id;
                        factFieldValueEmailAddress.DimFieldDisplaySettingsId = dimFieldDisplaySetting_emailAddress.Id;
                        factFieldValueEmailAddress.DimEmailAddrressId = email.Id;
                        factFieldValueEmailAddress.DimRegisteredDataSourceId = email.DimRegisteredDataSourceId;
                        _ttvContext.FactFieldValues.Add(factFieldValueEmailAddress);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.PROFILE_ADD_TTV_DATA,
                            state: LogContent.ActionState.FAILED,
                            error: true,
                            message: $"email: {ex.ToString()}"));
                }

                // web link
                try
                {
                    string webLinkSql = _ttvSqlService.GetSqlQuery_Select_DimWebLink(dimKnownPerson.Id, existingWebLinkIds);
                    List<DimTableMinimalDTO> webLinks = (await connection.QueryAsync<DimTableMinimalDTO>(webLinkSql)).ToList();
                    DimFieldDisplaySetting dimFieldDisplaySetting_webLink =
                        dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_WEB_LINK).First();
                    foreach (DimTableMinimalDTO webLink in webLinks)
                    {
                        FactFieldValue factFieldValueWebLink = this.GetEmptyFactFieldValue();
                        factFieldValueWebLink.DimUserProfileId = dimUserProfile.Id;
                        factFieldValueWebLink.DimFieldDisplaySettingsId = dimFieldDisplaySetting_webLink.Id;
                        factFieldValueWebLink.DimWebLinkId = webLink.Id;
                        factFieldValueWebLink.DimRegisteredDataSourceId = webLink.DimRegisteredDataSourceId;
                        _ttvContext.FactFieldValues.Add(factFieldValueWebLink);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                      LogContent.MESSAGE_TEMPLATE,
                      logUserIdentification,
                      new LogApiInfo(
                          action: LogContent.Action.PROFILE_ADD_TTV_DATA,
                          state: LogContent.ActionState.FAILED,
                          error: true,
                          message: $"web link: {ex.ToString()}"));
                }

                // telephone number
                try
                {
                    string telephoneNumberSql = _ttvSqlService.GetSqlQuery_Select_DimTelephoneNumber(dimKnownPerson.Id, existingTelephoneNumberIds);
                    List<DimTableMinimalDTO> telephoneNumbers = (await connection.QueryAsync<DimTableMinimalDTO>(telephoneNumberSql)).ToList();
                    DimFieldDisplaySetting dimFieldDisplaySetting_telephoneNumber =
                        dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER).First();
                    foreach (DimTableMinimalDTO telephoneNumber in telephoneNumbers)
                    {
                        FactFieldValue factFieldValueTelephoneNumber = this.GetEmptyFactFieldValue();
                        factFieldValueTelephoneNumber.DimUserProfileId = dimUserProfile.Id;
                        factFieldValueTelephoneNumber.DimFieldDisplaySettingsId = dimFieldDisplaySetting_telephoneNumber.Id;
                        factFieldValueTelephoneNumber.DimTelephoneNumberId = telephoneNumber.Id;
                        factFieldValueTelephoneNumber.DimRegisteredDataSourceId = telephoneNumber.DimRegisteredDataSourceId;
                        _ttvContext.FactFieldValues.Add(factFieldValueTelephoneNumber);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.PROFILE_ADD_TTV_DATA,
                            state: LogContent.ActionState.FAILED,
                            error: true,
                            message: $"telephone number: {ex.ToString()}"));
                }

                // researcher description
                try
                {
                    string researcherDescriptionSql = _ttvSqlService.GetSqlQuery_Select_DimResearcherDescription(dimKnownPerson.Id, existingResearcherDescriptionIds);
                    List<DimTableMinimalDTO> researcherDescriptions = (await connection.QueryAsync<DimTableMinimalDTO>(researcherDescriptionSql)).ToList();
                    DimFieldDisplaySetting dimFieldDisplaySetting_researcherDescription =
                        dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION).First();
                    foreach (DimTableMinimalDTO researcherDescription in researcherDescriptions)
                    {
                        FactFieldValue factFieldValueResearcherDescription = this.GetEmptyFactFieldValue();
                        factFieldValueResearcherDescription.DimUserProfileId = dimUserProfile.Id;
                        factFieldValueResearcherDescription.DimFieldDisplaySettingsId = dimFieldDisplaySetting_researcherDescription.Id;
                        factFieldValueResearcherDescription.DimResearcherDescriptionId = researcherDescription.Id;
                        factFieldValueResearcherDescription.DimRegisteredDataSourceId = researcherDescription.DimRegisteredDataSourceId;
                        _ttvContext.FactFieldValues.Add(factFieldValueResearcherDescription);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.PROFILE_ADD_TTV_DATA,
                            state: LogContent.ActionState.FAILED,
                            error: true,
                            message: $"researcher description: {ex.ToString()}"));
                }

                // affiliation
                try
                {
                    string affiliationSql = _ttvSqlService.GetSqlQuery_Select_DimAffiliation(dimKnownPerson.Id, existingAffiliationIds);
                    List<DimTableMinimalDTO> affiliations = (await connection.QueryAsync<DimTableMinimalDTO>(affiliationSql)).ToList();
                    DimFieldDisplaySetting dimFieldDisplaySetting_affiliation =
                        dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_AFFILIATION).First();
                    foreach (DimTableMinimalDTO affiliation in affiliations)
                    {
                        FactFieldValue factFieldValueAffiliation = this.GetEmptyFactFieldValue();
                        factFieldValueAffiliation.DimUserProfileId = dimUserProfile.Id;
                        factFieldValueAffiliation.DimFieldDisplaySettingsId = dimFieldDisplaySetting_affiliation.Id;
                        factFieldValueAffiliation.DimAffiliationId = affiliation.Id;
                        factFieldValueAffiliation.DimRegisteredDataSourceId = affiliation.DimRegisteredDataSourceId;
                        _ttvContext.FactFieldValues.Add(factFieldValueAffiliation);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.PROFILE_ADD_TTV_DATA,
                            state: LogContent.ActionState.FAILED,
                            error: true,
                            message: $"affiliation: {ex.ToString()}"));
                }

                // education
                try
                {
                    string educationSql = _ttvSqlService.GetSqlQuery_Select_DimEducation(dimKnownPerson.Id, existingEducationIds);
                    List<DimTableMinimalDTO> educations = (await connection.QueryAsync<DimTableMinimalDTO>(educationSql)).ToList();
                    DimFieldDisplaySetting dimFieldDisplaySetting_education =
                        dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_EDUCATION).First();
                    foreach (DimTableMinimalDTO education in educations)
                    {
                        FactFieldValue factFieldValueEducation = this.GetEmptyFactFieldValue();
                        factFieldValueEducation.DimUserProfileId = dimUserProfile.Id;
                        factFieldValueEducation.DimFieldDisplaySettingsId = dimFieldDisplaySetting_education.Id;
                        factFieldValueEducation.DimEducationId = education.Id;
                        factFieldValueEducation.DimRegisteredDataSourceId = education.DimRegisteredDataSourceId;
                        _ttvContext.FactFieldValues.Add(factFieldValueEducation);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.PROFILE_ADD_TTV_DATA,
                            state: LogContent.ActionState.FAILED,
                            error: true,
                            message: $"education: {ex.ToString()}"));
                }



                DimFieldDisplaySetting dimFieldDisplaySetting_name =
                    dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_NAME).First();
                DimFieldDisplaySetting dimFieldDisplaySetting_otherNames =
                    dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_OTHER_NAMES).First();
                DimFieldDisplaySetting dimFieldDisplaySetting_publication =
                    dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION).First();
                DimFieldDisplaySetting dimFieldDisplaySetting_fundingDecision =
                    dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION).First();
                DimFieldDisplaySetting dimFieldDisplaySetting_researchActivity =
                    dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_RESEARCH_ACTIVITY).First();
                DimFieldDisplaySetting dimFieldDisplaySetting_researchDataset =
                    dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET).First();

                // Loop DimNames, which have valid registered data source
                foreach (DimName dimName in dimKnownPerson.DimNames.Where(dimName => dimName.DimRegisteredDataSourceId != -1))
                {
                    // Name
                    // Exclude DimNames which are already in user profile or have a specific registered data source (see CanIncludeDimNameInUserProfile)
                    if (CanIncludeDimNameInUserProfile(existingNameIds, dimName))
                    {
                        if (!String.IsNullOrWhiteSpace(dimName.FirstNames) && !String.IsNullOrWhiteSpace(dimName.LastName))
                        {
                            // name: first_names & last_name
                            FactFieldValue factFieldValueName = this.GetEmptyFactFieldValue();
                            factFieldValueName.DimUserProfileId = dimUserProfile.Id;
                            factFieldValueName.DimFieldDisplaySettingsId = dimFieldDisplaySetting_name.Id;
                            factFieldValueName.DimNameId = dimName.Id;
                            factFieldValueName.DimRegisteredDataSourceId = dimName.DimRegisteredDataSourceId;
                            _ttvContext.FactFieldValues.Add(factFieldValueName);
                        }
                        else if (!String.IsNullOrWhiteSpace(dimName.FullName))
                        {
                            // other name: full_name
                            FactFieldValue factFieldValueOtherNames = this.GetEmptyFactFieldValue();
                            factFieldValueOtherNames.DimUserProfileId = dimUserProfile.Id;
                            factFieldValueOtherNames.DimFieldDisplaySettingsId = dimFieldDisplaySetting_otherNames.Id;
                            factFieldValueOtherNames.DimNameId = dimName.Id;
                            factFieldValueOtherNames.DimRegisteredDataSourceId = dimName.DimRegisteredDataSourceId;
                            _ttvContext.FactFieldValues.Add(factFieldValueOtherNames);
                        }
                    }

                    // fact_contribution
                    try
                    {
                        string factContributionSql = _ttvSqlService.GetSqlQuery_Select_FactContribution(dimName.Id);
                        List<FactContributionTableMinimalDTO> factContributions = (await connection.QueryAsync<FactContributionTableMinimalDTO>(factContributionSql)).ToList();

                        // Loop FactContributions related to a DimName. Add entries to user profile. Exclude already existing IDs.
                        foreach (FactContributionTableMinimalDTO fc in factContributions)
                        {
                            // publication
                            if (fc.DimPublicationId != -1 && !existingPublicationIds.Contains(fc.DimPublicationId))
                            {
                                FactFieldValue factFieldValuePublication = this.GetEmptyFactFieldValue();
                                factFieldValuePublication.DimUserProfileId = dimUserProfile.Id;
                                factFieldValuePublication.DimFieldDisplaySettingsId = dimFieldDisplaySetting_publication.Id;
                                factFieldValuePublication.DimPublicationId = fc.DimPublicationId;
                                factFieldValuePublication.DimRegisteredDataSourceId = dimName.DimRegisteredDataSourceId;
                                _ttvContext.FactFieldValues.Add(factFieldValuePublication);
                            }

                            // research activity
                            if (fc.DimResearchActivityId != -1 && !existingResearchActivityIds.Contains(fc.DimResearchActivityId))
                            {
                                FactFieldValue factFieldValueResearchActivity = this.GetEmptyFactFieldValue();
                                factFieldValueResearchActivity.DimUserProfileId = dimUserProfile.Id;
                                factFieldValueResearchActivity.DimFieldDisplaySettingsId = dimFieldDisplaySetting_researchActivity.Id;
                                factFieldValueResearchActivity.DimResearchActivityId = fc.DimResearchActivityId;
                                factFieldValueResearchActivity.DimRegisteredDataSourceId = dimName.DimRegisteredDataSourceId;
                                _ttvContext.FactFieldValues.Add(factFieldValueResearchActivity);
                            }

                            // research dataset
                            if (fc.DimResearchDatasetId != -1 && !existingResearchDatasetIds.Contains(fc.DimResearchDatasetId))
                            {
                                FactFieldValue factFieldValueResearchDataset = this.GetEmptyFactFieldValue();
                                factFieldValueResearchDataset.DimUserProfileId = dimUserProfile.Id;
                                factFieldValueResearchDataset.DimFieldDisplaySettingsId = dimFieldDisplaySetting_researchDataset.Id;
                                factFieldValueResearchDataset.DimResearchDatasetId = fc.DimResearchDatasetId;
                                factFieldValueResearchDataset.DimRegisteredDataSourceId = dimName.DimRegisteredDataSourceId;
                                _ttvContext.FactFieldValues.Add(factFieldValueResearchDataset);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            LogContent.MESSAGE_TEMPLATE,
                            logUserIdentification,
                            new LogApiInfo(
                                action: LogContent.Action.PROFILE_ADD_TTV_DATA,
                                state: LogContent.ActionState.FAILED,
                                error: true,
                                message: $"fact_contribution: {ex.ToString()}"));
                    }

                    // Funding decisions via br_participates_in_funding_group
                    // fact_contribution
                    try
                    {
                        string brParticipatesInFundingGroupSql = _ttvSqlService.GetSqlQuery_Select_BrParticipatesInFundingGroup(dimName.Id, existingFundingDecisionIds);
                        List<int> fundingDecisionIds = (await connection.QueryAsync<int>(brParticipatesInFundingGroupSql)).ToList();
                        foreach (int fundingDecisionId in fundingDecisionIds)
                        {
                            FactFieldValue factFieldValueFundingDecision = this.GetEmptyFactFieldValue();
                            factFieldValueFundingDecision.DimUserProfileId = dimUserProfile.Id;
                            factFieldValueFundingDecision.DimFieldDisplaySettingsId = dimFieldDisplaySetting_fundingDecision.Id;
                            factFieldValueFundingDecision.DimFundingDecisionId = fundingDecisionId;
                            factFieldValueFundingDecision.DimRegisteredDataSourceId = dimName.DimRegisteredDataSourceId;
                            _ttvContext.FactFieldValues.Add(factFieldValueFundingDecision);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            LogContent.MESSAGE_TEMPLATE,
                            logUserIdentification,
                            new LogApiInfo(
                                action: LogContent.Action.PROFILE_ADD_TTV_DATA,
                                state: LogContent.ActionState.FAILED,
                                error: true,
                                message: $"br_participates_in_funding_group: {ex.ToString()}"));
                    }
                }

                try
                {
                    await _ttvContext.SaveChangesAsync();

                    _logger.LogError(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.PROFILE_ADD_TTV_DATA,
                            state: LogContent.ActionState.COMPLETE));
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                            new LogApiInfo(
                                action: LogContent.Action.PROFILE_ADD_TTV_DATA,
                                state: LogContent.ActionState.FAILED,
                                error: true,
                                message: $"Add TTV data save changes: {ex.ToString()}"));
                }

            }
        }



        /*
         * Create user profile.
         * The following entities will be created in the database:
         *   - DimPid
         *   - DimKnownPerson
         *   - DimUserProfile
         *   - DimFieldDisplaySettings
         *   - FactFieldValues
         *   - BrGrantedPermissions
         */
        public async Task CreateProfile(string orcidId, LogUserIdentification logUserIdentification)
        {
            // Get DimPid by ORCID ID.
            DimPid dimPid = await _ttvContext.DimPids
                // FactContribution
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimNames)
                        .ThenInclude(dn => dn.FactContributions).AsNoTracking()
                // DimName
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimNames)
                        .ThenInclude(dn => dn.DimRegisteredDataSource).AsNoTracking()
                // DimUserProfile
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimUserProfiles)
                        .ThenInclude(dup => dup.DimFieldDisplaySettings).AsNoTracking()
                .FirstOrDefaultAsync(dimPid => dimPid.PidContent == orcidId && dimPid.PidType == Constants.PidTypes.ORCID);

            // Get current DateTime
            DateTime currentDateTime = _utilityService.GetCurrentDateTime();

            // DimPid and DimKnownPerson
            if (dimPid == null)
            {
                // DimPid was not found, add new.
                dimPid = GetEmptyDimPid();
                dimPid.PidContent = orcidId;
                dimPid.PidType = Constants.PidTypes.ORCID;
                dimPid.SourceId = Constants.SourceIdentifiers.PROFILE_API;

                // Since new DimPid is added, then new DimKnownPerson must be added.
                dimPid.DimKnownPerson = GetNewDimKnownPerson(orcidId, currentDateTime);
                _ttvContext.DimPids.Add(dimPid);
            }
            else if (dimPid.DimKnownPerson == null || dimPid.DimKnownPersonId == -1)
            {
                // DimPid was found but it does not have related DimKnownPerson, add new.
                DimKnownPerson kp = GetNewDimKnownPerson(orcidId, currentDateTime);
                _ttvContext.DimKnownPeople.Add(kp);
                dimPid.DimKnownPerson = kp;
            }

            // Save DimPid and DimKnownPerson changes.
            await _ttvContext.SaveChangesAsync();

            // DimUserProfile
            DimUserProfile dimUserProfile = dimPid.DimKnownPerson.DimUserProfiles.FirstOrDefault();
            if (dimUserProfile == null)
            {
                // DimUserProfile was not found, add new.
                dimUserProfile = new DimUserProfile()
                {
                    DimKnownPersonId = dimPid.DimKnownPerson.Id,
                    OrcidId = orcidId,
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = currentDateTime,
                    AllowAllSubscriptions = false
                };
                _ttvContext.DimUserProfiles.Add(dimUserProfile);
            }

            // DimFieldDisplaySettings
            if (dimUserProfile.DimFieldDisplaySettings.Count == 0)
            {
                // DimUserProfile does not have DimFieldDisplaySettings, add new.
                foreach (int fieldIdentifier in GetFieldIdentifiers())
                {
                    DimFieldDisplaySetting dimFieldDisplaySetting = new()
                    {
                        FieldIdentifier = fieldIdentifier,
                        DimUserProfile = dimUserProfile,
                        Show = false,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySetting);
                }
            }

            // BrGrantedPermissions - Add default sharing permissions
            _ttvContext.BrGrantedPermissions.AddRange(
                await _sharingService.GetDefaultSharingPermissionsListForUserProfile(dimUserProfile)
            );

            // Save DimUserProfile, DimFieldDisplaySettings and BrGrantedPermissions changes.
            await _ttvContext.SaveChangesAsync();

            // Search TTV database and add related entries into user profile.
            await AddTtvDataToUserProfile(
                dimKnownPerson: dimPid.DimKnownPerson,
                dimUserProfile: dimUserProfile,
                logUserIdentification: logUserIdentification);

            // Save FactFieldValues changes.
            await _ttvContext.SaveChangesAsync();
        }



        /*
         *  Get profile data. New version using data structure,
         *  where each item contains a list of data sources.
         */
        public async Task<ProfileEditorDataResponse> GetProfileDataAsync(int userprofileId, LogUserIdentification logUserIdentification, bool forElasticsearch = false)
        {
            // Response data
            ProfileEditorDataResponse profileDataResponse = new() { };

            // Get SQL statement for profile data query
            string profileDataSql = _ttvSqlService.GetSqlQuery_ProfileData(userprofileId, forElasticsearch);

            // Execute SQL statement using Dapper
            var connection = _ttvContext.Database.GetDbConnection();
            List<ProfileDataFromSql> profileDataList = (await connection.QueryAsync<ProfileDataFromSql>(profileDataSql)).ToList();

            // Helper list, which is used in collecting list of unique data sources and detecting whether the item is already in the list.
            List<int> uniqueDataSourceIds = new();

            foreach (ProfileDataFromSql p in profileDataList)
            {
                // Organization name translation
                NameTranslation nameTranslationSourceOrganization = _languageService.GetNameTranslation(
                    nameFi: p.DimRegisteredDataSource_DimOrganization_NameFi,
                    nameEn: p.DimRegisteredDataSource_DimOrganization_NameEn,
                    nameSv: p.DimRegisteredDataSource_DimOrganization_NameSv
                );

                // Source object containing registered data source and organization name.
                ProfileEditorSource profileEditorSource = new()
                {
                    Id = p.DimRegisteredDataSource_Id,
                    RegisteredDataSource = p.DimRegisteredDataSource_Name,
                    Organization = new Organization()
                    {
                        NameFi = nameTranslationSourceOrganization.NameFi,
                        NameEn = nameTranslationSourceOrganization.NameEn,
                        NameSv = nameTranslationSourceOrganization.NameSv,
                        SectorId = p.DimRegisteredDataSource_DimOrganization_DimSector_SectorId
                    }
                };

                // Add data source into list of unique data sources.
                if (!uniqueDataSourceIds.Contains(profileEditorSource.Id))
                {
                    profileDataResponse.uniqueDataSources.Add(profileEditorSource);
                    uniqueDataSourceIds.Add(profileEditorSource.Id);
                }

                switch (p.DimFieldDisplaySettings_FieldIdentifier)
                {
                    // Name
                    case Constants.FieldIdentifiers.PERSON_NAME:
                        profileDataResponse.personal.names.Add(
                            new ProfileEditorName()
                            {
                                FirstNames = p.DimName_FirstNames,
                                LastName = p.DimName_LastName,
                                itemMeta = new ProfileEditorItemMeta()
                                {
                                    Id = p.FactFieldValues_DimNameId,
                                    Type = Constants.ItemMetaTypes.PERSON_NAME,
                                    Show = p.FactFieldValues_Show,
                                    PrimaryValue = p.FactFieldValues_PrimaryValue
                                },
                                DataSources = new List<ProfileEditorSource> { profileEditorSource }
                            }
                        );
                        break;

                    // Other name
                    case Constants.FieldIdentifiers.PERSON_OTHER_NAMES:
                        profileDataResponse.personal.otherNames.Add(
                            new ProfileEditorName()
                            {
                                FullName = p.DimName_FullName,
                                itemMeta = new ProfileEditorItemMeta()
                                {
                                    Id = p.FactFieldValues_DimNameId,
                                    Type = Constants.ItemMetaTypes.PERSON_OTHER_NAMES,
                                    Show = p.FactFieldValues_Show,
                                    PrimaryValue = p.FactFieldValues_PrimaryValue
                                },
                                DataSources = new List<ProfileEditorSource> { profileEditorSource }
                            }
                        );
                        break;

                    // Web link
                    case Constants.FieldIdentifiers.PERSON_WEB_LINK:
                        profileDataResponse.personal.webLinks.Add(
                            new ProfileEditorWebLink()
                            {
                                Url = p.DimWebLink_Url,
                                LinkLabel = p.DimWebLink_LinkLabel,
                                itemMeta = new ProfileEditorItemMeta()
                                {
                                    Id = p.FactFieldValues_DimWebLinkId,
                                    Type = Constants.ItemMetaTypes.PERSON_WEB_LINK,
                                    Show = p.FactFieldValues_Show,
                                    PrimaryValue = p.FactFieldValues_PrimaryValue
                                },
                                DataSources = new List<ProfileEditorSource> { profileEditorSource }
                            }
                        );
                        break;

                    // Researcher description
                    case Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION:
                        // Researcher description name translation
                        NameTranslation nameTranslationResearcherDescription = _languageService.GetNameTranslation(
                            nameFi: p.DimResearcherDescription_ResearchDescriptionFi,
                            nameEn: p.DimResearcherDescription_ResearchDescriptionEn,
                            nameSv: p.DimResearcherDescription_ResearchDescriptionSv
                        );
                        profileDataResponse.personal.researcherDescriptions.Add(
                            new ProfileEditorResearcherDescription()
                            {
                                ResearchDescriptionFi = nameTranslationResearcherDescription.NameFi,
                                ResearchDescriptionEn = nameTranslationResearcherDescription.NameEn,
                                ResearchDescriptionSv = nameTranslationResearcherDescription.NameSv,
                                itemMeta = new ProfileEditorItemMeta()
                                {
                                    Id = p.FactFieldValues_DimResearcherDescriptionId,
                                    Type = Constants.ItemMetaTypes.PERSON_RESEARCHER_DESCRIPTION,
                                    Show = p.FactFieldValues_Show,
                                    PrimaryValue = p.FactFieldValues_PrimaryValue
                                },
                                DataSources = new List<ProfileEditorSource> { profileEditorSource }
                            }
                        );
                        break;

                    // Email
                    case Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS:
                        profileDataResponse.personal.emails.Add(
                            new ProfileEditorEmail()
                            {
                                Value = p.DimEmailAddrress_Email,
                                itemMeta = new ProfileEditorItemMeta()
                                {
                                    Id = p.FactFieldValues_DimEmailAddrressId,
                                    Type = Constants.ItemMetaTypes.PERSON_EMAIL_ADDRESS,
                                    Show = p.FactFieldValues_Show,
                                    PrimaryValue = p.FactFieldValues_PrimaryValue
                                },
                                DataSources = new List<ProfileEditorSource> { profileEditorSource }
                            }
                        );
                        break;

                    // Telephone number
                    case Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER:
                        profileDataResponse.personal.telephoneNumbers.Add(
                            new ProfileEditorTelephoneNumber()
                            {
                                Value = p.DimTelephoneNumber_TelephoneNumber,
                                itemMeta = new ProfileEditorItemMeta()
                                {
                                    Id = p.FactFieldValues_DimTelephoneNumberId,
                                    Type = Constants.ItemMetaTypes.PERSON_TELEPHONE_NUMBER,
                                    Show = p.FactFieldValues_Show,
                                    PrimaryValue = p.FactFieldValues_PrimaryValue
                                },
                                DataSources = new List<ProfileEditorSource> { profileEditorSource }
                            }
                        );
                        break;

                    // Field of science
                    /*
                    case Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE:
                        // Field of science name translation
                        NameTranslation nameTranslationFieldOfScience = _languageService.GetNameTranslation(
                            nameFi: p.DimFieldOfScience_NameFi,
                            nameEn: p.DimFieldOfScience_NameEn,
                            nameSv: p.DimFieldOfScience_NameSv
                        );

                        profileDataResponse.personal.fieldOfSciences.Add(
                            new ProfileEditorFieldOfScience()
                            {
                                NameFi = nameTranslationFieldOfScience.NameFi,
                                NameEn = nameTranslationFieldOfScience.NameEn,
                                NameSv = nameTranslationFieldOfScience.NameSv,
                                itemMeta = new ProfileEditorItemMeta()
                                {
                                    Id = p.FactFieldValues_DimFieldOfScienceId,
                                    Type = Constants.ItemMetaTypes.PERSON_FIELD_OF_SCIENCE,
                                    Show = p.FactFieldValues_Show,
                                    PrimaryValue = p.FactFieldValues_PrimaryValue
                                },
                                DataSources = new List<ProfileEditorSource> { profileEditorSource }
                            }
                        );
                        break;
                    */

                    // Keyword
                    case Constants.FieldIdentifiers.PERSON_KEYWORD:
                        profileDataResponse.personal.keywords.Add(
                            new ProfileEditorKeyword()
                            {
                                Value = p.DimKeyword_Keyword,
                                itemMeta = new ProfileEditorItemMeta()
                                {
                                    Id = p.FactFieldValues_DimKeywordId,
                                    Type = Constants.ItemMetaTypes.PERSON_KEYWORD,
                                    Show = p.FactFieldValues_Show,
                                    PrimaryValue = p.FactFieldValues_PrimaryValue
                                },
                                DataSources = new List<ProfileEditorSource> { profileEditorSource }
                            }
                        );
                        break;

                    // External identifier
                    case Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER:
                        profileDataResponse.personal.externalIdentifiers.Add(
                            new ProfileEditorExternalIdentifier()
                            {
                                PidContent = p.DimPid_PidContent,
                                PidType = p.DimPid_PidType,
                                itemMeta = new ProfileEditorItemMeta()
                                {
                                    Id = p.FactFieldValues_DimPidId,
                                    Type = Constants.ItemMetaTypes.PERSON_EXTERNAL_IDENTIFIER,
                                    Show = p.FactFieldValues_Show,
                                    PrimaryValue = p.FactFieldValues_PrimaryValue
                                },
                                DataSources = new List<ProfileEditorSource> { profileEditorSource }
                            }
                        );
                        break;

                    // Affiliation
                    case Constants.FieldIdentifiers.ACTIVITY_AFFILIATION:
                        // Affiliation organization search order:
                        // 1. DimAffiliation_DimOrganizationBroader_Id
                        // 2. DimAffiliation_DimOrganization_Id
                        // 3. DimIdentifierlessData
                        // 

                        // Name translation service ensures that none of the language fields is empty.
                        NameTranslation nameTranslationAffiliationOrganization = new();
                        NameTranslation nameTranslationAffiliationOrganizationSector = new();
                        NameTranslation nameTranslationAffiliationDepartment = new();

                        // Organization name
                        if (p.DimAffiliation_DimOrganizationBroader_Id > 0)
                        {
                            nameTranslationAffiliationOrganization = _languageService.GetNameTranslation(
                                nameFi: p.DimAffiliation_DimOrganizationBroader_NameFi,
                                nameEn: p.DimAffiliation_DimOrganizationBroader_NameEn,
                                nameSv: p.DimAffiliation_DimOrganizationBroader_NameSv
                            );

                            nameTranslationAffiliationOrganizationSector = _languageService.GetNameTranslation(
                                nameFi: p.DimAffiliation_DimOrganizationBroader_DimSector_NameFi,
                                nameEn: p.DimAffiliation_DimOrganizationBroader_DimSector_NameEn,
                                nameSv: p.DimAffiliation_DimOrganizationBroader_DimSector_NameSv
                            );
                        }
                        else if (p.DimAffiliation_DimOrganization_Id > 0)
                        {
                            nameTranslationAffiliationOrganization = _languageService.GetNameTranslation(
                                nameFi: p.DimAffiliation_DimOrganization_NameFi,
                                nameEn: p.DimAffiliation_DimOrganization_NameEn,
                                nameSv: p.DimAffiliation_DimOrganization_NameSv
                            );

                            nameTranslationAffiliationOrganizationSector = _languageService.GetNameTranslation(
                                nameFi: p.DimAffiliation_DimOrganization_DimSector_NameFi,
                                nameEn: p.DimAffiliation_DimOrganization_DimSector_NameEn,
                                nameSv: p.DimAffiliation_DimOrganization_DimSector_NameSv
                            );
                        }
                        else if (p.FactFieldValues_DimIdentifierlessDataId > -1 &&
                            p.DimIdentifierlessData_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                        {
                            nameTranslationAffiliationOrganization = _languageService.GetNameTranslation(
                                nameFi: p.DimIdentifierlessData_ValueFi,
                                nameEn: p.DimIdentifierlessData_ValueEn,
                                nameSv: p.DimIdentifierlessData_ValueSv
                            );
                        }

                        // Department name
                        if (p.DimAffiliation_DimOrganizationBroader_Id > 0)
                        {
                            // When DimOrganizationBroader is available, it contains the organization name and DimOrganization contains department name.
                            nameTranslationAffiliationDepartment = _languageService.GetNameTranslation(
                                nameFi: p.DimAffiliation_DimOrganization_NameFi,
                                nameEn: p.DimAffiliation_DimOrganization_NameEn,
                                nameSv: p.DimAffiliation_DimOrganization_NameSv
                            );
                        }
                        else if (p.DimIdentifierlessData_Type != null && p.DimIdentifierlessData_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                        {
                            nameTranslationAffiliationDepartment = _languageService.GetNameTranslation(
                                nameFi: p.DimIdentifierlessData_ValueFi,
                                nameEn: p.DimIdentifierlessData_ValueEn,
                                nameSv: p.DimIdentifierlessData_ValueSv
                            );
                        }
                        else if (p.DimIdentifierlessData_Child_Type != null && p.DimIdentifierlessData_Child_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                        {
                            nameTranslationAffiliationDepartment = _languageService.GetNameTranslation(
                                nameFi: p.DimIdentifierlessData_Child_ValueFi,
                                nameEn: p.DimIdentifierlessData_Child_ValueEn,
                                nameSv: p.DimIdentifierlessData_Child_ValueSv
                            );
                        }

                        // Name translation for position name
                        NameTranslation nameTranslationPositionName = _languageService.GetNameTranslation(
                            nameFi: p.DimAffiliation_PositionNameFi,
                            nameEn: p.DimAffiliation_PositionNameEn,
                            nameSv: p.DimAffiliation_PositionNameSv
                        );

                        ProfileEditorAffiliation affiliation = new()
                        {
                            OrganizationNameFi = nameTranslationAffiliationOrganization.NameFi,
                            OrganizationNameEn = nameTranslationAffiliationOrganization.NameEn,
                            OrganizationNameSv = nameTranslationAffiliationOrganization.NameSv,
                            DepartmentNameFi = nameTranslationAffiliationDepartment.NameFi,
                            DepartmentNameEn = nameTranslationAffiliationDepartment.NameSv,
                            DepartmentNameSv = nameTranslationAffiliationDepartment.NameEn,
                            PositionNameFi = nameTranslationPositionName.NameFi,
                            PositionNameEn = nameTranslationPositionName.NameEn,
                            PositionNameSv = nameTranslationPositionName.NameSv,
                            Type = p.DimAffiliation_DimReferenceData_NameFi,
                            StartDate = new ProfileEditorDate()
                            {
                                Year = p.DimAffiliation_StartDate_Year,
                                Month = p.DimAffiliation_StartDate_Month,
                                Day = p.DimAffiliation_StartDate_Day
                            },
                            EndDate = new ProfileEditorDate()
                            {
                                Year = p.DimAffiliation_EndDate_Year,
                                Month = p.DimAffiliation_EndDate_Month,
                                Day = p.DimAffiliation_EndDate_Day
                            },
                            itemMeta = new ProfileEditorItemMeta()
                            {
                                Id = p.FactFieldValues_DimAffiliationId,
                                Type = Constants.ItemMetaTypes.ACTIVITY_AFFILIATION,
                                Show = p.FactFieldValues_Show,
                                PrimaryValue = p.FactFieldValues_PrimaryValue
                            },
                            DataSources = new List<ProfileEditorSource> { profileEditorSource }
                        };

                        // Add Elasticsearch person index related data.
                        if (forElasticsearch && !String.IsNullOrWhiteSpace(p.DimAffiliation_DimOrganization_DimSector_SectorId))
                        {
                            affiliation.sector = new List<ProfileEditorSector>
                            {
                                new ProfileEditorSector()
                                {
                                    sectorId = p.DimAffiliation_DimOrganization_DimSector_SectorId,
                                    nameFiSector = nameTranslationAffiliationOrganizationSector.NameFi,
                                    nameEnSector = nameTranslationAffiliationOrganizationSector.NameEn,
                                    nameSvSector = nameTranslationAffiliationOrganizationSector.NameSv,
                                    organization = new List<ProfileEditorSectorOrganization>() {
                                        new ProfileEditorSectorOrganization()
                                        {
                                            organizationId = p.DimAffiliation_DimOrganization_OrganizationId,
                                            OrganizationNameFi = nameTranslationAffiliationOrganization.NameFi,
                                            OrganizationNameEn = nameTranslationAffiliationOrganization.NameEn,
                                            OrganizationNameSv = nameTranslationAffiliationOrganization.NameSv
                                        }
                                    }
                                }
                            };
                        }
                        profileDataResponse.activity.affiliations.Add(affiliation);
                        break;

                    // Education
                    case Constants.FieldIdentifiers.ACTIVITY_EDUCATION:
                        // Name translation service ensures that none of the language fields is empty.
                        NameTranslation nameTraslationEducation = _languageService.GetNameTranslation(
                            nameFi: p.DimEducation_NameFi,
                            nameEn: p.DimEducation_NameEn,
                            nameSv: p.DimEducation_NameSv
                        );

                        profileDataResponse.activity.educations.Add(
                            new()
                            {
                                NameFi = nameTraslationEducation.NameFi,
                                NameEn = nameTraslationEducation.NameEn,
                                NameSv = nameTraslationEducation.NameSv,
                                DegreeGrantingInstitutionName = p.DimEducation_DegreeGrantingInstitutionName,
                                StartDate = new ProfileEditorDate()
                                {
                                    Year = p.DimEducation_StartDate_Year,
                                    Month = p.DimEducation_StartDate_Month,
                                    Day = p.DimEducation_StartDate_Day
                                },
                                EndDate = new ProfileEditorDate()
                                {
                                    Year = p.DimEducation_EndDate_Year,
                                    Month = p.DimEducation_EndDate_Month,
                                    Day = p.DimEducation_EndDate_Day
                                },
                                itemMeta = new ProfileEditorItemMeta()
                                {
                                    Id = p.FactFieldValues_DimEducationId,
                                    Type = Constants.ItemMetaTypes.ACTIVITY_EDUCATION,
                                    Show = p.FactFieldValues_Show,
                                    PrimaryValue = p.FactFieldValues_PrimaryValue
                                },
                                DataSources = new List<ProfileEditorSource> { profileEditorSource }
                            }
                        );
                        break;

                    // Publication
                    case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION:
                        profileDataResponse.activity.publications =
                            _duplicateHandlerService.AddPublicationToProfileEditorData(
                                dataSource: profileEditorSource,
                                profileData: p,
                                publications: profileDataResponse.activity.publications
                            );
                        break;

                    // Publication (ORCID)
                    case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY:
                        profileDataResponse.activity.publications =
                            _duplicateHandlerService.AddPublicationToProfileEditorData(
                                dataSource: profileEditorSource,
                                profileData: p,
                                publications: profileDataResponse.activity.publications
                            );
                        break;

                    // Research activity
                    case Constants.FieldIdentifiers.ACTIVITY_RESEARCH_ACTIVITY:
                        // Research activity can be stored in either DimResearchActivity or DimProfileOnlyResearchActivity

                        // DimResearchActivity
                        if (p.FactFieldValues_DimResearchActivityId != -1)
                        {
                            // Research activity organization search order:
                            // 1. DimResearchActivity_DimOrganizationBroader_Id
                            // 2. DimResearchActivity_DimOrganization_Id
                            // 3. DimIdentifierlessData

                            // Name translation service ensures that none of the language fields is empty.
                            NameTranslation nameTranslationResearchActivityOrganization = new();
                            NameTranslation nameTranslationResearchActivityOrganizationSector = new();
                            NameTranslation nameTranslationResearchActivityDepartment = new();

                            // Organization name
                            if (p.DimResearchActivity_DimOrganizationBroader_Id > 0)
                            {
                                nameTranslationResearchActivityOrganization = _languageService.GetNameTranslation(
                                    nameFi: p.DimResearchActivity_DimOrganizationBroader_NameFi,
                                    nameEn: p.DimResearchActivity_DimOrganizationBroader_NameEn,
                                    nameSv: p.DimResearchActivity_DimOrganizationBroader_NameSv
                                );

                                nameTranslationResearchActivityOrganizationSector = _languageService.GetNameTranslation(
                                    nameFi: p.DimResearchActivity_DimOrganizationBroader_DimSector_NameFi,
                                    nameEn: p.DimResearchActivity_DimOrganizationBroader_DimSector_NameEn,
                                    nameSv: p.DimResearchActivity_DimOrganizationBroader_DimSector_NameSv
                                );
                            }
                            else if (p.DimResearchActivity_DimOrganization_Id > 0)
                            {
                                nameTranslationResearchActivityOrganization = _languageService.GetNameTranslation(
                                    nameFi: p.DimResearchActivity_DimOrganization_NameFi,
                                    nameEn: p.DimResearchActivity_DimOrganization_NameEn,
                                    nameSv: p.DimResearchActivity_DimOrganization_NameSv
                                );

                                nameTranslationResearchActivityOrganizationSector = _languageService.GetNameTranslation(
                                    nameFi: p.DimResearchActivity_DimOrganization_DimSector_NameFi,
                                    nameEn: p.DimResearchActivity_DimOrganization_DimSector_NameEn,
                                    nameSv: p.DimResearchActivity_DimOrganization_DimSector_NameSv
                                );
                            }
                            else if (p.FactFieldValues_DimIdentifierlessDataId > -1 &&
                                p.DimIdentifierlessData_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                            {
                                nameTranslationResearchActivityOrganization = _languageService.GetNameTranslation(
                                    nameFi: p.DimIdentifierlessData_ValueFi,
                                    nameEn: p.DimIdentifierlessData_ValueEn,
                                    nameSv: p.DimIdentifierlessData_ValueSv
                                );
                            }

                            // Department name
                            if (p.DimResearchActivity_DimOrganizationBroader_Id > 0)
                            {
                                // When DimOrganizationBroader is available, it contains the organization name and DimOrganization contains department name.
                                nameTranslationResearchActivityDepartment = _languageService.GetNameTranslation(
                                    nameFi: p.DimResearchActivity_DimOrganization_NameFi,
                                    nameEn: p.DimResearchActivity_DimOrganization_NameEn,
                                    nameSv: p.DimResearchActivity_DimOrganization_NameSv
                                );
                            }
                            else if (p.DimIdentifierlessData_Type != null && p.DimIdentifierlessData_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                            {
                                nameTranslationResearchActivityDepartment = _languageService.GetNameTranslation(
                                    nameFi: p.DimIdentifierlessData_ValueFi,
                                    nameEn: p.DimIdentifierlessData_ValueEn,
                                    nameSv: p.DimIdentifierlessData_ValueSv
                                );
                            }
                            else if (p.DimIdentifierlessData_Child_Type != null && p.DimIdentifierlessData_Child_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                            {
                                nameTranslationResearchActivityDepartment = _languageService.GetNameTranslation(
                                    nameFi: p.DimIdentifierlessData_Child_ValueFi,
                                    nameEn: p.DimIdentifierlessData_Child_ValueEn,
                                    nameSv: p.DimIdentifierlessData_Child_ValueSv
                                );
                            }



                            NameTranslation nameTraslationResearchActivityName = _languageService.GetNameTranslation(
                                nameFi: p.DimResearchActivity_NameFi,
                                nameEn: p.DimResearchActivity_NameEn,
                                nameSv: p.DimResearchActivity_NameSv
                            );
                            NameTranslation nameTraslationResearchActivityDescription = _languageService.GetNameTranslation(
                                nameFi: p.DimResearchActivity_DescriptionFi,
                                nameEn: p.DimResearchActivity_DescriptionEn,
                                nameSv: p.DimResearchActivity_DescriptionSv
                            );
                            NameTranslation nameTraslationResearchActivityTypeName = _languageService.GetNameTranslation(
                                nameFi: p.DimResearchActivity_ActivityType_NameFi,
                                nameEn: p.DimResearchActivity_ActivityType_NameEn,
                                nameSv: p.DimResearchActivity_ActivityType_NameSv
                            );
                            NameTranslation nameTraslationResearchActivityRoleName = _languageService.GetNameTranslation(
                                nameFi: p.DimResearchActivity_Role_NameFi,
                                nameEn: p.DimResearchActivity_Role_NameEn,
                                nameSv: p.DimResearchActivity_Role_NameSv
                            );

                            ProfileEditorActivityAndReward activityAndReward = new()
                            {
                                NameFi = nameTraslationResearchActivityName.NameFi,
                                NameEn = nameTraslationResearchActivityName.NameEn,
                                NameSv = nameTraslationResearchActivityName.NameSv,
                                DescriptionFi = nameTraslationResearchActivityDescription.NameFi,
                                DescriptionEn = nameTraslationResearchActivityDescription.NameEn,
                                DescriptionSv = nameTraslationResearchActivityDescription.NameSv,
                                InternationalCollaboration = p.DimResearchActivity_InternationalCollaboration,
                                StartDate = new ProfileEditorDate()
                                {
                                    Year = p.DimResearchActivity_StartDate_Year,
                                    Month = p.DimResearchActivity_StartDate_Month,
                                    Day = p.DimResearchActivity_StartDate_Day
                                },
                                EndDate = new ProfileEditorDate()
                                {
                                    Year = p.DimResearchActivity_EndDate_Year,
                                    Month = p.DimResearchActivity_EndDate_Month,
                                    Day = p.DimResearchActivity_EndDate_Day
                                },
                                itemMeta = new()
                                {
                                    Id = p.FactFieldValues_DimResearchActivityId,
                                    Type = Constants.ItemMetaTypes.ACTIVITY_RESEARCH_ACTIVITY,
                                    Show = p.FactFieldValues_Show,
                                    PrimaryValue = p.FactFieldValues_PrimaryValue
                                },
                                ActivityTypeCode = p.DimResearchActivity_ActivityType_CodeValue,
                                ActivityTypeNameFi = nameTraslationResearchActivityTypeName.NameFi,
                                ActivityTypeNameEn = nameTraslationResearchActivityTypeName.NameEn,
                                ActivityTypeNameSv = nameTraslationResearchActivityTypeName.NameSv,
                                RoleCode = p.DimResearchActivity_Role_CodeValue,
                                RoleNameFi = nameTraslationResearchActivityRoleName.NameFi,
                                RoleNameEn = nameTraslationResearchActivityRoleName.NameEn,
                                RoleNameSv = nameTraslationResearchActivityRoleName.NameSv,
                                OrganizationNameFi = nameTranslationResearchActivityOrganization.NameFi,
                                OrganizationNameEn = nameTranslationResearchActivityOrganization.NameEn,
                                OrganizationNameSv = nameTranslationResearchActivityOrganization.NameSv,
                                DepartmentNameFi = nameTranslationResearchActivityDepartment.NameFi,
                                DepartmentNameEn = nameTranslationResearchActivityDepartment.NameEn,
                                DepartmentNameSv = nameTranslationResearchActivityDepartment.NameSv,
                                DataSources = new List<ProfileEditorSource> { profileEditorSource }
                            };

                            // Add Elasticsearch person index related data.
                            if (forElasticsearch && !String.IsNullOrWhiteSpace(p.DimResearchActivity_DimOrganization_DimSector_SectorId))
                            {
                                activityAndReward.sector = new List<ProfileEditorSector>
                                {
                                    new ProfileEditorSector()
                                    {
                                        sectorId = p.DimResearchActivity_DimOrganization_DimSector_SectorId,
                                        nameFiSector = nameTranslationResearchActivityOrganizationSector.NameFi,
                                        nameEnSector = nameTranslationResearchActivityOrganizationSector.NameEn,
                                        nameSvSector = nameTranslationResearchActivityOrganizationSector.NameSv,
                                        organization = new List<ProfileEditorSectorOrganization>() {
                                            new ProfileEditorSectorOrganization()
                                            {
                                                organizationId = p.DimResearchActivity_DimOrganization_OrganizationId,
                                                OrganizationNameFi = nameTranslationResearchActivityOrganization.NameFi,
                                                OrganizationNameEn = nameTranslationResearchActivityOrganization.NameEn,
                                                OrganizationNameSv = nameTranslationResearchActivityOrganization.NameSv
                                            }
                                        }
                                    }
                                };
                            }
                            profileDataResponse.activity.activitiesAndRewards.Add(activityAndReward);
                        }

                        // DimProfileOnlyResearchActivity
                        if (p.FactFieldValues_DimProfileOnlyResearchActivityId != -1)
                        {
                            // Research activity organization search order:
                            // 1. DimProfileOnlyResearchActivity_DimOrganizationBroader_Id
                            // 2. DimProfileOnlyResearchActivity_DimOrganization_Id
                            // 3. DimIdentifierlessData

                            // Name translation service ensures that none of the language fields is empty.
                            NameTranslation nameTranslationProfileOnlyResearchActivityOrganization = new();
                            NameTranslation nameTranslationProfileOnlyResearchActivityOrganizationSector = new();
                            NameTranslation nameTranslationProfileOnlyResearchActivityDepartment = new();

                            // Organization name
                            if (p.DimProfileOnlyResearchActivity_DimOrganizationBroader_Id > 0)
                            {
                                nameTranslationProfileOnlyResearchActivityOrganization = _languageService.GetNameTranslation(
                                    nameFi: p.DimProfileOnlyResearchActivity_DimOrganizationBroader_NameFi,
                                    nameEn: p.DimProfileOnlyResearchActivity_DimOrganizationBroader_NameEn,
                                    nameSv: p.DimProfileOnlyResearchActivity_DimOrganizationBroader_NameSv
                                );

                                nameTranslationProfileOnlyResearchActivityOrganization = _languageService.GetNameTranslation(
                                    nameFi: p.DimProfileOnlyResearchActivity_DimOrganizationBroader_DimSector_NameFi,
                                    nameEn: p.DimProfileOnlyResearchActivity_DimOrganizationBroader_DimSector_NameEn,
                                    nameSv: p.DimProfileOnlyResearchActivity_DimOrganizationBroader_DimSector_NameSv
                                );
                            }
                            else if (p.DimProfileOnlyResearchActivity_DimOrganization_Id > 0)
                            {
                                nameTranslationProfileOnlyResearchActivityOrganization = _languageService.GetNameTranslation(
                                    nameFi: p.DimProfileOnlyResearchActivity_DimOrganization_NameFi,
                                    nameEn: p.DimProfileOnlyResearchActivity_DimOrganization_NameEn,
                                    nameSv: p.DimProfileOnlyResearchActivity_DimOrganization_NameSv
                                );

                                nameTranslationProfileOnlyResearchActivityOrganizationSector = _languageService.GetNameTranslation(
                                    nameFi: p.DimProfileOnlyResearchActivity_DimOrganization_DimSector_NameFi,
                                    nameEn: p.DimProfileOnlyResearchActivity_DimOrganization_DimSector_NameEn,
                                    nameSv: p.DimProfileOnlyResearchActivity_DimOrganization_DimSector_NameSv
                                );
                            }
                            else if (p.FactFieldValues_DimIdentifierlessDataId > -1 &&
                                p.DimIdentifierlessData_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                            {
                                nameTranslationProfileOnlyResearchActivityOrganization = _languageService.GetNameTranslation(
                                    nameFi: p.DimIdentifierlessData_ValueFi,
                                    nameEn: p.DimIdentifierlessData_ValueEn,
                                    nameSv: p.DimIdentifierlessData_ValueSv
                                );
                            }

                            // Department name
                            if (p.DimProfileOnlyResearchActivity_DimOrganizationBroader_Id > 0)
                            {
                                // When DimOrganizationBroader is available, it contains the organization name and DimOrganization contains department name.
                                nameTranslationProfileOnlyResearchActivityDepartment = _languageService.GetNameTranslation(
                                    nameFi: p.DimProfileOnlyResearchActivity_DimOrganization_NameFi,
                                    nameEn: p.DimProfileOnlyResearchActivity_DimOrganization_NameEn,
                                    nameSv: p.DimProfileOnlyResearchActivity_DimOrganization_NameSv
                                );
                            }
                            else if (p.DimIdentifierlessData_Type != null && p.DimIdentifierlessData_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                            {
                                nameTranslationProfileOnlyResearchActivityDepartment = _languageService.GetNameTranslation(
                                    nameFi: p.DimIdentifierlessData_ValueFi,
                                    nameEn: p.DimIdentifierlessData_ValueEn,
                                    nameSv: p.DimIdentifierlessData_ValueSv
                                );
                            }
                            else if (p.DimIdentifierlessData_Child_Type != null && p.DimIdentifierlessData_Child_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                            {
                                nameTranslationProfileOnlyResearchActivityDepartment = _languageService.GetNameTranslation(
                                    nameFi: p.DimIdentifierlessData_Child_ValueFi,
                                    nameEn: p.DimIdentifierlessData_Child_ValueEn,
                                    nameSv: p.DimIdentifierlessData_Child_ValueSv
                                );
                            }

                            NameTranslation nameTraslationProfileOnlyResearchActivityName = _languageService.GetNameTranslation(
                                nameFi: p.DimProfileOnlyResearchActivity_NameFi,
                                nameEn: p.DimProfileOnlyResearchActivity_NameEn,
                                nameSv: p.DimProfileOnlyResearchActivity_NameSv
                            );
                            NameTranslation nameTraslationProfileOnlyResearchActivityDescription = _languageService.GetNameTranslation(
                                nameFi: p.DimProfileOnlyResearchActivity_DescriptionFi,
                                nameEn: p.DimProfileOnlyResearchActivity_DescriptionEn,
                                nameSv: p.DimProfileOnlyResearchActivity_DescriptionSv
                            );
                            NameTranslation nameTraslationProfileOnlyResearchActivityRoleName = _languageService.GetNameTranslation(
                                nameFi: p.DimProfileOnlyResearchActivity_Role_NameFi,
                                nameEn: p.DimProfileOnlyResearchActivity_Role_NameEn,
                                nameSv: p.DimProfileOnlyResearchActivity_Role_NameSv
                            );

                            ProfileEditorActivityAndReward activityAndRewardProfileOnly = new()
                            {
                                NameFi = nameTraslationProfileOnlyResearchActivityName.NameFi,
                                NameEn = nameTraslationProfileOnlyResearchActivityName.NameEn,
                                NameSv = nameTraslationProfileOnlyResearchActivityName.NameSv,
                                DescriptionFi = nameTraslationProfileOnlyResearchActivityDescription.NameFi,
                                DescriptionEn = nameTraslationProfileOnlyResearchActivityDescription.NameEn,
                                DescriptionSv = nameTraslationProfileOnlyResearchActivityDescription.NameSv,
                                InternationalCollaboration = null, // not available in DimProfileOnlyResearchActivity
                                StartDate = new ProfileEditorDate()
                                {
                                    Year = p.DimProfileOnlyResearchActivity_StartDate_Year,
                                    Month = p.DimProfileOnlyResearchActivity_StartDate_Month,
                                    Day = p.DimProfileOnlyResearchActivity_StartDate_Day
                                },
                                EndDate = new ProfileEditorDate()
                                {
                                    Year = p.DimProfileOnlyResearchActivity_EndDate_Year,
                                    Month = p.DimProfileOnlyResearchActivity_EndDate_Month,
                                    Day = p.DimProfileOnlyResearchActivity_EndDate_Day
                                },
                                itemMeta = new()
                                {
                                    Id = p.FactFieldValues_DimProfileOnlyResearchActivityId,
                                    Type = Constants.ItemMetaTypes.ACTIVITY_RESEARCH_ACTIVITY_PROFILE_ONLY,
                                    Show = p.FactFieldValues_Show,
                                    PrimaryValue = p.FactFieldValues_PrimaryValue
                                },
                                RoleCode = p.DimProfileOnlyResearchActivity_Role_CodeValue,
                                RoleNameFi = nameTraslationProfileOnlyResearchActivityRoleName.NameFi,
                                RoleNameEn = nameTraslationProfileOnlyResearchActivityRoleName.NameEn,
                                RoleNameSv = nameTraslationProfileOnlyResearchActivityRoleName.NameSv,
                                OrganizationNameFi = nameTranslationProfileOnlyResearchActivityOrganization.NameFi,
                                OrganizationNameEn = nameTranslationProfileOnlyResearchActivityOrganization.NameEn,
                                OrganizationNameSv = nameTranslationProfileOnlyResearchActivityOrganization.NameSv,
                                DepartmentNameFi = nameTranslationProfileOnlyResearchActivityDepartment.NameFi,
                                DepartmentNameEn = nameTranslationProfileOnlyResearchActivityDepartment.NameEn,
                                DepartmentNameSv = nameTranslationProfileOnlyResearchActivityDepartment.NameSv,
                                DataSources = new List<ProfileEditorSource> { profileEditorSource }
                            };

                            // Add Elasticsearch person index related data.
                            if (forElasticsearch && !String.IsNullOrWhiteSpace(p.DimProfileOnlyResearchActivity_DimOrganization_DimSector_SectorId))
                            {
                                activityAndRewardProfileOnly.sector = new List<ProfileEditorSector>
                                {
                                    new ProfileEditorSector()
                                    {
                                        sectorId = p.DimProfileOnlyResearchActivity_DimOrganization_DimSector_SectorId,
                                        nameFiSector = nameTranslationProfileOnlyResearchActivityOrganizationSector.NameFi,
                                        nameEnSector = nameTranslationProfileOnlyResearchActivityOrganizationSector.NameEn,
                                        nameSvSector = nameTranslationProfileOnlyResearchActivityOrganizationSector.NameSv,
                                        organization = new List<ProfileEditorSectorOrganization>() {
                                            new ProfileEditorSectorOrganization()
                                            {
                                                organizationId = p.DimProfileOnlyResearchActivity_DimOrganization_OrganizationId,
                                                OrganizationNameFi = nameTranslationProfileOnlyResearchActivityOrganization.NameFi,
                                                OrganizationNameEn = nameTranslationProfileOnlyResearchActivityOrganization.NameEn,
                                                OrganizationNameSv = nameTranslationProfileOnlyResearchActivityOrganization.NameSv
                                            }
                                        }
                                    }
                                };
                            }
                            profileDataResponse.activity.activitiesAndRewards.Add(activityAndRewardProfileOnly);
                        }

                        break;

                    // Funding decision
                    case Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION:
                        // Name translation: funding decision name
                        NameTranslation nameTranslationFundingDecisionName = _languageService.GetNameTranslation(
                            nameFi: p.DimFundingDecision_NameFi,
                            nameEn: p.DimFundingDecision_NameEn,
                            nameSv: p.DimFundingDecision_NameSv
                        );
                        // Name translation: funding decision description
                        NameTranslation nameTranslationFundingDecisionDescription = _languageService.GetNameTranslation(
                            nameFi: p.DimFundingDecision_DescriptionFi,
                            nameEn: p.DimFundingDecision_DescriptionEn,
                            nameSv: p.DimFundingDecision_DescriptionSv
                        );
                        // Name translation: funder name
                        NameTranslation nameTranslationFunderName = _languageService.GetNameTranslation(
                            nameFi: p.DimFundingDecision_Funder_NameFi,
                            nameEn: p.DimFundingDecision_Funder_NameEn,
                            nameSv: p.DimFundingDecision_Funder_NameSv
                        );
                        // Name translation: call programme
                        NameTranslation nameTranslationCallProgramme = _languageService.GetNameTranslation(
                            nameFi: p.DimFundingDecision_DimCallProgramme_NameFi,
                            nameEn: p.DimFundingDecision_DimCallProgramme_NameEn,
                            nameSv: p.DimFundingDecision_DimCallProgramme_NameSv
                        );
                        // Name translation: type of funding name
                        NameTranslation nameTranslationTypeOfFundingName = _languageService.GetNameTranslation(
                            nameFi: p.DimFundingDecision_DimTypeOfFunding_NameFi,
                            nameEn: p.DimFundingDecision_DimTypeOfFunding_NameEn,
                            nameSv: p.DimFundingDecision_DimTypeOfFunding_NameSv
                        );
                        profileDataResponse.activity.fundingDecisions.Add(
                            new ProfileEditorFundingDecision()
                            {
                                ProjectId = p.FactFieldValues_DimFundingDecisionId,
                                ProjectAcronym = p.DimFundingDecision_Acronym,
                                ProjectNameFi = nameTranslationFundingDecisionName.NameFi,
                                ProjectNameEn = nameTranslationFundingDecisionName.NameEn,
                                ProjectNameSv = nameTranslationFundingDecisionName.NameSv,
                                ProjectDescriptionFi = nameTranslationFundingDecisionDescription.NameFi,
                                ProjectDescriptionEn = nameTranslationFundingDecisionDescription.NameEn,
                                ProjectDescriptionSv = nameTranslationFundingDecisionDescription.NameSv,
                                FunderNameFi = nameTranslationFunderName.NameFi,
                                FunderNameEn = nameTranslationFunderName.NameEn,
                                FunderNameSv = nameTranslationFunderName.NameSv,
                                FunderProjectNumber = p.DimFundingDecision_FunderProjectNumber,
                                TypeOfFundingNameFi = nameTranslationTypeOfFundingName.NameFi,
                                TypeOfFundingNameEn = nameTranslationTypeOfFundingName.NameEn,
                                TypeOfFundingNameSv = nameTranslationTypeOfFundingName.NameSv,
                                CallProgrammeNameFi = nameTranslationCallProgramme.NameFi,
                                CallProgrammeNameEn = nameTranslationCallProgramme.NameEn,
                                CallProgrammeNameSv = nameTranslationCallProgramme.NameSv,
                                FundingStartYear = p.DimFundingDecision_StartDate_Year,
                                FundingEndYear = p.DimFundingDecision_EndDate_Year,
                                AmountInEur = p.DimFundingDecision_amount_in_EUR,
                                itemMeta = new ProfileEditorItemMeta()
                                {
                                    Id = p.FactFieldValues_DimFundingDecisionId,
                                    Type = Constants.ItemMetaTypes.ACTIVITY_FUNDING_DECISION,
                                    Show = p.FactFieldValues_Show,
                                    PrimaryValue = p.FactFieldValues_PrimaryValue
                                },
                                DataSources = new List<ProfileEditorSource> { profileEditorSource }
                            }
                        );
                        break;

                    // Research dataset
                    case Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET:
                        // Name translation: research dataset name
                        NameTranslation nameTranslationResearchDatasetName = _languageService.GetNameTranslation(
                            nameFi: p.DimResearchDataset_NameFi,
                            nameEn: p.DimResearchDataset_NameEn,
                            nameSv: p.DimResearchDataset_NameSv
                        );
                        // Name translation: research dataset description
                        NameTranslation nameTranslationResearchDatasetDescription = _languageService.GetNameTranslation(
                            nameFi: p.DimResearchDataset_DescriptionFi,
                            nameEn: p.DimResearchDataset_DescriptionEn,
                            nameSv: p.DimResearchDataset_DescriptionSv
                        );
                        profileDataResponse.activity.researchDatasets.Add(
                            new ProfileEditorResearchDataset()
                            {
                                // List<ProfileEditorActor> Actor
                                Identifier = p.DimResearchDataset_LocalIdentifier,
                                NameFi = nameTranslationResearchDatasetName.NameFi,
                                NameEn = nameTranslationResearchDatasetName.NameEn,
                                NameSv = nameTranslationResearchDatasetName.NameSv,
                                DescriptionFi = nameTranslationResearchDatasetDescription.NameFi,
                                DescriptionSv = nameTranslationResearchDatasetDescription.NameFi,
                                DescriptionEn = nameTranslationResearchDatasetDescription.NameFi,
                                // Only year part of datetime is set in DatasetCreated 
                                DatasetCreated =
                                    (p.DimResearchDataset_DatasetCreated != null) ? p.DimResearchDataset_DatasetCreated.Value.Year : null,
                                PreferredIdentifiers =
                                    (await connection.QueryAsync<ProfileEditorPreferredIdentifier>(
                                        $"SELECT pid_type AS 'PidType', pid_content AS 'PidContent' FROM dim_pid WHERE dim_research_dataset_id={p.FactFieldValues_DimResearchDatasetId}"
                                    )).ToList(),
                                itemMeta = new ProfileEditorItemMeta()
                                {
                                    Id = p.FactFieldValues_DimResearchDatasetId,
                                    Type = Constants.ItemMetaTypes.ACTIVITY_RESEARCH_DATASET,
                                    Show = p.FactFieldValues_Show,
                                    PrimaryValue = p.FactFieldValues_PrimaryValue
                                },
                                DataSources = new List<ProfileEditorSource> { profileEditorSource }
                            }
                        );
                        break;

                    default:
                        break;
                }
            }

            return profileDataResponse;
        }

        /*
         * Delete profile data using Dapper
         * - get FactFieldValues
         * - delete FactFieldValues
         * - delete DimIdentifierlessData (children and parent)
         * - deöete ORCID put codes from DimPid
         * - delete FactFieldValues related items
         * - delete DimFieldDisplaySettings
         * - delete BrGrantedPermissions (sharing permission)
         * - delete DimUserChoices (co-operation selection) 
         * - delete DimUserProfile  
         */
        public async Task<bool> DeleteProfileDataAsync(int userprofileId, LogUserIdentification logUserIdentification)
        {
            using (var connection = _ttvContext.Database.GetDbConnection())
            {
                // Get list of FactFieldValues using Entity Framework, which ensures that model FactFieldValue populates correctly.
                // After that delete database items using Dapper.
                List<FactFieldValue> factFieldValues =
                    await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId)
                    .AsNoTracking().ToListAsync();

                // Open database connection
                await connection.OpenAsync();
                // Begin database transaction
                var transaction = connection.BeginTransaction();

                // For efficiency, rows from a table are deleted in on SQL statement.
                // Collect deletable IDs into lists.
                List<int> dimAffiliationIds = new();
                List<int> dimCompetenceIds = new();
                List<int> dimEducationIds = new();
                List<int> dimEmailAddrressIds = new();
                List<int> dimEventIds = new();
                List<int> dimFieldOfScienceIds = new();
                List<int> dimFundingDecisionIds = new();
                List<int> dimKeywordIds = new();
                List<int> dimNameIds = new();
                List<int> dimProfileOnlyPublicationIds = new();
                List<int> dimProfileOnlyResearchActivityIds = new();
                List<int> dimPidIds = new();
                List<int> dimResearchActivityIds = new();
                List<int> dimResearchCommunityIds = new();
                List<int> dimResearchDatasetIds = new();
                List<int> dimResearcherDescriptionIds = new();
                List<int> dimResearcherToResearchCommunityIds = new();
                List<int> dimTelephoneNumberIds = new();
                List<int> dimWebLinkIds = new();

                try
                {
                    // Delete fact_field_values
                    await connection.ExecuteAsync(
                        sql: _ttvSqlService.GetSqlQuery_Delete_FactFieldValues(userprofileId),
                        transaction: transaction
                    );

                    // Delete fact_field_values related items
                    foreach (FactFieldValue factFieldValue in factFieldValues)
                    {
                        // Not all related data should be automatically deleted
                        if (CanDeleteFactFieldValueRelatedData(factFieldValue))
                        {
                            // dim_identifierless_data needs special handling, since it can have nested items
                            if (factFieldValue.DimIdentifierlessDataId != -1)
                            {
                                // First delete possible child items from dim_identifierless_data
                                await connection.ExecuteAsync(
                                    sql: _ttvSqlService.GetSqlQuery_Delete_DimIdentifierlessData_Children(factFieldValue.DimIdentifierlessDataId),
                                    transaction: transaction
                                );

                                // Then delete parent from dim_identifierless_data
                                await connection.ExecuteAsync(
                                    sql: _ttvSqlService.GetSqlQuery_Delete_DimIdentifierlessData_Parent(factFieldValue.DimIdentifierlessDataId),
                                    transaction: transaction
                                );
                            }

                            // Collect IDs
                            if (factFieldValue.DimAffiliationId != -1) dimAffiliationIds.Add(factFieldValue.DimAffiliationId);
                            if (factFieldValue.DimCompetenceId != -1) dimCompetenceIds.Add(factFieldValue.DimCompetenceId);
                            if (factFieldValue.DimEducationId != -1) dimEducationIds.Add(factFieldValue.DimEducationId);
                            if (factFieldValue.DimEmailAddrressId != -1) dimEmailAddrressIds.Add(factFieldValue.DimEmailAddrressId);
                            if (factFieldValue.DimEventId != -1) dimEventIds.Add(factFieldValue.DimEventId);
                            if (factFieldValue.DimReferencedataFieldOfScienceId != -1) dimFieldOfScienceIds.Add(factFieldValue.DimReferencedataFieldOfScienceId);
                            if (factFieldValue.DimFundingDecisionId != -1) dimFundingDecisionIds.Add(factFieldValue.DimFundingDecisionId);
                            if (factFieldValue.DimKeywordId != -1) dimKeywordIds.Add(factFieldValue.DimKeywordId);
                            if (factFieldValue.DimNameId != -1) dimNameIds.Add(factFieldValue.DimNameId);
                            if (factFieldValue.DimProfileOnlyPublicationId != -1) dimProfileOnlyPublicationIds.Add(factFieldValue.DimProfileOnlyPublicationId);
                            if (factFieldValue.DimProfileOnlyResearchActivityId != -1) dimProfileOnlyResearchActivityIds.Add(factFieldValue.DimProfileOnlyResearchActivityId);
                            if (factFieldValue.DimPidId != -1) dimPidIds.Add(factFieldValue.DimPidId);
                            if (factFieldValue.DimPidIdOrcidPutCode != -1) dimPidIds.Add(factFieldValue.DimPidIdOrcidPutCode);
                            if (factFieldValue.DimResearchActivityId != -1) dimResearchActivityIds.Add(factFieldValue.DimResearchActivityId);
                            if (factFieldValue.DimResearchCommunityId != -1) dimResearchCommunityIds.Add(factFieldValue.DimResearchCommunityId);
                            if (factFieldValue.DimResearchDatasetId != -1) dimResearchDatasetIds.Add(factFieldValue.DimResearchDatasetId);
                            if (factFieldValue.DimResearcherDescriptionId != -1) dimResearcherDescriptionIds.Add(factFieldValue.DimResearcherDescriptionId);
                            if (factFieldValue.DimResearcherToResearchCommunityId != -1) dimResearcherToResearchCommunityIds.Add(factFieldValue.DimResearcherToResearchCommunityId);
                            if (factFieldValue.DimTelephoneNumberId != -1) dimTelephoneNumberIds.Add(factFieldValue.DimTelephoneNumberId);
                            if (factFieldValue.DimWebLinkId != -1) dimWebLinkIds.Add(factFieldValue.DimWebLinkId);
                        }
                    }

                    // Delete affiliations
                    if (dimAffiliationIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimAffiliations(dimAffiliationIds),
                            transaction: transaction
                        );
                    }
                    // Delete competences
                    if (dimCompetenceIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimCompetences(dimCompetenceIds),
                            transaction: transaction
                        );
                    }
                    // Delete educations
                    if (dimEducationIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimEducations(dimEducationIds),
                            transaction: transaction
                        );
                    }
                    // Delete email addresses
                    if (dimEmailAddrressIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimEmailAddrresses(dimEmailAddrressIds),
                            transaction: transaction
                        );
                    }
                    // Delete events
                    if (dimEventIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimEvents(dimEventIds),
                            transaction: transaction
                        );
                    }
                    // Delete fields of science
                    if (dimFieldOfScienceIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimFieldsOfScience(dimFieldOfScienceIds),
                            transaction: transaction
                        );
                    }
                    // Delete funding decisions
                    if (dimFundingDecisionIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimFundingDecisions(dimFundingDecisionIds),
                            transaction: transaction
                        );
                    }
                    // Delete Keywords
                    if (dimKeywordIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimKeyword(dimKeywordIds),
                            transaction: transaction
                        );
                    }
                    // Delete names
                    if (dimNameIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimNames(dimNameIds),
                            transaction: transaction
                        );
                    }
                    // Delete profile only publications
                    if (dimProfileOnlyPublicationIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimProfileOnlyPublications(dimProfileOnlyPublicationIds),
                            transaction: transaction
                        );
                    }
                    // Delete profile only research activities
                    if (dimProfileOnlyResearchActivityIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimProfileOnlyResearchActivities(dimProfileOnlyResearchActivityIds),
                            transaction: transaction
                        );
                    }
                    // Delete PIDs
                    if (dimPidIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimPids(dimPidIds),
                            transaction: transaction
                        );
                    }
                    // Delete research activities
                    if (dimResearchActivityIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimResearchActivities(dimResearchActivityIds),
                            transaction: transaction
                        );
                    }
                    // Delete research communities
                    if (dimResearchCommunityIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimResearchCommunities(dimResearchCommunityIds),
                            transaction: transaction
                        );
                    }
                    // Delete research datasets
                    if (dimResearchDatasetIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimResearchDatasets(dimResearchDatasetIds),
                            transaction: transaction
                        );
                    }
                    // Delete researcher descriptions
                    if (dimResearcherDescriptionIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimResearchDescriptions(dimResearcherDescriptionIds),
                            transaction: transaction
                        );
                    }
                    // Delete researcher to research communities
                    if (dimResearcherToResearchCommunityIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimResearcherToResearchCommunities(dimResearcherToResearchCommunityIds),
                            transaction: transaction
                        );
                    }
                    // Delete telephone numbers
                    if (dimTelephoneNumberIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimTelephoneNumbers(dimTelephoneNumberIds),
                            transaction: transaction
                        );
                    }
                    // Delete web links
                    if (dimWebLinkIds.Count > 0)
                    {
                        await connection.ExecuteAsync(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimWebLinks(dimWebLinkIds),
                            transaction: transaction
                        );
                    }

                    // Delete dim_field_display_settings
                    await connection.ExecuteAsync(
                        sql: _ttvSqlService.GetSqlQuery_Delete_DimFieldDisplaySettings(userprofileId),
                        transaction: transaction
                    );

                    // Delete br_granted_permissions
                    await connection.ExecuteAsync(
                        sql: _ttvSqlService.GetSqlQuery_Delete_BrGrantedPermissions(userprofileId),
                        transaction: transaction
                    );

                    // Delete dim_user_choices
                    await connection.ExecuteAsync(
                        sql: _ttvSqlService.GetSqlQuery_Delete_DimUserChoices(userprofileId),
                        transaction: transaction
                    );

                    // Delete dim_user_profile
                    await connection.ExecuteAsync(
                        sql: _ttvSqlService.GetSqlQuery_Delete_DimUserProfile(userprofileId),
                        transaction: transaction
                    );

                    // Commit transaction
                    transaction.Commit();
                }
                catch (Exception exceptionFromProfileDelete)
                {
                    // Log error from profile deletion
                    _logger.LogError($"Error deleting user profile (dim_user_profile.id={userprofileId}): " + exceptionFromProfileDelete.ToString());

                    // Rollback
                    _logger.LogInformation($"Try to rollback user profile deletion (dim_user_profile.id={userprofileId})");
                    try
                    {
                        transaction.Rollback();
                        _logger.LogInformation($"Rollback success (dim_user_profile.id={userprofileId})");
                    }
                    catch (Exception exceptionFromRollback)
                    {
                        // Log error from rollback
                        _logger.LogError($"Error in rollback of user profile deletion (dim_user_profile.id={userprofileId}): " + exceptionFromRollback.ToString());
                    }
                    return false;
                }
            }
            return true;
        }

        /*
         * Check by dim_user_profile.id if user profile is published.
         * Logic: User profile is considered as published, if more than 1 item has property show=true.
         *        Property 'show' is checked from table fact_field_values
         *        
         *        In user profile, the name from ORCID has always show=1, hence the requirement "more than 1 item".
         */
        public async Task<bool> IsUserprofilePublished(int dimUserProfileId)
        {
            int publishedCount = 0;
            using (var connection = _ttvContext.Database.GetDbConnection())
            {
                string publishedCountSql = _ttvSqlService.GetSqlQuery_Select_CountPublishedItemsInUserprofile(dimUserProfileId);
                publishedCount = (await connection.QueryAsync<int>(publishedCountSql)).First();
            }
            return publishedCount > 1;
        }

        /*
         * Execute raw sql.
         */
        public async Task ExecuteRawSql(string sql)
        {
            await _ttvContext.Database.ExecuteSqlRawAsync(sql);
        }
    }
}
