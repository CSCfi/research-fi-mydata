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

        // Fill empty organization name properties.
        public Organization getOrganization(string nameFi, string nameEn, string nameSv)
        {
            if (nameEn == "") nameEn = nameFi;
            if (nameSv == "") nameSv = nameFi;

            return new Organization()
            {
                NameFi = nameFi,
                NameEn = nameEn,
                NameSv = nameSv
            };
        }
    }
}