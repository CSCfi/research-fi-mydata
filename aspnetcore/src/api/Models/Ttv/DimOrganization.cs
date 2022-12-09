using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimOrganization
    {
        public DimOrganization()
        {
            BrFundingConsortiumParticipations = new HashSet<BrFundingConsortiumParticipation>();
            BrParticipatesInFundingGroups = new HashSet<BrParticipatesInFundingGroup>();
            DimAffiliations = new HashSet<DimAffiliation>();
            DimExternalServices = new HashSet<DimExternalService>();
            DimFundingDecisions = new HashSet<DimFundingDecision>();
            DimPids = new HashSet<DimPid>();
            DimPurposes = new HashSet<DimPurpose>();
            DimRegisteredDataSources = new HashSet<DimRegisteredDataSource>();
            DimResearchActivities = new HashSet<DimResearchActivity>();
            DimWebLinks = new HashSet<DimWebLink>();
            FactContributions = new HashSet<FactContribution>();
            FactUpkeeps = new HashSet<FactUpkeep>();
            InverseDimOrganizationBroaderNavigation = new HashSet<DimOrganization>();
            DimCallProgrammes = new HashSet<DimCallProgramme>();
            DimOrganizationid2s = new HashSet<DimOrganization>();
            DimOrganizationid2sNavigation = new HashSet<DimOrganization>();
            DimOrganizations = new HashSet<DimOrganization>();
            DimOrganizationsNavigation = new HashSet<DimOrganization>();
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
        public virtual ICollection<BrFundingConsortiumParticipation> BrFundingConsortiumParticipations { get; set; }
        public virtual ICollection<BrParticipatesInFundingGroup> BrParticipatesInFundingGroups { get; set; }
        public virtual ICollection<DimAffiliation> DimAffiliations { get; set; }
        public virtual ICollection<DimExternalService> DimExternalServices { get; set; }
        public virtual ICollection<DimFundingDecision> DimFundingDecisions { get; set; }
        public virtual ICollection<DimPid> DimPids { get; set; }
        public virtual ICollection<DimPurpose> DimPurposes { get; set; }
        public virtual ICollection<DimRegisteredDataSource> DimRegisteredDataSources { get; set; }
        public virtual ICollection<DimResearchActivity> DimResearchActivities { get; set; }
        public virtual ICollection<DimWebLink> DimWebLinks { get; set; }
        public virtual ICollection<FactContribution> FactContributions { get; set; }
        public virtual ICollection<FactUpkeep> FactUpkeeps { get; set; }
        public virtual ICollection<DimOrganization> InverseDimOrganizationBroaderNavigation { get; set; }

        public virtual ICollection<DimCallProgramme> DimCallProgrammes { get; set; }
        public virtual ICollection<DimOrganization> DimOrganizationid2s { get; set; }
        public virtual ICollection<DimOrganization> DimOrganizationid2sNavigation { get; set; }
        public virtual ICollection<DimOrganization> DimOrganizations { get; set; }
        public virtual ICollection<DimOrganization> DimOrganizationsNavigation { get; set; }
    }
}
