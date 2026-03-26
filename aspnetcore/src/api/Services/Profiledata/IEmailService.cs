using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.ProfileEditor.Items;

namespace api.Services.Profiledata
{
    public interface IEmailService
    {
        Task<List<ProfileEditorEmail>> GetProfileEditorEmails(int userprofileId, bool forElasticsearch = false);
    }
}