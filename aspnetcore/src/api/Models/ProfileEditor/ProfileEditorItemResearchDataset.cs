namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorItemResearchDataset : ProfileEditorItem
    {
        public ProfileEditorItemResearchDataset()
        {
            LocalIdentifier = "";
            NameFi = "";
            NameSv = "";
            NameEn = "";
        }

        // TODO: Add properties according to DimResearchDataset
        public string LocalIdentifier { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
    }
}
