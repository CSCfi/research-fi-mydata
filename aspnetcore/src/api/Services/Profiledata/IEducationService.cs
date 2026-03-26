using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.ProfileEditor.Items;

namespace api.Services.Profiledata
{
    public interface IEducationService
    {
        Task<List<ProfileEditorEducation>> GetProfileEditorEducations(int userprofileId, bool forElasticsearch = false);
    }
}