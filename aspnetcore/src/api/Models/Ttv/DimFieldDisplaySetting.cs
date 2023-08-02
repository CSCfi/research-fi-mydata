using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimFieldDisplaySetting
{
    public int Id { get; set; }

    public int DimUserProfileId { get; set; }

    public int FieldIdentifier { get; set; }

    public bool Show { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public virtual DimUserProfile DimUserProfile { get; set; }

    public virtual ICollection<FactFieldValue> FactFieldValues { get; set; } = new List<FactFieldValue>();

    public virtual ICollection<DimRegisteredDataSource> DimRegisteredDataSources { get; set; } = new List<DimRegisteredDataSource>();
}
