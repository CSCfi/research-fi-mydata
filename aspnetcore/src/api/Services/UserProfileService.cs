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
using api.Services.Profiledata;
using Dapper;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace api.Services
{
    /*
     * UserProfileService implements utilities, which simplify handling of userprofile and related data.
     */
    public class UserProfileService : IUserProfileService
    {
        private readonly TtvContext _ttvContext;
        private readonly IAffiliationService _affiliationService;
        private readonly IEducationService _educationService;
        private readonly IEmailService _emailService;
        private readonly IExternalIdentifierService _externalIdentifierService;
        private readonly IKeywordService _keywordService;
        private readonly INameService _nameService;
        private readonly IPublicationService _publicationService;
        private readonly IResearcherDescriptionService _researcherDescriptionService;
        private readonly ITelephoneNumberService _telephoneNumberService;
        private readonly IWebLinkService _webLinkService;
        private readonly IFundingDecisionService _fundingDecisionService;
        private readonly IResearchDatasetService _researchDatasetService;
        private readonly IResearchActivityService _researchActivityService;
        private readonly IUniqueDataSourcesService _uniqueDataSourcesService;
        private readonly IDataSourceHelperService _dataSourceHelperService;
        private readonly IUtilityService _utilityService;
        private readonly ILanguageService _languageService;
        private readonly IDuplicateHandlerService _duplicateHandlerService;
        private readonly ISharingService _sharingService;
        private readonly ITtvSqlService _ttvSqlService;
        private readonly ILogger<UserProfileService> _logger;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ISettingsService _settingsService;
        private readonly ICooperationChoicesService _cooperationChoicesService;

        public UserProfileService(
            TtvContext ttvContext,
            IDataSourceHelperService dataSourceHelperService,
            IUtilityService utilityService,
            ILanguageService languageService,
            IDuplicateHandlerService duplicateHandlerService,
            ISharingService sharingService,
            ITtvSqlService ttvSqlService,
            ILogger<UserProfileService> logger,
            IElasticsearchService elasticsearchService,
            IAffiliationService affiliationService,
            IEducationService educationService,
            IEmailService emailService,
            IExternalIdentifierService externalIdentifierService,
            IKeywordService keywordService,
            INameService nameService,
            IPublicationService publicationService,
            IResearcherDescriptionService researcherDescriptionService,
            ITelephoneNumberService telephoneNumberService,
            IWebLinkService webLinkService,
            IFundingDecisionService fundingDecisionService,
            IResearchDatasetService researchDatasetService,
            IResearchActivityService researchActivityService,
            IUniqueDataSourcesService uniqueDataSourcesService,
            ISettingsService settingsService,
            ICooperationChoicesService cooperationChoicesService)
        {
            _ttvContext = ttvContext;
            _dataSourceHelperService = dataSourceHelperService;
            _utilityService = utilityService;
            _languageService = languageService;
            _settingsService = settingsService;
            _duplicateHandlerService = duplicateHandlerService;
            _sharingService = sharingService;
            _ttvSqlService = ttvSqlService;
            _logger = logger;
            _cooperationChoicesService = cooperationChoicesService;
            _elasticsearchService = elasticsearchService;
            _uniqueDataSourcesService = uniqueDataSourcesService;
            _researchActivityService = researchActivityService;
            _affiliationService = affiliationService;
            _educationService = educationService;
            _emailService = emailService;
            _externalIdentifierService = externalIdentifierService;
            _keywordService = keywordService;
            _nameService = nameService;
            _publicationService = publicationService;
            _researcherDescriptionService = researcherDescriptionService;
            _telephoneNumberService = telephoneNumberService;
            _webLinkService = webLinkService;
            _fundingDecisionService = fundingDecisionService;
            _researchDatasetService = researchDatasetService;
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

        public UserProfileService(IUtilityService utilityService, ILogger<UserProfileService> logger) {
            _utilityService = utilityService;
            _logger = logger;
        }

        public UserProfileService(IDataSourceHelperService dataSourceHelperService)
        {
            _dataSourceHelperService = dataSourceHelperService;
        }

        public UserProfileService(ILanguageService languageService)
        {
            _languageService = languageService;
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
         * Get DimUserProfile based on ORCID Id.
         * Returns tracking entity to allow modifications.
         */
        public async Task<DimUserProfile> GetUserprofileTracking(string orcidId)
        {
            return await _ttvContext.DimUserProfiles.Where(dup => dup.OrcidId == orcidId).FirstOrDefaultAsync();
        }

        /*
         * Get DimUserProfile based on Id.
         */
        public async Task<DimUserProfile> GetUserprofileById(int Id)
        {
            return await _ttvContext.DimUserProfiles.Where(dup => dup.Id == Id).AsNoTracking().FirstOrDefaultAsync();
        }

        /*
         * Check if user profile exists for ORCID Id.
         */
        public async Task<(bool UserProfileExists, int UserProfileId)> GetUserprofileIdForOrcidId(string orcidId)
        {   
            IntegerDTO dimUserProfileIdDTO = await _ttvContext.DimUserProfiles.Where(dup => dup.OrcidId == orcidId).AsNoTracking()
                .Select(dimUserProfile => new IntegerDTO()  
                {  
                    Value = dimUserProfile.Id
                }).FirstOrDefaultAsync();
            if (dimUserProfileIdDTO == null || dimUserProfileIdDTO.Value < 0) {
                return (UserProfileExists: false,  UserProfileId: -1);
            }
            else {
                return (UserProfileExists: true,  UserProfileId: dimUserProfileIdDTO.Value);
            }
        }

        /*
         * Set 'modified' timestamp in user profile
         */
        public async Task SetModifiedTimestampInUserProfile(int Id)
        {
            await ExecuteRawSql(
                _ttvSqlService.GetSqlQuery_Update_DimUserProfile_Modified(Id)
            ); 
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
         * Get empty DimProfileOnlyDataset.
         * Must use -1 in required foreign keys.
         */
        public DimProfileOnlyDataset GetEmptyDimProfileOnlyDataset()
        {
            return new DimProfileOnlyDataset()
            {
                DimReferencedataIdAvailability = null,
                OrcidWorkType = "",
                LocalIdentifier = "",
                NameFi = "",
                NameEn = "",
                NameSv = "",
                NameUnd = "",
                DescriptionFi = "",
                DescriptionSv = "",
                DescriptionEn = "",
                DescriptionUnd = "",
                VersionInfo = "",
                DatasetCreated = null,
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = null,
                Modified = null,
                DimRegisteredDataSourceId = -1
            };
        }

        /*
         * Get empty DimProfileOnlyFundingDecision.
         * Must use -1 in required foreign keys.
         */
        public DimProfileOnlyFundingDecision GetEmptyDimProfileOnlyFundingDecision()
        {
            return new DimProfileOnlyFundingDecision()
            {
                DimDateIdApproval = -1,
                DimDateIdStart = -1,
                DimDateIdEnd = -1,
                DimCallProgrammeId = -1,
                DimTypeOfFundingId = -1,
                DimOrganizationIdFunder = null,
                OrcidWorkType = "",
                FunderProjectNumber = "",
                Acronym = "",
                NameFi = "",
                NameSv = "",
                NameEn = "",
                NameUnd = "",
                DescriptionFi = "",
                DescriptionEn = "",
                DescriptionSv = "",
                AmountInEur = -1,
                AmountInFundingDecisionCurrency = null,
                FundingDecisionCurrencyAbbreviation = "",
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = null,
                Modified = null,
                DimRegisteredDataSourceId = -1
            };
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
                PublicationCountryCode = -1,
                PublisherName = null,
                PublisherLocation = null,
                ParentPublicationName = null,
                ParentPublicationEditors = null,
                LicenseCode = -1,
                LanguageCode = -1,
                OpenAccessCode = null,
                OriginalPublicationId = null,
                PeerReviewed = null,
                Report = null,
                ThesisTypeCode = -1,
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
                DimResearchDataCatalogId = -1,
                DimResearchActivityId = -1,
                DimEventId = -1,
                DimProfileOnlyDatasetId = -1,
                DimProfileOnlyFundingDecisionId = -1,
                DimProfileOnlyPublicationId = -1,
                DimResearchCommunityId = -1,
                DimResearchProjectId = -1,

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
        public bool CanIncludeDimNameInUserProfile(List<long> existingIDs, DimName dimName)
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
         * Check if research activities are duplicates.
         * Comparison is based on fields:
         * - start year
         * - name FI
         * - name EN
         * - name SV
         */
        public bool IsResearchActivityDuplicate(int aYear, string aNameFi, string aNameEn, string aNameSv, int bYear, string bNameFi, string bNameEn, string bNameSv)
        {
            return (
                aYear == bYear &&
                aNameFi == bNameFi &&
                aNameEn == bNameEn &&
                aNameSv == bNameSv);
        }

        /*
         * Get fullName from lastName and firstNames
         */
        public string GetFullname(string lastName, string firstNames)
        {
            return $"{lastName.Trim()} {firstNames.Trim()}".Trim();
        }

        /*
         * Helper method, set parameter 'show' of new FactFieldValue.
         * This decides wheter new items are automatically included in the profile.
         *
         * Despite its name, the setting DimUserProfile.PublishNewOrcidData covers both ORCID and TTV data.
         */
        public bool SetFactFieldValuesShow(DimUserProfile dimUserProfile, int fieldIdentifier, LogUserIdentification logUserIdentification)
        {
            try
            {
                if (dimUserProfile == null)
                {
                    _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_MODIFY_SHOW,
                        state: LogContent.ActionState.FAILED,
                        message: $"Failed to assert auto publish criteria: dimUserProfile==null"
                        ));
                    return false;
                }
                else if (fieldIdentifier < 0)
                {
                    _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_MODIFY_SHOW,
                        state: LogContent.ActionState.FAILED,
                        message: $"Failed to assert auto publish criteria: fieldIdentifier<0"
                        ));
                    return false;
                }

                return
                    dimUserProfile.PublishNewOrcidData &&
                    fieldIdentifier != Constants.FieldIdentifiers.PERSON_NAME &&
                    fieldIdentifier != Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER &&
                    fieldIdentifier != Constants.FieldIdentifiers.PERSON_KEYWORD &&
                    fieldIdentifier != Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION;
            }
            catch
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_MODIFY_SHOW,
                        state: LogContent.ActionState.FAILED,
                        message: $"Failed to set FactFieldValues.Show"
                        ));
                return false;
            }
        }

        /*
         * Search and add data from TTV database.
         * This is data that is already linked to the ORCID id in DimPid and it's related DimKnownPerson.
         * ProfileOnly* items must be excluded in these queries.
         */
        public async Task AddTtvDataToUserProfile(DimKnownPerson dimKnownPerson, DimUserProfile dimUserProfile, LogUserIdentification logUserIdentification)
        {
            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_ADD_TTV_DATA,
                    state: LogContent.ActionState.START,
                    message: $"dim_user_profile.id={dimUserProfile.Id}"
                    ));

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
            List<long> existingNameIds = new();
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
                existingNameIds = ffvs.Where(ffv => ffv.DimNameId != -1).Select(ffv => ffv.DimNameId).Distinct().ToList<long>();
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
                        factFieldValueEmailAddress.Show = this.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS, logUserIdentification);
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
                            message: $"dim_user_profile.id={dimUserProfile.Id}, email: {ex.ToString()}"));
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
                        factFieldValueWebLink.Show = this.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.PERSON_WEB_LINK, logUserIdentification);
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
                          message: $"dim_user_profile.id={dimUserProfile.Id}, web link: {ex.ToString()}"));
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
                        factFieldValueTelephoneNumber.Show = this.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER, logUserIdentification);
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
                            message: $"dim_user_profile.id={dimUserProfile.Id}, telephone number: {ex.ToString()}"));
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
                        factFieldValueResearcherDescription.Show = this.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION, logUserIdentification);
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
                            message: $"dim_user_profile.id={dimUserProfile.Id}, researcher description: {ex.ToString()}"));
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
                        factFieldValueAffiliation.Show = this.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.ACTIVITY_AFFILIATION, logUserIdentification);
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
                            message: $"dim_user_profile.id={dimUserProfile.Id}, affiliation: {ex.ToString()}"));
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
                        factFieldValueEducation.Show = this.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.ACTIVITY_EDUCATION, logUserIdentification);
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
                            message: $"dim_user_profile.id={dimUserProfile.Id}, education: {ex.ToString()}"));
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
                            factFieldValueName.Show = this.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.PERSON_NAME, logUserIdentification);
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
                            factFieldValueOtherNames.Show = this.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.PERSON_OTHER_NAMES, logUserIdentification);
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
                            //
                            // Co-publications have multiple rows in table fact_contribution. Here only the "main" publication should be included.
                            // If FactContributionTableMinimalDTO.CoPublication_Parent_DimPublicationId has value (other than -1), that must be used.
                            // Otherwise FactContributionTableMinimalDTO.DimPublicationId must be used.
                            int publicationId = fc.CoPublication_Parent_DimPublicationId > 0 ? fc.CoPublication_Parent_DimPublicationId : fc.DimPublicationId;
                            if (publicationId != -1 && !existingPublicationIds.Contains(publicationId))
                            {
                                FactFieldValue factFieldValuePublication = this.GetEmptyFactFieldValue();
                                factFieldValuePublication.DimUserProfileId = dimUserProfile.Id;
                                factFieldValuePublication.DimFieldDisplaySettingsId = dimFieldDisplaySetting_publication.Id;
                                factFieldValuePublication.DimPublicationId = publicationId;
                                factFieldValuePublication.DimRegisteredDataSourceId = dimName.DimRegisteredDataSourceId;
                                factFieldValuePublication.Show = this.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.ACTIVITY_PUBLICATION, logUserIdentification);
                                _ttvContext.FactFieldValues.Add(factFieldValuePublication);
                                // Prevent duplicate key error with publications
                                existingPublicationIds.Add(publicationId);
                            }

                            // research activity
                            if (fc.DimResearchActivityId != -1 && !existingResearchActivityIds.Contains(fc.DimResearchActivityId))
                            {
                                FactFieldValue factFieldValueResearchActivity = this.GetEmptyFactFieldValue();
                                factFieldValueResearchActivity.DimUserProfileId = dimUserProfile.Id;
                                factFieldValueResearchActivity.DimFieldDisplaySettingsId = dimFieldDisplaySetting_researchActivity.Id;
                                factFieldValueResearchActivity.DimResearchActivityId = fc.DimResearchActivityId;
                                factFieldValueResearchActivity.DimRegisteredDataSourceId = dimName.DimRegisteredDataSourceId;
                                factFieldValueResearchActivity.Show = this.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.ACTIVITY_RESEARCH_ACTIVITY, logUserIdentification);
                                _ttvContext.FactFieldValues.Add(factFieldValueResearchActivity);
                                // Prevent duplicate key error with research activities
                                existingResearchActivityIds.Add(fc.DimResearchActivityId);
                            }

                            // research dataset
                            if (fc.DimResearchDatasetId != -1 && !existingResearchDatasetIds.Contains(fc.DimResearchDatasetId))
                            {
                                FactFieldValue factFieldValueResearchDataset = this.GetEmptyFactFieldValue();
                                factFieldValueResearchDataset.DimUserProfileId = dimUserProfile.Id;
                                factFieldValueResearchDataset.DimFieldDisplaySettingsId = dimFieldDisplaySetting_researchDataset.Id;
                                factFieldValueResearchDataset.DimResearchDatasetId = fc.DimResearchDatasetId;
                                factFieldValueResearchDataset.DimRegisteredDataSourceId = dimName.DimRegisteredDataSourceId;
                                factFieldValueResearchDataset.Show = this.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET, logUserIdentification);
                                _ttvContext.FactFieldValues.Add(factFieldValueResearchDataset);
                                // Prevent duplicate key error with research datasets
                                existingResearchDatasetIds.Add(fc.DimResearchDatasetId);
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
                                message: $"dim_user_profile.id={dimUserProfile.Id}, fact_contribution: {ex.ToString()}"));
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
                            factFieldValueFundingDecision.Show = this.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION, logUserIdentification);
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
                                message: $"dim_user_profile.id={dimUserProfile.Id}, br_participates_in_funding_group: {ex.ToString()}"));
                    }
                }

                try
                {
                    await _ttvContext.SaveChangesAsync();

                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.PROFILE_ADD_TTV_DATA,
                            state: LogContent.ActionState.COMPLETE,
                            message: $"dim_user_profile.id={dimUserProfile.Id}"));
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
                                message: $"Add TTV data save changes (dim_user_profile.id={dimUserProfile.Id}): {ex.ToString()}"));
                }
            }
        }

        /*
         * Add TTV publications to profile by matching DOIs from ORCID publications.
         * This is to cover the case where TTV publications are not linked to DimKnownPerson via FactContribution.
         */
        public async Task AddTtvPublicationsByDoiToUserProfile(int dimUserProfileId, LogUserIdentification logUserIdentification)
        {
            var doiMatchingStopwatch = Stopwatch.StartNew();

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_ADD_TTV_DATA_PUBLICATIONS_BY_DOI,
                    state: LogContent.ActionState.START,
                    message: $"dim_user_profile.id={dimUserProfileId}"
                    ));

            // Get SQL statement for Doi matching
            string doiMatchingSql = _ttvSqlService.GetSqlQuery_Select_PublicationDoiMatching(dimUserProfileId);

            // Execute SQL statement
            List<PublicationDoiMatchingDTO> doiMatchingDTOs = new();
            try
            {
                doiMatchingDTOs = (await _ttvContext.Database.GetDbConnection().QueryAsync<PublicationDoiMatchingDTO>(doiMatchingSql)).ToList();
                if (doiMatchingDTOs.Count == 0)
                {
                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.PROFILE_ADD_TTV_DATA_PUBLICATIONS_BY_DOI,
                            state: LogContent.ActionState.COMPLETE,
                            message: $"dim_user_profile.id={dimUserProfileId}, nothing to add"
                            ));
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_ADD_TTV_DATA_PUBLICATIONS_BY_DOI,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: $"dim_user_profile.id={dimUserProfileId}, exception: {ex.ToString()}"));
                return;
            }
            
            // Add publications to user profile
            List<string> addedPublicationIds = new();
            try
            {
                // Get DimfieldDisplaySetting for publication
                int dimFieldDisplaySettingsId_publication = _ttvContext.DimFieldDisplaySettings.Where(
                    dfds => dfds.DimUserProfileId == dimUserProfileId && dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION
                ).Select(dfds => dfds.Id).First();

                foreach (PublicationDoiMatchingDTO dto in doiMatchingDTOs)
                {
                    // Skip if publication is actually different publication
                    if (_duplicateHandlerService.HasSameDoiButIsDifferentPublication(
                            publicationName: dto.DimProfileOnlyPublication_PublicationName,
                            ttvPublicationName: dto.DimPublication_PublicationName,
                            ttvPublicationTypeCode: dto.DimPublication_TypeCode))
                    {
                        continue;
                    }
                    FactFieldValue factFieldValuePublication = this.GetEmptyFactFieldValue();
                    factFieldValuePublication.DimUserProfileId = dimUserProfileId;
                    factFieldValuePublication.DimFieldDisplaySettingsId = dimFieldDisplaySettingsId_publication;
                    factFieldValuePublication.DimPublicationId = dto.DimPublication_Id;
                    factFieldValuePublication.DimRegisteredDataSourceId = _dataSourceHelperService.DimRegisteredDataSourceId_TTV; // Data source is TTV
                    factFieldValuePublication.Show = dto.FactFieldValues_Show; // Copy from ORCID publication
                    _ttvContext.FactFieldValues.Add(factFieldValuePublication);
                    addedPublicationIds.Add(dto.DimPublication_PublicationId);
                }
                await _ttvContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.PROFILE_ADD_TTV_DATA_PUBLICATIONS_BY_DOI,
                            state: LogContent.ActionState.FAILED,
                            error: true,
                            message: $"dim_user_profile.id={dimUserProfileId}), exception: {ex.ToString()}"));
                return;
            }

            doiMatchingStopwatch.Stop();

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_ADD_TTV_DATA_PUBLICATIONS_BY_DOI,
                    state: LogContent.ActionState.COMPLETE,
                    message: $"dim_user_profile.id={dimUserProfileId}, took {doiMatchingStopwatch.ElapsedMilliseconds}ms, added {addedPublicationIds.Count}: {string.Join(",", addedPublicationIds)}"
                    ));
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
                    Modified = currentDateTime,
                    AllowAllSubscriptions = false,
                    Hidden = false,
                    PublishNewOrcidData = false
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

            // DimUserChoices
            // Get available choices from DimReferencedata
            List<DimReferencedatum> userChoicesInDimReferenceData =
                await _ttvContext.DimReferencedata.Where(dr =>
                    dr.CodeScheme == Constants.ReferenceDataCodeSchemes.USER_CHOICES).ToListAsync();
            // To prevent duplicates, check what user choices already exist in the database for the user profile and only add missing ones.
            // Duplicates could be introduced if this method is called multiple times, for example, because of retries or error scenarios in the frontend.
            List<int> existingDimUserChoices_dimReferenceDataIds = await _ttvContext.DimUserChoices.Where(duc => duc.DimUserProfileId == dimUserProfile.Id)
                .Select(duc => duc.DimReferencedataIdAsUserChoiceLabelNavigation.Id).ToListAsync();

            foreach (DimReferencedatum userChoiceInDimReferenceData in userChoicesInDimReferenceData)
            {
                if (!existingDimUserChoices_dimReferenceDataIds.Contains(userChoiceInDimReferenceData.Id))
                {
                    DimUserChoice dimUserChoice = new()
                    {
                        UserChoiceValue = false,
                        DimUserProfileId = dimUserProfile.Id,
                        DimReferencedataIdAsUserChoiceLabelNavigation = userChoiceInDimReferenceData,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = _utilityService.GetCurrentDateTime(),
                        Modified = _utilityService.GetCurrentDateTime()
                    };
                    _ttvContext.DimUserChoices.Add(dimUserChoice);
                }
            }
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
         * Create object indincating data source. Used for every profile data item.
         */
        public ProfileEditorSource GetProfileEditorSource(ProfileDataFromSql p)
        {
            // Organization name translation
            NameTranslation nameTranslationSourceOrganization = _languageService.GetNameTranslation(
                nameFi: p.DimRegisteredDataSource_DimOrganization_NameFi,
                nameEn: p.DimRegisteredDataSource_DimOrganization_NameEn,
                nameSv: p.DimRegisteredDataSource_DimOrganization_NameSv
            );

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
            return profileEditorSource;
        }

        /*
         * Get profile data.
         */
        public async Task<ProfileEditorDataResponse> GetProfileData(int userprofileId, LogUserIdentification logUserIdentification, bool forElasticsearch = false)
        {
            ProfileEditorDataResponse profileDataResponse = new()
            {
                personal = new ProfileEditorDataPersonal()
                {
                    names = await _nameService.GetProfileEditorNames(userprofileId, forElasticsearch),
                    otherNames = await _nameService.GetProfileEditorOtherNames(userprofileId, forElasticsearch),
                    emails = await _emailService.GetProfileEditorEmails(userprofileId, forElasticsearch),
                    telephoneNumbers = await _telephoneNumberService.GetProfileEditorTelephoneNumbers(userprofileId, forElasticsearch),
                    webLinks = await _webLinkService.GetProfileEditorWebLinks(userprofileId, forElasticsearch),
                    keywords = await _keywordService.GetProfileEditorKeywords(userprofileId, forElasticsearch),
                    fieldOfSciences = new(), // These are currently not included in the profile data response.
                    researcherDescriptions = await _researcherDescriptionService.GetProfileEditorResearcherDescriptions(userprofileId, forElasticsearch),
                    externalIdentifiers = await _externalIdentifierService.GetProfileEditorExternalIdentifiers(userprofileId, forElasticsearch)
                },
                activity = new ProfileEditorDataActivity()
                {
                    educations = await _educationService.GetProfileEditorEducations(userprofileId, forElasticsearch),
                    affiliations = await _affiliationService.GetProfileEditorAffiliations(userprofileId, forElasticsearch),
                    publications = await _publicationService.GetProfileEditorPublications(userprofileId, forElasticsearch),
                    fundingDecisions = await _fundingDecisionService.GetProfileEditorFundingDecisions(userprofileId, forElasticsearch),
                    researchDatasets = await _researchDatasetService.GetProfileEditorResearchDatasets(userprofileId, forElasticsearch),
                    activitiesAndRewards = await _researchActivityService.GetProfileEditorActiviesAndRewards(userprofileId, forElasticsearch)
                },
                settings = await _settingsService.GetProfileSettings(userprofileId, forElasticsearch),
                cooperation = await _cooperationChoicesService.GetCooperationChoices(userprofileId, forElasticsearch),
                uniqueDataSources = await _uniqueDataSourcesService.GetUniqueDataSources(userprofileId, forElasticsearch)
            };
            return profileDataResponse;
        }

        /*
         * Delete profile data using Dapper
         * - get FactFieldValues
         * - delete FactFieldValues
         * - delete DimIdentifierlessData (children and parent)
         * - delete ORCID put codes from DimPid
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
                // Get certain related items, which are used to collect list of removable items.
                // After that delete database items using Dapper.
                List<FactFieldValue> factFieldValues =
                    await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId)
                        .Include(ffv => ffv.DimProfileOnlyDataset)
                            .ThenInclude(ds => ds.DimWebLinks)
                        .Include(ffv => ffv.DimProfileOnlyFundingDecision)
                            .ThenInclude(fd => fd.DimWebLinks)
                        .Include(ffv => ffv.DimProfileOnlyResearchActivity)
                            .ThenInclude(ra => ra.DimWebLinks)
                    .AsNoTracking().ToListAsync();

                // Open database connection
                await connection.OpenAsync();

                // For efficiency, rows from a table are deleted in on SQL statement.
                // Collect deletable IDs into lists.
                List<int> dimAffiliationIds = new();
                List<int> dimCompetenceIds = new();
                List<int> dimEducationIds = new();
                List<int> dimEmailAddrressIds = new();
                List<int> dimEventIds = new();
                List<int> dimFieldOfScienceIds = new();
                List<int> dimKeywordIds = new();
                List<long> dimNameIds = new();
                List<int> dimProfileOnlyDatasetIds = new();
                List<int> dimProfileOnlyFundingDecisionIds = new();
                List<int> dimProfileOnlyPublicationIds = new();
                List<int> dimProfileOnlyResearchActivityIds = new();
                List<int> dimPidIds = new();
                List<int> dimResearchActivityIds = new();
                List<int> dimResearchCommunityIds = new();
                List<int> dimResearcherDescriptionIds = new();
                List<int> dimResearcherToResearchCommunityIds = new();
                List<int> dimTelephoneNumberIds = new();
                List<int> dimWebLinkIds = new();
                List<int> dimIdentifierlessDataIds = new();

                // Begin transaction
                using var deleteTransaction = await _ttvContext.Database.BeginTransactionAsync();

                try
                {
                    // Delete fact_field_values
                    await ExecuteSqlQueryWithProfiling(
                        sql: _ttvSqlService.GetSqlQuery_Delete_FactFieldValues(userprofileId)
                    );

                    // Delete fact_field_values related items
                    foreach (FactFieldValue factFieldValue in factFieldValues)
                    {
                        // Not all related data should be automatically deleted
                        if (CanDeleteFactFieldValueRelatedData(factFieldValue))
                        {
                            // Collect IDs
                            if (factFieldValue.DimAffiliationId != -1) dimAffiliationIds.Add(factFieldValue.DimAffiliationId);
                            if (factFieldValue.DimCompetenceId != -1) dimCompetenceIds.Add(factFieldValue.DimCompetenceId);
                            if (factFieldValue.DimEducationId != -1) dimEducationIds.Add(factFieldValue.DimEducationId);
                            if (factFieldValue.DimEmailAddrressId != -1) dimEmailAddrressIds.Add(factFieldValue.DimEmailAddrressId);
                            if (factFieldValue.DimEventId != -1) dimEventIds.Add(factFieldValue.DimEventId);
                            if (factFieldValue.DimReferencedataFieldOfScienceId != -1) dimFieldOfScienceIds.Add(factFieldValue.DimReferencedataFieldOfScienceId);
                            if (factFieldValue.DimKeywordId != -1) dimKeywordIds.Add(factFieldValue.DimKeywordId);
                            if (factFieldValue.DimNameId != -1) dimNameIds.Add(factFieldValue.DimNameId);
                            if (factFieldValue.DimIdentifierlessDataId != -1) dimIdentifierlessDataIds.Add(factFieldValue.DimIdentifierlessDataId);
                            if (factFieldValue.DimProfileOnlyDatasetId != -1)
                            {
                                dimProfileOnlyDatasetIds.Add(factFieldValue.DimProfileOnlyDatasetId);
                                // Collect related DimWebLink IDs
                                foreach (DimWebLink dimWebLink in factFieldValue.DimProfileOnlyDataset.DimWebLinks)
                                {
                                    dimWebLinkIds.Add(dimWebLink.Id);
                                }
                            }
                            if (factFieldValue.DimProfileOnlyFundingDecisionId != -1)
                            {
                                dimProfileOnlyFundingDecisionIds.Add(factFieldValue.DimProfileOnlyFundingDecisionId);
                                // Collect related DimWebLink IDs
                                foreach (DimWebLink dimWebLink in factFieldValue.DimProfileOnlyFundingDecision.DimWebLinks)
                                {
                                    dimWebLinkIds.Add(dimWebLink.Id);
                                }
                            }
                            if (factFieldValue.DimProfileOnlyPublicationId != -1) dimProfileOnlyPublicationIds.Add(factFieldValue.DimProfileOnlyPublicationId);
                            if (factFieldValue.DimProfileOnlyResearchActivityId != -1)
                            {
                                dimProfileOnlyResearchActivityIds.Add(factFieldValue.DimProfileOnlyResearchActivityId);
                                // Collect related DimWebLink IDs
                                foreach (DimWebLink dimWebLink in factFieldValue.DimProfileOnlyResearchActivity.DimWebLinks)
                                {
                                    dimWebLinkIds.Add(dimWebLink.Id);
                                }
                            }
                            if (factFieldValue.DimPidId != -1) dimPidIds.Add(factFieldValue.DimPidId);
                            if (factFieldValue.DimPidIdOrcidPutCode != -1) dimPidIds.Add(factFieldValue.DimPidIdOrcidPutCode);
                            if (factFieldValue.DimResearchActivityId != -1) dimResearchActivityIds.Add(factFieldValue.DimResearchActivityId);
                            if (factFieldValue.DimResearchCommunityId != -1) dimResearchCommunityIds.Add(factFieldValue.DimResearchCommunityId);
                            if (factFieldValue.DimResearcherDescriptionId != -1) dimResearcherDescriptionIds.Add(factFieldValue.DimResearcherDescriptionId);
                            if (factFieldValue.DimResearcherToResearchCommunityId != -1) dimResearcherToResearchCommunityIds.Add(factFieldValue.DimResearcherToResearchCommunityId);
                            if (factFieldValue.DimTelephoneNumberId != -1) dimTelephoneNumberIds.Add(factFieldValue.DimTelephoneNumberId);
                            if (factFieldValue.DimWebLinkId != -1) dimWebLinkIds.Add(factFieldValue.DimWebLinkId);
                        }
                    }

                    // Delete web links
                    if (dimWebLinkIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimWebLinks(dimWebLinkIds)
                        );
                    }
                    // Delete affiliations
                    if (dimAffiliationIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimAffiliations(dimAffiliationIds)
                        );
                    }
                    // Delete competences
                    if (dimCompetenceIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimCompetences(dimCompetenceIds)
                        );
                    }
                    // Delete educations
                    if (dimEducationIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimEducations(dimEducationIds)
                        );
                    }
                    // Delete email addresses
                    if (dimEmailAddrressIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimEmailAddrresses(dimEmailAddrressIds)
                        );
                    }
                    // Delete events
                    if (dimEventIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimEvents(dimEventIds)
                        );
                    }
                    // Delete fields of science
                    if (dimFieldOfScienceIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimFieldsOfScience(dimFieldOfScienceIds)
                        );
                    }
                    // Delete Keywords
                    if (dimKeywordIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimKeyword(dimKeywordIds)
                        );
                    }
                    // Delete names
                    if (dimNameIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimNames(dimNameIds)
                        );
                    }
                    // Delete identifierless data
                    if (dimIdentifierlessDataIds.Count > 0)
                    {
                        // Delete children first
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimIdentifierlessData_Children(dimIdentifierlessDataIds)
                        );
                        // Then delete parents
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimIdentifierlessData_Parent(dimIdentifierlessDataIds)
                        );
                    }
                    // Delete profile only datasets
                    if (dimProfileOnlyDatasetIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimProfileOnlyDatasets(dimProfileOnlyDatasetIds)
                        );
                    }
                    // Delete profile only funding decisions
                    if (dimProfileOnlyFundingDecisionIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimProfileOnlyFundingDecisions(dimProfileOnlyFundingDecisionIds)
                        );
                    }
                    // Delete profile only publications
                    if (dimProfileOnlyPublicationIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimProfileOnlyPublications(dimProfileOnlyPublicationIds)
                        );
                    }
                    // Delete profile only research activities
                    if (dimProfileOnlyResearchActivityIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimProfileOnlyResearchActivities(dimProfileOnlyResearchActivityIds)
                        );
                    }
                    // Delete PIDs
                    if (dimPidIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimPids(dimPidIds)
                        );
                    }
                    // Delete research activities
                    if (dimResearchActivityIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimResearchActivities(dimResearchActivityIds)
                        );
                    }
                    // Delete research communities
                    if (dimResearchCommunityIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimResearchCommunities(dimResearchCommunityIds)
                        );
                    }
                    // Delete researcher descriptions
                    if (dimResearcherDescriptionIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimResearchDescriptions(dimResearcherDescriptionIds)
                        );
                    }
                    // Delete researcher to research communities
                    if (dimResearcherToResearchCommunityIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimResearcherToResearchCommunities(dimResearcherToResearchCommunityIds)
                        );
                    }
                    // Delete telephone numbers
                    if (dimTelephoneNumberIds.Count > 0)
                    {
                        await ExecuteSqlQueryWithProfiling(
                            sql: _ttvSqlService.GetSqlQuery_Delete_DimTelephoneNumbers(dimTelephoneNumberIds)
                        );
                    }
                    // Delete dim_field_display_settings
                    await ExecuteSqlQueryWithProfiling(
                        sql: _ttvSqlService.GetSqlQuery_Delete_DimFieldDisplaySettings(userprofileId)
                    );

                    // Delete br_granted_permissions
                    await ExecuteSqlQueryWithProfiling(
                        sql: _ttvSqlService.GetSqlQuery_Delete_BrGrantedPermissions(userprofileId)
                    );

                    // Delete dim_user_choices
                    await ExecuteSqlQueryWithProfiling(
                        sql: _ttvSqlService.GetSqlQuery_Delete_DimUserChoices(userprofileId)
                    );

                    // Delete dim_user_profile
                    await ExecuteSqlQueryWithProfiling(
                        sql: _ttvSqlService.GetSqlQuery_Delete_DimUserProfile(userprofileId)
                    );

                    // Commit transaction
                    deleteTransaction.Commit();
                }
                catch (Exception exceptionFromProfileDelete)
                {
                    // Log error from profile deletion
                    _logger.LogError($"Error deleting user profile (dim_user_profile.id={userprofileId}): " + exceptionFromProfileDelete.ToString());

                    // Rollback
                    _logger.LogInformation($"Try to rollback user profile deletion (dim_user_profile.id={userprofileId})");
                    try
                    {
                        deleteTransaction.Rollback();
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
         * Execute a raw SQL query and log the execution time for profiling purposes.
         * This method is useful for performance analysis and optimization.
         */
        private async Task ExecuteSqlQueryWithProfiling(string sql)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogDebug($"SQL query start: {sql}");
            await _ttvContext.Database.ExecuteSqlRawAsync(sql: sql);
            stopwatch.Stop();
            _logger.LogDebug($"SQL query complete, execution time: {stopwatch.ElapsedMilliseconds} ms");
        }

        /*
         * Check by dim_user_profile.id if user profile is published.
         * Logic: User profile is considered as published if
         *  1. it is not hidden
         *  2. more than 1 item has property show=true.
         * 
         * Property 'show' is checked from table fact_field_values       
         * In user profile, the name from ORCID has always show=1, hence the requirement "more than 1 item".
         */
        public async Task<bool> IsUserprofilePublished(int dimUserProfileId)
        {
            bool hidden = false;
            int publishedCount = 0;

            try
            {
                using (var connection = _ttvContext.Database.GetDbConnection())
                {
                    string hiddenSql = _ttvSqlService.GetSqlQuery_Select_GetHiddenInUserprofile(dimUserProfileId);
                    hidden = (await connection.QueryAsync<bool>(hiddenSql)).First();

                    if (hidden)
                    {
                        return false;
                    }
                    else
                    {
                        string publishedCountSql = _ttvSqlService.GetSqlQuery_Select_CountPublishedItemsInUserprofile(dimUserProfileId);
                        publishedCount = (await connection.QueryAsync<int>(publishedCountSql)).First();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return publishedCount > 1;
        }

        /*
         * Update profile in Elasticsearch
         */
        public async Task<bool> UpdateProfileInElasticsearch(string orcidId, int userprofileId, LogUserIdentification logUserIdentification, string logAction = LogContent.Action.ELASTICSEARCH_UPDATE)
        {
            bool isUserprofilePublished = await IsUserprofilePublished(userprofileId);
            if (!isUserprofilePublished)
            {
                // Profile is not published. Make sure it is deleted from Elasticsearch
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: logAction,
                        state: LogContent.ActionState.INITIALIZING,
                        message: "Profile is not published, delete from Elasticsearch"));

                await DeleteProfileFromElasticsearch(orcidId, logUserIdentification);
                return true;
            }

            // Profile is published. Update in Elasticsearch
            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: logAction,
                    state: LogContent.ActionState.INITIALIZING,
                    message: "Profile is published, update in Elasticsearch"));

            bool startBackgroudTaskResult = await _elasticsearchService.BackgroundUpdate(
                orcidId: orcidId,
                userprofileId: userprofileId,
                logUserIdentification: logUserIdentification,
                logAction: logAction);

            return startBackgroudTaskResult;
        }

        /*
         * Delete profile from Elasticsearch
         */
        public async Task<bool> DeleteProfileFromElasticsearch(string orcidId, LogUserIdentification logUserIdentification, string logAction = LogContent.Action.ELASTICSEARCH_DELETE)
        {
            bool startBackgroudTaskResult = await _elasticsearchService.BackgroundDelete(
                orcidId: orcidId,
                logUserIdentification: logUserIdentification,
                logAction: logAction);

            return startBackgroudTaskResult;
        }

        /*
         * Set profile state to "hidden".
         */
        public async Task HideProfile(string orcidId, LogUserIdentification logUserIdentification)
        {
            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_HIDE,
                    state: LogContent.ActionState.START));

            // Delete profile from Elasticsearch
            await DeleteProfileFromElasticsearch(orcidId, logUserIdentification);
        }

        /*
         * Reveal profile from state "hidden".
         */
        public async Task RevealProfile(string orcidId, LogUserIdentification logUserIdentification)
        {
            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_REVEAL,
                    state: LogContent.ActionState.START));

            // Update profile in Elasticsearch
            DimUserProfile dimUserProfile = await GetUserprofile(orcidId);
            await UpdateProfileInElasticsearch(orcidId: orcidId, userprofileId: dimUserProfile.Id, logUserIdentification: logUserIdentification);
        }

        /*
         * Get profile settings
         */
        public ProfileSettings GetProfileSettings(DimUserProfile dimUserProfile) {
            return new ProfileSettings
            {
                Hidden = dimUserProfile.Hidden,
                PublishNewData = dimUserProfile.PublishNewOrcidData,
                HighlightOpeness = dimUserProfile.HighlightOpeness
            };
        }

        /*
         * Save profile settings
         */
        public async Task SaveProfileSettings(string orcidId, DimUserProfile dimUserProfile, ProfileSettings profileSettings, LogUserIdentification logUserIdentification)
        {
            bool hiddenToggled = false;

            // Hidden
            if (profileSettings.Hidden != null)
            {
                dimUserProfile.Hidden = profileSettings.Hidden.Value;
                hiddenToggled = true;
            }
            // PublishNewData
            if (profileSettings.PublishNewData != null)
            {
                dimUserProfile.PublishNewOrcidData = profileSettings.PublishNewData.Value;
            }
            // HighlightOpeness
            if (profileSettings.HighlightOpeness != null)
            {
                dimUserProfile.HighlightOpeness = profileSettings.HighlightOpeness.Value;
            }

            // Save DimUserProfile changes before further processing
            await _ttvContext.SaveChangesAsync();

            // Change user profile visibility according to new settings
            if (hiddenToggled) {
                if (profileSettings.Hidden == true)
                {
                    // Hide profile
                    await HideProfile(orcidId: orcidId, logUserIdentification: logUserIdentification);
                }
                else if (profileSettings.Hidden == false)
                {
                    // Reveal profile
                    await RevealProfile(orcidId: orcidId, logUserIdentification: logUserIdentification);
                }
            }
        }

        /*
         * Returns memory cache key for user profile
         */
        public string GetCMemoryCacheKey_UserProfile(string orcidId)
        {
            return $"userprofile-{orcidId}";
        }

        /*
         * Returns memory cache key for profile settings
         */
        public string GetCMemoryCacheKey_ProfileSettings(string orcidId)
        {
            return $"profilesettings-{orcidId}";
        }

        /*
         * Returns memory cache key for user choices
         */
        public string GetCMemoryCacheKey_UserChoices(string orcidId)
        {
            return $"userchoices-{orcidId}";
        }

        /*
         * Returns memory cache key for share purposes
         */
        public string GetCMemoryCacheKey_SharePurposes()
        {
            return "share_purposes";
        }

        /*
         * Returns memory cache key for share permissions
         */
        public string GetCMemoryCacheKey_SharePermissions()
        {
            return "share_permissions";
        }

        /*
         * Returns memory cache key for given permissions
         */
        public string GetCMemoryCacheKey_GivenPermissions(string orcidId)
        {
            return $"given_permissions-{orcidId}";
        }

        /*
         * Execute raw sql.
         */
        public async Task ExecuteRawSql(string sql)
        {
            await ExecuteSqlQueryWithProfiling(sql: sql);
        }
    }
}
