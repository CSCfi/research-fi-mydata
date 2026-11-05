using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Publisher (schema: publisher).
    /// </summary>
    public class Publisher
    {
        /// <summary>Publication forum classification of the publisher.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PublicationForumChannel IsClassifiedByPublicationForum { get; set; }

        /// <summary>Publisher name.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PublisherName { get; set; }
    }
}
