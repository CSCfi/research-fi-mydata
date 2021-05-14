using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorDataPersonal
    {
        public ProfileEditorDataPersonal()
        {
            firstNamesGroups = new List<ProfileEditorGroupFirstNames> ();
            lastNameGroups = new List<ProfileEditorGroupLastName>();
            otherNamesGroups = new List<ProfileEditorGroupOtherNames>();
            webLinkGroups = new List<ProfileEditorGroupWebLink> ();
            keywordGroups = new List<ProfileEditorGroupKeyword>();
            researcherDescriptionGroups = new List<ProfileEditorGroupResearcherDescription>();
        }

        public List<ProfileEditorGroupFirstNames> firstNamesGroups { get; set; }
        public List<ProfileEditorGroupLastName> lastNameGroups { get; set; }
        public List<ProfileEditorGroupOtherNames> otherNamesGroups { get; set; }
        public List<ProfileEditorGroupWebLink> webLinkGroups { get; set; }
        public List<ProfileEditorGroupKeyword> keywordGroups { get; set; }
        public List<ProfileEditorGroupResearcherDescription> researcherDescriptionGroups { get; set; }
    }
}

