using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class BrRelatedFundingDecision
    {
        public int DimFundingDecisionFromId { get; set; }
        public int DimFundingDecisionToId { get; set; }

        public virtual DimFundingDecision DimFundingDecisionFrom { get; set; }
        public virtual DimFundingDecision DimFundingDecisionTo { get; set; }
    }
}
