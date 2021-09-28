using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class GroupFieldOfScience
    {
        public GroupFieldOfScience()
        {
        }

        public Source source { get; set; }
        public List<ItemFieldOfScience> items { get; set; }
    }
}
