using System.Collections.Generic;
using api.Models.ProfileEditor.Items;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorSharingAddPermissionsResponse
    {
        public ProfileEditorSharingAddPermissionsResponse()
        {
            source = new ProfileEditorSource();
            publicationsAdded = new List<ProfileEditorPublication>();
            publicationsAlreadyInProfile = new List<string>();
            publicationsNotFound = new List<string>();
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorPublication> publicationsAdded { get; set; }
        public List<string> publicationsAlreadyInProfile { get; set; }
        public List<string> publicationsNotFound { get; set; }
    }
}
