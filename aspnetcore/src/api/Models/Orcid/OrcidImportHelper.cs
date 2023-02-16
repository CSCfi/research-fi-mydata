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
            dimKeywordIds = new();
            dimPidIds = new();
            dimAffiliationIds = new();
            dimEducationIds = new();
            dimOrcidPublicationIds = new();
        }

        public List<int> dimNameIds { get; set; }
        public List<int> dimWebLinkIds { get; set; }
        public List<int> dimResearcherDescriptionIds { get; set; }
        public List<int> dimEmailAddressIds { get; set; }
        public List<int> dimKeywordIds { get; set; }
        public List<int> dimPidIds { get; set; }
        public List<int> dimAffiliationIds { get; set; }
        public List<int> dimEducationIds { get; set; }
        public List<int> dimOrcidPublicationIds { get; set; }
    }
}