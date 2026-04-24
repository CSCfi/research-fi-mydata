using System.Threading.Tasks;
using api.Models.ProfileEditor;

namespace api.Services
{
    public interface ISettingsService
    {
       Task<ProfileSettings> GetProfileSettings(int userprofileId, bool forElasticsearch = false);
    }
}