using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorDataActivity2
    {
        public ProfileEditorDataActivity2()
        {
            affiliations = new List<ProfileEditorAffiliation>();
            educations = new List<ProfileEditorEducation>();
            publications = new List<ProfileEditorPublication>();
            fundingDecisions = new List<ProfileEditorFundingDecision>();
            researchDatasets = new List<ProfileEditorResearchDataset>();
        }

        public List<ProfileEditorAffiliation> affiliations { get; set; }
        public List<ProfileEditorEducation> educations { get; set; }
        public List<ProfileEditorPublication> publications { get; set; }
        public List<ProfileEditorFundingDecision> fundingDecisions { get; set; }
        public List<ProfileEditorResearchDataset> researchDatasets { get; set; }
    }
}
