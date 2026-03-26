using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.ProfileEditor.Items;

namespace api.Services.Profiledata
{
    public interface INameService
    {
        Task<List<ProfileEditorName>> GetProfileEditorNames(int userprofileId, bool forElasticsearch = false);
        Task<List<ProfileEditorName>> GetProfileEditorOtherNames(int userprofileId, bool forElasticsearch = false);
    }
}