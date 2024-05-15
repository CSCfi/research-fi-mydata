using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimName
{
    public int Id { get; set; }

    public string LastName { get; set; }

    public string FirstNames { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int DimKnownPersonIdConfirmedIdentity { get; set; }

    public string SourceProjectId { get; set; }

    public string FullName { get; set; }

    public int DimRegisteredDataSourceId { get; set; }

    public virtual ICollection<BrParticipatesInFundingGroup> BrParticipatesInFundingGroups { get; set; } = new List<BrParticipatesInFundingGroup>();

    public virtual ICollection<DimFundingDecision> DimFundingDecisions { get; set; } = new List<DimFundingDecision>();

    public virtual DimKnownPerson DimKnownPersonIdConfirmedIdentityNavigation { get; set; }

    public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }

    public virtual ICollection<DimResearchProject> DimResearchProjects { get; set; } = new List<DimResearchProject>();

    public virtual ICollection<FactContribution> FactContributions { get; set; } = new List<FactContribution>();

    public virtual ICollection<FactFieldValue> FactFieldValues { get; set; } = new List<FactFieldValue>();
}
