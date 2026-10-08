using System.Collections.Generic;

namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Research dataset (schema: research_dataset).
    /// </summary>
    public class ResearchDataset
    {
        /// <summary>Dataset title.</summary>
        public List<DescriptiveItem> DatasetTitle { get; set; }

        /// <summary>Dataset description.</summary>
        public List<DescriptiveItem> DatasetDescription { get; set; }

        /// <summary>Dataset creation date.</summary>
        public PartialDate DatasetCreatedDate { get; set; }

        /// <summary>Person's role in the dataset.</summary>
        public List<ReferenceData> RoleInDataset { get; set; }

        /// <summary>Persistent identifiers, for example DOI, URN or URL.</summary>
        public List<Identifier> DatasetIsIdentifiedBy { get; set; }
    }
}
