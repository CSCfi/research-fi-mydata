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

namespace api.Services
{
    /*
     * UserProfileService implements utilities, which simplify handling of userprofile and related data.
     */
    public class UserProfileService
    {
        private readonly TtvContext _ttvContext;
        private readonly DataSourceHelperService _dataSourceHelperService;
        private readonly UtilityService _utilityService;
        private readonly LanguageService _languageService;
        private readonly DuplicateHandlerService _duplicateHandlerService;
        private readonly OrganizationHandlerService _organizationHandlerService;
        private readonly SharingService _sharingService;

        public UserProfileService(TtvContext ttvContext,
            DataSourceHelperService dataSourceHelperService,
            UtilityService utilityService,
            LanguageService languageService,
            DuplicateHandlerService duplicateHandlerService,
            OrganizationHandlerService organizationHandlerService,
            SharingService sharingService)
        {
            _ttvContext = ttvContext;
            _dataSourceHelperService = dataSourceHelperService;
            _utilityService = utilityService;
            _languageService = languageService;
            _duplicateHandlerService = duplicateHandlerService;
            _organizationHandlerService = organizationHandlerService;
            _sharingService = sharingService;
        }

        // For unit test
        public UserProfileService(){}

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
                Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET
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
         * Data from registered data sources ORCID and TTV can be removed.
         */
        public bool CanDeleteFactFieldValueRelatedData(FactFieldValue ffv)
        {
            return ffv.DimRegisteredDataSourceId == _dataSourceHelperService.DimRegisteredDataSourceId_ORCID || ffv.DimRegisteredDataSourceId == _dataSourceHelperService.DimRegisteredDataSourceId_TTV;
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


        //public async Task<bool> AddTtvTelephoneNumbers(DimKnownPerson dimKnownPerson)
        //{
        //    var dimUserProfile = dimKnownPerson.DimUserProfiles.FirstOrDefault();
        //    if (dimUserProfile != null)
        //    {
        //        foreach (DimTelephoneNumber dimTelephoneNumber in dimKnownPerson.DimTelephoneNumbers)
        //        {
        //            // Find DimFieldDisplaySettings for registered data source
        //            var dimFieldDisplaySettingsTelephoneNumber =
        //                await _ttvContext.DimFieldDisplaySettings.FirstOrDefaultAsync(
        //                    dfds =>
        //                        dfds.DimUserProfileId == dimUserProfile.Id &&
        //                        dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER &&
        //                        dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSourceId == dimTelephoneNumber.DimRegisteredDataSourceId
        //                );

        //            if (dimFieldDisplaySettingsTelephoneNumber == null)
        //            {
        //                // Add new DimFieldDisplaySettings for DimTelephoneNumber
        //                dimFieldDisplaySettingsTelephoneNumber = new DimFieldDisplaySetting()
        //                {
        //                    DimUserProfileId = dimUserProfile.Id,
        //                }

        //                dimFieldDisplaySettingsTelephoneNumber.BrFieldDisplaySettingsDimRegisteredDataSources.Add(
        //                    new BrFieldDisplaySettingsDimRegisteredDataSource()
        //                    {
        //                        DimFieldDisplaySettingsId = dimFieldDisplaySettingsTelephoneNumber.Id,
        //                        DimRegisteredDataSourceId = orcidRegisteredDataSourceId
        //                    }
        //                );
        //            }
        //        }
        //        await _ttvContext.SaveChangesAsync();
        //    }
        //    return false;
        //}

        /*
         * Add publications from DimPublication into userprofile.
         */
        public async Task AddTtvPublications(DimKnownPerson dimKnownPerson, DimUserProfile dimUserProfile)
        {
            /*
             * Loop DimNames, then related FactContributions. FactContribution may have relation to DimPublication (DimPublicationId != -1).
             * 
             * DimKnownPerson => DimName => FactContribution => DimPublication
             * 
             * NOTE! Data source for DimPublication must be taken from DimName, not from DimPublication.
             * Skip item if DimName does not have data source set.
             */

            // Get DimFieldDisplaySetting
            // TODO: optimize case when dimKnownPerson.DimNames.Count == 0
            DimFieldDisplaySetting dimFieldDisplaySetting = await _ttvContext.DimFieldDisplaySettings.FirstOrDefaultAsync(dfds => dfds.DimUserProfileId == dimUserProfile.Id && dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION);

            foreach (DimName dimName in dimKnownPerson.DimNames)
            {
                // Collect publication ids into a list.
                List<int> publicationsIds = new();
                // Registered data source from DimName must be used as a publication data source.
                DimRegisteredDataSource dimNameRegisteredDataSource = dimName.DimRegisteredDataSource;


                // Skip if DimName does not have registered data source.
                if (dimNameRegisteredDataSource != null)
                {
                    // DimName can relate to many FactContributions. Collect valid publication ids (not -1).
                    foreach (FactContribution factContribution in dimName.FactContributions.Where(fc => fc.DimPublicationId != -1))
                    {
                        publicationsIds.Add(factContribution.DimPublicationId);
                    }

                    // Add FactFieldValues for DimPublications
                    foreach (int publicationId in publicationsIds.Distinct())
                    {
                        FactFieldValue factFieldValuePublication = this.GetEmptyFactFieldValue();
                        factFieldValuePublication.DimUserProfileId = dimUserProfile.Id;
                        factFieldValuePublication.DimFieldDisplaySettingsId = dimFieldDisplaySetting.Id;
                        factFieldValuePublication.DimPublicationId = publicationId;
                        factFieldValuePublication.DimRegisteredDataSourceId = dimNameRegisteredDataSource.Id;
                        _ttvContext.FactFieldValues.Add(factFieldValuePublication);
                    }
                    await _ttvContext.SaveChangesAsync();
                }
            }
        }

        /*
         * Search and add data from TTV database.
         * This is data that is already linked to the ORCID id in DimPid and it's related DimKnownPerson.
         */
        public async Task AddTtvDataToUserProfile(DimKnownPerson dimKnownPerson, DimUserProfile dimUserProfile)
        {
            await this.AddTtvPublications(dimKnownPerson, dimUserProfile);
        }

        /*
         * Create user profile.
         */
        public async Task CreateProfile(string orcidId)
        {
            // Get DimPid by ORCID id.
            // Also get related entities. Needed when searching existing data that should be automatically included in profile.
            DimPid dimPid = await _ttvContext.DimPids
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimNames)
                        .ThenInclude(dn => dn.FactContributions).AsNoTracking()
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimNames)
                        .ThenInclude(dn => dn.DimRegisteredDataSource).AsNoTracking()
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimTelephoneNumbers)
                        .ThenInclude(dtn => dtn.DimRegisteredDataSource).AsNoTracking()
                .Include(dp => dp.DimKnownPerson)
                    .ThenInclude(dkp => dkp.DimUserProfiles).AsNoTracking().FirstOrDefaultAsync(p => p.PidContent == orcidId && p.PidType == Constants.PidTypes.ORCID);

