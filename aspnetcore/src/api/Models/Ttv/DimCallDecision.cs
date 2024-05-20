using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

/// <summary>
/// Rahoituspäätöspaneeli
/// </summary>
public partial class DimCallDecision
{
    public int Id { get; set; }

    public int DecisionMaker { get; set; }

    public int DimDateIdApproval { get; set; }

    public int DimCallProgrammeId { get; set; }

    /// <summary>
    /// Rahoituspäätöspaneeli - Haun vaihe
    /// </summary>
    public string CallProcessingPhase { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public virtual DimReferencedatum DecisionMakerNavigation { get; set; }

    public virtual DimCallProgramme DimCallProgramme { get; set; }

    public virtual DimDate DimDateIdApprovalNavigation { get; set; }

    public virtual ICollection<DimFundingDecision> DimFundingDecisions { get; set; } = new List<DimFundingDecision>();
}
