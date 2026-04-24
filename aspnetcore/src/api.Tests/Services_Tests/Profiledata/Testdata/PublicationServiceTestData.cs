using api.Models.Ttv;
using api.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;
using System.Linq;

namespace api.Tests.Profiledata
{
    public class PublicationServiceTestData
    {
        public List<DimSector> DimSectors { get; private set; }
        public List<DimOrganization> DimOrganizations { get; private set; }
        public List<DimRegisteredDataSource> DimRegisteredDataSources { get; private set; }
        public List<DimFieldDisplaySetting> FieldDisplaySettings { get; private set; }
        public DimUserProfile UserProfile { get; private set; }
        public List<FactFieldValue> FactFieldValues { get; private set; }

        public static PublicationServiceTestData Create()
        {
            var data = new PublicationServiceTestData();
            UtilityService utilityService = new UtilityService();
            DataSourceHelperService dataSourceHelperService = new DataSourceHelperService();
            
            UserProfileService userProfileService = new UserProfileService(utilityService: utilityService);

            data.UserProfile = new DimUserProfile { Id = 1, SourceId = "Source1" };
            data.DimSectors = new List<DimSector>();
            data.DimOrganizations = new List<DimOrganization>();
            data.DimRegisteredDataSources = new List<DimRegisteredDataSource>();
            data.DimSectors.Add(new DimSector { Id = -1, SectorId = "", NameFi = "", NameSv = "", NameEn = "", SourceId = "Source1" });
            data.DimSectors.Add(new DimSector { Id = 1, SectorId = "S1", NameFi = "Sector Fi", NameSv = "Sector Sv", NameEn = "Sector En", SourceId = "Source1" });
            data.DimOrganizations.Add(new DimOrganization { Id = -1, NameFi = "", NameEn = "", NameSv = "", DimSector = data.DimSectors[0], SourceId = "Source1" });
            data.DimOrganizations.Add(new DimOrganization { Id = 1, NameFi = "Org name Fi", NameEn = "Org name En", NameSv = "Org name Sv", DimSector = data.DimSectors[1], SourceId = "Source1" });
            data.DimOrganizations.Add(new DimOrganization { Id = 2, NameFi = "Org name", NameEn = "", NameSv = "", DimSector = data.DimSectors[1], SourceId = "Source1" });
            data.DimOrganizations.Add(new DimOrganization { Id = 3, NameFi = "TTV Fi", NameEn = "TTV En", NameSv = "TTV Sv", DimSector = data.DimSectors[1], SourceId = "Source1" });
            data.DimOrganizations.Add(new DimOrganization { Id = 4, NameFi = "", NameEn = "ORCID", NameSv = "", DimSector = data.DimSectors[1], SourceId = "Source1" });
            data.DimRegisteredDataSources.Add(new DimRegisteredDataSource { Id = 1, Name = "DataSource1", DimOrganization = data.DimOrganizations[1], SourceId = "Source1"});
            data.DimRegisteredDataSources.Add(new DimRegisteredDataSource { Id = 2, Name = "DataSource2", DimOrganization = data.DimOrganizations[2], SourceId = "Source1"});
            data.DimRegisteredDataSources.Add(new DimRegisteredDataSource { Id = 3, Name = "TTV" , DimOrganization = data.DimOrganizations[3], SourceId = "Source1"});
            data.DimRegisteredDataSources.Add(new DimRegisteredDataSource { Id = 4, Name = "ORCID" , DimOrganization = data.DimOrganizations[4], SourceId = "Source1"});
            data.FactFieldValues = new List<FactFieldValue>();
            DimFieldDisplaySetting dfdsActivityPublication = new DimFieldDisplaySetting { Id = 13, FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION, SourceId = "Source1"};
            DimFieldDisplaySetting dfdsActivityPublicationProfileOnly = new DimFieldDisplaySetting { Id = 14, FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY, SourceId = "Source1"};
            data.FieldDisplaySettings = new List<DimFieldDisplaySetting>
            {
                dfdsActivityPublication,
                dfdsActivityPublicationProfileOnly
            };
    

            /*
             * DimPublications
             *
             * Test scenario:
             *  - 3 publications:
             *      - DimPublication 1 and DimPublication 2 have the same PublicationId. They should be deduplicated based on PublicationId, and only one of them should be included in the results. Which one is included does not matter, as long as only one of them is included.
             *      - DimPublication 3 has a different PublicationId. It should not be deduplicated with DimPublication 1 or DimPublication 2.
             *      - DimPublication 3 has the same Doi as one of DimProfileOnlyPublications. They should be deduplicated based on Doi.
             */

            // DimPublication 1
            FactFieldValue ffvPublication1 = userProfileService.GetEmptyFactFieldValue();
            ffvPublication1.DimUserProfileId = data.UserProfile.Id;
            ffvPublication1.DimUserProfile = data.UserProfile;
            ffvPublication1.DimFieldDisplaySettingsId = dfdsActivityPublication.Id; // ACTIVITY_PUBLICATION
            ffvPublication1.DimFieldDisplaySettings = dfdsActivityPublication;
            ffvPublication1.DimPublicationId = 1;
            ffvPublication1.DimPublication = new DimPublication {
                Id = 1,
                ArticleNumberText = "DimPublication1 Article number text",
                AuthorsText = "DimPublication1 Authors text",
                ConferenceName = "DimPublication1 Conference name",
                DimLocallyReportedPubInfos = new List<DimLocallyReportedPubInfo> {
                    new DimLocallyReportedPubInfo {
                        Id = 1,
                        SelfArchivedUrl = "https://example.com/selfarchivedurl1",
                        SourceId = "Source1"
                    },
                },
                DimPids = new List<DimPid> {
                    new DimPid {
                        Id = 1,
                        PidType = Constants.PidTypes.DOI,
                        PidContent = "10.1234/doi_dimpublication_1",
                        SourceId = "Source1"
                    }
                },
                DoiHandle = null,
                IssueNumber = "DimPublication1 Issue number",
                JournalName = "DimPublication1 Journal name",
                OpenAccessCode = 5000,
                OpenAccessCodeNavigation = new DimReferencedatum {
                    Id = 5000,
                    CodeValue = "1",
                    CodeScheme = "AvoinSaatavuusKytkin",
                    SourceId = "Source1",
                    SourceDescription = ""
                },
                PageNumberText = "123",
                ParentPublicationName = "DimPublication1 Parent publication name",
                PublicationId = "DimPublication PublicationId to deduplicate", // Same PublicationId as DimPublication 2. Should be deduplicated based on PublicationId.
                PublicationName = "DimPublication1 Publication name",
                PublicationTypeCode = 6000,
                PublicationTypeCodeNavigation = new DimReferencedatum {
                    Id = 6000,
                    CodeValue = "A1",
                    CodeScheme = "julkaisutyyppiluokitus",
                    SourceId = "Source1",
                    SourceDescription = ""
                },
                PublicationYear = 2020,
                PublicationOrgId = "DimPublication1 Publication orgId",
                PublisherName = "DimPublication1 Publisher name",
                SelfArchivedCode = true,
                Volume = "DimPublication1 Volume number",
                SourceId = "Source1"
            };
            ffvPublication1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvPublication1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvPublication1.Show = true;
            ffvPublication1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvPublication1);

