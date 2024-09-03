using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimCallProgramme
{
    public int Id { get; set; }

    public int DimDateIdDue { get; set; }

    public int DimDateIdOpen { get; set; }

    public TimeOnly? DueDateDueTime { get; set; }

    public string Abbreviation { get; set; }

    public string EuCallId { get; set; }

    public string NameFi { get; set; }

    public string NameSv { get; set; }

    public string NameEn { get; set; }

    public string NameUnd { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int? DimCallProgrammeId { get; set; }

    public string SourceProgrammeId { get; set; }

    public int DimRegisteredDataSourceId { get; set; }

    public string DescriptionFi { get; set; }

    public string DescriptionSv { get; set; }

    public string DescriptionEn { get; set; }

    public string ApplicationTermsFi { get; set; }

    public string ApplicationTermsSv { get; set; }

    public string ApplicationTermsEn { get; set; }

    public string ContactInformation { get; set; }

    public bool? ContinuousApplicationPeriod { get; set; }

    public bool IsOpenCall { get; set; }

    public string CallNameDetailsFi { get; set; }

    public string CallNameDetailsEn { get; set; }

    public string CallNameDetailsSv { get; set; }

    public string LocalIdentifier { get; set; }

    public int? TypeOfFunding { get; set; }

    public virtual ICollection<DimCallDecision> DimCallDecisions { get; set; } = new List<DimCallDecision>();

    public virtual DimCallProgramme DimCallProgrammeNavigation { get; set; }

    public virtual DimDate DimDateIdDueNavigation { get; set; }

    public virtual DimDate DimDateIdOpenNavigation { get; set; }

    public virtual ICollection<DimFundingDecision> DimFundingDecisions { get; set; } = new List<DimFundingDecision>();

    public virtual ICollection<DimProfileOnlyFundingDecision> DimProfileOnlyFundingDecisions { get; set; } = new List<DimProfileOnlyFundingDecision>();

    public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }

    public virtual ICollection<DimWebLink> DimWebLinks { get; set; } = new List<DimWebLink>();

    public virtual ICollection<DimCallProgramme> InverseDimCallProgrammeNavigation { get; set; } = new List<DimCallProgramme>();

    public virtual DimReferencedatum TypeOfFundingNavigation { get; set; }

    public virtual ICollection<DimCallProgramme> DimCallProgrammeId2s { get; set; } = new List<DimCallProgramme>();

    public virtual ICollection<DimCallProgramme> DimCallProgrammes { get; set; } = new List<DimCallProgramme>();

    public virtual ICollection<DimOrganization> DimOrganizations { get; set; } = new List<DimOrganization>();

    public virtual ICollection<DimReferencedatum> DimReferencedata { get; set; } = new List<DimReferencedatum>();
}
