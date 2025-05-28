using System.Collections.Generic;

namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorResearchDataset : ProfileEditorItem
    {
        public ProfileEditorResearchDataset()
        {
            AccessType = "";
            Actor = new List<ProfileEditorActor>();
            FairdataUrl = "";
            Identifier = "";
            NameFi = "";
            NameSv = "";
            NameEn = "";
            DescriptionFi = "";
            DescriptionSv = "";
            DescriptionEn = "";
            Url = "";
            DatasetCreated = null;
            PreferredIdentifiers = new List<ProfileEditorPreferredIdentifier>();
            
        }

        // Properties are according to ElasticSearch index, not according to model DimResearchDataset
        public string AccessType { get; set; }
        public List<ProfileEditorActor> Actor { get; set; }
        public string FairdataUrl { get; set; }
        public int? DatasetCreated { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionSv { get; set; }
        public string Identifier { get; set; }
        public string NameEn { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public List<ProfileEditorPreferredIdentifier> PreferredIdentifiers { get; set; }
        public string Url { get; set; }
    }
}
