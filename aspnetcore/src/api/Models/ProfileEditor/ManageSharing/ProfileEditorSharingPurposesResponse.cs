using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorSharingPurposesResponse    {
        public ProfileEditorSharingPurposesResponse(List<ProfileEditorSharingPurposeItem> items)
        {
            purposes = items;
        }

        public List<ProfileEditorSharingPurposeItem> purposes { get; set; }
    }
}
