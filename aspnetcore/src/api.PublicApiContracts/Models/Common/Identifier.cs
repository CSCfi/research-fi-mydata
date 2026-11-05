using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Persistent identifier (schema: identifier).
    /// </summary>
    public class Identifier
    {
        /// <summary>Persistent identifier value.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PidContent { get; set; }

        /// <summary>Persistent identifier type, for example DOI or URN.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PidType { get; set; }
    }
}
