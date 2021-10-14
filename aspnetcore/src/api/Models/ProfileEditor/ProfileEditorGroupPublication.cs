using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorGroupPublication : ProfileEditorGroup
    {
        public ProfileEditorGroupPublication()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemPublication> items { get; set; }
    }
}
