using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class FactUpkeep
    {
        public int DimOrganizationId { get; set; }
        public int DimGeoId { get; set; }
        public int DimInfrastructureId { get; set; }
        public int DimServiceId { get; set; }
        public int DimServicePointId { get; set; }
        public int DimDateIdStart { get; set; }
        public int DimDateIdEnd { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimDate DimDateIdEndNavigation { get; set; }
        public virtual DimDate DimDateIdStartNavigation { get; set; }
        public virtual DimGeo DimGeo { get; set; }
        public virtual DimInfrastructure DimInfrastructure { get; set; }
        public virtual DimOrganization DimOrganization { get; set; }
        public virtual DimService DimService { get; set; }
        public virtual DimServicePoint DimServicePoint { get; set; }
    }
}
