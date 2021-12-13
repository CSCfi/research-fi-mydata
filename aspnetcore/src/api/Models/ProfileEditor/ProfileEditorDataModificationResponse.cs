using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorDataModificationResponse    {
        public ProfileEditorDataModificationResponse()
        {
            groups = new List<ProfileEditorGroupMeta>();
            items = new List<ProfileEditorItemMeta>();
        }

        public List<ProfileEditorGroupMeta> groups { get; set; }
        public List<ProfileEditorItemMeta> items { get; set; }
    }
}
