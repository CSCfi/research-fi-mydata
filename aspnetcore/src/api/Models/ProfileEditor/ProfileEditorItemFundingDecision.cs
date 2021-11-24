namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorItemFundingDecision : ProfileEditorItem
    {
        public ProfileEditorItemFundingDecision()
        {
            FunderProjectNumber = "";
            NameFi = "";
            NameSv = "";
            NameEn = "";
            FunderNameFi = "";
            FunderNameSv = "";
            FunderNameEn = "";
            StartDate = new ProfileEditorItemDate();
            EndDate = new ProfileEditorItemDate();
        }

        public string FunderProjectNumber { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string FunderNameFi { get; set; }
        public string FunderNameSv { get; set; }
        public string FunderNameEn { get; set; }
        public ProfileEditorItemDate StartDate { get; set; }
        public ProfileEditorItemDate EndDate { get; set; }
        public decimal AmountInEur { get; set; }
    }
}
