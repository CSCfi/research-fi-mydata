using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class GroupResearcherDescription
    {
        public GroupResearcherDescription()
        {
        }

        public Source source { get; set; }
        public List<ItemResearcherDescription> items { get; set; }
    }
}
