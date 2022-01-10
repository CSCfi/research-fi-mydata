using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimKeyword
    {
        public DimKeyword()
        {
            BrKeywordDimFundingDecisions = new HashSet<BrKeywordDimFundingDecision>();
            BrKeywordDimKeywordDimKeywordId2Navigations = new HashSet<BrKeywordDimKeyword>();
            BrKeywordDimKeywordDimKeywords = new HashSet<BrKeywordDimKeyword>();
            BrKeywordDimPublications = new HashSet<BrKeywordDimPublication>();
            BrResearchDatasetDimKeywords = new HashSet<BrResearchDatasetDimKeyword>();
            DimResearchActivityDimKeywords = new HashSet<DimResearchActivityDimKeyword>();
            FactFieldValues = new HashSet<FactFieldValue>();
            FactInfraKeywords = new HashSet<FactInfraKeyword>();
        }

        public int Id { get; set; }
        public string Keyword { get; set; }
        public string Scheme { get; set; }
        public string ConceptUri { get; set; }
        public string SchemeUri { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int DimRegisteredDataSourceId { get; set; }
        public int DimReferencedataIdLanguageCode { get; set; }

        public virtual DimReferencedatum DimReferencedataIdLanguageCodeNavigation { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual ICollection<BrKeywordDimFundingDecision> BrKeywordDimFundingDecisions { get; set; }
        public virtual ICollection<BrKeywordDimKeyword> BrKeywordDimKeywordDimKeywordId2Navigations { get; set; }
        public virtual ICollection<BrKeywordDimKeyword> BrKeywordDimKeywordDimKeywords { get; set; }
        public virtual ICollection<BrKeywordDimPublication> BrKeywordDimPublications { get; set; }
        public virtual ICollection<BrResearchDatasetDimKeyword> BrResearchDatasetDimKeywords { get; set; }
        public virtual ICollection<DimResearchActivityDimKeyword> DimResearchActivityDimKeywords { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
        public virtual ICollection<FactInfraKeyword> FactInfraKeywords { get; set; }
    }
}
