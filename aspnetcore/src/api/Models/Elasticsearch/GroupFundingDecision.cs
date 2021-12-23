using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class GroupFundingDecision
    {
        public GroupFundingDecision()
        {
        }

        public Source source { get; set; }
        public List<ItemFundingDecision> items { get; set; }
    }
}
