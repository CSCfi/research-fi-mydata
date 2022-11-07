namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorEducation : ProfileEditorItem
    {
        public ProfileEditorEducation()
        {
            NameFi = "";
            NameEn = "";
            NameSv = "";
            DegreeGrantingInstitutionName = "";
            StartDate = new ProfileEditorDate();
            EndDate = new ProfileEditorDate();
        }

        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public string DegreeGrantingInstitutionName { get; set; }
        public ProfileEditorDate StartDate { get; set; }
        public ProfileEditorDate EndDate { get; set; }
    }
}
