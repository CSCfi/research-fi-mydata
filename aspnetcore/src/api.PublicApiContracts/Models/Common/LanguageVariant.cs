using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Text in Finnish, Swedish and English (schema: language_variant).
    /// </summary>
    public class LanguageVariant
    {
        /// <summary>Finnish.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Fi { get; set; }

        /// <summary>Swedish.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Sv { get; set; }

        /// <summary>English.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string En { get; set; }
    }
}
