using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimIdentifierlessOrganization
{
    public int Id { get; set; }

    public string UnlinkedIdentifier { get; set; }

    public string NameFi { get; set; }

    public string NameEn { get; set; }

    public string NameSv { get; set; }

    public string NameUnd { get; set; }

    public string WeblinkUrl { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public virtual ICollection<FactOrganizationGeneral> FactOrganizationGenerals { get; set; } = new List<FactOrganizationGeneral>();
}
