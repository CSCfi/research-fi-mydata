using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchAffiliation : ElasticsearchItem
    {
        public ElasticsearchAffiliation()
        {
            OrganizationNameFi = "";
            OrganizationNameSv = "";
            OrganizationNameEn = "";
            DepartmentNameFi = "";
            DepartmentNameSv = "";
            DepartmentNameEn = "";
            PositionNameFi = "";
            PositionNameSv = "";
            PositionNameEn = "";
            Type = "";
            StartDate = new ElasticsearchDate();
            EndDate = new ElasticsearchDate();
        }

        public string OrganizationNameFi { get; set; }
        public string OrganizationNameSv { get; set; }
        public string OrganizationNameEn { get; set; }
        public string DepartmentNameFi { get; set; }
        public string DepartmentNameSv { get; set; }
        public string DepartmentNameEn { get; set; }
        public string PositionNameFi { get; set; }
        public string PositionNameSv { get; set; }
        public string PositionNameEn { get; set; }
        public string Type { get; set; } 
        public ElasticsearchDate StartDate { get; set; }
        public ElasticsearchDate EndDate { get; set; }
    }
}
