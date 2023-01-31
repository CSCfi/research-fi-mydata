using System.Collections.Generic;

namespace api.Models.Orcid
{
    public class OrcidImportHelper {
        public OrcidImportHelper()
        {
            dimNameIds = new();
            dimWebLinkIds = new();
            dimResearcherDescriptionIds = new();
            dimEmailAddressIds = new();
        }

        public List<int> dimNameIds { get; set; }
        public List<int> dimWebLinkIds { get; set; }
        public List<int> dimResearcherDescriptionIds { get; set; }
        public List<int> dimEmailAddressIds { get; set; }
    }
}