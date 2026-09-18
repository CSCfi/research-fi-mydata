using System.Threading.Tasks;
using api.Models.Ai;
using api.Models.ProfileEditor.Items;

namespace api.Services
{
    public interface IBiographyService
    {
        string GetSystemPrompt(string targetLanguage);
        Task<string?> GetProfileDataForPromt(string orcidId);
        Task<Biography> GetBiography(int userprofileId);
        Task<(bool, ProfileEditorItemMeta)> CreateOrUpdateBiography(int userprofileId, Biography biography);
        Task<bool> DeleteBiography(int userprofileId);
        Task<bool> HasEnoughPublishedItems(int userprofileId);
    }
}