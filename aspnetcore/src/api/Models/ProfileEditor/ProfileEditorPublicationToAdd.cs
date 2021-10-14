namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorPublicationToAdd
    {
        public ProfileEditorPublicationToAdd()
        {
        }

        public string PublicationId { get; set; }
        public bool? Show { get; set; }
        // TODO: "PrimaryValue" is used in demo to indicate manually selected publication. Need to add dedicated property in the database model.
        public bool? PrimaryValue { get; set; }
    }
}
