using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimFieldOfScience
    {
        public DimFieldOfScience()
        {
            BrFieldOfScienceDimFundingDecisions = new HashSet<BrFieldOfScienceDimFundingDecision>();
            BrFieldOfScienceDimPublications = new HashSet<BrFieldOfScienceDimPublication>();
            BrInfrastructureDimFieldOfSciences = new HashSet<BrInfrastructureDimFieldOfScience>();
            BrResearchDatasetDimFieldOfSciences = new HashSet<BrResearchDatasetDimFieldOfScience>();
            DimFieldOfScienceDimResearchActivities = new HashSet<DimFieldOfScienceDimResearchActivity>();
            DimKnownPersonDimFieldOfSciences = new HashSet<DimKnownPersonDimFieldOfScience>();
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

        public virtual ICollection<BrFieldOfScienceDimFundingDecision> BrFieldOfScienceDimFundingDecisions { get; set; }
        public virtual ICollection<BrFieldOfScienceDimPublication> BrFieldOfScienceDimPublications { get; set; }
        public virtual ICollection<BrInfrastructureDimFieldOfScience> BrInfrastructureDimFieldOfSciences { get; set; }
        public virtual ICollection<BrResearchDatasetDimFieldOfScience> BrResearchDatasetDimFieldOfSciences { get; set; }
        public virtual ICollection<DimFieldOfScienceDimResearchActivity> DimFieldOfScienceDimResearchActivities { get; set; }
        public virtual ICollection<DimKnownPersonDimFieldOfScience> DimKnownPersonDimFieldOfSciences { get; set; }
    }
}
