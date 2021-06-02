using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorDataActivity    {
        public ProfileEditorDataActivity()
        {
            affiliationGroups = new List<ProfileEditorGroupAffiliation>();
            educationGroups = new List<ProfileEditorGroupEducation>();
            publicationGroups = new List<ProfileEditorGroupPublication>();
        }

        public List<ProfileEditorGroupAffiliation> affiliationGroups { get; set; }
        public List<ProfileEditorGroupEducation> educationGroups { get; set; }
        public List<ProfileEditorGroupPublication> publicationGroups { get; set; }
    }
}
