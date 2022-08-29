using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorItem2
    {
        public ProfileEditorItem2()
        {
        }

        public ProfileEditorItemMeta itemMeta { get; set; }
        public List<ProfileEditorSource> DataSources { get; set; }
    }
}
