namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorSharingPurposeItem
    {
        public ProfileEditorSharingPurposeItem()
        {
            PurposeId = -1;
            NameFi = "";
            NameEn = "";
            NameSv = "";
            DescriptionFi = "";
            DescriptionEn = "";
            DescriptionSv = "";
        }

        public int PurposeId { get; set; }
        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionSv { get; set; }
    }
}
