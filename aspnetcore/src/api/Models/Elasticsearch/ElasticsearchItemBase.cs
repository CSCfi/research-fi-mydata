using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchItemBase
    {
        public ElasticsearchItemBase()
        {
        }

        public List<ElasticsearchSource> DataSources { get; set; }
    }
}
