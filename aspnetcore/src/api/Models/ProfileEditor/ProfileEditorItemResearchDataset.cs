using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorItemResearchDataset : ProfileEditorItem
    {
        public ProfileEditorItemResearchDataset()
        {
            Identifier = "";
            NameFi = "";
            NameSv = "";
            NameEn = "";
            DescriptionFi = "";
            DescriptionSv = "";
            DescriptionEn = "";
        }

        // Properties are according to ElasticSearch index, not according to model DimResearchDataset
        public string Identifier { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionSv { get; set; }
        public string DescriptionEn { get; set; }
        public List<ProfileEditorPreferredIdentifier> PreferredIdentifiers { get; set; }
    }
}
