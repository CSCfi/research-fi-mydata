using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrLanguageCodesForDataset
    {
        public int DimResearchDatasetId { get; set; }
        public int DimReferencedataId { get; set; }

        public virtual DimReferencedatum DimReferencedata { get; set; }
        public virtual DimResearchDataset DimResearchDataset { get; set; }
    }
}
