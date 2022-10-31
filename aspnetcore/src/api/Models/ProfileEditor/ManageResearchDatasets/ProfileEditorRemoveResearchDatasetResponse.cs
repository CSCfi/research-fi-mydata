using System.Collections.Generic;
namespace api.Models.ProfileEditor

{
    public partial class ProfileEditorRemoveResearchDatasetResponse    {
        public ProfileEditorRemoveResearchDatasetResponse()
        {
            researchDatasetsRemoved = new List<string>();
            researchDatasetsNotFound = new List<string>();
        }

        public List<string> researchDatasetsRemoved { get; set; }
        public List<string> researchDatasetsNotFound { get; set; }
    }
}
