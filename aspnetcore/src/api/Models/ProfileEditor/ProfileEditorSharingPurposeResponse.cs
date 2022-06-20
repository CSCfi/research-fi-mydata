using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorSharingPurposeResponse    {
        public ProfileEditorSharingPurposeResponse(List<ProfileEditorSharingPurposeItem> items)
        {
            purposes = items;
        }

        public List<ProfileEditorSharingPurposeItem> purposes { get; set; }
    }
}
