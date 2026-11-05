using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Channel a publication is published on (schema: publication_channel).
    /// </summary>
    public class PublicationChannel
    {
        /// <summary>Publisher.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Publisher IsPublishedBy { get; set; }

        /// <summary>Publication series.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SerialPublication IsPublishedInSerie { get; set; }

        /// <summary>Related conference.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ConferenceEvent RelatesToConference { get; set; }

        /// <summary>Platform the publication is available on.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Repository AvailableOnPlatform { get; set; }
    }
}
