using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class FactJufoClassCodesForPubChannel
{
    public int DimPublicationChannelId { get; set; }

    public int JufoClasses { get; set; }

    public int Year { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public virtual DimPublicationChannel DimPublicationChannel { get; set; }

    public virtual DimReferencedatum JufoClassesNavigation { get; set; }
}
