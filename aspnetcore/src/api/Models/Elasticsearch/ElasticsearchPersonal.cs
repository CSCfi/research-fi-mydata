using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchPersonal
    {
        public ElasticsearchPersonal()
        {
            names = new List<ElasticsearchName>();
            otherNames = new List<ElasticsearchName>();
            emails = new List<ElasticsearchEmail>();
            telephoneNumbers = new List<ElasticsearchTelephoneNumber>();
            webLinks = new List<ElasticsearchWebLink>();
            keywords = new List<ElasticsearchKeyword>();
            fieldOfSciences = new List<ElasticsearchFieldOfScience>();
            researcherDescriptions = new List<ElasticsearchResearcherDescription>();
            externalIdentifiers = new List<ElasticsearchExternalIdentifier>();
        }

        public List<ElasticsearchName> names { get; set; }
        public List<ElasticsearchName> otherNames { get; set; }
        public List<ElasticsearchEmail> emails { get; set; }
        public List<ElasticsearchTelephoneNumber> telephoneNumbers { get; set; }
        public List<ElasticsearchWebLink> webLinks { get; set; }
        public List<ElasticsearchKeyword> keywords { get; set; }
        public List<ElasticsearchFieldOfScience> fieldOfSciences { get; set; }
        public List<ElasticsearchResearcherDescription> researcherDescriptions { get; set; }
        public List<ElasticsearchExternalIdentifier> externalIdentifiers { get; set; }
    }
}

