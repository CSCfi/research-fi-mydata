using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class GroupWebLink
    {
        public GroupWebLink()
        {
        }

        public Source source { get; set; }
        public List<ItemWebLink> items { get; set; }
    }
}
