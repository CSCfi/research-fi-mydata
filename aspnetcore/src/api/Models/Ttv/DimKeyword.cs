using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimKeyword
    {
        public DimKeyword()
        {
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
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimKeyword DimKeywordCloseMatchNavigation { get; set; }
        public virtual DimKeyword DimKeywordLanguageVariantNavigation { get; set; }
        public virtual DimKeyword DimKeywordRelatedNavigation { get; set; }
        public virtual ICollection<DimKeyword> InverseDimKeywordCloseMatchNavigation { get; set; }
        public virtual ICollection<DimKeyword> InverseDimKeywordLanguageVariantNavigation { get; set; }
        public virtual ICollection<DimKeyword> InverseDimKeywordRelatedNavigation { get; set; }
    }
}
