using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimPid
    {
        public DimPid()
        {
            DimFundingDecision = new HashSet<DimFundingDecision>();
            FactFieldDisplayContent = new HashSet<FactFieldDisplayContent>();
        }

        public string PidContent { get; set; }
        public string PidType { get; set; }
        public int DimOrganizationId { get; set; }
        public int DimKnownPersonId { get; set; }
        public int DimPublicationId { get; set; }
        public int DimServiceId { get; set; }
        public int DimInfrastructureid { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimInfrastructure DimInfrastructure { get; set; }
        public virtual DimKnownPerson DimKnownPerson { get; set; }
        public virtual DimOrganization DimOrganization { get; set; }
        public virtual DimPublication DimPublication { get; set; }
        public virtual DimService DimService { get; set; }
        public virtual ICollection<DimFundingDecision> DimFundingDecision { get; set; }
        public virtual ICollection<FactFieldDisplayContent> FactFieldDisplayContent { get; set; }
    }
}
