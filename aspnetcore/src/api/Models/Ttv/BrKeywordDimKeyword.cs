using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrKeywordDimKeyword
    {
        public int DimKeywordId { get; set; }
        public int DimKeywordId2 { get; set; }
        public int DimReferencedataIdRelationshipTypeCode { get; set; }

        public virtual DimKeyword DimKeyword { get; set; }
        public virtual DimKeyword DimKeywordId2Navigation { get; set; }
        public virtual DimReferencedatum DimReferencedataIdRelationshipTypeCodeNavigation { get; set; }
    }
}
