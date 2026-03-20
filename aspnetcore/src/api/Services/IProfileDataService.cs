using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.ProfileEditor.Items;

namespace api.Services
{
    public interface IProfileDataService
    {
        Task<List<ProfileEditorName>> GetProfileEditorNames(int userprofileId, bool forElasticsearch = false);
        Task<List<ProfileEditorName>> GetProfileEditorOtherNames(int userprofileId, bool forElasticsearch = false);
        Task<List<ProfileEditorEmail>> GetProfileEditorEmails(int userprofileId, bool forElasticsearch = false);
        Task<List<ProfileEditorTelephoneNumber>> GetProfileEditorTelephoneNumbers(int userprofileId, bool forElasticsearch = false);
        Task<List<ProfileEditorWebLink>> GetProfileEditorWebLinks(int userprofileId, bool forElasticsearch = false);
        Task<List<ProfileEditorKeyword>> GetProfileEditorKeywords(int userprofileId, bool forElasticsearch = false);
        Task<List<ProfileEditorResearcherDescription>> GetProfileEditorResearcherDescriptions(int userprofileId, bool forElasticsearch = false);
        Task<List<ProfileEditorExternalIdentifier>> GetProfileEditorExternalIdentifiers(int userprofileId, bool forElasticsearch = false);
        Task<List<ProfileEditorEducation>> GetProfileEditorEducations(int userprofileId, bool forElasticsearch = false);
        Task<List<ProfileEditorAffiliation>> GetProfileEditorAffiliations(int userprofileId, bool forElasticsearch = false);
    }
}