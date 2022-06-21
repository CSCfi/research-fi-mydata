using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorSharingGivenPermissionsResponse    {
        public ProfileEditorSharingGivenPermissionsResponse(List<ProfileEditorSharingItem> items)
        {
            givenPermissions = items;
        }

        public List<ProfileEditorSharingItem> givenPermissions { get; set; }
    }
}
