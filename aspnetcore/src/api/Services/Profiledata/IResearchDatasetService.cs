using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.ProfileEditor.Items;

namespace api.Services.Profiledata
{
    public interface IResearchDatasetService
    {
        Task<List<ProfileEditorResearchDataset>> GetProfileEditorResearchDatasets(int userprofileId, bool forElasticsearch = false);
    }
}