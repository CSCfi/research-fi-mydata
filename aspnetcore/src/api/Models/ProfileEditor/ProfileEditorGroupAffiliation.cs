using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorGroupAffiliation : ProfileEditorGroup
    {
        public ProfileEditorGroupAffiliation()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemAffiliation> items { get; set; }
    }
}
