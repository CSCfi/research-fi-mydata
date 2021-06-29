using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimFundingDecision
    {
        public DimFundingDecision()
        {
            BrFieldOfScienceDimFundingDecisions = new HashSet<BrFieldOfScienceDimFundingDecision>();
            BrFundingConsortiumParticipations = new HashSet<BrFundingConsortiumParticipation>();
            BrFundingDecisionDimFieldOfArts = new HashSet<BrFundingDecisionDimFieldOfArt>();
            BrKeywordDimFundingDecisions = new HashSet<BrKeywordDimFundingDecision>();
            BrParticipatesInFundingGroups = new HashSet<BrParticipatesInFundingGroup>();
            BrPreviousFundingDecisionDimFundingDecisionFroms = new HashSet<BrPreviousFundingDecision>();
            BrPreviousFundingDecisionDimFundingDecisionTos = new HashSet<BrPreviousFundingDecision>();
            BrRelatedFundingDecisionDimFundingDecisionFroms = new HashSet<BrRelatedFundingDecision>();
            BrRelatedFundingDecisionDimFundingDecisionTos = new HashSet<BrRelatedFundingDecision>();
            DimPids = new HashSet<DimPid>();
            DimWebLinks = new HashSet<DimWebLink>();
            FactContributions = new HashSet<FactContribution>();
            FactFieldValues = new HashSet<FactFieldValue>();
            InverseDimFundingDecisionIdParentDecisionNavigation = new HashSet<DimFundingDecision>();
        }

        public int Id { get; set; }
        public int DimDateIdApproval { get; set; }
        public int DimDateIdStart { get; set; }
        public int DimDateIdEnd { get; set; }
        public int DimNameIdContactPerson { get; set; }
        public int DimCallProgrammeId { get; set; }
        public int DimGeoId { get; set; }
        public int DimTypeOfFundingId { get; set; }
        public int? DimOrganizationIdFunder { get; set; }
        public string DimPidPidContent { get; set; }
        public int DimFundingDecisionIdParentDecision { get; set; }
        public string FunderProjectNumber { get; set; }
        public string Acronym { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string NameUnd { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionSv { get; set; }
        public int? HasInternationalCollaboration { get; set; }
        public int? HasBusinessCollaboration { get; set; }
        public decimal AmountInEur { get; set; }
        public decimal? AmountInFundingDecisionCurrency { get; set; }
        public string FundingDecisionCurrencyAbbreviation { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int DimRegisteredDataSourceId { get; set; }

        public virtual DimCallProgramme DimCallProgramme { get; set; }
        public virtual DimDate DimDateIdApprovalNavigation { get; set; }
        public virtual DimDate DimDateIdEndNavigation { get; set; }
        public virtual DimDate DimDateIdStartNavigation { get; set; }
        public virtual DimFundingDecision DimFundingDecisionIdParentDecisionNavigation { get; set; }
        public virtual DimGeo DimGeo { get; set; }
        public virtual DimName DimNameIdContactPersonNavigation { get; set; }
        public virtual DimOrganization DimOrganizationIdFunderNavigation { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual DimTypeOfFunding DimTypeOfFunding { get; set; }
        public virtual ICollection<BrFieldOfScienceDimFundingDecision> BrFieldOfScienceDimFundingDecisions { get; set; }
        public virtual ICollection<BrFundingConsortiumParticipation> BrFundingConsortiumParticipations { get; set; }
        public virtual ICollection<BrFundingDecisionDimFieldOfArt> BrFundingDecisionDimFieldOfArts { get; set; }
        public virtual ICollection<BrKeywordDimFundingDecision> BrKeywordDimFundingDecisions { get; set; }
        public virtual ICollection<BrParticipatesInFundingGroup> BrParticipatesInFundingGroups { get; set; }
        public virtual ICollection<BrPreviousFundingDecision> BrPreviousFundingDecisionDimFundingDecisionFroms { get; set; }
        public virtual ICollection<BrPreviousFundingDecision> BrPreviousFundingDecisionDimFundingDecisionTos { get; set; }
        public virtual ICollection<BrRelatedFundingDecision> BrRelatedFundingDecisionDimFundingDecisionFroms { get; set; }
        public virtual ICollection<BrRelatedFundingDecision> BrRelatedFundingDecisionDimFundingDecisionTos { get; set; }
        public virtual ICollection<DimPid> DimPids { get; set; }
        public virtual ICollection<DimWebLink> DimWebLinks { get; set; }
        public virtual ICollection<FactContribution> FactContributions { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
        public virtual ICollection<DimFundingDecision> InverseDimFundingDecisionIdParentDecisionNavigation { get; set; }
    }
}
