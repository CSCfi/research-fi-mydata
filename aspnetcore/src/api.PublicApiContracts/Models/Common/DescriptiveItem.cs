using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Text content in a single language (schema: descriptive_item).
    /// </summary>
    public class DescriptiveItem
    {
        /// <summary>Descriptive text.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string DescriptiveContent { get; set; }

        /// <summary>Language of the text.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Language { get; set; }
    }
}
