using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class BrFieldOfEducationDimPublication
    {
        public int DimFieldOfEducationId { get; set; }
        public int DimPublicationId { get; set; }

        public virtual DimFieldOfEducation DimFieldOfEducation { get; set; }
        public virtual DimPublication DimPublication { get; set; }
    }
}
