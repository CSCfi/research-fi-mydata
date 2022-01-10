using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class GroupResearchDataset
    {
        public GroupResearchDataset()
        {
        }

        public Source source { get; set; }
        public List<ItemResearchDataset> items { get; set; }
    }
}
