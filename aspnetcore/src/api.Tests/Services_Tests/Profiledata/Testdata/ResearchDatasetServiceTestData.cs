using api.Models.Ttv;
using api.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;
using System.Linq;

namespace api.Tests.Profiledata
{
    public class ResearchDatasetServiceTestData
    {
        public List<DimSector> DimSectors { get; private set; }
        public List<DimOrganization> DimOrganizations { get; private set; }
        public List<DimRegisteredDataSource> DimRegisteredDataSources { get; private set; }
        public List<DimFieldDisplaySetting> FieldDisplaySettings { get; private set; }
        public DimUserProfile UserProfile { get; private set; }
        public List<FactFieldValue> FactFieldValues { get; private set; }

        public static ResearchDatasetServiceTestData Create()
        {
            var data = new ResearchDatasetServiceTestData();
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
            data.DimRegisteredDataSources.Add(new DimRegisteredDataSource { Id = 1, Name = "DataSource1", DimOrganization = data.DimOrganizations[1], SourceId = "Source1"});
            data.DimRegisteredDataSources.Add(new DimRegisteredDataSource { Id = 2, Name = "DataSource2", DimOrganization = data.DimOrganizations[2], SourceId = "Source1"});
            data.FactFieldValues = new List<FactFieldValue>();
            DimFieldDisplaySetting dfdsResearchDataset = new DimFieldDisplaySetting { Id = 1, FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET, SourceId = "Source1"};
            data.FieldDisplaySettings = new List<DimFieldDisplaySetting>
            {
                dfdsResearchDataset
            };

            // DimResearchDataset 1 
            FactFieldValue ffvDimResearchDataset1 = userProfileService.GetEmptyFactFieldValue();
            ffvDimResearchDataset1.DimUserProfileId = data.UserProfile.Id;
            ffvDimResearchDataset1.DimUserProfile = data.UserProfile;
            ffvDimResearchDataset1.DimFieldDisplaySettingsId = dfdsResearchDataset.Id; // RESEARCH_DATASET
            ffvDimResearchDataset1.DimFieldDisplaySettings = dfdsResearchDataset;
            ffvDimResearchDataset1.DimResearchDatasetId = 123;
            ffvDimResearchDataset1.DimResearchDataset = new DimResearchDataset {
                Id = 123,
                DatasetCreated = new System.DateTime(1999, 1, 1),
                DimDescriptiveItems = new List<DimDescriptiveItem>()
                {
                    new DimDescriptiveItem { Id = 1, DescriptiveItemType = "name", DescriptiveItemLanguage = "fi", DescriptiveItem = "dimResearchDataset1 name fi", SourceId = "Source1" },
                    new DimDescriptiveItem { Id = 2, DescriptiveItemType = "name", DescriptiveItemLanguage = "sv", DescriptiveItem = "dimResearchDataset1 name sv", SourceId = "Source1" },
                    new DimDescriptiveItem { Id = 3, DescriptiveItemType = "name", DescriptiveItemLanguage = "en", DescriptiveItem = "dimResearchDataset1 name en", SourceId = "Source1" },
                    new DimDescriptiveItem { Id = 4, DescriptiveItemType = "description", DescriptiveItemLanguage = "fi", DescriptiveItem = "dimResearchDataset1 description fi", SourceId = "Source1" },
                    new DimDescriptiveItem { Id = 5, DescriptiveItemType = "description", DescriptiveItemLanguage = "sv", DescriptiveItem = "dimResearchDataset1 description sv", SourceId = "Source1" },
                    new DimDescriptiveItem { Id = 6, DescriptiveItemType = "description", DescriptiveItemLanguage = "en", DescriptiveItem = "dimResearchDataset1 description en", SourceId = "Source1" }
                },
                DimReferencedataAvailability = 321,
                DimReferencedataAvailabilityNavigation = new DimReferencedatum {
                    Id = 321,
                    CodeScheme = "dimResearchDataset1 DimReferencedataAvailability CodeScheme",
                    CodeValue = "dimResearchDataset1 DimReferencedataAvailability CodeValue",
                    SourceId = "Source1",
                    SourceDescription = "SourceDescription"
                },
                LocalIdentifier = "dimResearchDataset1 LocalIdentifier",
                DimPids = new List<DimPid>()
                {
                    new DimPid { Id = 300, PidType = "dimResearchDataset1 DimPid1 type", PidContent = "dimResearchDataset1 DimPid1 content", SourceId = "Source1" },
                    new DimPid { Id = 301, PidType = "dimResearchDataset1 DimPid2 type", PidContent = "dimResearchDataset1 DimPid2 content", SourceId = "Source1" }
                },
                SourceId = "Source1"
            };
            ffvDimResearchDataset1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvDimResearchDataset1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvDimResearchDataset1.Show = true;
            ffvDimResearchDataset1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvDimResearchDataset1);

