using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrDimReferencedataDimCallProgramme
    {
        public int DimCallProgrammeId { get; set; }
        public int DimReferencedataId { get; set; }

        public virtual DimCallProgramme DimCallProgramme { get; set; }
        public virtual DimReferencedatum DimReferencedata { get; set; }
    }
}
