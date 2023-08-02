using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class FactContribution
{
    public int DimFundingDecisionId { get; set; }

    public int DimOrganizationId { get; set; }

    public int DimDateId { get; set; }

    public int DimNameId { get; set; }

    public int DimPublicationId { get; set; }

    public int DimGeoId { get; set; }

    public int DimInfrastructureId { get; set; }

    public int DimNewsFeedId { get; set; }

    public int DimResearchDatasetId { get; set; }

    public int DimResearchDataCatalogId { get; set; }

    public int DimIdentifierlessDataId { get; set; }

    public int DimResearchActivityId { get; set; }

    public int DimResearchCommunityId { get; set; }

    public int DimReferencedataActorRoleId { get; set; }

    public string ContributionType { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public virtual DimDate DimDate { get; set; }

    public virtual DimFundingDecision DimFundingDecision { get; set; }

    public virtual DimGeo DimGeo { get; set; }

    public virtual DimIdentifierlessDatum DimIdentifierlessData { get; set; }

    public virtual DimInfrastructure DimInfrastructure { get; set; }

    public virtual DimName DimName { get; set; }

    public virtual DimNewsFeed DimNewsFeed { get; set; }

    public virtual DimOrganization DimOrganization { get; set; }

    public virtual DimPublication DimPublication { get; set; }

    public virtual DimReferencedatum DimReferencedataActorRole { get; set; }

    public virtual DimResearchActivity DimResearchActivity { get; set; }

    public virtual DimResearchCommunity DimResearchCommunity { get; set; }

    public virtual DimResearchDataCatalog DimResearchDataCatalog { get; set; }

    public virtual DimResearchDataset DimResearchDataset { get; set; }
}
