namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorDataResponse2    {
        public ProfileEditorDataResponse2()
        {
            personal = new ProfileEditorDataPersonal2();
            activity = new ProfileEditorDataActivity2();
        }

        public ProfileEditorDataPersonal2 personal { get; set; }
        public ProfileEditorDataActivity2 activity { get; set; }
    }
}
