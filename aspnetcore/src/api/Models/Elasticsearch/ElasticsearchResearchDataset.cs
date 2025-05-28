using System;
using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchResearchDataset : ElasticsearchItem
    {
        public ElasticsearchResearchDataset()
        {
            AccessType = "";
            Actor = new List<ElasticsearchActor>();
            FairdataUrl = "";
            Identifier = "";
            NameFi = "";
            NameSv = "";
            NameEn = "";
            DescriptionFi = "";
            DescriptionSv = "";
            DescriptionEn = "";
            Url = "";
            DatasetCreated = null;
            PreferredIdentifiers = new List<ElasticsearchPreferredIdentifier>();
        }

        // Properties are according to ElasticSearch index, not according to model DimResearchDataset
        public string AccessType { get; set; }
        public List<ElasticsearchActor> Actor { get; set; }
        public string FairdataUrl { get; set; }
        public int? DatasetCreated { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionSv { get; set; }
        public string Identifier { get; set; }
        public string NameEn { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public List<ElasticsearchPreferredIdentifier> PreferredIdentifiers { get; set; }
        public string Url { get; set; }
    }
}
