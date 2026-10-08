using System.Collections.Generic;

namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Audiovisual publication (schema: audiovisualPublication).
    /// </summary>
    public class AudiovisualPublication
    {
        /// <summary>Identifiers, for example DOI or URN.</summary>
        public List<Identifier> AVPublicationIsIdentifiedBy { get; set; }

        /// <summary>Channel the publication was published on.</summary>
        public PublicationChannel PublishedOn { get; set; }

        /// <summary>Publication year.</summary>
        public int? PublicationYear { get; set; }

        /// <summary>Publication name.</summary>
        public string PublicationName { get; set; }

        /// <summary>Authors as listed in the original publication.</summary>
        public string ListOfAuthors { get; set; }
    }
}
