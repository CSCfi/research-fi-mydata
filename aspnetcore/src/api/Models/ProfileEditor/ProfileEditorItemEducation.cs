namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorItemEducation : ProfileEditorItem
    {
        public ProfileEditorItemEducation()
        {
            NameFi = "";
            NameEn = "";
            NameSv = "";
            DegreeGrantingInstitutionName = "";
            StartDate = new ProfileEditorItemDate();
            EndDate = new ProfileEditorItemDate();
        }

        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public string DegreeGrantingInstitutionName { get; set; }
        public ProfileEditorItemDate StartDate { get; set; }
        public ProfileEditorItemDate EndDate { get; set; }
    }
}
