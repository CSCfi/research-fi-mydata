using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrArtpublicationTypecategory
    {
        public int DimPublicationId { get; set; }
        public int DimReferencedataid { get; set; }

        public virtual DimPublication DimPublication { get; set; }
        public virtual DimReferencedatum DimReferencedata { get; set; }
    }
}
