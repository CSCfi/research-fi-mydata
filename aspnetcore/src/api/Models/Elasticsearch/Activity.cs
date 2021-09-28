using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class Activity
    {
        public Activity()
        {
            affiliationGroups = new List<GroupAffiliation>();
            educationGroups = new List<GroupEducation>();
            publicationGroups = new List<GroupPublication>();
        }

        public List<GroupAffiliation> affiliationGroups { get; set; }
        public List<GroupEducation> educationGroups { get; set; }
        public List<GroupPublication> publicationGroups { get; set; }
    }
}
