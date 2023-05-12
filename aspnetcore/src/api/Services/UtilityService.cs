using System;
using System.Globalization;
using api.Models.Common;
using Nest;

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

        /*
         * Convert string to decimal?
         */
        public decimal? StringToNullableDecimal(string s)
        {
            if (Decimal.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out decimal result))
            {
                return result;
            }
            return null;
        }
    }
}