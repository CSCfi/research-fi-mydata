namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorItemFieldOfScience : ProfileEditorItem
    {
        public ProfileEditorItemFieldOfScience()
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
