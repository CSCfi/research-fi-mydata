using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchExternalIdentifier : ElasticsearchItem
    {
        public ElasticsearchExternalIdentifier()
        {
            PidContent = "";
            PidType = "";
        }

        public string PidContent { get; set; }
        public string PidType { get; set; }
    }
}
