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
    public class ResearcherDescriptionService : IResearcherDescriptionService
    {
        private readonly TtvContext _ttvContext;
        private readonly IDataSourceHelperService _dataSourceHelperService;
        private readonly ILanguageService _languageService;
        private readonly ILogger<ResearcherDescriptionService> _logger;


        public ResearcherDescriptionService(
            TtvContext ttvContext,
            IDataSourceHelperService dataSourceHelperService,
            ILanguageService languageService,
            ILogger<ResearcherDescriptionService> logger)
        {
            _ttvContext = ttvContext;
            _dataSourceHelperService = dataSourceHelperService;
            _languageService = languageService;
            _logger = logger;
        }

        /*
         * Researcher descriptions
         */
        public async Task<List<ProfileEditorResearcherDescription>> GetProfileEditorResearcherDescriptions(int userprofileId, bool forElasticsearch = false)
        {
            var stopwatch = Stopwatch.StartNew();
            List<ProfileEditorResearcherDescription> researcherDescriptions = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimResearcherDescriptionId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION
                && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new ProfileEditorResearcherDescription()
                {
                    ResearchDescriptionFi = ffv.DimResearcherDescription.ResearchDescriptionFi,
                    ResearchDescriptionEn = ffv.DimResearcherDescription.ResearchDescriptionEn,
                    ResearchDescriptionSv = ffv.DimResearcherDescription.ResearchDescriptionSv,
                    itemMeta = new ProfileEditorItemMeta(
                        ffv.DimResearcherDescriptionId,
                        Constants.ItemMetaTypes.PERSON_RESEARCHER_DESCRIPTION,
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

            /*
             * Postprocessing. Translate data source organizaton names and researcher description names.
             * Researcher description name translation must be skipped when data source is tiedejatutkimus.fi.
             * That indicates user generated value (from AI assisted researcher description feature), whose translation is handled in the frontend.
             */
            foreach (ProfileEditorResearcherDescription researcherDescription in researcherDescriptions)
            {
                // Translate data source organization names.
                foreach (ProfileEditorSource dataSource in researcherDescription.DataSources)
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

                // Translate researcher description names if data source is not tiedejatutkimus.fi.
                if (researcherDescription.DataSources[0].RegisteredDataSource != _dataSourceHelperService.DimRegisteredDataSourceName_TTV)
                {
                    NameTranslation nameTranslationResearcherDescription = _languageService.GetNameTranslation(
                        nameFi: researcherDescription.ResearchDescriptionFi,
                        nameEn: researcherDescription.ResearchDescriptionEn,
                        nameSv: researcherDescription.ResearchDescriptionSv
                    );
                    researcherDescription.ResearchDescriptionFi = nameTranslationResearcherDescription.NameFi;
                    researcherDescription.ResearchDescriptionEn = nameTranslationResearcherDescription.NameEn;
                    researcherDescription.ResearchDescriptionSv = nameTranslationResearcherDescription.NameSv;
                }
            }

            stopwatch.Stop();
            _logger.LogInformation($"GetProfileEditorResearcherDescriptions. {researcherDescriptions.Count} items in {stopwatch.ElapsedMilliseconds}ms.");
            
            return researcherDescriptions;
        }
    }
}