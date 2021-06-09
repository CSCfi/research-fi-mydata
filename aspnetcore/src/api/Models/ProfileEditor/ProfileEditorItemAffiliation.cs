using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorItemAffiliation : ProfileEditorItem
    {
        public ProfileEditorItemAffiliation()
        {
            OrganizationNameFi = "";
            OrganizationNameEn = "";
            OrganizationNameSv = "";
            PositionNameFi = "";
            PositionNameEn = "";
            PositionNameSv = "";
            Type = "";
            StartDate = new ProfileEditorItemDate();
            EndDate = new ProfileEditorItemDate();
        }

        public string OrganizationNameFi { get; set; }
        public string OrganizationNameEn { get; set; }
        public string OrganizationNameSv { get; set; }
        public string PositionNameFi { get; set; }
        public string PositionNameEn { get; set; }
        public string PositionNameSv { get; set; }
        public string Type { get; set; } 
        public ProfileEditorItemDate StartDate { get; set; }
        public ProfileEditorItemDate EndDate { get; set; }
    }
}
