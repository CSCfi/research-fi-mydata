using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchExternalIdentifier : ElasticsearchItemBase
    {
        public ElasticsearchExternalIdentifier()
        {
            PidContent = "";
            PidType = "";
            PrimaryValue = null;
        }

        public string PidContent { get; set; }
        public string PidType { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}
