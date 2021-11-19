using api.Models.Common;

namespace api.Services
{
    /*
     * LanguageService implements language and translation related functionality.
     */
    public class LanguageService
    {
        public LanguageService()
        {
        }

        /*
         * Fill empty name properties.
         * Logic: If only one language field contains value, copy it to all other fields.
         */

        public NameTranslation getNameTranslation(string nameFi, string nameEn, string nameSv)
        {
            // Only FI contains value => copy to EN and SV
            if (
                (nameFi != null && nameFi != "") &&
                (nameEn == null || nameEn == "") &&
                (nameSv == null || nameSv == "")
            )
            {
                nameEn = nameFi;
                nameSv = nameFi;
            }

            // Only EN contains value => copy to FI and SV
            if (
                (nameEn != null && nameEn != "") &&
                (nameFi == null || nameFi == "") &&
                (nameSv == null || nameSv == "")
            )
            {
                nameFi = nameEn;
                nameSv = nameEn;
            }

            // Only SV contains value => copy to FI and EN
            if (
                (nameSv != null && nameSv != "") &&
                (nameFi == null || nameFi == "") &&
                (nameEn == null || nameEn == "")
            )
            {
                nameFi = nameSv;
                nameEn = nameSv;
            }

            return new NameTranslation()
            {
                NameFi = nameFi,
                NameEn = nameEn,
                NameSv = nameSv
            };
        }
    }
}