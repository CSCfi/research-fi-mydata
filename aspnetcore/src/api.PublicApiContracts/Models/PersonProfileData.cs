using System.Collections.Generic;

namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Person profile data for public API (schema: profile).
    /// </summary>
    public class PersonProfileData
    {
        /// <summary>
        /// Funding granted to the person.
        /// </summary>
        public List<GrantedFunding> HasGrantedFunding { get; set; }

        /// <summary>
        /// Description of the person's activities.
        /// </summary>
        public LanguageVariant DescriptionOfActivities { get; set; }

        /// <summary>
        /// Person's name.
        /// </summary>
        public PersonName IsNamedBy { get; set; }

        /// <summary>
        /// Person's contact information.
        /// </summary>
        public ContactInformation IsReachableFrom { get; set; }

        /// <summary>
        /// Outputs the person has contributed to.
        /// </summary>
        public List<Output> ContributesToOutput { get; set; }

        /// <summary>
        /// Person's affiliations.
        /// </summary>
        public List<Affiliation> Affiliations { get; set; }

        /// <summary>
        /// Person's ORCID identifier.
        /// </summary>
        public string PersonIsIdentifiedByORCID { get; set; }

        /// <summary>
        /// Collaboration interests expressed by the person.
        /// </summary>
        public List<ReferenceData> CollaborationInterests { get; set; }

        /// <summary>
        /// Person's completed education.
        /// </summary>
        public List<Education> HasEducations { get; set; }
    }
}
