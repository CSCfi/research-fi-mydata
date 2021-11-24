using System.Collections.Generic;
namespace api.Models.ProfileEditor

{
    public partial class ProfileEditorAddFundingDecisionResponse    {
        public ProfileEditorAddFundingDecisionResponse()
        {
            source = new ProfileEditorSource();
            fundingDecisionsAdded = new List<ProfileEditorItemFundingDecision>();
            fundingDecisionsAlreadyInProfile = new List<string>();
            fundingDecisionsNotFound = new List<string>();
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemFundingDecision> fundingDecisionsAdded { get; set; }
        public List<string> fundingDecisionsAlreadyInProfile { get; set; }
        public List<string> fundingDecisionsNotFound { get; set; }
    }
}
