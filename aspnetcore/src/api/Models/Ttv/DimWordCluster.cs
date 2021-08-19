using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimWordCluster
    {
        public DimWordCluster()
        {
            BrWordsDefineAClusters = new HashSet<BrWordsDefineACluster>();
            DimFundingDecisions = new HashSet<DimFundingDecision>();
        }

        public int Id { get; set; }

        public virtual ICollection<BrWordsDefineACluster> BrWordsDefineAClusters { get; set; }
        public virtual ICollection<DimFundingDecision> DimFundingDecisions { get; set; }
    }
}
