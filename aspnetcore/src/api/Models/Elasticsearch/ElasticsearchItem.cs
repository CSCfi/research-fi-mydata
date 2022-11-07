using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchItem
    {
        public ElasticsearchItem()
        {
        }

        public ElasticsearchItemMeta itemMeta { get; set; }
        public List<ElasticsearchSource> DataSources { get; set; }
    }
}
