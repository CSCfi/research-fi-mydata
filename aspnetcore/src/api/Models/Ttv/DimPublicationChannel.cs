using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimPublicationChannel
{
    public int Id { get; set; }

    public string JufoCode { get; set; }

    public string ChannelNameAnylang { get; set; }

    public string PublisherNameText { get; set; }

    public virtual ICollection<DimPid> DimPids { get; set; } = new List<DimPid>();

    public virtual ICollection<DimPublication> DimPublications { get; set; } = new List<DimPublication>();

    public virtual ICollection<DimResearchActivity> DimResearchActivities { get; set; } = new List<DimResearchActivity>();

    public virtual ICollection<FactJufoClassCodesForPubChannel> FactJufoClassCodesForPubChannels { get; set; } = new List<FactJufoClassCodesForPubChannel>();
}
