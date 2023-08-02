using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimResearchCommunity
{
    public int Id { get; set; }

    public string Acronym { get; set; }

    public string NameFi { get; set; }

    public string NameSv { get; set; }

    public string NameEn { get; set; }

    public string NameUnd { get; set; }

    public string DescriptionFi { get; set; }

    public string DescriptionEn { get; set; }

    public string DescriptionSv { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int DimRegisteredDataSourceId { get; set; }

    public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }

    public virtual ICollection<DimResearcherToResearchCommunity> DimResearcherToResearchCommunities { get; set; } = new List<DimResearcherToResearchCommunity>();

    public virtual ICollection<DimWebLink> DimWebLinks { get; set; } = new List<DimWebLink>();

    public virtual ICollection<FactContribution> FactContributions { get; set; } = new List<FactContribution>();

    public virtual ICollection<FactFieldValue> FactFieldValues { get; set; } = new List<FactFieldValue>();
}
