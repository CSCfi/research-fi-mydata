using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorGroupEducation : ProfileEditorGroup
    {
        public ProfileEditorGroupEducation()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemEducation> items { get; set; }
    }
}
