using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class PidM
{
    public int Id { get; set; }

    public int? DimInfrastructureId { get; set; }

    public int? DimServiceId { get; set; }

    public string ActionType { get; set; }

    public DateTime RowCreated { get; set; }

    public DateTime? Completed { get; set; }
}
