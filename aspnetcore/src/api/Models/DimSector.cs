using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimSector
    {
        public DimSector()
        {
            DimOrganization = new HashSet<DimOrganization>();
        }

        public int Id { get; set; }
        public string SectorId { get; set; }
        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ICollection<DimOrganization> DimOrganization { get; set; }
    }
}
