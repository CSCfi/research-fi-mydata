using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class BrPredecessorOrganization
    {
        public int DimOrganizationid { get; set; }
        public int DimOrganizationid2 { get; set; }

        public virtual DimOrganization DimOrganization { get; set; }
        public virtual DimOrganization DimOrganizationid2Navigation { get; set; }
    }
}
