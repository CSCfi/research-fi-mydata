using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrParticipatesInFundingGroup
    {
        public int DimFundingDecisionid { get; set; }
        public int DimNameId { get; set; }
        public int DimOrganizationId { get; set; }
        public string RoleInFundingGroup { get; set; }
        public decimal? ShareOfFundingInEur { get; set; }
        public string SourceId { get; set; }
        public bool? EndOfParticipation { get; set; }

        public virtual DimFundingDecision DimFundingDecision { get; set; }
        public virtual DimName DimName { get; set; }
        public virtual DimOrganization DimOrganization { get; set; }
    }
}
