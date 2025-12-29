using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimKeyword
{
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

    public virtual ICollection<FactFieldValue> FactFieldValues { get; set; } = new List<FactFieldValue>();

    public virtual ICollection<FactInfraKeyword> FactInfraKeywords { get; set; } = new List<FactInfraKeyword>();

    public virtual ICollection<FactKeyword> FactKeywords { get; set; } = new List<FactKeyword>();

    public virtual ICollection<DimKeyword> InverseDimKeywordCloseMatchNavigation { get; set; } = new List<DimKeyword>();

    public virtual ICollection<DimKeyword> InverseDimKeywordLanguageVariantNavigation { get; set; } = new List<DimKeyword>();

    public virtual ICollection<DimKeyword> InverseDimKeywordRelatedNavigation { get; set; } = new List<DimKeyword>();

    public virtual ICollection<DimFundingDecision> DimFundingDecisions { get; set; } = new List<DimFundingDecision>();

    public virtual ICollection<DimPublication> DimPublications { get; set; } = new List<DimPublication>();
}
