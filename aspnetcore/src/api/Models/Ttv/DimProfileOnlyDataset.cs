using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimProfileOnlyDataset
    {
        public DimProfileOnlyDataset()
        {
            DimPids = new HashSet<DimPid>();
            DimWebLinks = new HashSet<DimWebLink>();
            FactFieldValues = new HashSet<FactFieldValue>();
        }

        public int Id { get; set; }
        public int? DimReferencedataIdAvailability { get; set; }
        public string OrcidWorkType { get; set; }
        public string LocalIdentifier { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string NameUnd { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionSv { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionUnd { get; set; }
        public string VersionInfo { get; set; }
        public DateTime? DatasetCreated { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int DimRegisteredDataSourceId { get; set; }

        public virtual DimReferencedatum DimReferencedataIdAvailabilityNavigation { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual ICollection<DimPid> DimPids { get; set; }
        public virtual ICollection<DimWebLink> DimWebLinks { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
    }
}
