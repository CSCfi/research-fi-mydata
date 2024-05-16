using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimFundingDecision
{
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

    /// <summary>
    /// Päätöksen paikallinen tunniste (tiedon toimittajan)
    /// </summary>
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

    /// <summary>
    /// Rahoituspäätös - Päätöspaneeli
    /// </summary>
    public int? DimCallDecisionsId { get; set; }

    public virtual ICollection<BrFundingConsortiumParticipation> BrFundingConsortiumParticipations { get; set; } = new List<BrFundingConsortiumParticipation>();

    public virtual ICollection<BrParticipatesInFundingGroup> BrParticipatesInFundingGroups { get; set; } = new List<BrParticipatesInFundingGroup>();

    public virtual ICollection<BrWordClusterDimFundingDecision> BrWordClusterDimFundingDecisions { get; set; } = new List<BrWordClusterDimFundingDecision>();

    public virtual DimCallDecision DimCallDecisions { get; set; }

    public virtual DimCallProgramme DimCallProgramme { get; set; }

    public virtual DimDate DimDateIdApprovalNavigation { get; set; }

    public virtual DimDate DimDateIdEndNavigation { get; set; }

    public virtual DimDate DimDateIdStartNavigation { get; set; }

    public virtual DimFundingDecision DimFundingDecisionIdParentDecisionNavigation { get; set; }

    public virtual DimGeo DimGeo { get; set; }

    public virtual DimName DimNameIdContactPersonNavigation { get; set; }

    public virtual DimOrganization DimOrganizationIdFunderNavigation { get; set; }

    public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }

    public virtual DimReferencedatum DimTypeOfFunding { get; set; }

    public virtual ICollection<DimWebLink> DimWebLinks { get; set; } = new List<DimWebLink>();

    public virtual ICollection<FactContribution> FactContributions { get; set; } = new List<FactContribution>();

    public virtual ICollection<FactDimReferencedataFieldOfScience> FactDimReferencedataFieldOfSciences { get; set; } = new List<FactDimReferencedataFieldOfScience>();

    public virtual ICollection<FactFieldValue> FactFieldValues { get; set; } = new List<FactFieldValue>();

    public virtual ICollection<DimFundingDecision> InverseDimFundingDecisionIdParentDecisionNavigation { get; set; } = new List<DimFundingDecision>();

    public virtual ICollection<DimFundingDecision> DimFundingDecisionFroms { get; set; } = new List<DimFundingDecision>();

    public virtual ICollection<DimFundingDecision> DimFundingDecisionTos { get; set; } = new List<DimFundingDecision>();

    public virtual ICollection<DimKeyword> DimKeywords { get; set; } = new List<DimKeyword>();
}
