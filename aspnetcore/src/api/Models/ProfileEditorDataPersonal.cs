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
            webLinkGroups = new List<ProfileEditorGroupWebLink> ();
            researcherDescriptionGroups = new List<ProfileEditorGroupResearcherDescription>();
        }

        public List<ProfileEditorGroupFirstNames> firstNamesGroups { get; set; }
        public List<ProfileEditorGroupLastName> lastNameGroups { get; set; }
        public List<ProfileEditorGroupWebLink> webLinkGroups { get; set; }
        public List<ProfileEditorGroupResearcherDescription> researcherDescriptionGroups { get; set; }
    }
}

