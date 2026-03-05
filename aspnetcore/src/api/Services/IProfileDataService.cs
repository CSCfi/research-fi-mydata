using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.ProfileEditor.Items;

namespace api.Services
{
    public interface IProfileDataService
    {
        Task<List<ProfileEditorName>> GetProfileEditorNames(int userprofileId);
        Task<List<ProfileEditorName>> GetProfileEditorOtherNames(int userprofileId);
        Task<List<ProfileEditorEmail>> GetProfileEditorEmails(int userprofileId);
        Task<List<ProfileEditorTelephoneNumber>> GetProfileEditorTelephoneNumbers(int userprofileId);
        Task<List<ProfileEditorWebLink>> GetProfileEditorWebLinks(int userprofileId);
        Task<List<ProfileEditorKeyword>> GetProfileEditorKeywords(int userprofileId);
        Task<List<ProfileEditorResearcherDescription>> GetProfileEditorResearcherDescriptions(int userprofileId);
        Task<List<ProfileEditorExternalIdentifier>> GetProfileEditorExternalIdentifiers(int userprofileId);
        Task<List<ProfileEditorEducation>> GetProfileEditorEducations(int userprofileId);
    }
}