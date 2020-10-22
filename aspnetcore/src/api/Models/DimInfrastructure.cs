using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimInfrastructure
    {
        public DimInfrastructure()
        {
            BrEsfriDimInfrastructure = new HashSet<BrEsfriDimInfrastructure>();
            BrInfrastructureDimFieldOfScience = new HashSet<BrInfrastructureDimFieldOfScience>();
            BrMerilDimInfrastructure = new HashSet<BrMerilDimInfrastructure>();
            DimPid = new HashSet<DimPid>();
            FactContribution = new HashSet<FactContribution>();
            FactInfraKeywords = new HashSet<FactInfraKeywords>();
            FactUpkeep = new HashSet<FactUpkeep>();
            InverseNextInfastructure = new HashSet<DimInfrastructure>();
        }

        public int Id { get; set; }
        public int NextInfastructureId { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionSv { get; set; }
        public string DescriptionEn { get; set; }
        public string ScientificDescriptionFi { get; set; }
        public string ScientificDescriptionSv { get; set; }
        public string ScientificDescriptionEn { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public string Acronym { get; set; }
        public bool FinlandRoadmap { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimInfrastructure NextInfastructure { get; set; }
        public virtual ICollection<BrEsfriDimInfrastructure> BrEsfriDimInfrastructure { get; set; }
        public virtual ICollection<BrInfrastructureDimFieldOfScience> BrInfrastructureDimFieldOfScience { get; set; }
        public virtual ICollection<BrMerilDimInfrastructure> BrMerilDimInfrastructure { get; set; }
        public virtual ICollection<DimPid> DimPid { get; set; }
        public virtual ICollection<FactContribution> FactContribution { get; set; }
        public virtual ICollection<FactInfraKeywords> FactInfraKeywords { get; set; }
        public virtual ICollection<FactUpkeep> FactUpkeep { get; set; }
        public virtual ICollection<DimInfrastructure> InverseNextInfastructure { get; set; }
    }
}
