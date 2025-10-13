using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimResearchDataset
{
    public int Id { get; set; }

    public int? DimResearchDataCatalogId { get; set; }

    public int? DimReferencedataAvailability { get; set; }

    public string LocalIdentifier { get; set; }

    public DateTime? DatasetCreated { get; set; }

    public DateTime? DatasetModified { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int DimRegisteredDataSourceId { get; set; }

    public string VersionInfo { get; set; }

    public virtual ICollection<BrDatasetDatasetRelationship> BrDatasetDatasetRelationshipDimResearchDatasetId2Navigations { get; set; } = new List<BrDatasetDatasetRelationship>();

    public virtual ICollection<BrDatasetDatasetRelationship> BrDatasetDatasetRelationshipDimResearchDatasets { get; set; } = new List<BrDatasetDatasetRelationship>();

    public virtual ICollection<DimDescriptiveItem> DimDescriptiveItems { get; set; } = new List<DimDescriptiveItem>();

    public virtual ICollection<DimPid> DimPids { get; set; } = new List<DimPid>();

    public virtual DimReferencedatum DimReferencedataAvailabilityNavigation { get; set; }

    public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }

    public virtual DimResearchDataCatalog DimResearchDataCatalog { get; set; }

    public virtual ICollection<DimWebLink> DimWebLinks { get; set; } = new List<DimWebLink>();

    public virtual ICollection<FactContribution> FactContributions { get; set; } = new List<FactContribution>();

    public virtual ICollection<FactDimReferencedataFieldOfScience> FactDimReferencedataFieldOfSciences { get; set; } = new List<FactDimReferencedataFieldOfScience>();

    public virtual ICollection<FactFieldValue> FactFieldValues { get; set; } = new List<FactFieldValue>();

    public virtual ICollection<FactKeyword> FactKeywords { get; set; } = new List<FactKeyword>();

    public virtual ICollection<FactReferencedatum> FactReferencedata { get; set; } = new List<FactReferencedatum>();

    public virtual ICollection<FactRelation> FactRelationFromResearchDatasets { get; set; } = new List<FactRelation>();

    public virtual ICollection<FactRelation> FactRelationToResearchDatasets { get; set; } = new List<FactRelation>();
}
