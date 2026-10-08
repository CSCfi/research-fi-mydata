using System.Collections.Generic;

namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Publication series (schema: publication_serie).
    /// </summary>
    public class PublicationSerie
    {
        /// <summary>Volume.</summary>
        public string Volume { get; set; }

        /// <summary>Publication forum classification of the series.</summary>
        public PublicationForumChannel IsClassifiedByPublicationForum { get; set; }

        /// <summary>ISSNs.</summary>
        public List<string> ISSN { get; set; }

        /// <summary>Series name.</summary>
        public string SerieName { get; set; }
    }
}
