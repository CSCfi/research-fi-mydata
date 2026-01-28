using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimTelephoneNumber
{
    public int Id { get; set; }

    public string TelephoneNumber { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int DimRegisteredDataSourceId { get; set; }

    public int DimKnownPersonId { get; set; }

    public int DimContactInformationId { get; set; }

    public virtual DimContactInformation DimContactInformation { get; set; }

    public virtual DimKnownPerson DimKnownPerson { get; set; }

    public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }

    public virtual ICollection<FactFieldValue> FactFieldValues { get; set; } = new List<FactFieldValue>();
}
