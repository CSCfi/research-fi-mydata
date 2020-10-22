using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class BrEsfriDimInfrastructure
    {
        public int DimEsfriId { get; set; }
        public int DimInfrastructureId { get; set; }

        public virtual DimEsfri DimEsfri { get; set; }
        public virtual DimInfrastructure DimInfrastructure { get; set; }
    }
}
