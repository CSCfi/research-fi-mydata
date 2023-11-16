using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class FactJufoClassCodesForPubChannel
{
    public int DimPublicationChannelId { get; set; }

    public int JufoClasses { get; set; }

    public int Year { get; set; }

    public virtual DimPublicationChannel DimPublicationChannel { get; set; }

    public virtual DimReferencedatum JufoClassesNavigation { get; set; }
}
