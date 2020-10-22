using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimCallProgramme
    {
        public DimCallProgramme()
        {
            BrCallProgrammeDimCallProgrammeDimCallProgramme = new HashSet<BrCallProgrammeDimCallProgramme>();
            BrCallProgrammeDimCallProgrammeDimCallProgrammeId2Navigation = new HashSet<BrCallProgrammeDimCallProgramme>();
            BrDimAuroraHakualatDimCallProgramme = new HashSet<BrDimAuroraHakualatDimCallProgramme>();
            BrOrganizationsFundCallProgrammes = new HashSet<BrOrganizationsFundCallProgrammes>();
            DimFundingDecision = new HashSet<DimFundingDecision>();
            DimWebLink = new HashSet<DimWebLink>();
        }

        public int Id { get; set; }
        public int? DimDateIdDue { get; set; }
        public int? DimDateIdOpen { get; set; }
        public string Abbreviation { get; set; }
        public string EuCallId { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string NameUnd { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionSv { get; set; }
        public string DescriptionEn { get; set; }
        public string ApplicationTermsFi { get; set; }
        public string ApplicationTermsSv { get; set; }
        public string ApplicationTermsEn { get; set; }
        public string ContactInformation { get; set; }
        public bool? ContinuosApplicationPeriod { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimDate DimDateIdDueNavigation { get; set; }
        public virtual DimDate DimDateIdOpenNavigation { get; set; }
        public virtual ICollection<BrCallProgrammeDimCallProgramme> BrCallProgrammeDimCallProgrammeDimCallProgramme { get; set; }
        public virtual ICollection<BrCallProgrammeDimCallProgramme> BrCallProgrammeDimCallProgrammeDimCallProgrammeId2Navigation { get; set; }
        public virtual ICollection<BrDimAuroraHakualatDimCallProgramme> BrDimAuroraHakualatDimCallProgramme { get; set; }
        public virtual ICollection<BrOrganizationsFundCallProgrammes> BrOrganizationsFundCallProgrammes { get; set; }
        public virtual ICollection<DimFundingDecision> DimFundingDecision { get; set; }
        public virtual ICollection<DimWebLink> DimWebLink { get; set; }
    }
}
