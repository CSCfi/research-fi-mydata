namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorItemResearchdataset : ProfileEditorItem
    {
        public ProfileEditorItemResearchdataset()
        {
            LocalIdentifier = "";
            NameFi = "";
            NameSv = "";
            NameEn = "";
        }

        public string LocalIdentifier { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
    }
}
