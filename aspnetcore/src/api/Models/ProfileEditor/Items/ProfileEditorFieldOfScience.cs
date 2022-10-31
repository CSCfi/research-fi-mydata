namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorFieldOfScience : ProfileEditorItem
    {
        public ProfileEditorFieldOfScience()
        {
            NameFi = "";
            NameEn = "";
            NameSv = "";
        }

        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
    }
}
