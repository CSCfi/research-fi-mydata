using api.Models.Ttv;
using api.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;

namespace api.Tests.Profiledata
{
    public class AffiliationServiceTestData
    {
        public List<DimSector> DimSectors { get; private set; }
        public List<DimOrganization> DimOrganizations { get; private set; }
        public List<DimRegisteredDataSource> DimRegisteredDataSources { get; private set; }
        public List<DimFieldDisplaySetting> FieldDisplaySettings { get; private set; }
        public DimUserProfile UserProfile { get; private set; }
        public List<FactFieldValue> FactFieldValues { get; private set; }

        public static AffiliationServiceTestData Create()
        {
            var data = new AffiliationServiceTestData();
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
            DimFieldDisplaySetting dfdsActivityAffiliation = new DimFieldDisplaySetting { Id = 11, FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_AFFILIATION, SourceId = "Source1"};
            data.FieldDisplaySettings = new List<DimFieldDisplaySetting>
            {
                dfdsActivityAffiliation
            };

            /*
             * 1st affiliation
             *   - Has DimOrganization => OrganizationBroader.
             *   - Has all language versions populated.
             */
            FactFieldValue ffvAffiliation1 = userProfileService.GetEmptyFactFieldValue();
            ffvAffiliation1.DimUserProfileId = data.UserProfile.Id;
            ffvAffiliation1.DimUserProfile = data.UserProfile;
            ffvAffiliation1.DimFieldDisplaySettingsId = dfdsActivityAffiliation.Id; // ACTIVITY_AFFILIATION
            ffvAffiliation1.DimFieldDisplaySettings = dfdsActivityAffiliation;
            ffvAffiliation1.DimAffiliationId = 1000;
            ffvAffiliation1.DimAffiliation = new DimAffiliation {
                Id = 1000,
                PositionNameFi = "Affiliation 1 position name Fi",
                PositionNameEn = "Affiliation 1 position name En",
                PositionNameSv = "Affiliation 1 position name Sv",
                AffiliationTypeFi = "Affiliation 1 type Fi",
                AffiliationTypeEn = "Affiliation 1 type En",
                AffiliationTypeSv = "Affiliation 1 type Sv",
                StartDate = 1000,
                StartDateNavigation = new DimDate { Id = 1000, Year = 2021, Month = 2, Day = 1, SourceId = "Source1" },
                EndDate = 1001,
                EndDateNavigation = new DimDate { Id = 1001, Year = 2023, Month = 1, Day = 31, SourceId = "Source1" },
                DimOrganizationId = 1000,
                DimOrganization = new DimOrganization {
                    Id = 1000,
                    OrganizationId = "Affiliation 1 organization organizationId",
                    NameFi = "Affiliation 1 organization name Fi",
                    NameEn = "Affiliation 1 organization name En",
                    NameSv = "Affiliation 1 organization name Sv",
                    SourceId = "Source1",
                    DimSectorid = 1000,
                    DimSector = new DimSector { Id = 1000, SectorId = "S2", NameFi = "Sector 2 Fi", NameSv = "Sector 2 Sv", NameEn = "Sector 2 En", SourceId = "Source1" },
                    DimOrganizationBroader = 1001,
                    DimOrganizationBroaderNavigation = new DimOrganization {
                        Id = 1001,
                        OrganizationId = "Affiliation 1 organization broader organizationId",
                        NameFi = "Affiliation 1 organization broader name Fi",
                        NameEn = "Affiliation 1 organization broader name En",
                        NameSv = "Affiliation 1 organization broader name Sv",
                        SourceId = "Source1",
                        DimSectorid = 1001,
                        DimSector = new DimSector { Id = 1001, SectorId = "S3", NameFi = "Sector 3 Fi", NameSv = "Sector 3 Sv", NameEn = "Sector 3 En", SourceId = "Source1" }
                    }
                },
                SourceId = "Source1"
            };
            ffvAffiliation1.DimIdentifierlessDataId = -1;
            ffvAffiliation1.DimIdentifierlessData = new DimIdentifierlessDatum { Id = -1, SourceId = "Source1" };
            ffvAffiliation1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvAffiliation1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvAffiliation1.Show = true;
            ffvAffiliation1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvAffiliation1);

