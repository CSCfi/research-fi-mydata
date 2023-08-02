using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimWordCluster
{
    public int Id { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public string SourceId { get; set; }

    public virtual ICollection<BrWordClusterDimFundingDecision> BrWordClusterDimFundingDecisions { get; set; } = new List<BrWordClusterDimFundingDecision>();

    public virtual ICollection<BrWordsDefineACluster> BrWordsDefineAClusters { get; set; } = new List<BrWordsDefineACluster>();
}
