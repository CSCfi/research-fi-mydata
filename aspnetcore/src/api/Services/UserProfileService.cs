using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Ttv;
using api.Models.Elasticsearch;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    /*
     * UserProfileService implements utilities, which simplify handling of userprofile and related data.
     */
    public class UserProfileService
    {
        private readonly TtvContext _ttvContext;
        private readonly UtilityService _utilityService;

        public UserProfileService(TtvContext ttvContext, UtilityService utilityService)
        {
            _ttvContext = ttvContext;
            _utilityService = utilityService;
        }

        /*
         * Check if data related to FactFieldValue can be removed.
         */
        public bool CanDeleteFactFieldValueRelatedData(FactFieldValue ffv)
        {
            // ORCID and demo data can be removed.
            return ffv.SourceId == Constants.SourceIdentifiers.ORCID || ffv.SourceId == Constants.SourceIdentifiers.DEMO;
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
                    SourceId = Constants.SourceIdentifiers.ORCID,
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
                    SourceId = Constants.SourceIdentifiers.ORCID,
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
        public async Task<int> GetTiedejatutkimusFiRegisteredDataSourceId()
        {
            var tiedejatutkimusfiRegisteredDataSource = await _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefaultAsync(p => p.Name == Constants.SourceIdentifiers.TIEDEJATUTKIMUS);
            if (tiedejatutkimusfiRegisteredDataSource == null)
            {
                return -1;
            }
            else
            {
                return tiedejatutkimusfiRegisteredDataSource.Id;
            }
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
                    DimKnownPersonidFormerNames = -1,
                    SourceId = "",
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
                    SourceId = "",
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
                    SourceId = "",
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
                Show = false,
                PrimaryValue = false,
                SourceId = " ",
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime()
            };
        }

        /*
         * Get empty FactFieldValue. Set SourceID to ORCID.
         */
        public FactFieldValue GetEmptyFactFieldValueOrcid()
        {
            var factFieldValue = this.GetEmptyFactFieldValue();
            factFieldValue.SourceId = Constants.SourceIdentifiers.ORCID;
            return factFieldValue;
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
                SourceId = "",
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
                PidContent = " ",
                PidType = " ",
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
                SourceId = " ",
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
            foreach (DimName dimName in dimKnownPerson.DimNameDimKnownPersonIdConfirmedIdentityNavigations)
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

                    // Get DimFieldDisplaySetting for the registered data source.
                    var dimFieldDisplaySetting = await _ttvContext.DimFieldDisplaySettings
                        .Include(dfds => dfds.BrFieldDisplaySettingsDimRegisteredDataSources).AsNoTracking()
                            .FirstOrDefaultAsync(
                                dfds =>
                                    dfds.DimUserProfileId == dimUserProfile.Id &&
                                    dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION &&
                                    dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSourceId == dimNameRegisteredDataSource.Id
                            );

                    // If it was not found, then create DimFieldDisplaySetting for the registered data source.
                    if (dimFieldDisplaySetting == null)
                    {
                        dimFieldDisplaySetting = new DimFieldDisplaySetting()
                        {
                            DimUserProfileId = dimUserProfile.Id,
                            FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                            Show = false,
                            SourceId = " ",
                            SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                            Created = _utilityService.getCurrentDateTime(),
                            Modified = _utilityService.getCurrentDateTime()
                        };
                        dimFieldDisplaySetting.BrFieldDisplaySettingsDimRegisteredDataSources.Add(
                            new BrFieldDisplaySettingsDimRegisteredDataSource()
                            {
                                DimFieldDisplaySettingsId = dimFieldDisplaySetting.Id,
                                DimRegisteredDataSourceId = dimNameRegisteredDataSource.Id
                            }
                        );
                        _ttvContext.DimFieldDisplaySettings.Add(dimFieldDisplaySetting);

                    }
                    await _ttvContext.SaveChangesAsync();

                    // Add FactFieldValues for DimPublications
                    foreach (int publicationId in publicationsIds.Distinct())
                    {
                        var factFieldValuePublication = this.GetEmptyFactFieldValue();
                        factFieldValuePublication.DimUserProfileId = dimUserProfile.Id;
                        factFieldValuePublication.DimFieldDisplaySettingsId = dimFieldDisplaySetting.Id;
                        factFieldValuePublication.DimPublicationId = publicationId;
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
    }
}