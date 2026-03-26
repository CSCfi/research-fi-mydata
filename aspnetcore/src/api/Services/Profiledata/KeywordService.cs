using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using api.Models.Common;
using api.Models.ProfileEditor.Items;

namespace api.Services.Profiledata
{
    public class KeywordService : IKeywordService
    {
        private readonly TtvContext _ttvContext;
        private readonly IDataSourceHelperService _dataSourceHelperService;
        private readonly ILanguageService _languageService;
        private readonly ILogger<KeywordService> _logger;


        public KeywordService(
            TtvContext ttvContext,
            IDataSourceHelperService dataSourceHelperService,
            ILanguageService languageService,
            ILogger<KeywordService> logger)
        {
            _ttvContext = ttvContext;
            _dataSourceHelperService = dataSourceHelperService;
            _languageService = languageService;
            _logger = logger;
        }


        /*
         * Keywords
         */
        public async Task<List<ProfileEditorKeyword>> GetProfileEditorKeywords(int userprofileId, bool forElasticsearch = false)
        {
            List<ProfileEditorKeyword> keywords = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimKeywordId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.PERSON_KEYWORD
                && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new ProfileEditorKeyword()
                {
                    Value = ffv.DimKeyword.Keyword,
                    itemMeta = new ProfileEditorItemMeta(
                        ffv.DimKeywordId,
                        Constants.ItemMetaTypes.PERSON_KEYWORD,
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
            foreach (ProfileEditorKeyword keyword in keywords)
            {
                foreach (ProfileEditorSource dataSource in keyword.DataSources)
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

            return keywords;
        }
    }
}