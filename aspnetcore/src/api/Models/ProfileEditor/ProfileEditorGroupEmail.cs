using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorGroupEmail : ProfileEditorGroup
    {
        public ProfileEditorGroupEmail()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemEmail> items { get; set; }
    }
}
