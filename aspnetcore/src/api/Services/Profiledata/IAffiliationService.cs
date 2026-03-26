using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.ProfileEditor.Items;

namespace api.Services.Profiledata
{
    public interface IAffiliationService
    {
        Task<List<ProfileEditorAffiliation>> GetProfileEditorAffiliations(int userprofileId, bool forElasticsearch = false);
    }
}