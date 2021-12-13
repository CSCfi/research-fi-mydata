using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorGroupTelephoneNumber : ProfileEditorGroup
    {
        public ProfileEditorGroupTelephoneNumber()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemTelephoneNumber> items { get; set; }
    }
}
