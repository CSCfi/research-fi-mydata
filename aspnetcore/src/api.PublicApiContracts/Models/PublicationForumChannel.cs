namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Channel classified in the Publication Forum (schema: publication_forum_channel).
    /// </summary>
    public class PublicationForumChannel
    {
        /// <summary>JuFo classification.</summary>
        public ReferenceData JufoClassification { get; set; }

        /// <summary>JuFo-Id.</summary>
        public Identifier IdentifierByPublicationForum { get; set; }
    }
}
