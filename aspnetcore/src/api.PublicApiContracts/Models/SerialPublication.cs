namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Serial publication, such as a journal or publication series (schema: serial_publication).
    /// </summary>
    public class SerialPublication
    {
        /// <summary>Volume.</summary>
        public string Volume { get; set; }

        /// <summary>Publication forum classification of the serial publication.</summary>
        public PublicationForumChannel IsClassifiedByPublicationForum { get; set; }

        /// <summary>Journal or series name.</summary>
        public string JournalName { get; set; }

        /// <summary>Serial publication identifier, pid_type = 'ISSN'.</summary>
        public Identifier SerialPublicationIsIdentifiedBy { get; set; }
    }
}
