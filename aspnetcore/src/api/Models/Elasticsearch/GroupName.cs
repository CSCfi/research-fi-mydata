using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class GroupName
    {
        public GroupName()
        {
        }

        public Source source { get; set; }
        public List<ItemName> items { get; set; }
    }
}
