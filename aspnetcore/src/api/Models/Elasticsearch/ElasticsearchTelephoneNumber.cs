using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchTelephoneNumber : ElasticsearchItem
    {
        public ElasticsearchTelephoneNumber()
        {
            Value = "";
        }

        public string Value { get; set; }
    }
}
