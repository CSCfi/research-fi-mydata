using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimResearchActivity
{
    public int Id { get; set; }

    public string LocalIdentifier { get; set; }

    public bool InternationalCollaboration { get; set; }

    public string NameFi { get; set; }

    public string NameSv { get; set; }

    public string NameEn { get; set; }

    public string NameUnd { get; set; }

    public string DescriptionFi { get; set; }

    public string DescriptionEn { get; set; }

    public string DescriptionSv { get; set; }

    public string IndentifierlessTargetOrg { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int DimStartDate { get; set; }

    public int DimEndDate { get; set; }

    public int? DimCountryCode { get; set; }

    public int? DimPublicationChannelId { get; set; }

    public int DimEventId { get; set; }

    public int DimOrganizationId { get; set; }

    public int DimRegisteredDataSourceId { get; set; }

    public virtual DimGeo DimCountryCodeNavigation { get; set; }

    public virtual DimDate DimEndDateNavigation { get; set; }

    public virtual DimEvent DimEvent { get; set; }

    public virtual DimOrganization DimOrganization { get; set; }

    public virtual ICollection<DimPid> DimPids { get; set; } = new List<DimPid>();

    public virtual DimPublicationChannel DimPublicationChannel { get; set; }

    public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }

    public virtual ICollection<DimResearchActivityDimKeyword> DimResearchActivityDimKeywords { get; set; } = new List<DimResearchActivityDimKeyword>();

    public virtual DimDate DimStartDateNavigation { get; set; }

    public virtual ICollection<DimWebLink> DimWebLinks { get; set; } = new List<DimWebLink>();

    public virtual ICollection<FactContribution> FactContributions { get; set; } = new List<FactContribution>();

    public virtual ICollection<FactFieldValue> FactFieldValues { get; set; } = new List<FactFieldValue>();

    public virtual ICollection<FactReferencedatum> FactReferencedata { get; set; } = new List<FactReferencedatum>();
}
