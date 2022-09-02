using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchTelephoneNumber : ElasticsearchItemBase
    {
        public ElasticsearchTelephoneNumber()
        {
            Value = "";
            PrimaryValue = null;
        }

        public string Value { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}
