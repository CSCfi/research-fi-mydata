using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class GroupKeyword
    {
        public GroupKeyword()
        {
        }

        public Source source { get; set; }
        public List<ItemKeyword> items { get; set; }
    }
}
