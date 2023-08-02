using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class FactInfraKeyword
{
    public int DimKeywordId { get; set; }

    public int DimServiceId { get; set; }

    public int DimServicePointId { get; set; }

    public int DimInfrastructureId { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public virtual DimInfrastructure DimInfrastructure { get; set; }

    public virtual DimKeyword DimKeyword { get; set; }

    public virtual DimService DimService { get; set; }

    public virtual DimServicePoint DimServicePoint { get; set; }
}
