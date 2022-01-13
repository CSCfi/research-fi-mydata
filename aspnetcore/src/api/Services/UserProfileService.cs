using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Ttv;
using api.Models.ProfileEditor;
using Microsoft.EntityFrameworkCore;
using api.Models.Common;

namespace api.Services
{
    /*
     * UserProfileService implements utilities, which simplify handling of userprofile and related data.
     */
    public class UserProfileService
    {
        private readonly TtvContext _ttvContext;
        private readonly UtilityService _utilityService;
        private readonly LanguageService _languageService;

        /*
         * Constructor with dependency injection.
         */
        public UserProfileService(TtvContext ttvContext, UtilityService utilityService, LanguageService languageService)
        {
            _ttvContext = ttvContext;
            _utilityService = utilityService;
            _languageService = languageService;
        }

        /*
         * Constructor without dependency injection.
         * Needed for simplifying unit tests.
         */
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
         * Check if data related to FactFieldValue can be removed.
         */
        public bool CanDeleteFactFieldValueRelatedData(FactFieldValue ffv)
        {
            // ORCID and demo data can be removed.
            return ffv.SourceId == Constants.SourceIdentifiers.PROFILE_API || ffv.SourceId == Constants.SourceIdentifiers.DEMO;
        }

        /*
         * Get Id of DimUserProfile based on ORCID Id in DimPid.
         */
        public async Task<int> GetUserprofileId(String orcidId)
        {
            // Use raw SQL query.
            var userProfileSql = $@"SELECT dup.*
                                    FROM dim_user_profile AS dup
                                    INNER JOIN dim_known_person AS dkp
                                    ON dup.dim_known_person_id = dkp.id
                                    INNER JOIN dim_pid AS dp
                                    ON dkp.id=dp.dim_known_person_id
                                    WHERE dp.pid_type='ORCID' AND dp.pid_content='{orcidId}'";

            var dimUserProfile = await _ttvContext.DimUserProfiles.FromSqlRaw(userProfileSql).AsNoTracking().FirstOrDefaultAsync();

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
         * Get id of ORCID DimOrganization.
         * Create organization if it does not exist.
         */
        public async Task<int> GetOrcidOrganizationId()
        {
            var orcidOrganizationName = "ORCID";

            // Use raw SQL query.
            var orcidOrganizationSql = $"SELECT * FROM dim_organization WHERE name_en='{orcidOrganizationName}'";

            var orcidOrganization = await _ttvContext.DimOrganizations.FromSqlRaw(orcidOrganizationSql).AsNoTracking().FirstOrDefaultAsync();

            // TODO: creation of ORCID organization should not be necessary when the database is properly populated. Remove this at some point?
            if (orcidOrganization == null)
            {
                orcidOrganization = new DimOrganization()
                {
                    DimSectorid = -1,
                    OrganizationId = orcidOrganizationName,
                    OrganizationActive = true,
                    NameFi = orcidOrganizationName,
                    NameEn = orcidOrganizationName,
                    NameSv = orcidOrganizationName,
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime(),
                    DimRegisteredDataSourceId = -1
                };
                _ttvContext.DimOrganizations.Add(orcidOrganization);
                await _ttvContext.SaveChangesAsync();
                return orcidOrganization.Id;
            }
            else
            {
                return orcidOrganization.Id;
            }
        }

        /*
         * Get id of ORCID DimRegisteredDataSource.
         * Create data source if it does not exist.
         */
        public async Task<int> GetOrcidRegisteredDataSourceId()
        {
            var orcidDatasourceName = "ORCID";

            // Use raw SQL query.
            var orcidDatasourceSql = $"SELECT * FROM dim_registered_data_source WHERE name='{orcidDatasourceName}'";

            var orcidRegisteredDataSource = await _ttvContext.DimRegisteredDataSources.FromSqlRaw(orcidDatasourceSql).AsNoTracking().FirstOrDefaultAsync();

            // TODO: creation of ORCID data source should not be necessary when the database is properly populated. Remove this at some point?
            if (orcidRegisteredDataSource == null)
            {
                // Get ORCID organization
                var orcidOrganizationId = await this.GetOrcidOrganizationId();

                orcidRegisteredDataSource = new DimRegisteredDataSource()
                {
                    DimOrganizationId = orcidOrganizationId,
                    Name = orcidDatasourceName,
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime()
                };
                _ttvContext.DimRegisteredDataSources.Add(orcidRegisteredDataSource);
                await _ttvContext.SaveChangesAsync();
                return orcidRegisteredDataSource.Id;
            }
            else
            {
                return orcidRegisteredDataSource.Id;
            }
        }

        /*
         * Get id of Tiedejatutkimus.fi DimRegisteredDataSource.
         */
        public async Task<DimRegisteredDataSource?> GetTiedejatutkimusFiRegisteredDataSource()
        {
            var tiedejatutkimusfiRegisteredDataSource = await _ttvContext.DimRegisteredDataSources.Where(drds => drds.Name == Constants.SourceIdentifiers.TIEDEJATUTKIMUS)
                                                                .Include(drds => drds.DimOrganization).AsNoTracking().FirstOrDefaultAsync();
            return tiedejatutkimusfiRegisteredDataSource;
        }

        /*
         * Add or update DimName.
         */
        public async Task<DimName> AddOrUpdateDimName(String lastName, String firstNames, int dimKnownPersonId, int dimRegisteredDataSourceId)
        {
            var dimName = await _ttvContext.DimNames.FirstOrDefaultAsync(dn => dn.DimKnownPersonIdConfirmedIdentityNavigation.Id == dimKnownPersonId && dn.DimRegisteredDataSourceId == dimRegisteredDataSourceId);
            if (dimName == null)
            {
                dimName = new DimName()
                {
                    LastName = lastName,
                    FirstNames = firstNames,
                    DimKnownPersonIdConfirmedIdentity = dimKnownPersonId,
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime(),
                    DimRegisteredDataSourceId = dimRegisteredDataSourceId
                };
                _ttvContext.DimNames.Add(dimName);
            }
            else
            {
                dimName.LastName = lastName;
                dimName.FirstNames = firstNames;
                dimName.Modified = _utilityService.getCurrentDateTime();
            }
            await _ttvContext.SaveChangesAsync();
            return dimName;
        }

        /*
         * Add or update DimResearcherDescription.
         */
        public async Task<DimResearcherDescription> AddOrUpdateDimResearcherDescription(String description_fi, String description_en, String description_sv, int dimKnownPersonId, int dimRegisteredDataSourceId)
        {
            var dimResearcherDescription = await _ttvContext.DimResearcherDescriptions.FirstOrDefaultAsync(dr => dr.DimKnownPersonId == dimKnownPersonId && dr.DimRegisteredDataSourceId == dimRegisteredDataSourceId);
            if (dimResearcherDescription == null)
            {
                dimResearcherDescription = new DimResearcherDescription()
                {
                    ResearchDescriptionFi = description_fi,
                    ResearchDescriptionEn = description_en,
                    ResearchDescriptionSv = description_sv,
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime(),
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
                dimResearcherDescription.Modified = _utilityService.getCurrentDateTime();
            }
            await _ttvContext.SaveChangesAsync();
            return dimResearcherDescription;
        }

        /*
         * Add or update DimEmailAddrress.
         */
        public async Task<DimEmailAddrress> AddOrUpdateDimEmailAddress(string emailAddress, int dimKnownPersonId, int dimRegisteredDataSourceId)
        {
            var dimEmailAddress = await _ttvContext.DimEmailAddrresses.FirstOrDefaultAsync(dr => dr.Email == emailAddress && dr.DimKnownPersonId == dimKnownPersonId && dr.DimRegisteredDataSourceId == dimRegisteredDataSourceId);
            if (dimEmailAddress == null)
            {
                dimEmailAddress = new DimEmailAddrress()
                {
                    Email = emailAddress,
                    SourceId = Constants.SourceIdentifiers.PROFILE_API,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime(),
                    DimKnownPersonId = dimKnownPersonId,
                    DimRegisteredDataSourceId = dimRegisteredDataSourceId
                };
                _ttvContext.DimEmailAddrresses.Add(dimEmailAddress);
            }
            else
            {
                dimEmailAddress.Modified = _utilityService.getCurrentDateTime();
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
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime()
            };
        }

        /*
         * Get empty FactFieldValue. Set SourceID to DEMO.
         */
        public FactFieldValue GetEmptyFactFieldValueDemo()
        {
            var factFieldValue = this.GetEmptyFactFieldValue();
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
                DimRegisteredDataSourceId = -1,
                DimReferencedataid = -1
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
                PidContent = " ",
                PidType = " ",
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime()
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
            var dimFieldDisplaySetting = await _ttvContext.DimFieldDisplaySettings.FirstOrDefaultAsync(dfds => dfds.DimUserProfileId == dimUserProfile.Id && dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION);

            foreach (DimName dimName in dimKnownPerson.DimNames)
            {
                // Collect publication ids into a list.
                var publicationsIds = new List<int>();
                // Registered data source from DimName must be used as a publication data source.
                var dimNameRegisteredDataSource = dimName.DimRegisteredDataSource;


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
                        var factFieldValuePublication = this.GetEmptyFactFieldValue();
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



        public async Task<ProfileEditorDataResponse> GetProfileDataAsync(int userprofileId)
        {
            // Get DimFieldDisplaySettings and related entities
            var dimFieldDisplaySettings = await _ttvContext.DimFieldDisplaySettings.Where(dfds => dfds.DimUserProfileId == userprofileId && dfds.FactFieldValues.Count() > 0)
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
                // DimIdentifierlessData
                //.Include(dfds => dfds.FactFieldValues)
                //    .ThenInclude(ffv => ffv.DimIdentifierlessData).AsNoTracking() // TODO: update model to match SQL table
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

            var profileDataResponse = new ProfileEditorDataResponse() { };

            // Collect data from DimFieldDisplaySettings and FactFieldValues entities
            foreach (DimFieldDisplaySetting dfds in dimFieldDisplaySettings)
            {
                // Group FactFieldValues by DimRegisteredDataSourceId
                var factFieldValues_GroupedBy_DataSourceId = dfds.FactFieldValues.GroupBy(ffv => ffv.DimRegisteredDataSourceId);

                // Loop groups
                foreach (var factFieldValueGroup in factFieldValues_GroupedBy_DataSourceId)
                {
                    var dimRegisteredDataSource = factFieldValueGroup.First().DimRegisteredDataSource;

                    // Organization name translation
                    var nameTranslationSourceOrganization = _languageService.getNameTranslation(
                        nameFi: dimRegisteredDataSource.DimOrganization.NameFi,
                        nameEn: dimRegisteredDataSource.DimOrganization.NameEn,
                        nameSv: dimRegisteredDataSource.DimOrganization.NameSv
                    );

                    // Source object containing registered data source and organization name.
                    var profileEditorSource = new ProfileEditorSource()
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
                            var nameGroup = new ProfileEditorGroupName()
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
                            var otherNameGroup = new ProfileEditorGroupOtherName()
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
                            var researcherDescriptionGroup = new ProfileEditorGroupResearcherDescription()
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
                            var webLinkGroup = new ProfileEditorGroupWebLink()
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
                            var emailGroup = new ProfileEditorGroupEmail()
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
                            var telephoneNumberGroup = new ProfileEditorGroupTelephoneNumber()
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
                            var fieldOfScienceGroup = new ProfileEditorGroupFieldOfScience()
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
                                var nameTranslationFieldOfScience = _languageService.getNameTranslation(
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
                            var keywordGroup = new ProfileEditorGroupKeyword()
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
                            var externalIdentifierGroup = new ProfileEditorGroupExternalIdentifier()
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
                            var affiliationGroup = new ProfileEditorGroupAffiliation()
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
                                // Name translation service ensures that none of the language fields is empty.
                                var nameTranslationAffiliationOrganization = _languageService.getNameTranslation(
                                    nameFi: ffv.DimAffiliation.DimOrganization.NameFi,
                                    nameEn: ffv.DimAffiliation.DimOrganization.NameEn,
                                    nameSv: ffv.DimAffiliation.DimOrganization.NameSv
                                );
                                var nameTranslationPositionName = _languageService.getNameTranslation(
                                    nameFi: ffv.DimAffiliation.PositionNameFi,
                                    nameEn: ffv.DimAffiliation.PositionNameEn,
                                    nameSv: ffv.DimAffiliation.PositionNameSv
                                );

                                // TODO: demo version stores ORDCID affiliation department name in DimOrganization.NameUnd
                                var nameTranslationAffiliationDepartment = new NameTranslation()
                                {
                                    NameFi = "",
                                    NameEn = "",
                                    NameSv = ""
                                };
                                if (ffv.DimAffiliation.DimOrganization.SourceId == Constants.SourceIdentifiers.PROFILE_API)
                                {
                                    nameTranslationAffiliationDepartment = _languageService.getNameTranslation(
                                        "",
                                        nameEn: ffv.DimAffiliation.DimOrganization.NameUnd,
                                        ""
                                    );
                                }

                                var affiliation = new ProfileEditorItemAffiliation()
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
                            var educationGroup = new ProfileEditorGroupEducation()
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
                                var nameTraslationEducation = _languageService.getNameTranslation(
                                    nameFi: ffv.DimEducation.NameFi,
                                    nameEn: ffv.DimEducation.NameEn,
                                    nameSv: ffv.DimEducation.NameSv
                                );

                                var education = new ProfileEditorItemEducation()
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
                            var publicationGroup = new ProfileEditorGroupPublication()
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
                                        Doi = ffv.DimPublication.DoiHandle,
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
                            var orcidPublicationGroup = new ProfileEditorGroupPublication()
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
                                        Doi = ffv.DimOrcidPublication.DoiHandle,
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
                            var fundingDecisionGroup = new ProfileEditorGroupFundingDecision()
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
                                var nameTraslationFundingDecision_ProjectName = _languageService.getNameTranslation(
                                    nameFi: ffv.DimFundingDecision.NameFi,
                                    nameSv: ffv.DimFundingDecision.NameSv,
                                    nameEn: ffv.DimFundingDecision.NameEn
                                );
                                var nameTraslationFundingDecision_ProjectDescription = _languageService.getNameTranslation(
                                    nameFi: ffv.DimFundingDecision.DescriptionFi,
                                    nameSv: ffv.DimFundingDecision.DescriptionSv,
                                    nameEn: ffv.DimFundingDecision.DescriptionEn
                                );
                                var nameTraslationFundingDecision_FunderName = _languageService.getNameTranslation(
                                    nameFi: ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameFi,
                                    nameSv: ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameSv,
                                    nameEn: ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameEn
                                );
                                var nameTranslationFundingDecision_TypeOfFunding = _languageService.getNameTranslation(
                                    nameFi: ffv.DimFundingDecision.DimTypeOfFunding.NameFi,
                                    nameSv: ffv.DimFundingDecision.DimTypeOfFunding.NameSv,
                                    nameEn: ffv.DimFundingDecision.DimTypeOfFunding.NameEn
                                );
                                var nameTranslationFundingDecision_CallProgramme = _languageService.getNameTranslation(
                                    nameFi: ffv.DimFundingDecision.DimCallProgramme.NameFi,
                                    nameSv: ffv.DimFundingDecision.DimCallProgramme.NameSv,
                                    nameEn: ffv.DimFundingDecision.DimCallProgramme.NameEn
                                );

                                var fundingDecision = new ProfileEditorItemFundingDecision()
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
                            var researchDatasetGroup = new ProfileEditorGroupResearchDataset()
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
                                var nameTraslationResearchDataset_Name = _languageService.getNameTranslation(
                                    nameFi: ffv.DimResearchDataset.NameFi,
                                    nameSv: ffv.DimResearchDataset.NameSv,
                                    nameEn: ffv.DimResearchDataset.NameEn
                                );
                                var nameTraslationResearchDataset_Description = _languageService.getNameTranslation(
                                    nameFi: ffv.DimResearchDataset.DescriptionFi,
                                    nameSv: ffv.DimResearchDataset.DescriptionSv,
                                    nameEn: ffv.DimResearchDataset.DescriptionEn
                                );

                                // Get values from DimPid. There is no FK between DimResearchDataset and DimPid,
                                // so the query must be done separately.
                                var dimPids = await _ttvContext.DimPids.Where(dp => dp.DimResearchDatasetId == ffv.DimResearchDatasetId && ffv.DimResearchDatasetId > -1).AsNoTracking().ToListAsync();

                                var preferredIdentifiers = new List<ProfileEditorPreferredIdentifier>();
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
                                var researchDataset = new ProfileEditorItemResearchDataset()
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
    }
}