using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimDate
    {
        public DimDate()
        {
            DimCallProgrammeDimDateIdDueNavigation = new HashSet<DimCallProgramme>();
            DimCallProgrammeDimDateIdOpenNavigation = new HashSet<DimCallProgramme>();
            DimFundingDecisionDimDateIdApprovalNavigation = new HashSet<DimFundingDecision>();
            DimFundingDecisionDimDateIdEndNavigation = new HashSet<DimFundingDecision>();
            DimFundingDecisionDimDateIdStartNavigation = new HashSet<DimFundingDecision>();
            FactContribution = new HashSet<FactContribution>();
            FactUpkeepDimDateIdEndNavigation = new HashSet<FactUpkeep>();
            FactUpkeepDimDateIdStartNavigation = new HashSet<FactUpkeep>();
        }

        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ICollection<DimCallProgramme> DimCallProgrammeDimDateIdDueNavigation { get; set; }
        public virtual ICollection<DimCallProgramme> DimCallProgrammeDimDateIdOpenNavigation { get; set; }
        public virtual ICollection<DimFundingDecision> DimFundingDecisionDimDateIdApprovalNavigation { get; set; }
        public virtual ICollection<DimFundingDecision> DimFundingDecisionDimDateIdEndNavigation { get; set; }
        public virtual ICollection<DimFundingDecision> DimFundingDecisionDimDateIdStartNavigation { get; set; }
        public virtual ICollection<FactContribution> FactContribution { get; set; }
        public virtual ICollection<FactUpkeep> FactUpkeepDimDateIdEndNavigation { get; set; }
        public virtual ICollection<FactUpkeep> FactUpkeepDimDateIdStartNavigation { get; set; }
    }
}
