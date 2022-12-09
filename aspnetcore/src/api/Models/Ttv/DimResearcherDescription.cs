using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimResearcherDescription
    {
        public DimResearcherDescription()
        {
            FactFieldValues = new HashSet<FactFieldValue>();
        }

        public int Id { get; set; }
        public int? DescriptionType { get; set; }
        public string ResearchDescriptionFi { get; set; }
        public string ResearchDescriptionEn { get; set; }
        public string ResearchDescriptionSv { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int DimKnownPersonId { get; set; }
        public int DimRegisteredDataSourceId { get; set; }

        public virtual DimKnownPerson DimKnownPerson { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
    }
}
