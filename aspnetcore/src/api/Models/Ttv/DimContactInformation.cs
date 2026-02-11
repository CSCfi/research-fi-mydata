using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

/// <summary>
/// https://iri.suomi.fi/model/researchfi_core/contact_information
/// https://iri.suomi.fi/model/researchfi_core/contact_point
/// </summary>
public partial class DimContactInformation
{
    public int Id { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core_agent/contact_name
    /// </summary>
    public string ContactLabel { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core/infraIsContactedVia
    /// </summary>
    public int DimInfrastructureId { get; set; }

    /// <summary>
    /// https://iri.suomi.fi/model/researchfi_core/serviceIsContactedVia
    /// </summary>
    public int DimServiceId { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public virtual ICollection<DimAddress> DimAddresses { get; set; } = new List<DimAddress>();

    public virtual ICollection<DimEmailAddrress> DimEmailAddrresses { get; set; } = new List<DimEmailAddrress>();

    public virtual DimInfrastructure DimInfrastructure { get; set; }

    public virtual DimService DimService { get; set; }

    public virtual ICollection<DimTelephoneNumber> DimTelephoneNumbers { get; set; } = new List<DimTelephoneNumber>();
}
