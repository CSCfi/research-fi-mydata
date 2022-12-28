using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class FactDimReferencedataFieldOfScience
    {
        public int DimReferencedataId { get; set; }
        public int DimResearchDatasetId { get; set; }
        public int DimKnownPersonId { get; set; }
        public int DimPublicationId { get; set; }
        public int DimResearchActivityId { get; set; }
        public int DimFundingDecisionId { get; set; }
        public int DimInfrastructureId { get; set; }

        public virtual DimFundingDecision DimFundingDecision { get; set; }
        public virtual DimInfrastructure DimInfrastructure { get; set; }
        public virtual DimKnownPerson DimKnownPerson { get; set; }
        public virtual DimPublication DimPublication { get; set; }
        public virtual DimReferencedatum DimReferencedata { get; set; }
        public virtual DimResearchDataset DimResearchDataset { get; set; }
    }
}
