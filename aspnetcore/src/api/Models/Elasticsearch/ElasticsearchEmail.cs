using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchEmail : ElasticsearchItemBase
    {
        public ElasticsearchEmail()
        {
            Value = "";
            PrimaryValue = null;
        }

        public string Value { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}
