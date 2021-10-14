namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorSource
    {
        public ProfileEditorSource()
        {
        }

        public int Id { get; set; }
        public string RegisteredDataSource { get; set; }
        public ProfileEditorSourceOrganization Organization { get; set; }
    }
}
