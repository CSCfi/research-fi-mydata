using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorGroupWebLink : ProfileEditorGroup
    {
        public ProfileEditorGroupWebLink()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemWebLink> items { get; set; }
    }
}
