using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class BrMerilDimInfrastructure
    {
        public int DimMerilId { get; set; }
        public int DimInfrastructureId { get; set; }

        public virtual DimInfrastructure DimInfrastructure { get; set; }
        public virtual DimMeril DimMeril { get; set; }
    }
}
