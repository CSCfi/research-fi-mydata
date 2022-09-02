namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchPerson
    {
        public ElasticsearchPerson()
        {
            id = "";
            personal = new ElasticsearchPersonal();
            activity = new ElasticsearchActivity();
        }

        public ElasticsearchPerson(string orcidId)
        {
            id = orcidId;
            personal = new ElasticsearchPersonal();
            activity = new ElasticsearchActivity();
        }

        public string id { get; set; } 
        public ElasticsearchPersonal personal { get; set; }
        public ElasticsearchActivity activity { get; set; }
    }
}
