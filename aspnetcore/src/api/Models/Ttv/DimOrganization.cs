using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimOrganization
    {
        public DimOrganization()
        {
            BrAffiliations = new HashSet<BrAffiliation>();
            BrFundingConsortiumParticipations = new HashSet<BrFundingConsortiumParticipation>();
            BrOrganizationsFundCallProgrammes = new HashSet<BrOrganizationsFundCallProgramme>();
            BrParticipatesInFundingGroups = new HashSet<BrParticipatesInFundingGroup>();
            BrPredecessorOrganizationDimOrganizationid2Navigations = new HashSet<BrPredecessorOrganization>();
            BrPredecessorOrganizationDimOrganizations = new HashSet<BrPredecessorOrganization>();
            BrSuccessorOrganizationDimOrganizationid2Navigations = new HashSet<BrSuccessorOrganization>();
            BrSuccessorOrganizationDimOrganizations = new HashSet<BrSuccessorOrganization>();
            DimExternalServices = new HashSet<DimExternalService>();
            DimFundingDecisions = new HashSet<DimFundingDecision>();
            DimPids = new HashSet<DimPid>();
            DimRegisteredDataSources = new HashSet<DimRegisteredDataSource>();
            DimResearchActivities = new HashSet<DimResearchActivity>();
            DimWebLinks = new HashSet<DimWebLink>();
            FactContributions = new HashSet<FactContribution>();
            FactUpkeeps = new HashSet<FactUpkeep>();
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
        public int DimRegisteredDataSourceId { get; set; }

        public virtual DimOrganization DimOrganizationBroaderNavigation { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual DimSector DimSector { get; set; }
        public virtual ICollection<BrAffiliation> BrAffiliations { get; set; }
        public virtual ICollection<BrFundingConsortiumParticipation> BrFundingConsortiumParticipations { get; set; }
        public virtual ICollection<BrOrganizationsFundCallProgramme> BrOrganizationsFundCallProgrammes { get; set; }
        public virtual ICollection<BrParticipatesInFundingGroup> BrParticipatesInFundingGroups { get; set; }
        public virtual ICollection<BrPredecessorOrganization> BrPredecessorOrganizationDimOrganizationid2Navigations { get; set; }
        public virtual ICollection<BrPredecessorOrganization> BrPredecessorOrganizationDimOrganizations { get; set; }
        public virtual ICollection<BrSuccessorOrganization> BrSuccessorOrganizationDimOrganizationid2Navigations { get; set; }
        public virtual ICollection<BrSuccessorOrganization> BrSuccessorOrganizationDimOrganizations { get; set; }
        public virtual ICollection<DimExternalService> DimExternalServices { get; set; }
        public virtual ICollection<DimFundingDecision> DimFundingDecisions { get; set; }
        public virtual ICollection<DimPid> DimPids { get; set; }
        public virtual ICollection<DimRegisteredDataSource> DimRegisteredDataSources { get; set; }
        public virtual ICollection<DimResearchActivity> DimResearchActivities { get; set; }
        public virtual ICollection<DimWebLink> DimWebLinks { get; set; }
        public virtual ICollection<FactContribution> FactContributions { get; set; }
        public virtual ICollection<FactUpkeep> FactUpkeeps { get; set; }
        public virtual ICollection<DimOrganization> InverseDimOrganizationBroaderNavigation { get; set; }
    }
}
