using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Serial publication, such as a journal or publication series (schema: serial_publication).
    /// </summary>
    public class SerialPublication
    {
        /// <summary>Volume.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Volume { get; set; }

        /// <summary>Publication forum classification of the serial publication.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PublicationForumChannel IsClassifiedByPublicationForum { get; set; }

        /// <summary>Journal or series name.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string JournalName { get; set; }

        /// <summary>Serial publication identifier, pid_type = 'ISSN'.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Identifier SerialPublicationIsIdentifiedBy { get; set; }
    }
}
