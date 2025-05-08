using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimResearchProject
{
    public int Id { get; set; }

    /// <summary>
    /// Hanke - vastuuorganisaatio
    /// </summary>
    public int ResponsibleOrganization { get; set; }

    /// <summary>
    /// Hanke - alkamispäivämäärä
    /// </summary>
    public int? StartDate { get; set; }

    /// <summary>
    /// Hanke - päättymispäivämäärä
    /// </summary>
    public int? EndDate { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int DimRegisteredDataSourceId { get; set; }

    public long ResponsiblePerson { get; set; }

    public virtual ICollection<DimDescriptiveItem> DimDescriptiveItems { get; set; } = new List<DimDescriptiveItem>();

    public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }

    public virtual DimDate EndDateNavigation { get; set; }

    public virtual DimOrganization ResponsibleOrganizationNavigation { get; set; }

    public virtual DimName ResponsiblePersonNavigation { get; set; }

    public virtual DimDate StartDateNavigation { get; set; }
}
