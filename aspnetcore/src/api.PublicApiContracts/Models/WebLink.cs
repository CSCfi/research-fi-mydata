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
        public string LinkLabel { get; set; }

        /// <summary>Link URL.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string LinkURL { get; set; }
    }
}
