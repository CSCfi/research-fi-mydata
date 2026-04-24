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
    public class UniqueDataSourcesService : IUniqueDataSourcesService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILanguageService _languageService;
        private readonly ILogger<UniqueDataSourcesService> _logger;


        public UniqueDataSourcesService(
            TtvContext ttvContext,
            ILanguageService languageService,
            ILogger<UniqueDataSourcesService> logger)
        {
            _ttvContext = ttvContext;
            _languageService = languageService;
            _logger = logger;
        }

        /*
         * Unique data sources.
         */
        public async Task<List<ProfileEditorSource>> GetUniqueDataSources(int userprofileId, bool forElasticsearch = false)
        {
            var stopwatch = Stopwatch.StartNew();
            List<ProfileEditorSource> dataSources = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimRegisteredDataSourceId > 0
                && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new ProfileEditorSource()
                {
                    Id = ffv.DimRegisteredDataSourceId,
                    RegisteredDataSource = ffv.DimRegisteredDataSource.Name,
                    Organization = new Organization() {
                        NameFi = ffv.DimRegisteredDataSource.DimOrganization.NameFi,
                        NameEn = ffv.DimRegisteredDataSource.DimOrganization.NameEn,
                        NameSv = ffv.DimRegisteredDataSource.DimOrganization.NameSv,
                        SectorId = ffv.DimRegisteredDataSource.DimOrganization.DimSector.SectorId
                    }
                }).Distinct()
                .AsNoTracking().ToListAsync();

            // Postprocessing. Translate data source organizaton names.
            foreach (ProfileEditorSource dataSource in dataSources)
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

            stopwatch.Stop();
            _logger.LogInformation($"GetUniqueDataSources. {dataSources.Count} items in {stopwatch.ElapsedMilliseconds}ms.");

            return dataSources;
        }
    }
}