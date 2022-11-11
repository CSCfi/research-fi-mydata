using System;
using api.Models.Common;

namespace api.Services
{
    /*
     * UtilityService implements miscellaneous utilities, which simplify code.
     */
    public class UtilityService : IUtilityService
    {
        public UtilityService()
        {
        }

        // Return current timestamp for database entries.
        public DateTime GetCurrentDateTime()
        {
            return DateTime.UtcNow;
        }

        /*
         * Get ORCID data source organization name.
         */
        public string GetDatasourceOrganizationName_ORCID()
        {
            return Constants.OrganizationNames.ORCID;
        }

        /*
         * Get TTV data source organization name.
         */
        public string GetDatasourceOrganizationName_TTV()
        {
            return Constants.OrganizationNames.TTV;
        }

        /*
         * Get OKM organization id.
         */
        public string GetOrganizationId_OKM()
        {
            return Constants.OrganizationIds.OKM;
        }
    }
}