using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorGroupEducation : ProfileEditorGroup
    {
        public ProfileEditorGroupEducation()
        {
        }

        public ProfileEditorDataSource dataSource { get; set; }
        public List<ProfileEditorItemEducation> items { get; set; }
    }
}
