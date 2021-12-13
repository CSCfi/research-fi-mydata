using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorGroupExternalIdentifier : ProfileEditorGroup
    {
        public ProfileEditorGroupExternalIdentifier()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemExternalIdentifier> items { get; set; }
    }
}
