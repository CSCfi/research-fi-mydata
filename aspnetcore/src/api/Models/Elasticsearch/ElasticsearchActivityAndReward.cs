﻿using api.Models.ProfileEditor;

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
    }
}
