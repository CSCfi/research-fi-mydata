using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorDataPersonal2
    {
        public ProfileEditorDataPersonal2()
        {
            names = new List<ProfileEditorName> ();
            otherNames = new List<ProfileEditorName>();
            emailGroups = new List<ProfileEditorGroupEmail>();
            telephoneNumberGroups = new List<ProfileEditorGroupTelephoneNumber>();
            webLinkGroups = new List<ProfileEditorGroupWebLink> ();
            keywordGroups = new List<ProfileEditorGroupKeyword>();
            fieldOfScienceGroups = new List<ProfileEditorGroupFieldOfScience>();
            researcherDescriptions = new List<ProfileEditorResearcherDescription>();
            externalIdentifierGroups = new List<ProfileEditorGroupExternalIdentifier>();
        }

        public List<ProfileEditorName> names { get; set; }
        public List<ProfileEditorName> otherNames { get; set; }
        public List<ProfileEditorGroupEmail> emailGroups { get; set; }
        public List<ProfileEditorGroupTelephoneNumber> telephoneNumberGroups { get; set; }
        public List<ProfileEditorGroupWebLink> webLinkGroups { get; set; }
        public List<ProfileEditorGroupKeyword> keywordGroups { get; set; }
        public List<ProfileEditorGroupFieldOfScience> fieldOfScienceGroups { get; set; }
        public List<ProfileEditorResearcherDescription> researcherDescriptions { get; set; }
        public List<ProfileEditorGroupExternalIdentifier> externalIdentifierGroups { get; set; }
    }
}

