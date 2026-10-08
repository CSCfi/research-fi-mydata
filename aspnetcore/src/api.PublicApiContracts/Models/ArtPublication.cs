using System.Collections.Generic;

namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Art publication (schema: art_publication).
    /// </summary>
    public class ArtPublication
    {
        /// <summary>Identifiers of the art publication.</summary>
        public List<Identifier> ArtPublicationIsIdentifiedBy { get; set; }

        /// <summary>Publication year.</summary>
        public int? PublicationYear { get; set; }

        /// <summary>Channel the publication was published on.</summary>
        public PublicationChannel PublishedOn { get; set; }

        /// <summary>Publication name.</summary>
        public string PublicationName { get; set; }

        /// <summary>Authors as listed in the original publication.</summary>
        public string ListOfAuthors { get; set; }

        /// <summary>Parent publication.</summary>
        public ParentPublication ArtPublicationHasParent { get; set; }
    }
}
