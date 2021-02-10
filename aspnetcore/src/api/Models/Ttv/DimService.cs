using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimService
    {
        public DimService()
        {
            DimPid = new HashSet<DimPid>();
            FactInfraKeywords = new HashSet<FactInfraKeywords>();
            FactUpkeep = new HashSet<FactUpkeep>();
        }

        public int Id { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionSv { get; set; }
        public string DescriptionEn { get; set; }
        public string ScientificDescriptionFi { get; set; }
        public string ScientificDescriptionSv { get; set; }
        public string ScientificDescriptionEn { get; set; }
        public string Acronym { get; set; }
        public string Type { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ICollection<DimPid> DimPid { get; set; }
        public virtual ICollection<FactInfraKeywords> FactInfraKeywords { get; set; }
        public virtual ICollection<FactUpkeep> FactUpkeep { get; set; }
    }
}
