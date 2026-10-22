using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class FactOrganizationGeneral
{
    public int Id { get; set; }

    public int DimOrganizationId { get; set; }

    public int DimIdentifierlessOrganizationId { get; set; }

    public int ParentOrgGeneralId { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public virtual DimIdentifierlessOrganization DimIdentifierlessOrganization { get; set; }

    public virtual DimOrganization DimOrganization { get; set; }

    public virtual ICollection<FactContribution> FactContributions { get; set; } = new List<FactContribution>();

    public virtual ICollection<FactOrganizationGeneral> InverseParentOrgGeneral { get; set; } = new List<FactOrganizationGeneral>();

    public virtual FactOrganizationGeneral ParentOrgGeneral { get; set; }
}
