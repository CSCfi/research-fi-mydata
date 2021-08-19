using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorGroupFieldOfScience : ProfileEditorGroup
    {
        public ProfileEditorGroupFieldOfScience()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemFieldOfScience> items { get; set; }
    }
}
