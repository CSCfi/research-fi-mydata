using api.Models.Common;

namespace api.Services
{
    public interface ILanguageService
    {
        NameTranslation GetNameTranslation(string nameFi, string nameEn, string nameSv);
    }
}