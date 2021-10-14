using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorGroupKeyword : ProfileEditorGroup
    {
        public ProfileEditorGroupKeyword()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemKeyword> items { get; set; }
    }
}
