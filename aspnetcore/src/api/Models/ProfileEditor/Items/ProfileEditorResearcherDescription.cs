namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorResearcherDescription : ProfileEditorItem
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
