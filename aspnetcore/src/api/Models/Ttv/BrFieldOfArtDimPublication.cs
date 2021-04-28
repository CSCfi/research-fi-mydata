using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrFieldOfArtDimPublication
    {
        public int DimFieldOfArtId { get; set; }
        public int DimPublicationId { get; set; }

        public virtual DimFieldOfArt DimFieldOfArt { get; set; }
        public virtual DimPublication DimPublication { get; set; }
    }
}
