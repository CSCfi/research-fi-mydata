using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimOrganization
    {
        public DimOrganization()
        {
            BrFundingConsortiumParticipation = new HashSet<BrFundingConsortiumParticipation>();
            BrOrganizationsFundCallProgrammes = new HashSet<BrOrganizationsFundCallProgrammes>();
            BrParticipatesInFundingGroup = new HashSet<BrParticipatesInFundingGroup>();
            BrPredecessorOrganizationDimOrganization = new HashSet<BrPredecessorOrganization>();
            BrPredecessorOrganizationDimOrganizationid2Navigation = new HashSet<BrPredecessorOrganization>();
            BrSuccessorOrganizationDimOrganization = new HashSet<BrSuccessorOrganization>();
            BrSuccessorOrganizationDimOrganizationid2Navigation = new HashSet<BrSuccessorOrganization>();
            DimExternalService = new HashSet<DimExternalService>();
            DimFundingDecision = new HashSet<DimFundingDecision>();
            DimPid = new HashSet<DimPid>();
            DimRegisteredDataSource = new HashSet<DimRegisteredDataSource>();
            DimWebLink = new HashSet<DimWebLink>();
            FactContribution = new HashSet<FactContribution>();
            FactUpkeep = new HashSet<FactUpkeep>();
            InverseDimOrganizationBroaderNavigation = new HashSet<DimOrganization>();
        }

        public int Id { get; set; }
        public int? DimOrganizationBroader { get; set; }
        public int DimSectorid { get; set; }
        public string OrganizationId { get; set; }
        public bool? OrganizationActive { get; set; }
        public string LocalOrganizationUnitId { get; set; }
        public string LocalOrganizationSector { get; set; }
        public string OrganizationBackground { get; set; }
        public string OrganizationType { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string NameVariants { get; set; }
        public string NameUnd { get; set; }
        public string CountryCode { get; set; }
        public DateTime? Established { get; set; }
        public string VisitingAddress { get; set; }
        public string PostalAddress { get; set; }
        public int? StaffCountAsFte { get; set; }
        public int? DegreeCountBsc { get; set; }
        public int? DegreeCountMsc { get; set; }
        public int? DegreeCountLic { get; set; }
        public int? DegreeCountPhd { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimOrganization DimOrganizationBroaderNavigation { get; set; }
        public virtual DimSector DimSector { get; set; }
        public virtual ICollection<BrFundingConsortiumParticipation> BrFundingConsortiumParticipation { get; set; }
        public virtual ICollection<BrOrganizationsFundCallProgrammes> BrOrganizationsFundCallProgrammes { get; set; }
        public virtual ICollection<BrParticipatesInFundingGroup> BrParticipatesInFundingGroup { get; set; }
        public virtual ICollection<BrPredecessorOrganization> BrPredecessorOrganizationDimOrganization { get; set; }
        public virtual ICollection<BrPredecessorOrganization> BrPredecessorOrganizationDimOrganizationid2Navigation { get; set; }
        public virtual ICollection<BrSuccessorOrganization> BrSuccessorOrganizationDimOrganization { get; set; }
        public virtual ICollection<BrSuccessorOrganization> BrSuccessorOrganizationDimOrganizationid2Navigation { get; set; }
        public virtual ICollection<DimExternalService> DimExternalService { get; set; }
        public virtual ICollection<DimFundingDecision> DimFundingDecision { get; set; }
        public virtual ICollection<DimPid> DimPid { get; set; }
        public virtual ICollection<DimRegisteredDataSource> DimRegisteredDataSource { get; set; }
        public virtual ICollection<DimWebLink> DimWebLink { get; set; }
        public virtual ICollection<FactContribution> FactContribution { get; set; }
        public virtual ICollection<FactUpkeep> FactUpkeep { get; set; }
        public virtual ICollection<DimOrganization> InverseDimOrganizationBroaderNavigation { get; set; }
    }
}
