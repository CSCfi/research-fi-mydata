using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimAuroraDisciplines
    {
        public DimAuroraDisciplines()
        {
            BrDimAuroraHakualatDimCallProgramme = new HashSet<BrDimAuroraHakualatDimCallProgramme>();
        }

        public int Id { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ICollection<BrDimAuroraHakualatDimCallProgramme> BrDimAuroraHakualatDimCallProgramme { get; set; }
    }
}
