using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class GroupExternalIdentifier
    {
        public GroupExternalIdentifier()
        {
        }

        public Source source { get; set; }
        public List<ItemExternalIdentifier> items { get; set; }
    }
}
