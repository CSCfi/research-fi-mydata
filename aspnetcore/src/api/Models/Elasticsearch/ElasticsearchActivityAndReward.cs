using System.Collections.Generic;
using api.Models.ProfileEditor;
using api.Models.ProfileEditor.Items;
using Nest;
using static api.Models.Common.Constants;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchActivityAndReward : ElasticsearchItem
    {
        public ElasticsearchActivityAndReward()
        {
            NameFi = "";
            NameEn = "";
            NameSv = "";
            DescriptionFi = "";
            DescriptionEn = "";
            DescriptionSv = "";
            InternationalCollaboration = null;
            StartDate = new ElasticsearchDate();
            EndDate = new ElasticsearchDate();
            ActivityTypeCode = "";
            ActivityTypeNameFi = "";
            ActivityTypeNameEn = "";
            ActivityTypeNameSv = "";
            RoleCode = "";
            RoleNameFi = "";
            RoleNameEn = "";
            RoleNameSv = "";
            OrganizationNameFi = "";
            OrganizationNameSv = "";
            OrganizationNameEn = "";
            DepartmentNameFi = "";
            DepartmentNameSv = "";
            DepartmentNameEn = "";
            sector = new List<ElasticsearchSector> { };
        }

        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionSv { get; set; }
        public bool? InternationalCollaboration { get; set; }
        public ElasticsearchDate StartDate { get; set; }
        public ElasticsearchDate EndDate { get; set; }
        public string ActivityTypeCode { get; set; }
        public string ActivityTypeNameFi { get; set; }
        public string ActivityTypeNameEn { get; set; }
        public string ActivityTypeNameSv { get; set; }
        public string RoleCode { get; set; }
        public string RoleNameFi { get; set; }
        public string RoleNameEn { get; set; }
        public string RoleNameSv { get; set; }
        public string OrganizationNameFi { get; set; }
        public string OrganizationNameSv { get; set; }
        public string OrganizationNameEn { get; set; }
        public string DepartmentNameFi { get; set; }
        public string DepartmentNameSv { get; set; }
        public string DepartmentNameEn { get; set; }

        [Nested]
        [PropertyName("sector")]
        public List<ElasticsearchSector> sector { get; set; }
    }
}
