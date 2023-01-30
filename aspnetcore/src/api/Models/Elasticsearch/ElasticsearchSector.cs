using System.Collections.Generic;
using Nest;

namespace api.Models.Elasticsearch
{
    [ElasticsearchType(RelationName = "sector")]
    public partial class ElasticsearchSector
    {
        public ElasticsearchSector()
        {
            sectorId = "";
            nameFiSector = "";
            nameEnSector = "";
            nameSvSector = "";
            organization = new List<ElasticsearchSectorOrganization> { new ElasticsearchSectorOrganization() };
        }

        public string sectorId { get; set; }
        public string nameFiSector { get; set; }
        public string nameEnSector { get; set; }
        public string nameSvSector { get; set; }

        [Nested]
        [PropertyName("organization")]
        public List<ElasticsearchSectorOrganization> organization { get; set; }
    }
}
