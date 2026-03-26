using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.ProfileEditor.Items;

namespace api.Services.Profiledata
{
    public interface IResearcherDescriptionService
    {
        Task<List<ProfileEditorResearcherDescription>> GetProfileEditorResearcherDescriptions(int userprofileId, bool forElasticsearch = false);
    }
}