using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class FactJufoClassCodesForPubChannels
    {
        public int DimPublicationChannelId { get; set; }
        public int DimReferencedataId { get; set; }
        public int Year { get; set; }

        public virtual DimPublicationChannel DimPublicationChannel { get; set; }
        public virtual DimReferencedata DimReferencedata { get; set; }
    }
}
