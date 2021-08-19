using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorGroupExternalIdentifier : ProfileEditorGroup
    {
        public ProfileEditorGroupExternalIdentifier()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemExternalIdentifier> items { get; set; }
    }
}
