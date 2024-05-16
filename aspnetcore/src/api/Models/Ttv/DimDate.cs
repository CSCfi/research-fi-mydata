using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimDate
{
    public int Id { get; set; }

    public int Year { get; set; }

    public int Month { get; set; }

    public int Day { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public virtual ICollection<DimAffiliation> DimAffiliationEndDateNavigations { get; set; } = new List<DimAffiliation>();

    public virtual ICollection<DimAffiliation> DimAffiliationStartDateNavigations { get; set; } = new List<DimAffiliation>();

    public virtual ICollection<DimCallDecision> DimCallDecisions { get; set; } = new List<DimCallDecision>();

    public virtual ICollection<DimCallProgramme> DimCallProgrammeDimDateIdDueNavigations { get; set; } = new List<DimCallProgramme>();

    public virtual ICollection<DimCallProgramme> DimCallProgrammeDimDateIdOpenNavigations { get; set; } = new List<DimCallProgramme>();

    public virtual ICollection<DimEducation> DimEducationDimEndDateNavigations { get; set; } = new List<DimEducation>();

    public virtual ICollection<DimEducation> DimEducationDimStartDateNavigations { get; set; } = new List<DimEducation>();

    public virtual ICollection<DimEvent> DimEventDimDateIdEndDateNavigations { get; set; } = new List<DimEvent>();

    public virtual ICollection<DimEvent> DimEventDimDateIdStartDateNavigations { get; set; } = new List<DimEvent>();

    public virtual ICollection<DimFundingDecision> DimFundingDecisionDimDateIdApprovalNavigations { get; set; } = new List<DimFundingDecision>();

    public virtual ICollection<DimFundingDecision> DimFundingDecisionDimDateIdEndNavigations { get; set; } = new List<DimFundingDecision>();

    public virtual ICollection<DimFundingDecision> DimFundingDecisionDimDateIdStartNavigations { get; set; } = new List<DimFundingDecision>();

    public virtual ICollection<DimProfileOnlyFundingDecision> DimProfileOnlyFundingDecisionDimDateIdApprovalNavigations { get; set; } = new List<DimProfileOnlyFundingDecision>();

    public virtual ICollection<DimProfileOnlyFundingDecision> DimProfileOnlyFundingDecisionDimDateIdEndNavigations { get; set; } = new List<DimProfileOnlyFundingDecision>();

    public virtual ICollection<DimProfileOnlyFundingDecision> DimProfileOnlyFundingDecisionDimDateIdStartNavigations { get; set; } = new List<DimProfileOnlyFundingDecision>();

    public virtual ICollection<DimProfileOnlyResearchActivity> DimProfileOnlyResearchActivityDimDateIdEndNavigations { get; set; } = new List<DimProfileOnlyResearchActivity>();

    public virtual ICollection<DimProfileOnlyResearchActivity> DimProfileOnlyResearchActivityDimDateIdStartNavigations { get; set; } = new List<DimProfileOnlyResearchActivity>();

    public virtual ICollection<DimResearchActivity> DimResearchActivityDimEndDateNavigations { get; set; } = new List<DimResearchActivity>();

    public virtual ICollection<DimResearchActivity> DimResearchActivityDimStartDateNavigations { get; set; } = new List<DimResearchActivity>();

    public virtual ICollection<DimResearchProject> DimResearchProjectEndDateNavigations { get; set; } = new List<DimResearchProject>();

    public virtual ICollection<DimResearchProject> DimResearchProjectStartDateNavigations { get; set; } = new List<DimResearchProject>();

    public virtual ICollection<DimResearcherToResearchCommunity> DimResearcherToResearchCommunityEndDateNavigations { get; set; } = new List<DimResearcherToResearchCommunity>();

    public virtual ICollection<DimResearcherToResearchCommunity> DimResearcherToResearchCommunityStartDateNavigations { get; set; } = new List<DimResearcherToResearchCommunity>();

    public virtual ICollection<FactContribution> FactContributions { get; set; } = new List<FactContribution>();

    public virtual ICollection<FactUpkeep> FactUpkeepDimDateIdEndNavigations { get; set; } = new List<FactUpkeep>();

    public virtual ICollection<FactUpkeep> FactUpkeepDimDateIdStartNavigations { get; set; } = new List<FactUpkeep>();
}
