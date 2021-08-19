using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimKnownPersonDimFieldOfScience
    {
        public int DimFieldOfScienceId { get; set; }
        public int DimKnownPersonId { get; set; }

        public virtual DimFieldOfScience DimFieldOfScience { get; set; }
        public virtual DimKnownPerson DimKnownPerson { get; set; }
    }
}
