using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class FactKeyword
{
    public int DimKeywordId { get; set; }

    public int DimResearchProjectId { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }
}
