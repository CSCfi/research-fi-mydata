using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using api.Models.Common;
using api.Models.ProfileEditor.Items;
using System.Diagnostics;

namespace api.Services.Profiledata
{
    public class NameService : INameService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILanguageService _languageService;
        private readonly ILogger<NameService> _logger;


        public NameService(
            TtvContext ttvContext,
            ILanguageService languageService,
            ILogger<NameService> logger)
        {
            _ttvContext = ttvContext;
            _languageService = languageService;
            _logger = logger;
        }

        /*
         * Names
         */
        public async Task<List<ProfileEditorName>> GetProfileEditorNames(int userprofileId, bool forElasticsearch = false)
        {
            var stopwatch = Stopwatch.StartNew();
            List<ProfileEditorName> names = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimNameId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.PERSON_NAME
                && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new ProfileEditorName()
                {
                    FirstNames = ffv.DimName.FirstNames.Trim(),
                    LastName = ffv.DimName.LastName.Trim(),
                    FullName = $"{ffv.DimName.LastName.Trim()} {ffv.DimName.FirstNames.Trim()}".Trim(), // Populate for Elasticsearch queries
                    itemMeta = new ProfileEditorItemMeta(
                        ffv.DimNameId,
                        Constants.ItemMetaTypes.PERSON_NAME,
                        ffv.Show,
                        ffv.PrimaryValue
                    ),
                    DataSources = new List<ProfileEditorSource> {
                        new ProfileEditorSource() {
                            Id = ffv.DimRegisteredDataSourceId,
                            RegisteredDataSource = ffv.DimRegisteredDataSource.Name,
                            Organization = new Organization() {
                                NameFi = ffv.DimRegisteredDataSource.DimOrganization.NameFi,
                                NameEn = ffv.DimRegisteredDataSource.DimOrganization.NameEn,
                                NameSv = ffv.DimRegisteredDataSource.DimOrganization.NameSv,
                                SectorId = ffv.DimRegisteredDataSource.DimOrganization.DimSector.SectorId
                            }
                        }
                    }
                }).AsNoTracking().ToListAsync();

            // Postprocessing. Translate data source organizaton names.
            foreach (ProfileEditorName name in names)
            {
                foreach (ProfileEditorSource dataSource in name.DataSources)
                {
                    NameTranslation dataSourceOrganizationName = _languageService.GetNameTranslation(
                        nameFi: dataSource.Organization.NameFi,
                        nameEn: dataSource.Organization.NameEn,
                        nameSv: dataSource.Organization.NameSv
                    );
                    dataSource.Organization.NameFi = dataSourceOrganizationName.NameFi;
                    dataSource.Organization.NameEn = dataSourceOrganizationName.NameEn;
                    dataSource.Organization.NameSv = dataSourceOrganizationName.NameSv;
                }
            }

            stopwatch.Stop();
            _logger.LogInformation($"GetProfileEditorNames. {names.Count} items in {stopwatch.ElapsedMilliseconds}ms.");

            return names;
        }

        /*
         * Other Names
         */
        public async Task<List<ProfileEditorName>> GetProfileEditorOtherNames(int userprofileId, bool forElasticsearch = false)
        {
            var stopwatch = Stopwatch.StartNew();
            List<ProfileEditorName> otherNames = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimNameId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.PERSON_OTHER_NAMES
                && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new ProfileEditorName()
                {
                    FirstNames = "",
                    LastName = "",
                    FullName = ffv.DimName.FullName.Trim(),
                    itemMeta = new ProfileEditorItemMeta(
                        ffv.DimNameId,
                        Constants.ItemMetaTypes.PERSON_OTHER_NAMES,
                        ffv.Show,
                        ffv.PrimaryValue
                    ),
                    DataSources = new List<ProfileEditorSource> {
                        new ProfileEditorSource() {
                            Id = ffv.DimRegisteredDataSourceId,
                            RegisteredDataSource = ffv.DimRegisteredDataSource.Name,
                            Organization = new Organization() {
                                NameFi = ffv.DimRegisteredDataSource.DimOrganization.NameFi,
                                NameEn = ffv.DimRegisteredDataSource.DimOrganization.NameEn,
                                NameSv = ffv.DimRegisteredDataSource.DimOrganization.NameSv,
                                SectorId = ffv.DimRegisteredDataSource.DimOrganization.DimSector.SectorId
                            }
                        }
                    }
                }).AsNoTracking().ToListAsync();

            // Postprocessing. Translate data source organizaton names.
            foreach (ProfileEditorName otherName in otherNames)
            {
                foreach (ProfileEditorSource dataSource in otherName.DataSources)
                {
                    NameTranslation dataSourceOrganizationName = _languageService.GetNameTranslation(
                        nameFi: dataSource.Organization.NameFi,
                        nameEn: dataSource.Organization.NameEn,
                        nameSv: dataSource.Organization.NameSv
                    );
                    dataSource.Organization.NameFi = dataSourceOrganizationName.NameFi;
                    dataSource.Organization.NameEn = dataSourceOrganizationName.NameEn;
                    dataSource.Organization.NameSv = dataSourceOrganizationName.NameSv;
                }
            }

            stopwatch.Stop();
            _logger.LogInformation($"GetProfileEditorOtherNames. {otherNames.Count} items in {stopwatch.ElapsedMilliseconds}ms.");

            return otherNames;
        }
    }
}