using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimPublicationChannel
    {
        public DimPublicationChannel()
        {
            DimPids = new HashSet<DimPid>();
            DimResearchActivities = new HashSet<DimResearchActivity>();
            FactJufoClassCodesForPubChannels = new HashSet<FactJufoClassCodesForPubChannel>();
        }

        public int Id { get; set; }
        public string JufoCode { get; set; }
        public string ChannelNameAnylang { get; set; }
        public string PublisherNameText { get; set; }
        public string OpenAccess { get; set; }

        public virtual ICollection<DimPid> DimPids { get; set; }
        public virtual ICollection<DimResearchActivity> DimResearchActivities { get; set; }
        public virtual ICollection<FactJufoClassCodesForPubChannel> FactJufoClassCodesForPubChannels { get; set; }
    }
}
