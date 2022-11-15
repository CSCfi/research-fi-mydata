using System.Collections.Generic;

namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorSector
    {
        public ProfileEditorSector()
        {
            sectorId = "";
            nameFiSector = "";
            nameEnSector = "";
            nameSvSector = "";
            organization = new List<ProfileEditorSectorOrganization> {};
        }

        public string sectorId { get; set; }
        public string nameFiSector { get; set; }
        public string nameEnSector { get; set; }
        public string nameSvSector { get; set; }
        public List<ProfileEditorSectorOrganization> organization { get; set; }
    }
}
