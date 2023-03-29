using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimCallProgramme
    {
        public DimCallProgramme()
        {
            DimFundingDecisions = new HashSet<DimFundingDecision>();
            DimWebLinks = new HashSet<DimWebLink>();
            DimCallProgrammeId2s = new HashSet<DimCallProgramme>();
            DimCallProgrammes = new HashSet<DimCallProgramme>();
            DimOrganizations = new HashSet<DimOrganization>();
            DimReferencedata = new HashSet<DimReferencedatum>();
        }

        public int Id { get; set; }
        public int DimDateIdDue { get; set; }
        public int DimDateIdOpen { get; set; }
        public TimeSpan? DueDateDueTime { get; set; }
        public string Abbreviation { get; set; }
        public string EuCallId { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string NameUnd { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int? DimCallProgrammeId { get; set; }
        public string SourceProgrammeId { get; set; }
        public int DimRegisteredDataSourceId { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionSv { get; set; }
        public string DescriptionEn { get; set; }
        public string ApplicationTermsFi { get; set; }
        public string ApplicationTermsSv { get; set; }
        public string ApplicationTermsEn { get; set; }
        public string ContactInformation { get; set; }
        public bool? ContinuousApplicationPeriod { get; set; }
        public bool IsOpenCall { get; set; }
        public string CallNameDetailsFi { get; set; }
        public string CallNameDetailsEn { get; set; }
        public string CallNameDetailsSv { get; set; }

        public virtual DimDate DimDateIdDueNavigation { get; set; }
        public virtual DimDate DimDateIdOpenNavigation { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual ICollection<DimFundingDecision> DimFundingDecisions { get; set; }
        public virtual ICollection<DimWebLink> DimWebLinks { get; set; }

        public virtual ICollection<DimCallProgramme> DimCallProgrammeId2s { get; set; }
        public virtual ICollection<DimCallProgramme> DimCallProgrammes { get; set; }
        public virtual ICollection<DimOrganization> DimOrganizations { get; set; }
        public virtual ICollection<DimReferencedatum> DimReferencedata { get; set; }
    }
}