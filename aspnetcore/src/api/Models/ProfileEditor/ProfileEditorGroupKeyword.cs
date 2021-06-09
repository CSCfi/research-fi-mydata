using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorGroupKeyword : ProfileEditorGroup
    {
        public ProfileEditorGroupKeyword()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemKeyword> items { get; set; }
    }
}
