using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class BrOrganizationsFundCallProgrammes
    {
        public int DimOrganizationid { get; set; }
        public int DimCallProgrammeid { get; set; }

        public virtual DimCallProgramme DimCallProgramme { get; set; }
        public virtual DimOrganization DimOrganization { get; set; }
    }
}
