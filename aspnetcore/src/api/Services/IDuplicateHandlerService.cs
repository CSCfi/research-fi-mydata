using System.Collections.Generic;
using api.Models.ProfileEditor;
using api.Models.Ttv;

namespace api.Services
{
    public interface IDuplicateHandlerService
    {
        List<ProfileEditorSource> AddDataSource(ProfileEditorPublicationExperimental publication, ProfileEditorSource dataSource);
        List<ProfileEditorPublicationExperimental> AddPublicationToProfileEditorData(ProfileEditorSource dataSource, FactFieldValue ffv, List<ProfileEditorPublicationExperimental> publications);
        bool HasSameDoiButIsDifferentPublication(DimOrcidPublication orcidPublication, ProfileEditorPublicationExperimental publication);
        bool IsOrcidPublication(FactFieldValue ffv);
    }
}