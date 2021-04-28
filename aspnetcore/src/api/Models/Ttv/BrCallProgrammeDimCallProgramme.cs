using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrCallProgrammeDimCallProgramme
    {
        public int DimCallProgrammeId { get; set; }
        public int DimCallProgrammeId2 { get; set; }

        public virtual DimCallProgramme DimCallProgramme { get; set; }
        public virtual DimCallProgramme DimCallProgrammeId2Navigation { get; set; }
    }
}
