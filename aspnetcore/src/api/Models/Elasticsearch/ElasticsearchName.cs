using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchName : ElasticsearchItem
    {
        public ElasticsearchName()
        {
            FirstNames = "";
            LastName = "";
            FullName = "";
            PrimaryValue = null;
        }

        public string FirstNames { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}