            // Get current DateTime
            DateTime currentDateTime = _utilityService.GetCurrentDateTime();

            // Check if DimPid 
            if (dimPid == null)
            {
                // DimPid was not found.

                // Add new DimPid, add new DimKnownPerson
                dimPid = GetEmptyDimPid();
                dimPid.PidContent = orcidId;
                dimPid.PidType = Constants.PidTypes.ORCID;

                // Since new DimPid is added, then new DimKnownPerson must be added
                dimPid.DimKnownPerson = new DimKnownPerson()
                {
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = currentDateTime,
                    Modified = currentDateTime
                };
                dimPid.SourceId = Constants.SourceIdentifiers.PROFILE_API;
                _ttvContext.DimPids.Add(dimPid);
                await _ttvContext.SaveChangesAsync();
            }
            else if (dimPid.DimKnownPerson == null || dimPid.DimKnownPersonId == -1)
            {
                // DimPid was found but it does not have related DimKnownPerson.
                DimKnownPerson kp = new()
                {
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = currentDateTime,
                    Modified = currentDateTime
                };
                _ttvContext.DimKnownPeople.Add(kp);
                dimPid.DimKnownPerson = kp;
                await _ttvContext.SaveChangesAsync();
            }

            // Add DimUserProfile
            DimUserProfile dimUserProfile = dimPid.DimKnownPerson.DimUserProfiles.FirstOrDefault();
            if (dimUserProfile == null)
            {
                // Add DimUserProfile
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
                await _ttvContext.SaveChangesAsync();

                // Add DimFieldDisplaySettings
                foreach (int fieldIdentifier in GetFieldIdentifiers())
                {
                    DimFieldDisplaySetting dimFieldDisplaySetting = new()
                    {
                        DimUserProfileId = dimUserProfile.Id,
                        FieldIdentifier = fieldIdentifier,
                        Show = false,
                        SourceId = Constants.SourceIdentifiers.PROFILE_API,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = currentDateTime,
                        Modified = currentDateTime
                    };
                    _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySetting);
                }
                await _ttvContext.SaveChangesAsync();


                // Demo data can be added to every user profile by uncommenting the following line
                // await _demoDataService.AddDemoDataToUserProfile(orcidId, dimUserProfile);

