using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class BrInfrastructureDimFieldOfScience
    {
        public int DimInfrastructureId { get; set; }
        public int DimFieldOfScienceId { get; set; }

        public virtual DimFieldOfScience DimFieldOfScience { get; set; }
        public virtual DimInfrastructure DimInfrastructure { get; set; }
    }
}
