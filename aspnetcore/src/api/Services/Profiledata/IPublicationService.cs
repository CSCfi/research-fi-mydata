using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.ProfileEditor.Items;

namespace api.Services.Profiledata
{
    public interface IPublicationService
    {
        Task<List<ProfileEditorPublication>> GetProfileEditorPublications(int userprofileId, bool forElasticsearch = false);
    }
}