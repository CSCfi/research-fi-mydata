using System.Collections.Generic;

namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Written publication (schema: written_publication).
    /// </summary>
    public class WrittenPublication
    {
        /// <summary>Publication year.</summary>
        public int? PublicationYear { get; set; }

        /// <summary>Channel the publication was published on.</summary>
        public PublicationChannel PublishedOn { get; set; }

        /// <summary>Identifiers, for example DOI, ISBN, URN or Handle.</summary>
        public List<Identifier> WrittenPublicationIsIdentifiedBy { get; set; }

        /// <summary>Parent publication.</summary>
        public ParentPublication WrittenPublicationIsPartOfParentPublication { get; set; }

        /// <summary>Publication name.</summary>
        public string PublicationName { get; set; }

        /// <summary>Authors as listed in the original publication.</summary>
        public string ListOfAuthors { get; set; }
    }
}
