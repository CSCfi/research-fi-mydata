using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.ProfileEditor.Items;

namespace api.Services.Profiledata
{
    public interface IFundingDecisionService
    {
        Task<List<ProfileEditorFundingDecision>> GetProfileEditorFundingDecisions(int userprofileId, bool forElasticsearch = false);
    }
}