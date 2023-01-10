using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class TempFactDimReferencedataFieldOfScience
    {
        public int DimReferencedataId { get; set; }
        public int DimResearchDatasetId { get; set; }
        public int DimKnownPersonId { get; set; }
        public int DimPublicationId { get; set; }
        public int DimResearchActivityId { get; set; }
        public int DimFundingDecisionId { get; set; }
        public int DimInfrastructureId { get; set; }
    }
}
