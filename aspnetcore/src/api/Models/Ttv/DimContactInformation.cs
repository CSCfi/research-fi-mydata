using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimContactInformation
{
    public int Id { get; set; }

    public string ContactLabel { get; set; }

    public int DimInfrastructureId { get; set; }

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
