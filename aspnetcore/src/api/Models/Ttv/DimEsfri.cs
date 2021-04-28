using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimEsfri
    {
        public DimEsfri()
        {
            BrEsfriDimInfrastructures = new HashSet<BrEsfriDimInfrastructure>();
        }

        public int Id { get; set; }
        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Created { get; set; }

        public virtual ICollection<BrEsfriDimInfrastructure> BrEsfriDimInfrastructures { get; set; }
    }
}
