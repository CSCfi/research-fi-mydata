using System.Collections.Generic;

namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorSectorOrganization
    {
        public ProfileEditorSectorOrganization()
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
