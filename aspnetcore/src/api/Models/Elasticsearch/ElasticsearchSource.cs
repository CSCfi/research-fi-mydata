using api.Models.Common;
namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchSource
    {
        public ElasticsearchSource()
        {
        }

        public string RegisteredDataSource { get; set; }
        public Organization Organization { get; set; }
    }
}
