using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchSectorOrganization
    {
        public ElasticsearchSectorOrganization()
        {
            organizationId = "";
            OrganizationNameFi = "";
            OrganizationNameEn = "";
            OrganizationNameSv = "";
        }

        public string organizationId { get; set; }
        public string OrganizationNameFi { get; set; }
        public string OrganizationNameEn { get; set; }
        public string OrganizationNameSv { get; set; }
    }
}
