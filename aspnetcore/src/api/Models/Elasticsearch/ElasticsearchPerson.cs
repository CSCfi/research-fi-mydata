using System;
using System.Collections.Generic;
using Nest;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchPerson
    {
        public ElasticsearchPerson()
        {
            id = "";
            personal = new();
            activity = new();
            cooperation = new();
            uniqueDataSources = new();
        }

        public ElasticsearchPerson(string orcidId)
        {
            id = orcidId;
            personal = new();
            activity = new();
            cooperation = new();
            uniqueDataSources = new();
        }

        public string id { get; set; } 
        public ElasticsearchPersonal personal { get; set; }
        [Nested]
        [PropertyName("activity")]
        public ElasticsearchActivity activity { get; set; }
        public List<ElasticsearchCooperation> cooperation { get; set; }
        public List<ElasticsearchSource> uniqueDataSources { get; set; }
        public DateTime? updated { get; set; }
    }
}
