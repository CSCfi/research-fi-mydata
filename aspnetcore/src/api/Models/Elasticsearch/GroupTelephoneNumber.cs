using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class GroupTelephoneNumber
    {
        public GroupTelephoneNumber()
        {
        }

        public Source source { get; set; }
        public List<ItemTelephoneNumber> items { get; set; }
    }
}
