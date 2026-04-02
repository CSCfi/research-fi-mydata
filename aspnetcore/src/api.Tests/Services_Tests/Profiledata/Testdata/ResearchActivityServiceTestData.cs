using api.Models.Ttv;
using api.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;

namespace api.Tests.Profiledata
{
    public class ResearchActivityServiceTestData
    {
        public List<DimSector> DimSectors { get; private set; }
        public List<DimOrganization> DimOrganizations { get; private set; }
        public List<DimRegisteredDataSource> DimRegisteredDataSources { get; private set; }
        public List<DimFieldDisplaySetting> FieldDisplaySettings { get; private set; }
        public DimUserProfile UserProfile { get; private set; }
        public List<FactFieldValue> FactFieldValues { get; private set; }

        public static ResearchActivityServiceTestData Create()
        {
            var data = new ResearchActivityServiceTestData();
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
            DimFieldDisplaySetting dfdsResearchActivity = new DimFieldDisplaySetting { Id = 1, FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_RESEARCH_ACTIVITY, SourceId = "Source1"};
            data.FieldDisplaySettings = new List<DimFieldDisplaySetting>
            {
                dfdsResearchActivity
            };
    
            /*
             * 1st DimResearchActivity:
             *   - Has DimOrganization => OrganizationBroader.
             *   - Has all language versions populated.
             */
            FactFieldValue ffvDimResearchActivity1 = userProfileService.GetEmptyFactFieldValue();
            ffvDimResearchActivity1.DimResearchActivityId = 1;
            ffvDimResearchActivity1.DimResearchActivity = new DimResearchActivity {
                Id = 100,
                DescriptionFi = "DimResearchActivity1 description fi",
                DescriptionSv = "DimResearchActivity1 description sv",
                DescriptionEn = "DimResearchActivity1 description en",
                DimEndDate = 100,
                DimEndDateNavigation = new DimDate { Id = 100, Year = 2020, Month = 2, Day = 22, SourceId = "Source1" },
                DimOrganizationId = 100,
                DimOrganization = new DimOrganization {
                    Id = 100,
                    OrganizationId = "DimResearchActivity1 organization organizationId",
                    NameFi = "DimResearchActivity1 organization name fi",
                    NameEn = "DimResearchActivity1 organization name en",
                    NameSv = "DimResearchActivity1 organization name sv",
                    SourceId = "Source1",
                    DimSectorid = 1000,
                    DimSector = new DimSector { Id = 1000, SectorId = "S2", NameFi = "Sector 2 fi", NameSv = "Sector 2 sv", NameEn = "Sector 2 en", SourceId = "Source1" },
                    DimOrganizationBroader = 101,
                    DimOrganizationBroaderNavigation = new DimOrganization {
                        Id = 101,
                        OrganizationId = "DimResearchActivity1 organization broader organizationId",
                        NameFi = "DimResearchActivity1 organization broader name fi",
                        NameEn = "DimResearchActivity1 organization broader name en",
                        NameSv = "DimResearchActivity1 organization broader name sv",
                        SourceId = "Source1",
                        DimSectorid = 100,
                        DimSector = new DimSector { Id = 100, SectorId = "S3", NameFi = "Sector 3 fi", NameSv = "Sector 3 sv", NameEn = "Sector 3 en", SourceId = "Source1" }
                    }
                },
                DimPids = new List<DimPid>(),
                DimStartDate = 101,
                DimStartDateNavigation = new DimDate { Id = 101, Year = 2019, Month = 1, Day = 11, SourceId = "Source1" },
                DimWebLinks = new List<DimWebLink>()
                {
                    new DimWebLink { Id = 101, Url = "https://example.com/dim-research-activity-1-wl1", LinkLabel = "DimResearchActivity1 Web Link 1", LinkType = "ProfileEditorWebLink", SourceId = "Source1" },
                    new DimWebLink { Id = 102, Url = "https://example.com/dim-research-activity-1-wl2", LinkLabel = "DimResearchActivity1 Web Link 2", LinkType = "ProfileEditorWebLink", SourceId = "Source1" }
                },
                InternationalCollaboration = true,
                LocalIdentifier = "DimResearchActivity1 local identifier",
                NameFi = "DimResearchActivity1 name fi",
                NameSv = "DimResearchActivity1 name sv",
                NameEn = "DimResearchActivity1 name en",
                SourceId = "Source1"
            };
            ffvDimResearchActivity1.DimUserProfileId = data.UserProfile.Id;
            ffvDimResearchActivity1.DimUserProfile = data.UserProfile;
            ffvDimResearchActivity1.DimFieldDisplaySettingsId = dfdsResearchActivity.Id; // RESEARCH_ACTIVITY
            ffvDimResearchActivity1.DimFieldDisplaySettings = dfdsResearchActivity;
            ffvDimResearchActivity1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvDimResearchActivity1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvDimResearchActivity1.Show = true;
            ffvDimResearchActivity1.PrimaryValue = true;
            ffvDimResearchActivity1.DimIdentifierlessDataId = -1;
            ffvDimResearchActivity1.DimIdentifierlessData = new DimIdentifierlessDatum { Id = -1, SourceId = "Source1" };
            data.FactFieldValues.Add(ffvDimResearchActivity1);
            
            /*
             * 2nd DimResearchActivity:
             *   - Has DimOrganization, but that organization does not have OrganizationBroader.
             *   - Not all language versions populated.
             */
            FactFieldValue ffvDimResearchActivity2 = userProfileService.GetEmptyFactFieldValue();
            ffvDimResearchActivity2.DimResearchActivityId = 2;
            ffvDimResearchActivity2.DimResearchActivity = new DimResearchActivity {
                Id = 200,
                DescriptionFi = "DimResearchActivity2 description",
                DescriptionSv = "",
                DescriptionEn = "",
                DimEndDate = 200,
                DimEndDateNavigation = new DimDate { Id = 200, Year = 2022, Month = 3, Day = 23, SourceId = "Source1" },
                DimOrganizationId = 200,
                DimOrganization = new DimOrganization {
                    Id = 200,
                    OrganizationId = "DimResearchActivity2 organization organizationId",
                    NameFi = "DimResearchActivity2 organization name",
                    NameEn = "",
                    NameSv = "",
                    SourceId = "Source1",
                    DimSectorid = 200,
                    DimSector = new DimSector { Id = 200, SectorId = "S2", NameFi = "Sector 2 Fi", NameSv = "Sector 2 Sv", NameEn = "Sector 2 En", SourceId = "Source1" }
                },
                DimPids = new List<DimPid>(),
                DimStartDate = 201,
                DimStartDateNavigation = new DimDate { Id = 201, Year = 2021, Month = 2, Day = 12, SourceId = "Source1" },
                DimWebLinks = new List<DimWebLink>()
                {
                    new DimWebLink { Id = 201, Url = "https://example.com/research-activity-2-wl1", LinkLabel = "DimResearchActivity2 Web Link 1", LinkType = "ProfileEditorWebLink", SourceId = "Source1" },
                    new DimWebLink { Id = 202, Url = "https://example.com/research-activity-2-wl2", LinkLabel = "DimResearchActivity2 Web Link 2", LinkType = "ProfileEditorWebLink", SourceId = "Source1" }
                },
                InternationalCollaboration = true,
                LocalIdentifier = "DimResearchActivity2 local identifier",
                NameFi = "DimResearchActivity2 name",
                NameSv = "",
                NameEn = "",
                SourceId = "Source1"
            };
            ffvDimResearchActivity2.DimUserProfileId = data.UserProfile.Id;
            ffvDimResearchActivity2.DimUserProfile = data.UserProfile;
            ffvDimResearchActivity2.DimFieldDisplaySettingsId = dfdsResearchActivity.Id; // RESEARCH_ACTIVITY
            ffvDimResearchActivity2.DimFieldDisplaySettings = dfdsResearchActivity;
            ffvDimResearchActivity2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvDimResearchActivity2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvDimResearchActivity2.Show = false;
            ffvDimResearchActivity2.PrimaryValue = false;
            ffvDimResearchActivity2.DimIdentifierlessDataId = -2;
            ffvDimResearchActivity2.DimIdentifierlessData = new DimIdentifierlessDatum { Id = -2, SourceId = "Source1" };
            data.FactFieldValues.Add(ffvDimResearchActivity2);

            /*
             * 3rd DimResearchActivity:
             *   - Does not have related DimOrganization, but has related DimIdentifierlessData.
             */
            FactFieldValue ffvDimResearchActivity3 = userProfileService.GetEmptyFactFieldValue();
            ffvDimResearchActivity3.DimResearchActivityId = 3;
            ffvDimResearchActivity3.DimResearchActivity = new DimResearchActivity {
                Id = 300,
                DescriptionFi = "DimResearchActivity3 description fi",
                DescriptionSv = "DimResearchActivity3 description sv",
                DescriptionEn = "DimResearchActivity3 description en",
                DimEndDate = 300,
                DimEndDateNavigation = new DimDate { Id = 300, Year = 2024, Month = 5, Day = 16, SourceId = "Source1" },
                DimOrganizationId = -1,
                DimOrganization = data.DimOrganizations[0],
                DimPids = new List<DimPid>(),
                DimStartDate = 301,
                DimStartDateNavigation = new DimDate { Id = 301, Year = 2023, Month = 4, Day = 15, SourceId = "Source1" },
                DimWebLinks = new List<DimWebLink>()
                {
                    new DimWebLink { Id = 301, Url = "https://example.com/dim-research-activity-3-wl1", LinkLabel = "DimResearchActivity3 Web Link 1", LinkType = "ProfileEditorWebLink", SourceId = "Source1" },
                    new DimWebLink { Id = 302, Url = "https://example.com/dim-research-activity-3-wl2", LinkLabel = "DimResearchActivity3 Web Link 2", LinkType = "ProfileEditorWebLink", SourceId = "Source1" }
                },
                InternationalCollaboration = true,
                LocalIdentifier = "DimResearchActivity3 local identifier",
                NameFi = "DimResearchActivity3 name fi",
                NameSv = "DimResearchActivity3 name sv",
                NameEn = "DimResearchActivity3 name en",
                SourceId = "Source1"
            };
            ffvDimResearchActivity3.DimUserProfileId = data.UserProfile.Id;
            ffvDimResearchActivity3.DimUserProfile = data.UserProfile;
            ffvDimResearchActivity3.DimFieldDisplaySettingsId = dfdsResearchActivity.Id; // RESEARCH_ACTIVITY
            ffvDimResearchActivity3.DimFieldDisplaySettings = dfdsResearchActivity;
            ffvDimResearchActivity3.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvDimResearchActivity3.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvDimResearchActivity3.Show = true;
            ffvDimResearchActivity3.PrimaryValue = true;
            ffvDimResearchActivity3.DimIdentifierlessDataId = 300;
            ffvDimResearchActivity3.DimIdentifierlessData = new DimIdentifierlessDatum {
                Id = 300,
                Type = Constants.IdentifierlessDataTypes.ORGANIZATION_NAME,
                ValueFi = "DimResearchActivity3 identifierless data value fi",
                ValueEn = "DimResearchActivity3 identifierless data value en",
                ValueSv = "DimResearchActivity3 identifierless data value sv",
                DimIdentifierlessDataId = 301,
                DimIdentifierlessData = new DimIdentifierlessDatum {
                    Id = 301,
                    Type = Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT,
                    ValueFi = "DimResearchActivity3 identifierless data child value fi",
                    ValueEn = "DimResearchActivity3 identifierless data child value en",
                    ValueSv = "DimResearchActivity3 identifierless data child value sv",
                    SourceId = "Source1"
                },
                SourceId = "Source1"
            };
            data.FactFieldValues.Add(ffvDimResearchActivity3);

            /*
             * 1st DimProfileOnlyResearchActivity:
             *   - Should be deduplicated.
             */

            /*
             * 2nd DimProfileOnlyResearchActivity:
             *   - Should not be deduplicated.
             */

            return data;
        }

        public async Task SeedAsync(TtvContext context)
        {
            context.FactFieldValues.AddRange(FactFieldValues);
            await context.SaveChangesAsync();
        }
    }
}