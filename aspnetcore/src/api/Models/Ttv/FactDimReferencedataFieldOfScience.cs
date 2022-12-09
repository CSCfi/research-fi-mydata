using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class FactDimReferencedataFieldOfScience
    {
        public int DimReferencedataId { get; set; }
        public int DimResearchDatasetId { get; set; }
        public int DimKnownPersonId { get; set; }

        public virtual DimKnownPerson DimKnownPerson { get; set; }
        public virtual DimReferencedatum DimReferencedata { get; set; }
        public virtual DimResearchDataset DimResearchDataset { get; set; }
    }
}
