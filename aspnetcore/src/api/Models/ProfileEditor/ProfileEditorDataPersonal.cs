using System.Collections.Generic;

namespace api.Models.ProfileEditor
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
            fieldOfScienceGroups = new List<ProfileEditorGroupFieldOfScience>();
            researcherDescriptionGroups = new List<ProfileEditorGroupResearcherDescription>();
            externalIdentifierGroups = new List<ProfileEditorGroupExternalIdentifier>();
        }

        public List<ProfileEditorGroupName> nameGroups { get; set; }
        public List<ProfileEditorGroupOtherName> otherNameGroups { get; set; }
        public List<ProfileEditorGroupEmail> emailGroups { get; set; }
        public List<ProfileEditorGroupTelephoneNumber> telephoneNumberGroups { get; set; }
        public List<ProfileEditorGroupWebLink> webLinkGroups { get; set; }
        public List<ProfileEditorGroupKeyword> keywordGroups { get; set; }
        public List<ProfileEditorGroupFieldOfScience> fieldOfScienceGroups { get; set; }
        public List<ProfileEditorGroupResearcherDescription> researcherDescriptionGroups { get; set; }
        public List<ProfileEditorGroupExternalIdentifier> externalIdentifierGroups { get; set; }
    }
}

