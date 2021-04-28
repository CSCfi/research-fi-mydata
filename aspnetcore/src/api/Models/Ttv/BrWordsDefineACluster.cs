using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrWordsDefineACluster
    {
        public int DimMinedWordsId { get; set; }
        public int DimWordClusterId { get; set; }

        public virtual DimMinedWord DimMinedWords { get; set; }
        public virtual DimWordCluster DimWordCluster { get; set; }
    }
}
