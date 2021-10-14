using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;

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
        private readonly string DemoOrganization1Name = "Yliopisto A";
        private readonly string DemoOrganization2Name = "Tutkimuslaitos X";
        private readonly string DemoOrganization3Name = Constants.SourceIdentifiers.TIEDEJATUTKIMUS;
        private readonly string DemoOrganization1DataSourceName = "Testidata";
        private readonly string DemoOrganization2DataSourceName = "Testidata";
        private readonly string DemoOrganization3DataSourceName = Constants.SourceIdentifiers.TIEDEJATUTKIMUS;
        private readonly string DemoOrganization1FieldOfScience1 = "Fysiikka";
        private readonly string DemoOrganization1FieldOfScience2 = "Historia";
        private readonly string DemoOrganization2FieldOfScience1 = "Yleislääketiede";
        private readonly string DemoOrganization2FieldOfScience2 = "Sisätaudit ja muut kliiniset lääketieteet";

        public DemoDataService(TtvContext ttvContext, UserProfileService userProfileService, UtilityService utilityService)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _utilityService = utilityService;
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

        public DimOrganization GetOrganization1()
        {
            return _ttvContext.DimOrganizations.FirstOrDefault(org => org.SourceId == Constants.SourceIdentifiers.DEMO && org.NameFi == this.DemoOrganization1Name);
        }

        public DimOrganization GetOrganization2()
        {
            return _ttvContext.DimOrganizations.FirstOrDefault(org => org.SourceId == Constants.SourceIdentifiers.DEMO && org.NameFi == this.DemoOrganization2Name);
        }

        public DimOrganization GetOrganization3()
        {
            return _ttvContext.DimOrganizations.FirstOrDefault(org => org.SourceId == Constants.SourceIdentifiers.DEMO && org.NameFi == this.DemoOrganization3Name);
        }

        public DimRegisteredDataSource GetOrganization1RegisteredDataSource()
        {
            var organization1 = this.GetOrganization1();
            return _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefault(drds => drds.DimOrganizationId == organization1.Id && drds.Name == this.DemoOrganization1DataSourceName && drds.SourceId == Constants.SourceIdentifiers.DEMO);
        }

        public DimRegisteredDataSource GetOrganization2RegisteredDataSource()
        {
            var organization2 = this.GetOrganization2();
            return _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefault(drds => drds.DimOrganizationId == organization2.Id && drds.Name == this.DemoOrganization2DataSourceName && drds.SourceId == Constants.SourceIdentifiers.DEMO);
        }

        public DimRegisteredDataSource GetOrganization3RegisteredDataSource()
        {
            var organization3 = this.GetOrganization3();
            return _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefault(drds => drds.DimOrganizationId == organization3.Id && drds.Name == this.DemoOrganization3DataSourceName && drds.SourceId == Constants.SourceIdentifiers.DEMO);
        }

        public void AddOrganizations()
        {
            // Organization 1
            var organization1 = this.GetOrganization1();
            if (organization1 == null)
            {
                organization1 = new DimOrganization()
                {
                    DimSectorid = -1,
                    NameFi = this.DemoOrganization1Name,
                    SourceId = Constants.SourceIdentifiers.DEMO,
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
                    SourceId = Constants.SourceIdentifiers.DEMO,
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
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime(),
                    DimRegisteredDataSourceId = -1
                };
                _ttvContext.DimOrganizations.Add(organization3);
            }

            _ttvContext.SaveChanges();
        }


        public void AddRegisteredDatasources()
        {
            // Registered data source 1
            var organization1 = this.GetOrganization1();
            var registeredDatasourceOrg1 = this.GetOrganization1RegisteredDataSource();
            if (registeredDatasourceOrg1 == null)
            {
                registeredDatasourceOrg1 = new DimRegisteredDataSource()
                {
                    Name = this.DemoOrganization1DataSourceName,
                    DimOrganizationId = organization1.Id,
                    SourceId = Constants.SourceIdentifiers.DEMO,
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
                    DimOrganizationId = organization2.Id,
                    SourceId = Constants.SourceIdentifiers.DEMO,
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
                    DimOrganizationId = organization3.Id,
                    SourceId = Constants.SourceIdentifiers.DEMO,
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
            var referenceData = _ttvContext.DimReferencedata.FirstOrDefault(dr => dr.SourceId == Constants.SourceIdentifiers.DEMO && dr.NameFi == "Työsuhde");
            if (referenceData == null)
            {
                referenceData = new DimReferencedatum()
                {
                    CodeScheme = "",
                    CodeValue = "",
                    NameFi = "Työsuhde",
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime()
                };
                _ttvContext.DimReferencedata.Add(referenceData);
                _ttvContext.SaveChanges();
            }
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
        //            DimRegisteredDataSourceId = registeredDatasourceOrg1.Id
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
        //            DimRegisteredDataSourceId = registeredDatasourceOrg2.Id
        //        }
        //    );
        //    _ttvContext.SaveChanges();
        //}


        public void AddFieldsOfScience()
        {
            var fieldsOfScienceNames = new List<string> {
                this.DemoOrganization1FieldOfScience1,
                this.DemoOrganization1FieldOfScience2,
                this.DemoOrganization2FieldOfScience1,
                this.DemoOrganization2FieldOfScience2
            };
            foreach (string fieldOfScienceName in fieldsOfScienceNames)
            {
                var dimFieldOfScience = _ttvContext.DimFieldOfSciences.FirstOrDefault(dfos => dfos.NameFi == fieldOfScienceName && dfos.SourceId == Constants.SourceIdentifiers.DEMO);
            
                if (dimFieldOfScience == null)
                {
                    dimFieldOfScience = new DimFieldOfScience()
                    {
                        FieldId = " ",
                        NameFi = fieldOfScienceName,
                        SourceId = Constants.SourceIdentifiers.DEMO,
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
            this.AddReferenceData(); // DimAffiliation.PositionCode => DimReferenceData
            //this.AddResearchCommunities();
            this.AddFieldsOfScience();
        }


        public async Task<DimOrganization> GetOrganization1Async()
        {
            return await _ttvContext.DimOrganizations.AsNoTracking().FirstOrDefaultAsync(org => org.NameFi == this.DemoOrganization1Name && org.SourceId == Constants.SourceIdentifiers.DEMO);
        }

        public async Task<DimOrganization> GetOrganization2Async()
        {
            return await _ttvContext.DimOrganizations.AsNoTracking().FirstOrDefaultAsync(org => org.NameFi == this.DemoOrganization2Name && org.SourceId == Constants.SourceIdentifiers.DEMO);
        }

        public async Task<DimOrganization> GetOrganization3Async()
        {
            return await _ttvContext.DimOrganizations.AsNoTracking().FirstOrDefaultAsync(org => org.NameFi == this.DemoOrganization3Name && org.SourceId == Constants.SourceIdentifiers.DEMO);
        }

        public async Task<DimRegisteredDataSource> GetOrganization1RegisteredDataSourceAsync()
        {
            var organization1 = await this.GetOrganization1Async();
            return await _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefaultAsync(drds => drds.DimOrganizationId == organization1.Id && drds.Name == this.DemoOrganization1DataSourceName && drds.SourceId == Constants.SourceIdentifiers.DEMO);
        }

        public async Task<DimRegisteredDataSource> GetOrganization2RegisteredDataSourceAsync()
        {
            var organization2 = await this.GetOrganization2Async();
            return await _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefaultAsync(drds => drds.DimOrganizationId == organization2.Id && drds.Name == this.DemoOrganization2DataSourceName && drds.SourceId == Constants.SourceIdentifiers.DEMO);
        }

        public async Task<DimRegisteredDataSource> GetOrganization3RegisteredDataSourceAsync()
        {
            var organization3 = await this.GetOrganization3Async();
            return await _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefaultAsync(drds => drds.DimOrganizationId == organization3.Id && drds.Name == this.DemoOrganization3DataSourceName && drds.SourceId == Constants.SourceIdentifiers.DEMO);
        }

        public async Task AddDemoDataToUserProfile(DimUserProfile dimUserProfile)
        {
            var organization1 = await this.GetOrganization1Async();
            var organization2 = await this.GetOrganization2Async();
            var organization1RegisteredDataSource = await this.GetOrganization1RegisteredDataSourceAsync();
            var organization2RegisteredDataSource = await this.GetOrganization2RegisteredDataSourceAsync();

            // Name
            var dimFieldDisplaySettings_name_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_NAME);
            var dimNameOrganization1 = new DimName()
            {
                FirstNames = "Tuisku",
                LastName = "Tutkija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                DimKnownPersonidFormerNames = -1,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime(),
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id
            };
            _ttvContext.DimNames.Add(dimNameOrganization1);

            var dimFieldDisplaySettings_name_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_NAME);
            var dimNameOrganization2 = new DimName()
            {
                FirstNames = "Ami",
                LastName = "Asiantuntija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                DimKnownPersonidFormerNames = -1,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime(),
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id
            };
            _ttvContext.DimNames.Add(dimNameOrganization2);
            await _ttvContext.SaveChangesAsync();

            var factFieldValue_name_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_name_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_name_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_name_Organization1.Id;
            factFieldValue_name_Organization1.DimNameId = dimNameOrganization1.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_name_Organization1);
            var factFieldValue_name_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_name_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_name_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_name_Organization2.Id;
            factFieldValue_name_Organization2.DimNameId = dimNameOrganization2.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_name_Organization2);
            await _ttvContext.SaveChangesAsync();


            // Other names
            var dimFieldDisplaySettings_othername_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_OTHER_NAMES);
            var dimOtherNameOrganization1_1 = new DimName()
            {
                FullName = "T. Tutkija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                DimKnownPersonidFormerNames = -1,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime(),
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id
            };
            _ttvContext.DimNames.Add(dimOtherNameOrganization1_1);
            var dimOtherNameOrganization1_2 = new DimName()
            {
                FullName = "T.A. Tutkija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                DimKnownPersonidFormerNames = -1,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime(),
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id
            };
            _ttvContext.DimNames.Add(dimOtherNameOrganization1_2);
            var dimFieldDisplaySettings_othername_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_OTHER_NAMES);
            var dimOtherNameOrganization2 = new DimName()
            {
                FullName = "Tuisku Tutkija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                DimKnownPersonidFormerNames = -1,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime(),
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id
            };
            _ttvContext.DimNames.Add(dimOtherNameOrganization2);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_othername_Organization1_1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_othername_Organization1_1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_othername_Organization1_1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_othername_Organization1.Id;
            factFieldValue_othername_Organization1_1.DimNameId = dimOtherNameOrganization1_1.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_othername_Organization1_1);
            var factFieldValue_othername_Organization1_2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_othername_Organization1_2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_othername_Organization1_2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_othername_Organization1.Id;
            factFieldValue_othername_Organization1_2.DimNameId = dimOtherNameOrganization1_2.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_othername_Organization1_2);
            var factFieldValue_othername_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_othername_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_othername_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_othername_Organization2.Id;
            factFieldValue_othername_Organization2.DimNameId = dimOtherNameOrganization2.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_othername_Organization2);
            await _ttvContext.SaveChangesAsync();


            // External identifiers (DimPid)
            var dimFieldDisplaySettings_externalIdentifier_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER);
            var dimPid_Organization1 = _userProfileService.GetEmptyDimPid();
            dimPid_Organization1.PidContent = "O-2000-1000";
            dimPid_Organization1.PidType = "ResearcherID";
            dimPid_Organization1.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
            dimPid_Organization1.SourceId = Constants.SourceIdentifiers.DEMO;
            _ttvContext.DimPids.Add(dimPid_Organization1);
            var dimFieldDisplaySettings_externalIdentifier_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER);
            var dimPid_Organization2 = _userProfileService.GetEmptyDimPid();
            dimPid_Organization2.PidContent = "https://isni.org/isni/0000000100020003";
            dimPid_Organization2.PidType = "ISNI";
            dimPid_Organization2.DimKnownPersonId = dimUserProfile.DimKnownPersonId;
            dimPid_Organization2.SourceId = Constants.SourceIdentifiers.DEMO;
            dimPid_Organization2.SourceDescription = Constants.SourceDescriptions.PROFILE_API;
            dimPid_Organization2.Created = _utilityService.getCurrentDateTime();
            _ttvContext.DimPids.Add(dimPid_Organization2);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_externalIdentifier_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_externalIdentifier_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_externalIdentifier_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_externalIdentifier_Organization1.Id;
            factFieldValue_externalIdentifier_Organization1.DimPidId = dimPid_Organization1.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_externalIdentifier_Organization1);
            var factFieldValue_externalIdentifier_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_externalIdentifier_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_externalIdentifier_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_externalIdentifier_Organization2.Id;
            factFieldValue_externalIdentifier_Organization2.DimPidId = dimPid_Organization2.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_externalIdentifier_Organization2);
            await _ttvContext.SaveChangesAsync();


            // Researcher description
            var dimFieldDisplaySettings_researcherDescription_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION);
            var dimResearcherDescription_Organization1 = new DimResearcherDescription()
            {
                ResearchDescriptionFi = "Tutkimuksen kuvausta suomeksi. Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                ResearchDescriptionEn = "Description of my research in English. Duis ullamcorper sem in sapien pretium bibendum. Vestibulum ex dui, volutpat commodo condimentum sed, lobortis at justo.",
                ResearchDescriptionSv = "Beskrivning av forskningen på svenska. Fusce in lorem tempor, feugiat nunc ut, consectetur erat. Integer purus sem, hendrerit at bibendum vel, laoreet nec tellus.",
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime()
            };
            _ttvContext.DimResearcherDescriptions.Add(dimResearcherDescription_Organization1);
            var dimFieldDisplaySettings_researcherDescription_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION);
            var dimResearcherDescription_Organization2 = new DimResearcherDescription()
            {
                ResearchDescriptionFi = "Tutkimuksen kuvausta suomeksi. Duis finibus velit rutrum euismod scelerisque. Praesent sit amet fermentum ex. Donec vitae tellus eu nisl dignissim laoreet.",
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime()
            };
            _ttvContext.DimResearcherDescriptions.Add(dimResearcherDescription_Organization2);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_researcherDescription_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_researcherDescription_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_researcherDescription_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_researcherDescription_Organization1.Id;
            factFieldValue_researcherDescription_Organization1.DimResearcherDescriptionId = dimResearcherDescription_Organization1.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_researcherDescription_Organization1);
            var factFieldValue_researcherDescription_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_researcherDescription_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_researcherDescription_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_researcherDescription_Organization2.Id;
            factFieldValue_researcherDescription_Organization2.DimResearcherDescriptionId = dimResearcherDescription_Organization2.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_researcherDescription_Organization2);
            await _ttvContext.SaveChangesAsync();


            // Keywords
            var dimFieldDisplaySettings_keyword_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_KEYWORD);
            var dimKeyword1_Organization1 = new DimKeyword()
            {
                Keyword = "digitalisaatio",
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime()
            };
            var dimKeyword2_Organization1 = new DimKeyword()
            {
                Keyword = "aerosolit",
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime()
            };
            var dimKeyword3_Organization1 = new DimKeyword()
            {
                Keyword = "sisätaudit",
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime()
            };
            var dimKeyword4_Organization1 = new DimKeyword()
            {
                Keyword = "Suomen historia",
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime()
            };
            _ttvContext.DimKeywords.Add(dimKeyword1_Organization1);
            _ttvContext.DimKeywords.Add(dimKeyword2_Organization1);
            _ttvContext.DimKeywords.Add(dimKeyword3_Organization1);
            _ttvContext.DimKeywords.Add(dimKeyword4_Organization1);
            var dimFieldDisplaySettings_keyword_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_KEYWORD);
            var dimKeyword1_Organization2 = new DimKeyword()
            {
                Keyword = "digitalization",
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime()
            };
            var dimKeyword2_Organization2 = new DimKeyword()
            {
                Keyword = "aerosols",
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime()
            };
            var dimKeyword3_Organization2 = new DimKeyword()
            {
                Keyword = "internal medicine",
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime()
            };
            var dimKeyword4_Organization2 = new DimKeyword()
            {
                Keyword = "history of Finland",
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime()
            };
            _ttvContext.DimKeywords.Add(dimKeyword1_Organization2);
            _ttvContext.DimKeywords.Add(dimKeyword2_Organization2);
            _ttvContext.DimKeywords.Add(dimKeyword3_Organization2);
            _ttvContext.DimKeywords.Add(dimKeyword4_Organization2);
            await _ttvContext.SaveChangesAsync();

            var factFieldValue_keyword1_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_keyword1_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword1_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_Organization1.Id;
            factFieldValue_keyword1_Organization1.DimKeywordId = dimKeyword1_Organization1.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword1_Organization1);
            var factFieldValue_keyword2_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_keyword2_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword2_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_Organization1.Id;
            factFieldValue_keyword2_Organization1.DimKeywordId = dimKeyword2_Organization1.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword2_Organization1);
            var factFieldValue_keyword3_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_keyword3_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword3_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_Organization1.Id;
            factFieldValue_keyword3_Organization1.DimKeywordId = dimKeyword3_Organization1.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword3_Organization1);
            var factFieldValue_keyword4_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_keyword4_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword4_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_Organization1.Id;
            factFieldValue_keyword4_Organization1.DimKeywordId = dimKeyword4_Organization1.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword4_Organization1);
            var factFieldValue_keyword1_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_keyword1_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword1_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_Organization2.Id;
            factFieldValue_keyword1_Organization2.DimKeywordId = dimKeyword1_Organization2.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword1_Organization2);
            var factFieldValue_keyword2_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_keyword2_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword2_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_Organization2.Id;
            factFieldValue_keyword2_Organization2.DimKeywordId = dimKeyword2_Organization2.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword2_Organization2);
            var factFieldValue_keyword3_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_keyword3_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword3_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_Organization2.Id;
            factFieldValue_keyword3_Organization2.DimKeywordId = dimKeyword3_Organization2.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword3_Organization2);
            var factFieldValue_keyword4_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_keyword4_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword4_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_Organization2.Id;
            factFieldValue_keyword4_Organization2.DimKeywordId = dimKeyword4_Organization2.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword4_Organization2);
            await _ttvContext.SaveChangesAsync();


            // Fields of science org1
            var dimFieldDisplaySettings_fieldOfScience_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE);
            var dimFieldOfScience1_Organization1 = _ttvContext.DimFieldOfSciences.FirstOrDefault(dfos => dfos.NameFi == this.DemoOrganization1FieldOfScience1 && dfos.SourceId == Constants.SourceIdentifiers.DEMO);
            var dimFieldOfScience2_Organization1 = _ttvContext.DimFieldOfSciences.FirstOrDefault(dfos => dfos.NameFi == this.DemoOrganization1FieldOfScience2 && dfos.SourceId == Constants.SourceIdentifiers.DEMO);
            var factFieldValue_fieldOfScience1_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_fieldOfScience1_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_fieldOfScience1_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_fieldOfScience_Organization1.Id;
            factFieldValue_fieldOfScience1_Organization1.DimFieldOfScienceId = dimFieldOfScience1_Organization1.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_fieldOfScience1_Organization1);
            var factFieldValue_fieldOfScience2_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_fieldOfScience2_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_fieldOfScience2_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_fieldOfScience_Organization1.Id;
            factFieldValue_fieldOfScience2_Organization1.DimFieldOfScienceId = dimFieldOfScience2_Organization1.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_fieldOfScience2_Organization1);

            // Fields of science org2
            var dimFieldDisplaySettings_fieldOfScience_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE);
            var dimFieldOfScience1_Organization2 = _ttvContext.DimFieldOfSciences.FirstOrDefault(dfos => dfos.NameFi == this.DemoOrganization2FieldOfScience1 && dfos.SourceId == Constants.SourceIdentifiers.DEMO);
            var dimFieldOfScience2_Organization2 = _ttvContext.DimFieldOfSciences.FirstOrDefault(dfos => dfos.NameFi == this.DemoOrganization2FieldOfScience2 && dfos.SourceId == Constants.SourceIdentifiers.DEMO);
            var factFieldValue_fieldOfScience1_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_fieldOfScience1_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_fieldOfScience1_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_fieldOfScience_Organization2.Id;
            factFieldValue_fieldOfScience1_Organization2.DimFieldOfScienceId = dimFieldOfScience1_Organization2.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_fieldOfScience1_Organization2);
            var factFieldValue_fieldOfScience2_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_fieldOfScience2_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_fieldOfScience2_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_fieldOfScience_Organization2.Id;
            factFieldValue_fieldOfScience2_Organization2.DimFieldOfScienceId = dimFieldOfScience2_Organization2.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_fieldOfScience2_Organization2);

            await _ttvContext.SaveChangesAsync();



            //var dimFieldDisplaySettings_fieldOfScience_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE);
            //var dimFieldOfScience1_Organization2 = new DimFieldOfScience()
            //{
            //    NameFi = "Yleislääketiede",
            //    FieldId = "",
            //    SourceId = Constants.SourceIdentifiers.DEMO,
            //    Created = _utilityService.getCurrentDateTime(),
            //    Modified = _utilityService.getCurrentDateTime()
            //};
            //var dimFieldOfScience2_Organization2 = new DimFieldOfScience()
            //{
            //    NameFi = "sisätaudit ja muut kliiniset lääketieteet",
            //    FieldId = "",
            //    SourceId = Constants.SourceIdentifiers.DEMO,
            //    Created = _utilityService.getCurrentDateTime(),
            //    Modified = _utilityService.getCurrentDateTime()
            //};
            //_ttvContext.DimFieldOfSciences.Add(dimFieldOfScience1_Organization2);
            //_ttvContext.DimFieldOfSciences.Add(dimFieldOfScience2_Organization2);
            //await _ttvContext.SaveChangesAsync();

            //var factFieldValue_fieldOfScience1_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            //factFieldValue_fieldOfScience1_Organization1.DimUserProfileId = dimUserProfile.Id;
            //factFieldValue_fieldOfScience1_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_fieldOfScience_Organization1.Id;
            //factFieldValue_fieldOfScience1_Organization1.DimFieldOfScienceId = dimFieldOfScience1_Organization1.Id;
            //_ttvContext.FactFieldValues.Add(factFieldValue_fieldOfScience1_Organization1);


            // Email
            var dimFieldDisplaySettings_email_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS);
            var dimEmail_Organization1 = new DimEmailAddrress()
            {
                Email = "tuisku.tutkija@yliopisto.fi",
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime()
            };
            _ttvContext.DimEmailAddrresses.Add(dimEmail_Organization1);
            var dimFieldDisplaySettings_email_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS);
            var dimEmail_Organization2 = new DimEmailAddrress()
            {
                Email = "ami.asiantuntija@tutkimuslaitos.fi",
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime()
            };
            _ttvContext.DimEmailAddrresses.Add(dimEmail_Organization2);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_emails_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_emails_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_emails_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_email_Organization1.Id;
            factFieldValue_emails_Organization1.DimEmailAddrressId = dimEmail_Organization1.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_emails_Organization1);
            var factFieldValue_emails_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_emails_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_emails_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_email_Organization2.Id;
            factFieldValue_emails_Organization2.DimEmailAddrressId = dimEmail_Organization2.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_emails_Organization2);
            await _ttvContext.SaveChangesAsync();


            // Telephone number
            var dimFieldDisplaySettings_telephone_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER);
            var dimTelephone_Organization1 = new DimTelephoneNumber()
            {
                TelephoneNumber = "+35899999999",
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime()
            };
            _ttvContext.DimTelephoneNumbers.Add(dimTelephone_Organization1);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_telephone_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_telephone_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_telephone_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_telephone_Organization1.Id;
            factFieldValue_telephone_Organization1.DimTelephoneNumberId = dimTelephone_Organization1.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_telephone_Organization1);
            await _ttvContext.SaveChangesAsync();


            // Web link
            var dimFieldDisplaySettings_weblink_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_WEB_LINK);
            var dimWeblink_Organization1 = new DimWebLink()
            {
                Url = "https://tutkijanomaverkkosivu.yliopisto.fii",
                LinkLabel = "Tutkijan oma verkkosivu",
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime()
            };
            _ttvContext.DimWebLinks.Add(dimWeblink_Organization1);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_weblink_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_weblink_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_weblink_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_weblink_Organization1.Id;
            factFieldValue_weblink_Organization1.DimWebLinkId = dimWeblink_Organization1.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_weblink_Organization1);
            await _ttvContext.SaveChangesAsync();


            // Role in research community



            // Affiliation
            var affiliationType = await _ttvContext.DimReferencedata.AsNoTracking().FirstOrDefaultAsync(drd => drd.SourceId == Constants.SourceIdentifiers.DEMO && drd.NameFi == "Työsuhde");
            var dimFieldDisplaySettings_affiliation_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_AFFILIATION);
            var dimStartDate_affiliation_organization1 = await _ttvContext.DimDates.AsNoTracking().FirstOrDefaultAsync(dd => dd.Year == 2020 && dd.Month == 1 && dd.Day == 1);
            if (dimStartDate_affiliation_organization1 == null)
            {
                dimStartDate_affiliation_organization1 = new DimDate()
                {
                    Year = 2020,
                    Month = 1,
                    Day = 1,
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime()
                };
                _ttvContext.DimDates.Add(dimStartDate_affiliation_organization1);
                await _ttvContext.SaveChangesAsync();
            }
            var dimAffiliation_Organization1 = new DimAffiliation()
            {
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                DimOrganizationId = organization1.Id,
                StartDate = dimStartDate_affiliation_organization1.Id,
                AffiliationType = affiliationType.Id,
                PositionNameFi = "Akatemiatutkija",
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime(),
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id
            };
            _ttvContext.DimAffiliations.Add(dimAffiliation_Organization1);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_affiliation_Organization1 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_affiliation_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_affiliation_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_affiliation_Organization1.Id;
            factFieldValue_affiliation_Organization1.DimAffiliationId = dimAffiliation_Organization1.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_affiliation_Organization1);
            await _ttvContext.SaveChangesAsync();
            var dimFieldDisplaySettings_affiliation_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_AFFILIATION);
            var dimStartDate_affiliation_organization2 = await _ttvContext.DimDates.AsNoTracking().FirstOrDefaultAsync(dd => dd.Year == 2016 && dd.Month == 1 && dd.Day == 1);
            if (dimStartDate_affiliation_organization2 == null)
            {
                dimStartDate_affiliation_organization2 = new DimDate()
                {
                    Year = 2016,
                    Month = 1,
                    Day = 1,
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime()
                };
                _ttvContext.DimDates.Add(dimStartDate_affiliation_organization2);
                await _ttvContext.SaveChangesAsync();
            }
            var dimEndDate_affiliation_organization2 = await _ttvContext.DimDates.AsNoTracking().FirstOrDefaultAsync(dd => dd.Year == 2019 && dd.Month == 12 && dd.Day == 31);
            if (dimEndDate_affiliation_organization2 == null)
            {
                dimEndDate_affiliation_organization2 = new DimDate()
                {
                    Year = 2019,
                    Month = 12,
                    Day = 31,
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime()
                };
                _ttvContext.DimDates.Add(dimEndDate_affiliation_organization2);
                await _ttvContext.SaveChangesAsync();
            }
            var dimAffiliation_Organization2 = new DimAffiliation()
            {
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                DimOrganizationId = organization2.Id,
                StartDate = dimStartDate_affiliation_organization2.Id,
                EndDate = dimEndDate_affiliation_organization2.Id,
                AffiliationType = affiliationType.Id,
                PositionNameFi = "Erikoistutkija",
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime(),
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id
            };
            _ttvContext.DimAffiliations.Add(dimAffiliation_Organization2);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_affiliation_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_affiliation_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_affiliation_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_affiliation_Organization2.Id;
            factFieldValue_affiliation_Organization2.DimAffiliationId = dimAffiliation_Organization2.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_affiliation_Organization2);
            await _ttvContext.SaveChangesAsync();


            // Education
            var dimFieldDisplaySettings_education_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2Name && dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_EDUCATION);
            var dimEndDate_education_organization2 = await _ttvContext.DimDates.AsNoTracking().FirstOrDefaultAsync(dd => dd.Year == 2011 && dd.Month == 0 && dd.Day == 0);
            if (dimEndDate_education_organization2 == null)
            {
                dimEndDate_education_organization2 = new DimDate()
                {
                    Year = 2011,
                    Month = 0,
                    Day = 0,
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                    Created = _utilityService.getCurrentDateTime(),
                    Modified = _utilityService.getCurrentDateTime()
                };
                _ttvContext.DimDates.Add(dimEndDate_education_organization2);
                await _ttvContext.SaveChangesAsync();
            }
            var dimEducation_Organization2 = new DimEducation()
            {
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                NameFi = "Filosofian tohtori, luonnontieteellinen ala",
                DegreeGrantingInstitutionName = "Yliopisto Y",
                DimEndDate = dimEndDate_education_organization2.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                SourceDescription = Constants.SourceDescriptions.PROFILE_API,
                Created = _utilityService.getCurrentDateTime(),
                Modified = _utilityService.getCurrentDateTime(),
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id
            };
            _ttvContext.DimEducations.Add(dimEducation_Organization2);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_education_Organization2 = _userProfileService.GetEmptyFactFieldValueDemo();
            factFieldValue_education_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_education_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_education_Organization2.Id;
            factFieldValue_education_Organization2.DimEducationId = dimEducation_Organization2.Id;
            _ttvContext.FactFieldValues.Add(factFieldValue_education_Organization2);
            await _ttvContext.SaveChangesAsync();
        }
    }
}