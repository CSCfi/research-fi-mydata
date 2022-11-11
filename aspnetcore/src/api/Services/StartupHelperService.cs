using api.Models.Common;
using api.Models.Ttv;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    /*
     * StartupHelperService contains methods using application startup.
     */
    public class StartupHelperService : IStartupHelperService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILogger<StartupHelperService> _logger;
        private readonly IUtilityService _utilityService;

        public StartupHelperService(TtvContext ttvContext, ILogger<StartupHelperService> logger, IUtilityService utilityService)
        {
            _ttvContext = ttvContext;
            _logger = logger;
            _utilityService = utilityService;
        }

        /*
         * Get ORCID registered data source and related organization.
         * 
         * Note! This method is synchronous, because it should be run in the application startup phase.
         */
        public DimRegisteredDataSource GetDimRegisteredDataSourceId_OnStartup_ORCID()
        {
            _logger.LogInformation("Get data source for ORCID in DimRegisteredDataSource");
            // Get data source organization name.
            string organizationName_ORCID = _utilityService.GetDatasourceOrganizationName_ORCID();
            // Get data source and related organization.
            DimRegisteredDataSource orcidRegisteredDataSource = _ttvContext.DimRegisteredDataSources
                .Include(drds => drds.DimOrganization)
                .Where(drds => drds.DimOrganization.NameFi == organizationName_ORCID).AsNoTracking().FirstOrDefault();

            // Log error and raise exception on missing ORCID registered data source. The application does not function without this.
            if (orcidRegisteredDataSource == null)
            {
                string errorMessage = "Registered data source was not found from dim_registered_data_source for organization: " + organizationName_ORCID;
                _logger.LogError(errorMessage);
                throw new System.Exception(errorMessage);
            }

            return orcidRegisteredDataSource;
        }

        /*
         * Get TTV registered data source and related organization.
         * 
         * Note! This method is synchronous, because it should be run in the application startup phase.
         */
        public DimRegisteredDataSource GetDimRegisteredDataSourceId_OnStartup_TTV()
        {
            _logger.LogInformation("Get data source for TTV in DimRegisteredDataSource");
            // Get data source organization name.
            string organizationName_TTV = _utilityService.GetDatasourceOrganizationName_TTV();
            // Get data source and related organization.
            DimRegisteredDataSource ttvRegisteredDataSource = _ttvContext.DimRegisteredDataSources
                .Include(drds => drds.DimOrganization)
                .Where(drds => drds.DimOrganization.NameFi == organizationName_TTV).AsNoTracking().FirstOrDefault();

            // Log error and raise exception on missing TTV registered data source. The application does not function without this.
            if (ttvRegisteredDataSource == null)
            {
                string errorMessage = "Registered data source was not found from dim_registered_data_source for organization: " + organizationName_TTV;
                _logger.LogError(errorMessage);
                throw new System.Exception(errorMessage);
            }

            return ttvRegisteredDataSource;
        }

        /*
         * Get TTV purpose.
         * 
         * Note! This method is synchronous, because it should be run in the application startup phase.
         */
        public DimPurpose GetDimPurposeId_OnStartup_TTV()
        {
            // Get organization ID.
            string organizationId_OKM = _utilityService.GetOrganizationId_OKM();

            DimPurpose dimPurpose = _ttvContext.DimPurposes
                .Include(dp => dp.DimOrganization)
                .Where(dp => dp.DimOrganization.OrganizationId == organizationId_OKM).AsNoTracking().FirstOrDefault();

            // Log error and raise exception on missing TTV purpose. The application does not function without this.
            if (dimPurpose == null)
            {
                string errorMessage = "Purpose was not found from dim_purpose for organization_id: " + organizationId_OKM;
                _logger.LogError(errorMessage);
                throw new System.Exception(errorMessage);
            }

            return dimPurpose;
        }
    }
}