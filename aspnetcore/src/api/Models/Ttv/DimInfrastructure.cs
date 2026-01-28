using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimInfrastructure
{
    public int Id { get; set; }

    public string Acronym { get; set; }

    public bool? FinlandRoadmap { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int DimRegisteredDataSourceId { get; set; }

    public int ResponsibleOrganizationId { get; set; }

    public int DimStartDate { get; set; }

    public int DimEndDate { get; set; }

    public string LocalIdentifier { get; set; }

    public virtual ICollection<DimContactInformation> DimContactInformations { get; set; } = new List<DimContactInformation>();

    public virtual ICollection<DimDescriptiveItem> DimDescriptiveItems { get; set; } = new List<DimDescriptiveItem>();

    public virtual DimDate DimEndDateNavigation { get; set; }

    public virtual ICollection<DimPid> DimPids { get; set; } = new List<DimPid>();

    public virtual ICollection<DimService> DimServices { get; set; } = new List<DimService>();

    public virtual DimDate DimStartDateNavigation { get; set; }

    public virtual ICollection<DimWebLink> DimWebLinks { get; set; } = new List<DimWebLink>();

    public virtual ICollection<FactContribution> FactContributions { get; set; } = new List<FactContribution>();

    public virtual ICollection<FactDimReferencedataFieldOfScience> FactDimReferencedataFieldOfSciences { get; set; } = new List<FactDimReferencedataFieldOfScience>();

    public virtual ICollection<FactInfraKeyword> FactInfraKeywords { get; set; } = new List<FactInfraKeyword>();

    public virtual ICollection<FactReferencedatum> FactReferencedata { get; set; } = new List<FactReferencedatum>();

    public virtual ICollection<FactRelation> FactRelationFromInfrastructures { get; set; } = new List<FactRelation>();

    public virtual ICollection<FactRelation> FactRelationToInfrastructures { get; set; } = new List<FactRelation>();

    public virtual ICollection<FactUpkeep> FactUpkeeps { get; set; } = new List<FactUpkeep>();

    public virtual DimOrganization ResponsibleOrganization { get; set; }

    public virtual ICollection<DimEsfri> DimEsfris { get; set; } = new List<DimEsfri>();

    public virtual ICollection<DimMeril> DimMerils { get; set; } = new List<DimMeril>();
}
