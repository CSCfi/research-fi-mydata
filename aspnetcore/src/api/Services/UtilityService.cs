using System;

namespace api.Services
{
    /*
     * UtilityService implements miscellaneous utilities, which simplify code.
     */
    public class UtilityService
    {
        public UtilityService()
        {
        }

        // Return current timestamp for database entries.
        public DateTime GetCurrentDateTime()
        {
            return DateTime.UtcNow;
        }
    }
}