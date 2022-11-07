using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchKeyword : ElasticsearchItem
    {
        public ElasticsearchKeyword()
        {
            Value = "";
        }

        public string Value { get; set; }
    }
}
