﻿using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimCallProgramme
    {
        public DimCallProgramme()
        {
            BrCallProgrammeDimCallProgrammeDimCallProgrammeId2Navigations = new HashSet<BrCallProgrammeDimCallProgramme>();
            BrCallProgrammeDimCallProgrammeDimCallProgrammes = new HashSet<BrCallProgrammeDimCallProgramme>();
            BrDimReferencedataDimCallProgrammes = new HashSet<BrDimReferencedataDimCallProgramme>();
            BrOrganizationsFundCallProgrammes = new HashSet<BrOrganizationsFundCallProgramme>();
            DimFundingDecisions = new HashSet<DimFundingDecision>();
            DimWebLinks = new HashSet<DimWebLink>();
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
        public int DimRegisteredDataSourceId { get; set; }

        public virtual DimDate DimDateIdDueNavigation { get; set; }
        public virtual DimDate DimDateIdOpenNavigation { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual ICollection<BrCallProgrammeDimCallProgramme> BrCallProgrammeDimCallProgrammeDimCallProgrammeId2Navigations { get; set; }
        public virtual ICollection<BrCallProgrammeDimCallProgramme> BrCallProgrammeDimCallProgrammeDimCallProgrammes { get; set; }
        public virtual ICollection<BrDimReferencedataDimCallProgramme> BrDimReferencedataDimCallProgrammes { get; set; }
        public virtual ICollection<BrOrganizationsFundCallProgramme> BrOrganizationsFundCallProgrammes { get; set; }
        public virtual ICollection<DimFundingDecision> DimFundingDecisions { get; set; }
        public virtual ICollection<DimWebLink> DimWebLinks { get; set; }
    }
}