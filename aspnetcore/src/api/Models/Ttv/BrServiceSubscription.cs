using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class BrServiceSubscription
{
    public int DimUserProfileId { get; set; }

    public int DimExternalServiceId { get; set; }

    public virtual DimExternalService DimExternalService { get; set; }

    public virtual DimUserProfile DimUserProfile { get; set; }
}
