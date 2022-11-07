namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchResearcherDescription : ElasticsearchItem
    {
        public ElasticsearchResearcherDescription()
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
