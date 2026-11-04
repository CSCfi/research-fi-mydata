using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Conference (schema: conference_event).
    /// </summary>
    public class ConferenceEvent
    {
        /// <summary>Publication forum classification of the conference.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PublicationForumChannel IsClassifiedByPublicationForum { get; set; }

        /// <summary>Conference name.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ConferenceName { get; set; }
    }
}
