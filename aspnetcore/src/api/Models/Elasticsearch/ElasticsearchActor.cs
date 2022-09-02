using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchActor
    {
        public ElasticsearchActor()
        {
        }

        public int actorRole { get; set; }
        public string actorRoleNameFi { get; set; }
        public string actorRoleNameSv { get; set; }
        public string actorRoleNameEn { get; set; }
    }
}
