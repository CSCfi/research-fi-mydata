namespace api.Models.Elasticsearch
{
    public partial class Source
    {
        public Source()
        {
        }

        public string RegisteredDataSource { get; set; }
        public SourceOrganization Organization { get; set; }
    }
}
