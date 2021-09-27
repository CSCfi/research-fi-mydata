using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class GroupEmail
    {
        public GroupEmail()
        {
        }

        public Source source { get; set; }
        public List<ItemEmail> items { get; set; }
    }
}
