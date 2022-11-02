using System.Collections.Generic;

namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorName : ProfileEditorItem
    {
        public ProfileEditorName()
        {
            FirstNames = "";
            LastName = "";
            FullName = "";
        }

        public string FirstNames { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
    }
}
