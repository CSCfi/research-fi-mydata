using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrFundingDecisionDimFieldOfArt
    {
        public int DimFundingDecisionId { get; set; }
        public int DimFieldOfArtId { get; set; }

        public virtual DimFieldOfArt DimFieldOfArt { get; set; }
        public virtual DimFundingDecision DimFundingDecision { get; set; }
    }
}
