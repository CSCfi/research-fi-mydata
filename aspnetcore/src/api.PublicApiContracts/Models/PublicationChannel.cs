namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Channel a publication is published on (schema: publication_channel).
    /// </summary>
    public class PublicationChannel
    {
        /// <summary>Publisher.</summary>
        public Publisher IsPublishedBy { get; set; }

        /// <summary>Publication series.</summary>
        public SerialPublication IsPublishedInSerie { get; set; }

        /// <summary>Related conference.</summary>
        public ConferenceEvent RelatesToConference { get; set; }

        /// <summary>Platform the publication is available on.</summary>
        public Repository AvailableOnPlatform { get; set; }
    }
}
