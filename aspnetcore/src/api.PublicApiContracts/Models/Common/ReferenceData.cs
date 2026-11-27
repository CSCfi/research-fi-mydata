using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Code list value (schema: reference_data).
    /// </summary>
    public class ReferenceData
    {
        /// <summary>Code label.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LanguageVariant CodeLabel { get; set; }

        /// <summary>Code value.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string CodeValue { get; set; }
    }
}
