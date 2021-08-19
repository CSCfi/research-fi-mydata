using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class FactJufoClassCodesForPubChannel
    {
        public int DimPublicationChannelId { get; set; }
        public int DimReferencedataId { get; set; }
        public int Year { get; set; }

        public virtual DimPublicationChannel DimPublicationChannel { get; set; }
        public virtual DimReferencedatum DimReferencedata { get; set; }
    }
}
