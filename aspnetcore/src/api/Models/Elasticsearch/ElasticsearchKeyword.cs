using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchKeyword : ElasticsearchItem
    {
        public ElasticsearchKeyword()
        {
            Value = "";
            PrimaryValue = null;
        }

        public string Value { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}
