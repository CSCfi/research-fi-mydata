using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Written publication (schema: written_publication).
    /// </summary>
    public class WrittenPublication
    {
        /// <summary>Publication year.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PublicationYear { get; set; }

        /// <summary>Channel the publication was published on.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PublicationChannel PublishedOn { get; set; }

        /// <summary>Identifiers, for example DOI, ISBN, URN or Handle.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Identifier> WrittenPublicationIsIdentifiedBy { get; set; }

        /// <summary>Parent publication.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ParentPublication WrittenPublicationIsPartOfParentPublication { get; set; }

        /// <summary>Publication name.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PublicationName { get; set; }

        /// <summary>Authors as listed in the original publication.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ListOfAuthors { get; set; }
    }
}
