using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Research dataset (schema: research_dataset).
    /// </summary>
    public class ResearchDataset
    {
        /// <summary>Dataset title.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<DescriptiveItem> DatasetTitle { get; set; }

        /// <summary>Dataset description.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<DescriptiveItem> DatasetDescription { get; set; }

        /// <summary>Dataset creation date.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PartialDate DatasetCreatedDate { get; set; }

        /// <summary>Person's role in the dataset.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<ReferenceData> RoleInDataset { get; set; }

        /// <summary>Persistent identifiers, for example DOI, URN or URL.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Identifier> DatasetIsIdentifiedBy { get; set; }
    }
}