            // DimResearchDataset 2 
            FactFieldValue ffvDimResearchDataset2 = userProfileService.GetEmptyFactFieldValue();
            ffvDimResearchDataset2.DimUserProfileId = data.UserProfile.Id;
            ffvDimResearchDataset2.DimUserProfile = data.UserProfile;
            ffvDimResearchDataset2.DimFieldDisplaySettingsId = dfdsResearchDataset.Id; // RESEARCH_DATASET
            ffvDimResearchDataset2.DimFieldDisplaySettings = dfdsResearchDataset;
            ffvDimResearchDataset2.DimResearchDatasetId = 124;
            ffvDimResearchDataset2.DimResearchDataset = new DimResearchDataset {
                Id = 124,
                DatasetCreated = null,
                DimDescriptiveItems = new List<DimDescriptiveItem>()
                {
                    // Only fi defined.
                    new DimDescriptiveItem { Id = 10, DescriptiveItemType = "name", DescriptiveItemLanguage = "fi", DescriptiveItem = "dimResearchDataset2 name", SourceId = "Source1" },
                    new DimDescriptiveItem { Id = 11, DescriptiveItemType = "description", DescriptiveItemLanguage = "fi", DescriptiveItem = "dimResearchDataset2 description", SourceId = "Source1" }
                },
                DimReferencedataAvailability = 322,
                DimReferencedataAvailabilityNavigation = new DimReferencedatum {
                    Id = 322,
                    CodeScheme = "dimResearchDataset2 DimReferencedataAvailability CodeScheme",
                    CodeValue = "dimResearchDataset2 DimReferencedataAvailability CodeValue",
                    SourceId = "Source1",
                    SourceDescription = "SourceDescription"
                },
                LocalIdentifier = "dimResearchDataset2 LocalIdentifier",
                DimPids = new List<DimPid>(),
                SourceId = "Source1"
            };
            ffvDimResearchDataset2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvDimResearchDataset2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvDimResearchDataset2.Show = true;
            ffvDimResearchDataset2.PrimaryValue = true;
            data.FactFieldValues.Add(ffvDimResearchDataset2);


            /*
             * DimProfileOnlyDatasets
             */

            // DimProfileOnlyDataset 1
            FactFieldValue ffvDimProfileOnlyDataset1 = userProfileService.GetEmptyFactFieldValue();
            ffvDimProfileOnlyDataset1.DimUserProfileId = data.UserProfile.Id;
            ffvDimProfileOnlyDataset1.DimUserProfile = data.UserProfile;
            ffvDimProfileOnlyDataset1.DimFieldDisplaySettingsId = dfdsResearchDataset.Id; // RESEARCH_DATASET
            ffvDimProfileOnlyDataset1.DimFieldDisplaySettings = dfdsResearchDataset;
            ffvDimProfileOnlyDataset1.DimProfileOnlyDatasetId = 234;
            ffvDimProfileOnlyDataset1.DimProfileOnlyDataset = new DimProfileOnlyDataset {
                Id = 234,
                DatasetCreated = new System.DateTime(2005, 1, 1),
                DescriptionFi = "dimProfileOnlyDataset1 description fi",
                DescriptionSv = "dimProfileOnlyDataset1 description sv",
                DescriptionEn = "dimProfileOnlyDataset1 description en",
                LocalIdentifier = "dimProfileOnlyDataset1 LocalIdentifier",
                NameFi = "dimProfileOnlyDataset1 name fi",
                NameEn = "dimProfileOnlyDataset1 name en",
                NameSv = "dimProfileOnlyDataset1 name sv",
                DimWebLinks = new List<DimWebLink>()
                {
                    new DimWebLink {
                        Id = 400,
                        Url = "https://example.com/dimProfileOnlyDataset1",
                        SourceId = "Source1"
                    }
                },
                SourceId = "Source2"
            };
            ffvDimProfileOnlyDataset1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvDimProfileOnlyDataset1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvDimProfileOnlyDataset1.Show = true;
            ffvDimProfileOnlyDataset1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvDimProfileOnlyDataset1);

            // DimProfileOnlyDataset 2
            FactFieldValue ffvDimProfileOnlyDataset2 = userProfileService.GetEmptyFactFieldValue();
            ffvDimProfileOnlyDataset2.DimUserProfileId = data.UserProfile.Id;
            ffvDimProfileOnlyDataset2.DimUserProfile = data.UserProfile;
            ffvDimProfileOnlyDataset2.DimFieldDisplaySettingsId = dfdsResearchDataset.Id; // RESEARCH_DATASET
            ffvDimProfileOnlyDataset2.DimFieldDisplaySettings = dfdsResearchDataset;
            ffvDimProfileOnlyDataset2.DimProfileOnlyDatasetId = 235;
            ffvDimProfileOnlyDataset2.DimProfileOnlyDataset = new DimProfileOnlyDataset {
                Id = 235,
                DatasetCreated = null,
                DescriptionFi = "dimProfileOnlyDataset2 description",
                DescriptionSv = "",
                DescriptionEn = "",
                LocalIdentifier = "dimProfileOnlyDataset2 LocalIdentifier",
                NameFi = "dimProfileOnlyDataset2 name",
                NameEn = "",
                NameSv = "",
                DimWebLinks = new List<DimWebLink>(),
                SourceId = "Source2"
            };
            ffvDimProfileOnlyDataset2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvDimProfileOnlyDataset2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvDimProfileOnlyDataset2.Show = false;
            ffvDimProfileOnlyDataset2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvDimProfileOnlyDataset2);

            return data;
        }


        public async Task SeedAsync(TtvContext context)
        {
            context.FactFieldValues.AddRange(FactFieldValues);
            await context.SaveChangesAsync();
        }
    }
}