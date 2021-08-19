using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimEvent
    {
        public DimEvent()
        {
            DimPids = new HashSet<DimPid>();
            DimResearchActivities = new HashSet<DimResearchActivity>();
            FactFieldValues = new HashSet<FactFieldValue>();
        }

        public int Id { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string NameUnd { get; set; }
        public int? EventLocationText { get; set; }
        public int DimDateIdStartDate { get; set; }
        public int? DimDateIdEndDate { get; set; }
        public int? DimGeoIdEventCountry { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int DimRegisteredDataSourceId { get; set; }

        public virtual DimDate DimDateIdEndDateNavigation { get; set; }
        public virtual DimDate DimDateIdStartDateNavigation { get; set; }
        public virtual DimGeo DimGeoIdEventCountryNavigation { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual ICollection<DimPid> DimPids { get; set; }
        public virtual ICollection<DimResearchActivity> DimResearchActivities { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
    }
}
