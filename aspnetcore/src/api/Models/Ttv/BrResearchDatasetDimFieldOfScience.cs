﻿using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrResearchDatasetDimFieldOfScience
    {
        public int DimResearchDatasetid { get; set; }
        public int DimFieldOfScienceid { get; set; }

        public virtual DimFieldOfScience DimFieldOfScience { get; set; }
        public virtual DimResearchDataset DimResearchDataset { get; set; }
    }
}