using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchWebLink : ElasticsearchItem
    {
        public ElasticsearchWebLink()
        {
            Url = "";
            LinkLabel = "";
            LinkType = "";
        }

        public string Url { get; set; }
        public string LinkLabel { get; set; }
        public string LinkType { get; set; }
    }
}
