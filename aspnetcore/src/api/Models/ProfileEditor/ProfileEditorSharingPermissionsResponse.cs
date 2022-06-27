using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorSharingPermissionsResponse    {
        public ProfileEditorSharingPermissionsResponse(List<ProfileEditorSharingPermissionItem> items)
        {
            permissions = items;
        }

        public List<ProfileEditorSharingPermissionItem> permissions { get; set; }
    }
}
