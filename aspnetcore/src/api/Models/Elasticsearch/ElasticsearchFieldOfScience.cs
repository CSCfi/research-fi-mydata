using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchFieldOfScience : ElasticsearchItem
    {
        public ElasticsearchFieldOfScience()
        {
            NameFi = "";
            NameEn = "";
            NameSv = "";
            PrimaryValue = null;
        }

        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}
