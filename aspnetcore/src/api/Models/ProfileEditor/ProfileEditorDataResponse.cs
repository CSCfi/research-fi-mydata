namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorDataResponse    {
        public ProfileEditorDataResponse()
        {
            personal = new ProfileEditorDataPersonal();
            activity = new ProfileEditorDataActivity();
        }

        public ProfileEditorDataPersonal personal { get; set; }
        public ProfileEditorDataActivity activity { get; set; }
    }
}