            // DimPublication 2
            FactFieldValue ffvPublication2 = userProfileService.GetEmptyFactFieldValue();
            ffvPublication2.DimUserProfileId = data.UserProfile.Id;
            ffvPublication2.DimUserProfile = data.UserProfile;
            ffvPublication2.DimFieldDisplaySettingsId = dfdsActivityPublication.Id; // ACTIVITY_PUBLICATION
            ffvPublication2.DimFieldDisplaySettings = dfdsActivityPublication;
            ffvPublication2.DimPublicationId = 2;
            ffvPublication2.DimPublication = new DimPublication {
                Id = 2,
                ArticleNumberText = "DimPublication2 Article number text",
                AuthorsText = "DimPublication2 Authors text",
                ConferenceName = "DimPublication2 Conference name",
                DimPids = new List<DimPid> {
                    new DimPid {
                        Id = 2,
                        PidType = Constants.PidTypes.DOI,
                        PidContent = "10.1234/doi_dimpublication_2",
                        SourceId = "Source1"
                    }
                },
                DoiHandle = null,
                IssueNumber = "DimPublication2 Issue number",
                JournalName = "DimPublication2 Journal name",
                OpenAccessCode = 5001,
                OpenAccessCodeNavigation = new DimReferencedatum {
                    Id = 5001,
                    CodeValue = "1",
                    CodeScheme = "AvoinSaatavuusKytkin",
                    SourceId = "Source1",
                    SourceDescription = ""
                },
                PageNumberText = "234",
                ParentPublicationName = "DimPublication2 Parent publication name",
                PublicationId = "DimPublication PublicationId to deduplicate", // Same PublicationId as DimPublication 1. Should be deduplicated based on PublicationId.
                PublicationName = "DimPublication2 Publication name",
                PublicationTypeCode = 6001,
                PublicationTypeCodeNavigation = new DimReferencedatum {
                    Id = 6001,
                    CodeValue = "A2",
                    CodeScheme = "julkaisutyyppiluokitus",
                    SourceId = "Source1",
                    SourceDescription = ""
                },
                PublicationYear = 2021,
                PublicationOrgId = "DimPublication2 Publication orgId",
                PublisherName = "DimPublication2 Publisher name",
                SelfArchivedCode = true,
                Volume = "DimPublication2 Volume number",
                SourceId = "Source1"
            };
            ffvPublication2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvPublication2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvPublication2.Show = false;
            ffvPublication2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvPublication2);

