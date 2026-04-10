using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.ProfileEditor.Items;

namespace api.Services.Profiledata
{
    public interface IUniqueDataSourcesService
    {
        Task<List<ProfileEditorSource>> GetUniqueDataSources(int userprofileId, bool forElasticsearch = false);
    }
}