using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorSharingItem
    {
        public ProfileEditorSharingItem()
        {
        }

        public ProfileEditorSharingPurposeItem Purpose { get; set; }
        public List<ProfileEditorSharingPermissionItem> Permissions { get; set; }
    }
}
