using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class Personal
    {
        public Personal()
        {
            nameGroups = new List<GroupName> ();
            otherNameGroups = new List<GroupOtherName>();
            emailGroups = new List<GroupEmail>();
        }

        public List<GroupName> nameGroups { get; set; }
        public List<GroupOtherName> otherNameGroups { get; set; }
        public List<GroupEmail> emailGroups { get; set; }
    }
}

