namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchCooperation
    {
        public ElasticsearchCooperation()
        {
        }

        public int Id { get; set; }
        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public int? Order { get; set; }
    }
}
