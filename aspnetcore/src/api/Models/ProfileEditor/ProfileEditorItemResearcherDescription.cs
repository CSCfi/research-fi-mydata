using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorItemResearcherDescription : ProfileEditorItem
    {
        public ProfileEditorItemResearcherDescription()
        {
            ResearchDescriptionFi = "";
            ResearchDescriptionEn = "";
            ResearchDescriptionSv = "";
        }

        public string ResearchDescriptionFi { get; set; }
        public string ResearchDescriptionEn { get; set; }
        public string ResearchDescriptionSv { get; set; }
    }
}
