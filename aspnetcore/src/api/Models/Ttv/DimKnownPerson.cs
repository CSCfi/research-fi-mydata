using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimKnownPerson
    {
        public DimKnownPerson()
        {
            DimNameDimKnownPersonIdConfirmedIdentityNavigation = new HashSet<DimName>();
            DimNameDimKnownPersonidFormerNamesNavigation = new HashSet<DimName>();
            DimPid = new HashSet<DimPid>();
            DimUserProfile = new HashSet<DimUserProfile>();
            DimWebLink = new HashSet<DimWebLink>();
        }

        public int Id { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public string ResearchDescription { get; set; }

        public virtual ICollection<DimName> DimNameDimKnownPersonIdConfirmedIdentityNavigation { get; set; }
        public virtual ICollection<DimName> DimNameDimKnownPersonidFormerNamesNavigation { get; set; }
        public virtual ICollection<DimPid> DimPid { get; set; }
        public virtual ICollection<DimUserProfile> DimUserProfile { get; set; }
        public virtual ICollection<DimWebLink> DimWebLink { get; set; }
    }
}
