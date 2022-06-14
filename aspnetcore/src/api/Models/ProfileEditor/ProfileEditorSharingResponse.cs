using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorSharingResponse    {
        public ProfileEditorSharingResponse(List<ProfileEditorSharingItem> items)
        {
            profileEditorSharingItems = items;
        }

        public List<ProfileEditorSharingItem> profileEditorSharingItems { get; set; }
    }
}
