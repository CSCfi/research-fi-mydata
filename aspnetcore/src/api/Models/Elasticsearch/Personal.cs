using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class Personal
    {
        public Personal()
        {
            nameGroups = new List<GroupName>();
            otherNameGroups = new List<GroupOtherName>();
            emailGroups = new List<GroupEmail>();
            telephoneNumberGroups = new List<GroupTelephoneNumber>();
            webLinkGroups = new List<GroupWebLink>();
            keywordGroups = new List<GroupKeyword>();
            fieldOfScienceGroups = new List<GroupFieldOfScience>();
            researcherDescriptionGroups = new List<GroupResearcherDescription>();
            externalIdentifierGroups = new List<GroupExternalIdentifier>();
        }

        public List<GroupName> nameGroups { get; set; }
        public List<GroupOtherName> otherNameGroups { get; set; }
        public List<GroupEmail> emailGroups { get; set; }
        public List<GroupTelephoneNumber> telephoneNumberGroups { get; set; }
        public List<GroupWebLink> webLinkGroups { get; set; }
        public List<GroupKeyword> keywordGroups { get; set; }
        public List<GroupFieldOfScience> fieldOfScienceGroups { get; set; }
        public List<GroupResearcherDescription> researcherDescriptionGroups { get; set; }
        public List<GroupExternalIdentifier> externalIdentifierGroups { get; set; }
    }
}

