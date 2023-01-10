using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimKnownPerson
    {
        public DimKnownPerson()
        {
            DimAffiliations = new HashSet<DimAffiliation>();
            DimCompetences = new HashSet<DimCompetence>();
            DimEducations = new HashSet<DimEducation>();
            DimEmailAddrresses = new HashSet<DimEmailAddrress>();
            DimNames = new HashSet<DimName>();
            DimOrcidPublications = new HashSet<DimOrcidPublication>();
            DimPids = new HashSet<DimPid>();
            DimResearcherDescriptions = new HashSet<DimResearcherDescription>();
            DimResearcherToResearchCommunities = new HashSet<DimResearcherToResearchCommunity>();
            DimTelephoneNumbers = new HashSet<DimTelephoneNumber>();
            DimUserProfiles = new HashSet<DimUserProfile>();
            DimWebLinks = new HashSet<DimWebLink>();
            FactDimReferencedataFieldOfSciences = new HashSet<FactDimReferencedataFieldOfScience>();
        }

        public int Id { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public string SourceProjectId { get; set; }
        public int? DimRegisteredDataSourceId { get; set; }

        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual ICollection<DimAffiliation> DimAffiliations { get; set; }
        public virtual ICollection<DimCompetence> DimCompetences { get; set; }
        public virtual ICollection<DimEducation> DimEducations { get; set; }
        public virtual ICollection<DimEmailAddrress> DimEmailAddrresses { get; set; }
        public virtual ICollection<DimName> DimNames { get; set; }
        public virtual ICollection<DimOrcidPublication> DimOrcidPublications { get; set; }
        public virtual ICollection<DimPid> DimPids { get; set; }
        public virtual ICollection<DimResearcherDescription> DimResearcherDescriptions { get; set; }
        public virtual ICollection<DimResearcherToResearchCommunity> DimResearcherToResearchCommunities { get; set; }
        public virtual ICollection<DimTelephoneNumber> DimTelephoneNumbers { get; set; }
        public virtual ICollection<DimUserProfile> DimUserProfiles { get; set; }
        public virtual ICollection<DimWebLink> DimWebLinks { get; set; }
        public virtual ICollection<FactDimReferencedataFieldOfScience> FactDimReferencedataFieldOfSciences { get; set; }
    }
}
