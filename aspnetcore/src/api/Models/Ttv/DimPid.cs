using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimPid
    {
        public DimPid()
        {
            FactFieldValuesDimPid = new HashSet<FactFieldValues>();
            FactFieldValuesDimPidIdOrcidPutCodeNavigation = new HashSet<FactFieldValues>();
        }

        public int Id { get; set; }
        public string PidContent { get; set; }
        public string PidType { get; set; }
        public int? DimOrganizationId { get; set; }
        public int? DimKnownPersonId { get; set; }
        public int? DimPublicationId { get; set; }
        public int? DimServiceId { get; set; }
        public int? DimInfrastructureId { get; set; }
        public int? DimPublicationChannelId { get; set; }
        public int? DimResearchDatasetId { get; set; }
        public int? DimFundingDecisionId { get; set; }
        public int? DimResearchDataCatalogId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimKnownPerson DimKnownPerson { get; set; }
        public virtual DimOrganization DimOrganization { get; set; }
        public virtual ICollection<FactFieldValues> FactFieldValuesDimPid { get; set; }
        public virtual ICollection<FactFieldValues> FactFieldValuesDimPidIdOrcidPutCodeNavigation { get; set; }
    }
}
