using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchActivity
    {
        public ElasticsearchActivity()
        {
            affiliations = new List<ElasticsearchAffiliation>();
            educations = new List<ElasticsearchEducation>();
            publications = new List<ElasticsearchPublication>();
            fundingDecisions = new List<ElasticsearchFundingDecision>();
        }

        public List<ElasticsearchAffiliation> affiliations { get; set; }
        public List<ElasticsearchEducation> educations { get; set; }
        public List<ElasticsearchPublication> publications { get; set; }
        public List<ElasticsearchFundingDecision> fundingDecisions { get; set; }
    }
}
