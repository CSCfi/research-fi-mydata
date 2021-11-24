using System.Collections.Generic;
namespace api.Models.ProfileEditor

{
    public partial class ProfileEditorRemoveFundingDecisionResponse    {
        public ProfileEditorRemoveFundingDecisionResponse()
        {
            fundingDecisionsRemoved = new List<string>();
            fundingDecisionsNotFound = new List<string>();
        }

        public List<string> fundingDecisionsRemoved { get; set; }
        public List<string> fundingDecisionsNotFound { get; set; }
    }
}
