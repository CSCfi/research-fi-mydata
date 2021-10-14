using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorGroupAffiliation : ProfileEditorGroup
    {
        public ProfileEditorGroupAffiliation()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemAffiliation> items { get; set; }
    }
}
