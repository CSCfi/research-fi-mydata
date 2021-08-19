using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrKeywordDimPublication
    {
        public int DimKeywordId { get; set; }
        public int DimPublicationId { get; set; }

        public virtual DimKeyword DimKeyword { get; set; }
        public virtual DimPublication DimPublication { get; set; }
    }
}
