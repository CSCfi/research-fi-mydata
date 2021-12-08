using System.Collections.Generic;
namespace api.Models.ProfileEditor

{
    public partial class ProfileEditorRemoveFundingDecisionResponse    {
        public ProfileEditorRemoveFundingDecisionResponse()
        {
            fundingDecisionsRemoved = new List<int>();
            fundingDecisionsNotFound = new List<int>();
        }

        public List<int> fundingDecisionsRemoved { get; set; }
        public List<int> fundingDecisionsNotFound { get; set; }
    }
}