            // DimPublication 3
            FactFieldValue ffvPublication3 = userProfileService.GetEmptyFactFieldValue();
            ffvPublication3.DimUserProfileId = data.UserProfile.Id;
            ffvPublication3.DimUserProfile = data.UserProfile;
            ffvPublication3.DimFieldDisplaySettingsId = dfdsActivityPublication.Id; // ACTIVITY_PUBLICATION
            ffvPublication3.DimFieldDisplaySettings = dfdsActivityPublication;
            ffvPublication3.DimPublicationId = 3;
            ffvPublication3.DimPublication = new DimPublication {
                Id = 3,
                ArticleNumberText = "DimPublication3 Article number text",
                AuthorsText = "DimPublication3 Authors text",
                ConferenceName = "DimPublication3 Conference name",
                DimPids = new List<DimPid> {
                    new DimPid {
                        Id = 3,
                        PidType = Constants.PidTypes.DOI,
                        PidContent = "10.1234/doi_to_deduplicate", // Same Doi with one of DimProfileOnlyPublications. Should be deduplicated based on Doi.   
                        SourceId = "Source1"
                    }
                },
                DoiHandle = null, 
                IssueNumber = "DimPublication3 Issue number",
                JournalName = "DimPublication3 Journal name",
                OpenAccessCode = 5002,
                OpenAccessCodeNavigation = new DimReferencedatum {
                    Id = 5002,
                    CodeValue = "0",
                    CodeScheme = "AvoinSaatavuusKytkin",
                    SourceId = "Source1",
                    SourceDescription = ""
                },
                PageNumberText = "345",
                ParentPublicationName = "DimPublication3 Parent publication name",
                PublicationId = "DimPublication3 PublicationId", // Different PublicationId. Should not be deduplicated with DimPublication 1 or DimPublication 2.                 
                PublicationName = "DimPublication3 Publication name",
                PublicationTypeCode = -1,
                PublicationTypeCodeNavigation = new DimReferencedatum {
                    Id = -1,
                    CodeValue = "",
                    CodeScheme = "",
                    SourceId = "Source1",
                    SourceDescription = ""
                },
                PublicationYear = 2022,
                PublicationOrgId = "DimPublication3 Publication orgId",
                PublisherName = "DimPublication3 Publisher name",
                SelfArchivedCode = null,
                Volume = "DimPublication3 Volume number",
                SourceId = "Source1"
            };
            ffvPublication3.DimRegisteredDataSourceId = data.DimRegisteredDataSources[2].Id;
            ffvPublication3.DimRegisteredDataSource = data.DimRegisteredDataSources[2];
            ffvPublication3.Show = false;
            ffvPublication3.PrimaryValue = false;
            data.FactFieldValues.Add(ffvPublication3);

