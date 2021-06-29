using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{ 
    public class UserProfileService
    {
        private readonly TtvContext _ttvContext;

        public UserProfileService(TtvContext ttvContext)
        {
            _ttvContext = ttvContext;
        }

        // Check if data related to FactFieldValue can be removed.
        public bool CanDeleteFactFieldValueRelatedData(FactFieldValue ffv)
        {
            // ORCID and demo data can be removed.
            return ffv.SourceId == Constants.SourceIdentifiers.ORCID || ffv.SourceId == Constants.SourceIdentifiers.DEMO;
        }

        // Get id of DimUserProfile.
        public async Task<int> GetUserprofileId(String orcidId)
        {
            var dimPid = await _ttvContext.DimPids
                .Include(i => i.DimKnownPerson)
                    .ThenInclude(kp => kp.DimUserProfiles).AsNoTracking().AsSplitQuery().FirstOrDefaultAsync(p => p.PidContent == orcidId && p.PidType == "ORCID");

            if (dimPid == null || dimPid.DimKnownPerson == null || dimPid.DimKnownPerson.DimUserProfiles.Count() == 0)
            {
                return -1;
            }
            else
            {
                return dimPid.DimKnownPerson.DimUserProfiles.FirstOrDefault().Id;
            }
        }

        // Get id of ORCID DimRegisteredDataSource.
        public async Task<int> GetOrcidRegisteredDataSourceId()
        {
            var orcidRegisteredDataSource = await _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefaultAsync(p => p.Name == "ORCID");
            if (orcidRegisteredDataSource == null)
            {
                return -1;
            }
            else
            {
                return orcidRegisteredDataSource.Id;
            }
        }

        // Get id of Tiedejatutkimus.fi DimRegisteredDataSource.
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

        // Add or update DimName.
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
                    SourceId = "",
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = DateTime.Now,
                    DimRegisteredDataSourceId = dimRegisteredDataSourceId
                };
                _ttvContext.DimNames.Add(dimName);
            }
            else
            {
                dimName.LastName = lastName;
                dimName.FirstNames = firstNames;
                dimName.Modified = DateTime.Now;
            }
            await _ttvContext.SaveChangesAsync();
            return dimName;
        }

        // Add or update DimResearcherDescription.
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
                    Created = DateTime.Now,
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
                dimResearcherDescription.Modified = DateTime.Now;
            }
            await _ttvContext.SaveChangesAsync();
            return dimResearcherDescription;
        }

        // Add or update DimEmailAddrress.
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
                    Created = DateTime.Now,
                    DimKnownPersonId = dimKnownPersonId,
                    DimRegisteredDataSourceId = dimRegisteredDataSourceId
                };
                _ttvContext.DimEmailAddrresses.Add(dimEmailAddress);
            }
            else
            {
                dimEmailAddress.Modified = DateTime.Now;
            }
            await _ttvContext.SaveChangesAsync();
            return dimEmailAddress;
        }

        // Get empty FactFieldValue.
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
                Created = DateTime.Now,
                Modified = null
            };
        }

        // Get empty FactFieldValue. Set SourceID to ORCID.
        public FactFieldValue GetEmptyFactFieldValueOrcid()
        {
            var factFieldValue = this.GetEmptyFactFieldValue();
            factFieldValue.SourceId = Constants.SourceIdentifiers.ORCID;
            return factFieldValue;
        }

        // Get empty FactFieldValue. Set SourceID to DEMO.
        public FactFieldValue GetEmptyFactFieldValueDemo()
        {
            var factFieldValue = this.GetEmptyFactFieldValue();
            factFieldValue.SourceId = Constants.SourceIdentifiers.DEMO;
            return factFieldValue;
        }

        // Get empty DimOrcidPublication.
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
                DimRegisteredDataSourceId = -1,
                DimReferencedataid = -1
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

        // Add publications from DimPublication into user profile.
        public async Task AddTtvPublications(DimKnownPerson dimKnownPerson, DimUserProfile dimUserProfile)
        {
            // Loop DimNames, then related FactContributions. FactContribution may have relation to DimPublication (DimPublicationId != -1).
            // NOTE! Data source for DimPublication must be taken from DimName, not from DimPublication. Skip item if DimName does not have data source set.
            foreach (DimName dimName in dimKnownPerson.DimNames)
            {
                var publicationsIds = new List<int>();
                var dimNameRegisteredDataSource = dimName.DimRegisteredDataSource;

                if (dimNameRegisteredDataSource != null)
                {

                    foreach (FactContribution factContribution in dimName.FactContributions)
                    {
                        if (factContribution.DimPublicationId != -1)
                        {
                            publicationsIds.Add(factContribution.DimPublicationId);
                        }
                    }

                    // DimFieldDisplaySetting
                    var dimFieldDisplaySetting = await _ttvContext.DimFieldDisplaySettings
                        .Include(dfds => dfds.BrFieldDisplaySettingsDimRegisteredDataSources).AsNoTracking()
                            .FirstOrDefaultAsync(
                                dfds =>
                                    dfds.DimUserProfileId == dimUserProfile.Id &&
                                    dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION &&
                                    dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSourceId == dimNameRegisteredDataSource.Id
                            );
                    if (dimFieldDisplaySetting == null)
                    {
                        dimFieldDisplaySetting = new DimFieldDisplaySetting()
                        {
                            DimUserProfileId = dimUserProfile.Id,
                            FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                            Show = false,
                            SourceId = " ",
                            SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                            Created = DateTime.Now
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
                    foreach (int publicationId in publicationsIds)
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

        public async Task AddTtvDataToUserProfile(DimKnownPerson dimKnownPerson, DimUserProfile dimUserProfile)
        {
            await this.AddTtvPublications(dimKnownPerson, dimUserProfile);
        }
    }
}