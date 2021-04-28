using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrFieldOfScienceDimPublication
    {
        public int DimFieldOfScienceId { get; set; }
        public int DimPublicationId { get; set; }

        public virtual DimFieldOfScience DimFieldOfScience { get; set; }
        public virtual DimPublication DimPublication { get; set; }
    }
}
