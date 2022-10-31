using System.Collections.Generic;

namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorItem
    {
        public ProfileEditorItem()
        {
        }

        public ProfileEditorItemMeta itemMeta { get; set; }
        public List<ProfileEditorSource> DataSources { get; set; }
    }
}
