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

            // Names
            var dimFieldDisplaySettingsYliopistoA = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == "Yliopisto A" && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_NAME);
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

            var dimFieldDisplaySettingsTutkimuslaitosX = dimUserProfile.DimFieldDisplaySettings.FirstOrDefault(dfds => dfds.SourceId == Constants.SourceIdentifiers.DEMO && dfds.SourceDescription == "Tutkimuslaitos X" && dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_NAME);
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
            factFieldValue_name_yliopistoA.DimFieldDisplaySettingsId = dimFieldDisplaySettingsYliopistoA.Id;
            factFieldValue_name_yliopistoA.DimNameId = dimNameYliopistoA.Id;
            factFieldValue_name_yliopistoA.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_name_yliopistoA.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_name_yliopistoA);
            var factFieldValue_name_tutkimuslaitosX = _userProfileService.GetEmptyFactFieldValue();
            factFieldValue_name_tutkimuslaitosX.DimUserProfileId = dimUserProfile.Id;
            factFieldValue_name_tutkimuslaitosX.DimFieldDisplaySettingsId = dimFieldDisplaySettingsTutkimuslaitosX.Id;
            factFieldValue_name_tutkimuslaitosX.DimNameId = dimNameTutkimuslaitosX.Id;
            factFieldValue_name_tutkimuslaitosX.SourceId = Constants.SourceIdentifiers.DEMO;
            factFieldValue_name_tutkimuslaitosX.Created = DateTime.Now;
            _ttvContext.FactFieldValues.Add(factFieldValue_name_tutkimuslaitosX);
            await _ttvContext.SaveChangesAsync();
        }
    }
}