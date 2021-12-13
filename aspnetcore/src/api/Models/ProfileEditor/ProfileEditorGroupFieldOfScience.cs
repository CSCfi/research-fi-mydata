using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorGroupFieldOfScience : ProfileEditorGroup
    {
        public ProfileEditorGroupFieldOfScience()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemFieldOfScience> items { get; set; }
    }
}
