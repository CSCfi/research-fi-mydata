namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Publisher (schema: publisher).
    /// </summary>
    public class Publisher
    {
        /// <summary>Publication forum classification of the publisher.</summary>
        public PublicationForumChannel IsClassifiedByPublicationForum { get; set; }

        /// <summary>Publisher name.</summary>
        public string PublisherName { get; set; }
    }
}
