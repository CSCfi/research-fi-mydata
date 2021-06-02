using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorItemAffiliation : ProfileEditorItem
    {
        public ProfileEditorItemAffiliation()
        {
            OrganizationName = "";
            PositionNameFi = "";
            PositionNameEn = "";
            PositionNameSv = "";
            StartDate = new ProfileEditorItemDate();
            EndDate = new ProfileEditorItemDate();
        }

        public string OrganizationName { get; set; }
        public string PositionNameFi { get; set; }
        public string PositionNameEn { get; set; }
        public string PositionNameSv { get; set; }
        public ProfileEditorItemDate StartDate { get; set; }
        public ProfileEditorItemDate EndDate { get; set; }
    }
}
