namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorAffiliation : ProfileEditorItem
    {
        public ProfileEditorAffiliation()
        {
            OrganizationNameFi = "";
            OrganizationNameSv = "";
            OrganizationNameEn = "";
            DepartmentNameFi = "";
            DepartmentNameSv = "";
            DepartmentNameEn = "";
            PositionNameFi = "";
            PositionNameSv = "";
            PositionNameEn = "";
            Type = "";
            StartDate = new ProfileEditorDate();
            EndDate = new ProfileEditorDate();
        }

        public string OrganizationNameFi { get; set; }
        public string OrganizationNameSv { get; set; }
        public string OrganizationNameEn { get; set; }
        public string DepartmentNameFi { get; set; }
        public string DepartmentNameSv { get; set; }
        public string DepartmentNameEn { get; set; }
        public string PositionNameFi { get; set; }
        public string PositionNameSv { get; set; }
        public string PositionNameEn { get; set; }
        public string Type { get; set; } 
        public ProfileEditorDate StartDate { get; set; }
        public ProfileEditorDate EndDate { get; set; }
    }
}
