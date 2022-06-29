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
         */

        public NameTranslation GetNameTranslation(string nameFi, string nameEn, string nameSv)
        {
            // Convert null to ""
            if (nameFi == null)
            {
                nameFi = "";
            }

            if (nameEn == null)
            {
                nameEn = "";
            }

            if (nameSv == null)
            {
                nameSv = "";
            }

            // Only FI contains value => copy to EN and SV
            if (nameFi != "" && nameEn == "" && nameSv == "")
            {
                nameEn = nameFi;
                nameSv = nameFi;
            }

            // Only EN contains value => copy to FI and SV
            if (nameEn != "" && nameFi == "" && nameSv == "")
            {
                nameFi = nameEn;
                nameSv = nameEn;
            }

            // Only SV contains value => copy to FI and EN
            if (nameSv != "" && nameFi == "" && nameEn == "")
            {
                nameFi = nameSv;
                nameEn = nameSv;
            }

            // FI and EN contain values => copy to FI to SV
            if (nameFi != "" && nameEn != "" && nameSv == "")
            {
                nameSv = nameFi;
            }

            // FI and SV contain values => copy to FI to EN
            if (nameFi != "" && nameSv != "" && nameEn == "")
            {
                nameEn = nameFi;
            }

            // EN and SV contain values => copy to SV to FI
            if (nameFi == "" && nameSv != "" && nameEn != "")
            {
                nameFi = nameSv;
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