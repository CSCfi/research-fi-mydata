using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorGroupPublication : ProfileEditorGroup
    {
        public ProfileEditorGroupPublication()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemPublication> items { get; set; }
    }
}
