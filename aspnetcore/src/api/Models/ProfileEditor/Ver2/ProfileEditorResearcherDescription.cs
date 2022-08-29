namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorResearcherDescription : ProfileEditorItem2
    {
        public ProfileEditorResearcherDescription()
        {
            ResearchDescriptionFi = "";
            ResearchDescriptionEn = "";
            ResearchDescriptionSv = "";
        }

        public string ResearchDescriptionFi { get; set; }
        public string ResearchDescriptionEn { get; set; }
        public string ResearchDescriptionSv { get; set; }
    }
}
