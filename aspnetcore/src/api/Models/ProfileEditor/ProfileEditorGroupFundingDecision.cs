using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorGroupFundingDecision : ProfileEditorGroup
    {
        public ProfileEditorGroupFundingDecision()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemFundingDecision> items { get; set; }
    }
}
