using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimResearcherToResearchCommunity
    {
        public DimResearcherToResearchCommunity()
        {
            FactFieldValues = new HashSet<FactFieldValue>();
        }

        public int Id { get; set; }
        public int DimResearchCommunityId { get; set; }
        public int DimKnownPersonId { get; set; }
        public int StartDate { get; set; }
        public int? EndDate { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionSv { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int DimRegisteredDataSourceId { get; set; }

        public virtual DimKnownPerson DimKnownPerson { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual DimResearchCommunity DimResearchCommunity { get; set; }
        public virtual DimDate EndDateNavigation { get; set; }
        public virtual DimDate StartDateNavigation { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
    }
}
