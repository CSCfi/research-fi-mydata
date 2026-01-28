using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimService
{
    public int Id { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int DimInfrastructureId { get; set; }

    public int StartDate { get; set; }

    public int EndDate { get; set; }

    public virtual ICollection<DimContactInformation> DimContactInformations { get; set; } = new List<DimContactInformation>();

    public virtual ICollection<DimDescriptiveItem> DimDescriptiveItems { get; set; } = new List<DimDescriptiveItem>();

    public virtual DimInfrastructure DimInfrastructure { get; set; }

    public virtual ICollection<DimPid> DimPids { get; set; } = new List<DimPid>();

    public virtual ICollection<DimWebLink> DimWebLinks { get; set; } = new List<DimWebLink>();

    public virtual DimDate EndDateNavigation { get; set; }

    public virtual ICollection<FactInfraKeyword> FactInfraKeywords { get; set; } = new List<FactInfraKeyword>();

    public virtual ICollection<FactReferencedatum> FactReferencedata { get; set; } = new List<FactReferencedatum>();

    public virtual ICollection<FactUpkeep> FactUpkeeps { get; set; } = new List<FactUpkeep>();

    public virtual DimDate StartDateNavigation { get; set; }
}
