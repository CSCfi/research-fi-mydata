using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimFieldOfScience
    {
        public DimFieldOfScience()
        {
            BrFieldOfScienceDimFundingDecision = new HashSet<BrFieldOfScienceDimFundingDecision>();
            BrFieldOfScienceDimPublication = new HashSet<BrFieldOfScienceDimPublication>();
            BrInfrastructureDimFieldOfScience = new HashSet<BrInfrastructureDimFieldOfScience>();
            DimKnownPersonDimFieldOfScience = new HashSet<DimKnownPersonDimFieldOfScience>();
        }

        public int Id { get; set; }
        public string FieldId { get; set; }
        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ICollection<BrFieldOfScienceDimFundingDecision> BrFieldOfScienceDimFundingDecision { get; set; }
        public virtual ICollection<BrFieldOfScienceDimPublication> BrFieldOfScienceDimPublication { get; set; }
        public virtual ICollection<BrInfrastructureDimFieldOfScience> BrInfrastructureDimFieldOfScience { get; set; }
        public virtual ICollection<DimKnownPersonDimFieldOfScience> DimKnownPersonDimFieldOfScience { get; set; }
    }
}
