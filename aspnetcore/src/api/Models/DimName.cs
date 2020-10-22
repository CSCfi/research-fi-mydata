using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimName
    {
        public DimName()
        {
            BrParticipatesInFundingGroup = new HashSet<BrParticipatesInFundingGroup>();
            DimFundingDecision = new HashSet<DimFundingDecision>();
            FactContribution = new HashSet<FactContribution>();
        }

        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstNames { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int DimKnownPersonIdConfirmedIdentity { get; set; }
        public int DimKnownPersonidFormerNames { get; set; }

        public virtual DimKnownPerson DimKnownPersonIdConfirmedIdentityNavigation { get; set; }
        public virtual DimKnownPerson DimKnownPersonidFormerNamesNavigation { get; set; }
        public virtual ICollection<BrParticipatesInFundingGroup> BrParticipatesInFundingGroup { get; set; }
        public virtual ICollection<DimFundingDecision> DimFundingDecision { get; set; }
        public virtual ICollection<FactContribution> FactContribution { get; set; }
    }
}
