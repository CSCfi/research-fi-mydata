using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class GroupAffiliation
    {
        public GroupAffiliation()
        {
        }

        public Source source { get; set; }
        public List<ItemAffiliation> items { get; set; }
    }
}
