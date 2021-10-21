using System.Collections.Generic;
namespace api.Models.ProfileEditor

{
    public partial class ProfileEditorRemovePublicationResponse    {
        public ProfileEditorRemovePublicationResponse()
        {
            publicationsRemoved = new List<string>();
            publicationsNotFound = new List<string>();
        }

        public List<string> publicationsRemoved { get; set; }
        public List<string> publicationsNotFound { get; set; }
    }
}
