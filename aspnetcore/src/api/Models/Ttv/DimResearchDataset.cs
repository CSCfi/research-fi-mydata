using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimResearchDataset
    {
        public DimResearchDataset()
        {
            BrLanguageCodesForDatasets = new HashSet<BrLanguageCodesForDatasets>();
            BrResearchDatasetDimFieldOfScience = new HashSet<BrResearchDatasetDimFieldOfScience>();
            DimPid = new HashSet<DimPid>();
            FactContribution = new HashSet<FactContribution>();
        }

        public int Id { get; set; }
        public int? DimPublicationChannelId { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionSv { get; set; }
        public string DescriptionEn { get; set; }
        public bool? InternationalCollaboration { get; set; }
        public DateTime? DatasetCreated { get; set; }
        public DateTime? DatasetModified { get; set; }
        public DateTime? TemporalCoverageStart { get; set; }
        public DateTime? TemporalCoverageEnd { get; set; }
        public string GeographicCoverage { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimPublicationChannel DimPublicationChannel { get; set; }
        public virtual ICollection<BrLanguageCodesForDatasets> BrLanguageCodesForDatasets { get; set; }
        public virtual ICollection<BrResearchDatasetDimFieldOfScience> BrResearchDatasetDimFieldOfScience { get; set; }
        public virtual ICollection<DimPid> DimPid { get; set; }
        public virtual ICollection<FactContribution> FactContribution { get; set; }
    }
}
