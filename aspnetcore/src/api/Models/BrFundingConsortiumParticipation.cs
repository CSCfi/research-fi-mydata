using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class BrFundingConsortiumParticipation
    {
        public int DimFundingDecisionId { get; set; }
        public int DimOrganizationid { get; set; }
        public string RoleInConsortium { get; set; }
        public decimal? ShareOfFundingInEur { get; set; }

        public virtual DimFundingDecision DimFundingDecision { get; set; }
        public virtual DimOrganization DimOrganization { get; set; }
    }
}
