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
    public class TelephoneNumberService : ITelephoneNumberService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILanguageService _languageService;
        private readonly ILogger<TelephoneNumberService> _logger;


        public TelephoneNumberService(
            TtvContext ttvContext,
            ILanguageService languageService,
            ILogger<TelephoneNumberService> logger)
        {
            _ttvContext = ttvContext;
            _languageService = languageService;
            _logger = logger;
        }

        /*
         * Telephone Numbers
         */
        public async Task<List<ProfileEditorTelephoneNumber>> GetProfileEditorTelephoneNumbers(int userprofileId, bool forElasticsearch = false)
        {
            var stopwatch = Stopwatch.StartNew();
            List<ProfileEditorTelephoneNumber> telephoneNumbers = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimTelephoneNumberId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER
                && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new ProfileEditorTelephoneNumber()
                {
                    Value = ffv.DimTelephoneNumber.TelephoneNumber,
                    itemMeta = new ProfileEditorItemMeta(
                        ffv.DimTelephoneNumberId,
                        Constants.ItemMetaTypes.PERSON_TELEPHONE_NUMBER,
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
            foreach (ProfileEditorTelephoneNumber telephoneNumber in telephoneNumbers)
            {
                foreach (ProfileEditorSource dataSource in telephoneNumber.DataSources)
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
            _logger.LogInformation($"GetProfileEditorTelephoneNumbers. {telephoneNumbers.Count} items in {stopwatch.ElapsedMilliseconds}ms.");

            return telephoneNumbers;
        }
    }
}