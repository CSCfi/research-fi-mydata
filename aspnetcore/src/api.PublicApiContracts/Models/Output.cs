using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Outputs the person has contributed to (schema: output).
    /// </summary>
    public class Output
    {
        /// <summary>Datasets contributed to.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<ResearchDataset> ContributesToDataset { get; set; }

        /// <summary>Applications contributed to.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Software> ContributesToApplication { get; set; }

        /// <summary>Audiovisual publications contributed to.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<AudiovisualPublication> ContributesToAudiovisualPublication { get; set; }

        /// <summary>Written publications contributed to.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<WrittenPublication> ContributesToWrittenPublication { get; set; }

        /// <summary>Art publications contributed to.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<ArtPublication> ContributesToArtpublication { get; set; }
    }
}
