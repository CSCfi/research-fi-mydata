using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.ProfileEditor.Items;

namespace api.Services.Profiledata
{
    public interface IKeywordService
    {
        Task<List<ProfileEditorKeyword>> GetProfileEditorKeywords(int userprofileId, bool forElasticsearch = false);
    }
}