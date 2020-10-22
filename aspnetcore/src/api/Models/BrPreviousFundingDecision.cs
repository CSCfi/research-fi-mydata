using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class BrPreviousFundingDecision
    {
        public int DimFundingDecisionFromId { get; set; }
        public int DimFundingDecisionToId { get; set; }

        public virtual DimFundingDecision DimFundingDecisionFrom { get; set; }
        public virtual DimFundingDecision DimFundingDecisionTo { get; set; }
    }
}
