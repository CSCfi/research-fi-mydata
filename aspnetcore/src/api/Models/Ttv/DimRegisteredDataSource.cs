using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimRegisteredDataSource
    {
        public DimRegisteredDataSource()
        {
            BrAffiliations = new HashSet<BrAffiliation>();
            BrFieldDisplaySettingsDimRegisteredDataSources = new HashSet<BrFieldDisplaySettingsDimRegisteredDataSource>();
            BrResearcherToResearchCommunities = new HashSet<BrResearcherToResearchCommunity>();
            DimCallProgrammes = new HashSet<DimCallProgramme>();
            DimCompetences = new HashSet<DimCompetence>();
            DimEducations = new HashSet<DimEducation>();
            DimEmailAddrresses = new HashSet<DimEmailAddrress>();
            DimEvents = new HashSet<DimEvent>();
            DimFundingDecisions = new HashSet<DimFundingDecision>();
            DimKeywords = new HashSet<DimKeyword>();
            DimNames = new HashSet<DimName>();
            DimOrcidPublications = new HashSet<DimOrcidPublication>();
            DimOrganizations = new HashSet<DimOrganization>();
            DimPublications = new HashSet<DimPublication>();
            DimResearchActivities = new HashSet<DimResearchActivity>();
            DimResearchCommunities = new HashSet<DimResearchCommunity>();
            DimResearchDatasets = new HashSet<DimResearchDataset>();
            DimResearcherDescriptions = new HashSet<DimResearcherDescription>();
            DimTelephoneNumbers = new HashSet<DimTelephoneNumber>();
        }

        public int Id { get; set; }
        public int DimOrganizationId { get; set; }
        public string Name { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Created { get; set; }

        public virtual DimOrganization DimOrganization { get; set; }
        public virtual ICollection<BrAffiliation> BrAffiliations { get; set; }
        public virtual ICollection<BrFieldDisplaySettingsDimRegisteredDataSource> BrFieldDisplaySettingsDimRegisteredDataSources { get; set; }
        public virtual ICollection<BrResearcherToResearchCommunity> BrResearcherToResearchCommunities { get; set; }
        public virtual ICollection<DimCallProgramme> DimCallProgrammes { get; set; }
        public virtual ICollection<DimCompetence> DimCompetences { get; set; }
        public virtual ICollection<DimEducation> DimEducations { get; set; }
        public virtual ICollection<DimEmailAddrress> DimEmailAddrresses { get; set; }
        public virtual ICollection<DimEvent> DimEvents { get; set; }
        public virtual ICollection<DimFundingDecision> DimFundingDecisions { get; set; }
        public virtual ICollection<DimKeyword> DimKeywords { get; set; }
        public virtual ICollection<DimName> DimNames { get; set; }
        public virtual ICollection<DimOrcidPublication> DimOrcidPublications { get; set; }
        public virtual ICollection<DimOrganization> DimOrganizations { get; set; }
        public virtual ICollection<DimPublication> DimPublications { get; set; }
        public virtual ICollection<DimResearchActivity> DimResearchActivities { get; set; }
        public virtual ICollection<DimResearchCommunity> DimResearchCommunities { get; set; }
        public virtual ICollection<DimResearchDataset> DimResearchDatasets { get; set; }
        public virtual ICollection<DimResearcherDescription> DimResearcherDescriptions { get; set; }
        public virtual ICollection<DimTelephoneNumber> DimTelephoneNumbers { get; set; }
    }
}
