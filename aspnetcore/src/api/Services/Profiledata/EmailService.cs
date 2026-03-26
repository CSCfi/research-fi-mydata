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
    public class EmailService : IEmailService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILanguageService _languageService;
        private readonly ILogger<EmailService> _logger;


        public EmailService(
            TtvContext ttvContext,
            ILanguageService languageService,
            ILogger<EmailService> logger)
        {
            _ttvContext = ttvContext;
            _languageService = languageService;
            _logger = logger;
        }

        /*
         * Emails
         */
        public async Task<List<ProfileEditorEmail>> GetProfileEditorEmails(int userprofileId, bool forElasticsearch = false)
        {
            List<ProfileEditorEmail> emails = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimEmailAddrressId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS
                && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new ProfileEditorEmail()
                {
                    Value = ffv.DimEmailAddrress.Email,
                    itemMeta = new ProfileEditorItemMeta(
                        ffv.DimEmailAddrressId,
                        Constants.ItemMetaTypes.PERSON_EMAIL_ADDRESS,
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
            foreach (ProfileEditorEmail email in emails)
            {
                foreach (ProfileEditorSource dataSource in email.DataSources)
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

            return emails;
        }
    }
}