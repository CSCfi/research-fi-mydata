using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorDataActivity    {
        public ProfileEditorDataActivity()
        {
            educationGroups = new List<ProfileEditorGroupEducation>();
            publicationGroups = new List<ProfileEditorGroupPublication>();
        }

        public List<ProfileEditorGroupEducation> educationGroups { get; set; }
        public List<ProfileEditorGroupPublication> publicationGroups { get; set; }
    }
}
