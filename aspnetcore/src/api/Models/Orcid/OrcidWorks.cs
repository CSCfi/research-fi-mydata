using System.Collections.Generic;

namespace api.Models.Orcid
{
    public partial class OrcidWorks
    {
        public OrcidWorks()
        {
            Datasets = new();
            Publications = new();
            ResearchActivities = new();
        }

        public List<OrcidDataset> Datasets { get; set; }
        public List<OrcidPublication> Publications { get; set; }
        public List<OrcidResearchActivity> ResearchActivities { get; set; }
    }
}