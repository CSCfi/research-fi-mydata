using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.ProfileEditor.Items;

namespace api.Services.Profiledata
{
    public interface IResearchActivityService
    {
        Task<List<ProfileEditorActivityAndReward>> GetProfileEditorActiviesAndRewards(int userprofileId, bool forElasticsearch = false);
    }
}