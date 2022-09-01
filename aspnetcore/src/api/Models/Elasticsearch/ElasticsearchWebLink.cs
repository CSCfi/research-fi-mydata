using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchWebLink : ElasticsearchItemBase
    {
        public ElasticsearchWebLink()
        {
            Url = "";
            LinkLabel = "";
            PrimaryValue = null;
        }

        public string Url { get; set; }
        public string LinkLabel { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}
