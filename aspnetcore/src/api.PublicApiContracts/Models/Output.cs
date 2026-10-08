using System.Collections.Generic;

namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Outputs the person has contributed to (schema: output).
    /// </summary>
    public class Output
    {
        /// <summary>Datasets contributed to.</summary>
        public List<ResearchDataset> ContributesToDataset { get; set; }

        /// <summary>Applications contributed to.</summary>
        public List<Software> ContributesToApplication { get; set; }

        /// <summary>Audiovisual publications contributed to.</summary>
        public List<AudiovisualPublication> ContributesToAudiovisualPublication { get; set; }

        /// <summary>Written publications contributed to.</summary>
        public List<WrittenPublication> ContributesToWrittenPublication { get; set; }

        /// <summary>Art publications contributed to.</summary>
        public List<ArtPublication> ContributesToArtpublication { get; set; }
    }
}
