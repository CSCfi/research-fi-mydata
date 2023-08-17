using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimKnownPerson
{
    public int Id { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public string SourceProjectId { get; set; }

    public int? DimRegisteredDataSourceId { get; set; }

    public virtual ICollection<DimAffiliation> DimAffiliations { get; set; } = new List<DimAffiliation>();

    public virtual ICollection<DimCompetence> DimCompetences { get; set; } = new List<DimCompetence>();

    public virtual ICollection<DimEducation> DimEducations { get; set; } = new List<DimEducation>();

    public virtual ICollection<DimEmailAddrress> DimEmailAddrresses { get; set; } = new List<DimEmailAddrress>();

    public virtual ICollection<DimName> DimNames { get; set; } = new List<DimName>();

    public virtual ICollection<DimPid> DimPids { get; set; } = new List<DimPid>();

    public virtual ICollection<DimProfileOnlyPublication> DimProfileOnlyPublications { get; set; } = new List<DimProfileOnlyPublication>();

    public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }

    public virtual ICollection<DimResearcherDescription> DimResearcherDescriptions { get; set; } = new List<DimResearcherDescription>();

    public virtual ICollection<DimResearcherToResearchCommunity> DimResearcherToResearchCommunities { get; set; } = new List<DimResearcherToResearchCommunity>();

    public virtual ICollection<DimTelephoneNumber> DimTelephoneNumbers { get; set; } = new List<DimTelephoneNumber>();

    public virtual ICollection<DimUserProfile> DimUserProfiles { get; set; } = new List<DimUserProfile>();

    public virtual ICollection<DimWebLink> DimWebLinks { get; set; } = new List<DimWebLink>();

    public virtual ICollection<FactDimReferencedataFieldOfScience> FactDimReferencedataFieldOfSciences { get; set; } = new List<FactDimReferencedataFieldOfScience>();
}
