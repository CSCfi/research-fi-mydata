using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class GroupOtherName
    {
        public GroupOtherName()
        {
        }

        public Source source { get; set; }
        public List<ItemName> items { get; set; }
    }
}
