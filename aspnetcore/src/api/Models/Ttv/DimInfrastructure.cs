using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimInfrastructure
{
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

    public virtual ICollection<DimPid> DimPids { get; set; } = new List<DimPid>();

    public virtual ICollection<FactContribution> FactContributions { get; set; } = new List<FactContribution>();

    public virtual ICollection<FactDimReferencedataFieldOfScience> FactDimReferencedataFieldOfSciences { get; set; } = new List<FactDimReferencedataFieldOfScience>();

    public virtual ICollection<FactInfraKeyword> FactInfraKeywords { get; set; } = new List<FactInfraKeyword>();

    public virtual ICollection<FactReferencedatum> FactReferencedata { get; set; } = new List<FactReferencedatum>();

    public virtual ICollection<FactRelation> FactRelationFromInfrastructures { get; set; } = new List<FactRelation>();

    public virtual ICollection<FactRelation> FactRelationToInfrastructures { get; set; } = new List<FactRelation>();

    public virtual ICollection<FactUpkeep> FactUpkeeps { get; set; } = new List<FactUpkeep>();

    public virtual ICollection<DimInfrastructure> InverseNextInfastructure { get; set; } = new List<DimInfrastructure>();

    public virtual DimInfrastructure NextInfastructure { get; set; }

    public virtual ICollection<DimEsfri> DimEsfris { get; set; } = new List<DimEsfri>();

    public virtual ICollection<DimMeril> DimMerils { get; set; } = new List<DimMeril>();
}
