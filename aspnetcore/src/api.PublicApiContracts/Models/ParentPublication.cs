using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Parent publication (schema: parent_publication).
    /// </summary>
    public class ParentPublication
    {
        /// <summary>Issue number of the journal the publication appeared in.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ParentPublicationIssueNumber { get; set; }

        /// <summary>Parent publication name.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ParentPublicationName { get; set; }

        /// <summary>Channel the parent publication was published on.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PublicationChannel PublishedOn { get; set; }
    }
}
