using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class GroupEducation
    {
        public GroupEducation()
        {
        }

        public Source source { get; set; }
        public List<ItemEducation> items { get; set; }
    }
}
