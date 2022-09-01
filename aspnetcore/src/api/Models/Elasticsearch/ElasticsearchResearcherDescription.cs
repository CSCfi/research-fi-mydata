namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchResearcherDescription : ElasticsearchItemBase
    {
        public ElasticsearchResearcherDescription()
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