            /*
             * 2nd affiliation
             *   - Has DimOrganization, but that organization does not have OrganizationBroader.
             */
            FactFieldValue ffvAffiliation2 = userProfileService.GetEmptyFactFieldValue();
            ffvAffiliation2.DimUserProfileId = data.UserProfile.Id;
            ffvAffiliation2.DimUserProfile = data.UserProfile;
            ffvAffiliation2.DimFieldDisplaySettingsId = dfdsActivityAffiliation.Id; // ACTIVITY_AFFILIATION
            ffvAffiliation2.DimFieldDisplaySettings = dfdsActivityAffiliation;
            ffvAffiliation2.DimAffiliationId = 1001;
            ffvAffiliation2.DimAffiliation = new DimAffiliation {
                Id = 1001,
                PositionNameFi = "",
                PositionNameEn = "Affiliation 2 position name En",
                PositionNameSv = "",
                AffiliationTypeFi = "",
                AffiliationTypeEn = "Affiliation 2 type En",
                AffiliationTypeSv = "",
                StartDate = 1002,
                StartDateNavigation = new DimDate { Id = 1002, Year = 2020, Month = 3, Day = 13, SourceId = "Source1" },
                EndDate = 1003,
                EndDateNavigation = new DimDate { Id = 1003, Year = 2022, Month = 2, Day = 30, SourceId = "Source1" },
                DimOrganizationId = 1002,
                DimOrganization = new DimOrganization {
                    Id = 1002,
                    OrganizationId = "Affiliation 2 organization organizationId",
                    NameFi = "Affiliation 2 organization name Fi",
                    NameEn = "Affiliation 2 organization name En",
                    NameSv = "Affiliation 2 organization name Sv",
                    SourceId = "Source1",
                    DimSectorid = 1002,
                    DimSector = new DimSector { Id = 1002, SectorId = "S2", NameFi = "Sector 2 Fi", NameSv = "Sector 2 Sv", NameEn = "Sector 2 En", SourceId = "Source1" }
                },
                SourceId = "Source1"
            };
            ffvAffiliation1.DimIdentifierlessDataId = -1;
            ffvAffiliation1.DimIdentifierlessData = new DimIdentifierlessDatum { Id = -1, SourceId = "Source1" };
            ffvAffiliation2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvAffiliation2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvAffiliation2.Show = false;
            ffvAffiliation2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvAffiliation2);

            /*
             * 3rd affiliation
             *   - Does not have related DimOrganization, but has related DimIdentifierlessData.
             */
            FactFieldValue ffvAffiliation3 = userProfileService.GetEmptyFactFieldValue();
            ffvAffiliation3.DimUserProfileId = data.UserProfile.Id;
            ffvAffiliation3.DimUserProfile = data.UserProfile;
            ffvAffiliation3.DimFieldDisplaySettingsId = dfdsActivityAffiliation.Id; // ACTIVITY_AFFILIATION
            ffvAffiliation3.DimFieldDisplaySettings = dfdsActivityAffiliation;
            ffvAffiliation3.DimAffiliationId = 1002;
            ffvAffiliation3.DimAffiliation = new DimAffiliation {
                Id = 1002,
                PositionNameFi = "Affiliation 3 position name Fi",
                PositionNameEn = "Affiliation 3 position name En",
                PositionNameSv = "Affiliation 3 position name Sv",
                AffiliationTypeFi = "Affiliation 3 type Fi",
                AffiliationTypeEn = "Affiliation 3 type En",
                AffiliationTypeSv = "Affiliation 3 type Sv",
                StartDate = 1004,
                StartDateNavigation = new DimDate { Id = 1004, Year = 2020, Month = 3, Day = 13, SourceId = "Source1" },
                EndDate = 1005,
                EndDateNavigation = new DimDate { Id = 1005, Year = 2022, Month = 2, Day = 30, SourceId = "Source1" },
                DimOrganizationId = -1,
                DimOrganization = data.DimOrganizations[0],
                SourceId = "Source1"
            };
            ffvAffiliation3.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvAffiliation3.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvAffiliation3.Show = false;
            ffvAffiliation3.PrimaryValue = false;
            ffvAffiliation3.DimIdentifierlessDataId = 1000;
            ffvAffiliation3.DimIdentifierlessData = new DimIdentifierlessDatum {
                Id = 1000,
                Type = Constants.IdentifierlessDataTypes.ORGANIZATION_NAME,
                ValueFi = "Affiliation 3 identifierless data value Fi",
                ValueEn = "Affiliation 3 identifierless data value En",
                ValueSv = "Affiliation 3 identifierless data value Sv",
                DimIdentifierlessDataId = 1001,
                DimIdentifierlessData = new DimIdentifierlessDatum {
                    Id = 1001,
                    Type = Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT,
                    ValueFi = "Affiliation 3 identifierless data child value Fi",
                    ValueEn = "Affiliation 3 identifierless data child value En",
                    ValueSv = "Affiliation 3 identifierless data child value Sv",
                    SourceId = "Source1"
                },
                SourceId = "Source1"
            };
            data.FactFieldValues.Add(ffvAffiliation3);

            return data;
        }


        public async Task SeedAsync(TtvContext context)
        {
            context.FactFieldValues.AddRange(FactFieldValues);
            await context.SaveChangesAsync();
        }
    }
}