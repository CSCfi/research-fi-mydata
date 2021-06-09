using System;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{ 
    public class DemoDataService
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;
        private readonly string DemoOrganization1 = "Yliopisto A";
        private readonly string DemoOrganization2 = "Tutkimuslaitos X";

        public DemoDataService(TtvContext ttvContext, UserProfileService userProfileService)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
        }


        public void AddOrganizations()
        {
            // Organization 1
            var organization1 = _ttvContext.DimOrganizations.FirstOrDefault(org => org.SourceId == Constants.SourceIdentifiers.DEMO && org.NameFi == this.DemoOrganization1);
            if (organization1 == null)
            {
                organization1 = new DimOrganization()
                {
                    DimSectorid = -1,
                    NameFi = this.DemoOrganization1,
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    Created = DateTime.Now,
                    DimRegisteredDataSourceId = -1
                };
                _ttvContext.DimOrganizations.Add(organization1);
            }

            // Organization 2
            var organization2 = _ttvContext.DimOrganizations.FirstOrDefault(org => org.SourceId == Constants.SourceIdentifiers.DEMO && org.NameFi == this.DemoOrganization2);
            if (organization2 == null)
            {
                organization2 = new DimOrganization()
                {
                    DimSectorid = -1,
                    NameFi = this.DemoOrganization2,
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    Created = DateTime.Now,
                    DimRegisteredDataSourceId = -1
                };
                _ttvContext.DimOrganizations.Add(organization2);
            }

            _ttvContext.SaveChanges();
        }


        public void AddRegisteredDatasources()
        {
            // Registered data source 1
            var organization1 = _ttvContext.DimOrganizations.AsNoTracking().FirstOrDefault(org => org.NameFi == this.DemoOrganization1);
            var registeredDatasourceOrg1 = _ttvContext.DimRegisteredDataSources.FirstOrDefault(drds => drds.SourceId == Constants.SourceIdentifiers.DEMO && drds.Name == this.DemoOrganization1);
            if (registeredDatasourceOrg1 == null)
            {
                registeredDatasourceOrg1 = new DimRegisteredDataSource()
                {
                    Name = "Testidata",
                    DimOrganizationId = organization1.Id,
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    Created = DateTime.Now
                };
                _ttvContext.DimRegisteredDataSources.Add(registeredDatasourceOrg1);
            }

            // Registered data source 2
            var organization2 = _ttvContext.DimOrganizations.AsNoTracking().FirstOrDefault(org => org.NameFi == this.DemoOrganization2);
            var registeredDatasourceOrg2 = _ttvContext.DimRegisteredDataSources.FirstOrDefault(drds => drds.Name == this.DemoOrganization2);
            if (registeredDatasourceOrg2 == null)
            {
                registeredDatasourceOrg2 = new DimRegisteredDataSource()
                {
                    Name = "Testidata",
                    DimOrganizationId = organization2.Id,
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    Created = DateTime.Now
                };
                _ttvContext.DimRegisteredDataSources.Add(registeredDatasourceOrg2);
            }

            _ttvContext.SaveChanges();
        }


        public void InitDemo()
        {
            this.AddOrganizations();
            this.AddRegisteredDatasources();
        }


        public async Task<DimOrganization> GetOrganization1()
        {
            return await _ttvContext.DimOrganizations.AsNoTracking().FirstOrDefaultAsync(org => org.NameFi == this.DemoOrganization1 && org.SourceId == Constants.SourceIdentifiers.DEMO);
        }

        public async Task<DimOrganization> GetOrganization2()
        {
            return await _ttvContext.DimOrganizations.AsNoTracking().FirstOrDefaultAsync(org => org.NameFi == this.DemoOrganization2 && org.SourceId == Constants.SourceIdentifiers.DEMO);
        }

        public async Task<DimRegisteredDataSource> GetOrganization1RegisteredDataSource()
        {
            var organization1 = await this.GetOrganization1();
            return await _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefaultAsync(drds => drds.DimOrganizationId == organization1.Id && drds.SourceId == Constants.SourceIdentifiers.DEMO);
        }

        public async Task<DimRegisteredDataSource> GetOrganization2RegisteredDataSource()
        {
            var organization2 = await this.GetOrganization2();
            return await _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefaultAsync(drds => drds.DimOrganizationId == organization2.Id && drds.SourceId == Constants.SourceIdentifiers.DEMO);
        }

        public async Task AddDemoDataToUserProfile(DimUserProfile dimUserProfile)
        {
            var organization1RegisteredDataSource = await this.GetOrganization1RegisteredDataSource();
            var organization2RegisteredDataSource = await this.GetOrganization2RegisteredDataSource();

            // Name
            var dimFieldDisplaySettings_name_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1 && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_NAME);
            var dimNameOrganization1 = new DimName()
            {
                FirstNames = "Tuisku",
                LastName = "Tutkija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now,
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id
            };
            _ttvContext.DimNames.Add(dimNameOrganization1);

            var dimFieldDisplaySettings_name_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2 && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_NAME);
            var dimNameOrganization2 = new DimName()
            {
                FirstNames = "Ami",
                LastName = "Asiantuntija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now,
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id
            };
            _ttvContext.DimNames.Add(dimNameOrganization2);
            await _ttvContext.SaveChangesAsync();

            var factFieldValue_name_Organization1 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_name_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_name_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_name_Organization1.Id;
            factFieldValue_name_Organization1.DimNameId = dimNameOrganization1.Id;
            factFieldValue_name_Organization1.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_name_Organization1.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_name_Organization1);
            var factFieldValue_name_Organization2 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_name_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_name_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_name_Organization2.Id;
            factFieldValue_name_Organization2.DimNameId = dimNameOrganization2.Id;
            factFieldValue_name_Organization2.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_name_Organization2.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_name_Organization2);
            await _ttvContext.SaveChangesAsync();


            // Other names
            var dimFieldDisplaySettings_othername_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1 && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_OTHER_NAMES);
            var dimOtherNameOrganization1_1 = new DimName()
            {
                FullName = "T. Tutkija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now,
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id
            };
            _ttvContext.DimNames.Add(dimOtherNameOrganization1_1);
            var dimOtherNameOrganization1_2 = new DimName()
            {
                FullName = "T.A. Tutkija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now,
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id
            };
            _ttvContext.DimNames.Add(dimOtherNameOrganization1_2);
            var dimFieldDisplaySettings_othername_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2 && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_OTHER_NAMES);
            var dimOtherNameOrganization2 = new DimName()
            {
                FullName = "Tuisku Tutkija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now,
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id
            };
            _ttvContext.DimNames.Add(dimOtherNameOrganization2);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_othername_Organization1_1 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_othername_Organization1_1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_othername_Organization1_1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_othername_Organization1.Id;
            factFieldValue_othername_Organization1_1.DimNameId = dimOtherNameOrganization1_1.Id;
            factFieldValue_othername_Organization1_1.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_othername_Organization1_1.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_othername_Organization1_1);
            var factFieldValue_othername_Organization1_2 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_othername_Organization1_2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_othername_Organization1_2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_othername_Organization1.Id;
            factFieldValue_othername_Organization1_2.DimNameId = dimOtherNameOrganization1_2.Id;
            factFieldValue_othername_Organization1_2.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_othername_Organization1_2.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_othername_Organization1_2);
            var factFieldValue_othername_Organization2 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_othername_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_othername_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_othername_Organization2.Id;
            factFieldValue_othername_Organization2.DimNameId = dimOtherNameOrganization2.Id;
            factFieldValue_othername_Organization2.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_othername_Organization2.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_othername_Organization2);
            await _ttvContext.SaveChangesAsync();


            // External identifiers (DimPid)
            var dimFieldDisplaySettings_externalIdentifier_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1 && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER);
            var dimPid_Organization1 = new DimPid()
            {
                PidContent = "O-2000-1000",
                PidType = "ResearcherID",
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            _ttvContext.DimPids.Add(dimPid_Organization1);
            var dimFieldDisplaySettings_externalIdentifier_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2 && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER);
            var dimPid_Organization2 = new DimPid()
            {
                PidContent = "https://isni.org/isni/0000000100020003",
                PidType = "ISNI",
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            _ttvContext.DimPids.Add(dimPid_Organization2);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_externalIdentifier_Organization1 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_externalIdentifier_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_externalIdentifier_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_externalIdentifier_Organization1.Id;
            factFieldValue_externalIdentifier_Organization1.DimPidId = dimPid_Organization1.Id;
            factFieldValue_externalIdentifier_Organization1.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_externalIdentifier_Organization1.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_externalIdentifier_Organization1);
            var factFieldValue_externalIdentifier_Organization2 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_externalIdentifier_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_externalIdentifier_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_externalIdentifier_Organization2.Id;
            factFieldValue_externalIdentifier_Organization2.DimPidId = dimPid_Organization2.Id;
            factFieldValue_externalIdentifier_Organization2.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_externalIdentifier_Organization2.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_externalIdentifier_Organization2);
            await _ttvContext.SaveChangesAsync();


            // Researcher description
            var dimFieldDisplaySettings_researcherDescription_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1 && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION);
            var dimResearcherDescription_Organization1 = new DimResearcherDescription()
            {
                ResearchDescriptionFi = "Tutkimuksen kuvausta suomeksi. Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                ResearchDescriptionEn = "Description of my research in English. Duis ullamcorper sem in sapien pretium bibendum. Vestibulum ex dui, volutpat commodo condimentum sed, lobortis at justo.",
                ResearchDescriptionSv = "Beskrivning av forskningen på svenska. Fusce in lorem tempor, feugiat nunc ut, consectetur erat. Integer purus sem, hendrerit at bibendum vel, laoreet nec tellus.",
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            _ttvContext.DimResearcherDescriptions.Add(dimResearcherDescription_Organization1);
            var dimFieldDisplaySettings_researcherDescription_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2 && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION);
            var dimResearcherDescription_Organization2 = new DimResearcherDescription()
            {
                ResearchDescriptionFi = "Tutkimuksen kuvausta suomeksi. Duis finibus velit rutrum euismod scelerisque. Praesent sit amet fermentum ex. Donec vitae tellus eu nisl dignissim laoreet.",
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            _ttvContext.DimResearcherDescriptions.Add(dimResearcherDescription_Organization2);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_researcherDescription_Organization1 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_researcherDescription_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_researcherDescription_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_researcherDescription_Organization1.Id;
            factFieldValue_researcherDescription_Organization1.DimResearcherDescriptionId = dimResearcherDescription_Organization1.Id;
            factFieldValue_researcherDescription_Organization1.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_researcherDescription_Organization1.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_researcherDescription_Organization1);
            var factFieldValue_researcherDescription_Organization2 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_researcherDescription_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_researcherDescription_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_researcherDescription_Organization2.Id;
            factFieldValue_researcherDescription_Organization2.DimResearcherDescriptionId = dimResearcherDescription_Organization2.Id;
            factFieldValue_researcherDescription_Organization2.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_researcherDescription_Organization2.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_researcherDescription_Organization2);
            await _ttvContext.SaveChangesAsync();


            // Keywords
            var dimFieldDisplaySettings_keyword_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1 && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_KEYWORD);
            var dimKeyword1_Organization1 = new DimKeyword()
            {
                Keyword = "digitalisaatio",
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            var dimKeyword2_Organization1 = new DimKeyword()
            {
                Keyword = "aerosolit",
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            var dimKeyword3_Organization1 = new DimKeyword()
            {
                Keyword = "sisätaudit",
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            var dimKeyword4_Organization1 = new DimKeyword()
            {
                Keyword = "Suomen historia",
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            _ttvContext.DimKeywords.Add(dimKeyword1_Organization1);
            _ttvContext.DimKeywords.Add(dimKeyword2_Organization1);
            _ttvContext.DimKeywords.Add(dimKeyword3_Organization1);
            _ttvContext.DimKeywords.Add(dimKeyword4_Organization1);
            var dimFieldDisplaySettings_keyword_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2 && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_KEYWORD);
            var dimKeyword1_Organization2 = new DimKeyword()
            {
                Keyword = "digitalization",
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            var dimKeyword2_Organization2 = new DimKeyword()
            {
                Keyword = "aerosols",
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            var dimKeyword3_Organization2 = new DimKeyword()
            {
                Keyword = "internal medicine",
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            var dimKeyword4_Organization2 = new DimKeyword()
            {
                Keyword = "history of Finland",
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            _ttvContext.DimKeywords.Add(dimKeyword1_Organization2);
            _ttvContext.DimKeywords.Add(dimKeyword2_Organization2);
            _ttvContext.DimKeywords.Add(dimKeyword3_Organization2);
            _ttvContext.DimKeywords.Add(dimKeyword4_Organization2);
            await _ttvContext.SaveChangesAsync();

            var factFieldValue_keyword1_Organization1 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_keyword1_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword1_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_Organization1.Id;
            factFieldValue_keyword1_Organization1.DimKeywordId = dimKeyword1_Organization1.Id;
            factFieldValue_keyword1_Organization1.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_keyword1_Organization1.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword1_Organization1);
            var factFieldValue_keyword2_Organization1 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_keyword2_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword2_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_Organization1.Id;
            factFieldValue_keyword2_Organization1.DimKeywordId = dimKeyword2_Organization1.Id;
            factFieldValue_keyword2_Organization1.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_keyword2_Organization1.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword2_Organization1);
            var factFieldValue_keyword3_Organization1 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_keyword3_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword3_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_Organization1.Id;
            factFieldValue_keyword3_Organization1.DimKeywordId = dimKeyword3_Organization1.Id;
            factFieldValue_keyword3_Organization1.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_keyword3_Organization1.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword3_Organization1);
            var factFieldValue_keyword4_Organization1 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_keyword4_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword4_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_Organization1.Id;
            factFieldValue_keyword4_Organization1.DimKeywordId = dimKeyword4_Organization1.Id;
            factFieldValue_keyword4_Organization1.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_keyword4_Organization1.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword4_Organization1);
            var factFieldValue_keyword1_Organization2 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_keyword1_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword1_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_Organization2.Id;
            factFieldValue_keyword1_Organization2.DimKeywordId = dimKeyword1_Organization2.Id;
            factFieldValue_keyword1_Organization2.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_keyword1_Organization2.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword1_Organization2);
            var factFieldValue_keyword2_Organization2 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_keyword2_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword2_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_Organization2.Id;
            factFieldValue_keyword2_Organization2.DimKeywordId = dimKeyword2_Organization2.Id;
            factFieldValue_keyword2_Organization2.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_keyword2_Organization2.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword2_Organization2);
            var factFieldValue_keyword3_Organization2 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_keyword3_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword3_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_Organization2.Id;
            factFieldValue_keyword3_Organization2.DimKeywordId = dimKeyword3_Organization2.Id;
            factFieldValue_keyword3_Organization2.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_keyword3_Organization2.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword3_Organization2);
            var factFieldValue_keyword4_Organization2 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_keyword4_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword4_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_Organization2.Id;
            factFieldValue_keyword4_Organization2.DimKeywordId = dimKeyword4_Organization2.Id;
            factFieldValue_keyword4_Organization2.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_keyword4_Organization2.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword4_Organization2);
            await _ttvContext.SaveChangesAsync();


            //// Fields of science
            //var dimFieldDisplaySettings_fieldOfScience_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1 && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE);
            //var dimFieldOfScience1_Organization1 = new DimFieldOfScience()
            //{
            //    NameFi = "Fysiikka",
            //    FieldId = "",
            //    SourceId = Constants.SourceIdentifiers.DEMO,
            //    Created = DateTime.Now
            //};
            //var dimFieldOfScience2_Organization1 = new DimFieldOfScience()
            //{
            //    NameFi = "historia",
            //    FieldId = "",
            //    SourceId = Constants.SourceIdentifiers.DEMO,
            //    Created = DateTime.Now
            //};
            //_ttvContext.DimFieldOfSciences.Add(dimFieldOfScience1_Organization1);
            //_ttvContext.DimFieldOfSciences.Add(dimFieldOfScience2_Organization1);
            //var dimFieldDisplaySettings_fieldOfScience_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2 && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE);
            //var dimFieldOfScience1_Organization2 = new DimFieldOfScience()
            //{
            //    NameFi = "Yleislääketiede",
            //    FieldId = "",
            //    SourceId = Constants.SourceIdentifiers.DEMO,
            //    Created = DateTime.Now
            //};
            //var dimFieldOfScience2_Organization2 = new DimFieldOfScience()
            //{
            //    NameFi = "sisätaudit ja muut kliiniset lääketieteet",
            //    FieldId = "",
            //    SourceId = Constants.SourceIdentifiers.DEMO,
            //    Created = DateTime.Now
            //};
            //_ttvContext.DimFieldOfSciences.Add(dimFieldOfScience1_Organization2);
            //_ttvContext.DimFieldOfSciences.Add(dimFieldOfScience2_Organization2);
            //await _ttvContext.SaveChangesAsync();

            //var factFieldValue_fieldOfScience1_Organization1 = _userProfileService.GetEmptyFactFieldValue();
            //factFieldValue_fieldOfScience1_Organization1.DimUserProfileId = dimUserProfile.Id;
            //factFieldValue_fieldOfScience1_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_fieldOfScience_Organization1.Id;
            //factFieldValue_fieldOfScience1_Organization1.DimFieldOfScienceId = dimFieldOfScience1_Organization1.Id;
            //factFieldValue_fieldOfScience1_Organization1.SourceId = Constants.SourceIdentifiers.DEMO;
            //factFieldValue_fieldOfScience1_Organization1.Created = DateTime.Now;
            //_ttvContext.FactFieldValues.Add(factFieldValue_fieldOfScience1_Organization1);


            // Email
            var dimFieldDisplaySettings_email_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1 && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS);
            var dimEmail_Organization1 = new DimEmailAddrress()
            {
                Email = "tuisku.tutkija@yliopisto.fi",
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                DimRegisteredDataSourceId = organization1RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            _ttvContext.DimEmailAddrresses.Add(dimEmail_Organization1);
            var dimFieldDisplaySettings_email_Organization2 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization2 && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS);
            var dimEmail_Organization2 = new DimEmailAddrress()
            {
                Email = "ami.asiantuntija@tutkimuslaitos.fi",
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            _ttvContext.DimEmailAddrresses.Add(dimEmail_Organization2);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_emails_Organization1 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_emails_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_emails_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_email_Organization1.Id;
            factFieldValue_emails_Organization1.DimEmailAddrressId = dimEmail_Organization1.Id;
            factFieldValue_emails_Organization1.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_emails_Organization1.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_emails_Organization1);
            var factFieldValue_emails_Organization2 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_emails_Organization2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_emails_Organization2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_email_Organization2.Id;
            factFieldValue_emails_Organization2.DimEmailAddrressId = dimEmail_Organization2.Id;
            factFieldValue_emails_Organization2.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_emails_Organization2.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_emails_Organization2);
            await _ttvContext.SaveChangesAsync();


            // Telephone number
            var dimFieldDisplaySettings_telephone_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1 && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER);
            var dimTelephone_Organization1 = new DimTelephoneNumber()
            {
                TelephoneNumber = "+35899999999",
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                DimRegisteredDataSourceId = organization2RegisteredDataSource.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            _ttvContext.DimTelephoneNumbers.Add(dimTelephone_Organization1);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_telephone_Organization1 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_telephone_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_telephone_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_telephone_Organization1.Id;
            factFieldValue_telephone_Organization1.DimTelephoneNumberId = dimTelephone_Organization1.Id;
            factFieldValue_telephone_Organization1.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_telephone_Organization1.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_telephone_Organization1);
            await _ttvContext.SaveChangesAsync();


            // Web link
            var dimFieldDisplaySettings_weblink_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1 && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_WEB_LINK);
            var dimWeblink_Organization1 = new DimWebLink()
            {
                Url = "https://tutkijanomaverkkosivu.yliopisto.fii",
                LinkLabel = "Tutkijan oma verkkosivu",
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            _ttvContext.DimWebLinks.Add(dimWeblink_Organization1);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_weblink_Organization1 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_weblink_Organization1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_weblink_Organization1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_weblink_Organization1.Id;
            factFieldValue_weblink_Organization1.DimWebLinkId = dimWeblink_Organization1.Id;
            factFieldValue_weblink_Organization1.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_weblink_Organization1.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_weblink_Organization1);
            await _ttvContext.SaveChangesAsync();


            // Affiliation
            //var dimFieldDisplaySettings_affiliation_Organization1 = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == this.DemoOrganization1 && dfds.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_AFFILIATION);

        }
    }
}