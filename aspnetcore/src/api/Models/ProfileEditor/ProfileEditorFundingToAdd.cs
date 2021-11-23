namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorFundingToAdd
    {
        public ProfileEditorFundingToAdd()
        {
        }

        // TODO: modify according to model
        public int Id { get; set; }
        public bool? Show { get; set; }
        // TODO: "PrimaryValue" is used in demo to indicate manually selected publication. Need to add dedicated property in the database model.
        public bool? PrimaryValue { get; set; }
    }
}
