using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Art publication (schema: art_publication).
    /// </summary>
    public class ArtPublication
    {
        /// <summary>Identifiers of the art publication.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Identifier> ArtPublicationIsIdentifiedBy { get; set; }

        /// <summary>Publication year.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PublicationYear { get; set; }

        /// <summary>Channel the publication was published on.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PublicationChannel PublishedOn { get; set; }

        /// <summary>Publication name.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PublicationName { get; set; }

        /// <summary>Authors as listed in the original publication.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ListOfAuthors { get; set; }

        /// <summary>Parent publication.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ParentPublication ArtPublicationHasParent { get; set; }
    }
}
