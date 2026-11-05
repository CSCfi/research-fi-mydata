using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Funding granted to the person (schema: granted_funding).
    /// </summary>
    public class GrantedFunding
    {
        /// <summary>Funding start date.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PartialDate FundingStartsOn { get; set; }

        /// <summary>Funder.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Organization IsFundedBy { get; set; }

        /// <summary>Funding end date.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PartialDate FundingEndsOn { get; set; }

        /// <summary>Funding name.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LanguageVariant GrantedFundingName { get; set; }
    }
}
