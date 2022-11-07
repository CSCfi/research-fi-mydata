using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimInfrastructure
    {
        public DimInfrastructure()
        {
            BrEsfriDimInfrastructures = new HashSet<BrEsfriDimInfrastructure>();
            BrInfrastructureDimFieldOfSciences = new HashSet<BrInfrastructureDimFieldOfScience>();
            BrMerilDimInfrastructures = new HashSet<BrMerilDimInfrastructure>();
            DimPids = new HashSet<DimPid>();
            FactContributions = new HashSet<FactContribution>();
            FactInfraKeywords = new HashSet<FactInfraKeyword>();
            FactUpkeeps = new HashSet<FactUpkeep>();
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
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public string Acronym { get; set; }
        public bool FinlandRoadmap { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public string Urn { get; set; }
        public string ScientificDescriptionFi { get; set; }
        public string ScientificDescriptionSv { get; set; }
        public string ScientificDescriptionEn { get; set; }

        public virtual DimInfrastructure NextInfastructure { get; set; }
        public virtual ICollection<BrEsfriDimInfrastructure> BrEsfriDimInfrastructures { get; set; }
        public virtual ICollection<BrInfrastructureDimFieldOfScience> BrInfrastructureDimFieldOfSciences { get; set; }
        public virtual ICollection<BrMerilDimInfrastructure> BrMerilDimInfrastructures { get; set; }
        public virtual ICollection<DimPid> DimPids { get; set; }
        public virtual ICollection<FactContribution> FactContributions { get; set; }
        public virtual ICollection<FactInfraKeyword> FactInfraKeywords { get; set; }
        public virtual ICollection<FactUpkeep> FactUpkeeps { get; set; }
        public virtual ICollection<DimInfrastructure> InverseNextInfastructure { get; set; }
    }
}
