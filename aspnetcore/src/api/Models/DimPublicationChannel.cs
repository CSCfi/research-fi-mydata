using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimPublicationChannel
    {
        public DimPublicationChannel()
        {
            DimPid = new HashSet<DimPid>();
            DimResearchDataset = new HashSet<DimResearchDataset>();
            FactJufoClassCodesForPubChannels = new HashSet<FactJufoClassCodesForPubChannels>();
        }

        public int Id { get; set; }
        public string JufoCode { get; set; }
        public string ChannelNameAnylang { get; set; }
        public string PublisherNameText { get; set; }

        public virtual ICollection<DimPid> DimPid { get; set; }
        public virtual ICollection<DimResearchDataset> DimResearchDataset { get; set; }
        public virtual ICollection<FactJufoClassCodesForPubChannels> FactJufoClassCodesForPubChannels { get; set; }
    }
}
