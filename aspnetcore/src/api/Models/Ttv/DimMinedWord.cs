using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimMinedWord
    {
        public DimMinedWord()
        {
            BrWordsDefineAClusters = new HashSet<BrWordsDefineACluster>();
        }

        public int Id { get; set; }
        public string Word { get; set; }

        public virtual ICollection<BrWordsDefineACluster> BrWordsDefineAClusters { get; set; }
    }
}
