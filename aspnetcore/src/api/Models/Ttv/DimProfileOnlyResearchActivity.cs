using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimProfileOnlyResearchActivity
{
    public int Id { get; set; }

    public int DimDateIdStart { get; set; }

    public int DimDateIdEnd { get; set; }

    public int? DimGeoIdCountry { get; set; }

    public int DimOrganizationId { get; set; }

    public int DimEventId { get; set; }

    public string LocalIdentifier { get; set; }

    public string OrcidWorkType { get; set; }

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

    public int DimRegisteredDataSourceId { get; set; }

    public virtual DimDate DimDateIdEndNavigation { get; set; }

    public virtual DimDate DimDateIdStartNavigation { get; set; }

    public virtual DimEvent DimEvent { get; set; }

    public virtual DimGeo DimGeoIdCountryNavigation { get; set; }

    public virtual DimOrganization DimOrganization { get; set; }

    public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }

    public virtual ICollection<DimWebLink> DimWebLinks { get; set; } = new List<DimWebLink>();

    public virtual ICollection<FactFieldValue> FactFieldValues { get; set; } = new List<FactFieldValue>();
}
