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
    public class WebLinkService : IWebLinkService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILanguageService _languageService;
        private readonly ILogger<WebLinkService> _logger;


        public WebLinkService(
            TtvContext ttvContext,
            ILanguageService languageService,
            ILogger<WebLinkService> logger)
        {
            _ttvContext = ttvContext;
            _languageService = languageService;
            _logger = logger;
        }

        /*
         * Web links
         */
        public async Task<List<ProfileEditorWebLink>> GetProfileEditorWebLinks(int userprofileId, bool forElasticsearch = false)
        {
            List<ProfileEditorWebLink> webLinks = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimWebLinkId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.PERSON_WEB_LINK
                && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new ProfileEditorWebLink()
                {
                    Url = ffv.DimWebLink.Url,
                    LinkLabel = ffv.DimWebLink.LinkLabel,
                    LinkType = ffv.DimWebLink.LinkType,
                    itemMeta = new ProfileEditorItemMeta(
                        ffv.DimWebLinkId,
                        Constants.ItemMetaTypes.PERSON_WEB_LINK,
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
            foreach (ProfileEditorWebLink webLink in webLinks)
            {
                foreach (ProfileEditorSource dataSource in webLink.DataSources)
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

            return webLinks;
        }
    }
}