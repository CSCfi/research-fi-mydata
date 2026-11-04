using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Person's affiliation to an organization (schema: affiliation).
    /// </summary>
    public class Affiliation
    {
        /// <summary>Affiliation start date.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PartialDate AffiliationStartsOn { get; set; }

        /// <summary>Affiliation end date.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PartialDate AffiliationEndsOn { get; set; }

        /// <summary>Affiliation type.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LanguageVariant AffiliationType { get; set; }

        /// <summary>Organization the affiliation refers to.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Organization AffiliationOrganization { get; set; }

        /// <summary>Position title.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LanguageVariant Position { get; set; }
    }
}
