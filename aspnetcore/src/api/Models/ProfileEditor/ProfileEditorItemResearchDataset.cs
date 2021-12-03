﻿using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorItemResearchDataset : ProfileEditorItem
    {
        public ProfileEditorItemResearchDataset()
        {
            Actor = new List<ProfileEditorActor>();
            Identifier = "";
            NameFi = "";
            NameSv = "";
            NameEn = "";
            DescriptionFi = "";
            DescriptionSv = "";
            DescriptionEn = "";
            PreferredIdentifiers = new List<ProfileEditorPreferredIdentifier>();
        }

        // Properties are according to ElasticSearch index, not according to model DimResearchDataset
        public List<ProfileEditorActor> Actor { get; set; }
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
