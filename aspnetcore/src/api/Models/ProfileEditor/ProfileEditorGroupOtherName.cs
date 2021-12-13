using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorGroupOtherName : ProfileEditorGroup
    {
        public ProfileEditorGroupOtherName()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemName> items { get; set; }
    }
}
