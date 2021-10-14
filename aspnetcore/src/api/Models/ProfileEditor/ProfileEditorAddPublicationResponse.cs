using System.Collections.Generic;
namespace api.Models.ProfileEditor

{
    public partial class ProfileEditorAddPublicationResponse    {
        public ProfileEditorAddPublicationResponse()
        {
            source = new ProfileEditorSource();
            publicationsAdded = new List<ProfileEditorItemPublication>();
            publicationsAlreadyInProfile = new List<string>();
            publicationsNotFound = new List<string>();
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemPublication> publicationsAdded { get; set; }
        public List<string> publicationsAlreadyInProfile { get; set; }
        public List<string> publicationsNotFound { get; set; }
    }
}
