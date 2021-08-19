using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorGroupEmail : ProfileEditorGroup
    {
        public ProfileEditorGroupEmail()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemEmail> items { get; set; }
    }
}
