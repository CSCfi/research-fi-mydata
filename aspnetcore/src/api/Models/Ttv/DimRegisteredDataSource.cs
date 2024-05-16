using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimRegisteredDataSource
{
    public int Id { get; set; }

    public int DimOrganizationId { get; set; }

    public string Name { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Modified { get; set; }

    public DateTime? Created { get; set; }

    public virtual ICollection<DimAffiliation> DimAffiliations { get; set; } = new List<DimAffiliation>();

    public virtual ICollection<DimCallProgramme> DimCallProgrammes { get; set; } = new List<DimCallProgramme>();

    public virtual ICollection<DimCompetence> DimCompetences { get; set; } = new List<DimCompetence>();

    public virtual ICollection<DimEducation> DimEducations { get; set; } = new List<DimEducation>();

    public virtual ICollection<DimEmailAddrress> DimEmailAddrresses { get; set; } = new List<DimEmailAddrress>();

    public virtual ICollection<DimEvent> DimEvents { get; set; } = new List<DimEvent>();

    public virtual ICollection<DimFundingDecision> DimFundingDecisions { get; set; } = new List<DimFundingDecision>();

    public virtual ICollection<DimKeyword> DimKeywords { get; set; } = new List<DimKeyword>();

    public virtual ICollection<DimKnownPerson> DimKnownPeople { get; set; } = new List<DimKnownPerson>();

    public virtual ICollection<DimName> DimNames { get; set; } = new List<DimName>();

    public virtual DimOrganization DimOrganization { get; set; }

    public virtual ICollection<DimOrganization> DimOrganizations { get; set; } = new List<DimOrganization>();

    public virtual ICollection<DimProfileOnlyDataset> DimProfileOnlyDatasets { get; set; } = new List<DimProfileOnlyDataset>();

    public virtual ICollection<DimProfileOnlyFundingDecision> DimProfileOnlyFundingDecisions { get; set; } = new List<DimProfileOnlyFundingDecision>();

    public virtual ICollection<DimProfileOnlyPublication> DimProfileOnlyPublications { get; set; } = new List<DimProfileOnlyPublication>();

    public virtual ICollection<DimProfileOnlyResearchActivity> DimProfileOnlyResearchActivities { get; set; } = new List<DimProfileOnlyResearchActivity>();

    public virtual ICollection<DimPublication> DimPublications { get; set; } = new List<DimPublication>();

    public virtual ICollection<DimResearchActivity> DimResearchActivities { get; set; } = new List<DimResearchActivity>();

    public virtual ICollection<DimResearchCommunity> DimResearchCommunities { get; set; } = new List<DimResearchCommunity>();

    public virtual ICollection<DimResearchDataset> DimResearchDatasets { get; set; } = new List<DimResearchDataset>();

    public virtual ICollection<DimResearchProject> DimResearchProjects { get; set; } = new List<DimResearchProject>();

    public virtual ICollection<DimResearcherDescription> DimResearcherDescriptions { get; set; } = new List<DimResearcherDescription>();

    public virtual ICollection<DimResearcherToResearchCommunity> DimResearcherToResearchCommunities { get; set; } = new List<DimResearcherToResearchCommunity>();

    public virtual ICollection<DimTelephoneNumber> DimTelephoneNumbers { get; set; } = new List<DimTelephoneNumber>();

    public virtual ICollection<DimWebLink> DimWebLinks { get; set; } = new List<DimWebLink>();

    public virtual ICollection<FactFieldValue> FactFieldValues { get; set; } = new List<FactFieldValue>();

    public virtual ICollection<DimFieldDisplaySetting> DimFieldDisplaySettings { get; set; } = new List<DimFieldDisplaySetting>();
}
