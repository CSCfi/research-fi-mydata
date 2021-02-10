using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimGeo
    {
        public DimGeo()
        {
            DimFundingDecision = new HashSet<DimFundingDecision>();
            FactContribution = new HashSet<FactContribution>();
            FactUpkeep = new HashSet<FactUpkeep>();
        }

        public int Id { get; set; }
        public string CountryCode { get; set; }
        public string CountryId { get; set; }
        public string RegionId { get; set; }
        public string MunicipalityId { get; set; }
        public string CountryFi { get; set; }
        public string CountryEn { get; set; }
        public string CountrySv { get; set; }
        public string RegionFi { get; set; }
        public string RegionSv { get; set; }
        public string MunicipalityFi { get; set; }
        public string MunicipalitySv { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ICollection<DimFundingDecision> DimFundingDecision { get; set; }
        public virtual ICollection<FactContribution> FactContribution { get; set; }
        public virtual ICollection<FactUpkeep> FactUpkeep { get; set; }
    }
}
