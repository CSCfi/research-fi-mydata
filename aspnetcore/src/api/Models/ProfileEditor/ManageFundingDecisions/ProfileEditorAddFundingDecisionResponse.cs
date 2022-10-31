using System.Collections.Generic;
namespace api.Models.ProfileEditor.Items

{
    public partial class ProfileEditorAddFundingDecisionResponse    {
        public ProfileEditorAddFundingDecisionResponse()
        {
            source = new ProfileEditorSource();
            fundingDecisionsAdded = new List<int>();
            fundingDecisionsAlreadyInProfile = new List<int>();
            fundingDecisionsNotFound = new List<int>();
        }

        public ProfileEditorSource source { get; set; }
        public List<int> fundingDecisionsAdded { get; set; }
        public List<int> fundingDecisionsAlreadyInProfile { get; set; }
        public List<int> fundingDecisionsNotFound { get; set; }
    }
}
