using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Web link (schema: weblink).
    /// </summary>
    public class WebLink
    {
        /// <summary>Link description.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string WeblinkLabel { get; set; }

        /// <summary>Link URL.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string WeblinkURL { get; set; }
    }
}
