using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorDataActivity2
    {
        public ProfileEditorDataActivity2()
        {
            affiliationGroups = new List<ProfileEditorGroupAffiliation>();
            educationGroups = new List<ProfileEditorGroupEducation>();
            publications = new List<ProfileEditorPublication>();
            fundingDecisionGroups = new List<ProfileEditorGroupFundingDecision>();
            researchDatasetGroups = new List<ProfileEditorGroupResearchDataset>();
        }

        public List<ProfileEditorGroupAffiliation> affiliationGroups { get; set; }
        public List<ProfileEditorGroupEducation> educationGroups { get; set; }
        public List<ProfileEditorPublication> publications { get; set; }
        public List<ProfileEditorGroupFundingDecision> fundingDecisionGroups { get; set; }
        public List<ProfileEditorGroupResearchDataset> researchDatasetGroups { get; set; }
    }
}
