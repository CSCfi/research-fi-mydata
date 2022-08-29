using System.Collections.Generic;
using api.Models.ProfileEditor;

namespace api.Services
{
    public interface IDuplicateHandlerService
    {
        List<ProfileEditorSource> AddDataSource(ProfileEditorPublication publication, ProfileEditorSource dataSource);
        List<ProfileEditorPublication> AddPublicationToProfileEditorData(ProfileEditorSource dataSource, ProfileDataRaw p, List<ProfileEditorPublication> publications);
        bool HasSameDoiButIsDifferentPublication(string orcidPublicationName, ProfileEditorPublication publication);
        bool IsOrcidPublication(ProfileDataRaw p);
    }
}