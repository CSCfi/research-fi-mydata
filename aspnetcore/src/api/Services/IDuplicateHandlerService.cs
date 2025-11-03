using System.Collections.Generic;
using api.Models.ProfileEditor;
using api.Models.ProfileEditor.Items;

namespace api.Services
{
    public interface IDuplicateHandlerService
    {
        List<ProfileEditorSource> AddDataSource(ProfileEditorPublication publication, ProfileEditorSource dataSource);
        List<ProfileEditorPublication> AddPublicationToProfileEditorData(ProfileEditorSource dataSource, ProfileDataFromSql profileData, List<ProfileEditorPublication> publications);
        bool HasSameDoiButIsDifferentPublication(string publicationName, string ttvPublicationName, string ttvPublicationTypeCode);
        bool IsOrcidPublication(ProfileDataFromSql profileData);
        int? HandlePublicationYear(int? dimDateYear);
    }
}