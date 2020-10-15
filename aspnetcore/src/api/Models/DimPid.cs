using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimPid
    {
        public DimPid()
        {
            DimOrganizationId = -1;
            DimInfrastructureid = -1;
            DimPublicationId = -1;
            DimServiceId = -1;
            SourceId = "ORCID";
            SourceDescription = "Researcher profile API";
            Created = DateTime.Now;
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

        public virtual DimKnownPerson DimKnownPerson { get; set; }
    }
}
