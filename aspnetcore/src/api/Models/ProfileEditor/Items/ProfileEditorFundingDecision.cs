namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorFundingDecision : ProfileEditorItem
    {
        public ProfileEditorFundingDecision()
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
            FundingStartYear = null;
            FundingEndYear = null;
            AmountInEur = -1;
            AmountInFundingDecisionCurrency = null;
            FundingDecisionCurrencyAbbreviation = null;
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
        public int? FundingStartYear { get; set; }
        public int? FundingEndYear { get; set; }
        public decimal AmountInEur { get; set; }
        public decimal? AmountInFundingDecisionCurrency { get; set; }
        public string? FundingDecisionCurrencyAbbreviation { get; set; }
    }
}
