using System.Collections.Generic;

namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Journal (schema: journal).
    /// </summary>
    public class Journal
    {
        /// <summary>Volume.</summary>
        public string Volume { get; set; }

        /// <summary>Publication forum classification of the journal.</summary>
        public PublicationForumChannel IsClassifiedByPublicationForum { get; set; }

        /// <summary>Journal name.</summary>
        public string JournalName { get; set; }

        /// <summary>ISSNs.</summary>
        public List<string> ISSN { get; set; }
    }
}
