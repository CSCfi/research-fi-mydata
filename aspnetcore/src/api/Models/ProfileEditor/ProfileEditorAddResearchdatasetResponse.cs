using System.Collections.Generic;
namespace api.Models.ProfileEditor

{
    public partial class ProfileEditorAddResearchdatasetResponse    {
        public ProfileEditorAddResearchdatasetResponse()
        {
            source = new ProfileEditorSource();
            researchdatasetAdded = new List<ProfileEditorItemResearchdataset>();
            researchdatasetAlreadyInProfile = new List<string>();
            researchdatasetNotFound = new List<string>();
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemResearchdataset> researchdatasetAdded { get; set; }
        public List<string> researchdatasetAlreadyInProfile { get; set; }
        public List<string> researchdatasetNotFound { get; set; }
    }
}
