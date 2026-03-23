using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimDescriptiveItem
{
    public int Id { get; set; }

    public int DimStartDate { get; set; }

    public int? DimEndDate { get; set; }

    public string DescriptiveItem { get; set; }

    public string DescriptiveItemType { get; set; }

    public string DescriptiveItemLanguage { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int DimResearchProjectId { get; set; }

    public int DimRegisteredDataSourceId { get; set; }

    public int DimPublicationId { get; set; }

    public int DimResearchDatasetId { get; set; }

    public int DimInfrastructureId { get; set; }

    public int DimServiceId { get; set; }

    public int DimResearchDataCatalogId { get; set; }

    public virtual DimDate DimEndDateNavigation { get; set; }

    public virtual DimInfrastructure DimInfrastructure { get; set; }

    public virtual DimPublication DimPublication { get; set; }

    public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }

    public virtual DimResearchDataCatalog DimResearchDataCatalog { get; set; }

    public virtual DimResearchDataset DimResearchDataset { get; set; }

    public virtual DimResearchProject DimResearchProject { get; set; }

    public virtual DimService DimService { get; set; }

    public virtual DimDate DimStartDateNavigation { get; set; }
}
