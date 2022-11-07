namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchPreferredIdentifier
    {
        public ElasticsearchPreferredIdentifier()
        {
            PidContent = "";
            PidType = "";
        }

        public string PidContent { get; set; }
        public string PidType { get; set; }
    }
}
