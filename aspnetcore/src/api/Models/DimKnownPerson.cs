using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimKnownPerson
    {
        public DimKnownPerson()
        {
            DimPid = new HashSet<DimPid>();
            SourceId = "ORCID";
            SourceDescription = "Researcher profile API";
            Created = DateTime.Now;
        }

        public int Id { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ICollection<DimPid> DimPid { get; set; }
    }
}
