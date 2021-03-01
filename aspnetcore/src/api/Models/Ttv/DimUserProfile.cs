using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimUserProfile
    {
        public DimUserProfile()
        {
            DimFieldDisplaySettings = new HashSet<DimFieldDisplaySettings>();
            FactFieldValues = new HashSet<FactFieldValues>();
        }

        public int Id { get; set; }
        public int DimKnownPersonId { get; set; }
        public bool AllowAllSubscriptions { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimKnownPerson DimKnownPerson { get; set; }
        public virtual ICollection<DimFieldDisplaySettings> DimFieldDisplaySettings { get; set; }
        public virtual ICollection<FactFieldValues> FactFieldValues { get; set; }
    }
}
