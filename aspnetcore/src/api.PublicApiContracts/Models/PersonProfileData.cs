using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Person profile data for public API (schema: profile).
    /// </summary>
    public class PersonProfileData
    {
        /// <summary>
        /// Person's identifier, pidType = 'ORCID'.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Identifier PersonIsIdentifiedBy { get; set; }

        /// <summary>
        /// Person's name.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PersonName IsNamedBy { get; set; }

        /// <summary>
        /// Person's contact information.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ContactInformation IsReachableFrom { get; set; }

        /// <summary>
        /// Person's affiliations.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Affiliation> Affiliations { get; set; }

        /// <summary>
        /// Person's completed education.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Education> HasEducations { get; set; }

        /// <summary>
        /// Outputs the person has contributed to.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Output> ContributesToOutput { get; set; }

        /// <summary>
        /// Description of the person's activities.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LanguageVariant DescriptionOfActivities { get; set; }

        /// <summary>
        /// Funding granted to the person.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<GrantedFunding> HasGrantedFunding { get; set; }

        /// <summary>
        /// Collaboration interests expressed by the person.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<ReferenceData> CollaborationInterests { get; set; }
    }
}
