using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Audiovisual publication (schema: audiovisualPublication).
    /// </summary>
    public class AudiovisualPublication
    {
        /// <summary>Identifiers, for example DOI or URN.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Identifier> AVPublicationIsIdentifiedBy { get; set; }

        /// <summary>Channel the publication was published on.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PublicationChannel PublishedOn { get; set; }

        /// <summary>Publication year.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PublicationYear { get; set; }

        /// <summary>Publication name.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PublicationName { get; set; }

        /// <summary>Authors as listed in the original publication.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ListOfAuthors { get; set; }
    }
}
