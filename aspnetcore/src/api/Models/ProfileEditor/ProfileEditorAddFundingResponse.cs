using System.Collections.Generic;
namespace api.Models.ProfileEditor

{
    public partial class ProfileEditorAddFundingResponse    {
        public ProfileEditorAddFundingResponse()
        {
            source = new ProfileEditorSource();
            fundingsAdded = new List<ProfileEditorItemFunding>();
            fundingsAlreadyInProfile = new List<int>();
            fundingsNotFound = new List<int>();
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemFunding> fundingsAdded { get; set; }
        public List<int> fundingsAlreadyInProfile { get; set; }
        public List<int> fundingsNotFound { get; set; }
    }
}
