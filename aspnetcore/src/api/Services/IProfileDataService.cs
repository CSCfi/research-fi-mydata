using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.ProfileEditor.Items;

namespace api.Services
{
    public interface IProfileDataService
    {
        Task<List<ProfileEditorName>> GetProfileEditorNames(int userprofileId);
        Task<List<ProfileEditorName>> GetProfileEditorOtherNames(int userprofileId);
    }
}