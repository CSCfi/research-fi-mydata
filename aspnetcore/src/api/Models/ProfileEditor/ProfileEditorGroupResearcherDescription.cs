using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorGroupResearcherDescription : ProfileEditorGroup
    {
        public ProfileEditorGroupResearcherDescription()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemResearcherDescription> items { get; set; }
    }
}
