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

        public DemoDataService(TtvContext ttvContext, UserProfileService userProfileService)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
        }

        public void AddRegisteredDatasources()
        {
            // Registered data source: Yliopisto A
            var datasourceYliopistoA = _ttvContext.DimRegisteredDataSources.FirstOrDefault(drds => drds.SourceId == Constants.SourceIdentifiers.DEMO && drds.Name == "Yliopisto A");
            if (datasourceYliopistoA == null)
            {
                datasourceYliopistoA = new DimRegisteredDataSource()
                {
                    Name = "Yliopisto A",
                    DimOrganizationId = -1,
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    SourceDescription = "Yliopisto A",
                    Created = DateTime.Now
                };
                _ttvContext.DimRegisteredDataSources.Add(datasourceYliopistoA);
            }

            // Registered data source: Tutkimuslaitos X
            var datasourceTutkimuslaitosX = _ttvContext.DimRegisteredDataSources.FirstOrDefault(drds => drds.Name == "Tutkimuslaitos X");
            if (datasourceTutkimuslaitosX == null)
            {
                datasourceTutkimuslaitosX = new DimRegisteredDataSource()
                {
                    Name = "Tutkimuslaitos X",
                    DimOrganizationId = -1,
                    SourceId = Constants.SourceIdentifiers.DEMO,
                    SourceDescription = "Tutkimuslaitos X",
                    Created = DateTime.Now
                };
                _ttvContext.DimRegisteredDataSources.Add(datasourceTutkimuslaitosX);
            }

            _ttvContext.SaveChanges();
        }

        public async Task<int> GetYliopistoARegisteredDataSourceId()
        {
            var yliopistoARegisteredDataSource = await _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefaultAsync(drds => drds.Name == "Yliopisto A" && drds.SourceId == Constants.SourceIdentifiers.DEMO);
            if (yliopistoARegisteredDataSource == null)
            {
                return -1;
            }
            else
            {
                return yliopistoARegisteredDataSource.Id;
            }
        }

        public async Task<int> GetTutkimuslaitosXRegisteredDataSourceId()
        {
            var tutkimuslaitosXRegisteredDataSource = await _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefaultAsync(drds => drds.Name == "Tutkimuslaitos X" && drds.SourceId == Constants.SourceIdentifiers.DEMO);
            if (tutkimuslaitosXRegisteredDataSource == null)
            {
                return -1;
            }
            else
            {
                return tutkimuslaitosXRegisteredDataSource.Id;
            }
        }

        public async Task AddDemoDataToUserProfile(DimUserProfile dimUserProfile)
        {
            var datasourceYliopistoA = await _ttvContext.DimRegisteredDataSources.FirstOrDefaultAsync(drds => drds.SourceId == Constants.SourceIdentifiers.DEMO && drds.Name == "Yliopisto A");
            var datasourceTutkimuslaitosX = await _ttvContext.DimRegisteredDataSources.FirstOrDefaultAsync(drds => drds.SourceId == Constants.SourceIdentifiers.DEMO && drds.Name == "Tutkimuslaitos X");

            // Name
            var dimFieldDisplaySettings_name_YliopistoA = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == "Yliopisto A" && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_NAME);
            var dimNameYliopistoA = new DimName()
            {
                FirstNames = "Tuisku",
                LastName = "Tutkija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now,
                DimRegisteredDataSourceId = datasourceYliopistoA.Id
            };
            _ttvContext.DimNames.Add(dimNameYliopistoA);

            var dimFieldDisplaySettings_name_TutkimuslaitosX = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == "Tutkimuslaitos X" && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_NAME);
            var dimNameTutkimuslaitosX = new DimName()
            {
                FirstNames = "Ami",
                LastName = "Asiantuntija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now,
                DimRegisteredDataSourceId = datasourceTutkimuslaitosX.Id
            };
            _ttvContext.DimNames.Add(dimNameTutkimuslaitosX);
            await _ttvContext.SaveChangesAsync();

            var factFieldValue_name_yliopistoA = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_name_yliopistoA.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_name_yliopistoA.DimFieldDisplaySettingsId = dimFieldDisplaySettings_name_YliopistoA.Id;
            factFieldValue_name_yliopistoA.DimNameId = dimNameYliopistoA.Id;
            factFieldValue_name_yliopistoA.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_name_yliopistoA.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_name_yliopistoA);
            var factFieldValue_name_tutkimuslaitosX = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_name_tutkimuslaitosX.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_name_tutkimuslaitosX.DimFieldDisplaySettingsId = dimFieldDisplaySettings_name_TutkimuslaitosX.Id;
            factFieldValue_name_tutkimuslaitosX.DimNameId = dimNameTutkimuslaitosX.Id;
            factFieldValue_name_tutkimuslaitosX.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_name_tutkimuslaitosX.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_name_tutkimuslaitosX);
            await _ttvContext.SaveChangesAsync();


            // Other names
            var dimFieldDisplaySettings_othername_YliopistoA = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == "Yliopisto A" && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_OTHER_NAMES);
            var dimOtherNameYliopistoA_1 = new DimName()
            {
                FullName = "T. Tutkija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now,
                DimRegisteredDataSourceId = datasourceYliopistoA.Id
            };
            _ttvContext.DimNames.Add(dimOtherNameYliopistoA_1);
            var dimOtherNameYliopistoA_2 = new DimName()
            {
                FullName = "T.A. Tutkija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now,
                DimRegisteredDataSourceId = datasourceYliopistoA.Id
            };
            _ttvContext.DimNames.Add(dimOtherNameYliopistoA_2);
            var dimFieldDisplaySettings_othername_TutkimuslaitosX = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == "Tutkimuslaitos X" && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_OTHER_NAMES);
            var dimOtherNameTutkimuslaitosX = new DimName()
            {
                FullName = "Tuisku Tutkija",
                DimKnownPersonIdConfirmedIdentity = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now,
                DimRegisteredDataSourceId = datasourceTutkimuslaitosX.Id
            };
            _ttvContext.DimNames.Add(dimOtherNameTutkimuslaitosX);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_othername_yliopistoA_1 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_othername_yliopistoA_1.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_othername_yliopistoA_1.DimFieldDisplaySettingsId = dimFieldDisplaySettings_othername_YliopistoA.Id;
            factFieldValue_othername_yliopistoA_1.DimNameId = dimOtherNameYliopistoA_1.Id;
            factFieldValue_othername_yliopistoA_1.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_othername_yliopistoA_1.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_othername_yliopistoA_1);
            var factFieldValue_othername_yliopistoA_2 = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_othername_yliopistoA_2.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_othername_yliopistoA_2.DimFieldDisplaySettingsId = dimFieldDisplaySettings_othername_YliopistoA.Id;
            factFieldValue_othername_yliopistoA_2.DimNameId = dimOtherNameYliopistoA_2.Id;
            factFieldValue_othername_yliopistoA_2.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_othername_yliopistoA_2.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_othername_yliopistoA_2);
            var factFieldValue_othername_tutkimuslaitosX = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_othername_tutkimuslaitosX.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_othername_tutkimuslaitosX.DimFieldDisplaySettingsId = dimFieldDisplaySettings_othername_TutkimuslaitosX.Id;
            factFieldValue_othername_tutkimuslaitosX.DimNameId = dimOtherNameTutkimuslaitosX.Id;
            factFieldValue_othername_tutkimuslaitosX.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_othername_tutkimuslaitosX.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_othername_tutkimuslaitosX);
            await _ttvContext.SaveChangesAsync();


            // External identifiers (DimPid)
            var dimFieldDisplaySettings_externalIdentifier_YliopistoA = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == "Yliopisto A" && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER);
            var dimPid_yliopistoA = new DimPid()
            {
                PidContent = "O-2000-1000",
                PidType = "ResearcherID",
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            _ttvContext.DimPids.Add(dimPid_yliopistoA);
            var dimFieldDisplaySettings_externalIdentifier_TutkimuslaitosX = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == "Tutkimuslaitos X" && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER);
            var dimPid_tutkimuslaitosX = new DimPid()
            {
                PidContent = "https://isni.org/isni/0000000100020003",
                PidType = "ISNI",
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            _ttvContext.DimPids.Add(dimPid_tutkimuslaitosX);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_externalIdentifier_yliopistoA = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_externalIdentifier_yliopistoA.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_externalIdentifier_yliopistoA.DimFieldDisplaySettingsId = dimFieldDisplaySettings_externalIdentifier_YliopistoA.Id;
            factFieldValue_externalIdentifier_yliopistoA.DimPidId = dimPid_yliopistoA.Id;
            factFieldValue_externalIdentifier_yliopistoA.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_externalIdentifier_yliopistoA.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_externalIdentifier_yliopistoA);
            var factFieldValue_externalIdentifier_tutkimuslaitosX = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_externalIdentifier_tutkimuslaitosX.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_externalIdentifier_tutkimuslaitosX.DimFieldDisplaySettingsId = dimFieldDisplaySettings_externalIdentifier_TutkimuslaitosX.Id;
            factFieldValue_externalIdentifier_tutkimuslaitosX.DimPidId = dimPid_tutkimuslaitosX.Id;
            factFieldValue_externalIdentifier_tutkimuslaitosX.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_externalIdentifier_tutkimuslaitosX.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_externalIdentifier_tutkimuslaitosX);
            await _ttvContext.SaveChangesAsync();


            // Researcher description
            var dimFieldDisplaySettings_researcherDescription_YliopistoA = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == "Yliopisto A" && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION);
            var dimResearcherDescription_yliopistoA = new DimResearcherDescription()
            {
                ResearchDescriptionFi = "Tutkimuksen kuvausta suomeksi. Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                ResearchDescriptionEn = "Description of my research in English. Duis ullamcorper sem in sapien pretium bibendum. Vestibulum ex dui, volutpat commodo condimentum sed, lobortis at justo.",
                ResearchDescriptionSv = "Beskrivning av forskningen på svenska. Fusce in lorem tempor, feugiat nunc ut, consectetur erat. Integer purus sem, hendrerit at bibendum vel, laoreet nec tellus.",
                DimRegisteredDataSourceId = datasourceYliopistoA.Id,
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            _ttvContext.DimResearcherDescriptions.Add(dimResearcherDescription_yliopistoA);
            var dimFieldDisplaySettings_researcherDescription_TutkimuslaitosX = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == "Tutkimuslaitos X" && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION);
            var dimResearcherDescription_tutkimuslaitosX = new DimResearcherDescription()
            {
                ResearchDescriptionFi = "Tutkimuksen kuvausta suomeksi. Duis finibus velit rutrum euismod scelerisque. Praesent sit amet fermentum ex. Donec vitae tellus eu nisl dignissim laoreet.",
                DimRegisteredDataSourceId = datasourceTutkimuslaitosX.Id,
                DimKnownPersonId = dimUserProfile.DimKnownPersonId,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            _ttvContext.DimResearcherDescriptions.Add(dimResearcherDescription_tutkimuslaitosX);
            await _ttvContext.SaveChangesAsync();
            var factFieldValue_researcherDescription_yliopistoA = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_researcherDescription_yliopistoA.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_researcherDescription_yliopistoA.DimFieldDisplaySettingsId = dimFieldDisplaySettings_researcherDescription_YliopistoA.Id;
            factFieldValue_researcherDescription_yliopistoA.DimResearcherDescriptionId = dimResearcherDescription_yliopistoA.Id;
            factFieldValue_researcherDescription_yliopistoA.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_researcherDescription_yliopistoA.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_researcherDescription_yliopistoA);
            var factFieldValue_researcherDescription_tutkimuslaitosX = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_researcherDescription_tutkimuslaitosX.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_researcherDescription_tutkimuslaitosX.DimFieldDisplaySettingsId = dimFieldDisplaySettings_researcherDescription_TutkimuslaitosX.Id;
            factFieldValue_researcherDescription_tutkimuslaitosX.DimResearcherDescriptionId = dimResearcherDescription_tutkimuslaitosX.Id;
            factFieldValue_researcherDescription_tutkimuslaitosX.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_researcherDescription_tutkimuslaitosX.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_researcherDescription_tutkimuslaitosX);
            await _ttvContext.SaveChangesAsync();


            // Keywords
            var dimFieldDisplaySettings_keyword_YliopistoA = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == "Yliopisto A" && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_KEYWORD);
            var dimKeyword1_yliopistoA = new DimKeyword()
            {
                Keyword = "digitalisaatio",
                DimRegisteredDataSourceId = datasourceYliopistoA.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            var dimKeyword2_yliopistoA = new DimKeyword()
            {
                Keyword = "aerosolit",
                DimRegisteredDataSourceId = datasourceYliopistoA.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            var dimKeyword3_yliopistoA = new DimKeyword()
            {
                Keyword = "sisätaudit",
                DimRegisteredDataSourceId = datasourceYliopistoA.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            var dimKeyword4_yliopistoA = new DimKeyword()
            {
                Keyword = "Suomen historia",
                DimRegisteredDataSourceId = datasourceYliopistoA.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            _ttvContext.DimKeywords.Add(dimKeyword1_yliopistoA);
            _ttvContext.DimKeywords.Add(dimKeyword2_yliopistoA);
            _ttvContext.DimKeywords.Add(dimKeyword3_yliopistoA);
            _ttvContext.DimKeywords.Add(dimKeyword4_yliopistoA);
            var dimFieldDisplaySettings_keyword_TutkimuslaitosX = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == "Tutkimuslaitos X" && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_KEYWORD);
            var dimKeyword1_tutkimuslaitosX = new DimKeyword()
            {
                Keyword = "digitalization",
                DimRegisteredDataSourceId = datasourceTutkimuslaitosX.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            var dimKeyword2_tutkimuslaitosX = new DimKeyword()
            {
                Keyword = "aerosols",
                DimRegisteredDataSourceId = datasourceTutkimuslaitosX.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            var dimKeyword3_tutkimuslaitosX = new DimKeyword()
            {
                Keyword = "internal medicine",
                DimRegisteredDataSourceId = datasourceTutkimuslaitosX.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            var dimKeyword4_tutkimuslaitosX = new DimKeyword()
            {
                Keyword = "history of Finland",
                DimRegisteredDataSourceId = datasourceTutkimuslaitosX.Id,
                SourceId = Constants.SourceIdentifiers.DEMO,
                Created = DateTime.Now
            };
            _ttvContext.DimKeywords.Add(dimKeyword1_tutkimuslaitosX);
            _ttvContext.DimKeywords.Add(dimKeyword2_tutkimuslaitosX);
            _ttvContext.DimKeywords.Add(dimKeyword3_tutkimuslaitosX);
            _ttvContext.DimKeywords.Add(dimKeyword4_tutkimuslaitosX);
            await _ttvContext.SaveChangesAsync();

            var factFieldValue_keyword1_yliopistoA = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_keyword1_yliopistoA.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword1_yliopistoA.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_YliopistoA.Id;
            factFieldValue_keyword1_yliopistoA.DimKeywordId = dimKeyword1_yliopistoA.Id;
            factFieldValue_keyword1_yliopistoA.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_keyword1_yliopistoA.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword1_yliopistoA);
            var factFieldValue_keyword2_yliopistoA = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_keyword2_yliopistoA.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword2_yliopistoA.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_YliopistoA.Id;
            factFieldValue_keyword2_yliopistoA.DimKeywordId = dimKeyword2_yliopistoA.Id;
            factFieldValue_keyword2_yliopistoA.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_keyword2_yliopistoA.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword2_yliopistoA);
            var factFieldValue_keyword3_yliopistoA = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_keyword3_yliopistoA.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword3_yliopistoA.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_YliopistoA.Id;
            factFieldValue_keyword3_yliopistoA.DimKeywordId = dimKeyword3_yliopistoA.Id;
            factFieldValue_keyword3_yliopistoA.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_keyword3_yliopistoA.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword3_yliopistoA);
            var factFieldValue_keyword4_yliopistoA = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_keyword4_yliopistoA.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword4_yliopistoA.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_YliopistoA.Id;
            factFieldValue_keyword4_yliopistoA.DimKeywordId = dimKeyword4_yliopistoA.Id;
            factFieldValue_keyword4_yliopistoA.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_keyword4_yliopistoA.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword4_yliopistoA);
            var factFieldValue_keyword1_tutkimuslaitosX = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_keyword1_tutkimuslaitosX.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword1_tutkimuslaitosX.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_TutkimuslaitosX.Id;
            factFieldValue_keyword1_tutkimuslaitosX.DimKeywordId = dimKeyword1_tutkimuslaitosX.Id;
            factFieldValue_keyword1_tutkimuslaitosX.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_keyword1_tutkimuslaitosX.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword1_tutkimuslaitosX);
            var factFieldValue_keyword2_tutkimuslaitosX = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_keyword2_tutkimuslaitosX.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword2_tutkimuslaitosX.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_TutkimuslaitosX.Id;
            factFieldValue_keyword2_tutkimuslaitosX.DimKeywordId = dimKeyword2_tutkimuslaitosX.Id;
            factFieldValue_keyword2_tutkimuslaitosX.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_keyword2_tutkimuslaitosX.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword2_tutkimuslaitosX);
            var factFieldValue_keyword3_tutkimuslaitosX = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_keyword3_tutkimuslaitosX.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword3_tutkimuslaitosX.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_TutkimuslaitosX.Id;
            factFieldValue_keyword3_tutkimuslaitosX.DimKeywordId = dimKeyword3_tutkimuslaitosX.Id;
            factFieldValue_keyword3_tutkimuslaitosX.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_keyword3_tutkimuslaitosX.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword3_tutkimuslaitosX);
            var factFieldValue_keyword4_tutkimuslaitosX = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_keyword4_tutkimuslaitosX.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_keyword4_tutkimuslaitosX.DimFieldDisplaySettingsId = dimFieldDisplaySettings_keyword_TutkimuslaitosX.Id;
            factFieldValue_keyword4_tutkimuslaitosX.DimKeywordId = dimKeyword4_tutkimuslaitosX.Id;
            factFieldValue_keyword4_tutkimuslaitosX.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_keyword4_tutkimuslaitosX.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_keyword4_tutkimuslaitosX);
            await _ttvContext.SaveChangesAsync();
        }
    }
}