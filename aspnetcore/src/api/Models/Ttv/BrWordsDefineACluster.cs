using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class BrWordsDefineACluster
{
    public int DimMinedWordsId { get; set; }

    public int DimWordClusterId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public string SourceId { get; set; }

    public virtual DimMinedWord DimMinedWords { get; set; }

    public virtual DimWordCluster DimWordCluster { get; set; }
}
