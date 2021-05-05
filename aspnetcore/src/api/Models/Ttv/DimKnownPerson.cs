using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimKnownPerson
    {
        public DimKnownPerson()
        {
            BrAffiliations = new HashSet<BrAffiliation>();
            BrResearcherToResearchCommunities = new HashSet<BrResearcherToResearchCommunity>();
            DimCompetences = new HashSet<DimCompetence>();
            DimEducations = new HashSet<DimEducation>();
            DimEmailAddrresses = new HashSet<DimEmailAddrress>();
            DimKnownPersonDimFieldOfSciences = new HashSet<DimKnownPersonDimFieldOfScience>();
            DimNameDimKnownPersonIdConfirmedIdentityNavigations = new HashSet<DimName>();
            DimNameDimKnownPersonIdOtherNamesNavigations = new HashSet<DimName>();
            DimOrcidPublications = new HashSet<DimOrcidPublication>();
            DimPids = new HashSet<DimPid>();
            DimResearcherDescriptions = new HashSet<DimResearcherDescription>();
            DimTelephoneNumbers = new HashSet<DimTelephoneNumber>();
            DimUserProfiles = new HashSet<DimUserProfile>();
            DimWebLinks = new HashSet<DimWebLink>();
        }

        public int Id { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public string SourceProjectId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ICollection<BrAffiliation> BrAffiliations { get; set; }
        public virtual ICollection<BrResearcherToResearchCommunity> BrResearcherToResearchCommunities { get; set; }
        public virtual ICollection<DimCompetence> DimCompetences { get; set; }
        public virtual ICollection<DimEducation> DimEducations { get; set; }
        public virtual ICollection<DimEmailAddrress> DimEmailAddrresses { get; set; }
        public virtual ICollection<DimKnownPersonDimFieldOfScience> DimKnownPersonDimFieldOfSciences { get; set; }
        public virtual ICollection<DimName> DimNameDimKnownPersonIdConfirmedIdentityNavigations { get; set; }
        public virtual ICollection<DimName> DimNameDimKnownPersonIdOtherNamesNavigations { get; set; }
        public virtual ICollection<DimOrcidPublication> DimOrcidPublications { get; set; }
        public virtual ICollection<DimPid> DimPids { get; set; }
        public virtual ICollection<DimResearcherDescription> DimResearcherDescriptions { get; set; }
        public virtual ICollection<DimTelephoneNumber> DimTelephoneNumbers { get; set; }
        public virtual ICollection<DimUserProfile> DimUserProfiles { get; set; }
        public virtual ICollection<DimWebLink> DimWebLinks { get; set; }
    }
}
