using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimDate
    {
        public DimDate()
        {
            DimAffiliationEndDateNavigations = new HashSet<DimAffiliation>();
            DimAffiliationStartDateNavigations = new HashSet<DimAffiliation>();
            DimCallProgrammeDimDateIdDueNavigations = new HashSet<DimCallProgramme>();
            DimCallProgrammeDimDateIdOpenNavigations = new HashSet<DimCallProgramme>();
            DimEducationDimEndDateNavigations = new HashSet<DimEducation>();
            DimEducationDimStartDateNavigations = new HashSet<DimEducation>();
            DimEventDimDateIdEndDateNavigations = new HashSet<DimEvent>();
            DimEventDimDateIdStartDateNavigations = new HashSet<DimEvent>();
            DimFundingDecisionDimDateIdApprovalNavigations = new HashSet<DimFundingDecision>();
            DimFundingDecisionDimDateIdEndNavigations = new HashSet<DimFundingDecision>();
            DimFundingDecisionDimDateIdStartNavigations = new HashSet<DimFundingDecision>();
            DimResearchActivityDimEndDateNavigations = new HashSet<DimResearchActivity>();
            DimResearchActivityDimStartDateNavigations = new HashSet<DimResearchActivity>();
            DimResearcherToResearchCommunityEndDateNavigations = new HashSet<DimResearcherToResearchCommunity>();
            DimResearcherToResearchCommunityStartDateNavigations = new HashSet<DimResearcherToResearchCommunity>();
            FactContributions = new HashSet<FactContribution>();
            FactUpkeepDimDateIdEndNavigations = new HashSet<FactUpkeep>();
            FactUpkeepDimDateIdStartNavigations = new HashSet<FactUpkeep>();
        }

        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ICollection<DimAffiliation> DimAffiliationEndDateNavigations { get; set; }
        public virtual ICollection<DimAffiliation> DimAffiliationStartDateNavigations { get; set; }
        public virtual ICollection<DimCallProgramme> DimCallProgrammeDimDateIdDueNavigations { get; set; }
        public virtual ICollection<DimCallProgramme> DimCallProgrammeDimDateIdOpenNavigations { get; set; }
        public virtual ICollection<DimEducation> DimEducationDimEndDateNavigations { get; set; }
        public virtual ICollection<DimEducation> DimEducationDimStartDateNavigations { get; set; }
        public virtual ICollection<DimEvent> DimEventDimDateIdEndDateNavigations { get; set; }
        public virtual ICollection<DimEvent> DimEventDimDateIdStartDateNavigations { get; set; }
        public virtual ICollection<DimFundingDecision> DimFundingDecisionDimDateIdApprovalNavigations { get; set; }
        public virtual ICollection<DimFundingDecision> DimFundingDecisionDimDateIdEndNavigations { get; set; }
        public virtual ICollection<DimFundingDecision> DimFundingDecisionDimDateIdStartNavigations { get; set; }
        public virtual ICollection<DimResearchActivity> DimResearchActivityDimEndDateNavigations { get; set; }
        public virtual ICollection<DimResearchActivity> DimResearchActivityDimStartDateNavigations { get; set; }
        public virtual ICollection<DimResearcherToResearchCommunity> DimResearcherToResearchCommunityEndDateNavigations { get; set; }
        public virtual ICollection<DimResearcherToResearchCommunity> DimResearcherToResearchCommunityStartDateNavigations { get; set; }
        public virtual ICollection<FactContribution> FactContributions { get; set; }
        public virtual ICollection<FactUpkeep> FactUpkeepDimDateIdEndNavigations { get; set; }
        public virtual ICollection<FactUpkeep> FactUpkeepDimDateIdStartNavigations { get; set; }
    }
}
