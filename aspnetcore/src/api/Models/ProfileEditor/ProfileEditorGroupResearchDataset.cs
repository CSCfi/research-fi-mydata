using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorGroupResearchDataset : ProfileEditorGroup
    {
        public ProfileEditorGroupResearchDataset()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemResearchDataset> items { get; set; }
    }
}
