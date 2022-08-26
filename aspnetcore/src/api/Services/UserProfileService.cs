using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Ttv;
using api.Models.ProfileEditor;
using Microsoft.EntityFrameworkCore;
using api.Models.Common;
using api.Models.Orcid;
using Dapper;
using System.Transactions;
using api.Controllers;
using Microsoft.Extensions.Logging;

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
        private readonly IOrganizationHandlerService _organizationHandlerService;
        private readonly ISharingService _sharingService;
        private readonly ITtvSqlService _ttvSqlService;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(TtvContext ttvContext,
            IDataSourceHelperService dataSourceHelperService,
            IUtilityService utilityService,
            ILanguageService languageService,
            IDuplicateHandlerService duplicateHandlerService,
            IOrganizationHandlerService organizationHandlerService,
            ISharingService sharingService,
            ITtvSqlService ttvSqlService,
            ILogger<UserProfileService> logger)
        {
            _ttvContext = ttvContext;
            _dataSourceHelperService = dataSourceHelperService;
            _utilityService = utilityService;
            _languageService = languageService;
            _duplicateHandlerService = duplicateHandlerService;
            _organizationHandlerService = organizationHandlerService;
            _sharingService = sharingService;
            _ttvSqlService = ttvSqlService;
            _logger = logger;
        }

        // For unit test
        public UserProfileService() { }

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
                Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID,
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
                DimOrcidPublicationId = -1,
                DimKeywordId = -1,
                DimAffiliationId = -1,
                DimResearcherToResearchCommunityId = -1,
                DimFieldOfScienceId = -1,
                DimResearchDatasetId = -1,
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
         * Get empty DimOrcidPublication.
         * Must use -1 in required foreign keys.
         */
        public DimOrcidPublication GetEmptyDimOrcidPublication()
        {
            return new DimOrcidPublication()
            {
                DimParentOrcidPublicationId = null,
                ParentPublicationTypeCode = -1,
                PublicationTypeCode = -1,
                PublicationTypeCode2 = -1,
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
                LanguageCode = null,
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
                OrcidPersonDataSource = -1,
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
                DimOrcidPublicationId = -1,
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.GetCurrentDateTime(),
                Modified = _utilityService.GetCurrentDateTime()
            };
        }

        /*
         * Search DimAffiliation items from TTV database and link them to user profile.
         */
        public void AddDimAffiliationToUserProfile(DimKnownPerson dimKnownPerson, DimUserProfile dimUserProfile)
        {
            DimFieldDisplaySetting dimFieldDisplaySetting_affiliation =
                dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_AFFILIATION).First();

            foreach (DimAffiliation dimAffiliation in dimKnownPerson.DimAffiliations.Where(dimAffiliation => dimAffiliation.DimRegisteredDataSourceId != -1))
            {
                FactFieldValue factFieldValueAffiliation = this.GetEmptyFactFieldValue();
                factFieldValueAffiliation.DimUserProfileId = dimUserProfile.Id;
                factFieldValueAffiliation.DimFieldDisplaySettingsId = dimFieldDisplaySetting_affiliation.Id;
                factFieldValueAffiliation.DimAffiliationId = dimAffiliation.Id;
                factFieldValueAffiliation.DimRegisteredDataSourceId = dimAffiliation.DimRegisteredDataSourceId;
                _ttvContext.FactFieldValues.Add(factFieldValueAffiliation);
            }
        }

        /*
         * Search DimEducation items from TTV database and link them to user profile.
         */
        public void AddDimEducationToUserProfile(DimKnownPerson dimKnownPerson, DimUserProfile dimUserProfile)
        {
            DimFieldDisplaySetting dimFieldDisplaySetting_education =
                dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_EDUCATION).First();

            foreach (DimEducation dimEducation in dimKnownPerson.DimEducations.Where(dimEducation => dimEducation.DimRegisteredDataSourceId != -1))
            {
                FactFieldValue factFieldValueEducation = this.GetEmptyFactFieldValue();
                factFieldValueEducation.DimUserProfileId = dimUserProfile.Id;
                factFieldValueEducation.DimFieldDisplaySettingsId = dimFieldDisplaySetting_education.Id;
                factFieldValueEducation.DimEducationId = dimEducation.Id;
                factFieldValueEducation.DimRegisteredDataSourceId = dimEducation.DimRegisteredDataSourceId;
                _ttvContext.FactFieldValues.Add(factFieldValueEducation);
            }
        }

        /*
         * Search DimResearcherDescription items from TTV database and link them to user profile.
         */
        public void AddDimResearcherDescriptionToUserProfile(DimKnownPerson dimKnownPerson, DimUserProfile dimUserProfile)
        {
            DimFieldDisplaySetting dimFieldDisplaySetting_researcher_description =
                dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION).First();

            foreach (DimResearcherDescription dimResearcherDescription in dimKnownPerson.DimResearcherDescriptions.Where(dimResearcherDescription => dimResearcherDescription.DimRegisteredDataSourceId != -1))
            {
                FactFieldValue factFieldValueResearcherDescription = this.GetEmptyFactFieldValue();
                factFieldValueResearcherDescription.DimUserProfileId = dimUserProfile.Id;
                factFieldValueResearcherDescription.DimFieldDisplaySettingsId = dimFieldDisplaySetting_researcher_description.Id;
                factFieldValueResearcherDescription.DimResearcherDescriptionId = dimResearcherDescription.Id;
                factFieldValueResearcherDescription.DimRegisteredDataSourceId = dimResearcherDescription.DimRegisteredDataSourceId;
                _ttvContext.FactFieldValues.Add(factFieldValueResearcherDescription);
            }
        }

        /*
         * Search DimEmailAddress items from TTV database and link them to user profile.
         */
        public void AddDimEmailAddressItemsToUserProfile(DimKnownPerson dimKnownPerson, DimUserProfile dimUserProfile)
        {
            DimFieldDisplaySetting dimFieldDisplaySetting_emailAddress =
                dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS).First();

            foreach (DimEmailAddrress dimEmailAddress in dimKnownPerson.DimEmailAddrresses.Where(dimEmailAddress => dimEmailAddress.DimRegisteredDataSourceId != -1))
            {
                FactFieldValue factFieldValueEmailAddress = this.GetEmptyFactFieldValue();
                factFieldValueEmailAddress.DimUserProfileId = dimUserProfile.Id;
                factFieldValueEmailAddress.DimFieldDisplaySettingsId = dimFieldDisplaySetting_emailAddress.Id;
                factFieldValueEmailAddress.DimEmailAddrressId = dimEmailAddress.Id;
                factFieldValueEmailAddress.DimRegisteredDataSourceId = dimEmailAddress.DimRegisteredDataSourceId;
                _ttvContext.FactFieldValues.Add(factFieldValueEmailAddress);
            }
        }

        /*
         * Search DimTelephoneNumber items from TTV database and link them to user profile.
         */
        public void AddDimTelephoneItemsToUserProfile(DimKnownPerson dimKnownPerson, DimUserProfile dimUserProfile)
        {
            DimFieldDisplaySetting dimFieldDisplaySetting_telephoneNumber =
               dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER).First();

            foreach (DimTelephoneNumber dimTelephoneNumber in dimKnownPerson.DimTelephoneNumbers.Where(dimTelephoneNumber => dimTelephoneNumber.DimRegisteredDataSourceId != -1))
            {
                FactFieldValue factFieldValueTelephoneNumber = this.GetEmptyFactFieldValue();
                factFieldValueTelephoneNumber.DimUserProfileId = dimUserProfile.Id;
                factFieldValueTelephoneNumber.DimFieldDisplaySettingsId = dimFieldDisplaySetting_telephoneNumber.Id;
                factFieldValueTelephoneNumber.DimTelephoneNumberId = dimTelephoneNumber.Id;
                factFieldValueTelephoneNumber.DimRegisteredDataSourceId = dimTelephoneNumber.DimRegisteredDataSourceId;
                _ttvContext.FactFieldValues.Add(factFieldValueTelephoneNumber);
            }
        }

        /*
         * Search FactContribution related items from TTV database and link them to user profile.
         */
        public void AddFactContributionItemsToUserProfile(DimKnownPerson dimKnownPerson, DimUserProfile dimUserProfile)
        {
            /*
             * Loop DimNames, then related FactContributions.
             * 
             *   DimKnownPerson
             *       => DimName
             *           => FactContribution
             *               => DimPublication
             *               => DimFundingDecision
             *               => DimResearchDataset
             *               => DimResearchActivity
             * 
             * NOTE! Registered data source must be taken from DimName.
             * Skip item if DimName does not have registered data source.
             */

            // DimFieldDisplaySetting for publications
            DimFieldDisplaySetting dimFieldDisplaySetting_publication =
                dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION).First();

            // DimFieldDisplaySetting for funding decisions
            DimFieldDisplaySetting dimFieldDisplaySetting_fundingDecision =
                dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION).First();

            // DimFieldDisplaySetting for research datasets
            DimFieldDisplaySetting dimFieldDisplaySetting_researchDataset =
                dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET).First();

            // DimFieldDisplaySetting for research activity
            DimFieldDisplaySetting dimFieldDisplaySetting_researchActivity =
                dimUserProfile.DimFieldDisplaySettings.Where(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_RESEARCH_ACTIVITY).First();

            // Loop DimNames, which have valid registered data source
            foreach (DimName dimName in dimKnownPerson.DimNames.Where(dimName => dimName.DimRegisteredDataSourceId != -1))
            {
                // Collect entity IDs into lists.
                List<int> publicationsIds = new();
                List<int> fundingDecisionIds = new();
                List<int> researchDatasetIds = new();
                List<int> researchActivityIds = new();

                // Loop FactContributions. Collect "non -1" values only.
                foreach (
                    FactContribution factContribution in dimName.FactContributions.Where(
                        fc => fc.DimPublicationId != -1 || fc.DimFundingDecisionId != -1 || fc.DimResearchDatasetId != -1 || fc.DimResearchActivityId != -1
                    )
                )
                {
                    // FactContribution is linked to DimPublication
                    if (factContribution.DimPublicationId != -1)
                    {
                        publicationsIds.Add(factContribution.DimPublicationId);
                    }

                    // FactContribution is linked to DimFundingDecision
                    if (factContribution.DimFundingDecisionId != -1)
                    {
                        fundingDecisionIds.Add(factContribution.DimFundingDecisionId);
                    }

                    // FactContribution is linked to DimResearchDataset
                    if (factContribution.DimResearchDatasetId != -1)
                    {
                        researchDatasetIds.Add(factContribution.DimResearchDatasetId);
                    }

                    // FactContribution is linked to DimResearchActivity
                    if (factContribution.DimResearchActivityId != -1)
                    {
                        researchActivityIds.Add(factContribution.DimResearchActivityId);
                    }
                }

                // Add FactFieldValues for DimPublication. Remove duplicate IDs.
                foreach (int publicationId in publicationsIds.Distinct())
                {
                    FactFieldValue factFieldValuePublication = this.GetEmptyFactFieldValue();
                    factFieldValuePublication.DimUserProfileId = dimUserProfile.Id;
                    factFieldValuePublication.DimFieldDisplaySettingsId = dimFieldDisplaySetting_publication.Id;
                    factFieldValuePublication.DimPublicationId = publicationId;
                    factFieldValuePublication.DimRegisteredDataSourceId = dimName.DimRegisteredDataSourceId;
                    _ttvContext.FactFieldValues.Add(factFieldValuePublication);
                }

                // Add FactFieldValues for DimFundingDecision. Remove duplicate IDs.
                foreach (int fundingDecisionId in fundingDecisionIds.Distinct())
                {
                    FactFieldValue factFieldValueFundingDecision = this.GetEmptyFactFieldValue();
                    factFieldValueFundingDecision.DimUserProfileId = dimUserProfile.Id;
                    factFieldValueFundingDecision.DimFieldDisplaySettingsId = dimFieldDisplaySetting_fundingDecision.Id;
                    factFieldValueFundingDecision.DimFundingDecisionId = fundingDecisionId;
                    factFieldValueFundingDecision.DimRegisteredDataSourceId = dimName.DimRegisteredDataSourceId;
                    _ttvContext.FactFieldValues.Add(factFieldValueFundingDecision);
                }

                // Add FactFieldValues for DimFundingDecision. Remove duplicate IDs.
                foreach (int researchDatasetId in researchDatasetIds.Distinct())
                {
                    FactFieldValue factFieldValueResearchDataset = this.GetEmptyFactFieldValue();
                    factFieldValueResearchDataset.DimUserProfileId = dimUserProfile.Id;
                    factFieldValueResearchDataset.DimFieldDisplaySettingsId = dimFieldDisplaySetting_researchDataset.Id;
                    factFieldValueResearchDataset.DimResearchDatasetId = researchDatasetId;
                    factFieldValueResearchDataset.DimRegisteredDataSourceId = dimName.DimRegisteredDataSourceId;
                    _ttvContext.FactFieldValues.Add(factFieldValueResearchDataset);
                }

                // Add FactFieldValues for DimResearchActivity. Remove duplicate IDs.
                foreach (int researchActivityId in researchActivityIds.Distinct())
                {
                    FactFieldValue factFieldValueResearchActivity = this.GetEmptyFactFieldValue();
                    factFieldValueResearchActivity.DimUserProfileId = dimUserProfile.Id;
                    factFieldValueResearchActivity.DimFieldDisplaySettingsId = dimFieldDisplaySetting_researchActivity.Id;
                    factFieldValueResearchActivity.DimResearchActivityId = researchActivityId;
                    factFieldValueResearchActivity.DimRegisteredDataSourceId = dimName.DimRegisteredDataSourceId;
                    _ttvContext.FactFieldValues.Add(factFieldValueResearchActivity);
                }
            }
        }

        /*
         * Search and add data from TTV database.
         * This is data that is already linked to the ORCID id in DimPid and it's related DimKnownPerson.
         */
        public void AddTtvDataToUserProfile(DimKnownPerson dimKnownPerson, DimUserProfile dimUserProfile)
        {
            // DimEmailAddress
            AddDimEmailAddressItemsToUserProfile(dimKnownPerson, dimUserProfile);

            // DimTelephoneNumber
            AddDimTelephoneItemsToUserProfile(dimKnownPerson, dimUserProfile);

            // DimAffiliation
            AddDimAffiliationToUserProfile(dimKnownPerson, dimUserProfile);

            // DimEducation
            AddDimEducationToUserProfile(dimKnownPerson, dimUserProfile);

            // DimResearcherDescription
            AddDimResearcherDescriptionToUserProfile(dimKnownPerson, dimUserProfile);

            // FactContribution
            AddFactContributionItemsToUserProfile(dimKnownPerson, dimUserProfile);
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
        public async Task CreateProfile(string orcidId)
        {
            // Get DimPid by ORCID ID.
            // Also get all related entities, that should be automatically included in profile.
            DimPid dimPid = await _ttvContext.DimPids
                // FactContribution
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimNames)
                        .ThenInclude(dn => dn.FactContributions).AsNoTracking()
                // DimName
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimNames).AsNoTracking()
                // DimAffiliation
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimAffiliations).AsNoTracking()
                // DimEducation
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimEducations).AsNoTracking()
                // DimReseacherDescription
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimResearcherDescriptions).AsNoTracking()
                // DimEmailAddress
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimEmailAddrresses).AsNoTracking()
                // DimTelephoneNumber
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimTelephoneNumbers).AsNoTracking()
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
                dimPid.DimKnownPerson = new DimKnownPerson()
                {
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = currentDateTime,
                    Modified = currentDateTime
                };

                _ttvContext.DimPids.Add(dimPid);
            }
            else if (dimPid.DimKnownPerson == null || dimPid.DimKnownPersonId == -1)
            {
                // DimPid was found but it does not have related DimKnownPerson, add new.
                DimKnownPerson kp = new()
                {
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = currentDateTime,
                    Modified = currentDateTime
                };
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

            // FactFieldValues - Search TTV database and add related entries into user profile.
            AddTtvDataToUserProfile(dimPid.DimKnownPerson, dimUserProfile);

            // Save FactFieldValues changes.
            await _ttvContext.SaveChangesAsync();
        }



        /*
         *  Get profile data.
         */
        public async Task<ProfileEditorDataResponse> GetProfileDataAsync(int userprofileId)
        {
            // Response data
            ProfileEditorDataResponse profileDataResponse = new() { };

            // Get SQL statement for profile data query
            string profileDataSql = _ttvSqlService.GetSqlQuery_ProfileData(userprofileId);

            // Execute SQL statement using Dapper
            var connection = _ttvContext.Database.GetDbConnection();
            List<ProfileDataRaw> profileDataRaws = (await connection.QueryAsync<ProfileDataRaw>(profileDataSql)).ToList();

            // Group response by DimRegisteredDataSource_Id
            IEnumerable<IGrouping<int, ProfileDataRaw>> profileDataRaw_GroupedBy_DataSourceId = profileDataRaws.GroupBy(p => p.DimRegisteredDataSource_Id);

            // Loop data source groups
            foreach (IGrouping<int, ProfileDataRaw> profileDataGroup in profileDataRaw_GroupedBy_DataSourceId)
            {
                // Set data source object. It can be taken from the first object in list, because all object in the group have the same data source.

                // Organization name translation
                NameTranslation nameTranslationSourceOrganization = _languageService.GetNameTranslation(
                    nameFi: profileDataGroup.First().DimRegisteredDataSource_DimOrganization_NameFi,
                    nameEn: profileDataGroup.First().DimRegisteredDataSource_DimOrganization_NameEn,
                    nameSv: profileDataGroup.First().DimRegisteredDataSource_DimOrganization_NameSv
                );

                // Source object containing registered data source and organization name.
                ProfileEditorSource profileEditorSource = new()
                {
                    Id = profileDataGroup.First().DimRegisteredDataSource_Id,
                    RegisteredDataSource = profileDataGroup.First().DimRegisteredDataSource_Name,
                    Organization = new Organization()
                    {
                        NameFi = nameTranslationSourceOrganization.NameFi,
                        NameEn = nameTranslationSourceOrganization.NameEn,
                        NameSv = nameTranslationSourceOrganization.NameSv
                    }
                };

                // Additionally, group the items by DimFieldDisplaySettings_FieldIdentifier.
                // FieldIdentifier indicates what type of data the field contains (name, other name, weblink, etc).
                IEnumerable<IGrouping<int, ProfileDataRaw>> profileDataGroups2 = profileDataGroup.GroupBy(q => q.DimFieldDisplaySettings_FieldIdentifier);

                // Loop field identifier groups.
                foreach (IGrouping<int, ProfileDataRaw> profileDataGroup2 in profileDataGroups2)
                {
                    // Data items are collected into own lists
                    List<ProfileEditorItemName> nameItems = new();
                    List<ProfileEditorItemName> otherNameItems = new();
                    List<ProfileEditorItemWebLink> weblinkItems = new();
                    List<ProfileEditorItemResearcherDescription> researcherDescriptionItems = new();
                    List<ProfileEditorItemEmail> emailItems = new();
                    List<ProfileEditorItemTelephoneNumber> telephoneNumberItems = new();
                    List<ProfileEditorItemFieldOfScience> fieldOfScienceItems = new();
                    List<ProfileEditorItemKeyword> keywordItems = new();
                    List<ProfileEditorItemExternalIdentifier> externalIdentifierItems = new();
                    List<ProfileEditorItemAffiliation> affiliationItems = new();
                    List<ProfileEditorItemEducation> educationItems = new();
                    List<ProfileEditorItemPublication> publicationItems = new();

                    // Loop items in a field identifier group
                    foreach (ProfileDataRaw profileData2 in profileDataGroup2)
                    {
                        // FieldIdentifier defines what type of data the field contains.
                        switch (profileData2.DimFieldDisplaySettings_FieldIdentifier)
                        {
                            // Name
                            case Constants.FieldIdentifiers.PERSON_NAME:
                                nameItems.Add(
                                    new ProfileEditorItemName()
                                    {
                                        FirstNames = profileData2.DimName_FirstNames,
                                        LastName = profileData2.DimName_LastName,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = profileData2.FactFieldValues_DimNameId,
                                            Type = Constants.FieldIdentifiers.PERSON_FIRST_NAMES,
                                            Show = profileData2.FactFieldValues_Show,
                                            PrimaryValue = profileData2.FactFieldValues_PrimaryValue
                                        }
                                    }
                                );
                                break;

                            // Other name
                            case Constants.FieldIdentifiers.PERSON_OTHER_NAMES:
                                otherNameItems.Add(
                                    new ProfileEditorItemName()
                                    {
                                        FullName = profileData2.DimName_FullName,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = profileData2.FactFieldValues_DimNameId,
                                            Type = Constants.FieldIdentifiers.PERSON_OTHER_NAMES,
                                            Show = profileData2.FactFieldValues_Show,
                                            PrimaryValue = profileData2.FactFieldValues_PrimaryValue
                                        }
                                    }
                                );
                                break;

                            // Web link
                            case Constants.FieldIdentifiers.PERSON_WEB_LINK:
                                weblinkItems.Add(
                                    new ProfileEditorItemWebLink()
                                    {
                                        Url = profileData2.DimWebLink_Url,
                                        LinkLabel = profileData2.DimWebLink_LinkLabel,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = profileData2.FactFieldValues_DimWebLinkId,
                                            Type = Constants.FieldIdentifiers.PERSON_WEB_LINK,
                                            Show = profileData2.FactFieldValues_Show,
                                            PrimaryValue = profileData2.FactFieldValues_PrimaryValue
                                        }
                                    }
                                );
                                break;

                            // ResearcherDescription
                            case Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION:
                                // Researcher description name translation
                                NameTranslation nameTranslationResearcherDescription = _languageService.GetNameTranslation(
                                    nameFi: profileData2.DimResearcherDescription_ResearchDescriptionFi,
                                    nameEn: profileData2.DimResearcherDescription_ResearchDescriptionEn,
                                    nameSv: profileData2.DimResearcherDescription_ResearchDescriptionSv
                                );
                                researcherDescriptionItems.Add(
                                    new ProfileEditorItemResearcherDescription()
                                    {
                                        ResearchDescriptionFi = nameTranslationResearcherDescription.NameFi,
                                        ResearchDescriptionEn = nameTranslationResearcherDescription.NameEn,
                                        ResearchDescriptionSv = nameTranslationResearcherDescription.NameSv,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = profileData2.FactFieldValues_DimResearcherDescriptionId,
                                            Type = Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION,
                                            Show = profileData2.FactFieldValues_Show,
                                            PrimaryValue = profileData2.FactFieldValues_PrimaryValue
                                        }
                                    }
                                );
                                break;

                            // Email
                            case Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS:
                                emailItems.Add(
                                    new ProfileEditorItemEmail()
                                    {
                                        Value = profileData2.DimEmailAddrress_Email,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = profileData2.FactFieldValues_DimEmailAddrressId,
                                            Type = Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS,
                                            Show = profileData2.FactFieldValues_Show,
                                            PrimaryValue = profileData2.FactFieldValues_PrimaryValue
                                        }
                                    }
                                );
                                break;

                            // Telephone number
                            case Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER:
                                telephoneNumberItems.Add(
                                    new ProfileEditorItemTelephoneNumber()
                                    {
                                        Value = profileData2.DimTelephoneNumber_TelephoneNumber,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = profileData2.FactFieldValues_DimTelephoneNumberId,
                                            Type = Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER,
                                            Show = profileData2.FactFieldValues_Show,
                                            PrimaryValue = profileData2.FactFieldValues_PrimaryValue
                                        }
                                    }
                                );
                                break;

                            // Field of science
                            case Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE:
                                // Field of science name translation
                                NameTranslation nameTranslationFieldOfScience = _languageService.GetNameTranslation(
                                    nameFi: profileData2.DimFieldOfScience_NameFi,
                                    nameEn: profileData2.DimFieldOfScience_NameEn,
                                    nameSv: profileData2.DimFieldOfScience_NameSv
                                );

                                fieldOfScienceItems.Add(
                                    new ProfileEditorItemFieldOfScience()
                                    {
                                        NameFi = nameTranslationFieldOfScience.NameFi,
                                        NameEn = nameTranslationFieldOfScience.NameEn,
                                        NameSv = nameTranslationFieldOfScience.NameSv,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = profileData2.FactFieldValues_DimFieldOfScienceId,
                                            Type = Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE,
                                            Show = profileData2.FactFieldValues_Show,
                                            PrimaryValue = profileData2.FactFieldValues_PrimaryValue
                                        }
                                    }
                                );
                                break;

                            // Keyword
                            case Constants.FieldIdentifiers.PERSON_KEYWORD:
                                keywordItems.Add(
                                    new ProfileEditorItemKeyword()
                                    {
                                        Value = profileData2.DimKeyword_Keyword,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = profileData2.FactFieldValues_DimKeywordId,
                                            Type = Constants.FieldIdentifiers.PERSON_KEYWORD,
                                            Show = profileData2.FactFieldValues_Show,
                                            PrimaryValue = profileData2.FactFieldValues_PrimaryValue
                                        }
                                    }
                                );
                                break;

                            // External identifier
                            case Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER:
                                externalIdentifierItems.Add(
                                    new ProfileEditorItemExternalIdentifier()
                                    {
                                        PidContent = profileData2.DimPid_PidContent,
                                        PidType = profileData2.DimPid_PidType,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = profileData2.FactFieldValues_DimPidId,
                                            Type = Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER,
                                            Show = profileData2.FactFieldValues_Show,
                                            PrimaryValue = profileData2.FactFieldValues_PrimaryValue
                                        }
                                    }
                                );
                                break;

                            // Affiliation
                            case Constants.FieldIdentifiers.ACTIVITY_AFFILIATION:
                                // Get affiliation organization name from related DimOrganization (ffv.DimAffiliation.DimOrganization), if exists.
                                // Otherwise from DimIdentifierlessData (ffv.DimIdentifierlessData).
                                // Name translation service ensures that none of the language fields is empty.
                                NameTranslation nameTranslationAffiliationOrganization = new();
                                if (profileData2.DimAffiliation_DimOrganization_Id > 0)
                                {
                                    nameTranslationAffiliationOrganization = _languageService.GetNameTranslation(
                                        nameFi: profileData2.DimAffiliation_DimOrganization_NameFi,
                                        nameEn: profileData2.DimAffiliation_DimOrganization_NameEn,
                                        nameSv: profileData2.DimAffiliation_DimOrganization_NameSv
                                    );
                                }
                                else if (profileData2.FactFieldValues_DimIdentifierlessDataId > -1 &&
                                    profileData2.DimIdentifierlessData_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                                {
                                    nameTranslationAffiliationOrganization = _languageService.GetNameTranslation(
                                        nameFi: profileData2.DimIdentifierlessData_ValueFi,
                                        nameEn: profileData2.DimIdentifierlessData_ValueEn,
                                        nameSv: profileData2.DimIdentifierlessData_ValueSv
                                    );
                                }

                                // Name translation for position name
                                NameTranslation nameTranslationPositionName = _languageService.GetNameTranslation(
                                    nameFi: profileData2.DimAffiliation_PositionNameFi,
                                    nameEn: profileData2.DimAffiliation_PositionNameEn,
                                    nameSv: profileData2.DimAffiliation_PositionNameSv
                                );

                                // Name translation for department name
                                NameTranslation nameTranslationAffiliationDepartment = new();
                                if (profileData2.DimIdentifierlessData_Type != null && profileData2.DimIdentifierlessData_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                                {
                                    nameTranslationAffiliationDepartment = _languageService.GetNameTranslation(
                                        nameFi: profileData2.DimIdentifierlessData_ValueFi,
                                        nameEn: profileData2.DimIdentifierlessData_ValueEn,
                                        nameSv: profileData2.DimIdentifierlessData_ValueSv
                                    );
                                }
                                else if (profileData2.DimIdentifierlessData_Child_Type != null && profileData2.DimIdentifierlessData_Child_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                                {
                                    nameTranslationAffiliationDepartment = _languageService.GetNameTranslation(
                                        nameFi: profileData2.DimIdentifierlessData_Child_ValueFi,
                                        nameEn: profileData2.DimIdentifierlessData_Child_ValueEn,
                                        nameSv: profileData2.DimIdentifierlessData_Child_ValueSv
                                    );
                                }

                                affiliationItems.Add(
                                    new()
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
                                        Type = profileData2.DimAffiliation_DimReferenceData_NameFi,
                                        StartDate = new ProfileEditorItemDate()
                                        {
                                            Year = profileData2.DimAffiliation_StartDate_Year,
                                            Month = profileData2.DimAffiliation_StartDate_Month,
                                            Day = profileData2.DimAffiliation_StartDate_Day
                                        },
                                        EndDate = new ProfileEditorItemDate()
                                        {
                                            Year = profileData2.DimAffiliation_EndDate_Year,
                                            Month = profileData2.DimAffiliation_EndDate_Month,
                                            Day = profileData2.DimAffiliation_EndDate_Day
                                        },
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = profileData2.FactFieldValues_DimAffiliationId,
                                            Type = Constants.FieldIdentifiers.ACTIVITY_AFFILIATION,
                                            Show = profileData2.FactFieldValues_Show,
                                            PrimaryValue = profileData2.FactFieldValues_PrimaryValue
                                        }
                                    }
                                );
                                break;

                            // Education
                            case Constants.FieldIdentifiers.ACTIVITY_EDUCATION:
                                // Name translation service ensures that none of the language fields is empty.
                                NameTranslation nameTraslationEducation = _languageService.GetNameTranslation(
                                    nameFi: profileData2.DimEducation_NameFi,
                                    nameEn: profileData2.DimEducation_NameEn,
                                    nameSv: profileData2.DimEducation_NameSv
                                );

                                educationItems.Add(
                                    new()
                                    {
                                        NameFi = nameTraslationEducation.NameFi,
                                        NameEn = nameTraslationEducation.NameEn,
                                        NameSv = nameTraslationEducation.NameSv,
                                        DegreeGrantingInstitutionName = profileData2.DimEducation_DegreeGrantingInstitutionName,
                                        StartDate = new ProfileEditorItemDate()
                                        {
                                            Year = profileData2.DimEducation_StartDate_Year,
                                            Month = profileData2.DimEducation_StartDate_Month,
                                            Day = profileData2.DimEducation_StartDate_Day
                                        },
                                        EndDate = new ProfileEditorItemDate()
                                        {
                                            Year = profileData2.DimEducation_EndDate_Year,
                                            Month = profileData2.DimEducation_EndDate_Month,
                                            Day = profileData2.DimEducation_EndDate_Day
                                        },
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = profileData2.FactFieldValues_DimEducationId,
                                            Type = Constants.FieldIdentifiers.ACTIVITY_EDUCATION,
                                            Show = profileData2.FactFieldValues_Show,
                                            PrimaryValue = profileData2.FactFieldValues_PrimaryValue
                                        }
                                    }
                                );
                                break;

                            // Publication
                            case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION:
                                publicationItems.Add(
                                    new ProfileEditorItemPublication()
                                    {
                                        PublicationId = profileData2.DimPublication_PublicationId,
                                        PublicationName = profileData2.DimPublication_PublicationName,
                                        PublicationYear = profileData2.DimPublication_PublicationYear,
                                        Doi = profileData2.DimPublication_Doi,
                                        TypeCode = profileData2.DimPublication_PublicationTypeCode,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = profileData2.FactFieldValues_DimPublicationId,
                                            Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                                            Show = profileData2.FactFieldValues_Show,
                                            PrimaryValue = profileData2.FactFieldValues_PrimaryValue
                                        }
                                    }
                                );

                                break;

                            // Publication(ORCID)
                            case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID:
                                publicationItems.Add(
                                    new ProfileEditorItemPublication()
                                    {
                                        PublicationId = profileData2.DimOrcidPublication_PublicationId,
                                        PublicationName = profileData2.DimOrcidPublication_PublicationName,
                                        PublicationYear = profileData2.DimOrcidPublication_PublicationYear,
                                        Doi = profileData2.DimOrcidPublication_Doi,
                                        TypeCode = "",
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = profileData2.FactFieldValues_DimOrcidPublicationId,
                                            Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID,
                                            Show = profileData2.FactFieldValues_Show,
                                            PrimaryValue = profileData2.FactFieldValues_PrimaryValue
                                        }
                                    }
                                );
                                break;
                        }
                    }

                    // Name groups
                    if (nameItems.Count > 0)
                    {
                        profileDataResponse.personal.nameGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = nameItems,
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = profileDataGroup2.First().DimFieldDisplaySettings_Id,
                                    Type = Constants.FieldIdentifiers.PERSON_NAME,
                                    Show = profileDataGroup2.First().DimFieldDisplaySettings_Show
                                }
                            }
                        );
                    }

                    // Other name groups
                    if (otherNameItems.Count > 0)
                    {
                        profileDataResponse.personal.otherNameGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = otherNameItems,
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = profileDataGroup2.First().DimFieldDisplaySettings_Id,
                                    Type = Constants.FieldIdentifiers.PERSON_OTHER_NAMES,
                                    Show = profileDataGroup2.First().DimFieldDisplaySettings_Show
                                }
                            }
                        );
                    }

                    // Web link groups
                    if (weblinkItems.Count > 0)
                    {
                        profileDataResponse.personal.webLinkGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = weblinkItems,
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = profileDataGroup2.First().DimFieldDisplaySettings_Id,
                                    Type = Constants.FieldIdentifiers.PERSON_WEB_LINK,
                                    Show = profileDataGroup2.First().DimFieldDisplaySettings_Show
                                }
                            }
                        );
                    }

                    // Researcher description groups
                    if (researcherDescriptionItems.Count > 0)
                    {
                        profileDataResponse.personal.researcherDescriptionGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = researcherDescriptionItems,
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = profileDataGroup2.First().DimFieldDisplaySettings_Id,
                                    Type = Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION,
                                    Show = profileDataGroup2.First().DimFieldDisplaySettings_Show
                                }
                            }
                        );
                    }

                    // Email groups
                    if (emailItems.Count > 0)
                    {
                        profileDataResponse.personal.emailGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = emailItems,
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = profileDataGroup2.First().DimFieldDisplaySettings_Id,
                                    Type = Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS,
                                    Show = profileDataGroup2.First().DimFieldDisplaySettings_Show
                                }
                            }
                        );
                    }

                    // Telephone number groups
                    if (telephoneNumberItems.Count > 0)
                    {
                        profileDataResponse.personal.telephoneNumberGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = telephoneNumberItems,
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = profileDataGroup2.First().DimFieldDisplaySettings_Id,
                                    Type = Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER,
                                    Show = profileDataGroup2.First().DimFieldDisplaySettings_Show
                                }
                            }
                        );
                    }

                    // Field of science groups
                    if (fieldOfScienceItems.Count > 0)
                    {
                        profileDataResponse.personal.fieldOfScienceGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = fieldOfScienceItems,
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = profileDataGroup2.First().DimFieldDisplaySettings_Id,
                                    Type = Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE,
                                    Show = profileDataGroup2.First().DimFieldDisplaySettings_Show
                                }
                            }
                        );
                    }

                    // Keyword groups
                    if (keywordItems.Count > 0)
                    {
                        profileDataResponse.personal.keywordGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = keywordItems,
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = profileDataGroup2.First().DimFieldDisplaySettings_Id,
                                    Type = Constants.FieldIdentifiers.PERSON_KEYWORD,
                                    Show = profileDataGroup2.First().DimFieldDisplaySettings_Show
                                }
                            }
                        );
                    }

                    // External identifier groups
                    if (externalIdentifierItems.Count > 0)
                    {
                        profileDataResponse.personal.externalIdentifierGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = externalIdentifierItems,
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = profileDataGroup2.First().DimFieldDisplaySettings_Id,
                                    Type = Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER,
                                    Show = profileDataGroup2.First().DimFieldDisplaySettings_Show
                                }
                            }
                        );
                    }

                    // Affiliation groups
                    if (affiliationItems.Count > 0)
                    {
                        profileDataResponse.activity.affiliationGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = affiliationItems,
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = profileDataGroup2.First().DimFieldDisplaySettings_Id,
                                    Type = Constants.FieldIdentifiers.ACTIVITY_AFFILIATION,
                                    Show = profileDataGroup2.First().DimFieldDisplaySettings_Show
                                }
                            }
                        );
                    }

                    // Education groups
                    if (educationItems.Count > 0)
                    {
                        profileDataResponse.activity.educationGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = educationItems,
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = profileDataGroup2.First().DimFieldDisplaySettings_Id,
                                    Type = Constants.FieldIdentifiers.ACTIVITY_EDUCATION,
                                    Show = profileDataGroup2.First().DimFieldDisplaySettings_Show
                                }
                            }
                        );
                    }

                    // Publication groups
                    if (publicationItems.Count > 0)
                    {
                        profileDataResponse.activity.publicationGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = publicationItems,
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = profileDataGroup2.First().DimFieldDisplaySettings_Id,
                                    Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                                    Show = profileDataGroup2.First().DimFieldDisplaySettings_Show
                                }
                            }
                        );
                    }
                }
            }

            return profileDataResponse;
        }

        /*
         *  Get profile data. New version using different data structure.
         */
        public async Task<ProfileEditorDataResponse2> GetProfileDataAsync2(int userprofileId)
        {
            // Get DimFieldDisplaySettings and related entities
            List<DimFieldDisplaySetting> dimFieldDisplaySettings = await _ttvContext.DimFieldDisplaySettings.TagWith("Get profile data").Where(dfds => dfds.DimUserProfileId == userprofileId && dfds.FactFieldValues.Count() > 0)
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimRegisteredDataSource)
                        .ThenInclude(drds => drds.DimOrganization).AsNoTracking()
                // DimName
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimName).AsNoTracking()
                // DimWebLink
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimWebLink).AsNoTracking()
                // DimFundingDecision
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                        .ThenInclude(dfd => dfd.DimOrganizationIdFunderNavigation).AsNoTracking() // DimFundingDecision related DimOrganization (funder organization)
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                        .ThenInclude(dfd => dfd.DimDateIdStartNavigation).AsNoTracking() // DimFundingDecision related start date (DimDate)
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                        .ThenInclude(dfd => dfd.DimDateIdEndNavigation).AsNoTracking() // DimFundingDecision related end date (DimDate)
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                        .ThenInclude(dfd => dfd.DimTypeOfFunding).AsNoTracking() // DimFundingDecision related DimTypeOfFunding
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                        .ThenInclude(dfd => dfd.DimCallProgramme).AsNoTracking() // DimFundingDecision related DimCallProgramme
                                                                                 // DimPublication
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPublication).AsNoTracking()
                // DimPid
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPid).AsNoTracking()
                // DimPidIdOrcidPutCodeNavigation
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPidIdOrcidPutCodeNavigation).AsNoTracking()
                // DimResearchActivity
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearchActivity).AsNoTracking()
                // DimEvent
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEvent).AsNoTracking()
                // DimEducation
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEducation)
                        .ThenInclude(de => de.DimStartDateNavigation).AsNoTracking()
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEducation)
                        .ThenInclude(de => de.DimEndDateNavigation).AsNoTracking()
                // DimAffiliation
                .Include(dfds => dfds.FactFieldValues)
                   .ThenInclude(ffv => ffv.DimAffiliation)
                       .ThenInclude(da => da.StartDateNavigation).AsNoTracking()
                .Include(dfds => dfds.FactFieldValues)
                   .ThenInclude(ffv => ffv.DimAffiliation)
                       .ThenInclude(da => da.EndDateNavigation).AsNoTracking()
                .Include(dfds => dfds.FactFieldValues)
                   .ThenInclude(ffv => ffv.DimAffiliation)
                       .ThenInclude(da => da.DimOrganization).AsNoTracking()
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimAffiliation)
                        .ThenInclude(da => da.AffiliationTypeNavigation).AsNoTracking()
                // DimCompetence
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimCompetence).AsNoTracking()
                // DimResearchCommunity
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearchCommunity).AsNoTracking()
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearcherToResearchCommunity)
                        .ThenInclude(drtrc => drtrc.DimResearchCommunity).AsNoTracking()
                // DimTelephoneNumber
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimTelephoneNumber).AsNoTracking()
                // DimEmailAddrress
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEmailAddrress).AsNoTracking()
                // DimResearcherDescription
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearcherDescription).AsNoTracking()
                // DimIdentifierlessData. Can have a child entity.
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimIdentifierlessData)
                        .ThenInclude(did => did.InverseDimIdentifierlessData).AsNoTracking()
                // DimOrcidPublication
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimOrcidPublication).AsNoTracking()
                // DimKeyword
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimKeyword).AsNoTracking()
                // DimFieldOfScience
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFieldOfScience).AsNoTracking()
                // DimResearchDataset
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearchDataset).AsNoTracking()
                //.ThenInclude(drd => drd.FactContributions) // FactContribution related to DimResearchDataset
                //.ThenInclude(fc => fc.DimName).AsNoTracking() // DimName related to FactContribution
                //.Include(dfds => dfds.FactFieldValues)
                //    .ThenInclude(ffv => ffv.DimResearchDataset)
                //        .ThenInclude(drd => drd.FactContributions) // FactContribution related to DimResearchDataset
                //            .ThenInclude(fc => fc.DimReferencedataActorRole).AsNoTracking() // DimName related to DimReferencedataActorRole
                .ToListAsync();

            ProfileEditorDataResponse2 profileDataResponse = new() { };

            // Collect data from DimFieldDisplaySettings and FactFieldValues entities
            foreach (DimFieldDisplaySetting dfds in dimFieldDisplaySettings)
            {
                // Group FactFieldValues by DimRegisteredDataSourceId
                IEnumerable<IGrouping<int, FactFieldValue>> factFieldValues_GroupedBy_DataSourceId = dfds.FactFieldValues.GroupBy(ffv => ffv.DimRegisteredDataSourceId);

                // Loop groups
                foreach (IGrouping<int, FactFieldValue> factFieldValueGroup in factFieldValues_GroupedBy_DataSourceId)
                {
                    DimRegisteredDataSource dimRegisteredDataSource = factFieldValueGroup.First().DimRegisteredDataSource;

                    // Organization name translation
                    NameTranslation nameTranslationSourceOrganization = _languageService.GetNameTranslation(
                        nameFi: dimRegisteredDataSource.DimOrganization.NameFi,
                        nameEn: dimRegisteredDataSource.DimOrganization.NameEn,
                        nameSv: dimRegisteredDataSource.DimOrganization.NameSv
                    );

                    // Source object containing registered data source and organization name.
                    ProfileEditorSource profileEditorSource = new()
                    {
                        Id = factFieldValueGroup.Key, // Key = registered data source id
                        RegisteredDataSource = dimRegisteredDataSource.Name,
                        Organization = new Organization()
                        {
                            NameFi = nameTranslationSourceOrganization.NameFi,
                            NameEn = nameTranslationSourceOrganization.NameEn,
                            NameSv = nameTranslationSourceOrganization.NameSv
                        }
                    };


                    // FieldIdentifier defines what type of data the field contains.
                    switch (dfds.FieldIdentifier)
                    {
                        // Name
                        case Constants.FieldIdentifiers.PERSON_NAME:
                            ProfileEditorGroupName nameGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ProfileEditorItemName>() { },
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = dfds.Id,
                                    Type = Constants.FieldIdentifiers.PERSON_NAME,
                                    Show = dfds.Show
                                }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                nameGroup.items.Add(
                                    new ProfileEditorItemName()
                                    {
                                        FirstNames = ffv.DimName.FirstNames,
                                        LastName = ffv.DimName.LastName,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = ffv.DimNameId,
                                            Type = Constants.FieldIdentifiers.PERSON_FIRST_NAMES,
                                            Show = ffv.Show,
                                            PrimaryValue = ffv.PrimaryValue
                                        }
                                    }
                                );
                            }
                            if (nameGroup.items.Count > 0)
                            {
                                profileDataResponse.personal.nameGroups.Add(nameGroup);
                            }
                            break;

                        // Other name
                        case Constants.FieldIdentifiers.PERSON_OTHER_NAMES:
                            ProfileEditorGroupOtherName otherNameGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ProfileEditorItemName>() { },
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = dfds.Id,
                                    Type = Constants.FieldIdentifiers.PERSON_OTHER_NAMES,
                                    Show = dfds.Show
                                }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                otherNameGroup.items.Add(
                                    new ProfileEditorItemName()
                                    {
                                        FullName = ffv.DimName.FullName,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = ffv.DimNameId,
                                            Type = Constants.FieldIdentifiers.PERSON_OTHER_NAMES,
                                            Show = ffv.Show,
                                            PrimaryValue = ffv.PrimaryValue
                                        }
                                    }
                                );
                            }
                            if (otherNameGroup.items.Count > 0)
                            {
                                profileDataResponse.personal.otherNameGroups.Add(otherNameGroup);
                            }
                            break;

                        // Researcher description
                        case Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION:
                            ProfileEditorGroupResearcherDescription researcherDescriptionGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ProfileEditorItemResearcherDescription>() { },
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = dfds.Id,
                                    Type = Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION,
                                    Show = dfds.Show
                                }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                researcherDescriptionGroup.items.Add(
                                    new ProfileEditorItemResearcherDescription()
                                    {
                                        ResearchDescriptionEn = ffv.DimResearcherDescription.ResearchDescriptionEn,
                                        ResearchDescriptionFi = ffv.DimResearcherDescription.ResearchDescriptionFi,
                                        ResearchDescriptionSv = ffv.DimResearcherDescription.ResearchDescriptionSv,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = ffv.DimResearcherDescriptionId,
                                            Type = Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION,
                                            Show = ffv.Show,
                                            PrimaryValue = ffv.PrimaryValue
                                        }
                                    }
                                );
                            }
                            if (researcherDescriptionGroup.items.Count > 0)
                            {
                                profileDataResponse.personal.researcherDescriptionGroups.Add(researcherDescriptionGroup);
                            }
                            break;

                        // Web link
                        case Constants.FieldIdentifiers.PERSON_WEB_LINK:
                            ProfileEditorGroupWebLink webLinkGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ProfileEditorItemWebLink>() { },
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = dfds.Id,
                                    Type = Constants.FieldIdentifiers.PERSON_WEB_LINK,
                                    Show = dfds.Show
                                }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                webLinkGroup.items.Add(
                                    new ProfileEditorItemWebLink()
                                    {
                                        Url = ffv.DimWebLink.Url,
                                        LinkLabel = ffv.DimWebLink.LinkLabel,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = ffv.DimWebLinkId,
                                            Type = Constants.FieldIdentifiers.PERSON_WEB_LINK,
                                            Show = ffv.Show,
                                            PrimaryValue = ffv.PrimaryValue
                                        }
                                    }
                                );
                            }
                            if (webLinkGroup.items.Count > 0)
                            {
                                profileDataResponse.personal.webLinkGroups.Add(webLinkGroup);
                            }
                            break;

                        // Email address
                        case Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS:
                            ProfileEditorGroupEmail emailGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ProfileEditorItemEmail>() { },
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = dfds.Id,
                                    Type = Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS,
                                    Show = dfds.Show
                                }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                emailGroup.items.Add(
                                    new ProfileEditorItemEmail()
                                    {
                                        Value = ffv.DimEmailAddrress.Email,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = ffv.DimEmailAddrressId,
                                            Type = Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS,
                                            Show = ffv.Show,
                                            PrimaryValue = ffv.PrimaryValue
                                        }
                                    }
                                );
                            }
                            if (emailGroup.items.Count > 0)
                            {
                                profileDataResponse.personal.emailGroups.Add(emailGroup);
                            }
                            break;

                        // Telephone number
                        case Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER:
                            ProfileEditorGroupTelephoneNumber telephoneNumberGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ProfileEditorItemTelephoneNumber>() { },
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = dfds.Id,
                                    Type = Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER,
                                    Show = dfds.Show
                                }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                telephoneNumberGroup.items.Add(
                                    new ProfileEditorItemTelephoneNumber()
                                    {
                                        Value = ffv.DimTelephoneNumber.TelephoneNumber,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = ffv.DimTelephoneNumberId,
                                            Type = Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER,
                                            Show = ffv.Show,
                                            PrimaryValue = ffv.PrimaryValue
                                        }
                                    }
                                );
                            }
                            if (telephoneNumberGroup.items.Count > 0)
                            {
                                profileDataResponse.personal.telephoneNumberGroups.Add(telephoneNumberGroup);
                            }
                            break;

                        // Field of science
                        case Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE:
                            ProfileEditorGroupFieldOfScience fieldOfScienceGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ProfileEditorItemFieldOfScience>() { },
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = dfds.Id,
                                    Type = Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE,
                                    Show = dfds.Show
                                }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                // Name translation service ensures that none of the language fields is empty.
                                NameTranslation nameTranslationFieldOfScience = _languageService.GetNameTranslation(
                                    nameFi: ffv.DimFieldOfScience.NameFi,
                                    nameEn: ffv.DimFieldOfScience.NameEn,
                                    nameSv: ffv.DimFieldOfScience.NameSv
                                );

                                fieldOfScienceGroup.items.Add(
                                    new ProfileEditorItemFieldOfScience()
                                    {
                                        NameFi = nameTranslationFieldOfScience.NameFi,
                                        NameEn = nameTranslationFieldOfScience.NameEn,
                                        NameSv = nameTranslationFieldOfScience.NameSv,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = ffv.DimFieldOfScienceId,
                                            Type = Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE,
                                            Show = ffv.Show,
                                            PrimaryValue = ffv.PrimaryValue
                                        }
                                    }
                                );
                            }
                            if (fieldOfScienceGroup.items.Count > 0)
                            {
                                profileDataResponse.personal.fieldOfScienceGroups.Add(fieldOfScienceGroup);
                            }
                            break;

                        // Keyword
                        case Constants.FieldIdentifiers.PERSON_KEYWORD:
                            ProfileEditorGroupKeyword keywordGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ProfileEditorItemKeyword>() { },
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = dfds.Id,
                                    Type = Constants.FieldIdentifiers.PERSON_KEYWORD,
                                    Show = dfds.Show
                                }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                keywordGroup.items.Add(
                                    new ProfileEditorItemKeyword()
                                    {
                                        Value = ffv.DimKeyword.Keyword,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = ffv.DimKeywordId,
                                            Type = Constants.FieldIdentifiers.PERSON_KEYWORD,
                                            Show = ffv.Show,
                                            PrimaryValue = ffv.PrimaryValue
                                        }
                                    }
                                );
                            }
                            if (keywordGroup.items.Count > 0)
                            {
                                profileDataResponse.personal.keywordGroups.Add(keywordGroup);
                            }
                            break;


                        // External identifier
                        case Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER:
                            ProfileEditorGroupExternalIdentifier externalIdentifierGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ProfileEditorItemExternalIdentifier>() { },
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = dfds.Id,
                                    Type = Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER,
                                    Show = dfds.Show
                                }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                externalIdentifierGroup.items.Add(
                                    new ProfileEditorItemExternalIdentifier()
                                    {
                                        PidContent = ffv.DimPid.PidContent,
                                        PidType = ffv.DimPid.PidType,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = ffv.DimPidId,
                                            Type = Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER,
                                            Show = ffv.Show,
                                            PrimaryValue = ffv.PrimaryValue
                                        }
                                    }
                                );
                            }
                            if (externalIdentifierGroup.items.Count > 0)
                            {
                                profileDataResponse.personal.externalIdentifierGroups.Add(externalIdentifierGroup);
                            }
                            break;

                        // Role in researcher community
                        case Constants.FieldIdentifiers.ACTIVITY_ROLE_IN_RESERCH_COMMUNITY:
                            // TODO
                            break;

                        // Affiliation
                        case Constants.FieldIdentifiers.ACTIVITY_AFFILIATION:
                            ProfileEditorGroupAffiliation affiliationGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ProfileEditorItemAffiliation>() { },
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = dfds.Id,
                                    Type = Constants.FieldIdentifiers.ACTIVITY_AFFILIATION,
                                    Show = dfds.Show
                                }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                // Get organization name from related DimOrganization (ffv.DimAffiliation.DimOrganization), if exists.
                                // Otherwise from DimIdentifierlessData (ffv.DimIdentifierlessData).
                                // Name translation service ensures that none of the language fields is empty.
                                NameTranslation nameTranslationAffiliationOrganization = new();
                                if (ffv.DimAffiliation.DimOrganizationId > 0)
                                {
                                    nameTranslationAffiliationOrganization = _languageService.GetNameTranslation(
                                        nameFi: ffv.DimAffiliation.DimOrganization.NameFi,
                                        nameEn: ffv.DimAffiliation.DimOrganization.NameEn,
                                        nameSv: ffv.DimAffiliation.DimOrganization.NameSv
                                    );
                                }
                                else if (ffv.DimIdentifierlessDataId > -1 && ffv.DimIdentifierlessData.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                                {
                                    nameTranslationAffiliationOrganization = _languageService.GetNameTranslation(
                                        nameFi: ffv.DimIdentifierlessData.ValueFi,
                                        nameEn: ffv.DimIdentifierlessData.ValueEn,
                                        nameSv: ffv.DimIdentifierlessData.ValueSv
                                    );
                                }

                                // Name translation for position name
                                NameTranslation nameTranslationPositionName = _languageService.GetNameTranslation(
                                    nameFi: ffv.DimAffiliation.PositionNameFi,
                                    nameEn: ffv.DimAffiliation.PositionNameEn,
                                    nameSv: ffv.DimAffiliation.PositionNameSv
                                );

                                // Name translation for department name
                                NameTranslation nameTranslationAffiliationDepartment = _languageService.GetNameTranslation(
                                    nameFi: "",
                                    nameEn: _organizationHandlerService.GetAffiliationDepartmentNameFromFactFieldValue(factFieldValue: ffv),
                                    nameSv: ""
                                );

                                ProfileEditorItemAffiliation affiliation = new()
                                {
                                    // TODO: DimOrganization handling
                                    OrganizationNameFi = nameTranslationAffiliationOrganization.NameFi,
                                    OrganizationNameEn = nameTranslationAffiliationOrganization.NameEn,
                                    OrganizationNameSv = nameTranslationAffiliationOrganization.NameSv,
                                    DepartmentNameFi = nameTranslationAffiliationDepartment.NameFi,
                                    DepartmentNameEn = nameTranslationAffiliationDepartment.NameEn,
                                    DepartmentNameSv = nameTranslationAffiliationDepartment.NameSv,
                                    PositionNameFi = nameTranslationPositionName.NameFi,
                                    PositionNameEn = nameTranslationPositionName.NameEn,
                                    PositionNameSv = nameTranslationPositionName.NameSv,
                                    Type = ffv.DimAffiliation.AffiliationTypeNavigation.NameFi,
                                    StartDate = new ProfileEditorItemDate()
                                    {
                                        Year = ffv.DimAffiliation.StartDateNavigation.Year,
                                        Month = ffv.DimAffiliation.StartDateNavigation.Month,
                                        Day = ffv.DimAffiliation.StartDateNavigation.Day
                                    },
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimAffiliationId,
                                        Type = Constants.FieldIdentifiers.ACTIVITY_AFFILIATION,
                                        Show = ffv.Show,
                                        PrimaryValue = ffv.PrimaryValue
                                    }
                                };

                                // Affiliation EndDate can be null
                                if (ffv.DimAffiliation.EndDateNavigation != null)
                                {
                                    affiliation.EndDate = new()
                                    {
                                        Year = ffv.DimAffiliation.EndDateNavigation.Year,
                                        Month = ffv.DimAffiliation.EndDateNavigation.Month,
                                        Day = ffv.DimAffiliation.EndDateNavigation.Day
                                    };
                                }
                                affiliationGroup.items.Add(affiliation);
                            }
                            if (affiliationGroup.items.Count > 0)
                            {
                                profileDataResponse.activity.affiliationGroups.Add(affiliationGroup);
                            }
                            break;

                        // Education
                        case Constants.FieldIdentifiers.ACTIVITY_EDUCATION:
                            ProfileEditorGroupEducation educationGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ProfileEditorItemEducation>() { },
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = dfds.Id,
                                    Type = Constants.FieldIdentifiers.ACTIVITY_EDUCATION,
                                    Show = dfds.Show
                                }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                // Name translation service ensures that none of the language fields is empty.
                                NameTranslation nameTraslationEducation = _languageService.GetNameTranslation(
                                    nameFi: ffv.DimEducation.NameFi,
                                    nameEn: ffv.DimEducation.NameEn,
                                    nameSv: ffv.DimEducation.NameSv
                                );

                                ProfileEditorItemEducation education = new()
                                {
                                    NameFi = nameTraslationEducation.NameFi,
                                    NameEn = nameTraslationEducation.NameEn,
                                    NameSv = nameTraslationEducation.NameSv,
                                    DegreeGrantingInstitutionName = ffv.DimEducation.DegreeGrantingInstitutionName,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimEducationId,
                                        Type = Constants.FieldIdentifiers.ACTIVITY_EDUCATION,
                                        Show = ffv.Show,
                                        PrimaryValue = ffv.PrimaryValue
                                    }
                                };
                                // Education StartDate can be null
                                if (ffv.DimEducation.DimStartDateNavigation != null)
                                {
                                    education.StartDate = new ProfileEditorItemDate()
                                    {
                                        Year = ffv.DimEducation.DimStartDateNavigation.Year,
                                        Month = ffv.DimEducation.DimStartDateNavigation.Month,
                                        Day = ffv.DimEducation.DimStartDateNavigation.Day
                                    };
                                }
                                // Education EndDate can be null
                                if (ffv.DimEducation.DimEndDateNavigation != null)
                                {
                                    education.EndDate = new ProfileEditorItemDate()
                                    {
                                        Year = ffv.DimEducation.DimEndDateNavigation.Year,
                                        Month = ffv.DimEducation.DimEndDateNavigation.Month,
                                        Day = ffv.DimEducation.DimEndDateNavigation.Day
                                    };
                                }
                                educationGroup.items.Add(education);
                            }
                            if (educationGroup.items.Count > 0)
                            {
                                profileDataResponse.activity.educationGroups.Add(educationGroup);
                            }
                            break;

                        // Publication
                        case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION:
                            // Experimental. Publications in alternative structure.
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                profileDataResponse.activity.publications = _duplicateHandlerService.AddPublicationToProfileEditorData(dataSource: profileEditorSource, ffv: ffv, publications:
profileDataResponse.activity.publications);
                            }
                            break;

                        // Publication (ORCID)
                        case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID:
                            // Experimental. ORCID Publications in alternative structure.
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                profileDataResponse.activity.publications = _duplicateHandlerService.AddPublicationToProfileEditorData(dataSource: profileEditorSource, ffv: ffv, publications:
profileDataResponse.activity.publications);
                            }
                            break;

                        // Funding decision
                        case Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION:
                            ProfileEditorGroupFundingDecision fundingDecisionGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ProfileEditorItemFundingDecision>() { },
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = dfds.Id,
                                    Type = Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION,
                                    Show = dfds.Show
                                }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                // Name translation service ensures that none of the language fields is empty.
                                NameTranslation nameTraslationFundingDecision_ProjectName = _languageService.GetNameTranslation(
                                    nameFi: ffv.DimFundingDecision.NameFi,
                                    nameSv: ffv.DimFundingDecision.NameSv,
                                    nameEn: ffv.DimFundingDecision.NameEn
                                );
                                NameTranslation nameTraslationFundingDecision_ProjectDescription = _languageService.GetNameTranslation(
                                    nameFi: ffv.DimFundingDecision.DescriptionFi,
                                    nameSv: ffv.DimFundingDecision.DescriptionSv,
                                    nameEn: ffv.DimFundingDecision.DescriptionEn
                                );
                                NameTranslation nameTraslationFundingDecision_FunderName = _languageService.GetNameTranslation(
                                    nameFi: ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameFi,
                                    nameSv: ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameSv,
                                    nameEn: ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameEn
                                );
                                NameTranslation nameTranslationFundingDecision_TypeOfFunding = _languageService.GetNameTranslation(
                                    nameFi: ffv.DimFundingDecision.DimTypeOfFunding.NameFi,
                                    nameSv: ffv.DimFundingDecision.DimTypeOfFunding.NameSv,
                                    nameEn: ffv.DimFundingDecision.DimTypeOfFunding.NameEn
                                );
                                NameTranslation nameTranslationFundingDecision_CallProgramme = _languageService.GetNameTranslation(
                                    nameFi: ffv.DimFundingDecision.DimCallProgramme.NameFi,
                                    nameSv: ffv.DimFundingDecision.DimCallProgramme.NameSv,
                                    nameEn: ffv.DimFundingDecision.DimCallProgramme.NameEn
                                );

                                ProfileEditorItemFundingDecision fundingDecision = new()
                                {
                                    ProjectId = ffv.DimFundingDecision.Id,
                                    ProjectAcronym = ffv.DimFundingDecision.Acronym,
                                    ProjectNameFi = nameTraslationFundingDecision_ProjectName.NameFi,
                                    ProjectNameSv = nameTraslationFundingDecision_ProjectName.NameSv,
                                    ProjectNameEn = nameTraslationFundingDecision_ProjectName.NameEn,
                                    ProjectDescriptionFi = nameTraslationFundingDecision_ProjectDescription.NameFi,
                                    ProjectDescriptionSv = nameTraslationFundingDecision_ProjectDescription.NameSv,
                                    ProjectDescriptionEn = nameTraslationFundingDecision_ProjectDescription.NameEn,
                                    FunderNameFi = nameTraslationFundingDecision_FunderName.NameFi,
                                    FunderNameSv = nameTraslationFundingDecision_FunderName.NameSv,
                                    FunderNameEn = nameTraslationFundingDecision_FunderName.NameEn,
                                    FunderProjectNumber = ffv.DimFundingDecision.FunderProjectNumber,
                                    TypeOfFundingNameFi = nameTranslationFundingDecision_TypeOfFunding.NameFi,
                                    TypeOfFundingNameSv = nameTranslationFundingDecision_TypeOfFunding.NameSv,
                                    TypeOfFundingNameEn = nameTranslationFundingDecision_TypeOfFunding.NameEn,
                                    CallProgrammeNameFi = nameTranslationFundingDecision_CallProgramme.NameFi,
                                    CallProgrammeNameSv = nameTranslationFundingDecision_CallProgramme.NameSv,
                                    CallProgrammeNameEn = nameTranslationFundingDecision_CallProgramme.NameEn,
                                    AmountInEur = ffv.DimFundingDecision.AmountInEur,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimFundingDecisionId,
                                        Type = Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION,
                                        Show = ffv.Show,
                                        PrimaryValue = ffv.PrimaryValue
                                    }
                                };

                                // Funding decision start year
                                if (ffv.DimFundingDecision.DimDateIdStartNavigation != null && ffv.DimFundingDecision.DimDateIdStartNavigation.Year > 0)
                                {
                                    fundingDecision.FundingStartYear = ffv.DimFundingDecision.DimDateIdStartNavigation.Year;
                                }
                                // Funding decision end year
                                if (ffv.DimFundingDecision.DimDateIdEndNavigation != null && ffv.DimFundingDecision.DimDateIdEndNavigation.Year > 0)
                                {
                                    fundingDecision.FundingEndYear = ffv.DimFundingDecision.DimDateIdEndNavigation.Year;
                                }

                                fundingDecisionGroup.items.Add(fundingDecision);
                            }
                            if (fundingDecisionGroup.items.Count > 0)
                            {
                                profileDataResponse.activity.fundingDecisionGroups.Add(fundingDecisionGroup);
                            }
                            break;

                        // Research dataset
                        case Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET:
                            ProfileEditorGroupResearchDataset researchDatasetGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ProfileEditorItemResearchDataset>() { },
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = dfds.Id,
                                    Type = Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET,
                                    Show = dfds.Show
                                }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                // Name translation service ensures that none of the language fields is empty.
                                NameTranslation nameTraslationResearchDataset_Name = _languageService.GetNameTranslation(
                                    nameFi: ffv.DimResearchDataset.NameFi,
                                    nameSv: ffv.DimResearchDataset.NameSv,
                                    nameEn: ffv.DimResearchDataset.NameEn
                                );
                                NameTranslation nameTraslationResearchDataset_Description = _languageService.GetNameTranslation(
                                    nameFi: ffv.DimResearchDataset.DescriptionFi,
                                    nameSv: ffv.DimResearchDataset.DescriptionSv,
                                    nameEn: ffv.DimResearchDataset.DescriptionEn
                                );

                                // Get values from DimPid. There is no FK between DimResearchDataset and DimPid,
                                // so the query must be done separately.
                                List<DimPid> dimPids = await _ttvContext.DimPids.Where(dp => dp.DimResearchDatasetId == ffv.DimResearchDatasetId && ffv.DimResearchDatasetId > -1).AsNoTracking().ToListAsync();

                                List<ProfileEditorPreferredIdentifier> preferredIdentifiers = new();
                                foreach (DimPid dimPid in dimPids)
                                {
                                    preferredIdentifiers.Add(
                                        new ProfileEditorPreferredIdentifier()
                                        {
                                            PidType = dimPid.PidType,
                                            PidContent = dimPid.PidContent
                                        }
                                    );
                                }

                                // TODO: add properties according to ElasticSearch index
                                ProfileEditorItemResearchDataset researchDataset = new()
                                {
                                    Actor = new List<ProfileEditorActor>(),
                                    Identifier = ffv.DimResearchDataset.LocalIdentifier,
                                    NameFi = nameTraslationResearchDataset_Name.NameFi,
                                    NameSv = nameTraslationResearchDataset_Name.NameSv,
                                    NameEn = nameTraslationResearchDataset_Name.NameEn,
                                    DescriptionFi = nameTraslationResearchDataset_Description.NameFi,
                                    DescriptionSv = nameTraslationResearchDataset_Description.NameSv,
                                    DescriptionEn = nameTraslationResearchDataset_Description.NameEn,
                                    PreferredIdentifiers = preferredIdentifiers,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimResearchDatasetId,
                                        Type = Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET,
                                        Show = ffv.Show,
                                        PrimaryValue = ffv.PrimaryValue
                                    }
                                };

                                // DatasetCreated
                                if (ffv.DimResearchDataset.DatasetCreated != null)
                                {
                                    researchDataset.DatasetCreated = ffv.DimResearchDataset.DatasetCreated.Value.Year;
                                }

                                // Fill actors list
                                foreach (FactContribution fc in ffv.DimResearchDataset.FactContributions)
                                {
                                    researchDataset.Actor.Add(
                                        new ProfileEditorActor()
                                        {
                                            actorRole = int.Parse(fc.DimReferencedataActorRole.CodeValue),
                                            actorRoleNameFi = fc.DimReferencedataActorRole.NameFi,
                                            actorRoleNameSv = fc.DimReferencedataActorRole.NameSv,
                                            actorRoleNameEn = fc.DimReferencedataActorRole.NameEn
                                        }
                                    );
                                }

                                researchDatasetGroup.items.Add(researchDataset);
                            }
                            if (researchDatasetGroup.items.Count > 0)
                            {
                                profileDataResponse.activity.researchDatasetGroups.Add(researchDatasetGroup);
                            }
                            break;

                        default:
                            break;
                    }
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
        public async Task<bool> DeleteProfileDataAsync(int userprofileId)
        {
            _logger.LogInformation($"Deleting user profile (dim_user_profile.id={userprofileId})");

            using (var connection = _ttvContext.Database.GetDbConnection())
            {
                // Get list of FactFieldValues using Entity Framework, which ensures that model FactFieldValue populates correctly.
                // After that delete database items using Dapper.
                List<FactFieldValue> factFieldValues =
                    await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId)
                    .AsNoTracking().ToListAsync();

                // Open 
                await connection.OpenAsync();
                var transaction = connection.BeginTransaction();

                try
                {
                    /*
                    // Get fact_field_values
                    string sqlFactFieldValues = _ttvSqlService.GetSqlQuery_Select_FactFieldValues(userprofileId);
                    List<FactFieldValue> factFieldValues =
                        (await connection.QueryAsync<FactFieldValue>(sql: sqlFactFieldValues, transaction: transaction)).ToList();
                    */
                    // Delete fact_field_values
                    string sqlDeleteFactFieldValues = _ttvSqlService.GetSqlQuery_Delete_FactFieldValues(userprofileId);
                    await connection.ExecuteAsync(sql: sqlDeleteFactFieldValues, transaction: transaction);

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
                                string sqlDeleteDimIdentifierlessDataChildren = _ttvSqlService.GetSqlQuery_Delete_DimIdentifierlessData_Children(factFieldValue.DimIdentifierlessDataId);
                                await connection.ExecuteAsync(sql: sqlDeleteDimIdentifierlessDataChildren, transaction: transaction);

                                // Then delete parent from dim_identifierless_data
                                string sqlDeleteDimIdentifierlessDataParent = _ttvSqlService.GetSqlQuery_Delete_DimIdentifierlessData_Parent(factFieldValue.DimIdentifierlessDataId);
                                await connection.ExecuteAsync(sql: sqlDeleteDimIdentifierlessDataParent, transaction: transaction);

                            }

                            // ORCID putcodes must be deleted separately
                            if (factFieldValue.DimPidIdOrcidPutCode != -1)
                            {
                                string sqlDeleteOrcidPutCode = _ttvSqlService.GetSqlQuery_Delete_DimPid_ORCID_PutCode(factFieldValue.DimPidIdOrcidPutCode);
                                await connection.ExecuteAsync(sql: sqlDeleteOrcidPutCode, transaction: transaction);
                            }

                            // Delete other related items than dim_identifierless_data
                            string sqlDeleteFactFieldValueRelatedData = _ttvSqlService.GetSqlQuery_Delete_FactFieldValueRelatedData(factFieldValue);
                            if (sqlDeleteFactFieldValueRelatedData != "")
                            {
                                await connection.ExecuteAsync(sql: sqlDeleteFactFieldValueRelatedData, transaction: transaction);
                            }

                        }
                    }

                    // Delete dim_field_display_settings
                    string sqlDeleteDimFieldDisplaySettings = _ttvSqlService.GetSqlQuery_Delete_DimFieldDisplaySettings(userprofileId);
                    await connection.ExecuteAsync(sql: sqlDeleteDimFieldDisplaySettings, transaction: transaction);

                    // Delete br_granted_permissions
                    string sqlDeleteBrGrantedPermissions = _ttvSqlService.GetSqlQuery_Delete_BrGrantedPermissions(userprofileId);
                    await connection.ExecuteAsync(sql: sqlDeleteBrGrantedPermissions, transaction: transaction);

                    // Delete dim_user_choices
                    string sqlDeleteDimUserChoices = _ttvSqlService.GetSqlQuery_Delete_DimUserChoices(userprofileId);
                    await connection.ExecuteAsync(sql: sqlDeleteDimUserChoices, transaction: transaction);

                    // Delete dim_user_profile
                    string sqlDeleteDimUserProfile = _ttvSqlService.GetSqlQuery_Delete_DimUserProfile(userprofileId);
                    await connection.ExecuteAsync(sql: sqlDeleteDimUserProfile, transaction: transaction);

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


            /*
            // Get DimUserProfile and related data that should be removed. 
            DimUserProfile dimUserProfile = await _ttvContext.DimUserProfiles.TagWith("Delete profile data")
                .Include(dup => dup.DimFieldDisplaySettings)
                .Include(dup => dup.DimUserChoices)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimRegisteredDataSource)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimName)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimWebLink)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPid)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPidIdOrcidPutCodeNavigation)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimAffiliation)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEducation)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimTelephoneNumber)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEmailAddrress)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearcherDescription)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimOrcidPublication)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimKeyword)
                .Include(dup => dup.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimIdentifierlessData)
                        .ThenInclude(did => did.InverseDimIdentifierlessData) // DimIdentifierlessData can have a child entity.
                .FirstOrDefaultAsync(up => up.Id == userprofileId);

            foreach (FactFieldValue ffv in dimUserProfile.FactFieldValues.Where(ffv => ffv.DimNameId == -1))
            {
                // Always remove FactFieldValue
                _ttvContext.FactFieldValues.Remove(ffv);

                // DimIdentifierlessData
                if (ffv.DimIdentifierlessDataId != -1)
                {
                    // Remove children
                    _ttvContext.DimIdentifierlessData.RemoveRange(ffv.DimIdentifierlessData.InverseDimIdentifierlessData);
                    _ttvContext.DimIdentifierlessData.Remove(ffv.DimIdentifierlessData);
                }

                // DimAffiliation
                if (ffv.DimAffiliationId != -1)
                {
                    if (CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimAffiliations.Remove(ffv.DimAffiliation);
                    }
                }

                // ORCID put code
                if (ffv.DimPidIdOrcidPutCode != -1)
                {
                    _ttvContext.DimPids.Remove(ffv.DimPidIdOrcidPutCodeNavigation);
                }

                // DimPid
                // DimPids related to FactFieldValue store person's external identifiers 
                if (ffv.DimPidId != -1)
                {
                    if (CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimPids.Remove(ffv.DimPid);
                    }
                }

                // DimWebLink
                else if (ffv.DimWebLinkId != -1)
                {
                    if (CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimWebLinks.Remove(ffv.DimWebLink);
                    }
                }

                // DimOrcidPublication
                else if (ffv.DimOrcidPublicationId != -1)
                {
                    if (CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimOrcidPublications.Remove(ffv.DimOrcidPublication);
                    }
                }

                // DimKeyword
                else if (ffv.DimKeywordId != -1)
                {
                    if (CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimKeywords.Remove(ffv.DimKeyword);
                    }
                }

                // DimEducation
                else if (ffv.DimEducationId != -1)
                {
                    if (CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimEducations.Remove(ffv.DimEducation);
                    }
                }

                // DimEmail
                else if (ffv.DimEmailAddrressId != -1)
                {
                    if (CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimEmailAddrresses.Remove(ffv.DimEmailAddrress);
                    }
                }

                // DimResearcherDescription
                else if (ffv.DimResearcherDescriptionId != -1)
                {
                    if (CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimResearcherDescriptions.Remove(ffv.DimResearcherDescription);
                    }
                }

                // DimTelephoneNumber
                else if (ffv.DimTelephoneNumberId != -1)
                {
                    if (CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimTelephoneNumbers.Remove(ffv.DimTelephoneNumber);
                    }
                }
            }
            await _ttvContext.SaveChangesAsync();

            // Remove DimName
            foreach (FactFieldValue ffv in dimUserProfile.FactFieldValues.Where(ffv => ffv.DimNameId != -1))
            {
                _ttvContext.FactFieldValues.Remove(ffv);

                if (ffv.DimPidIdOrcidPutCode != -1)
                {
                    _ttvContext.DimPids.Remove(ffv.DimPidIdOrcidPutCodeNavigation);
                }

                if (CanDeleteFactFieldValueRelatedData(ffv))
                {
                    _ttvContext.DimNames.Remove(ffv.DimName);
                }
            }
            await _ttvContext.SaveChangesAsync();

            // Remove sharing permissions.
            await _sharingService.DeleteAllGrantedPermissionsFromUserprofile(userprofileId: userprofileId);

            // Remove DimFieldDisplaySettings
            _ttvContext.DimFieldDisplaySettings.RemoveRange(dimUserProfile.DimFieldDisplaySettings);

            // Remove cooperation user choices
            _ttvContext.DimUserChoices.RemoveRange(dimUserProfile.DimUserChoices);

            // Remove DimUserProfile
            _ttvContext.DimUserProfiles.Remove(dimUserProfile);

            // Must not remove DimKnownPerson.
            // Must not remove DimPid (ORCID ID).

            await _ttvContext.SaveChangesAsync();
            */
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
