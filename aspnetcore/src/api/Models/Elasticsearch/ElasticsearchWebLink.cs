using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchWebLink : ElasticsearchItem
    {
        public ElasticsearchWebLink()
        {
            Url = "";
            LinkLabel = "";
        }

        public string Url { get; set; }
        public string LinkLabel { get; set; }
    }
}
