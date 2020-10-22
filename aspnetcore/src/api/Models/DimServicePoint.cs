using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimServicePoint
    {
        public DimServicePoint()
        {
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
        public string VisitingAddress { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string LinkAdditionalInfo { get; set; }
        public string LinkAccessPolicy { get; set; }
        public string LinkInternationalInfra { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ICollection<FactInfraKeywords> FactInfraKeywords { get; set; }
        public virtual ICollection<FactUpkeep> FactUpkeep { get; set; }
    }
}
