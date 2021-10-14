using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorGroupName : ProfileEditorGroup
    {
        public ProfileEditorGroupName()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemName> items { get; set; }
    }
}
