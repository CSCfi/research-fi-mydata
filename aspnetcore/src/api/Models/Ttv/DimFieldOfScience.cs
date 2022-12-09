using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimFieldOfScience
    {
        public DimFieldOfScience()
        {
            FactFieldValues = new HashSet<FactFieldValue>();
            DimFundingDecisions = new HashSet<DimFundingDecision>();
            DimInfrastructures = new HashSet<DimInfrastructure>();
            DimKnownPeople = new HashSet<DimKnownPerson>();
            DimPublications = new HashSet<DimPublication>();
            DimResearchActivities = new HashSet<DimResearchActivity>();
            DimResearchDatasets = new HashSet<DimResearchDataset>();
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

        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }

        public virtual ICollection<DimFundingDecision> DimFundingDecisions { get; set; }
        public virtual ICollection<DimInfrastructure> DimInfrastructures { get; set; }
        public virtual ICollection<DimKnownPerson> DimKnownPeople { get; set; }
        public virtual ICollection<DimPublication> DimPublications { get; set; }
        public virtual ICollection<DimResearchActivity> DimResearchActivities { get; set; }
        public virtual ICollection<DimResearchDataset> DimResearchDatasets { get; set; }
    }
}
