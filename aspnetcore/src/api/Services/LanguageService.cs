using api.Models.Common;

namespace api.Services
{
    /*
     * LanguageService implements language and translation related functionality.
     */
    public class LanguageService : ILanguageService
    {
        public LanguageService()
        {
        }

        /*
         * Fill empty name properties.
         * Logic: If only one language field contains value, copy it to all other fields.
         * Order: 1. FI, 2. SV, 3. EN
         */

        public NameTranslation GetNameTranslation(string nameFi, string nameEn, string nameSv)
        {
            // Coalesce nulls to empty strings
            nameFi ??= "";
            nameEn ??= "";
            nameSv ??= "";

            // Count how many fields have values
            int filledCount =
                (nameFi != "" ? 1 : 0) + 
                (nameEn != "" ? 1 : 0) + 
                (nameSv != "" ? 1 : 0);

            // Apply logic based on state
            if (filledCount == 1)
            {
                // Exactly one field filled - copy it to the others
                if (nameFi != "")
                {
                    // Only FI contains value => copy to EN and SV
                    nameEn = nameSv = nameFi;
                }
                else if (nameEn != "")
                {
                    // Only EN contains value => copy to FI and SV
                    nameFi = nameSv = nameEn;
                }
                else // nameSv must be filled
                {
                    // Only SV contains value => copy to FI and EN
                    nameFi = nameEn = nameSv;
                }
            }
            else if (filledCount == 2)
            {
                // Two fields filled - apply priority order (FI > SV > EN)
                if (nameFi == "")
                {
                    // FI is empty, one of EN/SV is filled
                    nameFi = nameSv != "" ? nameSv : nameEn;
                }
                else if (nameEn == "")
                {
                    // EN is empty, FI and SV are filled
                    nameEn = nameFi;
                }
                else // nameSv == ""
                {
                    // SV is empty, FI and EN are filled
                    nameSv = nameFi;
                }
            }
            // If filledCount == 0 or 3, all fields stay as-is

            return new NameTranslation
            {
                NameFi = nameFi,
                NameEn = nameEn,
                NameSv = nameSv
            };
        }
    }
}