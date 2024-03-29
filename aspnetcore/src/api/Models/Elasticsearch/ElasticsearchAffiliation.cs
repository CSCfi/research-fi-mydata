﻿using System.Collections.Generic;
using Nest;

namespace api.Models.Elasticsearch
{
    [ElasticsearchType(RelationName = "affiliations")]
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
            AffiliationTypeFi = "";
            AffiliationTypeEn = "";
            AffiliationTypeSv = "";
            StartDate = new ElasticsearchDate();
            EndDate = new ElasticsearchDate();
            sector = new List<ElasticsearchSector> {};
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
        public string AffiliationTypeFi { get; set; }
        public string AffiliationTypeEn { get; set; }
        public string AffiliationTypeSv { get; set; }
        public ElasticsearchDate StartDate { get; set; }
        public ElasticsearchDate EndDate { get; set; }

        [Nested]
        [PropertyName("sector")]
        public List<ElasticsearchSector> sector { get; set; }
    }
}
