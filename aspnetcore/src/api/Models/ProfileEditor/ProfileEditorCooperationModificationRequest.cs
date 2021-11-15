using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorCooperationModificationRequest    {
        public ProfileEditorCooperationModificationRequest()
        {
            items = new List<ProfileEditorCooperationItem>();
        }

        public List<ProfileEditorCooperationItem> items { get; set; }
    }
}
