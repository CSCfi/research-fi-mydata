using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class BrLanguageCodesForDatasets
    {
        public int DimResearchDatasetId { get; set; }
        public int DimReferencedataId { get; set; }

        public virtual DimReferencedata DimReferencedata { get; set; }
        public virtual DimResearchDataset DimResearchDataset { get; set; }
    }
}