            /*
             * DimProfileOnlyPublications
             * Test scenario:
             *  - 2 publications:
             *      - DimProfileOnlyPublication 1 has the same Doi as one of DimPublications. They should be deduplicated based on Doi.
             *      - DimProfileOnlyPublication 2 has a different Doi. It should not be deduplicated with any of DimPublications.
             *      - DimProfileOnlyPublication 3 does not have Doi. It should not be deduplicated with any of DimPublications.
             */

            // Use ORCID datasource for DimProfileOnlyPublications
            DimRegisteredDataSource registeredDataSource_Orcid = data.DimRegisteredDataSources.FirstOrDefault(r => r.Name == "ORCID");

            // DimProfileOnlyPublication 1
            FactFieldValue ffvProfileOnlyPublication1 = userProfileService.GetEmptyFactFieldValue();
            ffvProfileOnlyPublication1.DimUserProfileId = data.UserProfile.Id;
            ffvProfileOnlyPublication1.DimUserProfile = data.UserProfile;
            ffvProfileOnlyPublication1.DimFieldDisplaySettingsId = dfdsActivityPublicationProfileOnly.Id; // ACTIVITY_PUBLICATION_PROFILE_ONLY
            ffvProfileOnlyPublication1.DimFieldDisplaySettings = dfdsActivityPublicationProfileOnly;
            ffvProfileOnlyPublication1.DimProfileOnlyPublicationId = 1;
            ffvProfileOnlyPublication1.DimProfileOnlyPublication = new DimProfileOnlyPublication {
                Id = 21,
                ArticleNumberText = "DimProfileOnlyPublication1 Article number text",
                AuthorsText = "DimProfileOnlyPublication1 Authors text",
                ConferenceName = "DimProfileOnlyPublication1 Conference name",
                DoiHandle = "10.1234/doi_to_deduplicate", // Same Doi with one of DimPublications. Should be deduplicated based on Doi.
                IssueNumber = "DimProfileOnlyPublication1 Issue number",
                OpenAccessCode = "1",
                PageNumberText = "456",
                ParentPublicationName = "DimPublication1 Parent publication name",
                PublicationId = "DimProfileOnlyPublication1 PublicationId",
                PublicationName = "DimProfileOnlyPublication1 Publication name",
                PublicationYear = 2023,
                PublisherName = "DimProfileOnlyPublication1 Publisher name",
                Volume = "DimProfileOnlyPublication1 Volume number",
                SourceId = "Source1"
            };
            ffvProfileOnlyPublication1.DimRegisteredDataSourceId = registeredDataSource_Orcid.Id;
            ffvProfileOnlyPublication1.DimRegisteredDataSource = registeredDataSource_Orcid;
            ffvProfileOnlyPublication1.Show = true;
            ffvProfileOnlyPublication1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvProfileOnlyPublication1);

            // DimProfileOnlyPublication 2
            FactFieldValue ffvProfileOnlyPublication2 = userProfileService.GetEmptyFactFieldValue();
            ffvProfileOnlyPublication2.DimUserProfileId = data.UserProfile.Id;
            ffvProfileOnlyPublication2.DimUserProfile = data.UserProfile;
            ffvProfileOnlyPublication2.DimFieldDisplaySettingsId = dfdsActivityPublicationProfileOnly.Id; // ACTIVITY_PUBLICATION_PROFILE_ONLY
            ffvProfileOnlyPublication2.DimFieldDisplaySettings = dfdsActivityPublicationProfileOnly;
            ffvProfileOnlyPublication2.DimProfileOnlyPublicationId = 2;
            ffvProfileOnlyPublication2.DimProfileOnlyPublication = new DimProfileOnlyPublication {
                Id = 22,
                ArticleNumberText = "DimProfileOnlyPublication2 Article number text",
                AuthorsText = "DimProfileOnlyPublication2 Authors text",
                ConferenceName = "DimProfileOnlyPublication2 Conference name",
                DoiHandle = "10.1234/doi_dimprofileonlypublication_2", // Different Doi. Should not be deduplicated with any of DimPublications.
                IssueNumber = "DimProfileOnlyPublication2 Issue number",
                OpenAccessCode = "1",
                PageNumberText = "567",
                ParentPublicationName = "DimProfileOnlyPublication2 Parent publication name",
                PublicationId = "DimProfileOnlyPublication2 PublicationId",
                PublicationName = "DimProfileOnlyPublication2 Publication name",
                PublicationYear = 2024,
                PublisherName = "DimProfileOnlyPublication2 Publisher name",
                Volume = "DimProfileOnlyPublication2 Volume number",
                SourceId = "Source1"
            };
            ffvProfileOnlyPublication2.DimRegisteredDataSourceId = registeredDataSource_Orcid.Id;
            ffvProfileOnlyPublication2.DimRegisteredDataSource = registeredDataSource_Orcid;
            ffvProfileOnlyPublication2.Show = false;
            ffvProfileOnlyPublication2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvProfileOnlyPublication2);

            // DimProfileOnlyPublication 3
            FactFieldValue ffvProfileOnlyPublication3 = userProfileService.GetEmptyFactFieldValue();
            ffvProfileOnlyPublication3.DimUserProfileId = data.UserProfile.Id;
            ffvProfileOnlyPublication3.DimUserProfile = data.UserProfile;
            ffvProfileOnlyPublication3.DimFieldDisplaySettingsId = dfdsActivityPublicationProfileOnly.Id; // ACTIVITY_PUBLICATION_PROFILE_ONLY
            ffvProfileOnlyPublication3.DimFieldDisplaySettings = dfdsActivityPublicationProfileOnly;
            ffvProfileOnlyPublication3.DimProfileOnlyPublicationId = 3;
            ffvProfileOnlyPublication3.DimProfileOnlyPublication = new DimProfileOnlyPublication {
                Id = 23,
                ArticleNumberText = null,
                AuthorsText = "DimProfileOnlyPublication3 Authors text",
                ConferenceName = null,
                DoiHandle = "", // Doi not available.
                IssueNumber = null,
                OpenAccessCode = null, // Open access code not available.
                PageNumberText = null,
                ParentPublicationName = null,
                PublicationId = "", // PublicationId not available.
                PublicationName = "DimProfileOnlyPublication3 Publication name",
                PublicationYear = 2025,
                PublisherName = null,
                Volume = null,
                SourceId = "Source1"
            };
            ffvProfileOnlyPublication3.DimRegisteredDataSourceId = registeredDataSource_Orcid.Id;
            ffvProfileOnlyPublication3.DimRegisteredDataSource = registeredDataSource_Orcid;
            ffvProfileOnlyPublication3.Show = false;
            ffvProfileOnlyPublication3.PrimaryValue = false;
            data.FactFieldValues.Add(ffvProfileOnlyPublication3);

            return data;
        }


        public async Task SeedAsync(TtvContext context)
        {
            context.FactFieldValues.AddRange(FactFieldValues);
            await context.SaveChangesAsync();
        }
    }
}