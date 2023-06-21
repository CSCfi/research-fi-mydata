using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimProfileOnlyFundingDecision
    {
        public DimProfileOnlyFundingDecision()
        {
            DimPids = new HashSet<DimPid>();
            DimWebLinks = new HashSet<DimWebLink>();
            FactFieldValues = new HashSet<FactFieldValue>();
        }

        public int Id { get; set; }
        public int DimDateIdApproval { get; set; }
        public int DimDateIdStart { get; set; }
        public int DimDateIdEnd { get; set; }
        public int DimCallProgrammeId { get; set; }
        public int DimTypeOfFundingId { get; set; }
        public int? DimOrganizationIdFunder { get; set; }
        public string OrcidWorkType { get; set; }
        public string FunderProjectNumber { get; set; }
        public string Acronym { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string NameUnd { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionSv { get; set; }
        public decimal AmountInEur { get; set; }
        public decimal? AmountInFundingDecisionCurrency { get; set; }
        public string FundingDecisionCurrencyAbbreviation { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int DimRegisteredDataSourceId { get; set; }

        public virtual DimCallProgramme DimCallProgramme { get; set; }
        public virtual DimDate DimDateIdApprovalNavigation { get; set; }
        public virtual DimDate DimDateIdEndNavigation { get; set; }
        public virtual DimDate DimDateIdStartNavigation { get; set; }
        public virtual DimOrganization DimOrganizationIdFunderNavigation { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual DimTypeOfFunding DimTypeOfFunding { get; set; }
        public virtual ICollection<DimPid> DimPids { get; set; }
        public virtual ICollection<DimWebLink> DimWebLinks { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
    }
}
