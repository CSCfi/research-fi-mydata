namespace api.Models.Elasticsearch
{
    public partial class ItemResearcherDescription
    {
        public ItemResearcherDescription()
        {
            ResearchDescriptionFi = "";
            ResearchDescriptionEn = "";
            ResearchDescriptionSv = "";
            PrimaryValue = null;
        }

        public string ResearchDescriptionFi { get; set; }
        public string ResearchDescriptionEn { get; set; }
        public string ResearchDescriptionSv { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}
