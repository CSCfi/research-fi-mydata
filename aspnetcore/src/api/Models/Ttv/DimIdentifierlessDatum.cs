using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimIdentifierlessDatum
{
    public int Id { get; set; }

    public string Type { get; set; }

    public int? DimIdentifierlessDataId { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public string ValueFi { get; set; }

    public string ValueEn { get; set; }

    public string ValueSv { get; set; }

    public string UnlinkedIdentifier { get; set; }

    public virtual DimIdentifierlessDatum DimIdentifierlessData { get; set; }

    public virtual ICollection<FactContribution> FactContributions { get; set; } = new List<FactContribution>();

    public virtual ICollection<FactFieldValue> FactFieldValues { get; set; } = new List<FactFieldValue>();

    public virtual ICollection<DimIdentifierlessDatum> InverseDimIdentifierlessData { get; set; } = new List<DimIdentifierlessDatum>();
}
