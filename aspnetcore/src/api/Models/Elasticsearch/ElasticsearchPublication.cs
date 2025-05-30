using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchPublication : ElasticsearchItem
    {
        public ElasticsearchPublication()
        {
            AuthorsText = "";
            Doi = "";
            ConferenceName = "";
            JournalName = "";
            OpenAccess = 0;
            ParentPublicationName = "";
            PublicationId = "";
            PublicationName = "";
            PublicationTypeCode = "";
            PublicationYear = null;
            SelfArchivedAddress = "";
            SelfArchivedCode = "";
        }

        public string AuthorsText { get; set; }
        public string ConferenceName { get; set; }
        public string Doi { get; set; }
        public string JournalName { get; set; }
        public int OpenAccess { get; set; }
        public string ParentPublicationName { get; set; }
        public List<ElasticsearchPublicationPeerReviewed> PeerReviewed { get; set; } = new List<ElasticsearchPublicationPeerReviewed>();
        public string PublicationId { get; set; }
        public string PublicationName { get; set; }
        public string PublicationTypeCode { get; set; }
        public int? PublicationYear { get; set; }
        public string SelfArchivedAddress { get; set; }
        public string SelfArchivedCode { get; set; }
    }
}
