using System.Collections.Generic;

namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Software application (schema: software).
    /// </summary>
    public class Software
    {
        /// <summary>Channel the software was published on.</summary>
        public PublicationChannel PublishedOn { get; set; }

        /// <summary>Publication year.</summary>
        public int? PublicationYear { get; set; }

        /// <summary>Publication name.</summary>
        public string PublicationName { get; set; }

        /// <summary>Identifiers of the software.</summary>
        public List<Identifier> SoftwareIsIdentifiedBy { get; set; }

        /// <summary>Authors as listed in the original publication.</summary>
        public string ListOfAuthors { get; set; }
    }
}
