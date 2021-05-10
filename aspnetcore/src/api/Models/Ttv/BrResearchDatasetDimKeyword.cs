﻿using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrResearchDatasetDimKeyword
    {
        public int DimResearchDatasetId { get; set; }
        public int DimKeywordId { get; set; }

        public virtual DimKeyword DimKeyword { get; set; }
        public virtual DimResearchDataset DimResearchDataset { get; set; }
    }
}