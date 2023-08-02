using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimMinedWord
{
    public int Id { get; set; }

    public string Word { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public string SourceId { get; set; }

    public virtual ICollection<BrWordsDefineACluster> BrWordsDefineAClusters { get; set; } = new List<BrWordsDefineACluster>();
}
