using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class BrDimAuroraHakualatDimCallProgramme
    {
        public int DimAuroraHakualatid { get; set; }
        public int DimCallProgrammeid { get; set; }

        public virtual DimAuroraDisciplines DimAuroraHakualat { get; set; }
        public virtual DimCallProgramme DimCallProgramme { get; set; }
    }
}
