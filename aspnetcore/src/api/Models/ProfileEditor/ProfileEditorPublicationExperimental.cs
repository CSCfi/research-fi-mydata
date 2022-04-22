﻿using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorPublicationExperimental : ProfileEditorItem
    {
        public ProfileEditorPublicationExperimental()
        {
            PublicationId = "";
            PublicationName = "";
            PublicationYear = null;
            Doi = "";
            TypeCode = "";
            DataSources = new List<ProfileEditorSource>();
        }

        public string PublicationId { get; set; }
        public string PublicationName { get; set; }
        public int? PublicationYear { get; set; }
        public string Doi { get; set; }
        public string TypeCode { get; set; }
        public List<ProfileEditorSource> DataSources {get; set; }
    }
}
