namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorFundingDecisionToAdd
    {
        public ProfileEditorFundingDecisionToAdd()
        {
        }

        public int ProjectId { get; set; }
        public bool? Show { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}
