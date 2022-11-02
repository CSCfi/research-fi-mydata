using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchEducation : ElasticsearchItem
    {
        public ElasticsearchEducation()
        {
            NameFi = "";
            NameEn = "";
            NameSv = "";
            DegreeGrantingInstitutionName = "";
            StartDate = new ElasticsearchDate();
            EndDate = new ElasticsearchDate();
        }

        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public string DegreeGrantingInstitutionName { get; set; }
        public ElasticsearchDate StartDate { get; set; }
        public ElasticsearchDate EndDate { get; set; }
    }
}
