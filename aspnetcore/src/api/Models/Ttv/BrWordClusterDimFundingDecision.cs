using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrWordClusterDimFundingDecision
    {
        public int DimWordClusterId { get; set; }
        public int DimFundingDecisionId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public string SourceId { get; set; }

        public virtual DimFundingDecision DimFundingDecision { get; set; }
        public virtual DimWordCluster DimWordCluster { get; set; }
    }
}
