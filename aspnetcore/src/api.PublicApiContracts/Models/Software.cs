using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Software application (schema: software).
    /// </summary>
    public class Software
    {
        /// <summary>Channel the software was published on.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PublicationChannel PublishedOn { get; set; }

        /// <summary>Publication year.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PublicationYear { get; set; }

        /// <summary>Publication name.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PublicationName { get; set; }

        /// <summary>Identifiers of the software.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Identifier> SoftwareIsIdentifiedBy { get; set; }

        /// <summary>Authors as listed in the original publication.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ListOfAuthors { get; set; }
    }
}
