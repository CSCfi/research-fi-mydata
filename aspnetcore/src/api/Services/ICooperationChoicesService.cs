using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.ProfileEditor;

namespace api.Services
{
    public interface ICooperationChoicesService
    {
       Task<List<ProfileEditorCooperationItem>> GetCooperationChoices(int userprofileId, bool forElasticsearch = false);
    }
}