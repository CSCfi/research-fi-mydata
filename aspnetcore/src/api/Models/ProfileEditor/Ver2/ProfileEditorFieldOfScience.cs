namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorFieldOfScience : ProfileEditorItem2
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
