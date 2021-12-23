using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ItemResearchDataset
    {
        public ItemResearchDataset()
        {
            Actor = new List<Actor>();
            Identifier = "";
            NameFi = "";
            NameSv = "";
            NameEn = "";
            DescriptionFi = "";
            DescriptionSv = "";
            DescriptionEn = "";
            DatasetCreated = null;
            PreferredIdentifiers = new List<PreferredIdentifier>();
        }

        // Properties are according to ElasticSearch index, not according to model DimResearchDataset
        public List<Actor> Actor { get; set; }
        public string Identifier { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionSv { get; set; }
        public string DescriptionEn { get; set; }
        public int? DatasetCreated { get; set; }
        public List<PreferredIdentifier> PreferredIdentifiers { get; set; }
    }
}
