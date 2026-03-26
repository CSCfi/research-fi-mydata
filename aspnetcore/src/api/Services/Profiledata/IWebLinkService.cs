using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.ProfileEditor.Items;

namespace api.Services.Profiledata
{
    public interface IWebLinkService
    {
        Task<List<ProfileEditorWebLink>> GetProfileEditorWebLinks(int userprofileId, bool forElasticsearch = false);
    }
}