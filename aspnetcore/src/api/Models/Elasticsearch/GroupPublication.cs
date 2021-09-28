using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class GroupPublication
    {
        public GroupPublication()
        {
        }

        public Source source { get; set; }
        public List<ItemPublication> items { get; set; }
    }
}
