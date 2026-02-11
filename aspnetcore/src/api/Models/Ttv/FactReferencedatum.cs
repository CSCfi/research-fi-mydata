using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class FactReferencedatum
{
    public int DimReferencedataId { get; set; }

    public int DimResearchDatasetId { get; set; }

    public int DimInfrastructureId { get; set; }

    public int DimPublicationId { get; set; }

    public int DimResearchActivityId { get; set; }

    public int DimFundingDecisionId { get; set; }

    public int DimCallProgrammeId { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int DimServiceId { get; set; }

    public virtual DimCallProgramme DimCallProgramme { get; set; }

    public virtual DimFundingDecision DimFundingDecision { get; set; }

    public virtual DimInfrastructure DimInfrastructure { get; set; }

    public virtual DimPublication DimPublication { get; set; }

    public virtual DimReferencedatum DimReferencedata { get; set; }

    public virtual DimResearchActivity DimResearchActivity { get; set; }

    public virtual DimResearchDataset DimResearchDataset { get; set; }

    public virtual DimService DimService { get; set; }
}
