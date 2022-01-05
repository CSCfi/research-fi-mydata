using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace api.Services
{
    /*
     * DemoDataService adds demo data to each profile.
     * This service is used in summer 2021 demo and must be disabled after that.
     */
    public class DemoDataService
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;
        private readonly UtilityService _utilityService;
        private readonly ILogger<DemoDataService> _logger;
        private readonly string DemoOrganization1Name = "Yliopisto A";
        private readonly string DemoOrganization2Name = "Tutkimuslaitos X";
        private readonly string DemoOrganization3Name = Constants.SourceIdentifiers.TIEDEJATUTKIMUS;
        private readonly string DemoOrganizationFunder1Name = "Uusi akatemia";
        private readonly string DemoOrganizationFunder2Name = "Laitteistokehityssäätiö";
        private readonly string DemoOrganization1DataSourceName = "Testidata";
        private readonly string DemoOrganization2DataSourceName = "Testidata";
        private readonly string DemoOrganization3DataSourceName = Constants.SourceIdentifiers.TIEDEJATUTKIMUS;
        private readonly string DemoOrganization1FieldOfScience1 = "Fysiikka";
        private readonly string DemoOrganization1FieldOfScience2 = "Historia";
        private readonly string DemoOrganization2FieldOfScience1 = "Yleislääketiede";
        private readonly string DemoOrganization2FieldOfScience2 = "Sisätaudit ja muut kliiniset lääketieteet";
        private readonly string DemoCallProgrammeName1 = "Akatemiatutkija 6789";
        private readonly string DemoCallProgrammeName2 = "Vuoden 2019 haku";
        private readonly string DemoTypeOfFundingName1 = "Akatemiatutkijan tehtävä";
        private readonly string DemoTypeOfFundingName2 = "laitekehitys";
        private readonly string DemoFunderProjectNumber1 = "098765";
        private readonly string DemoFunderProjectNumber2 = "123456-A";
        private readonly string DemoResearchDatasetLocalIdentifier1 = "researchfi_demo_dataset_1";
        private readonly string DemoResearchDatasetLocalIdentifier2 = "researchfi_demo_dataset_2";
        private readonly string DemoReferenceDataAgentRoleCreator = "2";
        private readonly string DemoReferenceDataAgentRolePublisher = "4";
        private readonly string DemoReferenceDataAgentRoleRightsHolder = "5";

        public DemoDataService(TtvContext ttvContext, UserProfileService userProfileService, UtilityService utilityService, ILogger<DemoDataService> logger)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _utilityService = utilityService;
            _logger = logger;
        }

        public string getSourceDescription(string orcidId)
        {
            return Constants.SourceDescriptions.PROFILE_API + " " + orcidId;
        }

        public string GetDemoOrganization1Name()
        {
            return this.DemoOrganization1Name;
        }

        public string GetDemoOrganization2Name()
        {
            return this.DemoOrganization2Name;
        }

        public string GetDemoOrganization3Name()
        {
            return this.DemoOrganization3Name;
        }

        public string GetDemoOrganizationFunder1Name()
        {
            return this.DemoOrganizationFunder1Name;
        }

        public string GetDemoOrganizationFunder2Name()
        {
            return this.DemoOrganizationFunder2Name;
        }

        public DimOrganization GetOrganization1()
        {
            return _ttvContext.DimOrganizations.FirstOrDefault(org => org.SourceId == Constants.SourceIdentifiers.DEMO_COMMON && org.NameFi == this.DemoOrganization1Name);
        }

        public DimOrganization GetOrganization2()
        {
            return _ttvContext.DimOrganizations.FirstOrDefault(org => org.SourceId == Constants.SourceIdentifiers.DEMO_COMMON && org.NameFi == this.DemoOrganization2Name);
        }

        public DimOrganization GetOrganization3()
        {
            return _ttvContext.DimOrganizations.FirstOrDefault(org => org.SourceId == Constants.SourceIdentifiers.DEMO_COMMON && org.NameFi == this.DemoOrganization3Name);
        }

        public DimOrganization GetOrganizationFunder1()
        {
            return _ttvContext.DimOrganizations.FirstOrDefault(org => org.SourceId == Constants.SourceIdentifiers.DEMO_COMMON && org.NameFi == this.DemoOrganizationFunder1Name);
        }

        public DimOrganization GetOrganizationFunder2()
        {
            return _ttvContext.DimOrganizations.FirstOrDefault(org => org.SourceId == Constants.SourceIdentifiers.DEMO_COMMON && org.NameFi == this.DemoOrganizationFunder2Name);
        }

        public DimRegisteredDataSource GetOrganization1RegisteredDataSource()
        {
            var organization1 = this.GetOrganization1();
            return _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefault(drds => drds.DimOrganization == organization1 && drds.Name == this.DemoOrganization1DataSourceName && drds.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
        }

        public DimRegisteredDataSource GetOrganization2RegisteredDataSource()
        {
            var organization2 = this.GetOrganization2();
            return _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefault(drds => drds.DimOrganization == organization2 && drds.Name == this.DemoOrganization2DataSourceName && drds.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
        }

        public DimRegisteredDataSource GetOrganization3RegisteredDataSource()
        {
            var organization3 = this.GetOrganization3();
            return _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefault(drds => drds.DimOrganization == organization3 && drds.Name == this.DemoOrganization3DataSourceName && drds.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
        }

        public void AddOrganizations()
        {
            _logger.LogInformation("DemoDataService: AddOrganizations");
            // Organization 1
            var organization1 = this.GetOrganization1();
            if (organization1 == null)
            {
                organization1 = new DimOrganization()
                {
                    DimSectorid = -1,
                    NameFi = this.DemoOrganization1Name,
                    SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime(),
                    DimRegisteredDataSourceId = -1
                };
                _ttvContext.DimOrganizations.Add(organization1);
            }

            // Organization 2
            var organization2 = this.GetOrganization2();
            if (organization2 == null)
            {
                organization2 = new DimOrganization()
                {
                    DimSectorid = -1,
                    NameFi = this.DemoOrganization2Name,
                    SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime(),
                    DimRegisteredDataSourceId = -1
                };
                _ttvContext.DimOrganizations.Add(organization2);
            }

            // Organization 3
            var organization3 = this.GetOrganization3();
            if (organization3 == null)
            {
                organization3 = new DimOrganization()
                {
                    DimSectorid = -1,
                    NameFi = this.DemoOrganization3Name,
                    SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime(),
                    DimRegisteredDataSourceId = -1
                };
                _ttvContext.DimOrganizations.Add(organization3);
            }

            // Funder organization 1
            var organizationFunder1 = this.GetOrganizationFunder1();
            if (organizationFunder1 == null)
            {
                organizationFunder1 = new DimOrganization()
                {
                    DimSectorid = -1,
                    NameFi = this.DemoOrganizationFunder1Name,
                    SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime(),
                    DimRegisteredDataSourceId = -1
                };
                _ttvContext.DimOrganizations.Add(organizationFunder1);
            }

            // Funder organization 2
            var organizationFunder2 = this.GetOrganizationFunder2();
            if (organizationFunder2 == null)
            {
                organizationFunder2 = new DimOrganization()
                {
                    DimSectorid = -1,
                    NameFi = this.DemoOrganizationFunder2Name,
                    SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime(),
                    DimRegisteredDataSourceId = -1
                };
                _ttvContext.DimOrganizations.Add(organizationFunder2);
            }

            _ttvContext.SaveChanges();
        }


        public void AddRegisteredDatasources()
        {
            _logger.LogInformation("DemoDataService: AddRegisteredDatasources");

            // Registered data source 1
            var organization1 = this.GetOrganization1();
            var registeredDatasourceOrg1 = this.GetOrganization1RegisteredDataSource();
            if (registeredDatasourceOrg1 == null)
            {
                registeredDatasourceOrg1 = new DimRegisteredDataSource()
                {
                    Name = this.DemoOrganization1DataSourceName,
                    DimOrganization = organization1,
                    SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime()
                };
                _ttvContext.DimRegisteredDataSources.Add(registeredDatasourceOrg1);
            }

            // Registered data source 2
            var organization2 = this.GetOrganization2();
            var registeredDatasourceOrg2 = this.GetOrganization2RegisteredDataSource();
            if (registeredDatasourceOrg2 == null)
            {
                registeredDatasourceOrg2 = new DimRegisteredDataSource()
                {
                    Name = this.DemoOrganization2DataSourceName,
                    DimOrganization = organization2,
                    SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime()
                };
                _ttvContext.DimRegisteredDataSources.Add(registeredDatasourceOrg2);
            }

            // Registered data source 3
            var organization3 = this.GetOrganization3();
            var registeredDatasourceOrg3 = this.GetOrganization3RegisteredDataSource();
            if (registeredDatasourceOrg3 == null)
            {
                registeredDatasourceOrg3 = new DimRegisteredDataSource()
                {
                    Name = this.DemoOrganization3DataSourceName,
                    DimOrganization = organization3,
                    SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime()
                };
                _ttvContext.DimRegisteredDataSources.Add(registeredDatasourceOrg3);
            }

            _ttvContext.SaveChanges();
        }


        public void AddReferenceData()
        {
            _logger.LogInformation("DemoDataService: AddReferenceData");

            var referenceData = _ttvContext.DimReferencedata.FirstOrDefault(dr => dr.SourceId == Constants.SourceIdentifiers.DEMO_COMMON && dr.NameFi == "Työsuhde");
            if (referenceData == null)
            {
                referenceData = new DimReferencedatum()
                {
                    CodeScheme = "",
                    CodeValue = "",
                    NameFi = "Työsuhde",
                    SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime()
                };
                _ttvContext.DimReferencedata.Add(referenceData);
            }



            // Reference data actor roles
            // Creator
            var referencedataActorRoleCreator = _ttvContext.DimReferencedata.FirstOrDefault(dr => dr.NameEn == "Creator" && dr.CodeScheme == "agentrole" && dr.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
            if (referencedataActorRoleCreator == null)
            {
                _ttvContext.DimReferencedata.Add(
                    new DimReferencedatum()
                    {
                        CodeScheme = "agentrole",
                        CodeValue = this.DemoReferenceDataAgentRoleCreator,
                        NameFi = "Tekijä",
                        NameSv = "Upphovsperson",
                        NameEn = "Creator",
                        SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = _utilityService.getCurrentDateTime(),
                        Modified = _utilityService.getCurrentDateTime()
                    }
                );
            }
            // Publisher
            var referencedataActorRolePublisher = _ttvContext.DimReferencedata.FirstOrDefault(dr => dr.NameEn == "Publisher" && dr.CodeScheme == "agentrole" && dr.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
            if (referencedataActorRolePublisher == null)
            {
                _ttvContext.DimReferencedata.Add(
                    new DimReferencedatum()
                    {
                        CodeScheme = "agentrole",
                        CodeValue = this.DemoReferenceDataAgentRolePublisher,
                        NameFi = "Julkaisija",
                        NameSv = "Utgivare",
                        NameEn = "Publisher",
                        SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = _utilityService.getCurrentDateTime(),
                        Modified = _utilityService.getCurrentDateTime()
                    }
                );
            }
            // Rights holder
            var referencedataActorRoleRightsholder = _ttvContext.DimReferencedata.FirstOrDefault(dr => dr.NameEn == "Rights holder" && dr.CodeScheme == "agentrole" && dr.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
            if (referencedataActorRoleRightsholder == null)
            {
                _ttvContext.DimReferencedata.Add(
                    new DimReferencedatum()
                    {
                        CodeScheme = "agentrole",
                        CodeValue = this.DemoReferenceDataAgentRoleRightsHolder,
                        NameFi = "Oikeuksienhaltija",
                        NameSv = "Rättighetsinnehavare",
                        NameEn = "Rights holder",
                        SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = _utilityService.getCurrentDateTime(),
                        Modified = _utilityService.getCurrentDateTime()
                    }
                );
            }



            // User choices
            var choice1NameFi = "Olen kiinnostunut tiedotusvälineiden yhteydenotoista";
            var choice2NameFi = "Olen kiinnostunut yhteistyöstä muiden tutkijoiden ja tutkimusryhmien kanssa";
            var choice3NameFi = "Olen kiinnostunut yhteistyöstä yritysten kanssa";
            var choice4NameFi = "Olen kiinnostunut toimimaan tieteellisten julkaisujen vertaisarvioijana";

            var referenceData_choice1 = _ttvContext.DimReferencedata.FirstOrDefault(dr => dr.SourceId == Constants.SourceIdentifiers.DEMO_COMMON && dr.NameFi == choice1NameFi);
            if (referenceData_choice1 == null)
            {
                referenceData = new DimReferencedatum()
                {
                    CodeScheme = Constants.CodeSchemes.USER_CHOICES,
                    CodeValue = "",
                    NameFi = choice1NameFi,
                    NameEn = "I am interested in media contacts",
                    SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime()
                };
                _ttvContext.DimReferencedata.Add(referenceData);
            }

            var referenceData_choice2 = _ttvContext.DimReferencedata.FirstOrDefault(dr => dr.SourceId == Constants.SourceIdentifiers.DEMO_COMMON && dr.NameFi == choice2NameFi);
            if (referenceData_choice2 == null)
            {
                referenceData = new DimReferencedatum()
                {
                    CodeScheme = Constants.CodeSchemes.USER_CHOICES,
                    CodeValue = "",
                    NameFi = choice2NameFi,
                    NameEn = "I am interested in cooperation with other researchers and research groups",
                    SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime()
                };
                _ttvContext.DimReferencedata.Add(referenceData);
            }

            var referenceData_choice3 = _ttvContext.DimReferencedata.FirstOrDefault(dr => dr.SourceId == Constants.SourceIdentifiers.DEMO_COMMON && dr.NameFi == choice3NameFi);
            if (referenceData_choice3 == null)
            {
                referenceData = new DimReferencedatum()
                {
                    CodeScheme = Constants.CodeSchemes.USER_CHOICES,
                    CodeValue = "",
                    NameFi = choice3NameFi,
                    NameEn = "I am interested in working with companies",
                    SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime()
                };
                _ttvContext.DimReferencedata.Add(referenceData);
            }

            var referenceData_choice4 = _ttvContext.DimReferencedata.FirstOrDefault(dr => dr.SourceId == Constants.SourceIdentifiers.DEMO_COMMON && dr.NameFi == choice4NameFi);
            if (referenceData_choice4 == null)
            {
                referenceData = new DimReferencedatum()
                {
                    CodeScheme = Constants.CodeSchemes.USER_CHOICES,
                    CodeValue = "",
                    NameFi = choice4NameFi,
                    NameEn = "I am interested in being a peer reviewer for scientific publications",
                    SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime()
                };
                _ttvContext.DimReferencedata.Add(referenceData);
            }

            _ttvContext.SaveChanges();
        }

        public void AddFundingDecisions()
        {
            // Funding decision - organization 1
            var fundingDecision1 = _ttvContext.DimFundingDecisions.FirstOrDefault(fd => fd.FunderProjectNumber == this.DemoFunderProjectNumber1);
            if (fundingDecision1 == null)
            {
                var organizationFunder1 = this.GetOrganizationFunder1();

                // Call programme
                var dimDate_callProgramme = _ttvContext.DimDates.FirstOrDefault(dd => dd.Year == 2021 && dd.Month == 1 && dd.Day == 1);
                if (dimDate_callProgramme == null)
                {
                    dimDate_callProgramme = new DimDate()
                    {
                        Year = 2021,
                        Month = 1,
                        Day = 1,
                        SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = _utilityService.getCurrentDateTime(),
                        Modified = _utilityService.getCurrentDateTime(),
                    };
                    _ttvContext.DimDates.Add(dimDate_callProgramme);
                }

                var dimCallProgramme1 = _ttvContext.DimCallProgrammes.FirstOrDefault(dcp => dcp.SourceId == Constants.SourceIdentifiers.DEMO_COMMON && dcp.NameFi == this.DemoCallProgrammeName1);
                if (dimCallProgramme1 == null)
                {
                    dimCallProgramme1 = new DimCallProgramme()
                    {
                        NameFi = this.DemoCallProgrammeName1,
                        DimDateIdOpenNavigation = dimDate_callProgramme,
                        DimDateIdDueNavigation = dimDate_callProgramme,
                        SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = _utilityService.getCurrentDateTime(),
                        Modified = _utilityService.getCurrentDateTime(),
                        DimRegisteredDataSourceId = -1
                    };
                    _ttvContext.DimCallProgrammes.Add(dimCallProgramme1);
                }

                // Type of funding
                var dimTypeOfFunding1 = _ttvContext.DimTypeOfFundings.FirstOrDefault(dtof => dtof.SourceId == Constants.SourceIdentifiers.DEMO_COMMON && dtof.NameFi == this.DemoTypeOfFundingName1);
                if (dimTypeOfFunding1 == null)
                {
                    dimTypeOfFunding1 = new DimTypeOfFunding()
                    {
                        TypeId = "A2",
                        NameFi = this.DemoTypeOfFundingName1,
                        DimTypeOfFundingId = -1,
                        SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = _utilityService.getCurrentDateTime(),
                        Modified = _utilityService.getCurrentDateTime()
                    };
                    _ttvContext.DimTypeOfFundings.Add(dimTypeOfFunding1);
                }

                // Start date
                var dimStartDate_fundingDecision_organization1 = _ttvContext.DimDates.FirstOrDefault(dd => dd.Year == 2022 && dd.Month == 0 && dd.Day == 0);
                if (dimStartDate_fundingDecision_organization1 == null)
                {
                    dimStartDate_fundingDecision_organization1 = new DimDate()
                    {
                        Year = 2022,
                        Month = 0,
                        Day = 0,
                        SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = _utilityService.getCurrentDateTime(),
                        Modified = _utilityService.getCurrentDateTime()
                    };
                    _ttvContext.DimDates.Add(dimStartDate_fundingDecision_organization1);
                }

                // End date
                var dimEndDate_fundingDecision_organization1 = _ttvContext.DimDates.FirstOrDefault(dd => dd.Year == 0 && dd.Month == 0 && dd.Day == 0);
                if (dimEndDate_fundingDecision_organization1 == null)
                {
                    dimEndDate_fundingDecision_organization1 = new DimDate()
                    {
                        Year = 0,
                        Month = 0,
                        Day = 0,
                        SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = _utilityService.getCurrentDateTime(),
                        Modified = _utilityService.getCurrentDateTime()
                    };
                    _ttvContext.DimDates.Add(dimEndDate_fundingDecision_organization1);
                }

                // Funding decision
                var dimFundingDecision_Organization1 = new DimFundingDecision()
                {
                    FunderProjectNumber = this.DemoFunderProjectNumber1,
                    DimDateIdApproval = -1,
                    DimDateIdStartNavigation = dimStartDate_fundingDecision_organization1,
                    DimDateIdEndNavigation = dimEndDate_fundingDecision_organization1,
                    DimNameIdContactPerson = -1,
                    DimCallProgramme = dimCallProgramme1,
                    DimGeoId = -1,
                    DimTypeOfFunding = dimTypeOfFunding1,
                    DimOrganizationIdFunderNavigation = this.GetOrganizationFunder1(),
                    DimFundingDecisionIdParentDecision = -1,
                    Acronym = "SSTFaM",
                    NameFi = "Kestävyysnäkökulmat perheiden harrastusvalinnoissa",
                    DescriptionFi = "Kestävyyskysymykset ovat nousseet vahvasti julkiseen keskusteluun kuluneen vuosikymmenen aikana. Hankkeessa selvitetään julkisen keskustelun vaikutusta perheiden harrastusvalintoihin hyödyntäen parhaita saatavilla olevia aineistoja.",
                    AmountInEur = 387400,
                    SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime(),
                    DimRegisteredDataSourceId = this.GetOrganization1RegisteredDataSource().Id
                };
                _ttvContext.DimFundingDecisions.Add(dimFundingDecision_Organization1);
            }


            // Funding decision - organization 2
            var fundingDecision2 = _ttvContext.DimFundingDecisions.FirstOrDefault(fd => fd.FunderProjectNumber == this.DemoFunderProjectNumber2);
            if (fundingDecision2 == null)
            {
                var organizationFunder2 = this.GetOrganizationFunder2();

                // Call programme
                var dimDate_callProgramme = _ttvContext.DimDates.FirstOrDefault(dd => dd.Year == 2021 && dd.Month == 1 && dd.Day == 1);
                if (dimDate_callProgramme == null)
                {
                    dimDate_callProgramme = new DimDate()
                    {
                        Year = 2021,
                        Month = 1,
                        Day = 1,
                        SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = _utilityService.getCurrentDateTime(),
                        Modified = _utilityService.getCurrentDateTime()
                    };
                    _ttvContext.DimDates.Add(dimDate_callProgramme);
                }

                var dimCallProgramme2 = _ttvContext.DimCallProgrammes.FirstOrDefault(dcp => dcp.SourceId == Constants.SourceIdentifiers.DEMO_COMMON && dcp.NameFi == this.DemoCallProgrammeName2);
                if (dimCallProgramme2 == null)
                {
                    dimCallProgramme2 = new DimCallProgramme()
                    {
                        NameFi = this.DemoCallProgrammeName2,
                        DimDateIdOpenNavigation = dimDate_callProgramme,
                        DimDateIdDueNavigation = dimDate_callProgramme,
                        SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = _utilityService.getCurrentDateTime(),
                        Modified = _utilityService.getCurrentDateTime(),
                        DimRegisteredDataSourceId = -1
                    };
                    _ttvContext.DimCallProgrammes.Add(dimCallProgramme2);
                }

                // Type of funding
                var dimTypeOfFunding2 = _ttvContext.DimTypeOfFundings.FirstOrDefault(dtof => dtof.SourceId == Constants.SourceIdentifiers.DEMO_COMMON && dtof.NameFi == this.DemoTypeOfFundingName2);
                if (dimTypeOfFunding2 == null)
                {
                    dimTypeOfFunding2 = new DimTypeOfFunding()
                    {
                        TypeId = "A3",
                        NameFi = this.DemoTypeOfFundingName2,
                        DimTypeOfFundingId = -1,
                        SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = _utilityService.getCurrentDateTime(),
                        Modified = _utilityService.getCurrentDateTime()
                    };
                    _ttvContext.DimTypeOfFundings.Add(dimTypeOfFunding2);
                }

                // Start date
                var dimStartDate_fundingDecision_organization2 = _ttvContext.DimDates.FirstOrDefault(dd => dd.Year == 2019 && dd.Month == 0 && dd.Day == 0);
                if (dimStartDate_fundingDecision_organization2 == null)
                {
                    dimStartDate_fundingDecision_organization2 = new DimDate()
                    {
                        Year = 2019,
                        Month = 0,
                        Day = 0,
                        SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = _utilityService.getCurrentDateTime(),
                        Modified = _utilityService.getCurrentDateTime()
                    };
                    _ttvContext.DimDates.Add(dimStartDate_fundingDecision_organization2);
                }

                // End date
                var dimEndDate_fundingDecision_organization2 = _ttvContext.DimDates.FirstOrDefault(dd => dd.Year == 0 && dd.Month == 0 && dd.Day == 0);
                if (dimEndDate_fundingDecision_organization2 == null)
                {
                    dimEndDate_fundingDecision_organization2 = new DimDate()
                    {
                        Year = 0,
                        Month = 0,
                        Day = 0,
                        SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = _utilityService.getCurrentDateTime(),
                        Modified = _utilityService.getCurrentDateTime()
                    };
                    _ttvContext.DimDates.Add(dimEndDate_fundingDecision_organization2);
                }

                // Registered data source
                var registeredDatasourceOrg2 = this.GetOrganization2RegisteredDataSource();

                // Funding decision
                var dimFundingDecision_Organization2 = new DimFundingDecision()
                {
                    FunderProjectNumber = this.DemoFunderProjectNumber2,
                    DimDateIdApproval = -1,
                    DimDateIdStartNavigation = dimStartDate_fundingDecision_organization2,
                    DimDateIdEndNavigation = dimEndDate_fundingDecision_organization2,
                    DimNameIdContactPerson = -1,
                    DimCallProgramme = dimCallProgramme2,
                    DimGeoId = -1,
                    DimTypeOfFunding = dimTypeOfFunding2,
                    DimOrganizationIdFunderNavigation = this.GetOrganizationFunder2(),
                    DimFundingDecisionIdParentDecision = -1,
                    Acronym = "VIMPELI1",
                    NameFi = "Vimpelikiihdyttimen hyödyntäminen pienhiukkasten värimäärittelyssä",
                    DescriptionFi = "Pienhiukkasten (<10 nm) värillä on havaittu olevan merkitystä mielialaan. Värimääritys on nykyään aikaavievää ja edellyttää optisen mikroskoopin manuaalista käyttöä. Laboratorion vanhan vimpelikiihdyttimen on havaittu olevan pienin muutoksin sopiva pienhiukkasten automaattiseen värikarakterisointiin. Apurahalla tehdään kiihdytimelle tarvittavat muutokset.",
                    AmountInEur = 27000,
                    SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime(),
                    DimRegisteredDataSourceId = this.GetOrganization2RegisteredDataSource().Id
                };
                _ttvContext.DimFundingDecisions.Add(dimFundingDecision_Organization2);
            }

            _ttvContext.SaveChanges();
        }


        //public void AddResearchCommunities()
        //{
        //    var registeredDatasourceOrg1 = this.GetOrganization1RegisteredDataSource();
        //    var registeredDatasourceOrg2 = this.GetOrganization2RegisteredDataSource();

        //    _ttvContext.DimResearchCommunities.Add(
        //        new DimResearchCommunity()
        //        {
        //            NameFi = "Tutkimuskeskus A",
        //            SourceId = Constants.SourceIdentifiers.DEMO,
        //            SourceDescription = Constants.SourceDescriptions.PROFILE_API,
        //            Created = _utilityService.getCurrentDateTime(),
        //            Modified = _utilityService.getCurrentDateTime(),
        //            DimRegisteredDataSource = registeredDatasourceOrg1
        //        }
        //    );
        //    _ttvContext.DimResearchCommunities.Add(
        //        new DimResearchCommunity()
        //        {
        //            NameFi = "Tutkimuslaitos X",
        //            SourceId = Constants.SourceIdentifiers.DEMO,
        //            SourceDescription = Constants.SourceDescriptions.PROFILE_API,
        //            Created = _utilityService.getCurrentDateTime(),
        //            Modified = _utilityService.getCurrentDateTime(),
        //            DimRegisteredDataSource = registeredDatasourceOrg2
        //        }
        //    );
        //    _ttvContext.SaveChanges();
        //}


        public void AddFieldsOfScience()
        {
            _logger.LogInformation("DemoDataService: AddFieldsOfScience");

            var fieldsOfScienceNames = new List<string> {
                this.DemoOrganization1FieldOfScience1,
                this.DemoOrganization1FieldOfScience2,
                this.DemoOrganization2FieldOfScience1,
                this.DemoOrganization2FieldOfScience2
            };
            foreach (string fieldOfScienceName in fieldsOfScienceNames)
            {
                var dimFieldOfScience = _ttvContext.DimFieldOfSciences.FirstOrDefault(dfos => dfos.NameFi == fieldOfScienceName && dfos.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
            
                if (dimFieldOfScience == null)
                {
                    dimFieldOfScience = new DimFieldOfScience()
                    {
                        FieldId = " ",
                        NameFi = fieldOfScienceName,
                        SourceId = Constants.SourceIdentifiers.DEMO_COMMON,
                        SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                        Created = _utilityService.getCurrentDateTime(),
                        Modified = _utilityService.getCurrentDateTime()
                    };
                    _ttvContext.DimFieldOfSciences.Add(dimFieldOfScience);
                }
            }
            _ttvContext.SaveChanges();
        }


        public void InitDemo()
        {
            this.AddOrganizations();
            this.AddRegisteredDatasources();
            this.AddReferenceData();
            //this.AddResearchCommunities();
            this.AddFieldsOfScience();
            this.AddFundingDecisions();
        }


        public async Task<DimOrganization> GetOrganization1Async()
        {
            return await _ttvContext.DimOrganizations.AsNoTracking().FirstOrDefaultAsync(org => org.NameFi == this.DemoOrganization1Name && org.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
        }

        public async Task<DimOrganization> GetOrganization2Async()
        {
            return await _ttvContext.DimOrganizations.AsNoTracking().FirstOrDefaultAsync(org => org.NameFi == this.DemoOrganization2Name && org.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
        }

        public async Task<DimOrganization> GetOrganization3Async()
        {
            return await _ttvContext.DimOrganizations.AsNoTracking().FirstOrDefaultAsync(org => org.NameFi == this.DemoOrganization3Name && org.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
        }

        public async Task<DimOrganization> GetOrganizationFunder1Async()
        {
            // Do not use AsNoTracking here.
            return await _ttvContext.DimOrganizations.FirstOrDefaultAsync(org => org.NameFi == this.DemoOrganizationFunder1Name && org.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
        }

        public async Task<DimOrganization> GetOrganizationFunder2Async()
        {
            // Do not use AsNoTracking here.
            return await _ttvContext.DimOrganizations.FirstOrDefaultAsync(org => org.NameFi == this.DemoOrganizationFunder2Name && org.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
        }

        public async Task<DimRegisteredDataSource> GetOrganization1RegisteredDataSourceAsync()
        {
            var organization1 = await this.GetOrganization1Async();
            return await _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefaultAsync(drds => drds.DimOrganization == organization1 && drds.Name == this.DemoOrganization1DataSourceName && drds.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
        }

        public async Task<DimRegisteredDataSource> GetOrganization2RegisteredDataSourceAsync()
        {
            var organization2 = await this.GetOrganization2Async();
            return await _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefaultAsync(drds => drds.DimOrganization == organization2 && drds.Name == this.DemoOrganization2DataSourceName && drds.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
        }

        public async Task<DimRegisteredDataSource> GetOrganization3RegisteredDataSourceAsync()
        {
            var organization3 = await this.GetOrganization3Async();
            return await _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefaultAsync(drds => drds.DimOrganization == organization3 && drds.Name == this.DemoOrganization3DataSourceName && drds.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
        }

        public async Task AddDemoDataToUserProfile(string orcidId, DimUserProfile dimUserProfile)
        {
            _logger.LogInformation("DemoDataService: AddDemoDataToUserProfile: " + orcidId);

            var organization1 = await this.GetOrganization1Async();
            var organization2 = await this.GetOrganization2Async();
            var organization1RegisteredDataSource = await this.GetOrganization1RegisteredDataSourceAsync();
            var organization2RegisteredDataSource = await this.GetOrganization2RegisteredDataSourceAsync();
            var currentDateTime = _utilityService.getCurrentDateTime();
            var sourceDescription = this.getSourceDescription(orcidId);

            // Name
            var dimFieldDisplaySettings_name = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_NAME);
            var dimNameOrganization1 = new DimName()
            {
                FirstNames = "Tuisku",
                LastName = "Tutkija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime,
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id
            };
            _ttvContext.DimNames.Add(dimNameOrganization1);

            var dimNameOrganization2 = new DimName()
            {
                FirstNames = "Ami",
                LastName = "Asiantuntija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime,
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id
            };
            _ttvContext.DimNames.Add(dimNameOrganization2);

            var factFieldValue_name_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_name_Organization1.DimUserProfile = dimUserProfile;
            factFieldValue_name_Organization1.DimFieldDisplaySettings = dimFieldDisplaySettings_name;
            factFieldValue_name_Organization1.DimName = dimNameOrganization1;
            factFieldValue_name_Organization1.DimRegisteredDataSourceId = organization1RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_name_Organization1);
            var factFieldValue_name_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_name_Organization2.DimUserProfile = dimUserProfile;
            factFieldValue_name_Organization2.DimFieldDisplaySettings = dimFieldDisplaySettings_name;
            factFieldValue_name_Organization2.DimName = dimNameOrganization2;
            factFieldValue_name_Organization2.DimRegisteredDataSourceId = organization2RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_name_Organization2);


            // Other names
            var dimFieldDisplaySettings_othername = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_OTHER_NAMES);
            var dimOtherNameOrganization1_1 = new DimName()
            {
                FullName = "T. Tutkija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime,
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id
            };
            _ttvContext.DimNames.Add(dimOtherNameOrganization1_1);
            var dimOtherNameOrganization1_2 = new DimName()
            {
                FullName = "T.A. Tutkija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime,
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id
            };
            _ttvContext.DimNames.Add(dimOtherNameOrganization1_2);
            var dimOtherNameOrganization2 = new DimName()
            {
                FullName = "Tuisku Tutkija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime,
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id
            };
            _ttvContext.DimNames.Add(dimOtherNameOrganization2);
            var factFieldValue_othername_Organization1_1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_othername_Organization1_1.DimUserProfile = dimUserProfile;
            factFieldValue_othername_Organization1_1.DimFieldDisplaySettings = dimFieldDisplaySettings_othername;
            factFieldValue_othername_Organization1_1.DimName = dimOtherNameOrganization1_1;
            factFieldValue_othername_Organization1_1.DimRegisteredDataSourceId = organization1RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_othername_Organization1_1);
            var factFieldValue_othername_Organization1_2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_othername_Organization1_2.DimUserProfile = dimUserProfile;
            factFieldValue_othername_Organization1_2.DimFieldDisplaySettings = dimFieldDisplaySettings_othername;
            factFieldValue_othername_Organization1_2.DimName = dimOtherNameOrganization1_2;
            factFieldValue_othername_Organization1_2.DimRegisteredDataSourceId = organization1RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_othername_Organization1_2);
            var factFieldValue_othername_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_othername_Organization2.DimUserProfile = dimUserProfile;
            factFieldValue_othername_Organization2.DimFieldDisplaySettings = dimFieldDisplaySettings_othername;
            factFieldValue_othername_Organization2.DimName = dimOtherNameOrganization2;
            factFieldValue_othername_Organization2.DimRegisteredDataSourceId = organization2RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_othername_Organization2);


            // External identifiers (DimPid)
            var dimFieldDisplaySettings_externalIdentifier = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER);
            var dimPid_Organization1 = _userProfileService.GetEmptyDimPid();
            dimPid_Organization1.PidContent = "O-2000-1000";
            dimPid_Organization1.PidType = "ResearcherID";
            dimPid_Organization1.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
            dimPid_Organization1.SourceId = Constants.SourceIdentifiers.DEMO;
            _ttvContext.DimPids.Add(dimPid_Organization1);
            var dimPid_Organization2 = _userProfileService.GetEmptyDimPid();
            dimPid_Organization2.PidContent = "https://isni.org/isni/0000000100020003";
            dimPid_Organization2.PidType = "ISNI";
            dimPid_Organization2.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
            dimPid_Organization2.SourceId = Constants.SourceIdentifiers.DEMO;
            dimPid_Organization2.SourceDescription = sourceDescription;
            dimPid_Organization2.Created = currentDateTime;
            _ttvContext.DimPids.Add(dimPid_Organization2);
            var factFieldValue_externalIdentifier_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_externalIdentifier_Organization1.DimUserProfile = dimUserProfile;
            factFieldValue_externalIdentifier_Organization1.DimFieldDisplaySettings = dimFieldDisplaySettings_externalIdentifier;
            factFieldValue_externalIdentifier_Organization1.DimPid = dimPid_Organization1;
            factFieldValue_externalIdentifier_Organization1.DimRegisteredDataSourceId = organization1RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_externalIdentifier_Organization1);
            var factFieldValue_externalIdentifier_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_externalIdentifier_Organization2.DimUserProfile = dimUserProfile;
            factFieldValue_externalIdentifier_Organization2.DimFieldDisplaySettings = dimFieldDisplaySettings_externalIdentifier;
            factFieldValue_externalIdentifier_Organization2.DimPid = dimPid_Organization2;
            factFieldValue_externalIdentifier_Organization2.DimRegisteredDataSourceId = organization2RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_externalIdentifier_Organization2);


            // Researcher description
            var dimFieldDisplaySettings_researcherDescription = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION);
            var dimResearcherDescription_Organization1 = new DimResearcherDescription()
            {
                ResearchDescriptionFi = "Tutkimuksen kuvausta suomeksi. Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                ResearchDescriptionEn = "Description of my research in English. Duis ullamcorper sem in sapien pretium bibendum. Vestibulum ex dui, volutpat commodo condimentum sed, lobortis at justo.",
                ResearchDescriptionSv = "Beskrivning av forskningen på svenska. Fusce in lorem tempor, feugiat nunc ut, consectetur erat. Integer purus sem, hendrerit at bibendum vel, laoreet nec tellus.",
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime
            };
            _ttvContext.DimResearcherDescriptions.Add(dimResearcherDescription_Organization1);
            var dimResearcherDescription_Organization2 = new DimResearcherDescription()
            {
                ResearchDescriptionFi = "Tutkimuksen kuvausta suomeksi. Duis finibus velit rutrum euismod scelerisque. Praesent sit amet fermentum ex. Donec vitae tellus eu nisl dignissim laoreet.",
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime
            };
            _ttvContext.DimResearcherDescriptions.Add(dimResearcherDescription_Organization2);
            var factFieldValue_researcherDescription_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_researcherDescription_Organization1.DimUserProfile = dimUserProfile;
            factFieldValue_researcherDescription_Organization1.DimFieldDisplaySettings = dimFieldDisplaySettings_researcherDescription;
            factFieldValue_researcherDescription_Organization1.DimResearcherDescription = dimResearcherDescription_Organization1;
            factFieldValue_researcherDescription_Organization1.DimRegisteredDataSourceId = organization1RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_researcherDescription_Organization1);
            var factFieldValue_researcherDescription_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_researcherDescription_Organization2.DimUserProfile = dimUserProfile;
            factFieldValue_researcherDescription_Organization2.DimFieldDisplaySettings = dimFieldDisplaySettings_researcherDescription;
            factFieldValue_researcherDescription_Organization2.DimResearcherDescription = dimResearcherDescription_Organization2;
            factFieldValue_researcherDescription_Organization2.DimRegisteredDataSourceId = organization2RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_researcherDescription_Organization2);


            // Keywords
            var dimFieldDisplaySettings_keyword = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_KEYWORD);
            var dimKeyword1_Organization1 = new DimKeyword()
            {
                Keyword = "digitalisaatio",
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                DimReferencedataIdLanguageCode = -1,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime
            };
            var dimKeyword2_Organization1 = new DimKeyword()
            {
                Keyword = "aerosolit",
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                DimReferencedataIdLanguageCode = -1,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime
            };
            var dimKeyword3_Organization1 = new DimKeyword()
            {
                Keyword = "sisätaudit",
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                DimReferencedataIdLanguageCode = -1,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime
            };
            var dimKeyword4_Organization1 = new DimKeyword()
            {
                Keyword = "Suomen historia",
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                DimReferencedataIdLanguageCode = -1,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime
            };
            _ttvContext.DimKeywords.Add(dimKeyword1_Organization1);
            _ttvContext.DimKeywords.Add(dimKeyword2_Organization1);
            _ttvContext.DimKeywords.Add(dimKeyword3_Organization1);
            _ttvContext.DimKeywords.Add(dimKeyword4_Organization1);
            var dimKeyword1_Organization2 = new DimKeyword()
            {
                Keyword = "digitalization",
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                DimReferencedataIdLanguageCode = -1,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime
            };
            var dimKeyword2_Organization2 = new DimKeyword()
            {
                Keyword = "aerosols",
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                DimReferencedataIdLanguageCode = -1,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime
            };
            var dimKeyword3_Organization2 = new DimKeyword()
            {
                Keyword = "internal medicine",
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                DimReferencedataIdLanguageCode = -1,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime
            };
            var dimKeyword4_Organization2 = new DimKeyword()
            {
                Keyword = "history of Finland",
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                DimReferencedataIdLanguageCode = -1,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime
            };
            _ttvContext.DimKeywords.Add(dimKeyword1_Organization2);
            _ttvContext.DimKeywords.Add(dimKeyword2_Organization2);
            _ttvContext.DimKeywords.Add(dimKeyword3_Organization2);
            _ttvContext.DimKeywords.Add(dimKeyword4_Organization2);
  

            var factFieldValue_keyword1_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_keyword1_Organization1.DimUserProfile = dimUserProfile;
            factFieldValue_keyword1_Organization1.DimFieldDisplaySettings = dimFieldDisplaySettings_keyword;
            factFieldValue_keyword1_Organization1.DimKeyword = dimKeyword1_Organization1;
            factFieldValue_keyword1_Organization1.DimRegisteredDataSourceId = organization1RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword1_Organization1);
            var factFieldValue_keyword2_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_keyword2_Organization1.DimUserProfile = dimUserProfile;
            factFieldValue_keyword2_Organization1.DimFieldDisplaySettings = dimFieldDisplaySettings_keyword;
            factFieldValue_keyword2_Organization1.DimKeyword = dimKeyword2_Organization1;
            factFieldValue_keyword2_Organization1.DimRegisteredDataSourceId = organization1RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword2_Organization1);
            var factFieldValue_keyword3_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_keyword3_Organization1.DimUserProfile = dimUserProfile;
            factFieldValue_keyword3_Organization1.DimFieldDisplaySettings = dimFieldDisplaySettings_keyword;
            factFieldValue_keyword3_Organization1.DimKeyword = dimKeyword3_Organization1;
            factFieldValue_keyword3_Organization1.DimRegisteredDataSourceId = organization1RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword3_Organization1);
            var factFieldValue_keyword4_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_keyword4_Organization1.DimUserProfile = dimUserProfile;
            factFieldValue_keyword4_Organization1.DimFieldDisplaySettings = dimFieldDisplaySettings_keyword;
            factFieldValue_keyword4_Organization1.DimKeyword = dimKeyword4_Organization1;
            factFieldValue_keyword4_Organization1.DimRegisteredDataSourceId = organization1RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword4_Organization1);
            var factFieldValue_keyword1_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_keyword1_Organization2.DimUserProfile = dimUserProfile;
            factFieldValue_keyword1_Organization2.DimFieldDisplaySettings = dimFieldDisplaySettings_keyword;
            factFieldValue_keyword1_Organization2.DimKeyword = dimKeyword1_Organization2;
            factFieldValue_keyword1_Organization2.DimRegisteredDataSourceId = organization2RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword1_Organization2);
            var factFieldValue_keyword2_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_keyword2_Organization2.DimUserProfile = dimUserProfile;
            factFieldValue_keyword2_Organization2.DimFieldDisplaySettings = dimFieldDisplaySettings_keyword;
            factFieldValue_keyword2_Organization2.DimKeyword = dimKeyword2_Organization2;
            factFieldValue_keyword2_Organization2.DimRegisteredDataSourceId = organization2RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword2_Organization2);
            var factFieldValue_keyword3_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_keyword3_Organization2.DimUserProfile = dimUserProfile;
            factFieldValue_keyword3_Organization2.DimFieldDisplaySettings = dimFieldDisplaySettings_keyword;
            factFieldValue_keyword3_Organization2.DimKeyword = dimKeyword3_Organization2;
            factFieldValue_keyword3_Organization2.DimRegisteredDataSourceId = organization2RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword3_Organization2);
            var factFieldValue_keyword4_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_keyword4_Organization2.DimUserProfile = dimUserProfile;
            factFieldValue_keyword4_Organization2.DimFieldDisplaySettings = dimFieldDisplaySettings_keyword;
            factFieldValue_keyword4_Organization2.DimKeyword = dimKeyword4_Organization2;
            factFieldValue_keyword4_Organization2.DimRegisteredDataSourceId = organization2RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword4_Organization2);


            // Fields of science org1
            var dimFieldDisplaySettings_fieldOfScience = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE);
            var dimFieldOfScience1_Organization1 = _ttvContext.DimFieldOfSciences.FirstOrDefault(dfos => dfos.NameFi == this.DemoOrganization1FieldOfScience1 && dfos.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
            var dimFieldOfScience2_Organization1 = _ttvContext.DimFieldOfSciences.FirstOrDefault(dfos => dfos.NameFi == this.DemoOrganization1FieldOfScience2 && dfos.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
            var factFieldValue_fieldOfScience1_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_fieldOfScience1_Organization1.DimUserProfile = dimUserProfile;
            factFieldValue_fieldOfScience1_Organization1.DimFieldDisplaySettings = dimFieldDisplaySettings_fieldOfScience;
            factFieldValue_fieldOfScience1_Organization1.DimFieldOfScience = dimFieldOfScience1_Organization1;
            factFieldValue_fieldOfScience1_Organization1.DimRegisteredDataSourceId = organization1RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_fieldOfScience1_Organization1);
            var factFieldValue_fieldOfScience2_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_fieldOfScience2_Organization1.DimUserProfile = dimUserProfile;
            factFieldValue_fieldOfScience2_Organization1.DimFieldDisplaySettings = dimFieldDisplaySettings_fieldOfScience;
            factFieldValue_fieldOfScience2_Organization1.DimFieldOfScience = dimFieldOfScience2_Organization1;
            factFieldValue_fieldOfScience2_Organization1.DimRegisteredDataSourceId = organization1RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_fieldOfScience2_Organization1);

            // Fields of science org2
            var dimFieldOfScience1_Organization2 = _ttvContext.DimFieldOfSciences.FirstOrDefault(dfos => dfos.NameFi == this.DemoOrganization2FieldOfScience1 && dfos.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
            var dimFieldOfScience2_Organization2 = _ttvContext.DimFieldOfSciences.FirstOrDefault(dfos => dfos.NameFi == this.DemoOrganization2FieldOfScience2 && dfos.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);
            var factFieldValue_fieldOfScience1_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_fieldOfScience1_Organization2.DimUserProfile = dimUserProfile;
            factFieldValue_fieldOfScience1_Organization2.DimFieldDisplaySettings = dimFieldDisplaySettings_fieldOfScience;
            factFieldValue_fieldOfScience1_Organization2.DimFieldOfScience = dimFieldOfScience1_Organization2;
            factFieldValue_fieldOfScience1_Organization2.DimRegisteredDataSourceId = organization2RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_fieldOfScience1_Organization2);
            var factFieldValue_fieldOfScience2_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_fieldOfScience2_Organization2.DimUserProfile = dimUserProfile;
            factFieldValue_fieldOfScience2_Organization2.DimFieldDisplaySettings = dimFieldDisplaySettings_fieldOfScience;
            factFieldValue_fieldOfScience2_Organization2.DimFieldOfScience = dimFieldOfScience2_Organization2;
            factFieldValue_fieldOfScience2_Organization2.DimRegisteredDataSourceId = organization2RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_fieldOfScience2_Organization2);



            //var dimFieldDisplaySettings_fieldOfScience_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE);
            //var dimFieldOfScience1_Organization2 = new DimFieldOfScience()
            //{
            //    NameFi = "Yleislääketiede",
            //    FieldId = "",
            //    SourceId = Constants.SourceIdentifiers.DEMO,
            //    SourceDescription = sourceDescription,
            //    Created = currentDateTime,
            //    Modified = currentDateTime
            //};
            //var dimFieldOfScience2_Organization2 = new DimFieldOfScience()
            //{
            //    NameFi = "sisätaudit ja muut kliiniset lääketieteet",
            //    FieldId = "",
            //    SourceId = Constants.SourceIdentifiers.DEMO,
            //    SourceDescription = sourceDescription,
            //    Created = currentDateTime,
            //    Modified = currentDateTime
            //};
            //_ttvContext.DimFieldOfSciences.Add(dimFieldOfScience1_Organization2);
            //_ttvContext.DimFieldOfSciences.Add(dimFieldOfScience2_Organization2);

            //var factFieldValue_fieldOfScience1_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            //factFieldValue_fieldOfScience1_Organization1.DimUserProfile = dimUserProfile;
            //factFieldValue_fieldOfScience1_Organization1.DimFieldDisplaySettings = dimFieldDisplaySettings_fieldOfScience_Organization1;
            //factFieldValue_fieldOfScience1_Organization1.DimFieldOfScience = dimFieldOfScience1_Organization1;
            //_ttvContext.FactFieldValues.Add(factFieldValue_fieldOfScience1_Organization1);


            // Email
            var dimFieldDisplaySettings_email = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS);
            var dimEmail_Organization1 = new DimEmailAddrress()
            {
                Email = "tuisku.tutkija@yliopisto.fi",
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime
            };
            _ttvContext.DimEmailAddrresses.Add(dimEmail_Organization1);
            var dimEmail_Organization2 = new DimEmailAddrress()
            {
                Email = "ami.asiantuntija@tutkimuslaitos.fi",
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime
            };
            _ttvContext.DimEmailAddrresses.Add(dimEmail_Organization2);
            var factFieldValue_emails_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_emails_Organization1.DimUserProfile = dimUserProfile;
            factFieldValue_emails_Organization1.DimFieldDisplaySettings = dimFieldDisplaySettings_email;
            factFieldValue_emails_Organization1.DimEmailAddrress = dimEmail_Organization1;
            factFieldValue_emails_Organization1.DimRegisteredDataSourceId = organization1RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_emails_Organization1);
            var factFieldValue_emails_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_emails_Organization2.DimUserProfile = dimUserProfile;
            factFieldValue_emails_Organization2.DimFieldDisplaySettings = dimFieldDisplaySettings_email;
            factFieldValue_emails_Organization2.DimEmailAddrress = dimEmail_Organization2;
            factFieldValue_emails_Organization2.DimRegisteredDataSourceId = organization2RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_emails_Organization2);


            // Telephone number
            var dimFieldDisplaySettings_telephone = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER);
            var dimTelephone_Organization1 = new DimTelephoneNumber()
            {
                TelephoneNumber = "+35899999999",
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime
            };
            _ttvContext.DimTelephoneNumbers.Add(dimTelephone_Organization1);
            var factFieldValue_telephone_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_telephone_Organization1.DimUserProfile = dimUserProfile;
            factFieldValue_telephone_Organization1.DimFieldDisplaySettings = dimFieldDisplaySettings_telephone;
            factFieldValue_telephone_Organization1.DimTelephoneNumber = dimTelephone_Organization1;
            factFieldValue_telephone_Organization1.DimRegisteredDataSourceId = organization1RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_telephone_Organization1);


            // Web link
            var dimFieldDisplaySettings_weblink = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_WEB_LINK);
            var dimWeblink_Organization1 = new DimWebLink()
            {
                Url = "https://tutkijanomaverkkosivu.yliopisto.fii",
                LinkLabel = "Tutkijan oma verkkosivu",
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime
            };
            _ttvContext.DimWebLinks.Add(dimWeblink_Organization1);
            var factFieldValue_weblink_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_weblink_Organization1.DimUserProfile = dimUserProfile;
            factFieldValue_weblink_Organization1.DimFieldDisplaySettings = dimFieldDisplaySettings_weblink;
            factFieldValue_weblink_Organization1.DimWebLink = dimWeblink_Organization1;
            factFieldValue_weblink_Organization1.DimRegisteredDataSourceId = organization1RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_weblink_Organization1);


            // Role in research community



            // Affiliation - Organization1
            var affiliationType = await _ttvContext.DimReferencedata.AsNoTracking().FirstOrDefaultAsync(drd => drd.SourceId == Constants.SourceIdentifiers.DEMO_COMMON && drd.NameFi == "Työsuhde");
            var dimFieldDisplaySettings_affiliation = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_AFFILIATION);
            var dimStartDate_affiliation_organization1 = await _ttvContext.DimDates.FirstOrDefaultAsync(dd => dd.Year == 2020 && dd.Month == 1 && dd.Day == 1);
            if (dimStartDate_affiliation_organization1 == null)
            {
                dimStartDate_affiliation_organization1 = new DimDate()
                {
                    Year = 2020,
                    Month = 1,
                    Day = 1,
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    SourceDescription = sourceDescription,
                    Created = currentDateTime,
                    Modified = currentDateTime
                };
                _ttvContext.DimDates.Add(dimStartDate_affiliation_organization1);
            }
            var dimAffiliation_Organization1 = new DimAffiliation()
            {
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                DimOrganizationId = organization1.Id,
                StartDateNavigation = dimStartDate_affiliation_organization1,
                AffiliationType = affiliationType.Id,
                PositionNameFi = "Akatemiatutkija",
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime,
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id
            };
            _ttvContext.DimAffiliations.Add(dimAffiliation_Organization1);
            var factFieldValue_affiliation_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_affiliation_Organization1.DimUserProfile = dimUserProfile;
            factFieldValue_affiliation_Organization1.DimFieldDisplaySettings = dimFieldDisplaySettings_affiliation;
            factFieldValue_affiliation_Organization1.DimAffiliation = dimAffiliation_Organization1;
            factFieldValue_affiliation_Organization1.DimRegisteredDataSourceId = organization1RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_affiliation_Organization1);

            // Affiliation - Organization2
            var dimStartDate_affiliation_organization2 = await _ttvContext.DimDates.FirstOrDefaultAsync(dd => dd.Year == 2016 && dd.Month == 1 && dd.Day == 1);
            if (dimStartDate_affiliation_organization2 == null)
            {
                dimStartDate_affiliation_organization2 = new DimDate()
                {
                    Year = 2016,
                    Month = 1,
                    Day = 1,
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    SourceDescription = sourceDescription,
                    Created = currentDateTime,
                    Modified = currentDateTime
                };
                _ttvContext.DimDates.Add(dimStartDate_affiliation_organization2);
            }
            var dimEndDate_affiliation_organization2 = await _ttvContext.DimDates.FirstOrDefaultAsync(dd => dd.Year == 2019 && dd.Month == 12 && dd.Day == 31);
            if (dimEndDate_affiliation_organization2 == null)
            {
                dimEndDate_affiliation_organization2 = new DimDate()
                {
                    Year = 2019,
                    Month = 12,
                    Day = 31,
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    SourceDescription = sourceDescription,
                    Created = currentDateTime,
                    Modified = currentDateTime
                };
                _ttvContext.DimDates.Add(dimEndDate_affiliation_organization2);
            }
            var dimAffiliation_Organization2 = new DimAffiliation()
            {
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                DimOrganizationId = organization2.Id,
                StartDateNavigation = dimStartDate_affiliation_organization2,
                EndDateNavigation = dimEndDate_affiliation_organization2,
                AffiliationType = affiliationType.Id,
                PositionNameFi = "Erikoistutkija",
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime,
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id
            };
            _ttvContext.DimAffiliations.Add(dimAffiliation_Organization2);
            var factFieldValue_affiliation_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_affiliation_Organization2.DimUserProfile = dimUserProfile;
            factFieldValue_affiliation_Organization2.DimFieldDisplaySettings = dimFieldDisplaySettings_affiliation;
            factFieldValue_affiliation_Organization2.DimAffiliation = dimAffiliation_Organization2;
            factFieldValue_affiliation_Organization2.DimRegisteredDataSourceId = organization2RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_affiliation_Organization2);


            // Education
            var dimFieldDisplaySettings_education= dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_EDUCATION);
            var dimEndDate_education_organization2 = await _ttvContext.DimDates.FirstOrDefaultAsync(dd => dd.Year == 2011 && dd.Month == 0 && dd.Day == 0);
            if (dimEndDate_education_organization2 == null)
            {
                dimEndDate_education_organization2 = new DimDate()
                {
                    Year = 2011,
                    Month = 0,
                    Day = 0,
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    SourceDescription = sourceDescription,
                    Created = currentDateTime,
                    Modified = currentDateTime
                };
                _ttvContext.DimDates.Add(dimEndDate_education_organization2);
            }
            var dimEducation_Organization2 = new DimEducation()
            {
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                NameFi = "Filosofian tohtori, luonnontieteellinen ala",
                DegreeGrantingInstitutionName = "Yliopisto Y",
                DimEndDateNavigation = dimEndDate_education_organization2,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime,
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id
            };
            _ttvContext.DimEducations.Add(dimEducation_Organization2);
            var factFieldValue_education_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_education_Organization2.DimUserProfile = dimUserProfile;
            factFieldValue_education_Organization2.DimFieldDisplaySettings = dimFieldDisplaySettings_education;
            factFieldValue_education_Organization2.DimEducation = dimEducation_Organization2;
            factFieldValue_education_Organization2.DimRegisteredDataSourceId = organization2RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_education_Organization2);


            // Funding decision - Organization1
            var dimFundingDecision_Organization1 = await _ttvContext.DimFundingDecisions.FirstOrDefaultAsync(dfd => dfd.SourceId == Constants.SourceIdentifiers.DEMO_COMMON && dfd.FunderProjectNumber == this.DemoFunderProjectNumber1);
            var dimFieldDisplaySettings_fundingDecision = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION);
            var factFieldValue_fundingDecision_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_fundingDecision_Organization1.DimUserProfile = dimUserProfile;
            factFieldValue_fundingDecision_Organization1.DimFieldDisplaySettings = dimFieldDisplaySettings_fundingDecision;
            factFieldValue_fundingDecision_Organization1.DimFundingDecision = dimFundingDecision_Organization1;
            factFieldValue_fundingDecision_Organization1.DimRegisteredDataSourceId = organization1RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_fundingDecision_Organization1);

            // Funding decision - Organization2
            var dimFundingDecision_Organization2 = await _ttvContext.DimFundingDecisions.FirstOrDefaultAsync(dfd => dfd.SourceId == Constants.SourceIdentifiers.DEMO_COMMON && dfd.FunderProjectNumber == this.DemoFunderProjectNumber2);
            var factFieldValue_fundingDecision_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_fundingDecision_Organization2.DimUserProfile = dimUserProfile;
            factFieldValue_fundingDecision_Organization2.DimFieldDisplaySettings = dimFieldDisplaySettings_fundingDecision;
            factFieldValue_fundingDecision_Organization2.DimFundingDecision = dimFundingDecision_Organization2;
            factFieldValue_fundingDecision_Organization2.DimRegisteredDataSourceId = organization2RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_fundingDecision_Organization2);


            // Research datasets

            // Research dataset - DimReferencedata - Agentrole - Creator
            var dimReferencedata_Agentrole_Creator = await _ttvContext.DimReferencedata.FirstOrDefaultAsync(dr => dr.CodeScheme == "agentrole" && dr.CodeValue == this.DemoReferenceDataAgentRoleCreator && dr.SourceId == Constants.SourceIdentifiers.DEMO_COMMON);

            // Research dataset - Organization1 - DimResearchDataset
            var researchDataset_Organization1 = new DimResearchDataset()
                {
                    LocalIdentifier = this.DemoResearchDatasetLocalIdentifier1 + " " + orcidId,
                    NameEn = "Test dataset of things - years 2010-2020",
                    DescriptionEn = "Dataset that contains infromation from years 2010 to 2020. Includes information about subjects, tests and fully describes the outcome of analysis done. Most complete dataset to date about information. Please, see included documentation on how the analysis were done and data collected.",
                    DatasetCreated = new DateTime(2021, 1, 1),
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    SourceDescription = sourceDescription,
                    Created = currentDateTime,
                    Modified = currentDateTime,
                    DimRegisteredDataSourceId = organization1RegisteredDataSource.Id
            };
            _ttvContext.DimResearchDatasets.Add(researchDataset_Organization1);
            await _ttvContext.SaveChangesAsync(); // Do not remove this, DimPid creation requires researchDataset_Organization1.Id to be defined.

            // Research dataset - Organization1 - DimPid
            var dimPid_researchDataset_Organization1 = _userProfileService.GetEmptyDimPid();
            dimPid_researchDataset_Organization1.DimResearchDatasetId = researchDataset_Organization1.Id;
            dimPid_researchDataset_Organization1.PidType = "doi";
            dimPid_researchDataset_Organization1.PidContent = "doi:10.23729/12348970-30de-4c86-a69a-ae4ff1c0c49f";
            dimPid_researchDataset_Organization1.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
            dimPid_researchDataset_Organization1.SourceDescription = sourceDescription;
            dimPid_researchDataset_Organization1.SourceId = Constants.SourceIdentifiers.DEMO;
            _ttvContext.DimPids.Add(dimPid_researchDataset_Organization1);

            // Research dataset - Organization1 - FactContribution
            var factContribution_researchDataset_Organization1 = new FactContribution()
            {
                DimNameId = dimNameOrganization1.Id,
                DimResearchDatasetId = researchDataset_Organization1.Id,
                DimReferencedataActorRole = dimReferencedata_Agentrole_Creator,
                DimFundingDecisionId = -1,
                DimOrganizationId = -1,
                DimDateId = -1,
                DimPublicationId = -1,
                DimGeoId = -1,
                DimInfrastructureId = -1,
                DimNewsFeedId = -1,
                DimResearchDataCatalogId = -1,
                DimIdentifierlessDataId = -1,
                DimResearchActivityId = -1,
                DimResearchCommunityId = -1,
                DimReferencedataActorRoleId = -1,
                ContributionType = "",
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime
            };
            _ttvContext.FactContributions.Add(factContribution_researchDataset_Organization1);

            // Research dataset - Organization1 - FactFieldValue
            var dimFieldDisplaySettings_researchDataset = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET);
            var factFieldValue_researchDataset_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_researchDataset_Organization1.DimUserProfile = dimUserProfile;
            factFieldValue_researchDataset_Organization1.DimFieldDisplaySettings = dimFieldDisplaySettings_researchDataset;
            factFieldValue_researchDataset_Organization1.DimResearchDataset = researchDataset_Organization1;
            factFieldValue_researchDataset_Organization1.DimRegisteredDataSourceId = organization1RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_researchDataset_Organization1);


            // Research dataset - Organization2 - DimResearchDataset
            var researchDataset_Organization2 = new DimResearchDataset()
            {
                LocalIdentifier = this.DemoResearchDatasetLocalIdentifier2 + " " + orcidId,
                NameEn = "Preview of test subjects, first experiment",
                DescriptionEn = "Includes preview of test subjects and the first round of experiment. Further experiments are to be added and published later in another dataset. Data is structured so that information is clear. For any questions about dataset, analysis or experiments, contact us.",
                DatasetCreated = new DateTime(2017, 1, 1),
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime(),
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id
            };
            _ttvContext.DimResearchDatasets.Add(researchDataset_Organization2);
            await _ttvContext.SaveChangesAsync(); // Do not remove this, DimPid creation requires researchDataset_Organization2.Id to be defined.

            // Research dataset - Organization2 - DimPid
            var dimPid_researchDataset_Organization2 = _userProfileService.GetEmptyDimPid();
            dimPid_researchDataset_Organization2.DimResearchDatasetId = researchDataset_Organization2.Id;
            dimPid_researchDataset_Organization2.PidType = "urn";
            dimPid_researchDataset_Organization2.PidContent = "urn:nbn:fi:att:496efc55-f1fa-469a-bd39-12556068435";
            dimPid_researchDataset_Organization2.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
            dimPid_researchDataset_Organization2.SourceId = Constants.SourceIdentifiers.DEMO;
            dimPid_researchDataset_Organization2.SourceDescription = sourceDescription;
            _ttvContext.DimPids.Add(dimPid_researchDataset_Organization2);

            // Research dataset - Organization2 - FactContribution
            var factContribution_researchDataset_Organization2 = new FactContribution()
            {
                DimName = dimNameOrganization2,
                DimResearchDatasetId = researchDataset_Organization2.Id,
                DimReferencedataActorRole = dimReferencedata_Agentrole_Creator,
                DimFundingDecisionId = -1,
                DimOrganizationId = -1,
                DimDateId = -1,
                DimPublicationId = -1,
                DimGeoId = -1,
                DimInfrastructureId = -1,
                DimNewsFeedId = -1,
                DimResearchDataCatalogId = -1,
                DimIdentifierlessDataId = -1,
                DimResearchActivityId = -1,
                DimResearchCommunityId = -1,
                DimReferencedataActorRoleId = -1,
                ContributionType = "",
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = sourceDescription,
                Created = currentDateTime,
                Modified = currentDateTime
            };
            _ttvContext.FactContributions.Add(factContribution_researchDataset_Organization2);

            // Research dataset - Organization2 - FactFieldValue
            var factFieldValue_researchDataset_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_researchDataset_Organization2.DimUserProfile = dimUserProfile;
            factFieldValue_researchDataset_Organization2.DimFieldDisplaySettings = dimFieldDisplaySettings_researchDataset;
            factFieldValue_researchDataset_Organization2.DimResearchDataset = researchDataset_Organization2;
            factFieldValue_researchDataset_Organization2.DimRegisteredDataSourceId = organization2RegisteredDataSource.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_researchDataset_Organization2);

            await _ttvContext.SaveChangesAsync();
        }
    }
}