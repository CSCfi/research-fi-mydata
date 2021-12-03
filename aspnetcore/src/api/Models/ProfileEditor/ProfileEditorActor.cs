using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorActor
    {
        public ProfileEditorActor()
        {
        }

        public int actorRole { get; set; }
        public string actorRoleNameFi { get; set; }
        public string actorRoleNameSv { get; set; }
        public string actorRoleNameEn { get; set; }
    }
}
