using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimResearchDataCatalog
{
    public int Id { get; set; }

    public string NameFi { get; set; }

    public string NameSv { get; set; }

    public string NameEn { get; set; }

    public string DescriptionFi { get; set; }

    public string DescriptionSv { get; set; }

    public string DescriptionEn { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public virtual ICollection<DimPid> DimPids { get; set; } = new List<DimPid>();

    public virtual ICollection<DimResearchDataset> DimResearchDatasets { get; set; } = new List<DimResearchDataset>();

    public virtual ICollection<DimWebLink> DimWebLinks { get; set; } = new List<DimWebLink>();

    public virtual ICollection<FactContribution> FactContributions { get; set; } = new List<FactContribution>();
}
