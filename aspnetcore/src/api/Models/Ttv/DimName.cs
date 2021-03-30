using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimName
    {
        public DimName()
        {
            FactFieldValues = new HashSet<FactFieldValue>();
        }

        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstNames { get; set; }
        public string FullName { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int DimKnownPersonIdConfirmedIdentity { get; set; }
        public int DimKnownPersonidFormerNames { get; set; }

        public virtual DimKnownPerson DimKnownPersonIdConfirmedIdentityNavigation { get; set; }
        public virtual DimKnownPerson DimKnownPersonidFormerNamesNavigation { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
    }
}
