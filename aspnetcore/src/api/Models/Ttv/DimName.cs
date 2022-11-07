using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimName
    {
        public DimName()
        {
            BrParticipatesInFundingGroups = new HashSet<BrParticipatesInFundingGroup>();
            DimFundingDecisions = new HashSet<DimFundingDecision>();
            FactContributions = new HashSet<FactContribution>();
            FactFieldValues = new HashSet<FactFieldValue>();
        }

        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstNames { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int DimKnownPersonIdConfirmedIdentity { get; set; }
        public string SourceProjectId { get; set; }
        public string FullName { get; set; }
        public int DimRegisteredDataSourceId { get; set; }

        public virtual DimKnownPerson DimKnownPersonIdConfirmedIdentityNavigation { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual ICollection<BrParticipatesInFundingGroup> BrParticipatesInFundingGroups { get; set; }
        public virtual ICollection<DimFundingDecision> DimFundingDecisions { get; set; }
        public virtual ICollection<FactContribution> FactContributions { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
    }
}
