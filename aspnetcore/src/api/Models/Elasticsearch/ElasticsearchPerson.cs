using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchPerson
    {
        public ElasticsearchPerson()
        {
            id = "";
            personal = new();
            activity = new();
            uniqueDataSources = new();
        }

        public ElasticsearchPerson(string orcidId)
        {
            id = orcidId;
            personal = new();
            activity = new();
            uniqueDataSources = new();
        }

        public string id { get; set; } 
        public ElasticsearchPersonal personal { get; set; }
        public ElasticsearchActivity activity { get; set; }
        public List<ElasticsearchSource> uniqueDataSources { get; set; }
    }
}
