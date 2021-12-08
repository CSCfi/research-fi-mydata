namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorItemFundingDecision : ProfileEditorItem
    {
        public ProfileEditorItemFundingDecision()
        {
            ProjectAcronym = "";
            ProjectNameFi = "";
            ProjectNameSv = "";
            ProjectNameEn = "";
            ProjectDescriptionFi = "";
            ProjectDescriptionSv = "";
            ProjectDescriptionEn = "";
            FunderNameFi = "";
            FunderNameSv = "";
            FunderNameEn = "";
            FunderProjectNumber = "";
            TypeOfFundingNameFi = "";
            TypeOfFundingNameSv = "";
            TypeOfFundingNameEn = "";
            CallProgrammeNameFi = "";
            CallProgrammeNameSv = "";
            CallProgrammeNameEn = "";
            StartDate = new ProfileEditorItemDate();
            EndDate = new ProfileEditorItemDate();
            AmountInEur = -1;
        }

        public int ProjectId { get; set; }
        public string ProjectAcronym { get; set; }
        public string ProjectNameFi { get; set; }
        public string ProjectNameSv { get; set; }
        public string ProjectNameEn { get; set; }
        public string ProjectDescriptionFi { get; set; }
        public string ProjectDescriptionSv { get; set; }
        public string ProjectDescriptionEn { get; set; }
        public string FunderNameFi { get; set; }
        public string FunderNameSv { get; set; }
        public string FunderNameEn { get; set; }
        public string FunderProjectNumber { get; set; }
        public string TypeOfFundingNameFi { get; set; }
        public string TypeOfFundingNameSv { get; set; }
        public string TypeOfFundingNameEn { get; set; }
        public string CallProgrammeNameFi { get; set; }
        public string CallProgrammeNameSv { get; set; }
        public string CallProgrammeNameEn { get; set; }
        public ProfileEditorItemDate StartDate { get; set; }
        public ProfileEditorItemDate EndDate { get; set; }
        public decimal AmountInEur { get; set; }
    }
}
