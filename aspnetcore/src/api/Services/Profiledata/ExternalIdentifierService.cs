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
    public class ExternalIdentifierService : IExternalIdentifierService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILanguageService _languageService;
        private readonly ILogger<ExternalIdentifierService> _logger;


        public ExternalIdentifierService(
            TtvContext ttvContext,
            ILanguageService languageService,
            ILogger<ExternalIdentifierService> logger)
        {
            _ttvContext = ttvContext;
            _languageService = languageService;
            _logger = logger;
        }

        /*
         * External identifiers
         */
         public async Task<List<ProfileEditorExternalIdentifier>> GetProfileEditorExternalIdentifiers(int userprofileId, bool forElasticsearch = false)
         {
             List<ProfileEditorExternalIdentifier> externalIdentifiers = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimPidId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER
                && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new ProfileEditorExternalIdentifier()
                {
                    PidContent = ffv.DimPid.PidContent,
                    PidType = ffv.DimPid.PidType,
                    itemMeta = new ProfileEditorItemMeta(
                        ffv.DimPidId,
                        Constants.ItemMetaTypes.PERSON_EXTERNAL_IDENTIFIER,
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
            foreach (ProfileEditorExternalIdentifier externalIdentifier in externalIdentifiers)
            {
                foreach (ProfileEditorSource dataSource in externalIdentifier.DataSources)
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

            return externalIdentifiers;
        }
    }
}