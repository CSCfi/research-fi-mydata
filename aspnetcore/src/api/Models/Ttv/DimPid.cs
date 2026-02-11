using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimPid
{
    public int Id { get; set; }

    public string PidContent { get; set; }

    public string PidType { get; set; }

    public int DimOrganizationId { get; set; }

    public int DimKnownPersonId { get; set; }

    public int DimPublicationId { get; set; }

    public int DimServiceId { get; set; }

    public int DimInfrastructureId { get; set; }

    public int DimPublicationChannelId { get; set; }

    public int DimResearchDatasetId { get; set; }

    public int DimResearchDataCatalogId { get; set; }

    public int DimResearchActivityId { get; set; }

    public int DimEventId { get; set; }

    public int DimProfileOnlyPublicationId { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int? DimProfileOnlyDatasetId { get; set; }

    public int? DimProfileOnlyFundingDecisionId { get; set; }

    public int? DimResearchProjectId { get; set; }

    public int? DimResearchCommunityId { get; set; }

    public virtual DimEvent DimEvent { get; set; }

    public virtual DimInfrastructure DimInfrastructure { get; set; }

    public virtual DimKnownPerson DimKnownPerson { get; set; }

    public virtual DimOrganization DimOrganization { get; set; }

    public virtual DimProfileOnlyDataset DimProfileOnlyDataset { get; set; }

    public virtual DimProfileOnlyFundingDecision DimProfileOnlyFundingDecision { get; set; }

    public virtual DimProfileOnlyPublication DimProfileOnlyPublication { get; set; }

    public virtual DimPublication DimPublication { get; set; }

    public virtual DimPublicationChannel DimPublicationChannel { get; set; }

    public virtual DimResearchActivity DimResearchActivity { get; set; }

    public virtual DimResearchCommunity DimResearchCommunity { get; set; }

    public virtual DimResearchDataCatalog DimResearchDataCatalog { get; set; }

    public virtual DimResearchDataset DimResearchDataset { get; set; }

    public virtual DimResearchProject DimResearchProject { get; set; }

    public virtual DimService DimService { get; set; }

    public virtual ICollection<FactFieldValue> FactFieldValueDimPidIdOrcidPutCodeNavigations { get; set; } = new List<FactFieldValue>();

    public virtual ICollection<FactFieldValue> FactFieldValueDimPids { get; set; } = new List<FactFieldValue>();
}
