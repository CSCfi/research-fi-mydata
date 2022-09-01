using System.Collections.Generic;
using api.Models.ProfileEditor;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchResearchDataset : ElasticsearchItemBase
    {
        public ElasticsearchResearchDataset()
        {
            Actor = new List<ElasticsearchActor>();
            Identifier = "";
            NameFi = "";
            NameSv = "";
            NameEn = "";
            DescriptionFi = "";
            DescriptionSv = "";
            DescriptionEn = "";
            DatasetCreated = null;
            PreferredIdentifiers = new List<ElasticsearchPreferredIdentifier>();
        }

        // Properties are according to ElasticSearch index, not according to model DimResearchDataset
        public List<ElasticsearchActor> Actor { get; set; }
        public string Identifier { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionSv { get; set; }
        public string DescriptionEn { get; set; }
        public int? DatasetCreated { get; set; }
        public List<ElasticsearchPreferredIdentifier> PreferredIdentifiers { get; set; }
    }
}
