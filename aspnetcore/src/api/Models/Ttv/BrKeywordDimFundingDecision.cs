using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrKeywordDimFundingDecision
    {
        public int DimKeywordId { get; set; }
        public int DimFundingDecisionId { get; set; }

        public virtual DimFundingDecision DimFundingDecision { get; set; }
        public virtual DimKeyword DimKeyword { get; set; }
    }
}
