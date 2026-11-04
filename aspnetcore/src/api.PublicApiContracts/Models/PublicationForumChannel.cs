using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Channel classified in the Publication Forum (schema: publication_forum_channel).
    /// </summary>
    public class PublicationForumChannel
    {
        /// <summary>JuFo classification.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ReferenceData JufoClassification { get; set; }

        /// <summary>JuFo-Id.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Identifier IdentifierByPublicationForum { get; set; }
    }
}
