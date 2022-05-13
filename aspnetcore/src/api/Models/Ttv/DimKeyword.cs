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
            BrKeywordDimPublications = new HashSet<BrKeywordDimPublication>();
            FactFieldValues = new HashSet<FactFieldValue>();
            FactInfraKeywords = new HashSet<FactInfraKeyword>();
            InverseDimKeywordCloseMatchNavigation = new HashSet<DimKeyword>();
            InverseDimKeywordLanguageVariantNavigation = new HashSet<DimKeyword>();
            InverseDimKeywordRelatedNavigation = new HashSet<DimKeyword>();
        }

        public int Id { get; set; }
        public string Keyword { get; set; }
        public string Scheme { get; set; }
        public string Language { get; set; }
        public int? DimKeywordRelated { get; set; }
        public int? DimKeywordCloseMatch { get; set; }
        public int? DimKeywordLanguageVariant { get; set; }
        public string ConceptUri { get; set; }
        public string SchemeUri { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int DimRegisteredDataSourceId { get; set; }

        public virtual DimKeyword DimKeywordCloseMatchNavigation { get; set; }
        public virtual DimKeyword DimKeywordLanguageVariantNavigation { get; set; }
        public virtual DimKeyword DimKeywordRelatedNavigation { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual ICollection<BrKeywordDimFundingDecision> BrKeywordDimFundingDecisions { get; set; }
        public virtual ICollection<BrKeywordDimPublication> BrKeywordDimPublications { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
        public virtual ICollection<FactInfraKeyword> FactInfraKeywords { get; set; }
        public virtual ICollection<DimKeyword> InverseDimKeywordCloseMatchNavigation { get; set; }
        public virtual ICollection<DimKeyword> InverseDimKeywordLanguageVariantNavigation { get; set; }
        public virtual ICollection<DimKeyword> InverseDimKeywordRelatedNavigation { get; set; }
    }
}
