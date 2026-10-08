namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Parent publication (schema: parent_publication).
    /// </summary>
    public class ParentPublication
    {
        /// <summary>Issue number of the journal the publication appeared in.</summary>
        public string ParentPublicationIssueNumber { get; set; }

        /// <summary>Parent publication name.</summary>
        public string ParentPublicationName { get; set; }

        /// <summary>Channel the parent publication was published on.</summary>
        public PublicationChannel PublishedOn { get; set; }
    }
}
