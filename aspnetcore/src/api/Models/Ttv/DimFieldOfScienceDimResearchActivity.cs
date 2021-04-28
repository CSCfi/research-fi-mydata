using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimFieldOfScienceDimResearchActivity
    {
        public int DimFieldOfScienceId { get; set; }
        public int DimResearchActivityId { get; set; }

        public virtual DimFieldOfScience DimFieldOfScience { get; set; }
        public virtual DimResearchActivity DimResearchActivity { get; set; }
    }
}
