using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorDataPersonal
    {
        public ProfileEditorDataPersonal()
        {
            nameGroups = new List<ProfileEditorGroupName> ();
            otherNameGroups = new List<ProfileEditorGroupOtherName>();
            emailGroups = new List<ProfileEditorGroupEmail>();
            telephoneNumberGroups = new List<ProfileEditorGroupTelephoneNumber>();
            webLinkGroups = new List<ProfileEditorGroupWebLink> ();
            keywordGroups = new List<ProfileEditorGroupKeyword>();
            researcherDescriptionGroups = new List<ProfileEditorGroupResearcherDescription>();
        }

        public List<ProfileEditorGroupName> nameGroups { get; set; }
        public List<ProfileEditorGroupOtherName> otherNameGroups { get; set; }
        public List<ProfileEditorGroupEmail> emailGroups { get; set; }
        public List<ProfileEditorGroupTelephoneNumber> telephoneNumberGroups { get; set; }
        public List<ProfileEditorGroupWebLink> webLinkGroups { get; set; }
        public List<ProfileEditorGroupKeyword> keywordGroups { get; set; }
        public List<ProfileEditorGroupResearcherDescription> researcherDescriptionGroups { get; set; }
    }
}

