using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrSuccessorOrganization
    {
        public int DimOrganizationid { get; set; }
        public int DimOrganizationid2 { get; set; }

        public virtual DimOrganization DimOrganization { get; set; }
        public virtual DimOrganization DimOrganizationid2Navigation { get; set; }
    }
}
