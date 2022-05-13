using api.Models;
using api.Models.Ttv;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    /*
     * RegisteredDataSourceService handles DimRegisteredDataSource related actions.
     */
    public class RegisteredDataSourceService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILogger<RegisteredDataSourceService> _logger;

        public RegisteredDataSourceService(TtvContext ttvContext, ILogger<RegisteredDataSourceService> logger)
        {
            _ttvContext = ttvContext;
            _logger = logger;
        }

        /*
         * Check that required entries exist in DimRegisteredDatasource.
         * This application requires registered data sources for ORCID and Tutkimustietovaranto.
         * 
         * Note! This method is synchronous, because it should be run in the application startup phase.
         */
        public bool CheckRequiredRegisteredDataSourcesExistOnStartup()
        {
            _logger.LogInformation("Checking required entries in DimRegisteredDataSource");

            bool orcidRegisteredDataSourceExists = false;
            bool ttvRegisteredDataSourceExists = false;

            // Get data sources and related organizations.
            var registeredDataSources = _ttvContext.DimRegisteredDataSources
                .Include(drds => drds.DimOrganization)
                .Where(drds => drds.DimOrganization.NameFi == Constants.OrganizationNames.ORCID || drds.DimOrganization.NameFi == Constants.OrganizationNames.TTV).AsNoTracking().ToList();

            // Check required data sources are found.
            foreach (DimRegisteredDataSource drds in registeredDataSources)
            {
                if (drds.DimOrganization.NameFi == Constants.OrganizationNames.ORCID) orcidRegisteredDataSourceExists = true;
                if (drds.DimOrganization.NameFi == Constants.OrganizationNames.TTV) ttvRegisteredDataSourceExists = true;

            }

            // Log error on missing ORCID registered data source
            if (!orcidRegisteredDataSourceExists)
            {
                string errorMessage = "Registered data source was not found from dim_registered_data_source for organization: " + Constants.OrganizationNames.ORCID;
                _logger.LogError(errorMessage);
                throw new System.Exception(errorMessage);
            }

            // Log error on missing Tutkimustietovaranto registered data source
            if (!ttvRegisteredDataSourceExists)
            {
                string errorMessage = "Registered data source was not found from table dim_registered_data_source for organization: " + Constants.OrganizationNames.TTV;
                _logger.LogError(errorMessage);
                throw new System.Exception(errorMessage);
            }

            return orcidRegisteredDataSourceExists && ttvRegisteredDataSourceExists;
        }
    }
}