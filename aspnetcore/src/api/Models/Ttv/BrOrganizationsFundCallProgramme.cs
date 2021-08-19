using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrOrganizationsFundCallProgramme
    {
        public int DimOrganizationid { get; set; }
        public int DimCallProgrammeid { get; set; }

        public virtual DimCallProgramme DimCallProgramme { get; set; }
        public virtual DimOrganization DimOrganization { get; set; }
    }
}
