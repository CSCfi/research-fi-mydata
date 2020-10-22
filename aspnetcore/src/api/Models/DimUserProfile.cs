using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimUserProfile
    {
        public DimUserProfile()
        {
            DimFieldDisplaySettings = new HashSet<DimFieldDisplaySettings>();
            AllowAllSubscriptions = false;
            SourceId = "ORCID";
            SourceDescription = "Researcher profile API";
            Created = DateTime.Now;
        }

        public int Id { get; set; }
        public int DimKnownPersonId { get; set; }
        public bool AllowAllSubscriptions { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimKnownPerson DimKnownPerson { get; set; }
        public virtual ICollection<DimFieldDisplaySettings> DimFieldDisplaySettings { get; set; }
    }
}