                // Add Ttv data to user profile
                await AddTtvDataToUserProfile(dimPid.DimKnownPerson, dimUserProfile);
            }
        }

        /*
         *  Get profile data.
         */
        public async Task<ProfileEditorDataResponse> GetProfileDataAsync(int userprofileId)
        {
            // Get DimFieldDisplaySettings and related entities
            List<DimFieldDisplaySetting> dimFieldDisplaySettings = await _ttvContext.DimFieldDisplaySettings.Where(dfds => dfds.DimUserProfileId == userprofileId && dfds.FactFieldValues.Count() > 0)
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

            ProfileEditorDataResponse profileDataResponse = new() { };

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
                                    affiliation.EndDate = new ProfileEditorItemDate()
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
                                    education.StartDate = new()
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
                            ProfileEditorGroupPublication publicationGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ProfileEditorItemPublication>() { },
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = dfds.Id,
                                    Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                                    Show = dfds.Show
                                }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                publicationGroup.items.Add(
                                    new ProfileEditorItemPublication()
                                    {
                                        PublicationId = ffv.DimPublication.PublicationId,
                                        PublicationName = ffv.DimPublication.PublicationName,
                                        PublicationYear = ffv.DimPublication.PublicationYear,
                                        Doi = ffv.DimPublication.Doi,
                                        TypeCode = ffv.DimPublication.PublicationTypeCode,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = ffv.DimPublicationId,
                                            Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                                            Show = ffv.Show,
                                            PrimaryValue = ffv.PrimaryValue
                                        }
                                    }
                                );
                            }
                            if (publicationGroup.items.Count > 0)
                            {
                                profileDataResponse.activity.publicationGroups.Add(publicationGroup);
                            }
                            break;

                        // Publication (ORCID)
                        case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID:
                            ProfileEditorGroupPublication orcidPublicationGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ProfileEditorItemPublication>() { },
                                groupMeta = new ProfileEditorGroupMeta()
                                {
                                    Id = dfds.Id,
                                    Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID,
                                    Show = dfds.Show
                                }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                orcidPublicationGroup.items.Add(

                                    new ProfileEditorItemPublication()
                                    {
                                        PublicationId = ffv.DimOrcidPublication.PublicationId,
                                        PublicationName = ffv.DimOrcidPublication.PublicationName,
                                        PublicationYear = ffv.DimOrcidPublication.PublicationYear,
                                        Doi = ffv.DimOrcidPublication.DoiHandle, // TODO: ORCID doi field name?
                                        TypeCode = "", // TODO: ORCID publication type code handling
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = ffv.DimOrcidPublicationId,
                                            Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID,
                                            Show = ffv.Show,
                                            PrimaryValue = ffv.PrimaryValue
                                        }
                                    }
                                );
                            }
                            if (orcidPublicationGroup.items.Count > 0)
                            {
                                profileDataResponse.activity.publicationGroups.Add(orcidPublicationGroup);
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
         *  Get profile data. New version using different data structure.
         */
        public async Task<ProfileEditorDataResponse2> GetProfileDataAsync2(int userprofileId)
        {
            // Get DimFieldDisplaySettings and related entities
            List<DimFieldDisplaySetting> dimFieldDisplaySettings = await _ttvContext.DimFieldDisplaySettings.Where(dfds => dfds.DimUserProfileId == userprofileId && dfds.FactFieldValues.Count() > 0)
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
                                profileDataResponse.activity.publications = _duplicateHandlerService.AddPublicationToProfileEditorData(dataSource: profileEditorSource, ffv: ffv, publications: profileDataResponse.activity.publications);
                            }
                            break;

                        // Publication (ORCID)
                        case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID:
                            // Experimental. ORCID Publications in alternative structure.
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                profileDataResponse.activity.publications = _duplicateHandlerService.AddPublicationToProfileEditorData(dataSource: profileEditorSource, ffv: ffv, publications: profileDataResponse.activity.publications);
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
         * Delete profile data.
         */
        public async Task DeleteProfileDataAsync(int userprofileId)
        {
            // Get DimUserProfile and related data that should be removed. 
            DimUserProfile dimUserProfile = await _ttvContext.DimUserProfiles
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
                    .ThenInclude(ffv => ffv.DimResearchDataset)
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

                // DimResarchDataset
                else if (ffv.DimResearchDatasetId != -1)
                {
                    if (CanDeleteFactFieldValueRelatedData(ffv))
                    {
                        _ttvContext.DimResearchDatasets.Remove(ffv.DimResearchDataset);
                    }

                    // DEMO: remove test data from FactContribution
                    System.Collections.Generic.List<FactContribution> factContributions = await _ttvContext.FactContributions.Where(fc => fc.DimResearchDatasetId == ffv.DimResearchDatasetId && fc.SourceId == Constants.SourceIdentifiers.DEMO).ToListAsync();
                    foreach (FactContribution fc in factContributions)
                    {
                        _ttvContext.FactContributions.Remove(fc);
                        _ttvContext.Entry(fc).State = EntityState.Deleted;
                    }

                    // DEMO: remove test data from DimPids
                    System.Collections.Generic.List<DimPid> dimPids = await _ttvContext.DimPids.Where(dp => dp.DimResearchDatasetId == ffv.DimResearchDatasetId && dp.SourceId == Constants.SourceIdentifiers.DEMO).ToListAsync();
                    foreach (DimPid dp in dimPids)
                    {
                        _ttvContext.DimPids.Remove(dp);
                        _ttvContext.Entry(dp).State = EntityState.Deleted;
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