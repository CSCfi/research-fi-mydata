using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimFundingDecision
    {
        public DimFundingDecision()
        {
            BrFieldOfScienceDimFundingDecision = new HashSet<BrFieldOfScienceDimFundingDecision>();
            BrFundingConsortiumParticipation = new HashSet<BrFundingConsortiumParticipation>();
            BrFundingDecisionDimFieldOfArt = new HashSet<BrFundingDecisionDimFieldOfArt>();
            BrKeywordDimFundingDecision = new HashSet<BrKeywordDimFundingDecision>();
            BrParticipatesInFundingGroup = new HashSet<BrParticipatesInFundingGroup>();
            BrPreviousFundingDecisionDimFundingDecisionFrom = new HashSet<BrPreviousFundingDecision>();
            BrPreviousFundingDecisionDimFundingDecisionTo = new HashSet<BrPreviousFundingDecision>();
            BrRelatedFundingDecisionDimFundingDecisionFrom = new HashSet<BrRelatedFundingDecision>();
            BrRelatedFundingDecisionDimFundingDecisionTo = new HashSet<BrRelatedFundingDecision>();
            DimPid = new HashSet<DimPid>();
            DimWebLink = new HashSet<DimWebLink>();
            FactContribution = new HashSet<FactContribution>();
            FactFieldDisplayContent = new HashSet<FactFieldDisplayContent>();
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

        public virtual DimCallProgramme DimCallProgramme { get; set; }
        public virtual DimDate DimDateIdApprovalNavigation { get; set; }
        public virtual DimDate DimDateIdEndNavigation { get; set; }
        public virtual DimDate DimDateIdStartNavigation { get; set; }
        public virtual DimFundingDecision DimFundingDecisionIdParentDecisionNavigation { get; set; }
        public virtual DimGeo DimGeo { get; set; }
        public virtual DimName DimNameIdContactPersonNavigation { get; set; }
        public virtual DimOrganization DimOrganizationIdFunderNavigation { get; set; }
        public virtual DimTypeOfFunding DimTypeOfFunding { get; set; }
        public virtual ICollection<BrFieldOfScienceDimFundingDecision> BrFieldOfScienceDimFundingDecision { get; set; }
        public virtual ICollection<BrFundingConsortiumParticipation> BrFundingConsortiumParticipation { get; set; }
        public virtual ICollection<BrFundingDecisionDimFieldOfArt> BrFundingDecisionDimFieldOfArt { get; set; }
        public virtual ICollection<BrKeywordDimFundingDecision> BrKeywordDimFundingDecision { get; set; }
        public virtual ICollection<BrParticipatesInFundingGroup> BrParticipatesInFundingGroup { get; set; }
        public virtual ICollection<BrPreviousFundingDecision> BrPreviousFundingDecisionDimFundingDecisionFrom { get; set; }
        public virtual ICollection<BrPreviousFundingDecision> BrPreviousFundingDecisionDimFundingDecisionTo { get; set; }
        public virtual ICollection<BrRelatedFundingDecision> BrRelatedFundingDecisionDimFundingDecisionFrom { get; set; }
        public virtual ICollection<BrRelatedFundingDecision> BrRelatedFundingDecisionDimFundingDecisionTo { get; set; }
        public virtual ICollection<DimPid> DimPid { get; set; }
        public virtual ICollection<DimWebLink> DimWebLink { get; set; }
        public virtual ICollection<FactContribution> FactContribution { get; set; }
        public virtual ICollection<FactFieldDisplayContent> FactFieldDisplayContent { get; set; }
        public virtual ICollection<DimFundingDecision> InverseDimFundingDecisionIdParentDecisionNavigation { get; set; }
    }
}
