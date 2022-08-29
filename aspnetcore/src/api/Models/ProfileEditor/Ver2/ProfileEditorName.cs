using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorName : ProfileEditorItem2
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
