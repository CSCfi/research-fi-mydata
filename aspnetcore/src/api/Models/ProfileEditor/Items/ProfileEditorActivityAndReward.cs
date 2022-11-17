namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorActivityAndReward : ProfileEditorItem
    {
        public ProfileEditorActivityAndReward()
        {
            NameFi = "";
            NameEn = "";
            NameSv = "";
            DescriptionFi = "";
            DescriptionEn = "";
            DescriptionSv = "";
            InternationalCollaboration = false;
            StartDate = new ProfileEditorDate();
            EndDate = new ProfileEditorDate();
            RoleCode = "";
            RoleNameFi = "";
            RoleNameEn = "";
            RoleNameSv = "";
        }

        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionSv { get; set; }
        public bool InternationalCollaboration { get; set; }
        public ProfileEditorDate StartDate { get; set; }
        public ProfileEditorDate EndDate { get; set; }
        public string RoleCode { get; set; }
        public string RoleNameFi { get; set; }
        public string RoleNameEn { get; set; }
        public string RoleNameSv { get; set; }
    }
}
