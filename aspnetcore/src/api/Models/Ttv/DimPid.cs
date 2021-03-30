using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimPid
    {
        public DimPid()
        {
            FactFieldValueDimPidIdOrcidPutCodeNavigations = new HashSet<FactFieldValue>();
            FactFieldValueDimPids = new HashSet<FactFieldValue>();
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
        public virtual ICollection<FactFieldValue> FactFieldValueDimPidIdOrcidPutCodeNavigations { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValueDimPids { get; set; }
    }
}
