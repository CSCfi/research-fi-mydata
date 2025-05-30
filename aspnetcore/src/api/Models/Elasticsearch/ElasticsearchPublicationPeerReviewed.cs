namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchPublicationPeerReviewed
    {
        public ElasticsearchPublicationPeerReviewed()
        {
        }

        public string Id { get; set; }
        public string NameFiPeerReviewed { get; set; }
        public string NameSvPeerReviewed { get; set; }
        public string NameEnPeerReviewed { get; set; }
    }
}