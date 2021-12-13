using api.Models.Common;
namespace api.Models.Elasticsearch
{
    public partial class Source
    {
        public Source()
        {
        }

        public string RegisteredDataSource { get; set; }
        public Organization Organization { get; set; }
    }
}
