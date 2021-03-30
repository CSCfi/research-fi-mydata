using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimKnownPerson
    {
        public DimKnownPerson()
        {
            DimNameDimKnownPersonIdConfirmedIdentityNavigations = new HashSet<DimName>();
            DimNameDimKnownPersonidFormerNamesNavigations = new HashSet<DimName>();
            DimPids = new HashSet<DimPid>();
            DimUserProfiles = new HashSet<DimUserProfile>();
            DimWebLinks = new HashSet<DimWebLink>();
        }

        public int Id { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public string ResearchDescription { get; set; }

        public virtual ICollection<DimName> DimNameDimKnownPersonIdConfirmedIdentityNavigations { get; set; }
        public virtual ICollection<DimName> DimNameDimKnownPersonidFormerNamesNavigations { get; set; }
        public virtual ICollection<DimPid> DimPids { get; set; }
        public virtual ICollection<DimUserProfile> DimUserProfiles { get; set; }
        public virtual ICollection<DimWebLink> DimWebLinks { get; set; }
    }
}
