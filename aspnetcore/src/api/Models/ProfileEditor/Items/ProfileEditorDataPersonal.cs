using System.Collections.Generic;

namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorDataPersonal
    {
        public ProfileEditorDataPersonal()
        {
            names = new List<ProfileEditorName> ();
            otherNames = new List<ProfileEditorName>();
            emails = new List<ProfileEditorEmail>();
            telephoneNumbers = new List<ProfileEditorTelephoneNumber>();
            webLinks = new List<ProfileEditorWebLink> ();
            keywords = new List<ProfileEditorKeyword>();
            fieldOfSciences = new List<ProfileEditorFieldOfScience>();
            researcherDescriptions = new List<ProfileEditorResearcherDescription>();
            externalIdentifiers = new List<ProfileEditorExternalIdentifier>();
        }

        public List<ProfileEditorName> names { get; set; }
        public List<ProfileEditorName> otherNames { get; set; }
        public List<ProfileEditorEmail> emails { get; set; }
        public List<ProfileEditorTelephoneNumber> telephoneNumbers { get; set; }
        public List<ProfileEditorWebLink> webLinks { get; set; }
        public List<ProfileEditorKeyword> keywords { get; set; }
        public List<ProfileEditorFieldOfScience> fieldOfSciences { get; set; }
        public List<ProfileEditorResearcherDescription> researcherDescriptions { get; set; }
        public List<ProfileEditorExternalIdentifier> externalIdentifiers { get; set; }
    }
}

