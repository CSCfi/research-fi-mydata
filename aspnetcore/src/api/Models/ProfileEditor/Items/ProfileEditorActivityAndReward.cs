using System.Collections.Generic;

namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorActivityAndReward : ProfileEditorItem
    {
        public ProfileEditorActivityAndReward()
        {
            NameFi = "";
            NameEn = "";
            NameSv = "";
            DescriptionFi = "";
            DescriptionEn = "";
            DescriptionSv = "";
            InternationalCollaboration = null;
            StartDate = new ProfileEditorDate();
            EndDate = new ProfileEditorDate();
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
            Url = "";
            WebLinks = new();
            sector = new List<ProfileEditorSector> { };
        }

        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionSv { get; set; }
        public bool? InternationalCollaboration { get; set; }
        public ProfileEditorDate StartDate { get; set; }
        public ProfileEditorDate EndDate { get; set; }
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
        public string Url { get; set; }
        public List<ProfileEditorWebLink_WithoutItemMeta> WebLinks { get; set; }

        // Fields required in Elasticsearch person index. Elasticsearch model is mapped from ProfileEditor model.
        public List<ProfileEditorSector> sector { get; set; }
    }
}
