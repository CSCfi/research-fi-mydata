using api.Models.Ttv;
using api.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;
using System.Linq;

namespace api.Tests.Profiledata
{
    public class FundingDecisionServiceTestData
    {
        public List<DimSector> DimSectors { get; private set; }
        public List<DimOrganization> DimOrganizations { get; private set; }
        public List<DimRegisteredDataSource> DimRegisteredDataSources { get; private set; }
        public List<DimFieldDisplaySetting> FieldDisplaySettings { get; private set; }
        public DimUserProfile UserProfile { get; private set; }
        public List<FactFieldValue> FactFieldValues { get; private set; }

        public static FundingDecisionServiceTestData Create()
        {
            var data = new FundingDecisionServiceTestData();
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
            DimFieldDisplaySetting dfdsFundingDecision = new DimFieldDisplaySetting { Id = 1, FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION, SourceId = "Source1"};
            data.FieldDisplaySettings = new List<DimFieldDisplaySetting>
            {
                dfdsFundingDecision
            };

            // DimFundingDecision 1 
            FactFieldValue ffvDimFundingDecision1 = userProfileService.GetEmptyFactFieldValue();
            ffvDimFundingDecision1.DimUserProfileId = data.UserProfile.Id;
            ffvDimFundingDecision1.DimUserProfile = data.UserProfile;
            ffvDimFundingDecision1.DimFieldDisplaySettingsId = dfdsFundingDecision.Id; // FUNDING_DECISION
            ffvDimFundingDecision1.DimFieldDisplaySettings = dfdsFundingDecision;
            ffvDimFundingDecision1.DimFundingDecisionId = 123;
            ffvDimFundingDecision1.DimFundingDecision = new DimFundingDecision {
                Id = 123,
                Acronym = "dimFundingDecision1 acronym",
                AmountInEur = 123.45m,
                DimCallProgrammeId = 1,
                DimCallProgramme = new DimCallProgramme {
                    Id = 1,
                    NameFi = "dimFundingDecision1 call programme",
                    NameEn = "",
                    NameSv = "",
                    SourceId = "Source1"
                },
                DimDateIdEnd = 1,
                DimDateIdEndNavigation = new DimDate {
                    Id = 1,
                    Year = 2020,
                    Month = 12,
                    Day = 31,
                    SourceId = "Source1"
                },
                DimDateIdStart = 2,
                DimDateIdStartNavigation = new DimDate {
                    Id = 2,
                    Year = 2019,
                    Month = 1,
                    Day = 1,
                    SourceId = "Source1"
                },
                DescriptionFi = "dimFundingDecision1 description",
                DescriptionEn = "",
                DescriptionSv = "",
                FunderProjectNumber = "dimFundingDecision1 funder project number",
                NameFi = "dimFundingDecision1 name",
                NameEn = "",
                NameSv = "",
                DimTypeOfFundingId = 1,
                DimTypeOfFunding = new DimReferencedatum {
                    Id = 1,
                    CodeScheme = "rahoitusmuoto",
                    CodeValue = "dimFundingDecision1 type of funding code value",
                    NameFi = "dimFundingDecision1 type of funding name",
                    NameEn = "",
                    NameSv = "",
                    SourceId = "Source1",
                    SourceDescription = "Source description"
                },
                DimOrganizationIdFunder = data.DimOrganizations[1].Id,
                DimOrganizationIdFunderNavigation = data.DimOrganizations[1],
                SourceId = "Source1"
            };
            ffvDimFundingDecision1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvDimFundingDecision1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvDimFundingDecision1.Show = true;
            ffvDimFundingDecision1.PrimaryValue = true;
            data.FactFieldValues.Add(ffvDimFundingDecision1);

            // DimFundingDecision 2 
            FactFieldValue ffvDimFundingDecision2 = userProfileService.GetEmptyFactFieldValue();
            ffvDimFundingDecision2.DimUserProfileId = data.UserProfile.Id;
            ffvDimFundingDecision2.DimUserProfile = data.UserProfile;
            ffvDimFundingDecision2.DimFieldDisplaySettingsId = dfdsFundingDecision.Id; // FUNDING_DECISION
            ffvDimFundingDecision2.DimFieldDisplaySettings = dfdsFundingDecision;
            ffvDimFundingDecision2.DimFundingDecisionId = 124;
            ffvDimFundingDecision2.DimFundingDecision = new DimFundingDecision {
                Id = 124,
                Acronym = "dimFundingDecision2 acronym",
                AmountInEur = 124.45m,
                DimCallProgrammeId = 2,
                DimCallProgramme = new DimCallProgramme {
                    Id = 2,
                    NameFi = "dimFundingDecision2 call programme name fi",
                    NameEn = "dimFundingDecision2 call programme name en",
                    NameSv = "dimFundingDecision2 call programme name sv",
                    SourceId = "Source1"
                },
                DimDateIdEnd = 3,
                DimDateIdEndNavigation = new DimDate {
                    Id = 3,
                    Year = 2021,
                    Month = 12,
                    Day = 31,
                    SourceId = "Source1"
                },
                DimDateIdStart = 4,
                DimDateIdStartNavigation = new DimDate {
                    Id = 4,
                    Year = 2020,
                    Month = 1,
                    Day = 1,
                    SourceId = "Source1"
                },
                DescriptionFi = "dimFundingDecision2 description fi",
                DescriptionEn = "dimFundingDecision2 description en",
                DescriptionSv = "dimFundingDecision2 description sv",
                FunderProjectNumber = "dimFundingDecision2 funder project number",
                NameFi = "dimFundingDecision2 name fi",
                NameEn = "dimFundingDecision2 name en",
                NameSv = "dimFundingDecision2 name sv",
                DimTypeOfFundingId = 2,
                DimTypeOfFunding = new DimReferencedatum {
                    Id = 2,
                    CodeScheme = "rahoitusmuoto",
                    CodeValue = "dimFundingDecision2 type of funding code value",
                    NameFi = "dimFundingDecision2 type of funding name fi",
                    NameEn = "dimFundingDecision2 type of funding name en",
                    NameSv = "dimFundingDecision2 type of funding name sv",
                    SourceId = "Source1",
                    SourceDescription = "Source description"
                },
                DimOrganizationIdFunder = data.DimOrganizations[2].Id,
                DimOrganizationIdFunderNavigation = data.DimOrganizations[2],
                SourceId = "Source1"
            };
            ffvDimFundingDecision2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvDimFundingDecision2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvDimFundingDecision2.Show = false;
            ffvDimFundingDecision2.PrimaryValue = false;
            data.FactFieldValues.Add(ffvDimFundingDecision2);


            /*
             * DimProfileOnlyFundingDecisions
             */

            // DimProfileOnlyFundingDecision 1
            FactFieldValue ffvDimProfileOnlyFundingDecision1 = userProfileService.GetEmptyFactFieldValue();
            ffvDimProfileOnlyFundingDecision1.DimUserProfileId = data.UserProfile.Id;
            ffvDimProfileOnlyFundingDecision1.DimUserProfile = data.UserProfile;
            ffvDimProfileOnlyFundingDecision1.DimFieldDisplaySettingsId = dfdsFundingDecision.Id; // FUNDING_DECISION
            ffvDimProfileOnlyFundingDecision1.DimFieldDisplaySettings = dfdsFundingDecision;
            ffvDimProfileOnlyFundingDecision1.DimProfileOnlyFundingDecisionId = 234;
            ffvDimProfileOnlyFundingDecision1.DimProfileOnlyFundingDecision = new DimProfileOnlyFundingDecision {
                Id = 234,
                Acronym = "dimProfileOnlyFundingDecision1 acronym",
                AmountInEur = 234.56m,
                AmountInFundingDecisionCurrency = 112233.44m,
                DimCallProgrammeId = 100,
                DimCallProgramme = new DimCallProgramme {
                    Id = 100,
                    NameFi = "dimProfileOnlyFundingDecision1 call programme",
                    NameEn = "",
                    NameSv = "",
                    SourceId = "Source2"
                },
                DimDateIdEnd = 100,
                DimDateIdEndNavigation = new DimDate {
                    Id = 100,
                    Year = 1996,
                    Month = 12,
                    Day = 31,
                    SourceId = "Source2"
                },
                DimDateIdStart = 101,
                DimDateIdStartNavigation = new DimDate {
                    Id = 101,
                    Year = 1995,
                    Month = 1,
                    Day = 1,
                    SourceId = "Source2"
                },
                DescriptionFi = "dimProfileOnlyFundingDecision1 description",
                DescriptionEn = "",
                DescriptionSv = "",
                FunderProjectNumber = "dimProfileOnlyFundingDecision1 funder project number",
                FundingDecisionCurrencyAbbreviation = "EUR",
                NameFi = "dimProfileOnlyFundingDecision1 name",
                NameEn = "",
                NameSv = "",
                DimOrganizationIdFunder = data.DimOrganizations[1].Id,
                DimOrganizationIdFunderNavigation = data.DimOrganizations[1],
                DimWebLinks = new List<DimWebLink>()
                {
                    new DimWebLink
                    {
                        Id = 1,
                        Url = "https://example.com/profile_only_fundingdecision1",
                        LinkLabel = "dimProfileOnlyFundingDecision1 weblink label",
                        SourceId = "Source2"
                    }
                },
                SourceId = "Source2"
            };
            ffvDimProfileOnlyFundingDecision1.DimReferencedataActorRoleId = 100;
            ffvDimProfileOnlyFundingDecision1.DimReferencedataActorRole = new DimReferencedatum {
                Id = 100,
                CodeScheme = "dimProfileOnlyFundingDecision1 dimReferencedataActorRole code scheme",
                CodeValue = "dimProfileOnlyFundingDecision1 dimReferencedataActorRole code value",
                NameFi = "dimProfileOnlyFundingDecision1 dimReferencedataActorRole name fi",
                NameEn = "dimProfileOnlyFundingDecision1 dimReferencedataActorRole name en",
                NameSv = "dimProfileOnlyFundingDecision1 dimReferencedataActorRole name sv",
                SourceId = "Source2",
                SourceDescription = "Source description"
            };
            ffvDimProfileOnlyFundingDecision1.DimRegisteredDataSourceId = data.DimRegisteredDataSources[0].Id;
            ffvDimProfileOnlyFundingDecision1.DimRegisteredDataSource = data.DimRegisteredDataSources[0];
            ffvDimProfileOnlyFundingDecision1.Show = true;
            ffvDimProfileOnlyFundingDecision1.PrimaryValue = true;
            ffvDimProfileOnlyFundingDecision1.DimIdentifierlessDataId = -1;
            ffvDimProfileOnlyFundingDecision1.DimIdentifierlessData = new DimIdentifierlessDatum {
                Id = -1,
                Type = "",
                ValueFi = "",
                ValueEn = "",
                ValueSv = "",
                SourceId = "Source2"
            };
            data.FactFieldValues.Add(ffvDimProfileOnlyFundingDecision1);

            // DimProfileOnlyFundingDecision 2
            FactFieldValue ffvDimProfileOnlyFundingDecision2 = userProfileService.GetEmptyFactFieldValue();
            ffvDimProfileOnlyFundingDecision2.DimUserProfileId = data.UserProfile.Id;
            ffvDimProfileOnlyFundingDecision2.DimUserProfile = data.UserProfile;
            ffvDimProfileOnlyFundingDecision2.DimFieldDisplaySettingsId = dfdsFundingDecision.Id; // FUNDING_DECISION
            ffvDimProfileOnlyFundingDecision2.DimFieldDisplaySettings = dfdsFundingDecision;
            ffvDimProfileOnlyFundingDecision2.DimProfileOnlyFundingDecisionId = 235;
            ffvDimProfileOnlyFundingDecision2.DimProfileOnlyFundingDecision = new DimProfileOnlyFundingDecision {
                Id = 235,
                Acronym = "dimProfileOnlyFundingDecision2 acronym",
                AmountInEur = 345.67m,
                AmountInFundingDecisionCurrency = 223344.55m,
                DimCallProgrammeId = 101,
                DimCallProgramme = new DimCallProgramme {
                    Id = 101,
                    NameFi = "dimProfileOnlyFundingDecision2 call programme name fi",
                    NameEn = "dimProfileOnlyFundingDecision2 call programme name en",
                    NameSv = "dimProfileOnlyFundingDecision2 call programme name sv",
                    SourceId = "Source2"
                },
                DimDateIdEnd = 102,
                DimDateIdEndNavigation = new DimDate {
                    Id = 102,
                    Year = 2002,
                    Month = 12,
                    Day = 31,
                    SourceId = "Source2"
                },
                DimDateIdStart = 103,
                DimDateIdStartNavigation = new DimDate {
                    Id = 103,
                    Year = 2001,
                    Month = 1,
                    Day = 1,
                    SourceId = "Source2"
                },
                DescriptionFi = "dimProfileOnlyFundingDecision2 description fi",
                DescriptionEn = "dimProfileOnlyFundingDecision2 description en",
                DescriptionSv = "dimProfileOnlyFundingDecision2 description sv",
                FunderProjectNumber = "dimProfileOnlyFundingDecision2 funder project number",
                FundingDecisionCurrencyAbbreviation = "USD",
                NameFi = "dimProfileOnlyFundingDecision2 name fi",
                NameEn = "dimProfileOnlyFundingDecision2 name en",
                NameSv = "dimProfileOnlyFundingDecision2 name sv",
                DimOrganizationIdFunder = -1,
                DimOrganizationIdFunderNavigation = null,
                SourceId = "Source2",
                DimWebLinks = new List<DimWebLink>()
                {
                    new DimWebLink
                    {
                        Id = 2,
                        Url = "https://example.com/profile_only_fundingdecision2",
                        LinkLabel = "dimProfileOnlyFundingDecision2 weblink label",
                        SourceId = "Source2"
                    }
                },
            };
            ffvDimProfileOnlyFundingDecision2.DimReferencedataActorRoleId = 101;
            ffvDimProfileOnlyFundingDecision2.DimReferencedataActorRole = new DimReferencedatum {
                Id = 101,
                CodeScheme = "dimProfileOnlyFundingDecision2 dimReferencedataActorRole code scheme",
                CodeValue = "dimProfileOnlyFundingDecision2 dimReferencedataActorRole code value",
                NameFi = "dimProfileOnlyFundingDecision2 dimReferencedataActorRole name",
                NameEn = "",
                NameSv = "",
                SourceId = "Source2",
                SourceDescription = "Source description"
            };
            ffvDimProfileOnlyFundingDecision2.DimRegisteredDataSourceId = data.DimRegisteredDataSources[1].Id;
            ffvDimProfileOnlyFundingDecision2.DimRegisteredDataSource = data.DimRegisteredDataSources[1];
            ffvDimProfileOnlyFundingDecision2.Show = false;
            ffvDimProfileOnlyFundingDecision2.PrimaryValue = false;
            ffvDimProfileOnlyFundingDecision2.DimIdentifierlessDataId = 200;
            ffvDimProfileOnlyFundingDecision2.DimIdentifierlessData = new DimIdentifierlessDatum {
                Id = 200,
                Type = Constants.IdentifierlessDataTypes.ORGANIZATION_NAME,
                ValueFi = "dimProfileOnlyFundingDecision2 DimIdentifierlessData value fi",
                ValueEn = "dimProfileOnlyFundingDecision2 DimIdentifierlessData value en",
                ValueSv = "dimProfileOnlyFundingDecision2 DimIdentifierlessData value sv",
                SourceId = "Source2"
            };
            data.FactFieldValues.Add(ffvDimProfileOnlyFundingDecision2);

            return data;
        }


        public async Task SeedAsync(TtvContext context)
        {
            context.FactFieldValues.AddRange(FactFieldValues);
            await context.SaveChangesAsync();
        }
    }
}