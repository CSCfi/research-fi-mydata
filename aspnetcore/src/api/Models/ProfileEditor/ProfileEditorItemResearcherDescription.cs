namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorItemResearcherDescription : ProfileEditorItem
    {
        public ProfileEditorItemResearcherDescription()
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
