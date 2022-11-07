using System.Collections.Generic;
using api.Models.ProfileEditor.Items;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorAddResearchDatasetResponse    {
        public ProfileEditorAddResearchDatasetResponse()
        {
            source = new ProfileEditorSource();
            researchDatasetAdded = new List<string>();
            researchDatasetAlreadyInProfile = new List<string>();
            researchDatasetNotFound = new List<string>();
        }

        public ProfileEditorSource source { get; set; }
        public List<string> researchDatasetAdded { get; set; }
        public List<string> researchDatasetAlreadyInProfile { get; set; }
        public List<string> researchDatasetNotFound { get; set; }
    }
}
