namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Conference (schema: conference_event).
    /// </summary>
    public class ConferenceEvent
    {
        /// <summary>Publication forum classification of the conference.</summary>
        public PublicationForumChannel IsClassifiedByPublicationForum { get; set; }

        /// <summary>Conference name.</summary>
        public string ConferenceName { get; set; }
    }
}
