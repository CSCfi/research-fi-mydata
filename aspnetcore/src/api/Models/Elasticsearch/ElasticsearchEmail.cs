using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchEmail : ElasticsearchItem
    {
        public ElasticsearchEmail()
        {
            Value = "";
        }

        public string Value { get; set; }
    }
}
