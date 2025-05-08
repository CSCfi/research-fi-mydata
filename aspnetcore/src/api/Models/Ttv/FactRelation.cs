using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class FactRelation
{
    public int RelationTypeCode { get; set; }

    public int FromPublicationId { get; set; }

    public int FromResearchDatasetId { get; set; }

    public int FromIdentifierlessDataId { get; set; }

    public int FromInfrastructureId { get; set; }

    public int ToResearchDatasetId { get; set; }

    public int ToIdentifierlessDataId { get; set; }

    public int ToPublicationId { get; set; }

    public int ToInfrastructureId { get; set; }

    public int StartDate { get; set; }

    public int EndDate { get; set; }

    public int DimRegisteredDataSourceId { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public string SourceDescription { get; set; }

    public string SourceId { get; set; }

    public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }

    public virtual DimDate EndDateNavigation { get; set; }

    public virtual DimIdentifierlessDatum FromIdentifierlessData { get; set; }

    public virtual DimInfrastructure FromInfrastructure { get; set; }

    public virtual DimPublication FromPublication { get; set; }

    public virtual DimResearchDataset FromResearchDataset { get; set; }

    public virtual DimReferencedatum RelationTypeCodeNavigation { get; set; }

    public virtual DimDate StartDateNavigation { get; set; }

    public virtual DimIdentifierlessDatum ToIdentifierlessData { get; set; }

    public virtual DimInfrastructure ToInfrastructure { get; set; }

    public virtual DimPublication ToPublication { get; set; }

    public virtual DimResearchDataset ToResearchDataset { get; set; }
}
