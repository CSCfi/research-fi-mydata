using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class TempBrParticipatesInFundingGroup
    {
        public int DimFundingDecisionid { get; set; }
        public int DimNameId { get; set; }
        public int DimOrganizationId { get; set; }
        public string RoleInFundingGroup { get; set; }
        public decimal? ShareOfFundingInEur { get; set; }
        public string SourceId { get; set; }
        public bool? EndOfParticipation { get; set; }
    }
}
