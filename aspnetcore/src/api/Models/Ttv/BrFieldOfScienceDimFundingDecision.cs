using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class BrFieldOfScienceDimFundingDecision
    {
        public int DimFieldOfScienceId { get; set; }
        public int DimFundingDecisionId { get; set; }

        public virtual DimFieldOfScience DimFieldOfScience { get; set; }
        public virtual DimFundingDecision DimFundingDecision { get; set; }
    }
}
