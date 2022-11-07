using api.Models.ProfileEditor;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchActivityAndReward : ElasticsearchItem
    {
        public ElasticsearchActivityAndReward()
        {
            NameFi = "";
            NameEn = "";
            NameSv = "";
            DescriptionFi = "";
            DescriptionEn = "";
            DescriptionSv = "";
            InternationalCollaboration = false;
            StartDate = new ElasticsearchDate();
            EndDate = new ElasticsearchDate();
        }

        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionSv { get; set; }
        public bool InternationalCollaboration { get; set; }
        public ElasticsearchDate StartDate { get; set; }
        public ElasticsearchDate EndDate { get; set; }
    }
}
