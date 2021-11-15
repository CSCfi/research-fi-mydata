using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorCooperationModificationResponse
    {
        public ProfileEditorCooperationModificationResponse()
        {
            items = new List<ProfileEditorCooperationItem>();
        }

        public List<ProfileEditorCooperationItem> items { get; set; }
    }
}
